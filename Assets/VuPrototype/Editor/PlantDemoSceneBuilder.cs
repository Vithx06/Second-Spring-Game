#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlantDemoSceneBuilder
{
    [InitializeOnLoadMethod]
    private static void ConfigurePixelAtlas()
    {
        EditorApplication.delayCall += () =>
        {
            const string path = "Assets/VuPrototype/Art/Resources/PlantArt/PixelFarmAtlas.png";
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            if (importer.filterMode == FilterMode.Point && !importer.mipmapEnabled
                && importer.textureCompression == TextureImporterCompression.Uncompressed
                && importer.alphaIsTransparency && importer.npotScale == TextureImporterNPOTScale.None) return;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
        };
    }

    [MenuItem("Plants/Create Plant Demo Scene")]
    public static void CreateScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0, 0, -10);
        var camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 4;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(75, 100, 75, 255);
        cameraObject.AddComponent<AudioListener>();
        var plot = new GameObject("FarmPlotView");
        plot.transform.position = new Vector3(1.5f, 0, 0);
        plot.AddComponent<SpriteRenderer>();
        plot.AddComponent<BoxCollider2D>();
        plot.AddComponent<FarmPlotView>();
        if (!AssetDatabase.IsValidFolder("Assets/VuPrototype/Scenes")) AssetDatabase.CreateFolder("Assets/VuPrototype", "Scenes");
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/VuPrototype/Scenes/PlantDemo.unity");
        EditorSceneManager.SaveScene(scene, path);
        Selection.activeGameObject = plot;
        Debug.Log("Plant demo created: " + path + ". Press Play to test.");
    }
}
#endif
