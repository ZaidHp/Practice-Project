import api from '../services/axiosService';

export const authApi = {
  login: async (credentials) => {
    const response = await api.post('/auth/login', credentials);
    
    if (response.data && response.data.data.token) {
      localStorage.setItem('token', response.data.data.token);
    }
    
    return response.data;
  },
  
  logout: () => {
    localStorage.removeItem('token');
    window.location.replace("/login");
  }
};
