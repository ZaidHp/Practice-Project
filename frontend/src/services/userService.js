import api from './api';

export const userService = {

  createUser: async (userData) => {
    const response = await api.post('/users', userData);
    console.log('User creation response:', response);
    return response.data;
  },
  
  getUsers: async () => {
    const response = await api.get('/users');
    return response.data;
  },

  deleteUser: async (id) => {
    const response = await api.delete(`/users/${id}`); 
    return response.data;
  }
};