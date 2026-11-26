using System;
using System.Linq;
using SecondSpring.Minigames;
using SecondSpring.Prototype;
using UnityEngine;

namespace SecondSpring.IntegratedPrototype
{
    public sealed partial class IntegratedPrototypeView
    {
        private static readonly Rect Window = new Rect(158, 96, 964, 594);
        private static readonly Rect InvitationArea = new Rect(835, 78, 425, 105);
        private static readonly string[] ShopTabs = { "Mua vật phẩm", "Bán vật phẩm", "Mua con giống" };
        private Vector2 _scroll;
        private int _pen;
        private int _marketMode;
        private int _quantity = 1;
        private ShopOffer _selectedOffer;
        private Species _selectedAnimal;
        private Animal _meatConfirmation;

        private void DrawPanel()
        {
            Fill(new Rect(0, 0, Width, Height), new Color(.12f, .15f, .09f, .55f));
            Board(Window);
            if (Button(new Rect(1056, 114, 40, 34), "×"))
            {
                Open(Page.Farm);
                return;
            }
            switch (_page)
            {
                case Page.Pen: DrawPen(); break;
                case Page.Warehouse: DrawWarehouse(); break;
                case Page.Market: DrawMarket(); break;
                case Page.House: DrawHouse(); break;
                case Page.Games: DrawGames(); break;
                case Page.Fishing: DrawFishing(); break;
                case Page.Mole: DrawMole(); break;
            }
        }

        private void PanelTitle(string title, string subtitle)
        {
            Label(new Rect(193, 113, 842, 43), title, _title);
            Label(new Rect(194, 158, 876, 31), subtitle, _small);
            Fill(new Rect(193, 195, 893, 1), Line);
        }

        private void Notice()
        {
            if (string.IsNullOrEmpty(_panelNotice)) return;
            Fill(new Rect(191, 632, 897, 35), Cream);
            Label(new Rect(204, 633, 866, 33), _panelNotice, _small);
        }

        private void OpenPen(int pen)
        {
            _pen = pen;
            _meatConfirmation = null;
            Open(Page.Pen);
        }

