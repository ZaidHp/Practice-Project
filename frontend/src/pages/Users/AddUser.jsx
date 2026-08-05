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
import { userService } from '../../services/userService';
import { roleService } from '../../services/roleService';

const AddUser = () => {
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState({
    fullName: '',
    username: '', 
    email: '',
    password: '',
    roleId: '',
  });

  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [fetchingRoles, setFetchingRoles] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRoles = async () => {
      try {
        const response = await roleService.getRoles();
        setRoles(response.data || []); 
      } catch (err) {
        setError('Failed to load roles from the server.');
      } finally {
        setFetchingRoles(false);
      }
    };

    fetchRoles();
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
      roleId: parseInt(formData.roleId, 10) || 0
    };

    try {
      await userService.createUser(payload);
      navigate('/users');
    } catch (err) {
      const validationErrors = err.response?.data?.errors;
      let errorString = err.response?.data?.message || err.response?.data?.title || 'Failed to create user.';

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
    <Container maxWidth="sm" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4, borderRadius: 2 }}>
        <Typography variant="h5" component="h1" gutterBottom fontWeight="bold" color="primary">
          Add New User
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Create a new account for a staff member.
        </Typography>

        {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

        <Box component="form" onSubmit={handleSubmit} noValidate>
          <Grid container spacing={2}>
            {/* Full Name */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="fullName"
                name="fullName"
                label="Full Name"
                value={formData.fullName}
                onChange={handleChange}
              />
            </Grid>
            
            {/* User Name */}
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                required
                fullWidth
                id="userName"
                name="username"
                label="User Name"
                value={formData.username}
                onChange={handleChange}
              />
            </Grid>

            {/* Email */}
            <Grid size={12}>
              <TextField
                required
                fullWidth
                id="email"
                name="email"
                label="Email Address"
                type="email"
                value={formData.email}
                onChange={handleChange}
              />
            </Grid>

            {/* Password */}
            <Grid size={12}>
              <TextField
                required
                fullWidth
                id="password"
                name="password"
                label="Password"
                type="password"
                value={formData.password}
                onChange={handleChange}
                helperText="Must be at least 8 characters long."
              />
            </Grid>

            {/* Role Dropdown */}
            <Grid size={12}>
              <FormControl fullWidth required disabled={fetchingRoles}>
                <InputLabel id="role-select-label">Role</InputLabel>
                <Select
                  labelId="role-select-label"
                  id="roleId"
                  name="roleId"
                  value={formData.roleId}
                  label="Role"
                  onChange={handleChange}
                >
                  {fetchingRoles ? (
                    <MenuItem disabled value="">
                      <em>Loading roles...</em>
                    </MenuItem>
                  ) : (
                    roles.map((role) => (
                      <MenuItem key={role.roleId} value={role.roleId}>
                        {role.roleName}
                      </MenuItem>
                    ))
                  )}
                </Select>
              </FormControl>
            </Grid>
          </Grid>

          {/* Form Actions */}
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', mt: 3, gap: 2 }}>
            <Button 
              variant="outlined" 
              onClick={() => navigate('/users')}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              variant="contained" 
              disabled={loading || fetchingRoles}
              startIcon={loading ? <CircularProgress size={20} color="inherit" /> : null}
            >
              {loading ? 'Saving...' : 'Create User'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
};

export default AddUser;