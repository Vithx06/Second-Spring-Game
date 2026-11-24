using SecondSpring.Prototype;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VinhPrototypeBuilder
{
    const string Art="Assets/VinhPrototype/Art/Farm/";
    const string ScenePath="Assets/VinhPrototype/Scenes/vinh_prototype.unity";
    [Serializable] class Placement { public string id,group; public int x,y,width,height; }
    [Serializable] class Map { public Placement[] layers; }

    static readonly Dictionary<string,string> Names=new Dictionary<string,string> {
        {"horizon","Chân trời"},{"meadow","Đường đi và thảm cỏ"},{"windmill","Cối xay gió"},{"storehouse","Nhà kho"},
        {"workshop","Xưởng chế biến"},{"house","Nhà chính"},{"duck","Chuồng vịt"},{"chicken","Chuồng gà"},{"cow","Chuồng bò"},
        {"pig","Chuồng lợn"},{"pond","Ao cá"},{"gate","Cổng nông trại"},{"market","Chợ nông sản"},{"garden","Vườn hoa"},{"fountain","Đài phun nước"}
    };

    [MenuItem("Second Spring/Dựng lại cảnh nông trại")]
    public static void Build()
    {
        if(!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var camera=new GameObject("Máy ảnh").AddComponent<Camera>();
        camera.tag="MainCamera"; camera.orthographic=true; camera.orthographicSize=4.705f;
        camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=new Color32(28,32,28,255);
        camera.transform.position=new Vector3(0,0,-10);
        var view=new GameObject("Nông trại").AddComponent<VinhPrototypeView>();
        view.mapCamera=camera;
        var environment=new GameObject("Phong cảnh").transform;
        var buildings=new GameObject("Nhà và khu nuôi").transform;
        var scenery=new GameObject("Trang trí và khu vực của Vũ").transform;
        var layers=new List<SpriteRenderer>();
        var objects=new Dictionary<string,SpriteRenderer>();
        var map=JsonUtility.FromJson<Map>(AssetDatabase.LoadAssetAtPath<TextAsset>(Art+"positions.json").text);
        foreach(var item in map.layers)
        {
            string name=Names.TryGetValue(item.id,out string vn)?vn:"Luống đất "+item.id.Substring(5);
            var parent=item.group=="Ground"?environment:item.group=="Buildings"?buildings:scenery;
            var sr=Create(name,item.id,parent,layers.Count*2);
            sr.transform.position=new Vector3((item.x+item.width*.5f-836)/100,(470.5f-item.y-item.height*.5f)/100,0);
            objects.Add(item.id,sr);layers.Add(sr);
        }
        view.house=objects["house"];view.pond=objects["pond"];
        var horizon=objects["horizon"];
        view.nightSky=Create("Trời đêm","night",environment,horizon.sortingOrder+1);
        view.nightSky.transform.position=horizon.transform.position;
        view.nightSky.color=Color.clear;
        string[] ids={"chicken","duck","pig","cow"};
        view.barns=Array.ConvertAll(ids,id=>objects[id]);
        view.empty=Array.ConvertAll(ids,LoadSprite);
        view.occupied=Array.ConvertAll(ids,id=>LoadSprite(id+"_full"));
        for(int i=0;i<ids.Length;i++) view.barns[i].sprite=view.occupied[i];
        view.swimmers=new SpriteRenderer[4];
        for(int i=0;i<4;i++)
        {
            var fish=Create("Cá bơi "+(i+1),"swimmer",view.pond.transform,80+i);
            fish.transform.localPosition=new Vector3(-.18f+i*.14f,.15f+i*.035f,0);
            fish.transform.localScale=Vector3.one*.47f;fish.enabled=i<3;view.swimmers[i]=fish;
        }
        view.world=layers.ToArray();
        view.frame=LoadTexture("parchment");view.calendar=LoadTexture("calendar");view.dial=LoadTexture("clock");
        view.hole=LoadTexture("hole");view.moleArt=LoadTexture("mole");
        if(!EditorSceneManager.SaveScene(scene,ScenePath)) throw new Exception("Không lưu được scene.");
        AssetDatabase.SaveAssets();
    }

    static SpriteRenderer Create(string name,string asset,Transform parent,int order)
    {
        var sr=new GameObject(name).AddComponent<SpriteRenderer>();
        sr.transform.SetParent(parent,false);sr.sortingOrder=order;sr.sprite=LoadSprite(asset);
        return sr;
    }
    static Sprite LoadSprite(string id)
    {
        var result=AssetDatabase.LoadAssetAtPath<Sprite>(Art+id+".png");
        if(!result) throw new Exception("Thiếu sprite "+id);
        return result;
    }
    static Texture2D LoadTexture(string id) => LoadSprite(id).texture;
 public static void TestRules() {
  var f=new Farm(); if(f.Day!=1||f.Minute!=420) throw new System.Exception("Clock start");
  f.Add(0,Species.Chicken); if(!f.FeedPen(0)||f.FeedPen(0)||f.Feed[0]!=19)throw new System.Exception("Feeding");
  f.Advance(212.5); if(f.Day!=2||f.Pens[0].Animals[0].Growth!=1)throw new System.Exception("Day rollover");
  f.Advance(300); if(f.Pens[0].Animals[0].Growth!=1||f.Pens[0].Animals[0].Age!=2)throw new System.Exception("Missed feeding");
  for(int i=0;i<30;i++)if(!f.Add(4,Species.Perch))throw new System.Exception("Fish capacity too small");
  if(f.Add(4,Species.Perch))throw new System.Exception("Fish capacity exceeded");
  Debug.Log("VINH_PROTOTYPE_TESTS_PASS");
 }
}
