using System;
using SecondSpring.Animals;

int checks = 0;
void Check(bool condition, string label) { checks++; if (!condition) throw new Exception(label); }
var pens = new AnimalPenRegistry();
var service = new AnimalService(pens);
var chickenPen = pens.GetPen(AnimalType.Chicken);
var chicken = new Chicken(1);
Check(chickenPen.AddAnimal(chicken), "add chicken");
Check(!chickenPen.AddAnimal(new Chicken(1)), "duplicate ID in pen");
Check(!chickenPen.AddAnimal(new Duck(2)), "wrong species rejected");
Check(!service.FeedPen(AnimalType.Chicken, 1, 6, (_, _) => true), "before feeding time");
Check(!service.FeedPen(AnimalType.Chicken, 1, 7, (_, _) => false), "insufficient feed");
Check(service.FeedPen(AnimalType.Chicken, 1, 7, (type, amount) => type == FeedType.Poultry && amount == 1), "feed chicken");
var late = new Chicken(2); chickenPen.AddAnimal(late);
Check(!service.FeedPen(AnimalType.Chicken, 1, 8, (_, _) => true), "feed once daily");
service.OnDayChanged(2);
Check(chicken.Age == 1 && chicken.GrowthProgress == 1 && late.Age == 1 && late.GrowthProgress == 0, "late purchase not fed");
service.OnDayChanged(3);
Check(chicken.Age == 2 && chicken.GrowthProgress == 1, "missed feeding still ages");
for (int day = 3; day <= 4; day++) { Check(service.FeedPen(AnimalType.Chicken, day, 7, (_, _) => true), "feed growth day"); service.OnDayChanged(day + 1); }
Check(chicken.IsAdult() && chickenPen.StoredProducts == 1, "egg on maturity");
Check(service.FeedPen(AnimalType.Chicken, 5, 7, (_, _) => true), "feed adults"); service.OnDayChanged(6);
Check(chickenPen.StoredProducts == 3, "both chickens produce after maturity");
Check(service.CollectProducts(AnimalType.Chicken, (_, amount) => 1) == 1 && chickenPen.StoredProducts == 2, "partial product storage");
Check(!service.CollectMeat(AnimalType.Chicken, 1, (_, _) => false) && chickenPen.Animals.Count == 2, "failed meat storage preserves animal");
Check(service.CollectMeat(AnimalType.Chicken, 1, (type, amount) => type == AnimalProductType.ChickenMeat && amount == 2) && chickenPen.Animals.Count == 1, "meat removes adult");
var pond = pens.GetPen(AnimalType.Fish);
var fish = new Fish(3, FishType.Perch); pond.AddAnimal(fish);
Check(!service.ConsumeCaughtFish(FishType.Perch), "young fish cannot be caught");
for (int day = 6; day <= 7; day++) { Check(service.FeedPen(AnimalType.Fish, day, 17, (_, _) => true), "feed fish"); service.OnDayChanged(day + 1); }
Check(service.GetAdultFishTypes().Contains(FishType.Perch), "adult fish listed");
Check(service.ConsumeCaughtFish(FishType.Perch) && pond.Animals.Count == 0, "catch removes one adult fish");
var full = new AnimalPen(AnimalType.Fish);
for (int i = 1; i <= 30; i++) Check(full.AddAnimal(new Fish(i, FishType.Carp)), "pond capacity");
Check(full.GetFeedRequired() == 6 && !full.AddAnimal(new Fish(31, FishType.Perch)), "30 fish limit and feed amount");
var purchasePens = new AnimalPenRegistry(); int balance = 100; int nextId = 10;
var purchase = new AnimalPurchaseService(purchasePens, (type, fishType) => 20, () => nextId++, amount => { if (balance < amount) return false; balance -= amount; return true; }, amount => balance += amount);
Check(purchase.BuyAnimal(AnimalType.Chicken, 3) && balance == 40 && purchasePens.GetPen(AnimalType.Chicken).Animals.Count == 3, "purchase success");
Check(!purchase.BuyAnimal(AnimalType.Chicken, 3) && balance == 40 && purchasePens.GetPen(AnimalType.Chicken).Animals.Count == 3, "insufficient money preserves state");
Check(!purchase.BuyAnimal(AnimalType.Fish, 1), "fish type required");
Console.WriteLine($"Passed {checks} animal checks.");
