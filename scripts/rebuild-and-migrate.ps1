# Script để rebuild containers và chạy migration tự động
# Sử dụng: .\scripts\rebuild-and-migrate.ps1

Write-Host "Đang dừng containers..." -ForegroundColor Yellow
docker-compose down

Write-Host "Đang rebuild và start containers..." -ForegroundColor Yellow
docker-compose up --build -d

Write-Host "Đợi containers start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "Kiểm tra logs của API để xem migration đã chạy chưa:" -ForegroundColor Cyan
Write-Host "docker logs call-alert-api" -ForegroundColor Yellow

Write-Host "`nĐể xem logs real-time, chạy:" -ForegroundColor Cyan
Write-Host "docker logs -f call-alert-api" -ForegroundColor Yellow

