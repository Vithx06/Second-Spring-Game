#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class VuFarmSceneBuilder
{
    private const string PlotRoot="Ô đất tương tác của Vũ";
    [MenuItem("Plants/Create Vu Farm From Vinh Map")]
    public static void Create()
    {
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        string target=AssetDatabase.GenerateUniqueAssetPath("Assets/VuPrototype/Scenes/vu_farm.unity");
        if(!AssetDatabase.CopyAsset("Assets/VinhPrototype/Scenes/vinh_prototype.unity",target)) throw new Exception("Cannot copy Vinh scene.");
        var scene=EditorSceneManager.OpenScene(target,OpenSceneMode.Single);
        foreach(var root in scene.GetRootGameObjects())
            foreach(var script in root.GetComponentsInChildren<MonoBehaviour>(true))
                if(script.GetType().Name=="VinhPrototypeView") script.enabled=false;
        new GameObject("Vu Farm Controller").AddComponent<VuFarmMapView>();
        BuildPlots(scene);
        EditorSceneManager.SaveScene(scene,target);
    }
    [MenuItem("Plants/Rebuild Vu Farm Plots - 35 Tiles")]
    public static void Rebuild()
    {
        if(Application.isPlaying) throw new InvalidOperationException("Stop Play before rebuilding plots.");
        var scene=SceneManager.GetActiveScene();
        if(!scene.path.StartsWith("Assets/VuPrototype/Scenes/")) throw new InvalidOperationException("Open the Vu farm scene first.");
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        BuildPlots(scene);
        EditorSceneManager.SaveScene(scene);
    }
    private static void BuildPlots(Scene scene)
    {
        foreach(var root in scene.GetRootGameObjects())
        {
            if(root.name==PlotRoot) { Undo.DestroyObjectImmediate(root);continue; }
            foreach(var renderer in root.GetComponentsInChildren<SpriteRenderer>(true))
                if(renderer.name.StartsWith("Luống đất "))
                {
                    Undo.RecordObject(renderer,"Hide old plot artwork");
                    renderer.enabled=false;
                }
        }
        var parent=new GameObject(PlotRoot).transform;
        Undo.RegisterCreatedObjectUndo(parent.gameObject,"Create 35 plant plots");
        int count=0;
        // Source map is 1672 x 941 pixels, at 100 pixels per world unit.
        // Fit each patch inside its green boundary, leaving a thin grass seam.
        Patch(parent,new Rect(270,348,405,267),5,3,ref count);
        Patch(parent,new Rect(1364,345,274,289),3,3,ref count);
        Patch(parent,new Rect(298,645,380,102),4,1,ref count);
        Patch(parent,new Rect(990,667,621,101),7,1,ref count);
        if(count!=35) throw new InvalidOperationException("Expected exactly 35 plots.");
        Selection.activeGameObject=parent.gameObject;
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Vu farm: 35 fitted plots; old plot artwork hidden.");
    }
    private static void Patch(Transform parent,Rect area,int columns,int rows,ref int number)
    {
        float width=area.width/columns;
        float height=area.height/rows;
        float left=area.x+(area.width-columns*width)/2;
        float top=area.y+(area.height-rows*height)/2;
        for(int row=0;row<rows;row++) for(int col=0;col<columns;col++)
        {
            var go=new GameObject("Ô đất "+(++number).ToString("00"));
            go.transform.SetParent(parent,false);
            float x=left+(col+0.5f)*width;
            float y=top+(row+0.5f)*height;
            go.transform.position=new Vector3((x-836)/100f,(470.5f-y)/100f,-0.05f);
            go.transform.localScale=new Vector3((width-3)/100f*128f/125f,(height-3)/100f*128f/125f,1);
            var renderer=go.AddComponent<SpriteRenderer>();renderer.sortingOrder=150+Mathf.RoundToInt(y);
            var collider=go.AddComponent<BoxCollider2D>();collider.size=Vector2.one*(125f/128f);
            go.AddComponent<FarmPlotView>().ConfigureMapMode(go.name);
        }
    }
}
#endif
