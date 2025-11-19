# Hướng dẫn chạy Migration từ Visual Studio

## Bước 1: Đảm bảo SQL Server container đang chạy

Trước tiên, đảm bảo SQL Server container đã được start:

```powershell
docker-compose up -d db
```

Hoặc start tất cả containers:

```powershell
docker-compose up -d
```

Kiểm tra container đang chạy:

```powershell
docker ps
```

## Bước 2: Cài đặt EF Core Tools (nếu chưa có)

Mở **Package Manager Console** trong Visual Studio (Tools → NuGet Package Manager → Package Manager Console) và chạy:

```powershell
dotnet tool install --global dotnet-ef
```

Hoặc nếu đã cài rồi, cập nhật:

```powershell
dotnet tool update --global dotnet-ef
```

## Bước 3: Cấu hình Connection String

File `appsettings.json` đã được cấu hình để kết nối với SQL Server container:
- **Server**: `localhost,1433`
- **Port**: (nằm trong server string)
- **Database**: `call_alert_db`
- **User**: `sa`
- **Password**: `YourStrong!Passw0rd`

## Bước 4: Chạy Migration

### Cách 1: Dùng Package Manager Console (Visual Studio)

1. Mở **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)
2. Đảm bảo **Default project** là `CallAlert.Api`
3. Chạy lệnh:

```powershell
Update-Database
```

Hoặc nếu muốn chỉ định project:

```powershell
Update-Database -Project CallAlert.Api -StartupProject CallAlert.Api
```

### Cách 2: Dùng dotnet CLI trong Terminal của Visual Studio

1. Mở **Terminal** trong Visual Studio (View → Terminal)
2. Chuyển vào thư mục project:

```powershell
cd backend\CallAlert.Api
```

3. Chạy migration:

```powershell
dotnet ef database update
```

Hoặc chỉ định rõ project:

```powershell
dotnet ef database update --project . --startup-project .
```

### Cách 3: Tạo Migration mới (nếu cần)

Nếu bạn thay đổi entities và cần tạo migration mới:

**Package Manager Console:**
```powershell
Add-Migration TenMigrationMoi -Project CallAlert.Api -StartupProject CallAlert.Api
```

**dotnet CLI:**
```powershell
dotnet ef migrations add TenMigrationMoi --project . --startup-project .
```

## Bước 5: Kiểm tra kết quả

Sau khi migration chạy thành công, kiểm tra trong SQL Server:

```powershell
docker exec -it call-alert-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -d call_alert_db
```

Chạy SQL:

```sql
SELECT name FROM sys.tables;
GO
SELECT * FROM __EFMigrationsHistory;
GO
```

## Troubleshooting

- Đảm bảo SQL Server container đang chạy: `docker ps`
- Kiểm tra port 1433 có bị chiếm không: `netstat -an | findstr 1433`
- Đảm bảo connection string trong `appsettings.json` đúng

### Lỗi: "dotnet ef not found"

- Cài EF Core Tools: `dotnet tool install --global dotnet-ef`
- Đóng và mở lại Visual Studio
- Hoặc dùng Package Manager Console thay vì Terminal

### Lỗi: "The specified project was not found"

- Đảm bảo bạn đang ở đúng thư mục project
- Hoặc chỉ định rõ project: `--project CallAlert.Api`

### Lỗi: "Migration already applied"

- Migration đã chạy rồi, không cần chạy lại
- Nếu muốn reset, xóa database và chạy lại:
  ```powershell
  docker exec -it call-alert-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "DROP DATABASE IF EXISTS call_alert_db; CREATE DATABASE call_alert_db;"
  ```

## Lưu ý

- Khi chạy từ Visual Studio, connection string trong `appsettings.json` sẽ được dùng
- Port mặc định là **1433**; nếu đổi port trong compose, cập nhật lại connection string
- Nếu thay đổi mật khẩu/chuỗi kết nối trong `docker-compose.yml`, nhớ cập nhật `appsettings.json`

