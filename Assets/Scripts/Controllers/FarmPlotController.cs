using UnityEngine;
using UnityEngine.InputSystem;

// Component này nối một GameObject trong Unity với đúng một FarmPlot trong Model.
[RequireComponent(typeof(BoxCollider2D))]
public class FarmPlotController : MonoBehaviour
{
    [SerializeField] private int _plotId=1;
    [SerializeField] private float _tillingDuration=600f; // Giây game: 10 phút game, khoảng 2,08 giây thực.
    private Camera _camera;
    private BoxCollider2D _collider;
    private PlantSystemController _system;
    private PlantPlotView _view;
    public FarmPlot Model { get; private set; }

    private void Awake()
    {
        _camera=Camera.main;
        _collider=GetComponent<BoxCollider2D>();
    }
    public void Initialize(PlantSystemController system,PlantSystemView art)
    {
        _system=system;
        Model=new FarmPlot(_plotId,_tillingDuration);
        _view=GetComponent<PlantPlotView>();
        _view.Initialize(art);
    }
    public void Refresh(bool selected) { _view.Render(Model,selected); }

    private void Update()
    {
        // Khi đã nối hệ thống chung, Controller chung xử lý click/rê để không gọi trùng.
        if(_system)return;
        // Giữ bài thử click ban đầu khi kéo component này sang một scene chưa nối hệ thống.
        if(Mouse.current==null || !_camera || !Mouse.current.leftButton.wasPressedThisFrame)return;
        Vector2 mousePosition=Mouse.current.position.ReadValue();
        if(!_camera.pixelRect.Contains(mousePosition))return;
        Vector2 worldPosition=_camera.ScreenToWorldPoint(mousePosition);
        if(_collider.OverlapPoint(worldPosition))Debug.Log("Đã chọn: "+gameObject.name);
    }
}
