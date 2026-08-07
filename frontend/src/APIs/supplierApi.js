import api from './../services/axiosService';

export const supplierApi = {
    getSupplires: async (page = 1, pageSize = 10, search = '') => {
        const response = await api.get('/suppliers', {
            params: { page, pageSize, search }
        });
        return response.data;
    },

    createSupplier: async (supplierData) => {
        const response = await api.post('/suppliers', supplierData);
        return response.data;
    },

    getSupplierById: async (id) => {
        const response = await api.get(`/suppliers/${id}`);
        return response.data;
    },

    updateSupplier: async (id, supplierData) => {
        const response = await api.put(`/suppliers/${id}`, supplierData);
        return response.data
    },

    deleteSupplier: async (id) => {
        const response = await api.delete(`/suppliers/${id}`);
        return response.data;
    }
}