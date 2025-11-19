## 1. Mục tiêu ứng dụng mobile (Android – React Native)

Ứng dụng Android có nhiệm vụ:

- Cho phép người dùng đăng ký / đăng nhập.
- Quản lý danh sách số điện thoại **gọi đến** cần theo dõi (khi có cuộc gọi từ những số này thì kích hoạt cảnh báo).
- Lắng nghe sự kiện **cuộc gọi đến trên thiết bị**.
- Khi có cuộc gọi đến từ số nằm trong danh sách theo dõi:
  - Gửi sự kiện lên backend C# qua REST API.
  - (Tùy nhu cầu) hiển thị thông báo trên UI.

---

## 2. Đối tượng người dùng & use-case chính

- **User** (chủ thiết bị Android):
  - Đăng ký tài khoản và đăng nhập.
  - Cấu hình danh sách số điện thoại gọi đến cần được cảnh báo.
  - Bật/tắt chế độ theo dõi cuộc gọi.
  - Xem danh sách lịch sử các cuộc gọi đã được gửi lên backend (nếu cần).

---

## 3. Các màn hình dự kiến

### 3.1. Màn hình Splash / Khởi động

- Hiển thị logo, check trạng thái đăng nhập.
- Nếu đã đăng nhập -> chuyển sang màn hình chính.
- Nếu chưa đăng nhập -> chuyển sang màn hình đăng nhập.

### 3.2. Màn hình Đăng nhập (Login)

- **Trường dữ liệu**:
  - Số điện thoại / email hoặc username (tùy cách định danh).
  - Mật khẩu.
- **Chức năng**:
  - Gọi API đăng nhập tới backend.
  - Lưu token (JWT hoặc tương đương) vào storage an toàn (AsyncStorage, SecureStore, v.v.).
  - Điều hướng sang màn hình chính sau khi đăng nhập thành công.
  - Link sang màn hình đăng ký.

### 3.3. Màn hình Đăng ký (Register)

- **Trường dữ liệu** (gợi ý):
  - Tên hiển thị.
  - Số điện thoại (của chính thiết bị hoặc số liên hệ chính).
  - Email (tùy chọn).
  - Mật khẩu / xác nhận mật khẩu.
- **Chức năng**:
  - Gọi API đăng ký tới backend.
  - Sau khi đăng ký thành công: chuyển sang màn hình đăng nhập hoặc tự động đăng nhập (tùy thiết kế chi tiết).

### 3.4. Màn hình Chính (Dashboard)

- Hiển thị:
  - Trạng thái: đã bật/tắt **theo dõi cuộc gọi đến**.
  - Nút chuyển (toggle) bật/tắt theo dõi.
  - Lối tắt vào màn hình quản lý danh sách số theo dõi.
  - (Tùy chọn) hiển thị số cuộc gọi đã được ghi nhận gần đây.

### 3.5. Màn hình Quản lý danh sách số theo dõi

- **Danh sách**:
  - Các số điện thoại đang được theo dõi (số gọi đến sẽ kích hoạt cảnh báo).
  - Mỗi item hiển thị: số điện thoại, ghi chú (nếu có), trạng thái bật/tắt theo dõi.
- **Chức năng**:
  - Thêm số mới:
    - Nhập số điện thoại thủ công.
    - (Tùy chọn) Chọn từ danh bạ nếu được phép truy cập.
  - Sửa / xóa số trong danh sách.
  - Bật/tắt theo dõi từng số (toggle).
  - Đồng bộ danh sách với backend (tạo/sửa/xóa thông qua API).

### 3.6. Màn hình Lịch sử cuộc gọi đã gửi

- (Tuỳ yêu cầu, có thể là màn hình riêng hoặc tab trong Dashboard)
- Hiển thị:
  - Danh sách các cuộc gọi đến đã được app gửi lên backend.
  - Thông tin: số gọi đến, thời gian, trạng thái xử lý (đã gửi thành công, lỗi, v.v.).

### 3.7. Màn hình Cài đặt (Settings)

- **Nhóm Thông báo & theo dõi**:
  - Bật/tắt hiển thị thông báo khi có cuộc gọi được gửi lên backend.
  - Bật/tắt chế độ theo dõi cuộc gọi (tương tự toggle ở Dashboard, nhưng chi tiết hơn).
- **Nhóm Quyền & trạng thái hệ thống**:
  - Hiển thị trạng thái từng quyền: `READ_PHONE_STATE`, `READ_CALL_LOG`, `READ_CONTACTS` (đã cấp / chưa cấp).
  - Nút mở nhanh **App Settings** của Android để người dùng cấp lại quyền nếu đã tắt.
  - Hiển thị trạng thái dịch vụ chạy nền (nếu dùng service/worker riêng).
- **Nhóm Tài khoản**:
  - Hiển thị thông tin cơ bản của user (tên, số điện thoại).
  - Nút **Đăng xuất** (xoá token, đưa về màn hình Login).

---

## 4. Luồng kỹ thuật: Lắng nghe cuộc gọi và gửi sự kiện