        private void DrawPen()
        {
            var farm = _session.Livestock;
            var pen = farm.Pens[_pen];
            int adults = farm.AdultCount(_pen);
            PanelTitle(IntegratedPrototypeSession.Pens[_pen], $"{pen.Animals.Count}/{pen.Capacity} con  ·  {adults} trưởng thành  ·  {pen.Animals.Count - adults} con non");
            Fill(new Rect(191, 215, 515, 395), Cream);
            Label(new Rect(207, 224, 473, 33), "Đàn vật nuôi", _body);
            if (pen.Animals.Count == 0) Label(new Rect(208, 271, 483, 65), "Khu nuôi đang trống. Mua con giống ở chợ hoặc ngay tại đây.", _small);
            else
            {
                _scroll = GUI.BeginScrollView(new Rect(207, 269, 482, 327), _scroll,
                    new Rect(0, 0, 458, Math.Max(325, pen.Animals.Count * 59)));
                for (int i = 0; i < pen.Animals.Count; i++)
                {
                    var animal = pen.Animals[i];
                    int row = i * 59;
                    if (i % 2 == 0) Fill(new Rect(0, row, 457, 56), Paper);
                    Picture(new Rect(6, row + 4, 50, 48), AnimalPicture(animal.Species, animal.Adult));
                    int needed = Farm.RequiredDays(animal.Species);
                    string name = IntegratedPrototypeSession.Animals[(int)animal.Species];
                    string stage = animal.Adult ? "Trưởng thành" : $"Con non · {Math.Min(animal.Growth, needed)}/{needed} ngày chăm";
                    Label(new Rect(63, row + 1, 246, 25), $"{name} {i + 1}  ·  {animal.Age} ngày tuổi", _body);
                    Label(new Rect(64, row + 27, 231, 25), stage, _small);
                    if (_pen == 4 || !animal.Adult) continue;
                    bool confirm = _meatConfirmation == animal;
                    if (!Button(new Rect(318, row + 10, 129, 33), confirm ? "Xác nhận" : "Lấy thịt", confirm)) continue;
                    if (!confirm) { _meatConfirmation = animal; continue; }
                    _panelNotice = _session.CollectMeat(_pen, animal);
                    _meatConfirmation = null;
                    break;
                }
                GUI.EndScrollView();
            }

            string feedId = IntegratedPrototypeSession.FeedItems[farm.FeedType(_pen)];
            int stock = _session.Inventory.GetQuantity(feedId);
            int feedHour = Farm.StartHour(_pen);
            Label(new Rect(730, 213, 344, 38), "Chăm sóc", _body);
            Picture(new Rect(730, 254, 48, 48), _art.feedTextures[farm.FeedType(_pen)]);
            Label(new Rect(794, 254, 282, 26), $"{_session.Inventory.FindItem(feedId).Name} · {stock} trong kho", _small);
            Label(new Rect(794, 280, 282, 25), $"Khẩu phần {pen.FeedRequired} · từ {feedHour:00}:00", _small);
            string status = pen.FedDay == _session.Clock.Day ? "Hôm nay đã cho ăn" : _session.Clock.Hour < feedHour ? "Chưa đến giờ cho ăn" : "Cho ăn";
            if (Button(new Rect(731, 312, 347, 42), status, true, NeedsFeed(_pen))) _panelNotice = _session.Feed(_pen);
            if (_pen == 0 || _pen == 1 || _pen == 3)
            {
                int products = _pen == 3 ? pen.Milk : pen.Eggs;
                if (Button(new Rect(731, 362, 347, 38), $"Thu {products} {(_pen == 3 ? "sữa" : "trứng")}", false, products > 0))
                    _panelNotice = _session.Collect(_pen);
            }
            if (_pen == 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    var species = (Species)((int)Species.Perch + i);
                    if (Button(new Rect(731, 365 + 42 * i, 347, 36), $"Thả {IntegratedPrototypeSession.Animals[(int)species]} · {IntegratedPrototypeSession.Price(species)} xu"))
                        _panelNotice = _session.BuyAnimal(_pen, species, 1);
                }
                if (Button(new Rect(731, 501, 347, 43), "Ra câu cá", true)) EnterGame(MinigameType.Fishing);
            }
            else
            {
                var species = (Species)_pen;
                if (Button(new Rect(731, 418, 347, 39), $"Mua con non · {IntegratedPrototypeSession.Price(species)} xu"))
                    _panelNotice = _session.BuyAnimal(_pen, species, 1);
                if (Button(new Rect(731, 466, 347, 39), "Mở chợ mua thức ăn")) OpenMarket(0, _session.Catalog.FindOffer(feedId));
            }
            Notice();
        }

        private Texture2D AnimalPicture(Species species, bool adult)
        {
            if (species >= Species.Perch) return _art.fishTextures[(int)species - (int)Species.Perch];
            return (adult ? _art.adultTextures : _art.youngTextures)[(int)species];
        }

        private void DrawWarehouse()
        {
            var items = _session.Inventory.GetItems();
            PanelTitle("Nhà kho", $"{items.Count}/{_session.Inventory.Capacity} ô đang dùng  ·  chung cho trồng trọt, chăn nuôi và chợ");
            if (items.Count == 0) Label(new Rect(194, 229, 863, 35), "Kho đang trống.", _body);
            _scroll = GUI.BeginScrollView(new Rect(190, 215, 900, 384), _scroll,
                new Rect(0, 0, 874, Math.Max(381, ((items.Count + 5) / 6) * 120)));
            for (int i = 0; i < items.Count; i++)
            {
                var stack = items[i];
                var r = new Rect(i % 6 * 145, i / 6 * 120, 135, 111);
                Fill(r, Cream);
                DrawItem(new Rect(r.x + 39, r.y + 6, 57, 57), stack.Item.Id);
                Label(new Rect(r.x + 8, r.y + 60, 122, 32), stack.Item.Name, _small);
                Label(new Rect(r.x + 8, r.y + 88, 122, 19), "× " + stack.Quantity.ToString("N0"), _small);
            }
            GUI.EndScrollView();
            if (Button(new Rect(851, 620, 235, 42), "Mở chợ", true)) OpenMarket(0);
        }

