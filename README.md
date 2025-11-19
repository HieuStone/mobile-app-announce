## 1. Mô tả bài toán (tóm tắt từ `RD.text`)

Ứng dụng cần xây dựng một hệ thống cảnh báo cuộc gọi đến, gồm:

- **Ứng dụng điện thoại di động (Frontend – React Native, Android)**:

  - Quản lý người dùng.
  - Cho phép đăng ký danh sách số điện thoại được gửi cảnh báo (danh sách số **gọi đến** cần theo dõi).
  - Lắng nghe sự kiện **cuộc gọi đến trên chính thiết bị Android**.
  - Khi phát hiện cuộc gọi đến từ một số nằm trong danh sách theo dõi, gửi sự kiện lên backend.

- **Backend (C#)**:

  - Nhận các sự kiện cuộc gọi đến từ ứng dụng Android (qua REST API).
  - Xác thực, ghi log và xử lý business rule.
  - Gửi một message lên **MQTT Mosquitto** cho mỗi cuộc gọi đến hợp lệ.

- **Mạch vi xử lý Arduino**:
  - Lắng nghe (subscribe) sự kiện từ **MQTT Mosquitto**.
  - Khi nhận được thông báo cuộc gọi đến, xuất tín hiệu output điều khiển **đèn nhấp nháy** để cảnh báo.

> Ghi chú: một số chi tiết (mức độ chi tiết thông tin cuộc gọi, pattern nháy đèn, phân quyền người dùng...) sẽ được làm rõ dần trong các tài liệu tiếp theo.

---

## 2. Kiến trúc tổng thể đề xuất

- **Mobile App (React Native)**:

  - Nền tảng: **chỉ Android** (có thể tính đến iOS trong tương lai).
  - Giao tiếp với backend thông qua REST API (HTTPS).
  - Chạy nền (hoặc service phù hợp) để lắng nghe sự kiện cuộc gọi đến trên thiết bị.
  - Các chức năng chính:
    - Đăng ký / đăng nhập người dùng.
    - Quản lý danh sách số điện thoại nhận cảnh báo (danh sách số gọi đến cần theo dõi).
    - Gửi sự kiện cuộc gọi đến (số gọi, thời điểm, trạng thái...) lên backend khi có cuộc gọi đến.

- **Backend (C#, ASP.NET Core gợi ý)**:

  - Cung cấp REST API cho mobile app (đăng nhập, quản lý số theo dõi, nhận sự kiện cuộc gọi).
  - Lưu trữ dữ liệu người dùng và cấu hình bằng **SQL Server** (container Developer edition).
  - Nhận thông tin cuộc gọi đến từ ứng dụng Android (không phụ thuộc vào tổng đài riêng biệt).
  - Khi có cuộc gọi đến từ số nằm trong danh sách theo dõi:
    - Ghi log / lưu lịch sử.
    - Publish một message lên MQTT Broker (Mosquitto) với payload chứa thông tin cần thiết (số gọi đến, thời gian, loại cảnh báo...).

- **MQTT Broker (Mosquitto)**:

  - Đóng vai trò trung gian truyền message theo mô hình publish/subscribe.
  - Backend sẽ **publish** lên một topic, ví dụ: `calls/incoming`.
  - Arduino sẽ **subscribe** topic tương ứng để nhận sự kiện.

- **Arduino + Đèn cảnh báo (ESP32 WiFi)**:
  - Sử dụng board **ESP32** kết nối mạng qua **WiFi**.
  - Chạy client MQTT, subscribe topic `calls/incoming`.
  - Khi nhận message, bật đèn nhấp nháy với pattern xác định (nháy liên tục trong X giây, hoặc đến khi có tín hiệu tắt...).

---

## 3. Công nghệ chính sẽ sử dụng

- **Mobile**:

  - React Native (TypeScript khuyến khích).
  - Thư viện quản lý state (Redux Toolkit hoặc Zustand, sẽ chọn ở bước thiết kế chi tiết).
  - Thư viện điều hướng: React Navigation.
  - Tích hợp native module/permission để đọc trạng thái cuộc gọi đến trên Android.

- **Backend**:

  - C# với ASP.NET Core Web API.
  - Entity Framework Core (nếu dùng DB quan hệ).
  - Thư viện MQTT client cho .NET (ví dụ: `MQTTnet`) để publish tới Mosquitto.

- **MQTT Broker**:

  - Mosquitto (deploy trên server/VM hoặc container Docker).

- **Arduino / ESP32**:

  - Board **ESP32** hỗ trợ kết nối WiFi.
  - Thư viện MQTT cho ESP32 (ví dụ: `PubSubClient` hoặc thư viện MQTT dành riêng cho ESP32).

- **Triển khai & kiến trúc microservice**:

  - Mỗi thành phần chính (backend API, MQTT broker, các service phụ trợ nếu có) được đóng gói thành **container Docker** riêng.
  - Sử dụng Docker Compose (hoặc hệ thống orchestration khác trong tương lai) để quản lý, cấu hình và triển khai.
  - Hỗ trợ CI/CD: tự động build image, push lên registry và deploy.

---

## 4. Kế hoạch từng bước & các file tài liệu (read file)

Để dễ phân tích và kiểm soát, chúng ta sẽ chia nhỏ thành các bước, mỗi bước kèm một tài liệu riêng:

1. **Tài liệu kiến trúc tổng quan** (file hiện tại: `README.md`).
2. **Tài liệu chi tiết yêu cầu & luồng chức năng mobile**:
   - Dự kiến: `docs/mobile-spec.md`
   - Mô tả màn hình, use-case, API cần gọi từ mobile.
3. **Tài liệu chi tiết backend C# & MQTT**:
   - Dự kiến: `docs/backend-spec.md`
   - Mô tả các API, mô hình dữ liệu, luồng xử lý cuộc gọi, luồng publish MQTT.
4. **Tài liệu luồng Arduino & đèn cảnh báo**:
   - Dự kiến: `docs/arduino-spec.md`
   - Mô tả cách Arduino subscribe MQTT, format message, logic điều khiển đèn.
5. **Khi tài liệu được bạn duyệt**:
   - Bắt đầu khởi tạo code cho từng phần (project React Native, project ASP.NET Core, code mẫu Arduino).

---

## 5. Bước tiếp theo

Với các giả định/trao đổi hiện tại:

- Nền tảng mục tiêu: **Android**.
- Sự kiện cuộc gọi đến: lấy trực tiếp từ điện thoại cài app, app gửi sự kiện lên backend.
- Hệ thống backend + MQTT sẽ được triển khai theo mô hình **microservice bằng Docker**, hỗ trợ CI/CD.

**Bước tiếp theo**: xây dựng tài liệu chi tiết cho ứng dụng mobile Android (`docs/mobile-spec.md`) mô tả:

- Các màn hình chính.
- Luồng đăng ký số điện thoại theo dõi.
- Luồng lắng nghe cuộc gọi và gửi sự kiện lên backend.
- Danh sách API backend mà mobile cần sử dụng.