### 4.1. Quyền (permissions) trên Android

- App cần xin các quyền (tùy vào phiên bản Android và policy hiện hành):
  - `READ_PHONE_STATE` hoặc `READ_CALL_LOG` (tuỳ cách hiện thực).
  - `READ_CONTACTS` (nếu cho phép chọn số từ danh bạ).
  - Quyền chạy nền / nhận broadcast về trạng thái cuộc gọi (BroadcastReceiver/Telephony).
- Cần thiết kế sao cho:
  - Khi cài đặt lần đầu, app yêu cầu quyền một cách có giải thích (tooltip/onboarding).
  - Nếu quyền bị tắt, app hiển thị cảnh báo và hướng dẫn người dùng bật lại trong Settings.

### 4.2. Luồng sự kiện cuộc gọi đến

1. Điện thoại nhận cuộc gọi đến.
2. Module native (hoặc thư viện phù hợp) trên Android bắt được sự kiện cuộc gọi.
3. App lấy số gọi đến (nếu hệ thống cho phép).
4. App kiểm tra:
   - Số gọi đến có nằm trong **danh sách số theo dõi** (đã lưu local hoặc cache từ server) hay không.
5. Nếu **có**:
   - Tạo payload sự kiện và gửi lên backend qua API (xem phần 5).
   - (Tuỳ chọn) hiển thị thông báo nội bộ trong app/notification.
6. Nếu **không**:
   - Không gửi gì (hoặc có thể log nội bộ tuỳ yêu cầu).

> Lưu ý: việc lắng nghe và truy cập thông tin cuộc gọi trên Android phụ thuộc mạnh vào version OS và policy Google. Khi triển khai thực tế cần kiểm tra kỹ guideline hiện hành của Google Play để tránh vi phạm chính sách.

---

## 5. API backend mà mobile sử dụng (dự kiến)

Các endpoint dưới đây là gợi ý, chi tiết sẽ được chuẩn hoá trong `docs/backend-spec.md`:

- **Auth**

  - `POST /api/auth/register`
    - Body: thông tin đăng ký (tên, số điện thoại, email, mật khẩu, v.v.).
    - Response: thông tin user, token (tuỳ chọn) hoặc yêu cầu login lại.
  - `POST /api/auth/login`
    - Body: username/phone + password.
    - Response: token (JWT) để dùng cho các API khác.

- **Quản lý danh sách số theo dõi**

  - `GET /api/watch-numbers`
    - Lấy danh sách số đang được theo dõi cho user hiện tại.
  - `POST /api/watch-numbers`
    - Thêm số mới vào danh sách theo dõi.
  - `PUT /api/watch-numbers/{id}`
    - Cập nhật thông tin số theo dõi (ví dụ bật/tắt, ghi chú).
  - `DELETE /api/watch-numbers/{id}`
    - Xoá số khỏi danh sách theo dõi.

- **Gửi sự kiện cuộc gọi đến**

  - `POST /api/call-events`
    - App gọi tới endpoint này **mỗi khi có cuộc gọi đến từ số theo dõi**.
    - Body (ví dụ):
      - `callerNumber`: số gọi đến.
      - `calledAt`: thời điểm nhận cuộc gọi (timestamp).
      - `callStatus`: trạng thái (ringing/answered/missed – tuỳ mức độ lấy được).
      - `deviceId` hoặc thông tin nhận diện thiết bị (nếu cần).
    - Backend sẽ:
      - Xác thực token.
      - Kiểm tra business rule.
      - Ghi log / lưu DB.
      - Publish message lên MQTT.

- **Lịch sử sự kiện cuộc gọi**
  - `GET /api/call-events`
    - Lấy danh sách các sự kiện cuộc gọi mà backend đã nhận (lọc theo user).

---

## 6. Cấu trúc project React Native (gợi ý)

- `src/`
  - `screens/`
    - `LoginScreen.tsx`
    - `RegisterScreen.tsx`
    - `DashboardScreen.tsx`
    - `WatchListScreen.tsx`
    - `CallHistoryScreen.tsx`
    - `SettingsScreen.tsx`
  - `components/`
    - Các component dùng chung: button, input, list item, modal...
  - `navigation/`
    - Cấu hình React Navigation (stack/tab).
  - `store/` hoặc `state/`
    - Cấu hình Redux/Zustand, slice cho auth, watchList, callEvents...
  - `services/`
    - Gọi API backend (`apiClient`, `authService`, `callService`, v.v.).
  - `native/` (nếu cần)
    - Các bridge/module để giao tiếp với chức năng native Android (lắng nghe cuộc gọi).

---

## 7. Bước tiếp theo cho phần mobile

- Chốt danh sách màn hình và luồng điều hướng (navigation flow).
- Chốt format chính xác của payload gửi lên API `/api/call-events`.
- Xác định thư viện hoặc cách triển khai module native để lắng nghe cuộc gọi trên Android cho React Native.
- Sau khi bạn duyệt tài liệu này, có thể bắt đầu khởi tạo skeleton project React Native (Android-only) và base code cho các màn hình + luồng auth.
