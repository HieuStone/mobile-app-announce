/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import React, { useEffect } from 'react';
import { PermissionsAndroid, Platform, StatusBar, useColorScheme } from 'react-native';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { AuthProvider } from './src/context/AuthContext';
import { AppNavigator } from './src/navigation/AppNavigator';

const requestPermissions = async () => {
  if (Platform.OS === 'android') {
    try {
      const granted = await PermissionsAndroid.requestMultiple([
        PermissionsAndroid.PERMISSIONS.READ_PHONE_STATE,
        PermissionsAndroid.PERMISSIONS.READ_CALL_LOG,
      ]);

      if (
        granted['android.permission.READ_PHONE_STATE'] === PermissionsAndroid.RESULTS.GRANTED &&
        granted['android.permission.READ_CALL_LOG'] === PermissionsAndroid.RESULTS.GRANTED
      ) {
        console.log('Đã có quyền đọc trạng thái điện thoại');
      } else {
        console.log('Người dùng từ chối quyền');
      }
    } catch (err) {
      console.warn(err);
    }
  }
};

function App() {
  const isDarkMode = useColorScheme() === 'dark';
  console.log('APP.JS IS RUNNING');


  useEffect(() => {
    requestPermissions();
  }, []);
  return (
    <SafeAreaProvider>
      <StatusBar barStyle={isDarkMode ? 'light-content' : 'dark-content'} />
      <AuthProvider>
        <AppNavigator />
      </AuthProvider>
    </SafeAreaProvider>
  );
}

export default App;
