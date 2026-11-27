public static class CropGameRules
{
    public static CropDefinition[] CreateDefinitions(float rotGameSeconds)
    {
        // Giữ thứ tự hiển thị cũ; enum không phụ thuộc vị trí nút trong giao diện.
        CropType[] types={CropType.Rice,CropType.Corn,CropType.Sugarcane,CropType.Strawberry,CropType.Apple,CropType.Orange,CropType.Rose,CropType.Orchid};
        ItemType[] seeds={ItemType.RiceSeed,ItemType.CornSeed,ItemType.SugarcaneSeed,ItemType.StrawberrySeed,ItemType.AppleSeed,ItemType.OrangeSeed,ItemType.RoseSeed,ItemType.OrchidSeed};
        ItemType[] products={ItemType.Rice,ItemType.Corn,ItemType.Sugarcane,ItemType.Strawberry,ItemType.Apple,ItemType.Orange,ItemType.Rose,ItemType.Orchid};
        string[] names={"Lúa","Ngô","Mía","Dâu tây","Táo","Cam","Hoa hồng","Hoa lan"};
        int[] stageDays={1,2,2,2,4,5,2,3},yield={3,3,3,3,4,4,2,2},regrow={1,1,1,1,2,2,1,1};
        var definitions=new CropDefinition[types.Length];
        for(int i=0;i<definitions.Length;i++)
        {
            bool repeat=types[i]==CropType.Strawberry || types[i]==CropType.Apple || types[i]==CropType.Orange;
            var category=types[i]==CropType.Rose || types[i]==CropType.Orchid?CropCategory.Flower:repeat?CropCategory.FruitCrop:CropCategory.FieldCrop;
            // Một ngày game = 86.400 giây game. Đồng hồ sẽ đổi giây thực sang đơn vị này.
            definitions[i]=new CropDefinition(types[i],names[i],seeds[i],products[i],yield[i],
                stageDays[i]*86400f,stageDays[i]*86400f,12*3600f,86400f,rotGameSeconds,
                category,repeat?HarvestType.Regrowing:HarvestType.SingleHarvest,regrow[i]*86400f);
        }
        return definitions;
    }
}
