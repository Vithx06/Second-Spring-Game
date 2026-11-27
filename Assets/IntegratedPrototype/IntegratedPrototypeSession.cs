using System;
using System.Collections.Generic;
using System.Linq;
using SecondSpring.Minigames;
using SecondSpring.Prototype;

namespace SecondSpring.IntegratedPrototype
{
    internal enum CropTool { Hoe, Seed, Water, Harvest, Yield, Growth }

    internal sealed class IntegratedPrototypeSession
    {
        internal const string YieldFertilizer = "fertilizer.yield";
        internal const string GrowthFertilizer = "fertilizer.growth";
        internal static readonly string[] Pens = { "Chuồng gà", "Chuồng vịt", "Chuồng lợn", "Chuồng bò", "Ao cá" };
        internal static readonly string[] Animals = { "Gà", "Vịt", "Lợn", "Bò", "Cá rô", "Cá chép", "Cá tầm" };
        internal static readonly string[] FeedItems = { "feed.poultry", "feed.livestock", "feed.fish" };
        private static readonly string[] Products = { "animal.chicken-egg", "animal.duck-egg", "", "animal.milk" };
        private static readonly string[] Meat = { "animal.chicken-meat", "animal.duck-meat", "animal.pork", "animal.beef" };
        private static readonly int[] MeatYields = { 2, 3, 6, 8 };
        private static readonly string[] FishItems = { "fish.perch", "fish.carp", "fish.sturgeon" };
        private static readonly int[] AnimalPrices = { 20, 25, 100, 200, 10, 10, 10 };

        internal GameClock Clock { get; } = new GameClock();
        internal Inventory Inventory { get; } = new Inventory();
        internal Wallet Wallet { get; } = new Wallet(1000);
        internal CropDefinition[] Crops { get; } = CropGameRules.CreateDefinitions(86400);
        internal Farm Livestock { get; }
        internal ShopCatalog Catalog { get; }
        internal ShopFacade Market { get; }
        internal MinigameManager Invitations { get; }
        internal IMinigame Game { get; private set; }
        internal string GameResult { get; private set; }

        private readonly FarmingService _farming;
        private readonly Random _random;
        private readonly Dictionary<string, int> _pendingFertilizer = new Dictionary<string, int>();
        internal int PendingFertilizer => _pendingFertilizer.Values.Sum();

        internal IntegratedPrototypeSession(IEnumerable<FarmPlot> plots, Random random = null)
        {
            _random = random ?? new Random();
            Livestock = new Farm(Clock);
            Catalog = BuildCatalog();
            Market = new ShopFacade(Inventory, Wallet, Catalog);
            _farming = new FarmingService(Inventory, plots, Inventory,
                new ICropCreator[] { new SingleHarvestCropCreator(), new RegrowingCropCreator() },
                FertilizerDefinition.CreateDefaults());
            Invitations = new MinigameManager(() => Game != null && !Game.IsFinished ? .5d : _random.NextDouble(),
                CreateGame);
            Clock.Subscribe(_farming);
            Clock.Subscribe(Invitations);

            foreach (var crop in Crops) Add(crop.SeedItemId, 5);
            foreach (string feed in FeedItems) Add(feed, 5);
            Add(YieldFertilizer, 3);
            Add(GrowthFertilizer, 3);
        }

        internal void Tick(double realSeconds, int speed)
        {
            Clock.Advance(realSeconds * speed);
            if (Game != null && !Game.IsFinished) Game.Advance(realSeconds);
        }

        internal bool Work(FarmPlot plot, CropTool tool, int seed)
        {
            switch (tool)
            {
                case CropTool.Hoe: return _farming.Till(plot);
                case CropTool.Seed: return _farming.Plant(plot, Crops[seed]);
                case CropTool.Water: return _farming.Water(plot);
                case CropTool.Harvest: return _farming.Harvest(plot);
                case CropTool.Yield: return _farming.Fertilize(plot, ItemType.YieldBoostFertilizer);
                case CropTool.Growth: return _farming.Fertilize(plot, ItemType.GrowthBoostFertilizer);
                default: return false;
            }
        }

        internal static int Price(Species species) => AnimalPrices[(int)species];

