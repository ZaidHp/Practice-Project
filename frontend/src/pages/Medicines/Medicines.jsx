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
  Chip,
  IconButton,
  Alert,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  TextField,
  InputAdornment,
  TablePagination
} from '@mui/material';
import { 
  Add as AddIcon, 
  Delete as DeleteIcon, 
  Edit as EditIcon,
  Search as SearchIcon 
} from '@mui/icons-material';
import { medicineApi } from '../../APIs/medicineApi';

const Medicines = () => {
  const navigate = useNavigate();

  let userRole = null;
  const token = localStorage.getItem('token');
  
  if (token) {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      userRole = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role;
    } catch (e) {
      console.error('Failed to decode token:', e);
    }
  }

  const canManage = userRole === 'Admin' || userRole === 'Store Manager';

  const [medicines, setMedicines] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [search, setSearch] = useState('');

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedMedicineId, setSelectedMedicineId] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const fetchMedicines = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await medicineApi.getMedicines(page + 1, rowsPerPage, search);
      
      setMedicines(response.data?.items || []);
      setTotalCount(response.data?.totalCount || 0);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load medicines from the server.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const delayDebounceFn = setTimeout(() => {
      fetchMedicines();
    }, 500);

    return () => clearTimeout(delayDebounceFn);
  }, [search, page, rowsPerPage]);

  const handleSearchChange = (e) => {
    setSearch(e.target.value);
    setPage(0);
  };

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleDeleteClick = (medicineId) => {
    setSelectedMedicineId(medicineId);
    setDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!selectedMedicineId) return;
    setDeleting(true);

    try {
      await medicineApi.deleteMedicine(selectedMedicineId);
      fetchMedicines(); 
      setDeleteDialogOpen(false);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete medicine.');
    } finally {
      setDeleting(false);
      setSelectedMedicineId(null);
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4, borderRadius: 2 }}>
        {/* Header Section */}
        <Box 
          sx={{ 
            display: 'flex', 
            alignItems: 'center', 
            justifyContent: 'space-between',
            flexWrap: 'wrap',
            gap: 2,
            mb: 3 
          }}
        >
          <Box>
            <Typography variant="h5" component="h1" fontWeight="bold" color="primary">
              Medicine Inventory
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Manage all medicines, generic names, categorizations, and stock limits.
            </Typography>
          </Box>
          
          <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
            {/* Search Input */}
            <TextField
              size="small"
              placeholder="Search medicines..."
              value={search}
              onChange={handleSearchChange}
              slotProps={{
                input: {
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon />
                    </InputAdornment>
                  ),
                },
              }}
            />

            {canManage && (
              <Button
                variant="contained"
                color="primary"
                startIcon={<AddIcon />}
                onClick={() => navigate('/medicines/add')}
                sx={{ whiteSpace: 'nowrap' }}
              >
                Add Medicine
              </Button>
            )}
          </Box>
        </Box>

        {/* Error Notification */}
        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
            {error}
          </Alert>
        )}

        {/* Loading Spinner */}
        {loading && medicines.length === 0 ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', my: 6 }}>
            <CircularProgress />
          </Box>
        ) : (
          /* Medicines Table */
          <>
            <TableContainer>
              <Table sx={{ minWidth: 800 }} aria-label="medicine list table">
                <TableHead>
                  <TableRow>
                    <TableCell><strong>Code</strong></TableCell>
                    <TableCell><strong>Name</strong></TableCell>
                    <TableCell><strong>Generic Name</strong></TableCell>
                    <TableCell><strong>Category</strong></TableCell>
                    <TableCell><strong>Unit</strong></TableCell>
                    <TableCell align="center"><strong>Reorder Level</strong></TableCell>
                    
                    {canManage && (
                      <TableCell align="right"><strong>Actions</strong></TableCell>
                    )}
                  </TableRow>
                </TableHead>
                <TableBody>
                  {medicines.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={canManage ? 7 : 6} align="center" sx={{ py: 3 }}>
                        <Typography variant="body1" color="text.secondary">
                          No medicines found.
                        </Typography>
                      </TableCell>
                    </TableRow>
                  ) : (
                    medicines.map((medicine) => (
                      <TableRow key={medicine.medicineId} hover>
                        <TableCell>
                          <Chip label={medicine.medicineCode} size="small" variant="outlined" />
                        </TableCell>
                        <TableCell>{medicine.medicineName}</TableCell>
                        <TableCell>{medicine.genericName || 'N/A'}</TableCell>
                        <TableCell>{medicine.categoryName}</TableCell>
                        <TableCell>{medicine.unitOfMeasure}</TableCell>
                        <TableCell align="center">{medicine.reorderLevel}</TableCell>
                        
                        {canManage && (
                          <TableCell align="right">
                            <IconButton
                              color="primary"
                              aria-label="edit medicine"
                              onClick={() => navigate(`/medicines/edit/${medicine.medicineId}`)}
                            >
                              <EditIcon />
                            </IconButton>
                            <IconButton
                              color="error"
                              aria-label="delete medicine"
                              onClick={() => handleDeleteClick(medicine.medicineId)}
                            >
                              <DeleteIcon />
                            </IconButton>
                          </TableCell>
                        )}
                      </TableRow>
                    ))
                  )}
                </TableBody>
              </Table>
            </TableContainer>

            {/* Pagination Controls */}
            <TablePagination
              component="div"
              count={totalCount}
              page={page}
              onPageChange={handleChangePage}
              rowsPerPage={rowsPerPage}
              onRowsPerPageChange={handleChangeRowsPerPage}
              rowsPerPageOptions={[5, 10, 25, 50]}
            />
          </>
        )}
      </Paper>

      {/* Delete Confirmation Dialog */}
      {canManage && (
        <Dialog
          open={deleteDialogOpen}
          onClose={() => !deleting && setDeleteDialogOpen(false)}
        >
          <DialogTitle>Delete Medicine</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Are you sure you want to delete this medicine? This will soft-delete it from the database and may affect inventory tracking.
            </DialogContentText>
          </DialogContent>
          <DialogActions sx={{ p: 2 }}>
            <Button 
              onClick={() => setDeleteDialogOpen(false)} 
              disabled={deleting}
            >
              Cancel
            </Button>
            <Button
              onClick={handleConfirmDelete}
              color="error"
              variant="contained"
              disabled={deleting}
              startIcon={deleting ? <CircularProgress size={20} color="inherit" /> : null}
            >
              {deleting ? 'Deleting...' : 'Delete'}
            </Button>
          </DialogActions>
        </Dialog>
      )}
    </Container>
  );
};

export default Medicines;