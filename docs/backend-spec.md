## 1. Mục tiêu backend

Backend C# (ASP.NET Core Web API) có nhiệm vụ:

- Cung cấp API cho ứng dụng Android: auth, quản lý danh sách số theo dõi, nhận sự kiện cuộc gọi, trả về lịch sử.
- Xử lý logic kiểm tra số gọi đến có thuộc danh sách theo dõi hay không (nếu cần double-check phía server).
- Ghi log/lịch sử sự kiện cuộc gọi vào cơ sở dữ liệu.
- Publish message tới **MQTT Mosquitto** khi nhận được sự kiện cuộc gọi hợp lệ.
- Chạy dạng **microservice trong container Docker**, dễ triển khai và tích hợp CI/CD.

---

## 2. Kiến trúc logic backend

- **Thành phần API** (ASP.NET Core Web API):

  - Các controller:
    - `AuthController`: đăng ký, đăng nhập, refresh token (nếu dùng).
    - `WatchNumbersController`: CRUD danh sách số theo dõi.
    - `CallEventsController`: nhận sự kiện cuộc gọi từ app, trả lịch sử.
  - Lớp service:
    - `IUserService`, `IWatchNumberService`, `ICallEventService`, `IMqttPublisherService`.
  - Lớp repository (nếu dùng pattern này) cho thao tác DB.

- **MQTT Publisher**:

  - Một service dùng thư viện `MQTTnet` (hoặc tương đương), cấu hình broker Mosquitto (host, port, user/pass nếu có).
  - Khi `CallEventsController` ghi nhận sự kiện hợp lệ, gọi `IMqttPublisherService.PublishAsync(...)` để publish message lên topic, ví dụ: `calls/incoming`.

