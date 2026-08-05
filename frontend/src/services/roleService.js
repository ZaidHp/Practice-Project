import api from './api';

export const roleService = {
  getRoles: async () => {
    const response = await api.get('/role'); 
    return response.data;
  }
};