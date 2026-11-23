# Thiết kế minigame

## Minigame — Template Method

- Lớp trừu tượng `Minigame` quy định quy trình khởi tạo → chơi → kiểm tra kết quả → kết thúc và trả kết quả.
- UML v1 biểu diễn các bước bằng `Start()`, `Setup()`, `PlayCore()`, `EvaluateResult()`, `BuildReward()`.
- `FishingMinigame` và `WhackAMoleMinigame` cài đặt cách chơi riêng.
- `MinigameManager` nhận sự kiện giờ, quản lý lời mời, lượt đang chạy và áp dụng kết quả.
- `MinigameResult` mang kết quả thắng/thua, phần thưởng và thông tin cá tiêu thụ khi câu thành công.
- Luật và trọng số tại [minigame](../../requirements/minigames.md); các tham số độ khó chưa chốt.
