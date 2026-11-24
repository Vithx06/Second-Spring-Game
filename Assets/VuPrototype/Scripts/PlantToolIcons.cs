using UnityEngine;

// Small original pixel icons for the prototype toolbar. No font glyph dependency.
internal static class PlantToolIcons
{
    internal enum Kind { Hoe, Seed, Water, Basket, Yield, Growth, Drop, Coin, Crate, Stall }
    private static readonly Texture2D[] Cache=new Texture2D[10];
    internal static Texture2D Get(Kind kind)
    {
        if(kind==Kind.Coin)
        {
            var coin=Resources.Load<Texture2D>("PlantArt/FarmCoin");
            if(coin!=null) {coin.filterMode=FilterMode.Point;return coin;}
        }
        if(Cache[(int)kind]!=null) return Cache[(int)kind];
        var texture=new Texture2D(24,24,TextureFormat.RGBA32,false);
        texture.name="PlantTool_"+kind; texture.filterMode=FilterMode.Point;
        texture.wrapMode=TextureWrapMode.Clamp;
        var pixels=new Color32[24*24];
        Color32 brown=new Color32(100,65,36,255), gold=new Color32(225,170,65,255);
        Color32 green=new Color32(67,137,58,255), blue=new Color32(58,167,224,255);
        void Box(int x,int y,int w,int h,Color32 color)
        {
            for(int j=y;j<y+h;j++) for(int i=x;i<x+w;i++)
                if(i>=0 && i<24 && j>=0 && j<24) pixels[(23-j)*24+i]=color;
        }
        if(kind==Kind.Coin)
        {
            Box(6,2,12,20,brown);Box(3,5,18,14,brown);
            Box(6,3,12,18,gold);Box(4,6,16,12,gold);
            Box(8,5,8,14,new Color32(255,224,111,255));Box(11,7,2,10,brown);
        }
        else if(kind==Kind.Crate)
        {
            Box(2,5,20,17,brown);Box(4,7,16,13,gold);
            for(int x=7;x<20;x+=5)Box(x,7,2,13,brown);
            Box(3,9,18,2,brown);Box(3,17,18,2,brown);
        }
        else if(kind==Kind.Stall)
        {
            Box(4,8,2,14,brown);Box(18,8,2,14,brown);Box(3,17,18,5,gold);
            Box(2,4,20,6,new Color32(210,76,60,255));
            for(int x=5;x<22;x+=6)Box(x,4,3,6,new Color32(255,224,166,255));
            Box(7,13,4,4,green);Box(12,13,4,4,gold);
        }
        else if(kind==Kind.Drop)
        {
            for(int y=2;y<21;y++)
            {
                int half=y<12 ? (y-1)/2 : y<18 ? 6 : 21-y;
                Box(12-half,y,half*2+1,1,new Color32(26,104,173,255));
                if(half>1) Box(13-half,y,half*2-1,1,blue);
            }
            Box(9,11,2,6,new Color32(185,242,255,255));
        }
        else if(kind==Kind.Hoe)
        {
            for(int i=0;i<16;i++) Box(4+i,19-i,3,3,brown);
            Box(13,3,9,4,new Color32(76,89,92,255));
            Box(18,6,4,5,new Color32(163,181,181,255));
        }
        else if(kind==Kind.Water)
        {
            Box(2,6,9,10,brown);Box(4,8,6,6,new Color32(0,0,0,0));
            Box(7,8,11,12,new Color32(33,111,159,255));Box(9,9,8,9,blue);
            Box(16,10,4,4,blue);Box(19,7,3,5,blue);Box(20,5,3,3,gold);
            Box(10,4,4,5,blue);Box(8,3,8,2,gold);
        }
        else if(kind==Kind.Basket)
        {
            Box(5,5,14,11,brown);Box(7,7,10,8,new Color32(0,0,0,0));
            Box(6,11,4,5,green);Box(10,10,4,6,gold);Box(14,11,4,5,new Color32(217,92,63,255));
            Box(3,14,18,7,brown);Box(5,15,14,5,gold);
            for(int x=6;x<20;x+=4) Box(x,15,1,5,brown);
        }
        else if(kind==Kind.Seed)
        {
            Box(5,6,14,16,brown);Box(6,7,12,14,new Color32(247,220,153,255));
            Box(7,3,10,4,gold);Box(11,11,2,8,green);
            Box(7,10,5,4,green);Box(12,8,5,4,green);
        }
        else
        {
            Color32 accent=kind==Kind.Yield ? gold : blue;
            Box(7,3,10,4,brown);Box(5,7,14,14,brown);Box(6,8,12,12,new Color32(235,219,174,255));
            Box(7,9,10,9,accent);
            if(kind==Kind.Yield) {Box(11,10,2,7,green);Box(8,12,8,2,green);}
            else {Box(12,10,2,4,brown);Box(9,13,5,2,brown);}
        }
        texture.SetPixels32(pixels);texture.Apply();Cache[(int)kind]=texture;
        return texture;
    }
}
