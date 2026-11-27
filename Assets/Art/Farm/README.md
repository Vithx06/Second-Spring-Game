# Farm art và scene dựng sẵn

Mở `Assets/Scenes/MainFarm.unity`, bấm Play. Bấm ô đất để mở dụng cụ, chọn giống hoặc dụng cụ rồi bấm/rê qua các ô; phân bón áp dụng từng ô. Bấm nhà kho để xem giống, sản phẩm và phân bón. Đồng hồ dùng 300 giây thực cho một ngày game. Có thể kéo, đổi kích thước từng đối tượng trong Scene. Phần nghiệp vụ đang nối gồm trồng trọt và kho.

## Các nhóm ảnh

- `Backgrounds`: nền ngày/đêm, đường đi, cỏ, hàng rào; dải trời ngày/đêm riêng.
- `Buildings`: nhà kho, nhà chính, xưởng, chợ, chuồng vịt chính diện; cối xay tách thân và cánh.
- `Decoration`: cổng Second Spring, vườn hoa, bệ đài phun, mây.
- `Water`: hồ cá rời.
- `Effects`: các khung khói, nước phun, bướm và chim.
- `Plants`: trạng thái đất và các giai đoạn cây, 46 sprite có tên.
- `Animals`: chuồng trống, con trưởng thành/con non và các loài cá; không gộp con vật vào ảnh chuồng.
- `Items`: túi giống, sản phẩm, thức ăn và tiền.
- `Minigames`: ảnh hố và chuột.

PNG và sheet đã có cấu hình trong `.meta`: nền trong suốt, Point filter, không mipmap, các sprite được đặt tên. Bấm mũi tên cạnh sheet trong Project để chọn sprite rồi kéo vào Scene.

`FarmSceneMotion.cs` chỉ quay cánh quạt, cho mây/chim trôi và thay khung hình hiệu ứng. `FarmSceneCamera.cs` giữ tỷ lệ hiển thị bản đồ khi đổi kích thước cửa sổ. Hai script không phụ thuộc Model hoặc Controller gameplay.

Ảnh được dựng/chỉnh bằng imagegen theo các mẫu nhóm chốt; một số ảnh có sẵn được sao chép nguyên vẹn. Các đối tượng và vị trí trong scene đã được chỉnh riêng trong Unity.

`PlantSystemController` tạo kho và đồng hồ dùng chung, nối thao tác với `FarmingService`. `FarmPlotController` giữ Model riêng cho từng ô. `PlantPlotView` đổi hình theo trạng thái; `PlantSystemView` vẽ thanh công cụ và kho. Muốn gắn lại tham chiếu scene, dùng menu `Second Spring > Plants > Connect Plants And Inventory`.


