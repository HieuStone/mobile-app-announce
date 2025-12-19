import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import LoginScreen from '../screens/LoginScreen';
import RegisterScreen from '../screens/RegisterScreen';
import DashboardScreen from '../screens/DashboardScreen';
import WatchListScreen from '../screens/WatchListScreen';
import { useAuth } from '../context/AuthContext';
import { ActivityIndicator, View } from 'react-native';

export type AuthStackParamList = {
  Login: undefined;
  Register: undefined;
};

export type AppTabParamList = {
  Dashboard: undefined;
  WatchList: undefined;
};

const AuthStack = createNativeStackNavigator<AuthStackParamList>();
const AppTabs = createBottomTabNavigator<AppTabParamList>();

const AppTabsNavigator = () => (
  <AppTabs.Navigator screenOptions={{ headerShown: false }}>
    <AppTabs.Screen name="Dashboard" component={DashboardScreen} />
    <AppTabs.Screen name="WatchList" component={WatchListScreen} />
  </AppTabs.Navigator>
);

export const AppNavigator = () => {
  const { token, loading } = useAuth();

  if (loading) {
    return (
      <View style={{ flex: 1, alignItems: 'center', justifyContent: 'center' }}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  return (
    <NavigationContainer>
      {token ? (
        <AppTabsNavigator />
      ) : (
        <AuthStack.Navigator>
          <AuthStack.Screen
            name="Login"
            component={LoginScreen}
            options={{ headerShown: false }}
          />
          <AuthStack.Screen
            name="Register"
            component={RegisterScreen}
            options={{ headerShown: false }}
          />
        </AuthStack.Navigator>
      )}
    </NavigationContainer>
  );
};


