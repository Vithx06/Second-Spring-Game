using System;
using SecondSpring.Prototype;
using UnityEngine;
using Random = UnityEngine.Random;

public class VinhPrototypeView : MonoBehaviour
{
    public Camera mapCamera;
    public SpriteRenderer house, pond, nightSky;
    public SpriteRenderer[] barns, swimmers;
    public Sprite[] occupied, empty;
    public Texture2D frame, calendar, dial, hole, moleArt;
    public SpriteRenderer[] world;
    public AudioClip[] musicTracks;
    public AudioClip clickSound, openSound, closeSound;
    public bool enableClickSounds = false;
    [Range(0f, 1f)] public float musicVolume = .25f, effectVolume = .45f;

    AudioSource musicSource, effectSource;
    PrototypeScenery scenery;
    readonly System.Random musicRandom = new System.Random();
    readonly System.Collections.Generic.List<AudioClip> playlist = new System.Collections.Generic.List<AudioClip>();
    AudioClip lastTrack;
    AudioClip loadingTrack;
    int nextTrack;

    readonly Farm farm = new Farm();
    static readonly string[] PenNames = { "Chuồng gà", "Chuồng vịt", "Chuồng lợn", "Chuồng bò", "Ao cá" };
    static readonly string[] FishNames = { "cá rô", "cá chép", "cá tầm" };
    static readonly string[] CatchNames = { "cá rô", "cá chép", "cá tầm", "lon nhựa", "giày cũ", "quần rách" };
    static readonly int[] Weights = { 35, 25, 10, 10, 10, 10 };
    enum Page { Farm, Pen, House, Games, Invitation, Fishing, Mole }
    Page page;
    int houseTab;
    int penIndex, lastHour, invitation, catchIndex, moleIndex, score, lives;
    float marker, timeLeft, nextMole;
    int clockSpeed = 1;
    bool finished;
    string message = "";
    GUIStyle heading, body, fine, centered, plaque, button, digits;
    Texture2D feedingBubble;
    SpriteRenderer[,] animalIcons = new SpriteRenderer[4,2];
    Texture2D[,] animalArt = new Texture2D[4,2];
    Texture2D[] fishArt = new Texture2D[3];
    Texture2D[] feedArt = new Texture2D[3];
    Vector2 animalListScroll;
    float scale;
    Vector2 origin;
    readonly Color ink = new Color32(61,35,20,255);
    static readonly Rect Window = new Rect(274,156,732,406);

    void Start()
    {
        SetupAudio();
        SetupAnimalArt();
        scenery=gameObject.AddComponent<PrototypeScenery>();
        scenery.Initialize(world, nightSky);
        lastHour=7;
    }

