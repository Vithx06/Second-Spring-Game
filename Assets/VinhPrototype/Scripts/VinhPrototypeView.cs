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

    readonly Farm farm = new Farm();
    static readonly string[] PenNames = { "Chuồng gà", "Chuồng vịt", "Chuồng lợn", "Chuồng bò", "Ao cá" };
    static readonly string[] FishNames = { "cá rô", "cá chép", "cá tầm" };
    static readonly string[] CatchNames = { "cá rô", "cá chép", "cá tầm", "lon nhựa", "giày cũ", "quần rách" };
    static readonly int[] Weights = { 35, 25, 10, 10, 10, 10 };
    enum Page { Farm, Pen, House, Games, Invitation, Fishing, Mole }
    Page page;
    int penIndex, lastHour, invitation, catchIndex, moleIndex, score, lives;
    float marker, timeLeft, nextMole;
    bool finished;
    string message = "";
    GUIStyle heading, body, fine, centered, plaque, button, digits;
    float scale;
    Vector2 origin;
    readonly Color ink = new Color32(61,35,20,255);
    static readonly Rect Window = new Rect(274,156,732,406);

    void Start()
    {
        // Small playable fixture: the occupied pens and catches match the reference at launch.
        for (int i=0; i<4; i++) Seed(i,(Species)i);
        for (int i=0; i<3; i++) Seed(4, (Species)((int)Species.Perch+i));
        lastHour=7;
    }

    void Seed(int pen, Species species)
    {
        farm.Add(pen,species);
        var animals=farm.Pens[pen].Animals;
        Animal animal=animals[animals.Count-1];
        animal.Age=animal.Growth=Farm.RequiredDays(species);
    }

    void Update()
    {
        farm.Advance(Time.deltaTime);
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
        Color shade=Color.Lerp(new Color(.41f,.47f,.68f),Color.white,daylight);
        shade=Color.Lerp(shade,new Color(1,.70f,.52f),.60f*4*daylight*(1-daylight));
        foreach(var layer in world) layer.color=shade;
        nightSky.color=new Color(1,1,1,1-daylight);
        for(int i=0;i<4;i++) barns[i].sprite=farm.Pens[i].Animals.Count==0?empty[i]:occupied[i];
        for(int i=0;i<swimmers.Length;i++)
        {
            var fish=swimmers[i];
            fish.enabled=i<farm.Pens[4].Animals.Count;
            if(!fish.enabled) continue;
            float phase=Time.time*(.70f+i*.09f)+i*2;
            fish.transform.localPosition=new Vector3((10+30*Mathf.Cos(phase))/100,(18+10*Mathf.Sin(phase)+i*4)/100,0);
            fish.flipX=Mathf.Sin(phase)>0;
            fish.color=shade;
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
            if(Click(946,175,40,33,"×")) { page=Page.Farm; message=""; return; }
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
    bool Click(float x,float y,float w,float h,string text) => GUI.Button(new Rect(x,y,w,h),text,button);
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
        Hand((float)(farm.Minute/60%12)*30,8,2.4f);
        Hand((float)(farm.Minute%60)*6,12,1.6f);
        Text(1201,13,60,41,TimeText((int)farm.Minute),digits);
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
        if(GUI.Button(HitArea(house),GUIContent.none,GUIStyle.none)) page=Page.House;
        for(int i=0;i<5;i++)
        {
            var zone=HitArea(i==4?pond:barns[i]);
            if(GUI.Button(zone,GUIContent.none,GUIStyle.none)) { page=Page.Pen; penIndex=i; message=""; }
            var pen=farm.Pens[i];
            if(pen.Animals.Count>0 && pen.FedDay!=farm.Day && farm.Minute>=Farm.StartHour(i)*60)
                GUI.Label(new Rect(zone.xMax-75,zone.yMax-24,74,27),"Cho ăn",button);
        }
        if(Click(1104,677,148,34,"Trò chơi nhỏ")) page=Page.Games;
    }
    void PenPanel()
    {
        Pen pen=farm.Pens[penIndex];
        Text(305,170,600,46,PenNames[penIndex],heading);
        var art=penIndex==4?pond.sprite: barns[penIndex].sprite;
        Picture(new Rect(307,237,280,174),art.texture);
        int adults=farm.AdultCount(penIndex);
        string food=penIndex<2?"gia cầm":penIndex<4?"gia súc":"cho cá";
        Text(612,228,355,32,$"Số con   {pen.Animals.Count} / {pen.Capacity}",heading);
        Text(612,272,355,125,$"Con non   {pen.Animals.Count-adults}       Trưởng thành   {adults}\nThức ăn {food}   {farm.Feed[farm.FeedType(penIndex)]} đơn vị\nKhẩu phần hôm nay   {pen.FeedRequired} đơn vị\nGiờ cho ăn   {Farm.StartHour(penIndex):00}:00");
        if(penIndex==0 || penIndex==1) Text(612,387,340,30,$"Trứng chờ thu   {pen.Eggs}");
        if(penIndex==3) Text(612,387,340,30,$"Sữa chờ thu   {pen.Milk}");
        if(penIndex==4)
        {
            for(int i=0;i<3;i++) if(Click(306+i*159,428,153,36,"Thả "+FishNames[i]))
                AddAnimal((Species)((int)Species.Perch+i));
            if(Click(796,428,145,36,"Câu cá")) StartFishing();
        }
        else if(Click(306,428,168,36,"Thêm con non")) AddAnimal((Species)penIndex);
        if(Click(306,473,168,36,"Cho ăn cả đàn"))
            message=farm.FeedPen(penIndex)?"Đã cho cả đàn ăn.":pen.Animals.Count==0?"Khu nuôi đang trống.":pen.FedDay==farm.Day?"Hôm nay đàn đã được cho ăn.":farm.Minute<Farm.StartHour(penIndex)*60?$"Có thể cho ăn từ {Farm.StartHour(penIndex):00}:00.":"Không đủ thức ăn.";
        if((penIndex==0 || penIndex==1 || penIndex==3) && Click(487,473,165,36,"Thu sản phẩm"))
        {
            message=$"Đã thu {pen.Eggs} trứng và {pen.Milk} đơn vị sữa trong lượt thử.";
            pen.Eggs=0; pen.Milk=0;
        }
        Text(309,516,650,34,message,fine);
    }
    void AddAnimal(Species s)
    {
        message=farm.Add(penIndex,s)?"Đã thêm con giống vào khu nuôi thử.":"Khu nuôi đã đầy.";
    }
    void HousePanel()
    {
        Text(306,170,530,45,"Nhà chính",heading);
        Picture(new Rect(308,244,290,206),house.sprite.texture);
        Text(625,240,325,42,"Sổ thành tựu",heading);
        Text(625,296,320,115,"Đã nhận   0 / 30 thành tựu\nMốc chiến thắng   25 thành tựu\nThời hạn   365 ngày");
        Text(625,422,315,63,"Mục tiêu tiền và điều kiện từng thành tựu đang chờ nhóm chốt.",fine);
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
        Picture(new Rect(312,221,280,186),pond.sprite.texture);
        Text(615,255,315,98,finished?message:"Bấm Giật cần khi phao đi vào vùng xanh.");
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
            { score++; moleIndex=-1; nextMole=Time.time+.3f; }
        }
        Text(720,237,240,102,$"Thời gian   {Mathf.Max(0,Mathf.CeilToInt(timeLeft))} giây\nMạng còn lại   {lives} / 3\nĐiểm   {score} / 15");
        Text(720,357,241,100,finished?message:"Bấm vào chuột khi nó trồi lên. Bỏ lỡ một con sẽ mất một mạng.",fine);
        if(finished && Click(718,455,196,39,"Chơi lại")) StartMole();
    }
}