        internal string BuyAnimal(int pen, Species species, int quantity)
        {
            var target = Livestock.Pens[pen];
            if (quantity < 1 || quantity > target.Capacity - target.Animals.Count) return "Khu nuôi không còn đủ chỗ.";
            if ((pen == 4 && species < Species.Perch) || (pen != 4 && (int)species != pen)) return "Giống này cần khu nuôi khác.";
            if (!Wallet.TrySpend(Price(species) * quantity)) return "Ví không đủ xu.";
            for (int i = 0; i < quantity; i++) Livestock.Add(pen, species);
            return $"Đã đưa {quantity} {Animals[(int)species].ToLowerInvariant()} vào khu nuôi.";
        }

        internal string Feed(int pen)
        {
            var target = Livestock.Pens[pen];
            if (target.Animals.Count == 0) return "Khu nuôi đang trống.";
            if (target.FedDay == Clock.Day) return "Đàn đã được cho ăn hôm nay.";
            if (Clock.Hour < Farm.StartHour(pen)) return $"Có thể cho ăn từ {Farm.StartHour(pen):00}:00.";
            int food = Livestock.FeedType(pen);
            string item = FeedItems[food];
            int required = target.FeedRequired;
            if (!Inventory.TryRemove(item, required)) return "Thiếu thức ăn. Có thể mua thêm ở chợ.";
            // Vinh's prototype keeps its own feed array. Give it just this meal
            // so the shared inventory is the only stock the player can spend.
            Livestock.Feed[food] = required;
            if (Livestock.FeedPen(pen)) return $"Đã cho đàn ăn {required} đơn vị thức ăn.";
            Livestock.Feed[food] = 0;
            Add(item, required);
            return "Chưa thể cho đàn ăn.";
        }

        internal string Collect(int pen)
        {
            var target = Livestock.Pens[pen];
            int quantity = pen == 3 ? target.Milk : target.Eggs;
            if (quantity == 0) return "Chưa có sản phẩm để thu.";
            string item = Products[pen];
            if (!Add(item, quantity)) return "Kho đầy. Sản phẩm vẫn ở chuồng.";
            if (pen == 3) target.Milk = 0;
            else target.Eggs = 0;
            return $"Đã đưa {quantity} × {Inventory.FindItem(item).Name} vào kho.";
        }

        internal string CollectMeat(int pen, Animal animal)
        {
            var target = Livestock.Pens[pen];
            if (pen == 4 || !animal.Adult || !target.Animals.Contains(animal)) return "Chỉ lấy thịt từ con trưởng thành trong chuồng.";
            int quantity = MeatYields[pen];
            string id = Meat[pen];
            if (!Add(id, quantity)) return "Kho đầy. Vật nuôi vẫn ở chuồng.";
            target.Animals.Remove(animal);
            return $"Đã đưa {quantity} × {Inventory.FindItem(id).Name} vào kho.";
        }

        internal bool AcceptInvitation()
        {
            if ((Game != null && !Game.IsFinished) || !Invitations.TryAcceptInvitation(out _)) return false;
            WatchGame(Invitations.CurrentGame);
            return true;
        }

        internal void StartGame(MinigameType type)
        {
            if (Game != null && !Game.IsFinished) return;
            Invitations.DismissInvitation();
            var game = CreateGame(type);
            game.Start();
            WatchGame(game);
        }

        internal void CancelGame()
        {
            if (Game == null) return;
            // An accepted invitation stays active in its manager until the turn completes.
            if (!Game.IsFinished)
            {
                Game.Completed -= RewardGame;
                Game.Advance(30);
            }
            Game = null;
        }

        private IMinigame CreateGame(MinigameType type)
        {
            if (type == MinigameType.WhackAMole) return new WhackAMoleMinigame(_random.Next);
            var adultFish = Livestock.Pens[4].Animals.Where(animal => animal.Adult)
                .Select(animal => (FishingCatch)((int)animal.Species - (int)Species.Perch));
            return new FishingMinigame(adultFish, _random.NextDouble());
        }

        private void WatchGame(IMinigame game)
        {
            Game = game;
            GameResult = "";
            Game.Completed += RewardGame;
        }

