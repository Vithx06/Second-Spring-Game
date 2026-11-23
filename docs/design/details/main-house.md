# Thiết kế Nhà chính, thành tựu và lưu game

## Nhà chính, thành tựu và lưu game

- `MainHouseFacade` tập hợp thao tác mở thành tựu, xem tiến độ và lưu game.
- `AchievementManager` quản lý định nghĩa, trạng thái, cập nhật tiến độ và nhận thành tựu; sử dụng `AchievementDefinition`, `AchievementState`, `GameStats`.
- `WinConditionManager` kiểm tra số thành tựu đã nhận, tiền mục tiêu và giới hạn ngày.
- `SaveManager` phụ trách lưu/tải chung; cấu trúc file lưu chưa chốt.
- Luật tại [Nhà chính](../../requirements/main-house.md); không tự bổ sung tên thành tựu, mức tiền mục tiêu hoặc thưởng thành tựu.
