#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlantSceneSetup
{
    [MenuItem("Second Spring/Plants/Connect Plants And Inventory")]
    public static void Connect()
    {
        if(Application.isPlaying)throw new InvalidOperationException("Stop Play first.");
        var scene=Application.isBatchMode?EditorSceneManager.OpenScene("Assets/Scenes/MainFarm.unity",OpenSceneMode.Single):SceneManager.GetActiveScene();
        if(scene.path!="Assets/Scenes/MainFarm.unity")throw new InvalidOperationException("Open MainFarm first.");
        var renderers=scene.GetRootGameObjects().SelectMany(root=>root.GetComponentsInChildren<SpriteRenderer>(true)).ToArray();
        var soils=renderers.Where(sr=>sr.sprite && sr.sprite.name.StartsWith("Soil_")).OrderBy(sr=>sr.name,StringComparer.Ordinal).ToArray();
        if(soils.Length!=35)throw new Exception("Expected 35 soil objects; layout was not changed.");
        var systems=scene.GetRootGameObjects().SelectMany(root=>root.GetComponentsInChildren<PlantSystemController>(true)).ToArray();
        if(systems.Length>1)throw new Exception("Duplicate plant systems.");
        var system=systems.Length==1?systems[0]:new GameObject("05 · Trồng trọt và kho").AddComponent<PlantSystemController>();
        var view=system.GetComponent<PlantSystemView>();
        var plots=new FarmPlotController[soils.Length];
        for(int i=0;i<soils.Length;i++)
        {
            var soil=soils[i];var plot=soil.GetComponent<FarmPlotController>();if(!plot)plot=soil.gameObject.AddComponent<FarmPlotController>();
            var visual=soil.GetComponent<PlantPlotView>();if(!visual)visual=soil.gameObject.AddComponent<PlantPlotView>();
            var crop=soil.GetComponentsInChildren<SpriteRenderer>(true).FirstOrDefault(sr=>sr!=soil);
            if(!crop)
            {
                crop=new GameObject("Cây trồng").AddComponent<SpriteRenderer>();crop.transform.SetParent(soil.transform,false);
                crop.transform.localPosition=new Vector3(0,-.15f,0);crop.transform.localScale=Vector3.one*.9f;
            }
            crop.sortingLayerID=soil.sortingLayerID;crop.sortingOrder=soil.sortingOrder+1;crop.enabled=false;
            var plotData=new SerializedObject(plot);plotData.FindProperty("_plotId").intValue=i+1;
            plotData.FindProperty("_tillingDuration").floatValue=600;plotData.ApplyModifiedPropertiesWithoutUndo();
            var visualData=new SerializedObject(visual);visualData.FindProperty("_soil").objectReferenceValue=soil;
            visualData.FindProperty("_crop").objectReferenceValue=crop;visualData.ApplyModifiedPropertiesWithoutUndo();plots[i]=plot;
        }
        var data=new SerializedObject(system);
        data.FindProperty("_mapCamera").objectReferenceValue=scene.GetRootGameObjects().SelectMany(root=>root.GetComponentsInChildren<Camera>(true)).Single(c=>c.CompareTag("MainCamera"));
        data.FindProperty("_warehouse").objectReferenceValue=renderers.Single(sr=>sr.name=="Nhà kho");
        var array=data.FindProperty("_plots");array.arraySize=plots.Length;for(int i=0;i<plots.Length;i++)array.GetArrayElementAtIndex(i).objectReferenceValue=plots[i];
        data.ApplyModifiedPropertiesWithoutUndo();
        var art=new SerializedObject(view);
        Assign(art.FindProperty("_soilAndCropSprites"),"Assets/Art/Farm/Plants/CropAndSoilSheet.png");
        Assign(art.FindProperty("_itemSprites"),"Assets/Art/Farm/Items/SeedAndProductSheet.png");
        art.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.MarkSceneDirty(scene);if(!EditorSceneManager.SaveScene(scene))throw new Exception("Could not save plant scene.");
        Debug.Log("PLANTS_INVENTORY_CONNECTED: 35 plot IDs, shared typed inventory and clock; manual scene positions preserved.");
    }
    private static void Assign(SerializedProperty property,string path)
    {
        var sprites=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        if(sprites.Length==0)throw new Exception("Missing sprite atlas: "+path);
        property.arraySize=sprites.Length;for(int i=0;i<sprites.Length;i++)property.GetArrayElementAtIndex(i).objectReferenceValue=sprites[i];
    }
}
#endif
