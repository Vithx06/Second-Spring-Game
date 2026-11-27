using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlantTool
{
    Hoe, // Xới đất hoặc dọn tàn dư
    Sow, // Gieo giống đang chọn
    Water, // Tưới nước
    Harvest, // Thu hoạch cây chín
    YieldFertilizer, // Bón phân tăng năng suất
    GrowthFertilizer // Bón phân chín nhanh
}

// Controller khởi tạo Model chung và chuyển thao tác Unity thành lời gọi nghiệp vụ.
[DefaultExecutionOrder(-100),RequireComponent(typeof(PlantSystemView))]
public sealed class PlantSystemController : MonoBehaviour
{
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private SpriteRenderer _warehouse;
    [SerializeField] private FarmPlotController[] _plots;
    private PlantSystemView _view;
    private readonly HashSet<FarmPlotController> _visited=new HashSet<FarmPlotController>();
    private Vector2 _lastPointer;
    private bool _hasPointer;

    public Inventory Inventory { get; private set; }
    public ItemCatalog Catalog { get; private set; }
    public GameClock Clock { get; private set; }
    public FarmingService Farming { get; private set; }
    public CropDefinition[] Crops { get; private set; }
    public IReadOnlyList<FarmPlotController> Plots => _plots;
    public FarmPlotController Selected { get; private set; }
    public CropType SelectedCrop { get; private set; }=CropType.Rice;
    public PlantTool? ActiveTool { get; private set; }
    public bool WarehouseOpen { get; private set; }
    public bool Paused { get; set; }
    public string Notice { get; private set; }="Bấm một ô đất để mở dụng cụ. Bấm nhà kho để xem vật phẩm.";
    public Camera MapCamera => _mapCamera;

    private void Awake()
    {
        _view=GetComponent<PlantSystemView>();
        if(!_mapCamera)_mapCamera=Camera.main;
        if(_plots==null || _plots.Length==0 || !_mapCamera || !_warehouse)
            throw new InvalidOperationException("Plant scene references are missing.");

        Crops=CropGameRules.CreateDefinitions(86400);
        var items=new List<ItemDefinition>();
        foreach(var crop in Crops)
        {
            items.Add(new ItemDefinition(crop.SeedItem,"Giống "+crop.Name,ItemCategory.Seed));
            items.Add(new ItemDefinition(crop.ProductItem,crop.Name,ItemCategory.CropProduct));
        }
        items.Add(new ItemDefinition(ItemType.YieldBoostFertilizer,"Phân năng suất",ItemCategory.Fertilizer));
        items.Add(new ItemDefinition(ItemType.GrowthBoostFertilizer,"Phân chín nhanh",ItemCategory.Fertilizer));
        Catalog=new ItemCatalog(items);
        Inventory=new Inventory(Catalog);
        // Vật phẩm ban đầu để chơi thử, dùng chung cho tất cả ô đất và bảng kho.
        foreach(var crop in Crops)Inventory.TryAdd(crop.SeedItem,5);
        Inventory.TryAdd(ItemType.YieldBoostFertilizer,3);
        Inventory.TryAdd(ItemType.GrowthBoostFertilizer,3);

        _view.Initialize(this);
        foreach(var plot in _plots)plot.Initialize(this,_view);
        Farming=new FarmingService(Inventory,_plots.Select(p=>p.Model),Catalog,
            new ICropCreator[]{new SingleHarvestCropCreator(),new RegrowingCropCreator()},FertilizerDefinition.CreateDefaults());
        Clock=new GameClock();
        // Clock tự đổi giây thực sang giây game: 300 giây thực tương ứng một ngày game.
        // Chỉ đăng ký một lần; không tạo đồng hồ riêng cho mỗi ô.
        Clock.Subscribe(Farming);
        RefreshPlots();
    }

    private void Update()
    {
        if(Clock==null)return;
        HandlePointer();
        if(!Paused)Clock.Advance(Time.deltaTime);
        RefreshPlots();
    }

