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
## Cấu trúc thư mục dự kiến

Bấm vào từng thư mục để mở hoặc thu gọn. Các mục dưới đây gồm cả thư mục sẽ bổ sung khi triển khai.

<details>
<summary>📁 Second-Spring-Game</summary>

- `README.md`: giới thiệu và liên kết tài liệu.
- `CONTRIBUTING.md`: quy tắc làm việc chung.
- `LICENSE`: giấy phép.
- `.gitignore`: loại trừ file sinh tự động.

<details>
<summary>📁 Assets — mã nguồn và tài nguyên</summary>

<details>
<summary>📁 Scripts — tổ chức theo chức năng</summary>

- `Plants/`: ô đất, cây, enum, Factory, Strategy và dịch vụ trồng trọt — Vũ.
- `Inventory/`: kho và vật phẩm dùng chung — Vũ.
- `Economy/`: ví tiền — Vũ.
- `Shop/`: mua bán, danh mục hàng và Facade — Vũ.
- `Animals/`: chăn nuôi — Vinh.
- `Time/`: đồng hồ dùng chung — Vinh.
- `Achievements/`: thành tựu, tiến độ nhà chính — Vinh.
- `Minigames/`: minigame — Vinh.
- `UI/`: giao diện và nhận thao tác người chơi.
- `Save/`: lưu và tải tiến trình.

</details>

- `Scenes/`: scene chung và scene thử riêng của từng người.
- `Prefabs/`: đối tượng Unity dùng lại.
- `Art/`: hình cây, vật nuôi, môi trường và giao diện.
- `Audio/`: nhạc và hiệu ứng âm thanh.
- `Settings/`: cấu hình hiển thị, render.
- `Tests/`: kiểm thử logic và tích hợp Unity.

</details>

<details>
<summary>📁 docs — tài liệu</summary>

- `overview.md`: ý tưởng, phạm vi và điều kiện thắng/thua.
- `setup.md`: cách mở, chạy và cấu trúc dự án.

<details>
<summary>📁 requirements — luật chơi</summary>

- `shared.md`: quy tắc chung về thời gian, tiền và mua bán.
- `plants.md`: luật trồng trọt.
- `plant-prices.md`: bảng giá thực vật.
- `inventory.md`: quy tắc kho.
- `animals/`: luật chăn nuôi — Vinh.

</details>

<details>
<summary>📁 design — thiết kế</summary>

- `architecture.md`: phân tầng và interface dùng chung.
- `plants.md`: class và quan hệ phần thực vật.
- `inventory.md`: thiết kế kho, tiền và chợ.
- `patterns.md`: vai trò và lý do dùng Design Pattern.
- `animals/`: thiết kế phần Vinh.
- `diagrams/`: file `.drawio` và ảnh sơ đồ.

</details>

<details>
<summary>📁 testing — kiểm thử</summary>

- `vu.md`: kiểm thử phần Vũ.
- `animals/`: kiểm thử phần Vinh.
- `integration.md`: kiểm thử kết nối các hệ thống.

</details>

<details>
<summary>📁 delivery — bàn giao</summary>

- `checklist.md`: đối chiếu barem với minh chứng.
- `demo.md`: kịch bản demo.

</details>

</details>

- `Packages/`: danh sách và phiên bản package.
- `ProjectSettings/`: phiên bản Unity và cấu hình dự án.

</details>

Giữ file `.meta` cùng asset. Đổi tên hoặc di chuyển asset trong Unity để giữ liên kết.
