XÂY DỰNG WEBSITE GIÁM SÁT VÀ ĐIỀU PHỐI HỆ THỐNG ĐỘI XE
Website cung cấp giải pháp quản lý tập trung cho các doanh nghiệp vận tải, giúp giám sát lộ trình, điều phối xe/tài xế và tối ưu hóa quy trình vận hành đội xe theo thời gian thực.

✨ Tính năng chính
Hệ thống được thiết kế với 3 nhóm chức năng chính:

1. Quản lý & Giám sát (Admin)
   
- Quản lý tài khoản hệ thống.

- Giám sát thời gian thực: Theo dõi vị trí đội xe.

- Báo cáo & Thống kê: Xuất báo cáo doanh thu, hiệu suất vận hành và nhật ký bảo trì phương tiện.

2. Điều phối vận hành (Điều phối viên)

- Quản lý nguồn lực: Quản lý chi tiết hồ sơ tài xế, thông số kỹ thuật phương tiện.

- Quản lý đơn hàng: Tiếp nhận, kiểm tra tải trọng và xử lý các yêu cầu vận chuyển.

- Điều phối thông minh: Phân công tài xế và phương tiện phù hợp cho từng chuyến đi dựa trên trạng thái sẵn sàng.

- Xử lý sự cố: Tiếp nhận báo cáo khẩn cấp từ tài xế và điều phối đội cứu hộ/sửa chữa.

- Báo cáo & Thống kê: Xuất báo cáo doanh thu, hiệu suất vận hành và nhật ký bảo trì phương tiện.

- Tính toán chi phí tiết kiệm, tạo tuyến đường gợi ý cho chuyến đi.

3. Nghiệp vụ thực địa (Tài xế)
- Quản lý chuyến đi: Nhận lịch trình, cập nhật tiến độ giao hàng và xác nhận biên nhận (POD).

- Quản lý chi phí: Ghi lại nhật ký đổ xăng, phí cầu đường và các chi phí phát sinh dọc đường.

- Báo cáo sự cố: Gửi thông tin cứu hộ khẩn cấp kèm hình ảnh và tọa độ thực tế.

🛠 Công nghệ sử dụng

- Backend: .NET 8 (ASP.NET Core Web API).

- Frontend: React.js / Vite / Tailwind CSS.

- Cơ sở dữ liệu: Microsoft SQL Server.

- Bản đồ & Định vị: Google Maps API / Leaflet.


⚙️ Hướng dẫn cài đặt

1. Cài đặt và chạy Backend

Yêu cầu: .NET SDK và SQL Server Management Studio (SSMS).

- Clone repository

- Thiết lập cơ sở dữ liệu: Mở SQL Management Studio và thực thi file script.sql để tạo database.

- Cấu hình Secrets: chạy lệnh "dotnet user-secrets init" và set secrets theo appsettings.example.json

- Chạy ứng dụng: chạy "dotnet run"

2. Cài đặt và chạy Frontend
- Chạy lệnh "npm install" để cài đặt thư viện

- Chạy lệnh "npm run dev"