- **Database**:

  - Sử dụng **SQL Server** (phiên bản Developer/container).
  - Dùng Entity Framework Core với provider SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`).

- **Authentication & Authorization**:
  - JWT Bearer Token cho các API dành cho app mobile.
  - Một số endpoint public (ví dụ `health-check`), còn lại yêu cầu token.

---

## 3. Mô hình dữ liệu (Entities)

### 3.1. User

- Thuộc tính gợi ý:
  - `Id` (GUID hoặc int identity)
  - `Username` hoặc `PhoneNumber` (unique)
  - `Email` (nullable)
  - `PasswordHash`
  - `CreatedAt`
  - `UpdatedAt`

### 3.2. WatchNumber (Số theo dõi)

- Thuộc tính gợi ý:
  - `Id`
  - `UserId` (FK → User)
  - `PhoneNumber` (số sẽ được theo dõi khi gọi đến)
  - `Label` (ghi chú: tên khách hàng, đối tác...)
  - `IsActive` (bool – có đang được theo dõi hay không)
  - `CreatedAt`
  - `UpdatedAt`

### 3.3. CallEvent (Sự kiện cuộc gọi)

- Thuộc tính gợi ý:
  - `Id`
  - `UserId` (FK → User – chủ thiết bị/app gửi sự kiện)
  - `CallerNumber` (số gọi đến)
  - `CalledAt` (timestamp)
  - `CallStatus` (ringing / answered / missed – chuỗi hoặc enum)
  - `DeviceId` (nếu app gửi lên)
  - `IsWatchedNumber` (bool – có thuộc danh sách theo dõi hay không)
  - `MqttPublished` (bool – đã publish lên MQTT hay chưa)
  - `MqttPublishedAt` (nullable datetime)
  - `CreatedAt`

---

## 4. Thiết kế API chi tiết (dự kiến)

Tất cả các endpoint (trừ đăng ký/đăng nhập, health-check) sẽ yêu cầu header:

- `Authorization: Bearer <JWT_TOKEN>`

### 4.1. AuthController

- `POST /api/auth/register`

  - **Request body (ví dụ)**:
    - `username` hoặc `phoneNumber`
    - `password`
    - `email` (optional)
  - **Response**:
    - Thông tin user cơ bản (ẩn password).
    - Có thể trả kèm token hoặc yêu cầu người dùng login lại (tuỳ chiến lược).

- `POST /api/auth/login`
  - **Request body**:
    - `username` hoặc `phoneNumber`
    - `password`
  - **Response**:
    - `accessToken` (JWT)
    - Thông tin user cơ bản.

### 4.2. WatchNumbersController

- `GET /api/watch-numbers`

  - **Mô tả**: Lấy danh sách số theo dõi của user hiện tại.
  - **Response**: mảng các object:
    - `id`, `phoneNumber`, `label`, `isActive`, `createdAt`, `updatedAt`.

- `POST /api/watch-numbers`

  - **Request body** (ví dụ):
    - `phoneNumber`
    - `label` (optional)
  - **Response**:
    - Object vừa tạo.

- `PUT /api/watch-numbers/{id}`

  - **Mô tả**: Cập nhật thông tin một số theo dõi (ví dụ đổi label, bật/tắt `isActive`).
  - **Request body** (ví dụ):
    - `label`
    - `isActive`
  - **Response**:
    - Object sau khi cập nhật.

- `DELETE /api/watch-numbers/{id}`
  - **Mô tả**: Xoá một số khỏi danh sách theo dõi.
  - **Response**: 204 No Content (hoặc trả object xoá nếu muốn).

### 4.3. CallEventsController

- `POST /api/call-events`

  - **Mô tả**: App gửi mỗi khi có cuộc gọi đến từ số _có trong danh sách theo dõi_ (theo logic phía mobile). Backend có thể kiểm tra lại.
  - **Request body** (gợi ý):
    - `callerNumber`: string
    - `calledAt`: datetime (ISO 8601)
    - `callStatus`: string (`ringing`, `answered`, `missed`, v.v.)
    - `deviceId`: string (optional)
  - **Xử lý phía backend**:
    1. Xác thực token → lấy `UserId`.
    2. Kiểm tra `callerNumber` có thuộc danh sách `WatchNumber` đang `IsActive` của user hay không.
    3. Tạo bản ghi `CallEvent` (gán `IsWatchedNumber = true/false`).
    4. Nếu `IsWatchedNumber == true`:
       - Gọi `IMqttPublisherService.PublishAsync(...)` publish message lên MQTT.
       - Cập nhật `MqttPublished = true`, `MqttPublishedAt = now`.
  - **Response** (ví dụ):
    - Thông tin `CallEvent` vừa lưu (bao gồm flag `isWatchedNumber`, `mqttPublished`).

- `GET /api/call-events`
  - **Mô tả**: Lấy danh sách sự kiện cuộc gọi của user.
  - **Query params (tuỳ chọn)**:
    - `from`, `to`: lọc theo thời gian.
    - `isWatched`: chỉ lấy cuộc gọi từ số theo dõi.
  - **Response**: mảng `CallEvent` (hoặc dạng đã map sang DTO).

### 4.4. HealthCheck

- `GET /api/health`
  - Kiểm tra nhanh tình trạng service (DB ok, MQTT kết nối được, v.v. – tuỳ mức độ).

---

## 5. Thiết kế MQTT message

- **Broker**: Mosquitto.
- **Topic gợi ý**: `calls/incoming`
- **Payload JSON (ví dụ)**:

```json
{
  "userId": "xxx",
  "callerNumber": "+8490xxxxxxx",
  "calledAt": "2025-01-01T12:34:56Z",
  "callStatus": "ringing"
}
```

- Có thể bổ sung thêm:
  - `deviceId`
  - `label` của số theo dõi (nếu có)

MQTT client trên Arduino sẽ subscribe topic này và dựa vào payload để bật đèn cảnh báo.

---

## 6. Cấu trúc project backend (gợi ý)

- `src/`
  - `Controllers/`
    - `AuthController.cs`
    - `WatchNumbersController.cs`
    - `CallEventsController.cs`
    - `HealthController.cs`
  - `Services/`
    - `UserService.cs`
    - `WatchNumberService.cs`
    - `CallEventService.cs`
    - `MqttPublisherService.cs`
  - `Repositories/` (nếu dùng)
  - `Entities/`
    - `User.cs`, `WatchNumber.cs`, `CallEvent.cs`, ...
  - `Dtos/`
    - Request/Response models cho API.
  - `Infrastructure/`
    - Cấu hình EF Core, DbContext.
    - Cấu hình MQTT client (host, port, credential).
  - `Program.cs`, `appsettings.json`, ...

---

## 7. Docker & microservice

### 7.1. Dockerfile cho backend

- Backend sẽ có file `Dockerfile` dạng multi-stage:
  - Stage 1: build (dùng image SDK .NET).
  - Stage 2: runtime (dùng image ASP.NET Core runtime, copy output từ stage build).

### 7.2. Docker Compose

- File `docker-compose.yml` (sẽ viết sau) có thể gồm các service:

  - `api`: container backend C#.
  - `mqtt`: container Mosquitto.
  - `db`: container database **SQL Server** (image `mcr.microsoft.com/mssql/server`).

- Các service liên kết với nhau qua network nội bộ của Docker:
  - Backend kết nối DB SQL Server qua hostname `db`, port 1433, với user/password được cấu hình qua biến môi trường (`sa` / `YourStrong!Passw0rd`).
  - Backend publish MQTT tới hostname `mqtt`.

---

## 8. Bước tiếp theo cho backend

- Chốt cấu trúc entity và DTO (User, WatchNumber, CallEvent).
- Chốt chuẩn payload chính thức cho `/api/call-events` và message MQTT.
- Viết skeleton project ASP.NET Core (controller rỗng, entity, DbContext).
- Thêm thư viện `MQTTnet` và tạo `MqttPublisherService` để publish thử 1 message demo.
