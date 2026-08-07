import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  TextField,
  Button,
  Grid,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  Alert,
  CircularProgress,
  FormControlLabel,
  Switch
} from '@mui/material';
import { supplierApi } from '../../APIs/supplierApi';


const EditSupplier = () => {
  const navigate = useNavigate();
  const { id } = useParams();

  const [formData, setFormData] = useState({
    SupplierName: '',
    ContactPerson: '',
    Phone: '',
    Email: '',
    Address: '',
    isActive: true,
  });

  const [loading, setLoading] = useState(false);
  const [fetchingData, setFetchingData] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        const [supplierRes] = await Promise.all([
          supplierApi.getSupplierById(id),
        ]);

        const supplier = supplierRes.data || supplierApi;
        
        setFormData({
          SupplierName: supplier.supplierName || '',
          ContactPerson: supplier.contactPerson || '',
          Phone: supplier.phone || '',
          Email: supplier.email || '',
          Address: supplier.address || '',
          isActive: supplier.isActive !== undefined ? supplier.isActive : true,
        });
      } catch (err) {
        setError('Failed to load medicine details. It may have been deleted.');
      } finally {
        setFetchingData(false);
      }
    };

    if (id) {
      fetchInitialData();
    }
  }, [id]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    const payload = {
      ...formData
    };

    try {
      await supplierApi.updateSupplier(id, payload);
      navigate('/suppliers');
    } catch (err) {
      const validationErrors = err.response?.data?.errors;
      let errorString = err.response?.data?.title 
        || err.response?.data?.message 
        || 'Failed to update medicine. Please try again.';

      if (validationErrors) {
        const firstErrorKey = Object.keys(validationErrors)[0];
        errorString = validationErrors[firstErrorKey][0];
      }

      setError(errorString);
    } finally {
      setLoading(false);
    }
  };

  if (fetchingData) {
    return (
      <Container maxWidth="md" sx={{ mt: 4, mb: 4, textAlign: 'center' }}>
        <CircularProgress />
        <Typography variant="body1" sx={{ mt: 2 }}>Loading medicine details...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4, borderRadius: 2 }}>
        <Typography variant="h5" component="h1" gutterBottom fontWeight="bold" color="primary">
          Edit Medicine
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Update supplier details.
        </Typography>

        {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Box component="form" onSubmit={handleSubmit} noValidate>
          <Grid container spacing={2}>
            
            {/* Supplier Name */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="SupplierName"
                name="SupplierName"
                label="Supplier Name"
                value={formData.SupplierName}
                onChange={handleChange}
              />
            </Grid>

            {/* Contact Person */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="ContactPerson"
                name="ContactPerson"
                label="Contact Person"
                value={formData.ContactPerson}
                onChange={handleChange}
              />
            </Grid>

            {/* Phone */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                fullWidth
                id="Phone"
                name="Phone"
                label="Phone"
                value={formData.Phone}
                onChange={handleChange}
                placeholder="e.g., 03********"
              />
            </Grid>

            {/* Unit of Measure */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="Email"
                name="Email"
                label="Email"
                value={formData.Email}
                onChange={handleChange}
                placeholder="e.g., user@mail.com"
              />
            </Grid>

            {/* Address */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="Address"
                name="Address"
                label="Address"
                value={formData.Address}
                onChange={handleChange}
              />
            </Grid>

            {/* Active Status Toggle */}
            <Grid size={12}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.isActive}
                    onChange={handleChange}
                    name="isActive"
                    color="primary"
                  />
                }
                label={formData.isActive ? "Status: Active" : "Status: Inactive"}
              />
            </Grid>

          </Grid>

          {/* Form Actions */}
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 4, gap: 2 }}>
            <Button 
              variant="outlined" 
              onClick={() => navigate('/suppliers')}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              variant="contained" 
              disabled={loading  || !formData.SupplierName || !formData.Phone}
              startIcon={loading ? <CircularProgress size={20} color="inherit" /> : null}
            >
              {loading ? 'Saving...' : 'Update Supplier'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
};

export default EditSupplier;