        private void OpenMarket(int mode, ShopOffer offer = null)
        {
            Open(Page.Market);
            _marketMode = mode;
            _selectedOffer = offer;
            _quantity = 1;
        }

        private void DrawMarket()
        {
            PanelTitle("Chợ nông sản", "Dùng cùng ví và kho với nông trại. Con giống được đưa thẳng vào khu nuôi.");
            for (int i = 0; i < ShopTabs.Length; i++)
                if (Button(new Rect(192 + i * 192, 210, 181, 39), ShopTabs[i], _marketMode == i) && _marketMode != i) OpenMarket(i);

            Fill(new Rect(192, 262, 538, 356), Cream);
            Fill(new Rect(745, 262, 343, 356), Cream);
            if (_marketMode == 2) DrawAnimalOffers();
            else
            {
                bool buy = _marketMode == 0;
                var offers = _session.Catalog.Offers.Where(offer => buy ? offer.CanBuy
                    : offer.CanSell && _session.Inventory.GetQuantity(offer.Item.Id) > 0).ToArray();
                if (offers.Length == 0) Label(new Rect(209, 278, 486, 34), "Kho chưa có sản phẩm để bán.", _body);
                _scroll = GUI.BeginScrollView(new Rect(202, 270, 518, 341), _scroll,
                    new Rect(0, 0, 498, Math.Max(340, offers.Length * 50)));
                for (int i = 0; i < offers.Length; i++)
                {
                    ShopOffer offer = offers[i];
                    float y = i * 50;
                    if (Button(new Rect(0, y, 496, 45), "", _selectedOffer == offer)) { _selectedOffer = offer; _quantity = 1; _panelNotice = ""; }
                    DrawItem(new Rect(8, y + 4, 38, 37), offer.Item.Id);
                    var style = _selectedOffer == offer ? _light : _button;
                    Label(new Rect(51, y + 4, 280, 37), offer.Item.Name, style);
                    Label(new Rect(339, y + 4, 150, 37), $"{(buy ? offer.BuyPrice : offer.SellPrice)} xu", style);
                }
                GUI.EndScrollView();
                DrawTradeDetail(buy);
            }
            Notice();
        }

        private void DrawTradeDetail(bool buy)
        {
            if (_selectedOffer == null)
            {
                Label(new Rect(765, 283, 300, 70), "Chọn một vật phẩm để xem giá và số lượng.", _body);
                return;
            }
            ShopOffer offer = _selectedOffer;
            int price = buy ? offer.BuyPrice : offer.SellPrice;
            int stock = _session.Inventory.GetQuantity(offer.Item.Id);
            DrawItem(new Rect(877, 274, 77, 77), offer.Item.Id);
            Label(new Rect(767, 358, 298, 36), offer.Item.Name, _body);
            Label(new Rect(767, 399, 299, 27), $"Trong kho: {stock:N0}  ·  Đơn giá: {price} xu", _small);
            Quantity(446, buy ? 999 : Math.Max(1, Math.Min(999, stock)));
            Label(new Rect(767, 498, 300, 31), $"Tổng: {price * _quantity:N0} xu", _body);
            if (!Button(new Rect(767, 549, 299, 45), buy ? "Mua" : "Bán", true, buy || stock >= _quantity)) return;
            TradeResult result = buy ? _session.Market.Buy(offer.Item.Id, _quantity) : _session.Market.Sell(offer.Item.Id, _quantity);
            switch (result)
            {
                case TradeResult.Success:
                    _panelNotice = $"Đã {(buy ? "mua" : "bán")} {_quantity:N0} × {offer.Item.Name}.";
                    if (!buy && _session.Inventory.GetQuantity(offer.Item.Id) == 0) _selectedOffer = null;
                    break;
                case TradeResult.NotEnoughMoney: _panelNotice = "Ví không đủ xu."; break;
                case TradeResult.InventoryFull: _panelNotice = "Kho không đủ chỗ."; break;
                case TradeResult.NotEnoughItems: _panelNotice = "Kho không đủ số lượng để bán."; break;
                default: _panelNotice = "Chưa thực hiện được giao dịch."; break;
            }
        }

