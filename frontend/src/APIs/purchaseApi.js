import api from '../services/axiosService';

export const purchaseApi = {
    addPurchases: async (purchaseData) => {
        const response = await api.post('/purchases', purchaseData);
            return response.data;
    },

    getPurchases: async () => {
        const response = await api.get('/purchases');
        return response.data;
    }
}
