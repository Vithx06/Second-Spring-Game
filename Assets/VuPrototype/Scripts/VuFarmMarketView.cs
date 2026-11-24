using UnityEngine;
using System.Collections.Generic;

public partial class VuFarmMapView
{
    [SerializeField,Min(0)] private int _startingCoins=1000;
    internal static bool IsMenuOpen;
    private Wallet _wallet;
    private SpriteRenderer _warehouseBuilding,_marketBuilding;
    private ShopCatalog _catalog;
    private ShopFacade _shop;
    private int _marketPage; // 0 warehouse, 1 buy, 2 sell
    private int _warehouseFilter;
    private string _chosenItem;
    private int _tradeQuantity=1;
    private string _tradeMessage="Chọn vật phẩm để xem chi tiết.";
    private GUIStyle _marketText,_marketTitle,_marketSmall;
    private static readonly string[] CropIds={"rice","corn","sugarcane","strawberry","apple","orange","rose","orchid"};
    private static readonly string[] CropNames={"Lúa","Ngô","Mía","Dâu tây","Táo","Cam","Hoa hồng","Hoa lan"};
    private void InitializeMarket()
    {
        IsMenuOpen=false;
        foreach(var renderer in FindObjectsByType<SpriteRenderer>())
        {
            if(renderer.name=="Nhà kho")_warehouseBuilding=renderer;
            if(renderer.name=="Chợ nông sản")_marketBuilding=renderer;
        }
        _wallet=new Wallet(_startingCoins);
        var offers=new List<ShopOffer>();
        int[] buy={10,15,20,60,120,120,20,35},sell={8,12,16,14,20,20,18,30};
        for(int i=0;i<CropIds.Length;i++)
        {
            var seed=new ItemDefinition(CropIds[i]+".seed","Giống "+CropNames[i]);
            var product=new ItemDefinition(CropIds[i]+".product",CropNames[i]);
            Inventory.Register(seed);Inventory.Register(product);Inventory.TryAdd(seed,10);
            offers.Add(new ShopOffer(seed,buy[i],0));offers.Add(new ShopOffer(product,0,sell[i]));
        }
        foreach(string id in new[]{"fertilizer.yield","fertilizer.growth"})
        {
            var item=new ItemDefinition(id,id.EndsWith("yield") ? "Phân năng suất" : "Phân chín nhanh");
            Inventory.Register(item);Inventory.TryAdd(item,10);offers.Add(new ShopOffer(item,5,0));
        }
        _catalog=new ShopCatalog(offers);_shop=new ShopFacade(Inventory,_wallet,_catalog);
    }
    private static Color MarketColor(uint rgb) => new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255);
    private static void MarketFill(Rect r,uint rgb)
    { Color before=GUI.color;GUI.color=MarketColor(rgb);GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=before; }
    private void MarketStyles()
    {
        if(_marketText!=null)return;
        _marketText=new GUIStyle(GUI.skin.label){font=Font.CreateDynamicFontFromOSFont("Arial",18),fontSize=18,wordWrap=true,alignment=TextAnchor.MiddleCenter};
        _marketText.normal.textColor=MarketColor(0x3D2D1D);
        _marketTitle=new GUIStyle(_marketText){fontSize=25,fontStyle=FontStyle.Bold};
        _marketSmall=new GUIStyle(_marketText){fontSize=14};
    }
    private void MarketBoard(Rect r)
    {
        MarketFill(new Rect(r.x+5,r.y+6,r.width,r.height),0x433422);
        MarketFill(r,0x66442A);MarketFill(new Rect(r.x+5,r.y+5,r.width-10,r.height-10),0xD4A966);
        for(float y=r.y+12;y<r.yMax-8;y+=28) MarketFill(new Rect(r.x+7,y,r.width-14,1),0xC29755);
        MarketFill(new Rect(r.x+10,r.y+10,r.width-20,3),0xF3D59D);
    }
    private bool MarketButton(Rect r,string text,bool selected=false)
    {
        bool click=GUI.Button(r,GUIContent.none,GUIStyle.none);
        MarketFill(r,0x6A4C2F);MarketFill(new Rect(r.x+2,r.y+2,r.width-4,r.height-4),selected ? 0x8BAC59u : 0xE8C990u);
        GUI.Label(r,text,_marketText);return click;
    }
    private void OpenMarket(int page)
    { CancelDrag();Selected=null;IsMenuOpen=true;_marketPage=page;_chosenItem=null;_tradeQuantity=1;_tradeMessage="Chọn vật phẩm để xem chi tiết."; }
    private void DrawItemIcon(Rect r,string id)
    {
        for(int i=0;i<CropIds.Length;i++)
            if(id.StartsWith(CropIds[i]+"."))
            {
                if(!PlantItemSprites.Draw(r,i,id.EndsWith(".seed")) && PlantPixelSprites.IsReady)PlantPixelSprites.Draw(r,i+1,2);
                return;
            }
        GUI.DrawTexture(r,PlantToolIcons.Get(id.EndsWith("yield") ? PlantToolIcons.Kind.Yield : PlantToolIcons.Kind.Growth));
    }
    private void DrawMarketInterface()
    {
        MarketStyles();
        MarketBoard(new Rect(1080,12,184,56));
        GUI.DrawTexture(new Rect(1090,21,36,36),PlantToolIcons.Get(PlantToolIcons.Kind.Coin));
        GUI.Label(new Rect(1129,20,126,38),_wallet.Balance.ToString("N0"),_marketTitle);
        if(!IsMenuOpen && !IsDragging)
        {
            bool overTools=Selected!=null && new Rect(20,543,1240,166).Contains(Event.current.mousePosition);
            if(!overTools)
            {
                if(BuildingButton(_warehouseBuilding,"Mở nhà kho"))OpenMarket(0);
                if(!IsMenuOpen && BuildingButton(_marketBuilding,"Ghé chợ nông sản"))OpenMarket(1);
            }
        }
        if(!IsMenuOpen)return;
        MarketFill(new Rect(0,80,1280,641),0x344B32);
        MarketBoard(new Rect(105,94,1070,603));
        GUI.Label(new Rect(140,109,800,40),_marketPage==0 ? "NHÀ KHO" : _marketPage==1 ? "CHỢ NÔNG SẢN · MUA" : "CHỢ NÔNG SẢN · BÁN",_marketTitle);
        if(MarketButton(new Rect(1115,108,40,38),"×")) {IsMenuOpen=false;return;}
        if(_marketPage==0)
        {
            string[] groups={"Tất cả","Giống","Nông sản","Phân bón"};
            for(int i=0;i<4;i++) if(MarketButton(new Rect(133+i*157,160,150,36),groups[i],i==_warehouseFilter)) {_warehouseFilter=i;_chosenItem=null;}
        }
        else
        {
            if(MarketButton(new Rect(133,160,240,36),"Mua giống / phân",_marketPage==1))OpenMarket(1);
            if(MarketButton(new Rect(383,160,240,36),"Bán nông sản",_marketPage==2))OpenMarket(2);
        }
        var visible=new List<ShopOffer>();
        foreach(var offer in _catalog.Offers)
        {
            string id=offer.Item.Id;
            if(_marketPage==1 && !offer.CanBuy || _marketPage==2 && !offer.CanSell)continue;
            if(_marketPage==0)
            {
                if(Inventory.GetQuantity(id)==0)continue;
                if(_warehouseFilter==1 && !id.EndsWith(".seed") || _warehouseFilter==2 && !id.EndsWith(".product") || _warehouseFilter==3 && !id.StartsWith("fertilizer."))continue;
            }
            visible.Add(offer);
        }
        for(int i=0;i<visible.Count;i++)
        {
            var offer=visible[i];Rect cell=new Rect(133+(i%6)*106,213+(i/6)*123,98,113);
            if(MarketButton(cell,"",_chosenItem==offer.Item.Id)) {_chosenItem=offer.Item.Id;_tradeMessage="";}
            DrawItemIcon(new Rect(cell.x+23,cell.y+7,52,50),offer.Item.Id);
            GUI.Label(new Rect(cell.x+2,cell.y+57,94,32),offer.Item.Name,_marketSmall);
            GUI.Label(new Rect(cell.x+2,cell.y+88,94,23),_marketPage==0 ? "× "+Inventory.GetQuantity(offer.Item.Id) : (_marketPage==1 ? offer.BuyPrice : offer.SellPrice)+" xu",_marketSmall);
        }
        if(visible.Count==0)GUI.Label(new Rect(150,290,580,60),"Chưa có vật phẩm trong mục này.",_marketText);
        MarketBoard(new Rect(792,160,354,466));
        var chosen=_catalog.FindOffer(_chosenItem);
        if(chosen!=null)
        {
            DrawItemIcon(new Rect(915,180,104,100),chosen.Item.Id);
            GUI.Label(new Rect(810,288,318,55),chosen.Item.Name,_marketTitle);
            GUI.Label(new Rect(810,345,318,30),"Đang có: "+Inventory.GetQuantity(chosen.Item.Id),_marketText);
            if(_marketPage==0)
                GUI.Label(new Rect(813,390,312,120),chosen.Item.Id.EndsWith(".seed") ? "Kéo giống từ thanh trồng cây để gieo." : chosen.CanSell ? "Mang nông sản đến mục Bán của chợ." : "Dùng phân bón từ thanh chăm cây.",_marketText);
            else
            {
                GUI.Label(new Rect(811,383,318,30),"Đơn giá: "+(_marketPage==1 ? chosen.BuyPrice : chosen.SellPrice)+" xu",_marketText);
                if(MarketButton(new Rect(833,427,54,40),"−"))_tradeQuantity=Mathf.Max(1,_tradeQuantity-1);
                GUI.Label(new Rect(893,427,80,40),_tradeQuantity.ToString(),_marketTitle);
                if(MarketButton(new Rect(979,427,54,40),"+"))_tradeQuantity=Mathf.Min(999,_tradeQuantity+1);
                if(MarketButton(new Rect(1041,427,76,40),"+10"))_tradeQuantity=Mathf.Min(999,_tradeQuantity+10);
                GUI.Label(new Rect(811,479,318,34),"Tổng: "+((long)_tradeQuantity*(_marketPage==1 ? chosen.BuyPrice : chosen.SellPrice))+" xu",_marketTitle);
                if(MarketButton(new Rect(827,536,284,55),_marketPage==1 ? "Mua" : "Bán",true))
                    _tradeMessage=TradeMessage(_marketPage==1 ? _shop.Buy(chosen.Item.Id,_tradeQuantity) : _shop.Sell(chosen.Item.Id,_tradeQuantity));
            }
        }
        else GUI.Label(new Rect(812,296,314,100),"Chọn một vật phẩm bên trái.",_marketText);
        GUI.Label(new Rect(134,637,1008,45),_tradeMessage,_marketText);
        if(Event.current.type==EventType.KeyDown && Event.current.keyCode==KeyCode.Escape) {IsMenuOpen=false;Event.current.Use();}
        // Modal input never reaches the soil beneath this board.
        if(Event.current.isMouse)Event.current.Use();
    }
    private bool BuildingButton(SpriteRenderer building,string hint)
    {
        if(building==null || _camera==null || !building.enabled)return false;
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/721f);
        Vector3 origin=GUI.matrix.MultiplyPoint(Vector3.zero);
        Vector3 low=_camera.WorldToScreenPoint(building.bounds.min);
        Vector3 high=_camera.WorldToScreenPoint(building.bounds.max);
        var rect=new Rect((low.x-origin.x)/scale,(Screen.height-high.y-origin.y)/scale,
            (high.x-low.x)/scale,(high.y-low.y)/scale);
        bool clicked=GUI.Button(rect,GUIContent.none,GUIStyle.none);
        if(rect.Contains(Event.current.mousePosition))
        {
            var label=new Rect(rect.center.x-105,rect.y-29,210,26);
            MarketFill(label,0xEDD09A);GUI.Label(label,hint,_marketSmall);
        }
        return clicked;
    }
    private static string TradeMessage(TradeResult result)
    {
        switch(result)
        {
            case TradeResult.Success:return "Giao dịch thành công.";
            case TradeResult.NotEnoughMoney:return "Không đủ tiền.";
            case TradeResult.NotEnoughItems:return "Không đủ vật phẩm để bán.";
            case TradeResult.InventoryFull:return "Kho không đủ chỗ.";
            case TradeResult.AmountOverflow:return "Số tiền vượt giới hạn.";
            default:return "Chưa thể giao dịch. Kiểm tra vật phẩm và số lượng.";
        }
    }
}
