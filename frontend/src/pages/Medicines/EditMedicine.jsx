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
import { medicineService } from '../../services/medicineService';
import { categoryService } from '../../services/categoryService';

const EditMedicine = () => {
  const navigate = useNavigate();
  const { id } = useParams();

  const [formData, setFormData] = useState({
    medicineCode: '',
    medicineName: '',
    genericName: '',
    categoryId: '',
    reorderLevel: '',
    unitOfMeasure: '',
    isActive: true,
  });

  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [fetchingData, setFetchingData] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        const [medicineRes, categoriesRes] = await Promise.all([
          medicineService.getMedicineById(id),
          categoryService.getCategories()
        ]);

        const medicine = medicineRes.data || medicineRes;
        const fetchedCategories = categoriesRes.data || categoriesRes || [];

        setCategories(fetchedCategories);
        
        setFormData({
          medicineCode: medicine.medicineCode || '',
          medicineName: medicine.medicineName || '',
          genericName: medicine.genericName || '',
          categoryId: medicine.categoryId || '',
          reorderLevel: medicine.reorderLevel !== undefined ? medicine.reorderLevel : '',
          unitOfMeasure: medicine.unitOfMeasure || '',
          isActive: medicine.isActive !== undefined ? medicine.isActive : true,
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
      ...formData,
      categoryId: parseInt(formData.categoryId, 10) || 0,
      reorderLevel: parseInt(formData.reorderLevel, 10) || 0
    };

    try {
      await medicineService.updateMedicine(id, payload);
      navigate('/medicines');
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
          Update medicine details, categorization, or status.
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
              />
            </Grid>

            {/* Category Dropdown */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <FormControl fullWidth required>
                <InputLabel id="category-select-label">Category</InputLabel>
                <Select
                  labelId="category-select-label"
                  id="categoryId"
                  name="categoryId"
                  value={formData.categoryId}
                  label="Category"
                  onChange={handleChange}
                >
                  {categories.length === 0 ? (
                    <MenuItem disabled value="">
                      <em>No categories available</em>
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
              onClick={() => navigate('/medicines')}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              variant="contained" 
              disabled={loading || !formData.medicineCode || !formData.medicineName || !formData.categoryId}
              startIcon={loading ? <CircularProgress size={20} color="inherit" /> : null}
            >
              {loading ? 'Saving...' : 'Update Medicine'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
};

export default EditMedicine;