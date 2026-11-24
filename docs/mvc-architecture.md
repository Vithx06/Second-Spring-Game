# MVC là gì? Cách áp dụng vào Second Spring

MVC là cách chia một chức năng thành ba phần: **Model**, **View** và **Controller**. Mục đích là để mỗi phần có trách nhiệm rõ ràng: phần xử lý dữ liệu và luật, phần hiển thị, và phần kết nối thao tác với việc xử lý.

## Ví dụ ngoài thực tế: gọi món ở nhà hàng

Hãy tưởng tượng bạn vào nhà hàng và gọi một món ăn:

- **View** giống thực đơn và cách nhà hàng trình bày thông tin cho bạn. Bạn nhìn thực đơn để biết có món gì và giá bao nhiêu.
- **Controller** giống nhân viên phục vụ. Nhân viên nhận yêu cầu gọi món, chuyển yêu cầu đến bếp, rồi mang kết quả trở lại cho bạn.
- **Model** giống phần bếp và quy tắc xử lý đơn: kiểm tra nguyên liệu, chế biến món và cho biết món có thể phục vụ hay không.

Quá trình diễn ra như sau:

1. Bạn xem thực đơn và chọn một món.
2. Nhân viên nhận yêu cầu, chuyển cho bếp.
3. Bếp kiểm tra và chế biến; nếu hết nguyên liệu thì báo không thể làm món.
4. Nhân viên nhận kết quả và thông báo hoặc mang món ra cho bạn.

Đây là ví dụ để dễ hình dung, không phải quy định kỹ thuật cứng: trong code, tên class và cách các phần liên lạc có thể khác. Điều quan trọng là **giao diện không cần tự thực hiện toàn bộ luật xử lý**.

## Áp dụng vào game

Với minigame đập chuột trong Second Spring:

- **Model** là `WhackAMoleMinigame`: quản lý ô có chuột, điểm, mạng, thời gian và kết quả thắng/thua.
- **View** hiển thị bảng 9 ô, hình chuột, điểm, mạng và thời gian; đồng thời nhận thao tác click của người chơi.
- **Controller** nhận thông tin ô được click, gọi Model kiểm tra và cập nhật View theo trạng thái mới.

Ví dụ người chơi bấm ô số 4:

1. View phát hiện click ở ô 4 và báo cho Controller.
2. Controller gọi `Hit(4)` của `WhackAMoleMinigame`.
3. Model kiểm tra ô đó có chuột không và cập nhật điểm nếu đập trúng.
4. Controller đọc trạng thái mới; View hiển thị lại điểm và hình chuột.

Tương tự, Controller chuyển thời gian thực đã trôi qua cho Model bằng `Advance(...)`. Model quyết định lúc nào chuột chui xuống hoặc lượt chơi kết thúc; View hiển thị thời gian và kết quả cho người chơi.

Hiện `WhackAMoleMinigame` **đã có code**, còn View và Controller cho game chính **chưa được viết**. Ví dụ trên là luồng dự kiến, chưa phải một scene đã nối xong.

## Thư mục trong dự án

```text
Assets/Scripts/
├── Model/        # Dữ liệu và luật chơi
├── Views/        # Giao diện game chính
├── Controllers/  # Điều phối Model và View
├── Save/         # Đọc và ghi dữ liệu lưu
├── Shared/       # Interface và thành phần dùng chung
└── Exceptions/   # Ngoại lệ nghiệp vụ
```

Trong `Model/` có các thư mục theo chức năng như `Animals`, `Plants`, `Minigames` và `Achievements`. Các lớp như `AnimalService` vẫn thuộc Model vì chúng xử lý nghiệp vụ, dù tên không có chữ “Model”.

`Assets/VinhPrototype/` và `Assets/VuPrototype/` là các bản thử riêng. Chuyển thư mục nghiệp vụ sang `Model/` không tự nối prototype với View và Controller của game chính.

## Viết và gắn vào Unity như thế nào?

1. **Viết Model:** đặt luật trong `Model/<chức năng>/` và kiểm thử bằng dữ liệu giả. Ví dụ, kiểm tra đập trúng thì điểm tăng và hết mạng thì thua.
2. **Tạo View:** dựng bảng, nút, hình và chữ trong Unity hoặc tạo bằng code. Script View đặt trong `Views/<chức năng>/`.
3. **Viết Controller:** đặt trong `Controllers/<chức năng>/`; Controller gọi Model khi View báo thao tác, sau đó cập nhật View.
4. **Gắn component:** nếu script View hoặc Controller kế thừa `MonoBehaviour`, có thể gắn nó lên GameObject. Kéo các nút, hình, chữ vào các trường tương ứng trong Inspector khi script đã khai báo những trường đó.
5. **Chạy thử:** mở scene và kiểm tra người chơi bấm được, Model xử lý đúng và View hiển thị đúng.

Các lớp C# thuần trong `Model/` **không kéo trực tiếp lên GameObject**. Chúng được tạo bằng code hoặc nhận từ một nơi khởi tạo chung. Chỉ đặt file vào `Views/` hay `Controllers/` cũng không tự làm script kéo thả được; class cần được viết phù hợp với Unity.

Khi nhiều chức năng cần cùng ví, kho hoặc đồng hồ, chúng phải dùng cùng đối tượng tương ứng, không tự tạo bản riêng. Khi di chuyển asset hoặc script, giữ file `.meta` để Unity giữ liên kết.

## MVC và các mẫu thiết kế khác

MVC chia trách nhiệm giữa dữ liệu, giao diện và điều phối. Nó không thay thế những mẫu đã dùng trong nghiệp vụ: minigame vẫn có thể dùng **Template Method**, còn đồng hồ và các hệ thống nhận thông báo thời gian vẫn có thể dùng **Observer**.

Kiểm thử C# trong `Tools/` giúp kiểm tra luật Model; vẫn cần chạy Unity để kiểm tra scene, Inspector và thao tác thực tế.
