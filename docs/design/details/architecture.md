# Kiến trúc và kết nối các phân hệ

Tài liệu mô tả thiết kế dự kiến, chưa phải mã nguồn đã triển khai.

## Phân công

- Vinh: động vật, Nhà chính/thành tựu, thời gian, minigame; quản lý chung `AchievementManager` và `SaveManager`.
- Vũ: thực vật, kho, chợ (Shipper) và tiền tệ; cung cấp thống kê và trạng thái cần lưu/tải của phân hệ mình.
- Shipper/đơn hàng vẫn thuộc phần bổ sung theo [phạm vi dự án](../../overview.md); chưa có thiết kế chi tiết.

## Phân tầng trong UML v1

- **Giao diện**: ô đất, chợ, chuồng/ao, minigame và Nhà chính; nhận thao tác, hiển thị trạng thái.
- **Ứng dụng**: `FarmingService`, `ShopFacade`, `AnimalPurchaseService`, `AnimalService`, `MinigameManager`, `MainHouseFacade` điều phối các thao tác.
- **Dịch vụ dùng chung và lưu trữ**: kho/vật phẩm, ví tiền, đồng hồ, các interface và `SaveManager`.
- **Nghiệp vụ**: cây, ô đất, danh mục hàng, vật nuôi, chuồng/ao, chiến lược sinh trưởng, minigame và dữ liệu thành tựu.

## Đồng hồ và Observer

- Vinh xây dựng một `GameClock` chung.
- Dùng `IGameTimeListener`: giữ `OnTimeElapsed(deltaTime)` của Vũ, bổ sung `OnHourChanged(day, hour)` và `OnDayChanged(day)` theo UML v1.
- Vũ cập nhật `FarmingService` để tương thích. Các phân hệ đăng ký nhận sự kiện thay vì chạy đồng hồ riêng.
- Thông số và hành vi ở [luật thời gian](../../requirements/time.md); đơn vị `deltaTime` của hợp đồng chung cần được thống nhất khi triển khai.

## Kho, vật phẩm và ví

- Giữ hệ thống `IInventory`, `ItemDefinition`, `IItemCatalog` của Vũ cho thức ăn, trứng, sữa, thịt, cá và phân bón.
- Hai bên thống nhất `itemId`; tài liệu này chưa tự đặt mã vật phẩm.
- Dùng chung `IWallet` và `ShopCatalog`; không nhân bản ví hoặc bảng giá.
- `AnimalPurchaseService` kiểm tra sức chứa, trừ tiền và đưa con non vào chuồng/ao thay vì kho.
- Mua sản phẩm chăn nuôi dạng vật phẩm khác với mua con non sống. Bảng giá tại [giá chăn nuôi](../../requirements/animals/prices.md).

## Strategy và các mẫu thiết kế

- Động vật dùng `IAnimalGrowthStrategy` riêng; không sửa hoặc dùng thay `IFertilizerStrategy` của phần trồng trọt.
- Factory tạo vật nuôi theo loài; Strategy tách quy tắc sinh trưởng/sản xuất; Observer nhận sự kiện đồng hồ.
- Minigame dùng Template Method; chợ của Vũ dùng Facade.
- Chi tiết lớp phần Vinh tại [thiết kế chăn nuôi](animals.md), [thời gian](time.md), [minigame](minigames.md) và [Nhà chính](main-house.md).

## Thành tựu và lưu game

- Vinh quản lý chung `AchievementManager` và `SaveManager`.
- Vũ cung cấp thống kê cùng trạng thái cần lưu/tải của cây, kho, chợ và tiền tệ.
- `MainHouseFacade` trong UML v1 tập hợp thao tác thành tựu, tiến độ và lưu game; `WinConditionManager` kiểm tra điều kiện thắng/thua.
- Chưa chốt cấu trúc file lưu, chi tiết triển khai và kiểm thử. Không xem các phương thức trên UML là chức năng đã hoàn thành.

## Những điểm cần đối chiếu khi triển khai

- Quy đổi là **5 phút thực/ngày game**: dùng `300` giây/ngày cho `GameClock`.
- UML v1 ghi sức chứa mặc định `AnimalPen` là 20; luật đã chốt yêu cầu ao cá 30 con, chuồng còn lại 20 con. Cần cấu hình riêng cho ao.
- UML v1 có `GetProgressPercent()`; luật yêu cầu hiển thị riêng tiến độ tiền và thành tựu. Cần bảo đảm hợp đồng giao diện đáp ứng cả hai.
- Giá chăn nuôi là bội số 5$. Bảng giá thực vật hiện có giá 8$, 12$, 14$…; nếu muốn áp dụng toàn game cần thống nhất riêng với Vũ, không tự sửa bảng thực vật.
- Ưu tiên tiếp theo là đồng bộ các interface chung với Vũ, rồi triển khai theo các mẫu thiết kế đã chọn.
