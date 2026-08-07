import api from '../services/axiosService';

export const purchaseApi = {
    addPurchases: async (purchaseData) => {
        const response = await api.post('/purchases', purchaseData);
            return response.data;
    },

    getPurchases: async (page = 1, pageSize = 10, search = '') => {
        const response = await api.get('/purchases', {
            params: { page, pageSize, search }
        });
        return response.data;
    }
}
