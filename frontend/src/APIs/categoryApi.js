import api from './../services/axiosService';

export const categoryApi = {
  getCategories: async (page = 1, pageSize = 10, search = '') => {
    const response = await api.get('/category', {
      params: { page, pageSize, search }
    });
    return response.data;
  },

  createCategory: async (categoryData) => {
    const response = await api.post('/category', categoryData);
    return response.data;
  },

  getCategoryById: async (id) => {
    const response = await api.get(`/category/${id}`);
    return response.data;
  },

  updateCategory: async (id, categoryData) => {
    const response = await api.put(`/category/${id}`, categoryData);
    return response.data;
  },

  deleteCategory: async (id) => {
    const response = await api.delete(`/category/${id}`);
    return response.data;
  },

  getAllCategories: async () => {
    const response = await api.get('/category/all');
    return response.data;
  }
};