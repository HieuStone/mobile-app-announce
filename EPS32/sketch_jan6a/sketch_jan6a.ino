#include <WiFi.h>
#include <PubSubClient.h>

// --- Cấu hình thông tin ---
const char* ssid = "TEN_WIFI_CUA_BAN";
const char* password = "MAT_KHAU_WIFI";
const char* mqtt_server = "IP_MAY_CHU_MOSQUITTO"; // Ví dụ: 192.168.1.39
const char* topic_subscribe = "calls/incoming";
const int ledPin = 2; // Chân GPIO dùng để xuất tín hiệu (thường GPIO 2 là LED onboard)

WiFiClient espClient;
PubSubClient client(espClient);

// Hàm xử lý khi có tin nhắn mới đến
void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Tin nhắn mới từ topic [");
  Serial.print(topic);
  Serial.println("]");

  // Thực hiện nhấp nháy 10 lần
  for (int i = 0; i < 10; i++) {
    digitalWrite(ledPin, HIGH);
    delay(200); // Sáng 0.2 giây
    digitalWrite(ledPin, LOW);
    delay(200); // Tắt 0.2 giây
  }
}

// Hàm kết nối WiFi
void setup_wifi() {
  delay(10);
  Serial.println();
  Serial.print("Dang ket noi WiFi: ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi da ket noi!");
}

// Hàm kết nối MQTT Broker
void reconnect() {
  while (!client.connected()) {
    Serial.print("Dang ket noi MQTT...");
    // Tạo ID ngẫu nhiên cho ESP32
    String clientId = "ESP32Client-";
    clientId += String(random(0xffff), HEX);

    if (client.connect(clientId.c_str())) {
      Serial.println("Thanh cong!");
      // Đăng ký nhận tin nhắn từ topic sau khi kết nối
      client.subscribe(topic_subscribe);
    } else {
      Serial.print("That bai, ma loi rc=");
      Serial.print(client.state());
      Serial.println(" Thu lai sau 5 giay.");
      delay(5000);
    }
  }
}

void setup() {
  pinMode(ledPin, OUTPUT);
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqtt_server, 1883); // Cổng mặc định của Mosquitto là 1883
  client.setCallback(callback);
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop(); // Duy trì kết nối MQTT
}