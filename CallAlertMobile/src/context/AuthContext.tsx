import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from 'react';
import {login as loginApi, register as registerApi} from '../services/auth';
import {setAuthToken} from '../services/api';
import {clearToken, loadToken, saveToken} from '../storage/tokenStorage';

interface AuthContextValue {
  token: string | null;
  loading: boolean;
  login: (username: string, password: string) => Promise<void>;
  register: (
    username: string,
    password: string,
    phoneNumber?: string,
    email?: string,
  ) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue>({
  token: null,
  loading: true,
  login: async () => {},
  register: async () => {},
  logout: async () => {},
});

export const AuthProvider: React.FC<React.PropsWithChildren> = ({
  children,
}) => {
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const bootstrap = async () => {
      const storedToken = await loadToken();
      if (storedToken) {
        setToken(storedToken);
        setAuthToken(storedToken);
      }
      setLoading(false);
    };
    bootstrap();
  }, []);

  const login = useCallback(async (username: string, password: string) => {
    const response = await loginApi({username, password});
    setToken(response.accessToken);
    setAuthToken(response.accessToken);
    await saveToken(response.accessToken);
  }, []);

  const register = useCallback(
    async (
      username: string,
      password: string,
      phoneNumber?: string,
      email?: string,
    ) => {
      const response = await registerApi({
        username,
        password,
        phoneNumber,
        email,
      });
      setToken(response.accessToken);
      setAuthToken(response.accessToken);
      await saveToken(response.accessToken);
    },
    [],
  );

  const logout = useCallback(async () => {
    setToken(null);
    setAuthToken(null);
    await clearToken();
  }, []);

  const value: AuthContextValue = {
    token,
    loading,
    login,
    register,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => useContext(AuthContext);


