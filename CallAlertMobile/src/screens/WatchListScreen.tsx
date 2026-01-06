import React, { useEffect, useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TextInput,
  TouchableOpacity,
  Switch,
} from 'react-native';
import { apiClient } from '../services/api';

interface WatchNumber {
  id: string;
  phoneNumber: string;
  label?: string;
  isActive: boolean;
  userId: string;
}

const WatchListScreen: React.FC = () => {
  const [numbers, setNumbers] = useState<WatchNumber[]>([]);
  const [phoneNumber, setPhoneNumber] = useState('');
  const [label, setLabel] = useState('');
  const [loading, setLoading] = useState(false);
  const [refreshing, setRefreshing] = useState(false);

  const fetchNumbers = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get('/api/watchnumbers');
      setNumbers(response.data);
    } catch (error) {
      console.error("Lỗi khi lấy danh sách:", error);
      // alert("Không thể tải danh sách từ server");
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };
  useEffect(() => {
    fetchNumbers();
  }, []);

  const onRefresh = () => {
    setRefreshing(true);
    fetchNumbers();
  };

  const addNumber = async () => {
    if (!phoneNumber.trim()) {
      alert("Vui lòng nhập số điện thoại");
      return;
    }

    setLoading(true);

    try {
      const payload = {
        phoneNumber: phoneNumber,
        label: label,
        isActive: true,
      };

      const response = await apiClient.post('/api/watchnumbers', payload);

      if (response.status === 200 || response.status === 201) {
        // 3. Cập nhật giao diện với dữ liệu trả về từ server
        const savedNumber: WatchNumber = {
          id: response.data.id || Date.now().toString(),
          phoneNumber: response.data.phoneNumber,
          label: response.data.label,
          isActive: response.data.isActive,
          userId: response.data.userId.toString()
        };

        setNumbers(prev => [savedNumber, ...prev]);
        setPhoneNumber('');
        setLabel('');
        alert("Đã đăng ký số thành công!");
      }
    } catch (error: any) {
      console.error("Lỗi khi đăng ký số:", error);

      // Xử lý lỗi hiển thị cho người dùng
      const errorMsg = error.response?.data?.message || "Không thể kết nối tới server";
      alert("Lỗi: " + errorMsg);
    } finally {
      setLoading(false);
    }
  };

  const toggleActive = (id: string) => {
    setNumbers(prev =>
      prev.map(item =>
        item.id === id ? { ...item, isActive: !item.isActive } : item,
      ),
    );
  };

  const renderItem = ({ item }: { item: WatchNumber }) => (
    <View style={styles.item}>
      <View>
        <Text style={styles.phone}>{item.phoneNumber}</Text>
        {item.label ? <Text style={styles.label}>{item.label}</Text> : null}
      </View>
      <Switch value={item.isActive} onValueChange={() => toggleActive(item.id)} />
    </View>
  );

  return (
    <View style={styles.container}>
      <Text style={styles.heading}>Danh sách số theo dõi</Text>
      <View style={styles.form}>
        <TextInput
          style={styles.input}
          placeholder="Số điện thoại"
          keyboardType="phone-pad"
          value={phoneNumber}
          onChangeText={setPhoneNumber}
        />
        <TextInput
          style={styles.input}
          placeholder="Ghi chú (không bắt buộc)"
          value={label}
          onChangeText={setLabel}
        />
        <TouchableOpacity style={styles.button} onPress={addNumber}>
          <Text style={styles.buttonText}>Thêm số</Text>
        </TouchableOpacity>
      </View>
      <FlatList
        data={numbers}
        keyExtractor={item => item.id}
        renderItem={renderItem}
        ItemSeparatorComponent={() => <View style={styles.separator} />}
        refreshing={refreshing}
        onRefresh={onRefresh}
        ListEmptyComponent={() => (
          <View style={{ alignItems: 'center', marginTop: 20 }}>
            <Text>{loading ? 'Đang tải...' : 'Chưa có số nào được theo dõi'}</Text>
          </View>
        )}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
    backgroundColor: '#f8fafc',
  },
  heading: {
    fontSize: 22,
    fontWeight: '700',
    marginBottom: 16,
  },
  form: {
    marginBottom: 16,
  },
  input: {
    borderWidth: 1,
    borderColor: '#cbd5f5',
    padding: 12,
    borderRadius: 8,
    backgroundColor: '#fff',
    marginBottom: 12,
  },
  button: {
    backgroundColor: '#2563eb',
    padding: 14,
    borderRadius: 8,
    alignItems: 'center',
  },
  buttonText: {
    color: '#fff',
    fontWeight: '600',
  },
  item: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 12,
  },
  phone: {
    fontSize: 16,
    fontWeight: '600',
  },
  label: {
    fontSize: 13,
    color: '#475569',
  },
  separator: {
    height: 1,
    backgroundColor: '#e2e8f0',
  },
});

export default WatchListScreen;


