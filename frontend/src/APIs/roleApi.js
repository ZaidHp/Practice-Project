import api from '../services/axiosService';

export const roleApi = {
  getRoles: async () => {
    const response = await api.get('/role'); 
    return response.data;
  }
};