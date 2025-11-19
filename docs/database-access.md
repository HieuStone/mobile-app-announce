# Hướng dẫn truy cập SQL Server Database

## Thông tin kết nối

- **Host**: `localhost` (từ máy host) hoặc `db` (từ container)
- **Port**: `1433`
- **Database**: `call_alert_db`
- **User**: `sa`
- **Password**: `YourStrong!Passw0rd`

## Cách 1: Kết nối từ container SQL Server (sqlcmd)

```bash
docker exec -it call-alert-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd'
```

Sau khi vào `sqlcmd`, bạn có thể chạy:

```sql
-- Chọn database
USE call_alert_db;
GO

-- Xem danh sách bảng
SELECT name FROM sys.tables;
GO

-- Xem dữ liệu bảng Users
SELECT * FROM Users;
GO
```

## Cách 2: Kết nối từ máy host bằng sqlcmd

```bash
sqlcmd -S localhost,1433 -U sa -P YourStrong!Passw0rd -d call_alert_db
```

## Cách 3: Dùng Azure Data Studio / SQL Server Management Studio / DBeaver

1. Tạo connection:
   - **Server**: `localhost,1433`
   - **Authentication**: SQL Login
   - **Login**: `sa`
   - **Password**: `YourStrong!Passw0rd`
   - **Database**: `call_alert_db`
2. Kết nối và xem schemas/tables.

## Cách 4: Dùng VS Code Extension (SQL Server)

Cài extension **SQL Server (mssql)**, tạo connection với thông tin trên.

## Kiểm tra migrations đã chạy

```sql
SELECT * FROM __EFMigrationsHistory;
```

## Lưu ý

- Database được tạo khi SQL Server container start lần đầu.
- Migrations tự động chạy khi API start (xem `Program.cs`).
- Reset database:

```bash
docker-compose down -v
docker-compose up -d
```

