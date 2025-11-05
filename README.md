# FGInventoryManagement

FGInventoryManagement là dịch vụ Web API phục vụ cho việc quản lý tồn kho thành phẩm. Dự án chạy trên .NET 8 và tận dụng lại các lớp cấu hình sẵn có như `Startup`, các middleware tùy biến và hệ thống logging tập trung bằng NLog.

## Luồng khởi động từ `Program` đến `Startup`
1. **Khởi tạo host** – `Program.cs` tạo `WebApplicationBuilder`, loại bỏ các provider logging mặc định và đăng ký NLog làm bộ ghi log chính.
2. **Cấu hình dịch vụ** – `Program` khởi tạo lớp `Startup` với `Configuration` và `Environment` hiện tại rồi gọi `Startup.ConfigureServices` để đăng ký DbContext, middleware, SignalR, cấu hình Swagger và các service nội bộ.
3. **Xây dựng ứng dụng** – Sau khi dịch vụ được đăng ký, ứng dụng được build ra `WebApplication`.
4. **Thiết lập pipeline** – `Program` lấy `ILoggerFactory` và `ILoggerManager` từ DI container và chuyển cho `Startup.Configure` để cấu hình middleware pipeline (Static files, CORS, Authentication, Swagger UI, middleware tuỳ chỉnh...).
5. **Chạy ứng dụng** – Cuối cùng `app.Run()` khởi động web host.

Nhờ cách tổ chức này, dự án vẫn giữ được mô hình `Startup` quen thuộc nhưng hoàn toàn tương thích với kiểu hosting tối thiểu của .NET 8.
