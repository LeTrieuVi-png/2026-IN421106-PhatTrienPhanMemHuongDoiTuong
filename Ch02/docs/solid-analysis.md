# Phân tích vi phạm SOLID trong ClubManager

## 1. Tổng quan

`ClubManager` (file `Ch02.Refactor/ClubManager.cs`, bản starter) là God class: một class vừa quản lý CLB, thành viên, sự kiện, vừa ghi log, gửi thông báo và in báo cáo. Vì vậy class này khó đọc, khó kiểm thử và khó mở rộng.

## 2. Các vi phạm

### Vi phạm 1: CreateClub làm 4 việc (SRP)
- **Vị trí:** `CreateClub`, dòng 35–64
- **Nguyên tắc vi phạm:** Single Responsibility
- **Mô tả:** Một method vừa kiểm tra tên (dòng 38–41), vừa lưu CLB (dòng 44–45), vừa ghi log (dòng 48), vừa gửi thông báo (dòng 52–63).
- **Hậu quả:** Có 4 lý do để sửa cùng một method; khó đọc, khó test từng việc riêng.
- **Hướng sửa:** Chuyển luật kiểm tra tên vào `Club.Create`; `ClubService` chỉ điều phối; việc log và thông báo giao cho `IAppLogger` và `INotificationService`.

### Vi phạm 2: Chuỗi if/else khi gửi thông báo (OCP)
- **Vị trí:** `CreateClub`, dòng 52–63
- **Nguyên tắc vi phạm:** Open/Closed
- **Mô tả:** Cách gửi thông báo được chọn bằng if/else theo đuôi email (@gmail.com, @yahoo.com).
- **Hậu quả:** Muốn thêm SMS hoặc Slack phải mở method này ra sửa, dễ làm hỏng phần đang chạy tốt.
- **Hướng sửa:** Tạo `INotificationService` với các class Email/Sms/Slack, dùng `NotificationServiceFactory` chọn kênh theo `NotificationChannel`.

### Vi phạm 3: Ghi file trực tiếp (DIP)
- **Vị trí:** `CreateClub` dòng 48, `AddMemberToClub` dòng 79, `CreateEvent` dòng 94, `PrintClubReport` dòng 111
- **Nguyên tắc vi phạm:** Dependency Inversion
- **Mô tả:** Logic nghiệp vụ gọi thẳng `File.AppendAllText`, phụ thuộc vào class cụ thể.
- **Hậu quả:** Không thay được cách ghi log; khi chạy unit test sẽ tạo file thật trên ổ đĩa.
- **Hướng sửa:** Tạo `IAppLogger` (ConsoleLogger, FileLogger), nhận qua constructor.

### Vi phạm 4: CreateEvent tự gửi thông báo (SRP + OCP)
- **Vị trí:** `CreateEvent`, dòng 96–97
- **Nguyên tắc vi phạm:** Single Responsibility và Open/Closed
- **Mô tả:** Method tạo sự kiện tự duyệt từng thành viên và gửi email bằng `Console.WriteLine`.
- **Hậu quả:** Thêm một người nhận thông báo mới (ví dụ ghi log, gửi Slack) phải sửa `CreateEvent`.
- **Hướng sửa:** Dùng Observer: `IEventObserver` với `EmailNotifier`, `LogNotifier`; `EventService` chỉ gọi `OnEventCreated`.

### Vi phạm 5: PrintClubReport trộn nhiều việc (OCP + SRP)
- **Vị trí:** `PrintClubReport`, dòng 103–113
- **Nguyên tắc vi phạm:** Open/Closed và Single Responsibility
- **Mô tả:** Method vừa thống kê, vừa in ra console, vừa ghi file log (dòng 111).
- **Hậu quả:** Muốn xuất CSV phải sửa method này.
- **Hướng sửa:** Dùng Strategy: `IReportStrategy` với `ConsoleReportStrategy`, `CsvReportStrategy`; `ReportService.Generate(strategy)`.

### Vi phạm 6: Luật 50 thành viên nằm sai chỗ (SRP)
- **Vị trí:** `AddMemberToClub`, dòng 72
- **Nguyên tắc vi phạm:** Single Responsibility
- **Mô tả:** Luật "CLB tối đa 50 thành viên" viết cứng số 50 trong manager, trong khi `Club.MaxMembers` đã tồn tại.
- **Hậu quả:** Cùng một luật nằm ở hai chỗ, sửa một nơi sẽ quên nơi kia.
- **Hướng sửa:** Chuyển luật vào `Club.HasRoom`.

### Vi phạm 7: Dữ liệu nằm trong class và bị lộ ra ngoài (DIP)
- **Vị trí:** dòng 30–31 (khai báo `List<>`) và dòng 116 (`GetAllClubs`)
- **Nguyên tắc vi phạm:** Dependency Inversion
- **Mô tả:** Dữ liệu lưu bằng `List<>` ngay trong class; `GetAllClubs()` trả ra chính list nội bộ.
- **Hậu quả:** Bên ngoài có thể tự thêm hoặc xoá dữ liệu mà `ClubManager` không biết; không thay được nơi lưu trữ.
- **Hướng sửa:** Tạo `IClubRepository`, `IMemberRepository`, `IEventRepository` (bản InMemory) và bọc `LoggingClubRepository` (Decorator).

## 3. Lỗi phát hiện thêm

- `CreateEvent` (dòng 88–99) tạo đối tượng `Event` nhưng không lưu vào đâu, nên sự kiện bị mất sau khi method kết thúc.

## 4. Bảng tóm tắt

| # | Dòng | Nguyên tắc | Cách sửa |
|---|------|-----------|----------|
| 1 | 35–64 | SRP | Tách `ClubService`, `IAppLogger`, `INotificationService` |
| 2 | 52–63 | OCP | `INotificationService` + Factory |
| 3 | 48, 79, 94, 111 | DIP | `IAppLogger` |
| 4 | 96–97 | SRP + OCP | Observer |
| 5 | 103–113 | OCP + SRP | Strategy |
| 6 | 72 | SRP | `Club.HasRoom` |
| 7 | 30–31, 116 | DIP | Repository + Decorator |