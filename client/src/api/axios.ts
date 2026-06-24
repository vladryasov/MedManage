import axios from 'axios';

const STORAGE_KEYS = {
  TOKEN: 'jwt_token',
  REFRESH: 'jwt_refresh',
};

const apiClient = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

let isRefreshing = false;
let pendingQueue: Array<{
  resolve: (token: string) => void;
  reject: (err: unknown) => void;
}> = [];

function processQueue(error: unknown, token: string | null) {
  pendingQueue.forEach((p) => {
    if (error || !token) {
      p.reject(error);
    } else {
      p.resolve(token);
    }
  });
  pendingQueue = [];
}

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(STORAGE_KEYS.TOKEN);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error);
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        pendingQueue.push({ resolve, reject });
      })
        .then((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return apiClient(originalRequest);
        })
        .catch((err) => Promise.reject(err));
    }

    originalRequest._retry = true;
    isRefreshing = true;

    const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH);
    if (!refreshToken) {
      isRefreshing = false;
      window.dispatchEvent(new CustomEvent('auth:unauthorized'));
      return Promise.reject(error);
    }

    try {
      const { data } = await axios.post('/api/Auth/refresh', { refreshToken });
      const { accessToken, refreshToken: newRefresh } = data;

      localStorage.setItem(STORAGE_KEYS.TOKEN, accessToken);
      localStorage.setItem(STORAGE_KEYS.REFRESH, newRefresh);

      processQueue(null, accessToken);

      originalRequest.headers.Authorization = `Bearer ${accessToken}`;
      return apiClient(originalRequest);
    } catch (refreshError) {
      processQueue(refreshError, null);
      localStorage.removeItem(STORAGE_KEYS.TOKEN);
      localStorage.removeItem(STORAGE_KEYS.REFRESH);
      window.dispatchEvent(new CustomEvent('auth:unauthorized'));
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  },
);

export { STORAGE_KEYS };
export default apiClient;
