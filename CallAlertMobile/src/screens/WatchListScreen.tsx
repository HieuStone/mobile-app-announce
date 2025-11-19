import React, {useState} from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TextInput,
  TouchableOpacity,
  Switch,
} from 'react-native';

interface WatchNumber {
  id: string;
  phoneNumber: string;
  label?: string;
  isActive: boolean;
}

const sampleData: WatchNumber[] = [
  {id: '1', phoneNumber: '0909123456', label: 'Khách hàng A', isActive: true},
  {id: '2', phoneNumber: '0988887777', label: 'Sửa chữa', isActive: false},
];

const WatchListScreen: React.FC = () => {
  const [numbers, setNumbers] = useState(sampleData);
  const [phoneNumber, setPhoneNumber] = useState('');
  const [label, setLabel] = useState('');

  const addNumber = () => {
    if (!phoneNumber.trim()) {
      return;
    }
    const newNumber: WatchNumber = {
      id: Date.now().toString(),
      phoneNumber,
      label,
      isActive: true,
    };
    setNumbers(prev => [newNumber, ...prev]);
    setPhoneNumber('');
    setLabel('');
  };

  const toggleActive = (id: string) => {
    setNumbers(prev =>
      prev.map(item =>
        item.id === id ? {...item, isActive: !item.isActive} : item,
      ),
    );
  };

  const renderItem = ({item}: {item: WatchNumber}) => (
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


