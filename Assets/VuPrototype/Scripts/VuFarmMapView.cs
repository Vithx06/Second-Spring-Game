using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public partial class VuFarmMapView : MonoBehaviour
{
    internal static GameClock Clock;
    internal static PlantDemoInventory Inventory;
    internal static FarmPlotView Selected;
    internal static bool IsDragging;
    internal static PlantToolIcons.Kind DragTool;
    internal static int DragSeed;
    private Camera _camera;
    private static readonly HashSet<FarmPlotView> VisitedPlots=new HashSet<FarmPlotView>();
    private static Vector2 _lastStrokePoint;
    private static bool _hasStrokePoint;
    private FarmPlotView[] _plots;
    private static bool IsSingleDrop => DragTool==PlantToolIcons.Kind.Yield || DragTool==PlantToolIcons.Kind.Growth;
    internal static void BeginDrag(PlantToolIcons.Kind tool,int seed)
    {
        CancelDrag();
        DragTool=tool; DragSeed=seed; IsDragging=true;
        Event.current.Use();
    }
    private static void CancelDrag()
    {
        IsDragging=false;
        VisitedPlots.Clear();
        _hasStrokePoint=false;
    }
    private void OnApplicationFocus(bool focused) { if(!focused) CancelDrag(); }
    private void OnApplicationPause(bool paused) { if(paused) CancelDrag(); }
    private void OnDisable() { CancelDrag(); }

    private void Awake() { Clock=new GameClock(); Inventory=new PlantDemoInventory(); InitializeMarket(); Selected=null; CancelDrag(); _camera=Camera.main; _plots=FindObjectsByType<FarmPlotView>(); }
    private void Update()
    {
        Clock.Advance(UnityEngine.Time.deltaTime);
        if(_camera!=null) _camera.orthographicSize=Mathf.Max(4.705f,8.36f/_camera.aspect);
    }
    private void OnGUI()
    {
        Matrix4x4 previous=GUI.matrix;
        int previousDepth=GUI.depth;
        GUI.depth=-100;
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/721f);
        GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1280*scale)/2,(Screen.height-721*scale)/2,0),Quaternion.identity,new Vector3(scale,scale,1));
        GUI.Box(new Rect(16,12,455,56),"NGÀY "+Clock.Day+"   ·   "+Clock.Hour.ToString("00")+":"+Clock.Minute.ToString("00")+"\n1 ngày game = 5 phút thực · đồng hồ chung");
        DrawMarketInterface();
        if(IsDragging)
        {
            Event e=Event.current;
            FarmPlotView target=null;
            if(_camera!=null)
            {
                // The toolbar stays hidden so every soil cell can receive the gesture.
                foreach(var plot in _plots)
                    if(plot!=null && plot.MapContains(e.mousePosition,GUI.matrix,scale,_camera)) { target=plot;break; }
            }
            if(e.type==EventType.KeyDown && e.keyCode==KeyCode.Escape)
            { CancelDrag();e.Use(); }
            else if(e.type==EventType.MouseUp && e.button==0)
            {
                if(IsSingleDrop)
                {
                    if(target!=null) { Selected=target;target.ApplyMapDrop(DragTool,DragSeed); }
                }
                else SweepTo(e.mousePosition,GUI.matrix,scale);
                CancelDrag();e.Use();
            }
            else
            {
                if(e.type==EventType.MouseDrag && e.button==0)
                {
                    if(!IsSingleDrop) SweepTo(e.mousePosition,GUI.matrix,scale);
                    e.Use();
                }
                Rect ghost=new Rect(e.mousePosition.x-24,e.mousePosition.y-38,48,48);
                if(DragTool==PlantToolIcons.Kind.Seed)
                    PlantItemSprites.DrawSeed(ghost,DragSeed);
                else GUI.DrawTexture(ghost,PlantToolIcons.Get(DragTool));
                GUI.Box(new Rect(e.mousePosition.x+20,e.mousePosition.y+12,220,26),
                    IsSingleDrop ? (target==null ? "Thả phân lên một ô đất" : "Thả để bón phân")
                    : "Rê qua đất · nhả để dừng · Esc dừng");
            }
        }
        GUI.matrix=previous;
        GUI.depth=previousDepth;
    }
    private void SweepTo(Vector2 point,Matrix4x4 matrix,float scale)
    {
        if(_camera==null) return;
        // Start at the first drag position, not the hidden soil behind the source toolbar.
        Vector2 from=_hasStrokePoint ? _lastStrokePoint : point;
        int steps=Mathf.Max(1,Mathf.CeilToInt(Vector2.Distance(from,point)/4f));
        for(int step=0;step<=steps;step++)
        {
            Vector2 sample=Vector2.Lerp(from,point,step/(float)steps);
            foreach(var plot in _plots)
            {
                if(plot==null || VisitedPlots.Contains(plot) || !plot.MapContains(sample,matrix,scale,_camera)) continue;
                // Record even an invalid attempt: a growing plot must not receive repeated
                // watering, or become newly eligible and get processed twice in one stroke.
                VisitedPlots.Add(plot);
                Selected=plot;
                plot.ApplyMapDrop(DragTool,DragSeed);
            }
        }
        _lastStrokePoint=point;
        _hasStrokePoint=true;
    }
    private void OnDestroy() { Selected=null; Inventory=null; Clock=null; IsMenuOpen=false; CancelDrag(); }
}
