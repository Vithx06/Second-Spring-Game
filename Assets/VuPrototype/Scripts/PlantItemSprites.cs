using UnityEngine;

// One transparent atlas: two rows of seed packets, two rows of harvested sacks.
internal static class PlantItemSprites
{
    private static Texture2D _atlas;
    internal static bool Draw(Rect rect,int crop,bool seed)
    {
        if(_atlas==null)
        {
            _atlas=Resources.Load<Texture2D>("PlantArt/FarmItemBags");
            if(_atlas==null)return false;
            _atlas.filterMode=FilterMode.Point;
        }
        int cell=Mathf.Clamp(crop,0,7)+(seed ? 0 : 8);
        float side=Mathf.Min(rect.width,rect.height);
        var square=new Rect(rect.center.x-side/2,rect.center.y-side/2,side,side);
        Color previous=GUI.color;GUI.color=Color.white;
        GUI.DrawTextureWithTexCoords(square,_atlas,new Rect((cell%4)/4f,(3-cell/4)/4f,0.25f,0.25f),true);
        GUI.color=previous;
        return true;
    }
    internal static void DrawSeed(Rect rect,int crop)
    {
        if(!Draw(rect,crop,true) && PlantPixelSprites.IsReady)PlantPixelSprites.Draw(rect,crop+1,2);
    }
}
