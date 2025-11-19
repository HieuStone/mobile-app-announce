# Script để chạy migration từ container API
# Sử dụng: .\scripts\run-migration.ps1

Write-Host "Đang chạy migration từ container API..." -ForegroundColor Yellow

docker exec -it call-alert-api dotnet ef database update --project /app/CallAlert.Api.csproj --no-build

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration đã chạy thành công!" -ForegroundColor Green
} else {
    Write-Host "Có lỗi xảy ra khi chạy migration. Kiểm tra logs:" -ForegroundColor Red
    Write-Host "docker logs call-alert-api" -ForegroundColor Yellow
}

