using UnityEngine;

public partial class FarmPlotView
{
    [SerializeField] private bool _mapMode;
    [SerializeField] private string _mapPlotName="Ô đất";
    public void ConfigureMapMode(string plotName) { _mapMode=true;_mapPlotName=plotName; }
    private GUIStyle _seedCountStyle;
    private void DrawMapInterface()
    {
        if(VuFarmMapView.IsMenuOpen)return;
        EnsureInterfaceStyles();
        if(_seedCountStyle==null)
        {
            _seedCountStyle=new GUIStyle(_small){fontSize=15,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter};
            _seedCountStyle.normal.textColor=Color.white;
        }
        Matrix4x4 old=GUI.matrix;
        int depth=GUI.depth;
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/721f);
        GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1280*scale)/2,(Screen.height-721*scale)/2,0),Quaternion.identity,new Vector3(scale,scale,1));
        GUI.depth=-10;
        try
        {
            bool selected=VuFarmMapView.Selected==this;
            var cam=Camera.main;
            if(cam!=null)
            {
                var low=cam.WorldToScreenPoint(_spriteRenderer.bounds.min);
                var high=cam.WorldToScreenPoint(_spriteRenderer.bounds.max);
                Vector3 origin=GUI.matrix.MultiplyPoint(Vector3.zero);
                var hit=new Rect((low.x-origin.x)/scale,(Screen.height-high.y-origin.y)/scale,(high.x-low.x)/scale,(high.y-low.y)/scale);
                if(GUI.Button(hit,GUIContent.none,GUIStyle.none)) VuFarmMapView.Selected=this;
                // Two-tone edging separates adjacent cells even before selection.
                Fill(new Rect(hit.x,hit.y,hit.width,1),Hex(0xBFA36A));
                Fill(new Rect(hit.x,hit.yMax-1,hit.width,1),Hex(0x65452E));
                Fill(new Rect(hit.x,hit.y,1,hit.height),Hex(0xBFA36A));
                Fill(new Rect(hit.xMax-1,hit.y,1,hit.height),Hex(0x65452E));
                Crop thirsty=_plot.CurrentCrop;
                if(thirsty!=null && (thirsty.State==CropState.Stage1 || thirsty.State==CropState.Stage2) && thirsty.WateringCount==0)
                {
                    float bob=Mathf.Sin(Time.unscaledTime*3+transform.position.x)*3;
                    GUI.DrawTexture(new Rect(hit.center.x-10,hit.y-25+bob,20,25),PlantToolIcons.Get(PlantToolIcons.Kind.Drop));
                }
                if(selected || (VuFarmMapView.IsDragging && hit.Contains(Event.current.mousePosition)))
                {
                    Fill(new Rect(hit.x,hit.y,hit.width,3),Hex(0xFFD974));
                    Fill(new Rect(hit.x,hit.yMax-3,hit.width,3),Hex(0xFFD974));
                    Fill(new Rect(hit.x,hit.y,3,hit.height),Hex(0xFFD974));
                    Fill(new Rect(hit.xMax-3,hit.y,3,hit.height),Hex(0xFFD974));
                }
            }
            if(VuFarmMapView.Selected!=this || VuFarmMapView.IsDragging) return;
            Card(20,543,1240,166);
            Label(36,550,1100,26,_mapPlotName+"  /  "+SoilName()+"  ·  "+GameTimeHint());
            if(ActionButton(new Rect(1192,550,45,26),"×",true)) VuFarmMapView.Selected=null;
            for(int i=0;i<_definitions.Length;i++)
            {
                Rect card=new Rect(36+i*96,584,90,76);
                bool seedAvailable=_inventory.GetQuantity(_definitions[i].SeedItemId)>0;
                if(Event.current.type==EventType.MouseDown && Event.current.button==0 && card.Contains(Event.current.mousePosition) && seedAvailable)
                {
                    _selectedCrop=i;
                    VuFarmMapView.BeginDrag(PlantToolIcons.Kind.Seed,i);
                }
                ActionButton(card,"",seedAvailable,i==_selectedCrop);
                PlantItemSprites.DrawSeed(new Rect(card.x+20,card.y+1,50,44),i);
                Fill(new Rect(card.xMax-34,card.y+3,31,21),Hex(0x365F45));
                GUI.Label(new Rect(card.xMax-34,card.y+3,31,21),_inventory.GetQuantity(_definitions[i].SeedItemId).ToString(),_seedCountStyle);
                Color nameColor=_center.normal.textColor;
                if(i==_selectedCrop)_center.normal.textColor=Color.white;
                Label(card.x,card.y+44,card.width,25,_definitions[i].Name,_center);
                _center.normal.textColor=nameColor;
            }
            Crop c=_plot.CurrentCrop;
            if(ToolButton(new Rect(818,584,100,76),"Xới",PlantToolIcons.Kind.Hoe,_plot.CanTill())) Run(_service.Till(_plot));
            if(ToolButton(new Rect(926,584,100,76),"Gieo",PlantToolIcons.Kind.Seed,_plot.CanPlant())) Run(_service.Plant(_plot,_definitions[_selectedCrop]));
            if(ToolButton(new Rect(1034,584,100,76),"Tưới",PlantToolIcons.Kind.Water,c!=null && (c.State==CropState.Stage1 || c.State==CropState.Stage2))) Run(_service.Water(_plot));
            if(ToolButton(new Rect(1142,584,100,76),"Thu hoạch",PlantToolIcons.Kind.Basket,c!=null && c.CanHarvest())) Run(_service.Harvest(_plot));
            bool fertilize=c!=null && c.CanFertilize();
            if(ToolButton(new Rect(818,669,208,30),"Phân năng suất",PlantToolIcons.Kind.Yield,fertilize,true)) Run(_service.Fertilize(_plot,"fertilizer.yield"));
            if(ToolButton(new Rect(1034,669,208,30),"Phân chín nhanh",PlantToolIcons.Kind.Growth,fertilize,true)) Run(_service.Fertilize(_plot,"fertilizer.growth"));
            var def=_definitions[_selectedCrop];
            Label(36,670,772,28,_message,_small);
        }
        finally {GUI.matrix=old;GUI.depth=depth;}
    }
    private bool ToolButton(Rect rect,string label,PlantToolIcons.Kind icon,bool allowed,bool compact=false)
    {
        // Availability is checked on the receiving tile, not the tile whose toolbar is open.
        allowed=icon!=PlantToolIcons.Kind.Seed || _inventory.GetQuantity(_definitions[_selectedCrop].SeedItemId)>0;
        if(icon==PlantToolIcons.Kind.Yield) allowed=_inventory.GetQuantity("fertilizer.yield")>0;
        if(icon==PlantToolIcons.Kind.Growth) allowed=_inventory.GetQuantity("fertilizer.growth")>0;
        if(allowed && Event.current.type==EventType.MouseDown && Event.current.button==0 && rect.Contains(Event.current.mousePosition))
            VuFarmMapView.BeginDrag(icon,_selectedCrop);
        ActionButton(rect,"",allowed);
        Color previous=GUI.color;
        if(!allowed) GUI.color=new Color(1,1,1,0.35f);
        float size=compact ? 26 : 40;
        GUI.DrawTexture(new Rect(compact ? rect.x+5 : rect.center.x-size/2,rect.y+3,size,size),PlantToolIcons.Get(icon));
        GUI.color=previous;
        Label(compact ? rect.x+35 : rect.x,compact ? rect.y : rect.y+44,compact ? rect.width-38 : rect.width,28,label,_center);
        return false;
    }

    private string GameTimeHint()
    {
        if(_plot.Status==FarmPlotStatus.Tilling)return "Còn "+GameClock.FormatDuration(_plot.TillingTimeRemaining);
        Crop crop=_plot.CurrentCrop;
        if(crop==null)return "";
        if(crop.State==CropState.Ripe)return "Thối sau "+GameClock.FormatDuration(System.Math.Max(0,crop.Definition.RotThreshold-crop.RipeElapsedTime));
        if(crop.WateringCount==0)
        {
            float threshold=crop.HasDroughtPenalty ? crop.Definition.DroughtDeathThreshold : crop.Definition.DroughtPenaltyThreshold;
            return (crop.HasDroughtPenalty ? "Chết sau " : "Giảm năng suất sau ")+GameClock.FormatDuration(System.Math.Max(0,threshold-crop.WaterDeprivationElapsedTime));
        }
        return "Còn "+GameClock.FormatDuration(crop.GrowthTimeRemaining)+" đến giai đoạn tiếp theo";
    }
    internal bool MapContains(Vector2 mouse,Matrix4x4 matrix,float scale,Camera camera)
    {
        if(!_mapMode || !isActiveAndEnabled || _spriteRenderer==null) return false;
        Vector3 low=camera.WorldToScreenPoint(_spriteRenderer.bounds.min);
        Vector3 high=camera.WorldToScreenPoint(_spriteRenderer.bounds.max);
        Vector3 origin=matrix.MultiplyPoint(Vector3.zero);
        return new Rect((low.x-origin.x)/scale,(Screen.height-high.y-origin.y)/scale,
            (high.x-low.x)/scale,(high.y-low.y)/scale).Contains(mouse);
    }
    internal bool CanAcceptMapDrop(PlantToolIcons.Kind tool)
    {
        if(_service==null) return false;
        Crop crop=_plot.CurrentCrop;
        switch(tool)
        {
            case PlantToolIcons.Kind.Hoe: return _plot.CanTill();
            case PlantToolIcons.Kind.Seed: return _plot.CanPlant();
            case PlantToolIcons.Kind.Water: return crop!=null && (crop.State==CropState.Stage1 || crop.State==CropState.Stage2);
            case PlantToolIcons.Kind.Basket: return crop!=null && crop.CanHarvest();
            case PlantToolIcons.Kind.Yield:
            case PlantToolIcons.Kind.Growth: return crop!=null && crop.CanFertilize();
            default: return false;
        }
    }
    internal void ApplyMapDrop(PlantToolIcons.Kind tool,int seed)
    {
        if(!CanAcceptMapDrop(tool)) { _message="Ô đất chưa phù hợp với thao tác này.";return; }
        switch(tool)
        {
            case PlantToolIcons.Kind.Hoe: Run(_service.Till(_plot));break;
            case PlantToolIcons.Kind.Seed:
                _selectedCrop=Mathf.Clamp(seed,0,_definitions.Length-1);
                Run(_service.Plant(_plot,_definitions[_selectedCrop]));break;
            case PlantToolIcons.Kind.Water: Run(_service.Water(_plot));break;
            case PlantToolIcons.Kind.Basket: Run(_service.Harvest(_plot));break;
            case PlantToolIcons.Kind.Yield: Run(_service.Fertilize(_plot,"fertilizer.yield"));break;
            case PlantToolIcons.Kind.Growth: Run(_service.Fertilize(_plot,"fertilizer.growth"));break;
        }
    }

}
