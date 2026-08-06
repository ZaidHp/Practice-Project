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
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  Alert,
  CircularProgress
} from '@mui/material';
import { supplierApi } from '../../APIs/supplierApi';
const AddMedicine = () => {
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState({
    SupplierName: '',
    ContactPerson: '',
    Phone: '',
    Email: '',
    Address: '',
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);


  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
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
      await supplierApi.createSupplier(payload);
      navigate('/suppliers');
    } catch (err) {
      const validationErrors = err.response?.data?.errors;
      let errorString = err.response?.data?.title 
        || err.response?.data?.message 
        || 'Failed to create medicine. Please try again.';

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
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4, borderRadius: 2 }}>
        <Typography variant="h5" component="h1" gutterBottom fontWeight="bold" color="primary">
          Add New Supplier
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Register a new supplier into the inventory system.
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
                value={formData.unitOfMeasure}
                onChange={handleChange}
                placeholder="e.g., user@mail.com"
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="Address"
                name="Address"
                label="Address"
                value={formData.unitOfMeasure}
                onChange={handleChange}
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
              {loading ? 'Saving...' : 'Create Medicine'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
};

export default AddMedicine;