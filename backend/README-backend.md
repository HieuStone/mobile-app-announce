## Backend C# – Skeleton

File này mô tả nhanh phần backend để sau này tạo project ASP.NET Core Web API đúng theo `docs/backend-spec.md`.

### 1. Cấu trúc dự kiến

- Thư mục `backend/`:
  - `CallAlert.Api/` (sẽ tạo sau bằng `dotnet new webapi`):
    - `Program.cs`, `appsettings.json`, ...
    - `Controllers/` (`AuthController`, `WatchNumbersController`, `CallEventsController`, `HealthController`).
    - `Entities/`, `Dtos/`, `Services/`, `Infrastructure/` (EF Core + SQL Server, MQTT client).
  - `Dockerfile`: đã tạo, dùng để build image cho service `api` trong `docker-compose.yml`.

### 2. Kết nối với docker-compose

- Service `api` trong `docker-compose.yml`:
  - Build từ thư mục `backend/` với `Dockerfile` hiện tại.
  - Dùng biến môi trường:
    - `ConnectionStrings__DefaultConnection` trỏ tới SQL Server (`db`).
    - `Mqtt__Host` trỏ tới MQTT broker (`mqtt`).

### 3. Bước tiếp theo

- Tạo project ASP.NET Core Web API:
  - Chạy lệnh (sau này, bên ngoài): `dotnet new webapi -n CallAlert.Api` trong thư mục `backend/`.
- Cập nhật `Dockerfile`:
  - Bỏ comment các lệnh `COPY`, `dotnet restore`, `dotnet publish`, `COPY --from=build ...`.
- Cài gói:
  - `Microsoft.EntityFrameworkCore.SqlServer` cho EF + SQL Server.
  - `MQTTnet` cho MQTT client.


