# Thiết kế chăn nuôi

## Chăn nuôi — Factory và Strategy

- `Animal` là lớp cơ sở trừu tượng, quản lý định danh, loài, tuổi, giai đoạn và tiến độ sinh trưởng; các lớp cụ thể gồm `Chicken`, `Duck`, `Pig`, `Cow`, `Fish`.
- `Fish` có thêm `FishType`: `Perch`, `Carp`, `Sturgeon`, tương ứng cá rô, cá chép, cá tầm.
- `AnimalCreator` khai báo `CreateAnimal()`; `ChickenCreator`, `DuckCreator`, `PigCreator`, `CowCreator`, `FishCreator` tạo từng loại. `FishCreator` nhận loại cá.
- `IAnimalGrowthStrategy` tách cập nhật sinh trưởng, tạo sản phẩm, loại thức ăn và giờ bắt đầu cho ăn.
- Các chiến lược cụ thể là `PoultryGrowthStrategy`, `LivestockGrowthStrategy`, `FishGrowthStrategy`.
- `AnimalProfile` chứa thông số loài: thời gian trưởng thành, chu kỳ sản xuất, số lượng sản phẩm và sản lượng thịt.

Thông số dùng [luật chăn nuôi](../../requirements/animals/rules.md), không suy ra mọi loài có cùng tốc độ từ lớp chiến lược chung.

## Quản lý chuồng/ao và mua con non

- `AnimalPen` giữ danh sách cá thể, sức chứa, ngày đã cho ăn, thức ăn cần và sản phẩm tích lũy; cung cấp thao tác thêm con, cho ăn, cập nhật ngày, thu sản phẩm và loại con trưởng thành.
- `AnimalPenRegistry` tra cứu chuồng/ao theo loại vật nuôi.
- `AnimalService` điều phối chăm sóc, cập nhật theo thời gian, thu sản phẩm và cung cấp cá trưởng thành cho minigame. Service dùng kho và ví chung.
- `AnimalPurchaseService` dùng ví, danh mục hàng, registry và các creator để mua động vật non, đưa trực tiếp vào chuồng/ao.
- Chuồng gà/vịt/lợn/bò có sức chứa 20; ao cá có sức chứa 30. Giá trị mặc định 20 trên UML v1 không áp dụng cho ao cá.

## Kết nối với Vũ

Dùng chung kho, ví, danh mục hàng và interface thời gian theo [kiến trúc chung](architecture.md). Vũ cung cấp thống kê và trạng thái phân hệ để tích hợp thành tựu, lưu/tải. Các interface và chi tiết triển khai cần đồng bộ trước khi viết mã.
