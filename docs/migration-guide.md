# Hướng dẫn chạy Migration cho Database

## Cách 1: Tự động (Khuyến nghị)

API đã được cấu hình để tự động chạy migration khi start. Chỉ cần:

```powershell
# Rebuild và restart containers
docker-compose down
docker-compose up --build -d

# Kiểm tra logs để xem migration đã chạy
docker logs call-alert-api
```

Bạn sẽ thấy log: `"Database migrations applied successfully."` nếu thành công.

## Cách 2: Chạy migration thủ công từ container

Nếu migration không tự động chạy, bạn có thể chạy thủ công:

```powershell
# Vào container và chạy migration
docker exec -it call-alert-api dotnet ef database update --project /app/CallAlert.Api.csproj --no-build
```

## Cách 3: Chạy từ máy host (nếu đã cài EF Core Tools)

Nếu bạn đã cài `dotnet ef` tools trên máy:

```powershell
# Cài EF Core Tools (chỉ cần làm 1 lần)
dotnet tool install --global dotnet-ef

# Chạy migration
cd backend/CallAlert.Api
dotnet ef database update --project . --startup-project .
```

**Lưu ý**: Cần cấu hình connection string trong `appsettings.json` hoặc environment variable để kết nối được với SQL Server container.

## Cách 4: Dùng script PowerShell

Mình đã tạo sẵn script:

```powershell
# Rebuild và chạy migration tự động
.\scripts\rebuild-and-migrate.ps1

# Hoặc chỉ chạy migration (nếu container đã chạy)
.\scripts\run-migration.ps1
```

## Kiểm tra migration đã chạy

Sau khi migration chạy, kết nối vào SQL Server:

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

Bạn sẽ thấy các bảng:
- `Users`
- `WatchNumbers`
- `CallEvents`
- `__EFMigrationsHistory`

## Troubleshooting

### Lỗi: "Cannot connect to database"

- Đảm bảo SQL Server container đã start: `docker ps`
- Kiểm tra connection string trong `docker-compose.yml`
- Đợi SQL Server khởi động hoàn toàn (10-20 giây)

### Lỗi: "Migration already applied"

- Migration đã chạy rồi, không cần chạy lại
- Nếu muốn reset, xóa volume và chạy lại:
  ```powershell
  docker-compose down -v
  docker-compose up --build -d
  ```

### Lỗi: "dotnet ef not found"

- Cài EF Core Tools: `dotnet tool install --global dotnet-ef`
- Hoặc dùng cách 1 (tự động) hoặc cách 2 (từ container)