        private void RewardGame()
        {
            Game.Completed -= RewardGame;
            if (Game is WhackAMoleMinigame mole)
            {
                if (!mole.Result.IsWon) { GameResult = $"Hết lượt. Bạn đập được {mole.Result.Hits} con."; return; }
                string fertilizer = mole.Result.Fertilizer == MoleFertilizer.YieldBoost ? YieldFertilizer : GrowthFertilizer;
                int quantity = mole.Result.Quantity;
                if (Add(fertilizer, quantity)) GameResult = $"Đã đưa {quantity} × {Inventory.FindItem(fertilizer).Name} vào kho.";
                else
                {
                    _pendingFertilizer.TryGetValue(fertilizer, out int prior);
                    _pendingFertilizer[fertilizer] = prior + quantity;
                    GameResult = "Kho đầy. Có thể nhận phân thưởng sau ở Nhà chính.";
                }
                return;
            }

            var result = ((FishingMinigame)Game).Result;
            if (result.Outcome == FishingOutcome.Miss) { GameResult = "Lượt này chưa câu được cá."; return; }
            if (result.Outcome == FishingOutcome.Trash) { GameResult = "Bạn câu phải rác và đã bỏ đi."; return; }
            int index = (int)result.Catch.Value;
            string fish = FishItems[index];
            if (!Add(fish, 1)) { GameResult = "Kho đầy. Cá vẫn ở trong ao."; return; }
            if (!Livestock.Catch((Species)((int)Species.Perch + index)))
            {
                Inventory.TryRemove(fish, 1);
                GameResult = "Ao đã hết cá trưởng thành thuộc loài này.";
                return;
            }
            GameResult = $"Đã đưa {Inventory.FindItem(fish).Name.ToLowerInvariant()} vào kho.";
        }

        internal int CollectPendingFertilizer()
        {
            int count = 0;
            foreach (var reward in _pendingFertilizer.ToArray())
            {
                if (!Add(reward.Key, reward.Value)) continue;
                count += reward.Value;
                _pendingFertilizer.Remove(reward.Key);
            }
            return count;
        }

        private bool Add(string id, int quantity) => Inventory.TryAdd(Inventory.FindItem(id), quantity);

        private ShopCatalog BuildCatalog()
        {
            var offers = new List<ShopOffer>();
            void Offer(string id, string name, int buy, int sell)
            {
                var item = new ItemDefinition(id, name, int.MaxValue);
                Inventory.Register(item);
                offers.Add(new ShopOffer(item, buy, sell));
            }

            int[] seedPrices = { 10, 15, 20, 60, 120, 120, 20, 35 };
            int[] cropPrices = { 8, 12, 16, 14, 20, 20, 18, 30 };
            for (int i = 0; i < Crops.Length; i++)
            {
                Offer(Crops[i].SeedItemId, "Giống " + Crops[i].Name, seedPrices[i], 0);
                Offer(Crops[i].ProductItemId, Crops[i].Name, 0, cropPrices[i]);
            }
            Offer(YieldFertilizer, "Phân tăng năng suất", 5, 0);
            Offer(GrowthFertilizer, "Phân chín nhanh", 5, 0);
            Offer(FeedItems[0], "Thức ăn gia cầm", 5, 0);
            Offer(FeedItems[1], "Thức ăn gia súc", 5, 0);
            Offer(FeedItems[2], "Thức ăn cho cá", 5, 0);
            Offer(Products[0], "Trứng gà", 10, 5);
            Offer(Products[1], "Trứng vịt", 10, 5);
            Offer(Products[3], "Sữa bò", 25, 20);
            Offer(Meat[0], "Thịt gà", 100, 70);
            Offer(Meat[1], "Thịt vịt", 120, 85);
            Offer(Meat[2], "Thịt lợn", 200, 150);
            Offer(Meat[3], "Thịt bò", 350, 250);
            Offer(FishItems[0], "Cá rô", 30, 20);
            Offer(FishItems[1], "Cá chép", 60, 40);
            Offer(FishItems[2], "Cá tầm", 150, 100);
            return new ShopCatalog(offers);
        }
    }
}
