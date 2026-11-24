using UnityEngine;

// Presentation only. Every action still goes through FarmingService.
public partial class FarmPlotView
{
    private Font _interfaceFont;
    private GUIStyle _text, _heading, _small, _button, _center;
    private bool _showTestOptions;
    private static readonly Color Ink = Hex(0x263B32);
    private static readonly Color Muted = Hex(0x6B7A70);
    private static readonly Color Green = Hex(0x365F45);
    private static readonly Color Line = Hex(0xDCE3D5);

    private static Color Hex(uint rgb) => new Color32((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb, 255);
    private static void Fill(Rect rect, Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = old;
    }
    private void EnsureInterfaceStyles()
    {
        if (_text != null) return;
        _interfaceFont = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "DejaVu Sans", "Liberation Sans" }, 18);
        _text = new GUIStyle(GUI.skin.label) { font=_interfaceFont, fontSize=18, wordWrap=true };
        _text.normal.textColor = Ink;
        _heading = new GUIStyle(_text) { fontSize=26, fontStyle=FontStyle.Bold };
        _small = new GUIStyle(_text) { fontSize=14 };
        _small.normal.textColor = Muted;
        _center = new GUIStyle(_text) { alignment=TextAnchor.MiddleCenter };
        _button = new GUIStyle(_center) { fontStyle=FontStyle.Bold, fontSize=17 };
    }
    private void Label(float x,float y,float w,float h,string value,GUIStyle style=null)
        => GUI.Label(new Rect(x,y,w,h),value,style ?? _text);
    private void Card(float x,float y,float w,float h)
    {
        Fill(new Rect(x+3,y+4,w,h),Hex(0x493421));
        Fill(new Rect(x,y,w,h),Hex(0x795132));
        Fill(new Rect(x+4,y+4,w-8,h-8),Hex(0xE6C78D));
        for(float row=y+12;row<y+h-8;row+=28)
            Fill(new Rect(x+5,row,w-10,1),Hex(0xDDC082));
    }
    private bool ActionButton(Rect rect,string text,bool allowed,bool primary=false)
    {
        bool oldEnabled=GUI.enabled;
        GUI.enabled=oldEnabled && allowed;
        bool clicked=GUI.Button(rect,GUIContent.none,GUIStyle.none);
        bool hover=rect.Contains(Event.current.mousePosition) && allowed;
        Fill(rect,!allowed ? Hex(0xE8EBE3) : primary ? (hover ? Hex(0x477654) : Green)
            : hover ? Hex(0xE2ECD9) : Hex(0xF0F3E9));
        Color old=_button.normal.textColor;
        _button.normal.textColor=!allowed ? Hex(0x9CA69B) : primary ? Color.white : Ink;
        GUI.Label(rect,text,_button);
        _button.normal.textColor=old;
        GUI.enabled=oldEnabled;
        return clicked;
    }
    private string SoilName()
    {
        switch (_plot.Status)
        {
            case FarmPlotStatus.Tilling: return "Đang xới đất";
            case FarmPlotStatus.ReadyToSow: return "Đất đã sẵn sàng";
            case FarmPlotStatus.HaveResidue: return _plot.ResidueType==ResidueType.RottenCrop ? "Cây đã thối" : "Còn bã sau thu hoạch";
            case FarmPlotStatus.HavePlant:
                Crop c=_plot.CurrentCrop;
                if(c.State==CropState.Ripe) return "Đã chín · có thể thu hoạch";
                return (c.WateringCount==0 ? "Cần tưới nước" : "Đang sinh trưởng")
                    + (c.State==CropState.Stage1 ? " · giai đoạn 1" : " · giai đoạn 2");
            default: return "Đất chưa xới";
        }
    }
    private string Hint()
    {
        if(_plot.CanTill()) return "Xới ô đất để chuẩn bị cho một vụ trồng mới.";
        if(_plot.Status==FarmPlotStatus.Tilling) return "Đất tự xới đến khi hoàn thành. Không cần bấm lại.";
        if(_plot.CanPlant()) return "Chọn một loại giống bên trái, sau đó bấm Gieo hạt.";
        Crop c=_plot.CurrentCrop;
        if(c==null) return "Chọn thao tác phù hợp với ô đất.";
        if(c.CanHarvest()) return "Thu hoạch trước khi cây thối. Kho đầy sẽ chưa nhận được sản phẩm.";
        return c.WateringCount==0 ? "Tưới một lần để bắt đầu giai đoạn này. Để thiếu nước lâu sẽ giảm năng suất hoặc chết."
            : "Cây đang lớn. Tưới thêm không làm cây nhanh lớn; lần tưới thứ 5 trong giai đoạn sẽ làm cây chết.";
    }
    private void DrawPlantInterface()
    {
        EnsureInterfaceStyles();
        Matrix4x4 oldMatrix=GUI.matrix;
        Color oldColor=GUI.color;
        bool oldEnabled=GUI.enabled;
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/800f);
        GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1280*scale)/2,(Screen.height-800*scale)/2,0),Quaternion.identity,new Vector3(scale,scale,1));
        try
        {
            Fill(new Rect(0,0,1280,800),Hex(0xEEF2E7));
            Fill(new Rect(0,0,1280,8),Green);
            Label(28,25,650,40,"SECOND SPRING  /  VƯỜN TRỒNG",_heading);
            Label(30,68,750,24,"Chọn giống, chăm cây và đón vụ thu hoạch của bạn.",_small);
            Label(932,34,320,26,"BẢN THỬ PHẦN THỰC VẬT",_small);
            Label(932,61,320,24,"Dùng đồng hồ game chung",_small);
            Fill(new Rect(28,106,1224,54),Hex(0xDEE8D4));
            Label(44,119,800,32,"Ô đất 01   /   "+SoilName());
            Label(935,123,300,25,"Ngày "+_clock.Day+" · "+_clock.Hour.ToString("00")+":"+_clock.Minute.ToString("00"),_small);
            DrawSeedPanel();
            DrawGardenPanel();
            DrawActionPanel();
            Card(298,620,620,100);
            Label(318,631,580,26,"GỢI Ý CHĂM SÓC",_small);
            Label(318,660,580,50,Hint());
            Fill(new Rect(28,744,1224,38),Hex(0xE3E9DB));
            Label(42,752,1195,28,_message,_small);
        }
        finally { GUI.matrix=oldMatrix; GUI.color=oldColor; GUI.enabled=oldEnabled; }
    }
    private void DrawSeedPanel()
    {
        Card(28,180,250,540);
        Label(46,197,220,30,"Hạt giống",_heading);
        Label(46,234,218,26,"Chọn giống để gieo lên đất trống",_small);
        for(int i=0;i<_definitions.Length;i++)
        {
            Rect r=new Rect(44+(i%2)*114,276+(i/2)*89,106,78);
            bool selected=_selectedCrop==i;
            if(GUI.Button(r,GUIContent.none,GUIStyle.none)) _selectedCrop=i;
            Fill(r,selected ? Hex(0xDEEBD2) : Hex(0xF2F3EB));
            if(selected) Fill(new Rect(r.x,r.y,4,r.height),Green);
            Color tint=CropColor(i);
            Fill(new Rect(r.x+13,r.y+12,10,10),tint);
            Label(r.x+29,r.y+6,76,27,_definitions[i].Name,_small);
            Label(r.x+12,r.y+40,88,27,"Còn "+_inventory.GetQuantity(_definitions[i].SeedItemId),_small);
        }
        var selectedDef=_definitions[_selectedCrop];
        Label(46,648,214,26,selectedDef.HarvestType==HarvestType.Regrowing ? "Giống tái thu hoạch" : "Giống thu hoạch một lần",_small);
        Label(46,676,214,26,"Sản phẩm: "+_inventory.GetQuantity(selectedDef.ProductItemId),_small);
    }
    private void DrawGardenPanel()
    {
        Card(298,180,620,420);
        Crop c=_plot.CurrentCrop;
        Label(318,198,480,34,c==null ? "Ô đất của bạn" : c.Definition.Name,_heading);
        for(int i=0;i<4;i++)
        {
            int active=_plot.Status==FarmPlotStatus.Untilled || _plot.Status==FarmPlotStatus.HaveResidue ? 0
                : _plot.Status==FarmPlotStatus.Tilling ? 0 : c==null ? 1 : c.State==CropState.Ripe ? 3 : 2;
            Fill(new Rect(320+i*145,246,132,4),i<=active ? Green : Line);
            Label(320+i*145,258,137,24,new[]{"01  Xới đất","02  Gieo hạt","03  Chăm sóc","04  Thu hoạch"}[i],_small);
        }
        Fill(new Rect(318,300,580,228),Hex(0xDCE9CB));
        DrawPixelGarden();
        Label(337,308,540,26,_plot.Status==FarmPlotStatus.Tilling ? "Đang chuẩn bị đất..." : "Bấm ô đất để thực hiện thao tác tiếp theo",_small);
        float progress=0;string progressText="Sẵn sàng cho một vụ mới";
        if(_plot.Status==FarmPlotStatus.Tilling)
        {
            progress=1-_plot.TillingTimeRemaining/(_tillingGameHours*3600);
            progressText="Xới đất · còn "+GameClock.FormatDuration(_plot.TillingTimeRemaining);
        }
        else if(c!=null)
        {
            if(c.State==CropState.Ripe) { progress=1;progressText="Đã chín · hãy thu hoạch"; }
            else if(c.WateringCount==0) { progress=0;progressText="Chờ tưới để bắt đầu giai đoạn"; }
            else
            {
                float total=c.State==CropState.Stage1 ? c.Definition.Stage1Duration : Mathf.Max(c.Definition.Stage2Duration,c.Definition.RegrowthDuration);
                if(c.FertilizerType==FertilizerType.GrowthBoost) total*=0.75f;
                progress=1-c.GrowthTimeRemaining/total;
                progressText="Đang lớn · còn "+GameClock.FormatDuration(c.GrowthTimeRemaining);
            }
        }
        Fill(new Rect(320,548,574,8),Line);
        Fill(new Rect(320,548,574*Mathf.Clamp01(progress),8),Green);
        Label(320,565,574,24,progressText,_small);
    }
    private static Color CropColor(int index)
    {
        uint[] values={0xD4B34C,0xE8BF45,0x86AA60,0xD96A67,0xC35A47,0xE6A649,0xD76C85,0xAB8AC1};
        return Hex(values[Mathf.Clamp(index,0,7)]);
    }
    private void DrawPlant(float x,float ground,Crop c,int index)
    {
        float height=c.State==CropState.Stage1 ? 22 : 43;
        Fill(new Rect(x,ground-height,5,height),Hex(0x487242));
        Fill(new Rect(x-13,ground-height+8,14,7),Hex(0x78A35A));
        Fill(new Rect(x+5,ground-height+17,15,7),Hex(0x5B8B48));
        if(c.State==CropState.Ripe)
        {
            Fill(new Rect(x-4,ground-height-9,14,15),CropColor(index));
            Fill(new Rect(x-1,ground-height-12,8,4),CropColor(index));
        }
    }
    private void DrawActionPanel()
    {
        Card(938,180,314,540);
        Label(958,197,275,32,"Chăm sóc ô đất",_heading);
        Crop c=_plot.CurrentCrop;
        Label(958,239,275,46,c==null ? "Chọn thao tác bên dưới để bắt đầu." : "Đã tưới: "+c.WateringCount+" lần trong giai đoạn này",_small);
        if(ActionButton(new Rect(958,295,128,44),"Xới đất",_plot.CanTill())) Run(_service.Till(_plot));
        if(ActionButton(new Rect(1097,295,135,44),"Gieo hạt",_plot.CanPlant(),true)) Run(_service.Plant(_plot,_definitions[_selectedCrop]));
        bool growing=c!=null && (c.State==CropState.Stage1 || c.State==CropState.Stage2);
        if(ActionButton(new Rect(958,351,128,44),"Tưới nước",growing,true)) Run(_service.Water(_plot));
        if(ActionButton(new Rect(1097,351,135,44),"Thu hoạch",c!=null && c.CanHarvest(),true)) Run(_service.Harvest(_plot));
        Label(958,418,275,25,"PHÂN BÓN · chọn một loại / cây",_small);
        bool canFertilize=c!=null && c.CanFertilize();
        if(ActionButton(new Rect(958,452,274,44),"Năng suất +50%  ·  "+_inventory.GetQuantity("fertilizer.yield"),canFertilize)) Run(_service.Fertilize(_plot,"fertilizer.yield"));
        if(ActionButton(new Rect(958,508,274,44),"Chín nhanh -25%  ·  "+_inventory.GetQuantity("fertilizer.growth"),canFertilize)) Run(_service.Fertilize(_plot,"fertilizer.growth"));
        if(c!=null)
            Label(958,565,274,37,c.HasDroughtPenalty ? "Thiếu nước: đã mất 25% năng suất." : c.FertilizerType==FertilizerType.None ? "Cây chưa được bón phân." : "Cây đã được bón phân.",_small);
        if(ActionButton(new Rect(958,615,274,32),_showTestOptions ? "Ẩn tùy chọn thử" : "Tùy chọn thử",true)) _showTestOptions=!_showTestOptions;
        if(_showTestOptions)
        {
            Label(958,656,132,32,"Đồng hồ chung",_small);
            if(ActionButton(new Rect(1100,656,132,32),_inventory.Capacity==0 ? "Kho: đầy" : "Kho: trống",true))
                _inventory.Capacity=_inventory.Capacity==0 ? 100 : 0;
        }
        else Label(958,662,274,30,"Sinh trưởng theo ngày, giờ game.",_small);
    }
}
