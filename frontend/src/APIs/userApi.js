import api from '../services/axiosService';

export const userApi = {

  createUser: async (userData) => {
    const response = await api.post('/users', userData);
    return response.data;
  },
  
  getUsers: async (page = 1, pageSize = 10, search = '') => {
    const response = await api.get('/users', {
      params: { page, pageSize, search }
    });
    return response.data;
  },

  deleteUser: async (id) => {
    const response = await api.delete(`/users/${id}`);
    return response.data;
  }
};