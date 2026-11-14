# Hướng dẫn đóng góp

Tài liệu quy định cách quản lý mã nguồn, tổ chức công việc và phối hợp giữa các thành viên nhằm đảm bảo dự án được phát triển nhất quán, hạn chế xung đột và thuận tiện trong việc kiểm tra, bảo trì.

## 1. Quy ước Commit

Sử dụng chuẩn **Conventional Commits** để thống nhất cách ghi nhận thay đổi trong lịch sử Git.

**Cấu trúc:**

`<type>(<scope>): <description>`

Trong đó:
- `type`: Phân loại nội dung thay đổi.
- `scope`: Thành phần hoặc chức năng chịu ảnh hưởng, không bắt buộc.
- `description`: Mô tả ngắn gọn nội dung thay đổi bằng tiếng Anh.

**Các loại commit:**

| Type | Ý nghĩa | Trường hợp sử dụng |
|---|---|---|
| `feat` | Thêm tính năng | Xây dựng chức năng hoặc hành vi mới |
| `fix` | Sửa lỗi | Khắc phục chức năng hoạt động sai |
| `refactor` | Tái cấu trúc | Cải thiện tổ chức mã nguồn mà không thay đổi hành vi |
| `docs` | Tài liệu | Cập nhật README, UML, tài liệu thiết kế |
| `test` | Kiểm thử | Thêm hoặc chỉnh sửa các bài kiểm thử |
| `chore` | Bảo trì | Thay đổi cấu hình, công cụ và công việc phụ trợ |

**Nguyên tắc:**
- Mỗi commit tập trung vào một thay đổi có ý nghĩa.
- Thông điệp ngắn gọn, rõ ràng, viết bằng tiếng Anh.
- Kiểm tra các file trước khi commit, không đưa file tạm hoặc thông tin nhạy cảm lên Git.
- Commit thường xuyên sau khi hoàn thành từng phần công việc.

**Ví dụ:**
```text
git commit -m "feat(animal): add Chicken class"
git commit -m "fix(crop): correct growth calculation"
git commit -m "docs: update UML class diagram"
```

## 2. Quản lý Branch

Mỗi công việc được thực hiện trên một nhánh riêng, không phát triển trực tiếp trên `main`. Nhánh `main` lưu trữ phiên bản đã được tích hợp và kiểm tra của dự án.

**Quy ước đặt tên:**

`<type>/<short-description>`

| Tiền tố | Mục đích |
|---|---|
| `feature/` | Phát triển tính năng mới, bao gồm Unit Test liên quan |
| `fix/` | Sửa cấu trúc thư mục, sửa lỗi tính năng |
| `test/` | Kiểm thử tích hợp, kiểm thử hệ thống hoặc các đợt kiểm thử tổng thể |
| `docs/` | Cập nhật tài liệu, sơ đồ UML và thiết kế |

**Nguyên tắc:**
- Tạo nhánh từ phiên bản `main` mới nhất, mỗi nhánh tập trung vào một nhiệm vụ hoặc nhóm công việc liên quan.
- Unit Test được thực hiện cùng nhánh phát triển; chỉ tạo nhánh `test/` cho các nhiệm vụ kiểm thử độc lập, quy mô lớn.
- Cập nhật thay đổi từ `main` khi cần và giải quyết xung đột trước khi tích hợp.
- Trao đổi với nhóm trước khi chỉnh sửa các thành phần dùng chung hoặc công việc của thành viên khác.
- Xóa nhánh sau khi hoàn thành và merge thành công nếu không còn sử dụng.

**Ví dụ:**
```text
feature/animal-system
feature/crop-growth
fix/save-game
test/animal-crop-integration
test/gameplay-system
docs/class-diagram
```

## 3. Pull Request và Code Review

Mọi thay đổi được tích hợp vào `main` thông qua **Pull Request (PR)** nhằm đảm bảo mã nguồn được kiểm tra và thống nhất trước khi gộp.

**Quy trình thực hiện:**

