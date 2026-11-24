public static class CropGameRules
{
    // Agreed plant rules use game days; tilling and ripe-to-rot remain prototype settings.
    public static CropDefinition[] CreateDefinitions(float rotGameSeconds)
    {
        string[] ids={"rice","corn","sugarcane","strawberry","apple","orange","rose","orchid"};
        string[] names={"Lúa","Ngô","Mía","Dâu tây","Táo","Cam","Hoa hồng","Hoa lan"};
        int[] stageDays={1,2,2,2,4,5,2,3},yield={3,3,3,3,4,4,2,2},regrow={1,1,1,1,2,2,1,1};
        var definitions=new CropDefinition[8];
        for(int i=0;i<definitions.Length;i++)
        {
            bool repeat=i>=3 && i<=5;
            definitions[i]=new CropDefinition(ids[i],names[i],ids[i]+".seed",ids[i]+".product",yield[i],
                stageDays[i]*86400f,stageDays[i]*86400f,12*3600f,86400f,rotGameSeconds,
                i>=6 ? CropCategory.Flower : i>=3 ? CropCategory.FruitCrop : CropCategory.FieldCrop,
                repeat ? HarvestType.Regrowing : HarvestType.SingleHarvest,regrow[i]*86400f);
        }
        return definitions;
    }
}
