# Minigame — Vinh


## Lời mời chơi

- Chỉ có hai trò: câu cá (động vật) và đập chuột (thực vật).
- Mỗi giờ game có **20% xác suất** xuất hiện lời mời chơi một trong hai trò; người chơi chọn **Chơi** hoặc **Bỏ qua**.
- Không chạy hai lượt đồng thời. Thời gian game vẫn trôi trong lúc chơi.
- Tỷ lệ chọn giữa hai trò, thời hạn lời mời và xử lý lời mời khi đang chơi chưa được chốt chi tiết.

## Câu cá — Timing

- Mỗi lượt tối đa một con cá. Chỉ cá trưởng thành đang có trong ao mới được câu.
- Độ khó tăng dần: cá rô → cá chép → cá tầm; thanh Timing nhanh dần và vùng chính xác nhỏ dần.
- Có thể câu phải lon nhựa, giày cũ hoặc quần rách. Rác không đưa vào kho; trượt Timing không nhận gì.
- Câu thành công một con cá thì giảm một con tương ứng trong ao.

| Kết quả | Trọng số ban đầu |
|---|---:|
| Cá rô | 35% |
| Cá chép | 25% |
| Cá tầm | 10% |
| Lon nhựa | 10% |
| Giày cũ | 10% |
| Quần rách | 10% |

Loài chưa có cá trưởng thành trong ao bị loại khỏi lựa chọn. Phân phối lại xác suất theo tỷ trọng còn lại, không chia đều. Bảng này là trọng số chọn kết quả, không phải xác suất người chơi vượt qua Timing.

## Đập chuột — Click/Reaction

- Có 9 ô (3×3); chuột chui lên ngẫu nhiên, người chơi click để đập.
- Mỗi lượt kéo dài **30 giây thực tế**, bắt đầu với **3 mạng**.
- Chuột chui xuống mà chưa bị đập thì mất 1 mạng.
- Thắng khi đập ít nhất **20 con** trong 30 giây và còn tối thiểu **1 mạng**.
- Thua không mất gì; thắng nhận **3 đơn vị phân bón ngẫu nhiên**.
- Đề xuất ban đầu: mỗi lượt chỉ có một con chuột; chuột xuất hiện tại một trong 9 ô trong tối đa **30/22 giây thực** (khoảng 1,36 giây) rồi chui xuống. Nếu đập sớm, chuột tiếp theo xuất hiện khi hết khoảng thời gian đó. Có 22 cơ hội trong 30 giây, cho phép bỏ lỡ tối đa 2 con mà vẫn đạt 20 con; thông số có thể điều chỉnh sau khi chơi thử.
- Khi thắng, chọn ngẫu nhiên một trong hai loại phân tăng năng suất hoặc phân chín nhanh, mỗi loại có xác suất 50%; nhận 3 đơn vị cùng loại. Phần thưởng dự kiến cộng trực tiếp vào kho khi tích hợp.

## Chưa chốt

- Tốc độ thanh Timing, kích thước vùng chính xác và thời lượng lượt câu.
- Xử lý phần thưởng khi kho không đủ chỗ.
- Giao diện, sprite, âm thanh, hoạt ảnh và hiệu ứng.
