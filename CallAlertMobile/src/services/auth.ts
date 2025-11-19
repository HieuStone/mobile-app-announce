import {apiClient} from './api';

export interface AuthResponse {
  userId: number;
  username: string;
  phoneNumber?: string;
  email?: string;
  accessToken: string;
}

export interface LoginPayload {
  username: string;
  password: string;
}

export interface RegisterPayload extends LoginPayload {
  phoneNumber?: string;
  email?: string;
}

export const login = async (payload: LoginPayload) => {
  const {data} = await apiClient.post<AuthResponse>('/api/auth/login', payload);
  return data;
};

export const register = async (payload: RegisterPayload) => {
  const {data} = await apiClient.post<AuthResponse>(
    '/api/auth/register',
    payload,
  );
  return data;
};


