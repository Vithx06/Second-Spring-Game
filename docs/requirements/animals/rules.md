# Luật chăn nuôi — Vinh

Xem thêm bảng giá và thời gian chung.

## Loài và sinh trưởng

| Loài | Trưởng thành sau | Sản phẩm khi trưởng thành |
|---|---|---|
| Gà | 3 ngày | 1 trứng/con/ngày; 2 thịt/con khi lấy thịt |
| Vịt | 4 ngày | 1 trứng/con/2 ngày; 3 thịt/con khi lấy thịt |
| Lợn | 5 ngày | 6 thịt/con khi lấy thịt |
| Bò | 7 ngày | 2 sữa/con/2 ngày; 8 thịt/con khi lấy thịt |
| Cá rô | 2 ngày | 1 cá/lượt câu thành công |
| Cá chép | 4 ngày | 1 cá/lượt câu thành công |
| Cá tầm | 7 ngày | 1 cá/lượt câu thành công |

- Có đúng hai giai đoạn: con non và trưởng thành. Gà, vịt, lợn, bò chỉ có một giống; cá có ba loài.
- Người chơi mua con non về nuôi; không có sinh sản, bệnh tật hoặc chết đói.
- `Age` vẫn tăng theo ngày; `GrowthProgress` chỉ tăng khi chuồng đã được cho ăn hợp lệ. Thời gian trưởng thành trong bảng cần đủ số ngày tăng tiến độ tương ứng.
- Mỗi cá thể có `Id`, `AnimalType`, `Age`, `GrowthStage`, `GrowthProgress`.
- Không quản lý giới tính, sức khỏe, tâm trạng hoặc thanh đói riêng.

## Chuồng và ao

- Gà, vịt, lợn, bò: mỗi loài có chuồng riêng, tối đa **20 con/chuồng**.
- Ao cá chứa tối đa **30 con tổng cộng**, gồm cả ba loài.
- Động vật mua về được đưa trực tiếp vào chuồng/ao, không đưa vào kho.

## Cho ăn theo lịch cố định

| Công trình | Giờ bắt đầu | Hạn cuối |
|---|---|---|
| Chuồng gà | 07:00 | Trước 00:00 ngày sau |
| Chuồng vịt | 09:00 | Trước 00:00 ngày sau |
| Chuồng lợn | 11:00 | Trước 00:00 ngày sau |
| Chuồng bò | 14:00 | Trước 00:00 ngày sau |
| Ao cá | 17:00 (hoàng hôn) | Trước 00:00 ngày sau |

- Đến giờ bắt đầu, biểu tượng cần cho ăn hiện ngay phía trên công trình. Người chơi chủ động nhấn cho ăn trong khung giờ hợp lệ.
- Cho ăn đồng loạt cả chuồng/ao, tối đa một lần/ngày; chuồng trống không cần cho ăn.
- Một đơn vị thức ăn đủ cho tối đa 5 con/ngày. Lượng cần dùng = **làm tròn lên (số con / 5)**.
- Hệ thống tự trừ thức ăn trong kho; nếu không đủ thì không thực hiện.

| Số con | Đơn vị thức ăn/ngày |
|---|---:|
| 1–5 | 1 |
| 6–10 | 2 |
| 11–15 | 3 |
| 16–20 | 4 |
| 21–25 | 5 |
| 26–30 | 6 |

Gà và vịt dùng thức ăn gia cầm; lợn và bò dùng thức ăn gia súc; cá dùng thức ăn cho cá.

Sang ngày mới, chuồng chưa được cho ăn bị tính là bỏ lỡ ngày trước: không tăng tiến độ sinh trưởng và không tạo trứng/sữa. Ngày sau hoạt động bình thường khi cho ăn lại.

## Thu sản phẩm

- Chỉ con trưởng thành mới cho sản phẩm.
- Trứng/sữa tích lũy tại chuồng; người chơi chủ động thu vào kho dùng chung.
- Lấy thịt sẽ loại con vật tương ứng khỏi chuồng. Không bán động vật sống.
- Chỉ câu cá trưởng thành có trong ao; mỗi lượt thành công giảm một con cá tương ứng trong ao. Luật chơi tại minigame.
- Cách xử lý thu sản phẩm khi kho đầy, chọn cá thể lấy thịt, mua thêm con sau lượt cho ăn và thời điểm sản xuất đầu tiên chưa chốt; cần thống nhất khi triển khai.
