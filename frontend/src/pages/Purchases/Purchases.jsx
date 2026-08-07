import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  CircularProgress,
  Alert
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { purchaseApi } from '../../APIs/purchaseApi';

const Purchases = () => {
  const navigate = useNavigate();
  const [purchases, setPurchases] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchPurchases();
  }, []);

  const fetchPurchases = async () => {
    try {
      setLoading(true);
      const response = await purchaseApi.getPurchases();
      setPurchases(response.data || response || []);
      setError(null);
    } catch (err) {
      setError('Failed to load purchases. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4, borderRadius: 2 }}>
        
        {/* Header Section */}
        <Box 
          sx={{ 
            display: 'flex', 
            alignItems: 'flex-start', 
            mb: 3 
          }}
        >
          <Box sx={{ flexGrow: 1 }}>
            <Typography variant="h5" component="h1" fontWeight="bold" color="primary">
              Purchase History
            </Typography>
            <Typography variant="body2" color="text.secondary">
              View and manage incoming inventory invoices.
            </Typography>
          </Box>
          <Button 
            variant="contained" 
            color="primary"
            startIcon={<AddIcon />} 
            onClick={() => navigate('/purchases/add')}
            sx={{ ml: 'auto' }}
          >
            Add Purchase
          </Button>
        </Box>

        {/* Error Notification */}
        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
            {error}
          </Alert>
        )}

        {/* Data Table / Loading Spinner */}
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', my: 6 }}>
            <CircularProgress />
          </Box>
        ) : (
          <TableContainer>
            <Table sx={{ minWidth: 800 }} aria-label="purchases table">
              <TableHead>
                <TableRow>
                  <TableCell><strong>Purchase ID</strong></TableCell>
                  <TableCell><strong>Invoice Number</strong></TableCell>
                  <TableCell><strong>Supplier</strong></TableCell>
                  <TableCell><strong>Purchase Date</strong></TableCell>
                  <TableCell align="right"><strong>Total Amount</strong></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {purchases.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={5} align="center" sx={{ py: 3 }}>
                      <Typography variant="body1" color="text.secondary">
                        No purchases found.
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  purchases.map((purchase) => (
                    <TableRow key={purchase.purchaseId || purchase.id} hover>
                      <TableCell>{purchase.purchaseId || purchase.id}</TableCell>
                      <TableCell>{purchase.invoiceNumber}</TableCell>
                      <TableCell>{purchase.supplierName || `Supplier ID: ${purchase.supplierId}`}</TableCell>
                      <TableCell>{formatDate(purchase.purchaseDate)}</TableCell>
                      <TableCell align="right">
                        Rs.{(purchase.totalAmount || 0).toFixed(2)}
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>
    </Container>
  );
};

export default Purchases;