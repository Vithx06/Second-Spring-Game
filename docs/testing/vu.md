# Kiểm thử phần Vũ

## 1. Kiểm thử bằng chơi game

- Thao tác chuột: chọn đồ, xới, gieo, tưới, bón phân, thu hoạch và mở/đóng kho.
- Kiểm tra hình ảnh, thông báo và trạng thái giao diện khớp với dữ liệu.
- Chơi một vòng trồng trọt, thu hoạch, bán sản phẩm và mua giống mới.

## 2. Kiểm thử logic C# bằng Unity Test Framework

| ID | Ca kiểm thử | Kết quả mong đợi | Kết quả thực tế |
|---|---|---|---|
| V01 | Xới đất chưa xới khi có cuốc | Chuyển đang xới, đặt thời gian | Chưa chạy |
| V02 | Xới lại ô đang xới | Từ chối, không đặt lại tiến độ | Chưa chạy |
| V03 | Xới xong ô có tàn dư | Sẵn sàng gieo, không còn tàn dư | Chưa chạy |
| V04 | Gieo vào đất không sẵn sàng | Không mất hạt, không tạo cây | Chưa chạy |
| V05 | Gieo hợp lệ với đúng một hạt | Tạo một cây, hạt còn 0 | Chưa chạy |
| V06 | Gieo khi hết hạt | Không tạo cây | Chưa chạy |
| V07 | Chưa tưới rồi tưới lần đầu | Trước tưới chưa lớn; sau tưới bắt đầu tiến độ | Chưa chạy |
| V08 | Tưới lần 5 trong một giai đoạn | Cây chết, đất có cây thối | Chưa chạy |
| V09 | Thiếu nước qua ngưỡng phạt rồi tưới | Giảm 25% một lần, không phục hồi | Chưa chạy |
| V10 | Bón phân lần hai hoặc bón cây chín | Từ chối, không mất phân | Chưa chạy |
| V11 | Kho đầy rồi thu hoạch | Cây giữ nguyên, không thêm sản phẩm | Chưa chạy |
| V12 | Thêm vật phẩm cùng loại vào ô đang có | Cộng dồn đúng số lượng, không tạo ô khác loại | Chưa chạy |
| V13 | Trừ nhiều hơn số vật phẩm đang có | Từ chối, số lượng không âm | Chưa chạy |
| V14 | Thu hoạch cây năng suất gốc 8, phân năng suất và tổn thất | Nhận 10 sản phẩm, đất còn bã | Chưa chạy |
