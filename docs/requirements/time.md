# Thời gian — Vinh

Thiết kế: [thời gian và Observer](../design/details/time.md), [kiến trúc chung](../design/details/architecture.md).

## Đồng hồ chung

- Bắt đầu ở **ngày 1, lúc 07:00**.
- **5 phút thực = 1 ngày game; 12,5 giây thực = 1 giờ game**. Một ngày game tương ứng 300 giây thực.
- Hiển thị số ngày hiện tại và đồng hồ kim quay; thời lượng dùng ngày, giờ, phút game.
- Chỉ có một `GameClock` dùng chung cho tất cả phân hệ; phát sự kiện thời gian qua Observer.
- Không có Pause, chỉnh tốc độ, nghỉ ngơi hoặc tua ngày.
- Đồng hồ vẫn chạy khi mở chợ, chuồng, nhà chính và khi chơi minigame.

## Lưu và thoát

- Lưu thủ công tại Nhà chính; tạm ngừng cập nhật trong lúc đang lưu.
- Thời gian không chạy sau khi thoát game.
- Cấu trúc dữ liệu lưu/tải và chi tiết triển khai chưa chốt.

## Ánh sáng

| Giai đoạn | Giờ game | Hiệu ứng |
|---|---|---|
| Bình minh | 05:00–07:00 | Sáng dần |
| Ban ngày | 07:00–17:00 | Ánh sáng bình thường |
| Hoàng hôn | 17:00–19:00 | Tối dần |
| Ban đêm | 19:00–05:00 | Tông màu tối |

Dùng cùng một bộ asset; chuyển ánh sáng 2D dần theo giờ.

## Giới hạn lượt chơi

- Giới hạn 365 ngày; nếu chưa thắng thì thất bại lúc **00:00 ngày 366**.
- Thắng ngay khi đồng thời đủ điều kiện tiền và thành tựu trong thời hạn.
- Chi tiết tại [Nhà chính và điều kiện chiến thắng](main-house.md).
