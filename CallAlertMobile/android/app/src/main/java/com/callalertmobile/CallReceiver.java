package com.callalertmobile;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.provider.Settings;
import android.telephony.TelephonyManager;
import android.util.Log;
import android.widget.Toast; // Thêm thư viện Toast
import android.os.Handler;
import android.os.Looper;

import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;
import java.util.TimeZone;
import org.json.JSONObject;

public class CallReceiver extends BroadcastReceiver {
    private static final String API_URL = "https://locally-untinned-donn.ngrok-free.dev/api/callevents";

    @Override
    public void onReceive(Context context, Intent intent) {
        String state = intent.getStringExtra(TelephonyManager.EXTRA_STATE);

        // Hiển thị thông báo ngay khi nhận được tín hiệu (Để biết code có chạy)
        Log.d("CallReceiver", "Nhận được sự kiện điện thoại, Trạng thái: " + state);

        if (TelephonyManager.EXTRA_STATE_RINGING.equals(state)) {
            String incomingNumber = intent.getStringExtra(TelephonyManager.EXTRA_INCOMING_NUMBER);
            String deviceId = Settings.Secure.getString(context.getContentResolver(), Settings.Secure.ANDROID_ID);

            // Hiển thị Toast trên màn hình điện thoại
            showToast(context, "Phát hiện cuộc gọi: " + (incomingNumber != null ? incomingNumber : "Ẩn số"));

            new Thread(() -> {
                sendCallEventToServer(context, incomingNumber, state, deviceId);
            }).start();
        }
    }

    private void sendCallEventToServer(Context context, String callerNumber, String callStatus, String deviceId) {
        HttpURLConnection conn = null;
        try {
            URL url = new URL(API_URL);
            conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("POST");
            conn.setRequestProperty("Content-Type", "application/json; utf-8");
            conn.setRequestProperty("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9tb2JpbGVwaG9uZSI6IjAyMzE0NDU1ODQiLCJleHAiOjE3NjY1NTU5MjEsImlzcyI6IkNhbGxBbGVydCIsImF1ZCI6IkNhbGxBbGVydENsaWVudHMifQ.61mG4Zosu6WUUveDoWbGGM2ouFNxwGiBxRdRUOFoZ9o");
            conn.setConnectTimeout(5000); // Thêm Timeout để tránh treo
            conn.setDoOutput(true);

            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", Locale.US);
            sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
            String calledAt = sdf.format(new Date());

            JSONObject jsonParam = new JSONObject();
            jsonParam.put("CallerNumber", callerNumber != null ? callerNumber : "Unknown");
            jsonParam.put("CalledAt", calledAt);
            jsonParam.put("CallStatus", callStatus);
            jsonParam.put("DeviceId", deviceId);

            Log.d("CallReceiver", "Đang gửi JSON: " + jsonParam.toString());

            try (OutputStream os = conn.getOutputStream()) {
                byte[] input = jsonParam.toString().getBytes("utf-8");
                os.write(input, 0, input.length);
            }

            int code = conn.getResponseCode();
            Log.d("CallReceiver", "Kết quả API: " + code);

            if (code == 200 || code == 201) {
                showToast(context, "API: Gửi thành công!");
            } else {
                showToast(context, "API Lỗi: Code " + code);
            }

        } catch (Exception e) {
            Log.e("CallReceiver", "Lỗi mạng/API: " + e.getMessage());
            showToast(context, "Lỗi kết nối API: " + e.getMessage());
        } finally {
            if (conn != null) conn.disconnect();
        }
    }

    // Hàm phụ để hiển thị Toast từ Thread ngầm
    private void showToast(final Context context, final String message) {
        new Handler(Looper.getMainLooper()).post(() ->
                Toast.makeText(context, message, Toast.LENGTH_SHORT).show()
        );
    }
}