**Bước 1 — Hoàn thành công việc**
- Kiểm tra chức năng, đảm bảo mã nguồn hoạt động đúng và không ảnh hưởng đến các thành phần hiện có.
- Commit và push thay đổi lên nhánh công việc.
- Đồng bộ với `main`, xử lý xung đột nếu có.

**Bước 2 — Tạo Pull Request**
- Tạo PR từ nhánh công việc vào `main`, đặt tiêu đề phản ánh đúng nội dung thay đổi.
- Mô tả những thay đổi chính, cách kiểm tra và các vấn đề còn tồn tại.
- Liên kết GitHub Issue tương ứng nếu có.

**Bước 3 — Code Review**
- Reviewer kiểm tra tính đúng đắn, khả năng đọc hiểu, thiết kế OOP và ảnh hưởng đến hệ thống.
- Đưa ra góp ý cụ thể, có tính xây dựng và đề xuất phương án xử lý khi cần.
- Người tạo PR tiếp nhận, chỉnh sửa hoặc trao đổi để thống nhất giải pháp.

**Bước 4 — Merge**
- Chỉ merge khi PR đã được review và chấp thuận, các vấn đề quan trọng đã được giải quyết và dự án hoạt động bình thường.
- Sau khi merge, các thành viên cập nhật `main` trên máy cá nhân trước khi tiếp tục phát triển.

## 4. Quy ước mã nguồn và Unity

### 4.1. Mã nguồn C#

**Quy ước đặt tên:**

Sử dụng tiếng Anh và tuân thủ quy ước đặt tên của C#.

| Thành phần | Quy ước | Ví dụ |
|---|---|---|
| Class | `PascalCase` | `Animal`, `CropManager` |
| Interface | `I` + `PascalCase` | `IHarvestable` |
| Method | `PascalCase` | `HarvestCrop()` |
| Property | `PascalCase` | `GrowthTime` |
| Local variable, parameter | `camelCase` | `growthTime` |
| Private field | `_camelCase` | `_currentHealth` |

**Nguyên tắc thiết kế:**
- Tuân thủ thiết kế OOP đã thống nhất, đảm bảo tính đóng gói, phân chia trách nhiệm rõ ràng và hạn chế phụ thuộc không cần thiết.
- Mỗi class nên đảm nhiệm một trách nhiệm chính; ưu tiên tái sử dụng mã nguồn và tránh trùng lặp logic.
- Tổ chức file, thư mục theo chức năng; thông thường mỗi class chính được lưu trong một file riêng có tên tương ứng.
- Chỉ viết comment để giải thích logic phức tạp hoặc quyết định thiết kế, không mô tả lại những đoạn code hiển nhiên.
- Trao đổi với nhóm trước khi thay đổi class cơ sở, interface hoặc các thành phần dùng chung.

### 4.2. Dự án Unity

**Cấu hình và quản lý tài nguyên:**
- Sử dụng thống nhất phiên bản Unity và cấu hình dự án.
- Sử dụng `.gitignore` dành cho Unity để loại trừ các thư mục sinh tự động như `Library/`, `Temp/`, `Logs/`, `Obj/`.
- Giữ các file `.meta` đi kèm asset và sử dụng cấu hình **Visible Meta Files**, **Force Text**.
- Không tự ý xóa, di chuyển hoặc đổi tên asset dùng chung khi chưa kiểm tra các tham chiếu liên quan.

**Phối hợp phát triển:**
- Hạn chế chỉnh sửa đồng thời cùng một scene hoặc prefab; trao đổi và phân chia phạm vi phụ trách trước khi thực hiện.
- Ưu tiên xây dựng các script, prefab và thành phần độc lập để hạn chế xung đột và phụ thuộc giữa các thành viên.
- Kiểm tra lại các chức năng, scene và prefab liên quan sau khi tích hợp thay đổi.

---

**Quy định chung:** Tất cả thành viên cần tuân thủ các quy ước trên. Những thay đổi ảnh hưởng đến quy trình làm việc hoặc kiến trúc chung phải được thảo luận và thống nhất trong nhóm trước khi áp dụng.