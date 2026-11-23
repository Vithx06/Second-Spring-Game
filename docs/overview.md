# Ý tưởng và phạm vi

- Cốt truyện: nhân vật tốt nghiệp, chưa có việc, về quê thăm ông và nhận thử thách khởi nghiệp trên mảnh đất bỏ hoang.
- Mục tiêu: đạt số tiền mục tiêu trong ví và đã nhận ít nhất 25/30 thành tựu trong 365 ngày game; thắng ngay khi đồng thời đủ điều kiện. Nếu chưa thắng thì thất bại lúc 00:00 ngày 366. Mục tiêu tiền và nội dung cụ thể của thành tựu chưa chốt; xem [Nhà chính](requirements/main-house.md).
- Vòng chơi: mua giống → trồng trọt, chăn nuôi → thu sản phẩm → bán → tái đầu tư.
- Trồng trọt và chăn nuôi dùng chung kho, ví tiền và đồng hồ.
- Thời gian: 5 phút thực = 1 ngày game, bắt đầu ngày 1 lúc 07:00. Dùng chung GameClock; thời lượng hiển thị bằng ngày, giờ, phút game. Không Pause, chỉnh tốc độ hoặc tua ngày; đồng hồ vẫn chạy khi mở giao diện và chơi minigame, chỉ tạm ngừng cập nhật trong lúc lưu. Thời gian không chạy sau khi thoát game; xem [thời gian và ánh sáng](requirements/time.md).
- Kho: 100 ô, mỗi ô chứa một loại vật phẩm; không đủ chỗ thì không nhận thêm. Không làm thông báo hết hạn trong kho.
- Thực vật: xới đất → gieo → tưới, bón phân → thu hoạch → dọn tàn dư. Cuốc có sẵn, dùng vĩnh viễn, không cần mua.
- Vật nuôi: gà, vịt, bò, lợn, cá rô/cá chép/cá tầm; mua con non, cho ăn theo lịch để trưởng thành và thu sản phẩm. Không sinh sản, bệnh tật hoặc chết đói; xem [luật chăn nuôi](requirements/animals/rules.md).
- Chợ có mục mua/bán, dùng chung dữ liệu với kho và ví tiền; nhà chính hiển thị tiến độ và thành tựu.
- Bản cơ bản chỉ điều khiển bằng chuột, chưa có nhân vật điều khiển; có lưu/tải, chỉnh âm lượng và thoát game.
- Làm luồng cơ bản chạy được trước; triển khai hai [minigame đã chốt luật](requirements/minigames.md) sau: câu cá và đập chuột. Xưởng chế biến, shipper/đơn hàng thuộc phần bổ sung.
- Mở rộng: nhân vật điều khiển, nhà kính, thời tiết, sâu bệnh và nâng cấp công cụ.