        private void DrawAnimalOffers()
        {
            for (int i = 0; i < IntegratedPrototypeSession.Animals.Length; i++)
            {
                var species = (Species)i;
                float y = 272 + i * 47;
                if (Button(new Rect(203, y, 515, 42), "", _selectedAnimal == species))
                {
                    _selectedAnimal = species;
                    _quantity = 1;
                    _panelNotice = "";
                }
                var style = _selectedAnimal == species ? _light : _button;
                Picture(new Rect(217, y + 4, 36, 35), AnimalPicture(species, false));
                Label(new Rect(272, y + 3, 239, 36), IntegratedPrototypeSession.Animals[i], style);
                Label(new Rect(541, y + 4, 156, 35), $"{IntegratedPrototypeSession.Price(species)} xu", style);
            }
            int pen = _selectedAnimal >= Species.Perch ? 4 : (int)_selectedAnimal;
            var target = _session.Livestock.Pens[pen];
            int available = target.Capacity - target.Animals.Count;
            Picture(new Rect(877, 274, 76, 77), AnimalPicture(_selectedAnimal, false));
            Label(new Rect(767, 358, 298, 36), IntegratedPrototypeSession.Animals[(int)_selectedAnimal], _body);
            Label(new Rect(767, 399, 298, 27), $"{IntegratedPrototypeSession.Pens[pen]} · còn {available} chỗ", _small);
            Quantity(446, Math.Max(1, available));
            Label(new Rect(767, 498, 300, 31), $"Tổng: {IntegratedPrototypeSession.Price(_selectedAnimal) * _quantity:N0} xu", _body);
            if (Button(new Rect(767, 549, 299, 45), "Mua con giống", true, available > 0))
                _panelNotice = _session.BuyAnimal(pen, _selectedAnimal, _quantity);
        }

        private void Quantity(float y, int max)
        {
            _quantity = Mathf.Clamp(_quantity, 1, max);
            if (Button(new Rect(768, y, 53, 38), "−", false, _quantity > 1)) _quantity--;
            Label(new Rect(826, y, 96, 38), _quantity.ToString("N0"), _button);
            if (Button(new Rect(936, y, 52, 38), "+", false, _quantity < max)) _quantity++;
            if (Button(new Rect(997, y, 68, 38), "+10", false, _quantity < max)) _quantity = Math.Min(max, _quantity + 10);
        }

        private void DrawItem(Rect rect, string id)
        {
            for (int i = 0; i < _session.Crops.Length; i++)
            {
                var crop = _session.Crops[i];
                bool seed = crop.SeedItemId == id;
                if (!seed && crop.ProductItemId != id) continue;
                _plantArt.DrawItem(rect, i, seed);
                return;
            }
            int feed = Array.IndexOf(IntegratedPrototypeSession.FeedItems, id);
            if (feed >= 0) { Picture(rect, _art.feedTextures[feed]); return; }
            if (id.StartsWith("fish.", StringComparison.Ordinal))
            {
                int fish = Array.IndexOf(FishArtNames, id.Substring(5));
                if (fish >= 0) Picture(rect, _art.fishTextures[fish]);
                return;
            }
            var icon = id == IntegratedPrototypeSession.YieldFertilizer ? _art.toolIcons[(int)CropTool.Yield]
                : id == IntegratedPrototypeSession.GrowthFertilizer ? _art.toolIcons[(int)CropTool.Growth] : _art.crate;
            Picture(rect, icon);
        }

