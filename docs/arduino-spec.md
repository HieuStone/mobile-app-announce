## 1. Mục tiêu phần cứng ESP32

ESP32 sẽ đóng vai trò:

- Kết nối **WiFi** vào mạng nội bộ (cùng network với server chạy Mosquitto nếu có thể).
- Kết nối tới **MQTT Broker (Mosquitto)** và subscribe topic, ví dụ: `calls/incoming`.
- Khi nhận message báo có cuộc gọi đến, điều khiển **đèn cảnh báo** (LED / relay điều khiển đèn ngoài).

---

## 2. Kiến trúc tổng thể phía ESP32

- **Thành phần chính trên ESP32**:
  - Module kết nối WiFi (cấu hình SSID/password).
  - MQTT client (dùng thư viện phù hợp, ví dụ `PubSubClient` hoặc thư viện MQTT cho ESP32).
  - Logic xử lý message MQTT:
    - Parse JSON payload.
    - Kiểm tra các trường cần thiết (`callerNumber`, `calledAt`, `callStatus`, ...).
    - Kích hoạt đèn cảnh báo.
  - Module điều khiển đèn:
    - Có thể dùng LED trên board (để test) hoặc relay điều khiển đèn 220V bên ngoài.
    - Pattern nháy: ví dụ nháy liên tục trong X giây hoặc cho đến khi hết timeout.

---

## 3. Kết nối WiFi

- Cấu hình (hard-code hoặc đọc từ file/EEPROM tuỳ thiết kế):

  - `WIFI_SSID`
  - `WIFI_PASSWORD`

- Luồng kết nối:
  1. Khởi động ESP32.
  2. Kết nối vào WiFi với SSID/PASSWORD đã cấu hình.
  3. Sau khi có IP → tiến hành kết nối MQTT.
  4. Nếu mất WiFi, tự động thử reconnect theo chu kỳ.

---

## 4. Kết nối MQTT

- Thông số cấu hình:

  - `MQTT_BROKER_HOST` (ví dụ: IP server hoặc tên host trong Docker network).
  - `MQTT_BROKER_PORT` (mặc định 1883 nếu dùng không TLS).
  - `MQTT_USERNAME`, `MQTT_PASSWORD` (nếu broker yêu cầu auth).
  - `MQTT_TOPIC_SUB` = `calls/incoming` (theo backend-spec).

- Luồng xử lý:
  1. Sau khi WiFi kết nối thành công, ESP32 khởi tạo client MQTT.
  2. Thực hiện `connect` tới broker với clientId (có thể là ESP32-<MAC>).
  3. Khi kết nối thành công:
     - Subscribe topic `calls/incoming`.
  4. Trong vòng lặp chính:
     - Gọi `client.loop()` (tuỳ thư viện) để nhận message.
     - Khi có message mới → callback xử lý.
  5. Nếu MQTT bị disconnect:
     - Thử reconnect định kỳ.

---

## 5. Định dạng message MQTT từ backend

Theo `docs/backend-spec.md`, backend sẽ publish payload JSON dạng:

```json
{
  "userId": "xxx",
  "callerNumber": "+8490xxxxxxx",
  "calledAt": "2025-01-01T12:34:56Z",
  "callStatus": "ringing"
}
```

Trên ESP32:

- Parse JSON (có thể dùng `ArduinoJson` hoặc thư viện tương tự).
- Các trường tối thiểu cần dùng cho logic đèn:
  - `callerNumber`
  - `calledAt`
  - `callStatus` (nếu muốn phân biệt kiểu nháy: rung chuông vs gọi nhỡ, v.v.).

Tuỳ yêu cầu, bạn có thể dùng mọi message đến topic `calls/incoming` để bật đèn (không cần phân biệt userId), hoặc sau này mở rộng thêm logic filter theo user/thiết bị.

---

## 6. Logic điều khiển đèn cảnh báo

- **Phần cứng**:

  - Sử dụng 1 pin GPIO của ESP32 để điều khiển:
    - LED on-board (để test).
    - Hoặc module relay → đèn 220V bên ngoài.

- **Pattern nháy gợi ý**:

  - Khi nhận được message mới:
    - Bật đèn nhấp nháy nhanh trong X giây (vd: 10–15 giây).
    - Hoặc nhấp nháy cho đến khi hết thời gian timeout cấu hình.
  - Cách nháy:
    - Bật/tắt GPIO với chu kỳ 300–500 ms (tuỳ mức độ muốn nổi bật).

- **Pseudo-flow**:
  1. Callback MQTT nhận message → set một biến trạng thái `alertActive = true` và lưu `alertStartTime`.
  2. Trong `loop()`:
     - Nếu `alertActive == true` và `millis() - alertStartTime < ALERT_DURATION`:
       - Thực hiện nháy đèn (dựa trên `millis()` để tránh dùng `delay` chặn chương trình).
     - Nếu quá thời gian:
       - Tắt đèn, set `alertActive = false`.

---

## 7. Cấu trúc code trên ESP32 (gợi ý)

- File chính (ví dụ `main.ino` hoặc project PlatformIO):

  - `setup()`:
    - Khởi tạo Serial (debug).
    - Cấu hình pin cho đèn (OUTPUT).
    - Kết nối WiFi.
    - Kết nối MQTT và subscribe topic.
  - `loop()`:
    - Gọi hàm xử lý MQTT (`client.loop()`).
    - Gọi hàm xử lý logic nháy đèn (dựa trên trạng thái alert).

- Chia logic thành các hàm:
  - `connectWiFi()`
  - `connectMQTT()`
  - `onMqttMessage(topic, payload)`
  - `handleAlert()`

---

## 8. Bước tiếp theo cho ESP32

- Chốt tham số:
  - SSID/PASSWORD WiFi.
  - Địa chỉ broker MQTT (IP/hostname, port, auth).
  - Pattern nháy đèn (thời gian, tốc độ, kiểu nháy).
- Viết code mẫu ESP32 (Arduino-style hoặc PlatformIO) dựa trên đặc tả này.
- Test bằng cách:
  - Dùng tool publish MQTT (hoặc backend thật) gửi thử message JSON.
  - Quan sát LED/đèn nháy theo đúng kỳ vọng.