    public void Select(FarmPlotController plot)
    {
        if(Array.IndexOf(_plots,plot)<0)return;
        Selected=plot;
        Notice="Đã chọn "+plot.name+" · "+StateText(plot.Model);
    }
    public void ChooseCrop(CropType type)
    {
        if(!Crops.Any(c=>c.Type==type))return;
        SelectedCrop=type;ChooseTool(PlantTool.Sow);
    }
    public void ChooseTool(PlantTool tool)
    {
        ActiveTool=tool;
        if(Selected)Work(Selected,tool);
    }
    public bool Work(FarmPlotController plot,PlantTool tool)
    {
        if(Paused || Array.IndexOf(_plots,plot)<0)return false;
        bool success;
        switch(tool)
        {
            case PlantTool.Hoe:success=Farming.Till(plot.Model);break;
            case PlantTool.Sow:success=Farming.Plant(plot.Model,Crops.First(c=>c.Type==SelectedCrop));break;
            case PlantTool.Water:success=Farming.Water(plot.Model);break;
            case PlantTool.Harvest:success=Farming.Harvest(plot.Model);break;
            case PlantTool.YieldFertilizer:success=Farming.Fertilize(plot.Model,ItemType.YieldBoostFertilizer);break;
            case PlantTool.GrowthFertilizer:success=Farming.Fertilize(plot.Model,ItemType.GrowthBoostFertilizer);break;
            default:return false;
        }
        // Controller chỉ thông báo kết quả. Luật và việc trừ/cộng kho nằm trong FarmingService.
        Notice=plot.name+" · "+(success?"Đã thực hiện.":"Chưa đủ điều kiện hoặc thiếu vật phẩm.");
        return success;
    }
    public void CloseTools() { Selected=null;ActiveTool=null; }
    public void OpenWarehouse(bool open) { WarehouseOpen=open;_hasPointer=false; }
    private void RefreshPlots()
    {
        foreach(var plot in _plots)plot.Refresh(plot==Selected);
    }

    private void HandlePointer()
    {
        var mouse=Mouse.current;if(mouse==null)return;
        Vector2 screen=mouse.position.ReadValue();
        if(mouse.leftButton.wasPressedThisFrame) { _visited.Clear();_hasPointer=false; }
        if(WarehouseOpen || !_mapCamera.pixelRect.Contains(screen) || _view.Covers(screen)) { _hasPointer=false;return; }
        Vector2 point=_mapCamera.ScreenToWorldPoint(screen);
        if(mouse.leftButton.wasPressedThisFrame && _warehouse.bounds.Contains(new Vector3(point.x,point.y,0)))
        { OpenWarehouse(true);return; }
        if(!mouse.leftButton.isPressed) { _hasPointer=false;return; }
        // Nội suy cả đường chuột để rê nhanh không bỏ qua ô đất. Mỗi ô chỉ nhận một lần trong một lượt rê.
        Vector2 from=_hasPointer?_lastPointer:point;
        int steps=Mathf.Max(1,Mathf.CeilToInt(Vector2.Distance(from,point)/.12f));
        bool single=ActiveTool==PlantTool.YieldFertilizer || ActiveTool==PlantTool.GrowthFertilizer;
        if(!single || mouse.leftButton.wasPressedThisFrame)
            for(int i=0;i<=steps;i++)
                foreach(var collider in Physics2D.OverlapPointAll(Vector2.Lerp(from,point,i/(float)steps)))
                {
                    var plot=collider.GetComponent<FarmPlotController>();
                    if(!plot || Array.IndexOf(_plots,plot)<0 || !_visited.Add(plot))continue;
                    Select(plot);if(ActiveTool.HasValue)Work(plot,ActiveTool.Value);
                }
        _lastPointer=point;_hasPointer=true;
    }

    public static string StateText(FarmPlot plot)
    {
        switch(plot.Status)
        {
            case FarmPlotStatus.Untilled:return "Đất chưa xới";
            case FarmPlotStatus.Tilling:return "Đang xới · còn "+GameClock.FormatDuration(plot.TillingTimeRemaining);
            case FarmPlotStatus.ReadyToSow:return "Sẵn sàng gieo";
            case FarmPlotStatus.HaveResidue:return plot.ResidueType==ResidueType.RottenCrop?"Cây chết · xới để dọn":"Còn tàn dư · xới để dọn";
            default:
                var crop=plot.CurrentCrop;
                if(crop==null)return "Đất trống";
                return crop.Definition.Name+" · "+(crop.CanHarvest()?"Đã chín":crop.CanWater && crop.WateringCount==0?"Cần tưới":"Còn "+GameClock.FormatDuration(crop.GrowthTimeRemaining));
        }
    }
}