        private void DrawHouse()
        {
            PanelTitle("Nhà chính", "Cài đặt và hướng dẫn chơi thử");
            Fill(new Rect(193, 217, 424, 391), Cream);
            Fill(new Rect(633, 217, 453, 391), Cream);
            Label(new Rect(212, 228, 371, 32), "Âm thanh", _body);
            Label(new Rect(212, 274, 155, 24), "Nhạc nền", _small);
            _music.volume = GUI.HorizontalSlider(new Rect(368, 280, 224, 19), _music.volume, 0, 1);
            Label(new Rect(212, 311, 155, 24), "Hiệu ứng", _small);
            _effects.volume = GUI.HorizontalSlider(new Rect(368, 317, 224, 19), _effects.volume, 0, 1);
            if (Button(new Rect(212, 348, 381, 38), _clickSounds ? "Âm click: bật" : "Âm click: tắt", _clickSounds))
                _clickSounds = !_clickSounds;
            Label(new Rect(212, 410, 370, 28), "Tốc độ thử nghiệm", _body);
            int[] speeds = { 1, 10, 50 };
            for (int i = 0; i < speeds.Length; i++)
                if (Button(new Rect(212 + i * 130, 450, 120, 39), $"×{speeds[i]}", _clockSpeed == speeds[i])) _clockSpeed = speeds[i];
            Label(new Rect(212, 503, 384, 69), "Ở ×1, một ngày game dài 5 phút thực. Tốc độ chỉ áp dụng cho đồng hồ nông trại.", _small);

            Label(new Rect(652, 228, 413, 32), "Cách chơi", _body);
            Label(new Rect(652, 272, 413, 79), "Chọn ô đất để mở công cụ. Xới đất, gieo giống, tưới ở từng giai đoạn và thu hoạch khi cây chín.", _small);
            Label(new Rect(652, 356, 413, 101), "Bấm chuồng hoặc ao để mua giống và cho ăn. Thu sản phẩm vào kho, rồi đến chợ để bán và tái đầu tư.", _small);
            Label(new Rect(652, 460, 413, 62), "Dữ liệu chơi thử được đặt lại mỗi lần khởi động scene.", _small);
            if (_session.PendingFertilizer > 0 &&
                Button(new Rect(652, 545, 414, 40), $"Nhận phân thưởng ({_session.PendingFertilizer})", true))
            {
                int amount = _session.CollectPendingFertilizer();
                _panelNotice = amount == 0 ? "Kho chưa còn chỗ nhận phân." : $"Đã đưa {amount} đơn vị phân vào kho.";
            }
            Notice();
        }

        private void DrawInvitation()
        {
            var invitation = _session.Invitations.Invitation;
            if (!invitation.HasValue) return;
            Board(InvitationArea);
            Label(new Rect(850, 87, 394, 34), invitation == MinigameType.Fishing ? "Lời mời câu cá" : "Lời mời đập chuột", _body);
            if (Button(new Rect(850, 130, 203, 36), "Chơi", true) && _session.AcceptInvitation())
                Open(invitation == MinigameType.Fishing ? Page.Fishing : Page.Mole);
            if (Button(new Rect(1062, 130, 182, 36), "Bỏ qua")) _session.Invitations.DismissInvitation();
        }

        private void EnterGame(MinigameType type)
        {
            _session.StartGame(type);
            Open(type == MinigameType.Fishing ? Page.Fishing : Page.Mole);
        }

