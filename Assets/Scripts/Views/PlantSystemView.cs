using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// View chỉ vẽ thông tin, đọc dữ liệu và chuyển lựa chọn của người chơi cho Controller.
public sealed class PlantSystemView : MonoBehaviour
{
    [SerializeField] private Sprite[] _soilAndCropSprites;
    [SerializeField] private Sprite[] _itemSprites;
    private readonly Dictionary<string,Sprite> _sprites=new Dictionary<string,Sprite>();
    private PlantSystemController _controller;
    private GUIStyle _text,_small,_title,_button;
    private Vector2 _offset;
    private float _scale;
    private int _warehouseFilter;
    private Vector2 _warehouseScroll;
    private static readonly Rect Toolbar=new Rect(12,565,1256,144);
    private static readonly string[] ToolNames={"Xới đất","Gieo","Tưới nước","Thu hoạch","Phân năng suất","Phân chín nhanh"};

    public void Initialize(PlantSystemController controller)
    {
        _controller=controller;
        foreach(var sprite in _soilAndCropSprites.Concat(_itemSprites))if(sprite)_sprites[sprite.name]=sprite;
    }
    public Sprite FindSprite(string name) => _sprites.TryGetValue(name,out var sprite)?sprite:null;
    private void Layout()
    {
        _scale=Mathf.Min(Screen.width/1280f,Screen.height/721f);
        _offset=new Vector2((Screen.width-1280*_scale)/2,(Screen.height-721*_scale)/2);
    }
    public bool Covers(Vector2 screen)
    {
        Layout();
        Vector2 p=new Vector2((screen.x-_offset.x)/_scale,(Screen.height-screen.y-_offset.y)/_scale);
        return _controller.WarehouseOpen || new Rect(12,12,300,52).Contains(p)
            || (_controller.Selected && Toolbar.Contains(p));
    }
    private void OnGUI()
    {
        if(!_controller || _controller.Clock==null)return;
        Layout();Styles();
        var before=GUI.matrix;
        GUI.matrix=Matrix4x4.TRS(new Vector3(_offset.x,_offset.y,0),Quaternion.identity,Vector3.one*_scale);
        DrawClock();
        if(_controller.WarehouseOpen)DrawWarehouse();
        else
        {
            DrawWaterNeeds();
            if(_controller.Selected)DrawTools();
            else GUI.Label(new Rect(260,15,770,36),_controller.Notice,_small);
        }
        GUI.matrix=before;
    }
    private void Styles()
    {
        if(_text!=null)return;
        _text=new GUIStyle(GUI.skin.label){fontSize=16,wordWrap=true,alignment=TextAnchor.MiddleCenter};
        _text.normal.textColor=new Color32(57,40,24,255);
        _small=new GUIStyle(_text){fontSize=13};
        _title=new GUIStyle(_text){fontSize=20,fontStyle=FontStyle.Bold};
        _button=new GUIStyle(GUI.skin.button){fontSize=14,wordWrap=true};
    }
    private static void Fill(Rect rect,Color color)
    {
        var before=GUI.color;GUI.color=color;GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=before;
    }
    private static void Board(Rect rect)
    {
        Fill(rect,new Color32(97,62,31,255));
        Fill(new Rect(rect.x+4,rect.y+4,rect.width-8,rect.height-8),new Color32(235,207,153,255));
        Fill(new Rect(rect.x+8,rect.y+8,rect.width-16,3),new Color32(255,232,187,255));
    }
    private bool Button(Rect rect,string label) => GUI.Button(rect,label,_button);
    private static void Icon(Rect rect,Sprite sprite)
    {
        if(!sprite)return;
        var pixels=sprite.rect;float ratio=pixels.width/pixels.height;
        float width=Mathf.Min(rect.width,rect.height*ratio),height=width/ratio;
        // Sprite nằm trong atlas: chỉ vẽ đúng vùng UV, không vẽ toàn bộ sheet lên nút.
        GUI.DrawTextureWithTexCoords(new Rect(rect.center.x-width/2,rect.center.y-height/2,width,height),sprite.texture,
            new Rect(pixels.x/sprite.texture.width,pixels.y/sprite.texture.height,pixels.width/sprite.texture.width,pixels.height/sprite.texture.height));
    }
    private void DrawClock()
    {
        Board(new Rect(12,12,200,48));
        GUI.Label(new Rect(20,18,184,30),"Ngày "+_controller.Clock.Day+" · "+_controller.Clock.Hour.ToString("00")+":"+_controller.Clock.Minute.ToString("00"),_text);
        if(Button(new Rect(220,20,80,31),_controller.Paused?"Tiếp tục":"Tạm dừng"))_controller.Paused=!_controller.Paused;
    }
    private void DrawTools()
    {
        var selected=_controller.Selected;if(!selected)return;
        Board(Toolbar);
        GUI.Label(new Rect(28,572,1160,25),selected.name+" · "+PlantSystemController.StateText(selected.Model),_text);
        if(Button(new Rect(1223,575,30,24),"×")) { _controller.CloseTools();return; }
        for(int i=0;i<_controller.Crops.Length;i++)
        {
            var crop=_controller.Crops[i];var rect=new Rect(25+i*75,605,70,73);
            Color before=GUI.backgroundColor;
            if(_controller.SelectedCrop==crop.Type)GUI.backgroundColor=new Color(.55f,.8f,.40f);
            if(Button(rect,""))_controller.ChooseCrop(crop.Type);
            GUI.backgroundColor=before;
            Icon(new Rect(rect.x+13,rect.y+2,42,39),FindSprite("Seed_"+crop.Type));
            GUI.Label(new Rect(rect.x,rect.y+40,rect.width,23),crop.Name,_small);
            GUI.Label(new Rect(rect.x+40,rect.y+2,28,21),_controller.Inventory.GetQuantity(crop.SeedItem).ToString(),_small);
        }
        for(int i=0;i<6;i++)
        {
            var tool=(PlantTool)i;var rect=new Rect(640+i%3*200,605+i/3*43,190,39);
            if(Button(rect,""))_controller.ChooseTool(tool);
            GUI.DrawTexture(new Rect(rect.x+3,rect.y+3,32,32),ToolIcon(tool),ScaleMode.ScaleToFit);
            string name=ToolNames[i];
            if(tool==PlantTool.YieldFertilizer)name+=" · "+_controller.Inventory.GetQuantity(ItemType.YieldBoostFertilizer);
            if(tool==PlantTool.GrowthFertilizer)name+=" · "+_controller.Inventory.GetQuantity(ItemType.GrowthBoostFertilizer);
            GUI.Label(new Rect(rect.x+36,rect.y+2,150,34),name,_small);
        }
        GUI.Label(new Rect(25,681,1205,22),_controller.Notice,_small);
    }
    private static Texture2D ToolIcon(PlantTool tool)
    {
        FarmToolIcons.Kind[] kinds={FarmToolIcons.Kind.Hoe,FarmToolIcons.Kind.Seed,FarmToolIcons.Kind.Water,FarmToolIcons.Kind.Basket,FarmToolIcons.Kind.Yield,FarmToolIcons.Kind.Growth};
        return FarmToolIcons.Get(kinds[(int)tool]);
    }
    private void DrawWaterNeeds()
    {
        foreach(var plot in _controller.Plots)
        {
            var crop=plot.Model.CurrentCrop;
            if(crop==null || !crop.CanWater || crop.WateringCount!=0)continue;
            var screen=_controller.MapCamera.WorldToScreenPoint(plot.transform.position+Vector3.up*.5f);
            var point=new Vector2((screen.x-_offset.x)/_scale,(Screen.height-screen.y-_offset.y)/_scale);
            float bob=Mathf.Sin(Time.unscaledTime*3+plot.Model.Id)*3;
            GUI.DrawTexture(new Rect(point.x-10,point.y-24+bob,20,24),FarmToolIcons.Get(FarmToolIcons.Kind.Drop),ScaleMode.ScaleToFit);
        }
    }
    private void DrawWarehouse()
    {
        Board(new Rect(175,125,930,465));
        GUI.Label(new Rect(200,143,820,34),"Nhà kho · "+_controller.Inventory.UsedSlots+" / "+_controller.Inventory.Capacity+" ô",_title);
        if(Button(new Rect(1056,143,30,27),"×")) { _controller.OpenWarehouse(false);return; }
        string[] filters={"Tất cả","Hạt giống","Sản phẩm","Phân bón"};
        for(int i=0;i<filters.Length;i++)if(Button(new Rect(205+i*216,190,205,31),filters[i]))_warehouseFilter=i;
        var items=_controller.Inventory.GetItems().Where(item=>_warehouseFilter==0
            || (_warehouseFilter==1 && item.Item.Category==ItemCategory.Seed)
            || (_warehouseFilter==2 && item.Item.Category==ItemCategory.CropProduct)
            || (_warehouseFilter==3 && item.Item.Category==ItemCategory.Fertilizer)).OrderBy(item=>item.Item.Type).ToArray();
        _warehouseScroll=GUI.BeginScrollView(new Rect(205,237,880,295),_warehouseScroll,new Rect(0,0,855,Mathf.Max(280,Mathf.Ceil(items.Length/6f)*110)));
        for(int i=0;i<items.Length;i++)
        {
            var item=items[i];var rect=new Rect(i%6*142,i/6*110,132,102);
            Fill(rect,new Color32(217,183,121,255));
            var crop=_controller.Crops.FirstOrDefault(c=>c.SeedItem==item.Item.Type || c.ProductItem==item.Item.Type);
            if(crop!=null)Icon(new Rect(rect.x+34,rect.y+4,64,57),FindSprite((item.Item.Category==ItemCategory.Seed?"Seed_":"Product_")+crop.Type));
            else GUI.DrawTexture(new Rect(rect.x+43,rect.y+9,45,45),ToolIcon(item.Item.Type==ItemType.YieldBoostFertilizer?PlantTool.YieldFertilizer:PlantTool.GrowthFertilizer),ScaleMode.ScaleToFit);
            GUI.Label(new Rect(rect.x+3,rect.y+61,126,25),item.Item.Name,_small);
            GUI.Label(new Rect(rect.x+4,rect.y+82,124,20),"× "+item.Quantity,_small);
        }
        GUI.EndScrollView();
        GUI.Label(new Rect(205,542,880,28),items.Length==0?"Chưa có vật phẩm thuộc nhóm này.":"Giống được trừ khi gieo; sản phẩm được cộng khi thu hoạch thành công.",_small);
    }
}
