using System.Collections.Generic;
using static SecondSpring.Achievements.AchievementMetric;

namespace SecondSpring.Achievements
{
    internal static class AchievementCatalog
    {
        private static AchievementRequirement Need(AchievementMetric metric, long target) => new AchievementRequirement(metric, target);
        private static AchievementDefinition Define(int id, string title, string description, params AchievementRequirement[] requirements) =>
            new AchievementDefinition(id, title, description, id <= 15 ? AchievementDifficulty.Easy : id <= 25 ? AchievementDifficulty.Medium : AchievementDifficulty.Hard, requirements);

        public static IEnumerable<AchievementDefinition> Create()
        {
            return new[] {
                Define(1,"Bắt đầu gieo trồng","Gieo hạt lần đầu.",Need(SeedsSown,1)),
                Define(2,"Giọt nước đầu tiên","Tưới cây lần đầu.",Need(Waterings,1)),
                Define(3,"Vụ mùa đầu tiên","Thu hoạch cây trồng lần đầu.",Need(HarvestActions,1)),
                Define(4,"Người bạn mới","Mua vật nuôi đầu tiên.",Need(AnimalsBought,1)),
                Define(5,"Bữa ăn đầu tiên","Cho một chuồng hoặc ao ăn lần đầu.",Need(FeedActions,1)),
                Define(6,"Quả trứng đầu tiên","Thu trứng gà hoặc vịt lần đầu.",Need(EggsCollected,1)),
                Define(7,"Ly sữa đầu tiên","Thu sữa bò lần đầu.",Need(MilkCollected,1)),
                Define(8,"Mẻ cá đầu tiên","Câu thành công cá lần đầu.",Need(FishCaught,1)),
                Define(9,"Tay đập chuột mới","Thắng đập chuột lần đầu.",Need(MoleWins,1)),
                Define(10,"Khách hàng đầu tiên","Mua vật phẩm ở chợ lần đầu.",Need(ShopPurchases,1)),
                Define(11,"Người bán mới","Bán vật phẩm ở chợ lần đầu.",Need(ItemsSold,1)),
                Define(12,"Khu vườn nhỏ","Thu hoạch 20 sản phẩm cây trồng.",Need(CropsHarvested,20)),
                Define(13,"Đàn gia cầm nhỏ","Đang nuôi tổng cộng 5 con gà và vịt.",Need(PoultryCount,5)),
                Define(14,"Giỏ trứng nhỏ","Thu 20 quả trứng gà hoặc vịt.",Need(EggsCollected,20)),
                Define(15,"Người chăm gà","Cho gà ăn 3 ngày liên tiếp.",Need(ChickenFeedingStreak,3)),
                Define(16,"Khu vườn trù phú","Thu hoạch 100 sản phẩm cây trồng.",Need(CropsHarvested,100)),
                Define(17,"Đàn gà đông vui","Đang nuôi 15 con gà.",Need(Chickens,15)),
                Define(18,"Đàn vịt đông vui","Đang nuôi 15 con vịt.",Need(Ducks,15)),
                Define(19,"Trang trại bò sữa","Đang nuôi 10 con bò.",Need(Cows,10)),
                Define(20,"Giỏ trứng đầy","Thu 100 quả trứng gà hoặc vịt.",Need(EggsCollected,100)),
                Define(21,"Nhà cung cấp sữa","Thu 50 đơn vị sữa bò.",Need(MilkCollected,50)),
                Define(22,"Cần thủ chăm chỉ","Câu 20 con cá.",Need(FishCaught,20)),
                Define(23,"Bảo vệ mùa màng","Thắng 5 lượt đập chuột.",Need(MoleWins,5)),
                Define(24,"Chăm sóc đều đặn","Cho bò ăn 7 ngày liên tiếp.",Need(CowFeedingStreak,7)),
                Define(25,"Thương nhân tập sự","Bán 100 vật phẩm ở chợ.",Need(ItemsSold,100)),
                Define(26,"Mùa vàng bội thu","Thu hoạch 500 sản phẩm cây trồng.",Need(CropsHarvested,500)),
                Define(27,"Trang trại lớn","Đang nuôi ít nhất 15 con mỗi loại gà, vịt, lợn và bò.",Need(Chickens,15),Need(Ducks,15),Need(Pigs,15),Need(Cows,15)),
                Define(28,"Nhà cung cấp thực phẩm","Thu 300 quả trứng và 150 đơn vị sữa.",Need(EggsCollected,300),Need(MilkCollected,150)),
                Define(29,"Cần thủ lão luyện","Câu 100 con cá.",Need(FishCaught,100)),
                Define(30,"Người chăn nuôi tận tâm","Cho cả bốn chuồng ăn 14 ngày liên tiếp.",Need(AllPensFeedingStreak,14))
            };
        }
    }
}