        private void DrawGames()
        {
            PanelTitle("Trò chơi nhỏ", "Đồng hồ nông trại tiếp tục chạy khi chơi.");
            Fill(new Rect(192, 220, 434, 379), Cream);
            Fill(new Rect(650, 220, 436, 379), Cream);
            Picture(new Rect(222, 235, 375, 175), _pond.sprite.texture);
            Picture(new Rect(678, 235, 378, 175), _art.moleArt);
            Label(new Rect(219, 419, 375, 35), "Câu cá", _body);
            Label(new Rect(675, 419, 385, 35), "Đập chuột", _body);
            Label(new Rect(219, 459, 375, 61), "Giật cần trong vùng xanh. Cá bắt được sẽ vào kho.", _small);
            Label(new Rect(675, 459, 385, 61), "Đập đủ 20 con trong 30 giây để nhận 3 đơn vị phân.", _small);
            if (Button(new Rect(215, 539, 386, 43), "Câu cá", true)) EnterGame(MinigameType.Fishing);
            if (Button(new Rect(672, 539, 389, 43), "Đập chuột", true)) EnterGame(MinigameType.WhackAMole);
        }

        private void DrawFishing()
        {
            var game = (FishingMinigame)_session.Game;
            PanelTitle("Câu cá", "Bấm Giật cần khi phao đi vào vùng xanh.");
            Texture2D picture = game.IsFinished && game.Result.Outcome == FishingOutcome.Fish && game.Result.Catch.HasValue
                ? _art.fishTextures[(int)game.Result.Catch.Value] : _pond.sprite.texture;
            Picture(new Rect(267, 209, 396, 228), picture);
            string pond = _session.Livestock.AdultCount(4) == 0 ? "Ao chưa có cá trưởng thành." : $"Trong ao có {_session.Livestock.AdultCount(4)} cá trưởng thành.";
            Label(new Rect(700, 245, 350, 61), game.IsFinished ? _session.GameResult : pond, _body);
            if (!game.IsFinished)
                Label(new Rect(700, 323, 350, 40), $"Còn {Mathf.CeilToInt((float)(FishingMinigame.DurationSeconds - game.ElapsedSeconds))} giây", _body);
            const float left = 255, span = 771;
            Fill(new Rect(left - 3, 458, span + 6, 24), Wood);
            Fill(new Rect(left, 461, span, 18), Line);
            Fill(new Rect(left + (float)game.ZoneStart * span, 461, (float)game.ZoneWidth * span, 18), Leaf);
            Fill(new Rect(left + (float)game.MarkerPosition * span - 3, 451, 6, 38), Color.white);
            if (!Button(new Rect(505, 531, 270, 52), game.IsFinished ? "Câu lại" : "Giật cần", true)) return;
            if (game.IsFinished) EnterGame(MinigameType.Fishing);
            else game.Pull();
        }

        private void DrawMole()
        {
            var game = (WhackAMoleMinigame)_session.Game;
            PanelTitle("Đập chuột", "Đập ít nhất 20 con và giữ ít nhất 1 mạng đến hết lượt.");
            for (int i = 0; i < 9; i++)
            {
                var r = new Rect(211 + i % 3 * 148, 228 + i / 3 * 112, 135, 103);
                if (Button(r, "", game.ActiveHole == i, !game.IsFinished)) game.Hit(i);
                Picture(new Rect(r.x + 3, r.y + 3, r.width - 6, r.height - 6), game.ActiveHole == i ? _art.moleArt : _art.hole);
            }
            Fill(new Rect(678, 228, 399, 324), Cream);
            Label(new Rect(705, 248, 345, 43), $"Thời gian  {Mathf.CeilToInt((float)(WhackAMoleMinigame.DurationSeconds - game.ElapsedSeconds))} giây", _body);
            Label(new Rect(705, 296, 345, 43), $"Mạng  {game.Lives}/3", _body);
            Label(new Rect(705, 344, 345, 43), $"Điểm  {game.Hits}/{WhackAMoleMinigame.TargetHits}", _body);
            if (!game.IsFinished) return;
            Label(new Rect(705, 397, 345, 71), _session.GameResult, _small);
            if (Button(new Rect(705, 484, 345, 43), "Chơi lại", true)) EnterGame(MinigameType.WhackAMole);
        }
    }
}
