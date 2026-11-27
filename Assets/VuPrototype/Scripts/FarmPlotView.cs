using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public partial class FarmPlotView : MonoBehaviour
{
    [Header("Prototype tilling and rot settings; crop durations use CropGameRules")]
    [SerializeField, Min(0.01f)] private float _tillingGameHours = 1f/6f;
    [SerializeField, Min(0.01f)] private float _rotGameHours = 24f;
    [SerializeField] private bool _showDemoPanel = true;

    private FarmPlot _plot;
    private FarmingService _service;
    private PlantDemoInventory _inventory;
    private SpriteRenderer _spriteRenderer;
    private CropDefinition[] _definitions;
    private int _selectedCrop;
    private string _message = "Bắt đầu bằng cách xới đất, rồi chọn giống để gieo.";
    private GameClock _clock;
    private bool _ownsClock;
    private Sprite _placeholderSprite;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer.sprite == null)
        {
            _placeholderSprite = Sprite.Create(Texture2D.whiteTexture,
                new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                new Vector2(0.5f, 0.5f), Texture2D.whiteTexture.width);
            _spriteRenderer.sprite = _placeholderSprite;
        }
        _plot = new FarmPlot(Mathf.Round(_tillingGameHours * 3600f));
        _inventory = _mapMode && VuFarmMapView.Inventory != null ? VuFarmMapView.Inventory : new PlantDemoInventory();
        _definitions = CropGameRules.CreateDefinitions(_rotGameHours*3600f);
        _ownsClock=!_mapMode || VuFarmMapView.Clock==null;
        _clock=_ownsClock ? new GameClock() : VuFarmMapView.Clock;
        foreach (var definition in _definitions)
        {
            var seed = new ItemDefinition(definition.SeedItemId, definition.Name + " - giong");
            if (_inventory.FindItem(seed.Id) != null) continue;
            _inventory.Register(seed);
            _inventory.Register(new ItemDefinition(definition.ProductItemId, definition.Name));
            _inventory.TryAdd(seed, 10);
        }
        foreach (string id in new[] { "fertilizer.yield", "fertilizer.growth" })
        {
            if (_inventory.FindItem(id) != null) continue;
            var item = new ItemDefinition(id, id);
            _inventory.Register(item);
            _inventory.TryAdd(item, 10);
        }
        _service = new FarmingService(_inventory, new[] { _plot }, _inventory,
            new ICropCreator[] { new SingleHarvestCropCreator(), new RegrowingCropCreator() },
            FertilizerDefinition.CreateDefaults());
        RefreshColor();
    }

    private void Update()
    {
        if (_ownsClock) _clock.Advance(Time.deltaTime);
        RefreshColor();
    }

    private void OnEnable() { if(_clock!=null && _service!=null)_clock.Subscribe(_service); }
    private void OnDisable() { if(_clock!=null && _service!=null)_clock.Unsubscribe(_service); }

    private void HandlePlotClick()
    {
        // IMGUI events work in this demo without depending on either Input backend.
        Event click = Event.current;
        if (_service == null || click.type != EventType.MouseDown || click.button != 0) return;
        if (_showDemoPanel && new Rect(10,10,340,580).Contains(click.mousePosition)) return;
        Camera camera = Camera.main;
        if (camera == null) return;
        var screenPoint = new Vector3(click.mousePosition.x, Screen.height-click.mousePosition.y, 0);
        var hit = Physics2D.GetRayIntersection(camera.ScreenPointToRay(screenPoint));
        if (hit.collider == null || hit.collider.gameObject != gameObject) return;
        if (_plot.CanTill()) Run(_service.Till(_plot));
        else if (_plot.CanPlant()) Run(_service.Plant(_plot, _definitions[_selectedCrop]));
        else if (_plot.CurrentCrop != null && _plot.CurrentCrop.CanHarvest()) Run(_service.Harvest(_plot));
        else Run(_service.Water(_plot));
        click.Use();
    }

    private void Run(bool result)
    {
        _message = result ? "Đã thực hiện thành công." : "Chưa thực hiện được. Kiểm tra trạng thái cây, vật phẩm và chỗ trống trong kho.";
        RefreshColor();
    }

    private void RefreshColor()
    {
        if (_plot.Status == FarmPlotStatus.Tilling) _spriteRenderer.color = Color.yellow;
        else if (_plot.Status == FarmPlotStatus.ReadyToSow) _spriteRenderer.color = new Color32(180,120,60,255);
        else if (_plot.Status == FarmPlotStatus.HaveResidue)
            _spriteRenderer.color = _plot.ResidueType == ResidueType.RottenCrop ? Color.black : Color.gray;
        else if (_plot.CurrentCrop != null)
            _spriteRenderer.color = _plot.CurrentCrop.State == CropState.Ripe ? new Color32(255,180,30,255)
                : _plot.CurrentCrop.State == CropState.Stage1 ? new Color32(100,210,80,255) : new Color32(30,140,50,255);
        else _spriteRenderer.color = new Color32(110,70,35,255);
        RefreshPixelSprites();
    }

    private void OnGUI()
    {
        if (_service == null) return;
        if (_mapMode) DrawMapInterface();
        else if (_showDemoPanel) DrawPlantInterface();
        else HandlePlotClick();
    }

    private void OnDestroy()
    {
        if (_placeholderSprite != null) Destroy(_placeholderSprite);
        if (_interfaceFont != null) Destroy(_interfaceFont);
    }
}

