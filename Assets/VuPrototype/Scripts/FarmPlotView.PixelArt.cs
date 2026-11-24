using UnityEngine;

public partial class FarmPlotView
{
    private int _lastArtCrop;
    private SpriteRenderer _pixelCropRenderer;

    private int SoilArtIndex()
    {
        if(_plot.Status==FarmPlotStatus.Untilled) return 0;
        if(_plot.Status==FarmPlotStatus.Tilling) return 1;
        if(_plot.Status==FarmPlotStatus.HaveResidue && _plot.ResidueType==ResidueType.RottenCrop) return 5;
        return _plot.CurrentCrop!=null && _plot.CurrentCrop.WateringCount>0 ? 3 : 2;
    }
    private int CropArtIndex()
    {
        Crop crop=_plot.CurrentCrop;
        if(crop!=null)
        {
            int index=System.Array.FindIndex(_definitions,d=>d.Id==crop.Definition.Id);
            if(index>=0) _lastArtCrop=index;
            return crop.State==CropState.Stage1 ? 0 : crop.State==CropState.Stage2 ? 1 : crop.State==CropState.Ripe ? 2 : 3;
        }
        if(_plot.Status==FarmPlotStatus.HaveResidue)
            return _plot.ResidueType==ResidueType.RottenCrop ? 3 : 4;
        return -1;
    }
    private void RefreshPixelSprites()
    {
        if(!PlantPixelSprites.IsReady) return;
        int plant=CropArtIndex();
        _spriteRenderer.sprite=PlantPixelSprites.GetSprite(0,SoilArtIndex());
        _spriteRenderer.color=Color.white;
        if(_pixelCropRenderer==null)
        {
            var child=new GameObject("CropPixelArt");
            child.transform.SetParent(transform,false);
            _pixelCropRenderer=child.AddComponent<SpriteRenderer>();
            _pixelCropRenderer.sortingLayerID=_spriteRenderer.sortingLayerID;
            _pixelCropRenderer.sortingOrder=_spriteRenderer.sortingOrder+1;
        }
        if(_mapMode)
        {
            Vector3 size=transform.localScale;
            float unit=Mathf.Min(size.x,size.y);
            _pixelCropRenderer.transform.localScale=new Vector3(unit/size.x,unit/size.y,1);
        }
        _pixelCropRenderer.enabled=plant>=0;
        if(plant>=0) _pixelCropRenderer.sprite=PlantPixelSprites.GetSprite(_lastArtCrop+1,plant);
    }
    private void DrawPixelGarden()
    {
        Rect soilRect=new Rect(514,340,188,188);
        if(GUI.Button(soilRect,GUIContent.none,GUIStyle.none))
        {
            if(_plot.CanTill()) Run(_service.Till(_plot));
            else if(_plot.CanPlant()) Run(_service.Plant(_plot,_definitions[_selectedCrop]));
            else if(_plot.CurrentCrop!=null && _plot.CurrentCrop.CanHarvest()) Run(_service.Harvest(_plot));
            else Run(_service.Water(_plot));
        }
        if(!PlantPixelSprites.IsReady)
        {
            Fill(soilRect,Hex(0x8E6143));
            return;
        }
        PlantPixelSprites.Draw(soilRect,0,SoilArtIndex());
        int plant=CropArtIndex();
        if(plant>=0)
        {
            Rect frame=PlantPixelSprites.GetRect(_lastArtCrop+1,plant);
            // Constant pixel scale keeps seedlings smaller than mature plants.
            float pixelScale=1.3f;
            PlantPixelSprites.Draw(new Rect(608-frame.width*pixelScale/2,444-frame.height*pixelScale,
                frame.width*pixelScale,frame.height*pixelScale),_lastArtCrop+1,plant);
        }
    }
}

internal static class PlantPixelSprites
{
    private static Texture2D _atlas;
    private static bool _loadAttempted;
    private static Sprite[,] _sprites=new Sprite[9,6];
    // Source rectangles use top-left image coordinates; all image pixels are unmodified.
    private static readonly Rect[,] Frames=new Rect[9,6]
    {
        { new Rect(43,169,125,125), new Rect(207,170,124,124), new Rect(368,170,127,124), new Rect(529,170,127,124), new Rect(692,170,126,124), new Rect(855,170,126,124) },
        { new Rect(66,382,75,69), new Rect(213,350,104,103), new Rect(362,346,139,107), new Rect(531,350,124,103), new Rect(713,398,93,54), new Rect(0,0,0,0) },
        { new Rect(54,546,101,67), new Rect(208,495,118,119), new Rect(366,490,129,124), new Rect(534,502,117,112), new Rect(706,546,102,68), new Rect(0,0,0,0) },
        { new Rect(66,699,77,73), new Rect(205,656,115,118), new Rect(363,654,134,120), new Rect(530,654,127,120), new Rect(708,702,93,70), new Rect(0,0,0,0) },
        { new Rect(59,846,81,57), new Rect(200,810,128,96), new Rect(365,810,136,96), new Rect(528,818,131,88), new Rect(702,839,106,69), new Rect(0,0,0,0) },
        { new Rect(61,973,83,70), new Rect(204,937,121,108), new Rect(363,934,135,111), new Rect(526,942,130,103), new Rect(699,983,108,62), new Rect(0,0,0,0) },
        { new Rect(62,1110,83,74), new Rect(201,1073,127,113), new Rect(363,1069,135,117), new Rect(527,1076,128,109), new Rect(699,1122,108,64), new Rect(0,0,0,0) },
        { new Rect(63,1268,77,57), new Rect(214,1225,101,102), new Rect(367,1220,118,107), new Rect(537,1234,105,93), new Rect(720,1266,70,59), new Rect(0,0,0,0) },
        { new Rect(59,1416,85,50), new Rect(207,1364,113,105), new Rect(365,1358,122,111), new Rect(528,1371,116,98), new Rect(702,1407,102,61), new Rect(0,0,0,0) }
    };
    internal static bool IsReady
    {
        get
        {
            if(!_loadAttempted)
            {
                _loadAttempted=true;
                _atlas=Resources.Load<Texture2D>("PlantArt/PixelFarmAtlas");
                if(_atlas!=null) _atlas.filterMode=FilterMode.Point;
                else Debug.LogWarning("PixelFarmAtlas is missing; using placeholder soil.");
            }
            return _atlas!=null;
        }
    }
    internal static Rect GetRect(int row,int column) => Frames[row,column];
    private static Rect BottomRect(int row,int column)
    {
        Rect r=Frames[row,column];
        return new Rect(r.x,_atlas.height-r.y-r.height,r.width,r.height);
    }
    internal static Sprite GetSprite(int row,int column)
    {
        if(!IsReady) return null;
        if(_sprites[row,column]==null)
            _sprites[row,column]=Sprite.Create(_atlas,BottomRect(row,column),
                row==0 ? new Vector2(0.5f,0.5f) : new Vector2(0.5f,0),128f);
        return _sprites[row,column];
    }
    internal static void Draw(Rect destination,int row,int column)
    {
        Rect r=BottomRect(row,column);
        Color old=GUI.color;
        GUI.color=Color.white;
        GUI.DrawTextureWithTexCoords(destination,_atlas,new Rect(r.x/_atlas.width,r.y/_atlas.height,r.width/_atlas.width,r.height/_atlas.height),true);
        GUI.color=old;
    }
}
