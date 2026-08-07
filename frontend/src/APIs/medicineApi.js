import api from './../services/axiosService';

export const medicineApi = {
  getMedicines: async (page = 1, pageSize = 10, search = '') => {
    const response = await api.get('/medicines', {
      params: { page, pageSize, search }
    });
    return response.data;
  },

  createMedicine: async (medicineData) => {
    const response = await api.post('/medicines', medicineData);
    return response.data;
  },

  getMedicineById: async (id) => {
    const response = await api.get(`/medicines/${id}`);
    return response.data;
  },

  updateMedicine: async (id, medicineData) => {
    const response = await api.put(`/medicines/${id}`, medicineData);
    return response.data;
  },

  deleteMedicine: async (id) => {
    const response = await api.delete(`/medicines/${id}`);
    return response.data;
  }
};