    void SetupAnimalArt()
    {
        string[] names={"chicken","duck","pig","cow"};
        for(int i=0;i<names.Length;i++)
        {
            var barn=barns[i];
            Vector2 size=barn.sprite.bounds.size;
            var texture=Resources.Load<Texture2D>("Animals/"+names[i]+"_pen");
            if(!texture) continue;
            barn.sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),texture.width/size.x);
            size=barn.sprite.bounds.size;
            for(int j=0;j<2;j++)
            {
                var animal=Resources.Load<Texture2D>("Animals/"+names[i]+(j==0?"_adult":"_young"));
                animalArt[i,j]=animal;
                var icon=new GameObject(j==0?"Adult":"Young").AddComponent<SpriteRenderer>();
                icon.transform.SetParent(barn.transform,false);
                icon.sortingOrder=barn.sortingOrder+1+j;
                float animalWidth=i>=2?(j==0?.20f:.14f):(j==0?.17f:.10f);
                if(animal) icon.sprite=Sprite.Create(animal,new Rect(0,0,animal.width,animal.height),new Vector2(.5f,.5f),animal.width/(size.x*animalWidth));
                icon.transform.localPosition=new Vector3(size.x*(i==1?(j==0?-.16f:.15f):(j==0?-.31f:-.16f)),size.y*(i==1?(j==0?-.13f:-.19f):(j==0?.04f:0f)),0);
                icon.enabled=false;
                animalIcons[i,j]=icon;
            }
        }
        feedArt[0]=Resources.Load<Texture2D>("Animals/feed_poultry");
        feedArt[1]=Resources.Load<Texture2D>("Animals/feed_livestock");
        feedArt[2]=Resources.Load<Texture2D>("Animals/feed_fish");
        fishArt[0]=Resources.Load<Texture2D>("Animals/fish_perch");
        fishArt[1]=Resources.Load<Texture2D>("Animals/fish_carp");
        fishArt[2]=Resources.Load<Texture2D>("Animals/fish_sturgeon");
    }

    void Update()
    {
        UpdateMusic();
        farm.Advance(Time.deltaTime * clockSpeed);
        int hour=(int)farm.Minute/60;
        if(hour!=lastHour && page==Page.Farm && Random.value<.2f)
        {
            invitation=Random.Range(0,2);
            page=Page.Invitation;
        }
        lastHour=hour;
        mapCamera.orthographicSize=Mathf.Max(4.705f,8.36f/mapCamera.aspect);
        UpdateMap();
        if(finished) return;
        if(page==Page.Fishing)
        {
            float speed=catchIndex==2?1.35f:catchIndex==1?1f:.7f;
            marker=Mathf.PingPong(Time.time*speed,1);
        }
        if(page!=Page.Mole) return;
        timeLeft-=Time.deltaTime;
        if(timeLeft<=0) { FinishMole(); return; }
        if(Time.time<nextMole) return;
        if(moleIndex>=0 && --lives==0) { FinishMole(); return; }
        moleIndex=Random.Range(0,9);
        nextMole=Time.time+1.2f;
    }

    void UpdateMap()
    {
        float hour=(float)farm.Minute/60;
        float daylight=Mathf.SmoothStep(0,1,Mathf.InverseLerp(5,7,hour))*(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(17,19,hour)));
        Color shade=Color.Lerp(new Color(.62f,.66f,.78f),Color.white,daylight);
        shade=Color.Lerp(shade,new Color(1,.70f,.52f),.60f*4*daylight*(1-daylight));
        foreach(var layer in world) layer.color=shade;
        if (scenery) scenery.Animate(shade, daylight);
        nightSky.color=new Color(1,1,1,1-daylight);
        for(int i=0;i<4;i++)
        {
            bool adult=farm.Pens[i].Animals.Exists(a=>a.Adult);
            bool young=farm.Pens[i].Animals.Exists(a=>!a.Adult);
            if (!animalIcons[i,0] || !animalIcons[i,1]) continue;
            animalIcons[i,0].enabled=adult;
            animalIcons[i,1].enabled=young;
            for(int j=0;j<2;j++) animalIcons[i,j].color=shade;
        }
        for(int i=0;i<swimmers.Length;i++)
        {
            var fish=swimmers[i];
            fish.enabled=i<3 && farm.Pens[4].Animals.Count>0;
            if(!fish.enabled) continue;
            float phase=Time.time*(.70f+i*.09f)+i*2;
            fish.transform.localPosition=new Vector3((10+30*Mathf.Cos(phase))/100,(18+10*Mathf.Sin(phase)+i*4)/100,0);
            fish.flipX=Mathf.Sin(phase)>0;
            fish.color=new Color(shade.r,shade.g,shade.b,.5f);
        }
    }

    void MakeStyles()
    {
        if(body!=null) return;
        body=new GUIStyle(GUI.skin.label){fontSize=17,wordWrap=true};
        body.normal.textColor=ink;
        heading=new GUIStyle(body){fontSize=27,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleLeft};
        fine=new GUIStyle(body){fontSize=14};
        centered=new GUIStyle(body){alignment=TextAnchor.MiddleCenter,fontStyle=FontStyle.Bold};
        digits=new GUIStyle(centered){fontSize=19};
        plaque=new GUIStyle {border=new RectOffset(18,18,11,11)};
        plaque.normal.background=frame;
        button=new GUIStyle(plaque){alignment=TextAnchor.MiddleCenter,fontSize=16,fontStyle=FontStyle.Bold,wordWrap=true};
        button.normal.textColor=ink;
        button.hover.background=frame; button.hover.textColor=new Color(.43f,.25f,.06f);
        button.active.background=frame; button.active.textColor=Color.black;
        feedingBubble=CreateFeedingBubble();
    }
    static Texture2D CreateFeedingBubble()
    {
        const int size=128;
        var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);
        texture.name="Thông báo cho ăn";
        texture.filterMode=FilterMode.Bilinear;
        texture.wrapMode=TextureWrapMode.Clamp;
        var pixels=new Color[size*size];
        // Supersampled circle and lower-right tail, matching a speech-bubble icon.
        for(int y=0;y<size;y++) for(int x=0;x<size;x++)
        {
            Color sum=Color.clear;
            for(int sy=0;sy<2;sy++) for(int sx=0;sx<2;sx++)
            {
                Vector2 p=new Vector2(x+(sx+.5f)/2,y+(sy+.5f)/2);
                float radius=Vector2.Distance(p,new Vector2(59,68));
                bool outer=radius<49 || InTriangle(p,new Vector2(83,40),new Vector2(115,9),new Vector2(68,24));
                if(!outer) continue;
                bool inner=radius<41 || InTriangle(p,new Vector2(83,38),new Vector2(101,20),new Vector2(72,29));
                Color border=Color.Lerp(new Color(.36f,.16f,.06f),new Color(.66f,.32f,.10f),p.y/128f);
                Color parchment=Color.Lerp(new Color(.93f,.65f,.31f),new Color(1f,.87f,.58f),p.y/128f);
                sum+=(inner ? parchment : border)*.25f;
            }
            // Store straight-alpha colors for Unity's normal GUI blending.
            if(sum.a>0) { sum.r/=sum.a; sum.g/=sum.a; sum.b/=sum.a; }
            pixels[y*size+x]=sum;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    static bool InTriangle(Vector2 p,Vector2 a,Vector2 b,Vector2 c)
    {
        float ab=(b.x-a.x)*(p.y-a.y)-(b.y-a.y)*(p.x-a.x);
        float bc=(c.x-b.x)*(p.y-b.y)-(c.y-b.y)*(p.x-b.x);
        float ca=(a.x-c.x)*(p.y-c.y)-(a.y-c.y)*(p.x-c.x);
        return (ab>=0 && bc>=0 && ca>=0)||(ab<=0 && bc<=0 && ca<=0);
    }
    void OnDestroy()
    {
        if(feedingBubble) Destroy(feedingBubble);
    }
    void OnGUI()
    {
        MakeStyles();
        scale=Mathf.Min(Screen.width/1280f,Screen.height/(1280f*941/1672));
        origin=new Vector2((Screen.width-1280*scale)/2,(Screen.height-1280f*941/1672*scale)/2);
        GUI.matrix=Matrix4x4.TRS(origin,Quaternion.identity,new Vector3(scale,scale,1));
        if(page==Page.Farm) MapInput();
        else
        {
            Fill(new Rect(0,0,1280,721),new Color(.08f,.10f,.05f,.44f));
            GUI.Box(Window,GUIContent.none,plaque);
            if(Click(946,175,40,33,"×",closeSound)) { page=Page.Farm; message=""; return; }
            switch(page)
            {
                case Page.Pen: PenPanel(); break;
                case Page.House: HousePanel(); break;
                case Page.Games: case Page.Invitation: GamesPanel(); break;
                case Page.Fishing: FishingPanel(); break;
                case Page.Mole: MolePanel(); break;
            }
        }
        DrawClock();
    }
    bool Click(float x,float y,float w,float h,string text, AudioClip sound = null)
    {
        if (!GUI.Button(new Rect(x,y,w,h),text,button)) return false;
        PlayEffect(sound ? sound : clickSound);
        return true;
    }

    void SetupAudio()
    {
        if (!FindAnyObjectByType<AudioListener>()) mapCamera.gameObject.AddComponent<AudioListener>();
        musicSource=gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake=false;
        musicSource.loop=false;
        musicSource.volume=musicVolume;
        effectSource=gameObject.AddComponent<AudioSource>();
        effectSource.playOnAwake=false;
        effectSource.volume=effectVolume;
        if (musicTracks != null)
            foreach (var track in musicTracks)
                if (track && !playlist.Contains(track)) playlist.Add(track);
        ShufflePlaylist();
        UpdateMusic();
    }

    void ShufflePlaylist()
    {
        for (int i=playlist.Count-1;i>0;i--)
        {
            int j=musicRandom.Next(i+1);
            var temp=playlist[i]; playlist[i]=playlist[j]; playlist[j]=temp;
        }
        if (playlist.Count>1 && playlist[0]==lastTrack)
        {
            int j=musicRandom.Next(1,playlist.Count);
            var temp=playlist[0]; playlist[0]=playlist[j]; playlist[j]=temp;
        }
        nextTrack=0;
    }

    void UpdateMusic()
    {
        if (!musicSource || musicSource.isPlaying || AudioListener.pause || playlist.Count==0) return;
        if (!loadingTrack)
        {
            if (nextTrack>=playlist.Count) ShufflePlaylist();
            loadingTrack=playlist[nextTrack++];
            if (loadingTrack.loadState==AudioDataLoadState.Unloaded && !loadingTrack.LoadAudioData())
            {
                Debug.LogWarning("Không tải được nhạc nền: "+loadingTrack.name);
                loadingTrack=null;
                return;
            }
        }
        if (loadingTrack.loadState==AudioDataLoadState.Loading || loadingTrack.loadState==AudioDataLoadState.Unloaded) return;
        if (loadingTrack.loadState==AudioDataLoadState.Failed)
        {
            Debug.LogWarning("Không tải được nhạc nền: "+loadingTrack.name);
            loadingTrack=null;
            return;
        }
        lastTrack=loadingTrack;
        loadingTrack=null;
        musicSource.clip=lastTrack;
        Debug.Log("Nhạc nền đang phát: "+lastTrack.name);
        musicSource.Play();
    }

    void PlayEffect(AudioClip clip)
    {
        if (enableClickSounds && effectSource && clip) effectSource.PlayOneShot(clip);
    }
    void Text(float x,float y,float w,float h,string text,GUIStyle style=null) => GUI.Label(new Rect(x,y,w,h),text,style??body);
    static void Fill(Rect area,Color color)
    {
        var old=GUI.color; GUI.color=color; GUI.DrawTexture(area,Texture2D.whiteTexture); GUI.color=old;
    }
    static void Picture(Rect rect, Texture2D texture) => GUI.DrawTexture(rect,texture,ScaleMode.ScaleToFit);
    void DrawClock()
    {
        GUI.Box(new Rect(1007,8,263,52),GUIContent.none,plaque);
        Picture(new Rect(1024,16,32,33),calendar);
        Text(1058,13,82,41,"Ngày "+farm.Day.ToString("00"),digits);
        Fill(new Rect(1147,17,1,32),new Color(.53f,.31f,.19f));
        Picture(new Rect(1163,15,36,36),dial);
        int displayedMinute=(int)farm.Minute/10*10;
        Hand((float)(farm.Minute/60%12)*30,8,2.4f);
        Hand((float)(farm.Minute%60)*6,12,1.6f);
        Text(1201,13,60,41,TimeText(displayedMinute),digits);
    }
    void Hand(float angle,float length,float width)
    {
        // Pixel strokes share the HUD coordinate system at every Game View scale.
        float radians=angle*Mathf.Deg2Rad;
        Vector2 direction=new Vector2(Mathf.Sin(radians),-Mathf.Cos(radians));
        Vector2 center=new Vector2(1181,33);
        for(int i=0;i<=Mathf.CeilToInt(length);i++)
        {
            Vector2 point=center+direction*i;
            Fill(new Rect(point.x-width/2,point.y-width/2,width,width),ink);
        }
    }
    static string TimeText(int minute) => string.Format("{0:00}:{1:00}",minute/60,minute%60);
    Rect HitArea(SpriteRenderer renderer)
    {
        var b=renderer.bounds;
        var low=mapCamera.WorldToScreenPoint(b.min); var high=mapCamera.WorldToScreenPoint(b.max);
        return new Rect((low.x-origin.x)/scale,(Screen.height-high.y-origin.y)/scale,(high.x-low.x)/scale,(high.y-low.y)/scale);
    }
    void MapInput()
    {
        if(GUI.Button(HitArea(house),GUIContent.none,GUIStyle.none)) { PlayEffect(openSound); page=Page.House; }
        for(int i=0;i<5;i++)
        {
            var zone=HitArea(i==4?pond:barns[i]);
            if(GUI.Button(zone,GUIContent.none,GUIStyle.none)) { PlayEffect(openSound); page=Page.Pen; penIndex=i; animalListScroll=Vector2.zero; message=""; }
        }
        // Draw reminders after all map hit areas so neighboring buildings cannot intercept them.
        for(int i=0;i<5;i++)
        {
            var pen=farm.Pens[i];
            if(pen.Animals.Count==0 || pen.FedDay==farm.Day || farm.Minute<Farm.StartHour(i)*60) continue;
            var zone=HitArea(i==4?pond:barns[i]);
            float x=Mathf.Clamp(zone.x-14,4,1228);
            float y=Mathf.Max(4,zone.y-30);
            var reminder=new Rect(x,y,48,48);
            GUI.DrawTexture(reminder,feedingBubble);
            var foodIcon=feedArt[farm.FeedType(i)];
            if(foodIcon) Picture(new Rect(x+7,y+3,30,30),foodIcon);
            if(GUI.Button(reminder,new GUIContent("","Đến giờ cho ăn"),GUIStyle.none))
            {
                PlayEffect(openSound);
                page=Page.Pen;
                penIndex=i;
                animalListScroll=Vector2.zero;
                message="";
            }
        }
    }
    void PenPanel()
    {
        Pen pen=farm.Pens[penIndex];
        int adults=farm.AdultCount(penIndex);
        string food=penIndex<2?"gia c\u1ea7m":penIndex<4?"gia s\u00fac":"c\u00e1";
        Text(305,168,550,42,PenNames[penIndex],heading);
        Text(307,205,310,24,"DANH S\u00c1CH V\u1eacT NU\u00d4I",fine);
        Text(630,205,300,24,"CH\u0102M S\u00d3C",fine);
        Fill(new Rect(307,232,300,178),new Color(.45f,.25f,.10f,.10f));
        Fill(new Rect(627,232,329,178),new Color(.45f,.25f,.10f,.10f));
        if(pen.Animals.Count==0)
            Text(320,251,270,45,"Chu\u1ed3ng ch\u01b0a c\u00f3 v\u1eadt nu\u00f4i.",fine);
        else
        {
            animalListScroll=GUI.BeginScrollView(new Rect(313,238,288,166),animalListScroll,new Rect(0,0,262,pen.Animals.Count*55));
            for(int i=0;i<pen.Animals.Count;i++)
            {
                var animal=pen.Animals[i];
                if(i%2==0) Fill(new Rect(0,i*55,260,53),new Color(1f,.92f,.76f,.45f));
                string name=penIndex==4?FishNames[(int)animal.Species-(int)Species.Perch]:PenNames[penIndex].Substring(7);
                if(penIndex<4 && animalArt[penIndex,animal.Adult?0:1])
                    Picture(new Rect(5,i*55+3,50,45),animalArt[penIndex,animal.Adult?0:1]);
                float x=penIndex<4?62:8;
                string stage=animal.Adult?"Tr\u01b0\u1edfng th\u00e0nh":"Con non";
                Text(x,i*55+2,195,22,$"{i+1}. {name}  \u00b7  {stage}",fine);
                Text(x,i*55+25,195,24,$"{animal.Age} ng\u00e0y tu\u1ed5i  \u00b7  L\u1edbn {Math.Min(animal.Growth,Farm.RequiredDays(animal.Species))}/{Farm.RequiredDays(animal.Species)} ng\u00e0y",fine);
            }
            GUI.EndScrollView();
        }
        Text(642,241,275,27,$"S\u1ed1 con: {pen.Animals.Count} / {pen.Capacity}",body);
        Text(642,270,290,26,$"Con non: {pen.Animals.Count-adults}     Tr\u01b0\u1edfng th\u00e0nh: {adults}",fine);
        Fill(new Rect(642,299,292,1),new Color(.45f,.25f,.10f,.30f));
        Text(642,307,260,24,$"Th\u1ee9c \u0103n {food}: {farm.Feed[farm.FeedType(penIndex)]} \u0111\u01a1n v\u1ecb",fine);
        Text(642,331,270,24,$"Kh\u1ea9u ph\u1ea7n: {pen.FeedRequired}  \u00b7  T\u1eeb {Farm.StartHour(penIndex):00}:00",fine);
        string status=pen.Animals.Count==0?"Chu\u1ed3ng tr\u1ed1ng":pen.FedDay==farm.Day?"H\u00f4m nay \u0111\u00e3 cho \u0103n":farm.Minute<Farm.StartHour(penIndex)*60?"Ch\u01b0a \u0111\u1ebfn gi\u1edd cho \u0103n":"\u0110ang ch\u1edd cho \u0103n";
        Text(642,356,270,24,status,fine);
        if(penIndex==0 || penIndex==1) Text(642,380,260,24,$"Tr\u1ee9ng ch\u1edd thu: {pen.Eggs}",fine);
        if(penIndex==3) Text(642,380,260,24,$"S\u1eefa ch\u1edd thu: {pen.Milk}",fine);
        if(penIndex==4)
        {
            for(int i=0;i<3;i++) if(Click(307+i*157,421,150,35,"Th\u1ea3 "+FishNames[i])) AddAnimal((Species)((int)Species.Perch+i));
            if(Click(792,421,164,35,"C\u00e2u c\u00e1")) StartFishing();
        }
        else if(Click(307,421,190,35,"Th\u00eam con non")) AddAnimal((Species)penIndex);
        if(Click(307,465,190,35,"Cho \u0103n c\u1ea3 \u0111\u00e0n"))
            message=farm.FeedPen(penIndex)?"\u0110\u00e3 cho c\u1ea3 \u0111\u00e0n \u0103n.":pen.Animals.Count==0?"Khu nu\u00f4i \u0111ang tr\u1ed1ng.":pen.FedDay==farm.Day?"H\u00f4m nay \u0111\u00e0n \u0111\u00e3 \u0111\u01b0\u1ee3c cho \u0103n.":farm.Minute<Farm.StartHour(penIndex)*60?$"C\u00f3 th\u1ec3 cho \u0103n t\u1eeb {Farm.StartHour(penIndex):00}:00.":"Kh\u00f4ng \u0111\u1ee7 th\u1ee9c \u0103n.";
        if((penIndex==0 || penIndex==1 || penIndex==3) && Click(508,465,190,35,"Thu s\u1ea3n ph\u1ea9m"))
        {
            message=$"\u0110\u00e3 thu {pen.Eggs} tr\u1ee9ng v\u00e0 {pen.Milk} \u0111\u01a1n v\u1ecb s\u1eefa trong l\u01b0\u1ee3t th\u1eed.";
            pen.Eggs=0; pen.Milk=0;
        }
        Text(310,509,640,30,message,fine);
    }
    void AddAnimal(Species s)
    {
        message=farm.Add(penIndex,s)?"Đã thêm con giống vào khu nuôi thử.":"Khu nuôi đã đầy.";
    }
    void HousePanel()
    {
        Text(306,170,530,42,"Nh\u00e0 ch\u00ednh",heading);
        string[] tabs={"C\u00e0i \u0111\u1eb7t","Th\u00e0nh t\u1ef1u","Ti\u1ebfn \u0111\u1ed9 ch\u01a1i"};
        for(int i=0;i<tabs.Length;i++)
        {
            var old=GUI.color;
            if(houseTab==i) GUI.color=new Color(1f,.78f,.4f);
            if(Click(306+i*213,218,204,38,tabs[i])) houseTab=i;
            GUI.color=old;
        }
        if(houseTab==0)
        {
            Text(315,277,300,28,"\u00c2m l\u01b0\u1ee3ng nh\u1ea1c",body);
            musicVolume=GUI.HorizontalSlider(new Rect(565,287,220,20),musicVolume,0,1);
            Text(800,277,90,28,$"{Mathf.RoundToInt(musicVolume*100)}%",body);
            Text(315,320,300,28,"\u00c2m l\u01b0\u1ee3ng hi\u1ec7u \u1ee9ng",body);
            effectVolume=GUI.HorizontalSlider(new Rect(565,330,220,20),effectVolume,0,1);
            Text(800,320,90,28,$"{Mathf.RoundToInt(effectVolume*100)}%",body);
            if(musicSource) musicSource.volume=musicVolume;
            if(effectSource) effectSource.volume=effectVolume;
            if(Click(315,365,240,36,enableClickSounds?"\u00c2m click: B\u1eadt":"\u00c2m click: T\u1eaft")) enableClickSounds=!enableClickSounds;
            Text(315,418,240,30,"T\u1ed1c \u0111\u1ed9 th\u1eed nghi\u1ec7m",body);
            int[] speeds={1,10,50};
            for(int i=0;i<speeds.Length;i++)
            {
                int speed=speeds[i];
                var old=GUI.color;
                if(clockSpeed==speed) GUI.color=new Color(1f,.78f,.4f);
                if(Click(565+i*96,414,88,36,"x"+speed)) clockSpeed=speed;
                GUI.color=old;
            }
        }
        else if(houseTab==1)
        {
            Text(315,280,600,40,"S\u1ed5 th\u00e0nh t\u1ef1u",heading);
            Text(315,330,600,80,"\u0110\u00e3 nh\u1eadn: 0 / 30 th\u00e0nh t\u1ef1u\nN\u1ed9i dung v\u00e0 \u0111i\u1ec1u ki\u1ec7n t\u1eebng th\u00e0nh t\u1ef1u \u0111ang ch\u1edd nh\u00f3m ch\u1ed1t.",body);
        }
        else
        {
            Text(315,280,600,40,"Ti\u1ebfn \u0111\u1ed9 ch\u01a1i",heading);
            Text(315,330,600,140,$"Ng\u00e0y hi\u1ec7n t\u1ea1i: {farm.Day} / 365\nTh\u00e0nh t\u1ef1u \u0111\u00e3 nh\u1eadn: 0 / 25\nM\u1ee5c ti\u00eau ti\u1ec1n: ch\u01b0a ch\u1ed1t",body);
        }
    }
    void GamesPanel()
    {
        bool invited=page==Page.Invitation;
        Text(306,180,600,55,invited?"Lời mời chơi":"Trò chơi nhỏ",heading);
        if(!invited || invitation==0)
        {
            Picture(new Rect(334,243,230,145),pond.sprite.texture);
            if(Click(353,411,198,41,"Câu cá")) StartFishing();
        }
        if(!invited || invitation==1)
        {
            Picture(new Rect(670,250,245,138),moleArt);
            if(Click(692,411,195,41,"Đập chuột")) StartMole();
        }
        if(invited && Click(524,482,190,39,"Bỏ qua")) page=Page.Farm;
    }
    void StartFishing()
    {
        int total=0;
        var pool=new int[Weights.Length];
        for(int i=0;i<pool.Length;i++)
        {
            bool eligible=i>=3 || farm.Pens[4].Animals.Exists(a=>a.Adult && a.Species==(Species)((int)Species.Perch+i));
            pool[i]=eligible?Weights[i]:0; total+=pool[i];
        }
        int ticket=Random.Range(0,total);
        for(int i=0;i<pool.Length;i++) { ticket-=pool[i]; if(ticket<0){catchIndex=i; break;} }
        page=Page.Fishing; finished=false; message="";
    }
    void FishingPanel()
    {
        Text(306,170,560,45,"Câu cá",heading);
        if(finished && catchIndex<3 && fishArt[catchIndex] && message.StartsWith("\u0110\u00e3 c\u00e2u \u0111\u01b0\u1ee3c")) Picture(new Rect(312,221,280,186),fishArt[catchIndex]);
        else Picture(new Rect(312,221,280,186),pond.sprite.texture);
        Text(615,255,315,98,finished?message:"Bấm Giật cần khi phao đi vào vùng xanh.");
        if(farm.AdultCount(4)==0)
            Text(615,350,315,54,"Ao ch\u01b0a c\u00f3 c\u00e1 tr\u01b0\u1edfng th\u00e0nh; l\u01b0\u1ee3t n\u00e0y ch\u1ec9 c\u00f3 th\u1ec3 c\u00e2u r\u00e1c.",fine);
        float width=catchIndex==2?60:catchIndex==1?98:140;
        const float left=336, span=608, middle=left+span/2;
        Fill(new Rect(left-3,416,span+6,22),ink);
        Fill(new Rect(left,419,span,16),new Color(.75f,.55f,.33f));
        Fill(new Rect(middle-width/2,419,width,16),new Color(.32f,.65f,.36f));
        Fill(new Rect(left+marker*span-3,411,6,31),Color.white);
        if(!Click(529,469,222,43,finished?"Câu lần nữa":"Giật cần")) return;
        if(finished) { StartFishing(); return; }
        finished=true;
        if(Mathf.Abs(marker-.5f)*span>width/2) message="Trượt rồi! Hãy thử lại.";
        else if(catchIndex>=3) message="Bạn câu trúng "+CatchNames[catchIndex]+". Đã bỏ rác.";
        else message=farm.Catch((Species)((int)Species.Perch+catchIndex))?"Đã câu được một con "+CatchNames[catchIndex]+"!":"Ao đã hết cá trưởng thành thuộc loài này.";
    }
    void StartMole()
    {
        page=Page.Mole; finished=false; lives=3; score=0; timeLeft=30;
        moleIndex=-1; nextMole=Time.time+.3f; message="";
    }
    void FinishMole()
    {
        finished=true; moleIndex=-1;
        if(lives>0 && score>=15) { farm.Fertilizer[Random.Range(0,2)]+=3; message="Thắng! Bạn nhận được 3 đơn vị phân bón."; }
        else message="Hết lượt. Bạn đạt "+score+" điểm.";
    }
    void MolePanel()
    {
        Text(306,170,600,45,"Đập chuột",heading);
        for(int i=0;i<9;i++)
        {
            var rect=new Rect(312+i%3*122,234+i/3*90,116,82);
            Picture(rect,i==moleIndex?moleArt:hole);
            if(GUI.Button(rect,GUIContent.none,GUIStyle.none) && i==moleIndex && !finished)
            { PlayEffect(clickSound); score++; moleIndex=-1; nextMole=Time.time+.3f; }
        }
        Text(720,237,240,102,$"Thời gian   {Mathf.Max(0,Mathf.CeilToInt(timeLeft))} giây\nMạng còn lại   {lives} / 3\nĐiểm   {score} / 15");
        Text(720,357,241,100,finished?message:"Bấm vào chuột khi nó trồi lên. Bỏ lỡ một con sẽ mất một mạng.",fine);
        if(finished && Click(718,455,196,39,"Chơi lại")) StartMole();
    }
}

