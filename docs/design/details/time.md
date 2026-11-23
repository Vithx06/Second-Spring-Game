# Thiết kế thời gian

## Thời gian — Observer

- `GameClock` quản lý ngày/giờ và danh sách `IGameTimeListener`.
- Listener nhận `OnTimeElapsed(deltaTime)`, `OnHourChanged(day, hour)`, `OnDayChanged(day)`.
- Chăn nuôi nhận sự kiện để hiện nhu cầu cho ăn theo giờ và cập nhật tuổi, sinh trưởng, sản xuất theo ngày.
- Đồng hồ bắt đầu ngày 1 lúc 07:00; quy đổi là **5 phút thực/ngày**, tương ứng `300` giây/ngày.
- Quy tắc vận hành và ánh sáng tại [thời gian](../../requirements/time.md).
