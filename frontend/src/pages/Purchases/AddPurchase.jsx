import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  TextField,
  Button,
  Grid,
  Alert,
  CircularProgress,
  Autocomplete,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Divider,
  IconButton
} from '@mui/material';

import { medicineApi } from '../../APIs/medicineApi';
import { supplierApi } from '../../APIs/supplierApi';
import { purchaseApi } from '../../APIs/purchaseApi';

const AddPurchase = () => {
  const navigate = useNavigate();
  
  const [suppliers, setSuppliers] = useState([]);
  const [medicines, setMedicines] = useState([]);
  const [loading, setLoading] = useState(false);
  const [fetchingData, setFetchingData] = useState(true);
  const [error, setError] = useState(null);

  const [header, setHeader] = useState({
    supplierId: '',
    invoiceNumber: '',
    purchaseDate: new Date().toISOString().split('T')[0]
  });

  const [currentItem, setCurrentItem] = useState({
    medicineId: '',
    batchNumber: '',
    manufactureDate: '',
    expiryDate: '',
    quantityReceived: '',
    unitPrice: ''
  });

  const [purchaseItems, setPurchaseItems] = useState([]);

  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        const [medsRes, suppsRes] = await Promise.all([
          medicineApi.getAllMedicines(),
          supplierApi.getAllSuppliers()
        ]);
        setMedicines(medsRes.data || medsRes || []);
        setSuppliers(suppsRes.data || suppsRes || []);
      } catch (err) {
        setError('Failed to load initial data. Please refresh the page.');
      } finally {
        setFetchingData(false);
      }
    };
    fetchInitialData();
  }, []);

  const handleHeaderChange = (e) => {
    const { name, value } = e.target;
    setHeader((prev) => ({ ...prev, [name]: value }));
  };

  const handleItemChange = (e) => {
    const { name, value } = e.target;
    setCurrentItem((prev) => ({ ...prev, [name]: value }));
  };

  const handleAddItem = () => {
    if (
      !currentItem.medicineId || 
      !currentItem.quantityReceived || 
      !currentItem.unitPrice || 
      !currentItem.batchNumber ||
      !currentItem.expiryDate ||
      !currentItem.manufactureDate
    ) {
      alert("Please fill out all required medicine fields, including Manufacturing and Expiry dates.");
      return;
    }

    const med = medicines.find(m => m.id === currentItem.medicineId || m.medicineId === currentItem.medicineId);
    
    setPurchaseItems([...purchaseItems, { 
      ...currentItem, 
      medicineName: med?.name || med?.medicineName,
      medicineCode: med?.code || med?.medicineCode,
      quantityReceived: parseInt(currentItem.quantityReceived, 10),
      unitPrice: parseFloat(currentItem.unitPrice)
    }]);

    setCurrentItem({
      medicineId: '',
      batchNumber: '',
      manufactureDate: '',
      expiryDate: '',
      quantityReceived: '',
      unitPrice: ''
    });
  };

  const handleRemoveItem = (index) => {
    const newItems = purchaseItems.filter((_, i) => i !== index);
    setPurchaseItems(newItems);
  };

  const calculateTotalAmount = () => {
    return purchaseItems.reduce((total, item) => total + (item.quantityReceived * item.unitPrice), 0);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!header.supplierId || purchaseItems.length === 0) {
      setError("Please select a supplier and add at least one medicine to the invoice.");
      return;
    }

    setLoading(true);
    setError(null);

    const payload = {
      supplierId: parseInt(header.supplierId, 10),
      invoiceNumber: header.invoiceNumber,
      purchaseDate: new Date(header.purchaseDate).toISOString(),
      totalAmount: calculateTotalAmount(),
      medicines: purchaseItems.map(item => ({
        medicineId: parseInt(item.medicineId, 10),
        batchNumber: item.batchNumber,
        manufactureDate: item.manufactureDate ? new Date(item.manufactureDate).toISOString() : new Date().toISOString(),
        expiryDate: item.expiryDate ? new Date(item.expiryDate).toISOString() : new Date().toISOString(),
        quantityReceived: item.quantityReceived,
        unitPrice: item.unitPrice
      }))
    };

    try {
      await purchaseApi.addPurchases(payload);
      navigate('/purchases');
    } catch (err) {
      const validationErrors = err.response?.data?.errors;
      let errorString = err.response?.data?.title 
        || err.response?.data?.message 
        || 'Failed to process purchase. Please try again.';

      if (validationErrors) {
        const firstErrorKey = Object.keys(validationErrors)[0];
        errorString = validationErrors[firstErrorKey][0];
      }
      setError(errorString);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4, borderRadius: 2 }}>
        <Typography variant="h5" component="h1" gutterBottom fontWeight="bold" color="primary">
          Create Purchase Invoice
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Record an incoming invoice, update stock batches, and log transactions.
        </Typography>

        {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Box component="form" onSubmit={handleSubmit} noValidate>
          
          {/* --- SECTION 1: HEADER --- */}
          <Typography variant="h6" sx={{ mt: 2, mb: 2 }}>1. Invoice Details</Typography>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Autocomplete
                disabled={fetchingData}
                options={suppliers}
                // Adapt based on your supplier entity properties (e.g., s.id vs s.supplierId)
                getOptionLabel={(option) => option.name || option.supplierName || ''} 
                value={suppliers.find(s => (s.id || s.supplierId) === header.supplierId) || null}
                onChange={(event, newValue) => {
                  setHeader({ ...header, supplierId: newValue ? (newValue.id || newValue.supplierId) : '' });
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Search Supplier" required fullWidth />
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                required
                fullWidth
                id="invoiceNumber"
                name="invoiceNumber"
                label="Invoice Number"
                value={header.invoiceNumber}
                onChange={handleHeaderChange}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                required
                fullWidth
                type="date"
                id="purchaseDate"
                name="purchaseDate"
                label="Purchase Date"
                value={header.purchaseDate}
                onChange={handleHeaderChange}
                slotProps={{ inputLabel: { shrink: true } }}
              />
            </Grid>
          </Grid>

          <Divider sx={{ my: 4 }} />

          {/* --- SECTION 2: LINE ITEMS --- */}
          <Typography variant="h6" sx={{ mb: 2 }}>2. Add Medicines</Typography>
          <Grid container spacing={2} sx={{ alignItems: 'center' }}>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Autocomplete
                disabled={fetchingData}
                options={medicines}
                // Autocomplete automatically filters by the string returned here. 
                // This allows searching by either code or name.
                getOptionLabel={(option) => {
                   const code = option.code || option.medicineCode || '';
                   const name = option.name || option.medicineName || '';
                   return `${code} - ${name}`;
                }}
                value={medicines.find(m => (m.id || m.medicineId) === currentItem.medicineId) || null}
                onChange={(event, newValue) => {
                  setCurrentItem({ ...currentItem, medicineId: newValue ? (newValue.id || newValue.medicineId) : '' });
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Search Medicine" required fullWidth />
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                fullWidth
                id="batchNumber"
                name="batchNumber"
                label="Batch Number"
                value={currentItem.batchNumber}
                onChange={handleItemChange}
                required
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                fullWidth
                type="date"
                id="manufactureDate"
                name="manufactureDate"
                label="Mfg Date"
                slotProps={{ inputLabel: { shrink: true } }}
                onChange={handleItemChange}
                required
                value={currentItem.manufactureDate}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                fullWidth
                type="date"
                id="expiryDate"
                name="expiryDate"
                label="Expiry Date"
                value={currentItem.expiryDate}
                onChange={handleItemChange}
                slotProps={{ inputLabel: { shrink: true } }}
                required
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField
                fullWidth
                type="number"
                id="quantityReceived"
                name="quantityReceived"
                label="Quantity"
                value={currentItem.quantityReceived}
                onChange={handleItemChange}
                slotProps={{ htmlInput: { min: 1 } }}
                required
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField
                fullWidth
                type="number"
                id="unitPrice"
                name="unitPrice"
                label="Unit Price"
                value={currentItem.unitPrice}
                onChange={handleItemChange}
                slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                required
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <Button 
                variant="outlined" 
                fullWidth 
                onClick={handleAddItem}
                sx={{ height: '56px' }} 
              >
                Add Item
              </Button>
            </Grid>
          </Grid>

          <Divider sx={{ my: 4 }} />

          {/* --- SECTION 3: CART SUMMARY --- */}
          <Typography variant="h6" sx={{ mb: 2 }}>3. Invoice Summary</Typography>
          <TableContainer component={Paper} variant="outlined">
            <Table>
              <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
                <TableRow>
                  <TableCell><strong>Medicine</strong></TableCell>
                  <TableCell><strong>Batch</strong></TableCell>
                  <TableCell><strong>Qty</strong></TableCell>
                  <TableCell><strong>Unit Price</strong></TableCell>
                  <TableCell><strong>Total</strong></TableCell>
                  <TableCell align="center"><strong>Action</strong></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {purchaseItems.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} align="center" sx={{ py: 3, color: 'text.secondary' }}>
                      No medicines added to this invoice yet.
                    </TableCell>
                  </TableRow>
                ) : (
                  purchaseItems.map((item, index) => (
                    <TableRow key={index}>
                      <TableCell>{item.medicineCode} - {item.medicineName}</TableCell>
                      <TableCell>{item.batchNumber}</TableCell>
                      <TableCell>{item.quantityReceived}</TableCell>
                      <TableCell>Rs.{item.unitPrice.toFixed(2)}</TableCell>
                      <TableCell>Rs.{(item.quantityReceived * item.unitPrice).toFixed(2)}</TableCell>
                      <TableCell align="center">
                        <Button color="error" size="small" onClick={() => handleRemoveItem(index)}>
                          Remove
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>

          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 2 }}>
            <Typography variant="h6">
              Total Invoice Amount: Rs.{calculateTotalAmount().toFixed(2)}
            </Typography>
          </Box>

          {/* --- Form Actions --- */}
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 4, gap: 2 }}>
            <Button 
              variant="outlined" 
              onClick={() => navigate('/purchases')}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              variant="contained" 
              disabled={loading || fetchingData || purchaseItems.length === 0 || !header.supplierId}
              startIcon={loading ? <CircularProgress size={20} color="inherit" /> : null}
            >
              {loading ? 'Processing...' : 'Submit Complete Purchase'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
};

export default AddPurchase;