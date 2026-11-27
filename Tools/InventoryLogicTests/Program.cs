using System;
using System.Linq;

int count=0;
void Check(bool ok,string name) { if(!ok)throw new Exception(name);count++; }
var seed=new ItemDefinition(ItemType.RiceSeed,"Giống lúa",ItemCategory.Seed);
var rice=new ItemDefinition(ItemType.Rice,"Lúa",ItemCategory.CropProduct);
var catalog=new ItemCatalog(new[]{seed,rice});
var inventory=new Inventory(catalog,1);
Check(inventory.Capacity==1 && inventory.UsedSlots==0,"configured capacity");
Check(inventory.TryAdd(ItemType.RiceSeed,1),"typed add");
Check(inventory.TryAdd(ItemType.RiceSeed,1000) && inventory.UsedSlots==1,"same type uses one slot without artificial stack limit");
Check(!inventory.TryAdd(ItemType.Rice,1) && inventory.GetQuantity(ItemType.Rice)==0,"new type blocked at capacity");
Check(!inventory.TryAdd(ItemType.Corn,1),"unregistered type rejected");
Check(!inventory.TryAdd(ItemType.RiceSeed,0) && !inventory.TryAdd(ItemType.RiceSeed,-1),"invalid add quantities rejected");
Check(!inventory.TryRemove(ItemType.RiceSeed,1002) && inventory.GetQuantity(ItemType.RiceSeed)==1001,"overspend atomic");
Check(!inventory.TryRemove(ItemType.RiceSeed,-1),"negative removal rejected");
var snapshot=inventory.GetItems();
Check(inventory.TryRemove(ItemType.RiceSeed,1001) && inventory.UsedSlots==0,"last removal frees slot");
Check(snapshot.Count==1 && snapshot[0].Quantity==1001,"read snapshot remains immutable");
Check(inventory.TryAdd(ItemType.Rice,int.MaxValue),"large quantity uses one slot");
Check(!inventory.TryAdd(ItemType.Rice,1) && inventory.GetQuantity(ItemType.Rice)==int.MaxValue,"overflow cannot wrap");
Check(!inventory.TryRemove((ItemType)999,1),"unknown enum removal rejected");
Check(new Inventory(catalog,0).CanAdd(ItemType.Rice,1)==false,"zero-capacity test warehouse");
bool rejected=false;try{new ItemCatalog(new[]{seed,seed});}catch(ArgumentException){rejected=true;}
Check(rejected,"duplicate catalog type rejected");
Check(catalog.FindItem(ItemType.RiceSeed)==seed && catalog.FindItem(ItemType.Corn)==null,"catalog lookup");
Check(inventory.GetItems().Single().Item.Type==ItemType.Rice,"read uses typed definitions");
Check(ItemIdentifiers.Parse("animal.cowmilk")==ItemType.Milk,"legacy alias maps to same type");
Console.WriteLine($"Passed {count} typed inventory checks.");
