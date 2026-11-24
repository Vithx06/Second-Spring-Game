# Nông trại của Vinh

Mở `Scenes/vinh_prototype.unity` và bấm **Play**. Bấm nhà chính để xem sổ thành tựu, hoặc bấm trực tiếp vào một chuồng để quản lý. Lượt thử bắt đầu lúc 07:00 ngày 1 với một con trưởng thành trong mỗi chuồng và ba loài cá trưởng thành trong ao. Bạn có thể thả thêm cá, cho ăn, thu sản phẩm hoặc thử hai trò chơi qua nút **Trò chơi nhỏ**. Mỗi lần dừng Play, dữ liệu thử được đặt lại.

## Thử ao cá và đồng hồ

Bấm ao ở bên trái. Chọn **Thả cá chép**, đóng bảng bằng **×** và quan sát cá bơi. Bản đồ thể hiện tối đa bốn hình cá theo số con hiện có. Trong trò câu cá, bấm **Giật cần** khi con trỏ vào vùng xanh. Câu được cá sẽ bớt một con trưởng thành của loài đó trong ao.

Một ngày game kéo dài 5 phút thực. Kim và số trên đồng hồ dùng cùng nguồn thời gian. Từ 17:00 cảnh ngả màu hoàng hôn, đến 19:00 hiện trời đêm. Gần sáng, cảnh trở lại ban ngày từ 05:00 đến 07:00.

## Chỉnh hình trong Unity

Các lớp nằm trong **Phong cảnh**, **Nhà và khu nuôi**, **Trang trí và khu vực của Vũ** ở Hierarchy. Chọn một công trình và di chuyển bằng công cụ Move. Vùng bấm và cá bơi theo vị trí của công trình. Khu đất, chợ, kho và xưởng hiện là trang trí để thử bố cục.

`Art/Farm` chứa các sprite tách theo tọa độ của bản thiết kế 1672 × 941, kể cả hai trạng thái của từng chuồng. Công trình và vật trang trí dùng nét vẽ từ bản thiết kế của nhóm. Các nền được tạo bằng ImageGen với yêu cầu giữ nguyên bố cục, vẽ bù mặt cỏ phía sau công trình và tạo biến thể đêm có trăng sao. Hố chuột và chuột trồi lên dùng hai hình cùng phong cách. `positions.json` ghi tọa độ, kích thước gốc để dựng lại cảnh khi cần.

## Phạm vi chơi thử

Số thức ăn và đàn ban đầu là dữ liệu thử. Thêm giống chưa trừ tiền. Thu sản phẩm cập nhật số tích lũy tại chuồng. Việc kết nối kho và ví của Vũ, lưu game, âm lượng, điều kiện của 30 thành tựu và kiểm tra thắng toàn trò chơi còn chờ tích hợp. Luật mẫu chăn nuôi nằm trong `Scripts/FarmModel.cs`.

