import React, {useState} from 'react';
import {View, Text, StyleSheet, Switch, TouchableOpacity} from 'react-native';
import {useAuth} from '../context/AuthContext';

const DashboardScreen: React.FC = () => {
  const {logout} = useAuth();
  const [monitoring, setMonitoring] = useState(true);
  const [showNotification, setShowNotification] = useState(true);

  return (
    <View style={styles.container}>
      <Text style={styles.heading}>Bảng điều khiển</Text>
      <View style={styles.card}>
        <Text style={styles.cardTitle}>Theo dõi cuộc gọi đến</Text>
        <View style={styles.row}>
          <Text style={styles.label}>Đang bật</Text>
          <Switch value={monitoring} onValueChange={setMonitoring} />
        </View>
        <Text style={styles.description}>
          Khi bật, ứng dụng sẽ lắng nghe cuộc gọi đến và gửi sự kiện lên backend
          nếu số nằm trong danh sách theo dõi.
        </Text>
      </View>

      <View style={styles.card}>
        <Text style={styles.cardTitle}>Thông báo trên thiết bị</Text>
        <View style={styles.row}>
          <Text style={styles.label}>Hiển thị thông báo</Text>
          <Switch value={showNotification} onValueChange={setShowNotification} />
        </View>
      </View>

      <TouchableOpacity style={styles.logoutButton} onPress={logout}>
        <Text style={styles.logoutText}>Đăng xuất</Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#eef2ff',
    padding: 16,
  },
  heading: {
    fontSize: 24,
    fontWeight: '700',
    marginBottom: 16,
  },
  card: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 12,
    marginBottom: 16,
    elevation: 2,
  },
  cardTitle: {
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 8,
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  label: {
    fontSize: 16,
  },
  description: {
    fontSize: 13,
    color: '#475569',
  },
  logoutButton: {
    backgroundColor: '#ef4444',
    padding: 16,
    borderRadius: 12,
    alignItems: 'center',
    marginTop: 'auto',
  },
  logoutText: {
    color: '#fff',
    fontWeight: '600',
  },
});

export default DashboardScreen;


