# Luật trồng trọt — Vũ

## Đất và gieo

- Đất gồm: chưa xới, đang xới, sẵn sàng gieo, có cây, còn tàn dư.
- Xới cần sở hữu cuốc, mất thời gian và tự hoàn thành; nhiều ô được xới đồng thời.
- Không xới trùng ô đang xới. Không gieo khi đất chưa sẵn sàng.
- Gieo cần ít nhất một hạt đúng loại; thành công trừ một hạt và tạo cây mới. Thất bại không mất hạt.

## Phát triển và nước

- Hai giai đoạn; mỗi giai đoạn phải được tưới rồi mới đếm thời gian phát triển.
- Số lần tưới riêng từng giai đoạn: 0 là chờ nước; 1–4 đã có nước; lần 5 làm cây chết. Sang giai đoạn mới đặt lại bộ đếm.
- Tưới thêm không tăng tốc hay đặt lại thời gian phát triển.
- Thiếu nước quá ngưỡng giảm 25% sản lượng gốc; quá ngưỡng chết thì chết.
- Chỉ phạt năng suất một lần/vòng đời; tưới lại không phục hồi tổn thất.


## Thời gian phát triển và sản lượng cơ bản

Quy đổi: **1 ngày game = 5 phút thực**. Các mốc dưới là thông số ban đầu để code và chơi thử, có thể cân bằng lại sau.

| Loại cây | Giai đoạn 1 (thực) | Giai đoạn 2 (thực) | Tổng đến lần chín đầu (thực) | Sản lượng gốc mỗi lần | Thu hoạch |
|---|---|---|---|---:|---|
| Lúa | 1 phút | 1 phút | 2 phút | 3 | Một lần |
| Ngô | 1 phút 30 giây | 1 phút 30 giây | 3 phút | 3 | Một lần |
| Mía | 2 phút | 2 phút | 4 phút | 3 | Một lần |
| Hoa hồng | 1 phút 30 giây | 1 phút 30 giây | 3 phút | 2 | Một lần |
| Hoa lan | 2 phút 30 giây | 2 phút 30 giây | 5 phút | 2 | Một lần |
| Dâu tây | 2 phút | 2 phút | 4 phút | 3 | Nhiều đợt |
| Táo | 4 phút | 4 phút | 8 phút | 4 | Nhiều đợt |
| Cam | 5 phút | 5 phút | 10 phút | 4 | Nhiều đợt |

- Tổng đến lần chín đầu đã gồm cả hai giai đoạn, không gồm thời gian chờ tưới; mỗi giai đoạn chỉ bắt đầu phát triển sau khi tưới.
- Thông số chưa tính phân chín nhanh hoặc tổn thất năng suất.
- Cây nhiều đợt giữ cây sau khi hái; thời gian tái thu hoạch và yêu cầu tưới của đợt mới chưa chốt.
- Ví dụ: táo tưới đúng lúc chín sau 8 phút thực; nếu chờ tưới tổng cộng 1 phút thì chín sau 9 phút.
- Chốt điều kiện thắng sau khi có vòng chơi thực tế và thử cân bằng kinh tế.

## Ngưỡng thiếu nước chung

| Mốc tính từ khi bắt đầu cần nước | Thời gian thực | Kết quả |
|---|---|---|
| 9 giờ 36 phút game | 2 phút | Giảm 25% sản lượng gốc |
| 19 giờ 12 phút game | 4 phút | Cây chết nếu vẫn chưa được tưới |

- Áp dụng giống nhau cho mọi loại cây và cả hai giai đoạn.
- Hai mốc cùng tính từ lúc bắt đầu chờ nước, không cộng nối tiếp. Bộ đếm thiếu nước không chạy khi cây đã được tưới và đang phát triển.
- Khi được tưới, dừng và đặt lại bộ đếm thiếu nước; sang giai đoạn mới bắt đầu lượt chờ nước mới.
- Mức phạt năng suất chỉ áp dụng một lần trong vòng đời, không phục hồi sau tưới và không cộng dồn.

## Phân và sản lượng

- Mỗi cây bón một lần/vòng đời khi đang phát triển; không bón cây chín/chết.
- Chọn tăng 50% năng suất hoặc giảm 25% thời gian phát triển còn lại; vẫn cần tưới đủ hai giai đoạn. Bón phân chín nhanh càng sớm thì càng tiết kiệm thời gian; không rút ngắn thời gian chờ tưới.
- Các mức tăng/giảm tính trên sản lượng gốc: có cả phân năng suất và tổn thất → 125%.
- Làm tròn khi thu hoạch: dưới 0,5 xuống; trên 0,5 lên; đúng 0,5 ngẫu nhiên lên/xuống. Tỉ lệ dự kiến 50/50.
- Lượng phân tiêu hao, thời gian xới và thời gian chín đến thối chưa chốt.

## Thu hoạch và dọn

- Click cây chín để thu hoạch, không cần công cụ.
- Kho không đủ chỗ: giữ nguyên cây và không thêm sản phẩm.
- Cây chín để quá lâu bị thối; thời gian tùy giống, chưa chốt.
- Với cây thu hoạch một lần: thu hoạch bỏ cây, đất còn bã thân; cây chết bỏ cây, đất còn cây thối, không có sản phẩm.
- Xới dọn đưa đất về sẵn sàng gieo, xóa tàn dư.

## Phạm vi

Dự kiến lúa, ngô, mía, táo, cam, dâu tây, hồng, lan. Làm cây thu hoạch một lần trước; cơ chế cây nhiều vụ, nhà kính và sâu bệnh thuộc mở rộng.
