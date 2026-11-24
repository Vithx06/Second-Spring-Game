# Second Spring — Game mô phỏng nông trại

**Bài tập cuối kỳ: Lập trình hướng đối tượng — MI4090**  
Nhóm 77 · Mã lớp 174373 · Học kỳ 20261

Người chơi xây dựng trang trại bằng trồng trọt, chăn nuôi và mua bán để tích lũy vốn khởi nghiệp. Dự án tập trung vào thiết kế và cài đặt hướng đối tượng bằng C# trong Unity.

| Thành viên | MSSV | GitHub | Phần phụ trách |
|---|---|---|---|
| Đinh Thế Vinh | 202419017 | [Vithx06](https://github.com/Vithx06) | Động vật, minigame, thành tựu, thời gian |
| Phạm Đình Vũ | 202419021 | [dinhvu07112k6-cmyk](https://github.com/dinhvu07112k6-cmyk) | Thực vật, kho, tiền, chợ |

## Ý tưởng và phạm vi

- Cốt truyện: nhân vật tốt nghiệp, chưa có việc, về quê thăm ông và nhận thử thách khởi nghiệp trên mảnh đất bỏ hoang.
- Mục tiêu: đạt số tiền mục tiêu trong ví và đã nhận ít nhất 25/30 thành tựu trong 365 ngày game; thắng ngay khi đồng thời đủ điều kiện. Nếu chưa thắng thì thất bại lúc 00:00 ngày 366. Mục tiêu tiền chưa chốt; danh sách thành tựu xem Nhà chính.
- Vòng chơi: mua giống → trồng trọt, chăn nuôi → thu sản phẩm → bán → tái đầu tư.
- Trồng trọt và chăn nuôi dùng chung kho, ví tiền và đồng hồ.
- Thời gian: 5 phút thực = 1 ngày game, bắt đầu ngày 1 lúc 07:00. Dùng chung GameClock; thời lượng hiển thị bằng ngày, giờ, phút game. Không Pause, chỉnh tốc độ hoặc tua ngày; đồng hồ vẫn chạy khi mở giao diện và chơi minigame, chỉ tạm ngừng cập nhật trong lúc lưu. Thời gian không chạy sau khi thoát game; xem thời gian và ánh sáng.
- Kho: 100 ô, mỗi ô chứa một loại vật phẩm; không đủ chỗ thì không nhận thêm. Không làm thông báo hết hạn trong kho.
- Thực vật: xới đất → gieo → tưới, bón phân → thu hoạch → dọn tàn dư. Cuốc có sẵn, dùng vĩnh viễn, không cần mua.
- Vật nuôi: gà, vịt, bò, lợn, cá rô/cá chép/cá tầm; mua con non, cho ăn theo lịch để trưởng thành và thu sản phẩm. Không sinh sản, bệnh tật hoặc chết đói; xem luật chăn nuôi.
- Chợ có mục mua/bán, dùng chung dữ liệu với kho và ví tiền; nhà chính hiển thị tiến độ và thành tựu.
- Bản cơ bản chỉ điều khiển bằng chuột, chưa có nhân vật điều khiển; có lưu/tải, chỉnh âm lượng và thoát game.
- Làm luồng cơ bản chạy được trước; triển khai hai minigame đã chốt luật sau: câu cá và đập chuột. Xưởng chế biến, shipper/đơn hàng thuộc phần bổ sung.
- Mở rộng: nhân vật điều khiển, nhà kính, thời tiết, sâu bệnh và nâng cấp công cụ.

## Mở và chạy dự án

Project Unity nằm tại thư mục gốc của repository, cùng với `Assets/`, `Packages/` và `ProjectSettings/`.

1. Cài Unity Editor **6000.6.8f1** qua Unity Hub (theo ProjectSettings/ProjectVersion.txt).
2. Clone repository.
3. Unity Hub → Add project from disk → chọn thư mục gốc repo có Assets, Packages và ProjectSettings.
4. Mở bằng đúng phiên bản; chờ Unity tạo Library và tải package cần thiết.
5. Để thử bản prototype, mở `Assets/VinhPrototype/Scenes/vinh_prototype.unity` và nhấn Play. Xem hướng dẫn prototype.
6. Nếu Console có lỗi biên dịch, xử lý trước khi thử chức năng.

### Quản lý file

- Commit Assets (kèm .meta), Packages, ProjectSettings và .gitignore.
- Library, Temp, Logs và UserSettings được tạo lại trên từng máy, không đưa lên Git.
- Không làm luân phiên ở hai bản project khác thư mục.
- Thực hiện branch, commit và PR theo CONTRIBUTING.

### Điều khiển dự kiến

- Bản cơ bản chỉ dùng chuột, chưa có nhân vật điều khiển.
- Click trái thanh đồ: chọn công cụ hoặc vật phẩm; click lại để bỏ chọn.
- Click trái ô đất/cây: xới, gieo, tưới, bón phân hoặc thu hoạch theo lựa chọn hiện tại.
- Click trái biểu tượng kho hoặc menu để mở; click nút đóng để đóng bảng.
- Lưu/tải, âm lượng và thoát game đều thao tác bằng các nút giao diện.
- Nhân vật điều khiển để mở rộng khi còn thời gian.

Điều khiển và hướng dẫn build sẽ được cập nhật theo bản thực thi thực tế.

## Cấu trúc thư mục

Code nghiệp vụ chính nằm trong `Assets/Scripts/`; code chơi thử nằm riêng trong `Assets/VinhPrototype/`. Một số thư mục vẫn đang chờ triển khai.

```text
Second-Spring-Game/
├── Assets/
│   ├── Scripts/                  # Code game chính, chia theo chức năng
│   │   ├── Achievements/         # Thành tựu
│   │   ├── Animals/              # Chăn nuôi
│   │   ├── Economy/              # Ví tiền
│   │   ├── Exceptions/           # Ngoại lệ nghiệp vụ dự kiến
│   │   │   ├── Animals/          # Chuồng và vật nuôi
│   │   │   ├── Economy/          # Ví tiền
│   │   │   ├── Inventory/        # Kho và vật phẩm
│   │   │   ├── Plants/           # Trồng trọt
│   │   │   ├── Shop/             # Giao dịch
│   │   │   └── Time/             # Đồng hồ game
│   │   ├── Inventory/            # Kho, vật phẩm
│   │   ├── MainHouse/            # Nhà chính và điều kiện thắng/thua
│   │   ├── Minigames/            # Trò chơi nhỏ
│   │   ├── Plants/               # Cây trồng và ô đất
│   │   ├── Save/                 # Lưu và tải
│   │   ├── Shared/               # Interface và thành phần dùng chung
│   │   ├── Shop/                 # Mua bán
│   │   ├── Time/                 # Đồng hồ chung
│   │   └── UI/                   # Giao diện
│   ├── Art/                      # Hình ảnh dùng chung
│   ├── Audio/                    # Nhạc và hiệu ứng
│   ├── Data/                     # Dữ liệu cấu hình game
│   ├── Prefabs/                  # Đối tượng Unity dùng lại
│   ├── Scenes/                   # Scene chung, hiện có SampleScene
│   ├── Settings/                 # Cấu hình đồ họa và input
│   ├── Tests/                    # Kiểm thử
│   │   ├── Unit/                 # Kiểm thử đơn vị
│   │   ├── Integration/          # Kiểm thử tích hợp
│   │   └── System/               # Kiểm thử toàn hệ thống
│   ├── VinhPrototype/            # Bản chơi thử của Vinh
│   │   ├── Art/                  # Hình ảnh; Farm/ chứa sprite và tọa độ
│   │   ├── Editor/               # Công cụ dựng scene
│   │   ├── Scenes/               # Scene vinh_prototype
│   │   ├── Scripts/              # Logic và giao diện chơi thử
│   │   └── README.md             # Hướng dẫn chơi thử
│   ├── VuPrototype/              # Thư mục chơi thử của Vũ, hiện còn trống
│   │   ├── Art/                  # Hình ảnh chơi thử
│   │   ├── Editor/               # Công cụ Unity Editor
│   │   ├── Scenes/               # Scene chơi thử
│   │   └── Scripts/              # Code chơi thử
│   └── Welcome/                  # Nội dung mẫu Unity
├── docs/
│   ├── design/
│   │   ├── diagrams/             # Sơ đồ lớp UML
│   │   └── layouts/              # Bố cục nông trại
│   ├── requirements/
│   │   ├── animals/              # Luật và giá vật nuôi
│   │   ├── plants/               # Luật và giá cây trồng
│   │   └── shared/               # Kho, thời gian, nhà chính, minigame
│   └── testing/                  # Thư mục kiểm thử, hiện còn trống
├── Packages/                     # Package Unity
├── ProjectSettings/              # Cấu hình dự án Unity
├── .gitignore                    # File và thư mục không đưa lên Git
├── CONTRIBUTING.md               # Quy tắc đóng góp
├── LICENSE                       # Giấy phép
└── README.md                     # Giới thiệu và hướng dẫn dự án
```

Các file `.meta` và một số file con được lược bớt trong cây. Giữ file `.meta` cùng asset; đổi tên hoặc di chuyển asset trong Unity để giữ liên kết.

Các class trong `Assets/Scripts/Exceptions/` là ngoại lệ dự kiến, chưa được tích hợp vào nghiệp vụ. Nhóm sẽ thống nhất cách sử dụng khi phát triển từng phân hệ.

## Tài liệu

- [Trồng trọt](docs/requirements/plants/rules.md) · [Bảng giá](docs/requirements/plants/prices.md)
- [Chăn nuôi](docs/requirements/animals/rules.md) · [Bảng giá](docs/requirements/animals/prices.md)
- [Kho](docs/requirements/shared/inventory.md) · [Thời gian](docs/requirements/shared/time.md) · [Minigame](docs/requirements/shared/minigames.md) · [Nhà chính và thành tựu](docs/requirements/shared/main-house.md)
- [UML v1 — sơ đồ lớp phân tầng](docs/design/diagrams/Class_Diagram_v1.png)
- [Bố cục game](docs/design/layouts/game_layout.png)
- [Quy tắc đóng góp](CONTRIBUTING.md)
