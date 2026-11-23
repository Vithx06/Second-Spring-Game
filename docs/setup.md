# Mở và chạy dự án

1. Cài Unity Editor **6000.6.8f1** qua Unity Hub (theo ProjectSettings/ProjectVersion.txt).
2. Clone repository.
3. Unity Hub → Add project from disk → chọn thư mục gốc repo có Assets, Packages và ProjectSettings.
4. Mở bằng đúng phiên bản; chờ Unity tạo Library và tải package cần thiết.
5. Mở Assets/Scenes/SampleScene.unity nếu có, nhấn Play. Scene demo gameplay chính sẽ cập nhật sau.
6. Nếu Console có lỗi biên dịch, xử lý trước khi thử chức năng.

## Quản lý file

- Commit Assets (kèm .meta), Packages, ProjectSettings và .gitignore.
- Library, Temp, Logs và UserSettings được tạo lại trên từng máy, không đưa lên Git.
- Không làm luân phiên ở hai bản project khác thư mục.
- Thực hiện branch, commit và PR theo [CONTRIBUTING](../CONTRIBUTING.md).

## Điều khiển dự kiến

- Bản cơ bản chỉ dùng chuột, chưa có nhân vật điều khiển.
- Click trái thanh đồ: chọn công cụ hoặc vật phẩm; click lại để bỏ chọn.
- Click trái ô đất/cây: xới, gieo, tưới, bón phân hoặc thu hoạch theo lựa chọn hiện tại.
- Click trái biểu tượng kho hoặc menu để mở; click nút đóng để đóng bảng.
- Lưu/tải, âm lượng và thoát game đều thao tác bằng các nút giao diện.
- Nhân vật điều khiển để mở rộng khi còn thời gian.

Điều khiển và hướng dẫn build sẽ được cập nhật theo bản thực thi thực tế.