# Second Spring · Prototype tích hợp

Mở `Scenes/integrated_prototype.unity` bằng Unity **6000.6.8f1** và nhấn **Play**. Scene có bản đồ và tài nguyên hình ảnh từ hai bản thử, với một đồng hồ, kho và ví chung.

Các ảnh của scene nằm trong `Art/`. Chọn **Integrated Farm** trong Hierarchy và bấm asset **Artwork** trong trường **Art** để chỉnh atlas cây trồng, ảnh vật nuôi, ảnh giao diện hoặc ảnh hiệu ứng phong cảnh. Các công trình và lớp bản đồ có sprite riêng trên từng `SpriteRenderer`.

`Art/References/` gom ảnh tham chiếu gốc của bản đồ (`game_layout.png`), bảng hình vật nuôi (`asset_animals.png`) và ba ảnh nguồn của Vũ (`PixelFarmAtlas.png`, `FarmItemBags.png`, `FarmCoin.png`). Scene tích hợp dùng trực tiếp ba ảnh nguồn của Vũ trong thư mục này.

## Chơi thử

- Chọn một ô đất để mở thanh công cụ. Chọn cuốc, rồi bấm hoặc giữ chuột trái và rê qua các ô đất. Sau khi xới xong, chọn gói giống và gieo. Tưới ở từng giai đoạn, thu hoạch khi cây chín và xới lại để dọn tàn dư. Phân bón áp dụng cho một ô khi nhả chuột.
- Bấm chuồng hoặc ao để mua giống, cho ăn và thu sản phẩm. Trong chợ có mục **Mua con giống** để mua nhiều con cùng lúc. Thức ăn được lấy từ kho và sản phẩm thu được đưa vào kho để bán.
- Mở **Trò chơi nhỏ** hoặc nhận lời mời trên bản đồ. Câu cá lấy cá trưởng thành từ ao và đưa vào kho. Thắng đập chuột nhận phân bón dùng được trên ruộng.
- Vào **Nhà chính** để chỉnh âm lượng hoặc chọn tốc độ thử `×1`, `×10`, `×50`. Tốc độ này chỉ ảnh hưởng đồng hồ nông trại.

Phiên thử bắt đầu với 1.000 xu và một ít giống, phân, thức ăn. Đóng Play Mode sẽ đặt lại dữ liệu. Scene mở trực tiếp trong Editor. Các thiết lập build của dự án vẫn dùng danh sách scene hiện có.
