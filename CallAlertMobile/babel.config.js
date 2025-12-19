module.exports = function(api) {
  api.cache(true);
  return {
    presets: ['babel-preset-expo'], // Sử dụng preset của Expo thay vì mặc định
    plugins: [
      'react-native-reanimated/plugin', // Cần thiết để sửa lỗi "Property 'require' doesn't exist"
    ],
  };
};