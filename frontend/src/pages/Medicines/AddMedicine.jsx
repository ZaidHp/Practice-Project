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
import { medicineService } from '../../services/medicineService';
import { categoryService } from '../../services/categoryService';
const AddMedicine = () => {
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState({
    medicineCode: '',
    medicineName: '',
    genericName: '',
    categoryId: '',
    reorderLevel: '',
    unitOfMeasure: '',
  });

  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [fetchingCategories, setFetchingCategories] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await categoryService.getCategories();
        setCategories(response.data || []); 
      } catch (err) {
        setError('Failed to load categories. Please refresh the page.');
      } finally {
        setFetchingCategories(false);
      }
    };

    fetchCategories();
  }, []);

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
      ...formData,
      categoryId: parseInt(formData.categoryId, 10) || 0,
      reorderLevel: parseInt(formData.reorderLevel, 10) || 0
    };

    try {
      await medicineService.createMedicine(payload);
      navigate('/medicines');
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
          Add New Medicine
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Register a new medicine into the inventory system.
        </Typography>

        {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Box component="form" onSubmit={handleSubmit} noValidate>
          <Grid container spacing={2}>
            
            {/* Medicine Code */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="medicineCode"
                name="medicineCode"
                label="Medicine Code"
                value={formData.medicineCode}
                onChange={handleChange}
                placeholder="e.g., MED-001"
              />
            </Grid>

            {/* Medicine Name */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="medicineName"
                name="medicineName"
                label="Medicine Name"
                value={formData.medicineName}
                onChange={handleChange}
              />
            </Grid>

            {/* Generic Name */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                fullWidth
                id="genericName"
                name="genericName"
                label="Generic Name"
                value={formData.genericName}
                onChange={handleChange}
                placeholder="Optional generic equivalent"
              />
            </Grid>

            {/* Unit of Measure */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="unitOfMeasure"
                name="unitOfMeasure"
                label="Unit of Measure"
                value={formData.unitOfMeasure}
                onChange={handleChange}
                placeholder="e.g., mg, ml, tablet, box"
              />
            </Grid>

            {/* Category Dropdown */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <FormControl fullWidth required disabled={fetchingCategories}>
                <InputLabel id="category-select-label">Category</InputLabel>
                <Select
                  labelId="category-select-label"
                  id="categoryId"
                  name="categoryId"
                  value={formData.categoryId}
                  label="Category"
                  onChange={handleChange}
                >
                  {fetchingCategories ? (
                    <MenuItem disabled value="">
                      <em>Loading categories...</em>
                    </MenuItem>
                  ) : (
                    categories.map((category) => (
                      <MenuItem key={category.categoryId} value={category.categoryId}>
                        {category.categoryName}
                      </MenuItem>
                    ))
                  )}
                </Select>
              </FormControl>
            </Grid>

            {/* Reorder Level */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="reorderLevel"
                name="reorderLevel"
                label="Reorder Level"
                type="number"
                inputProps={{ min: 0 }}
                value={formData.reorderLevel}
                onChange={handleChange}
                helperText="Stock alert threshold"
              />
            </Grid>

          </Grid>

          {/* Form Actions */}
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 4, gap: 2 }}>
            <Button 
              variant="outlined" 
              onClick={() => navigate('/medicines')}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              variant="contained" 
              disabled={loading || fetchingCategories || !formData.medicineCode || !formData.medicineName}
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