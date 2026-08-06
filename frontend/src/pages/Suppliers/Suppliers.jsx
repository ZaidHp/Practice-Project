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
  DialogTitle
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  Edit as EditIcon
} from '@mui/icons-material';
import { supplierApi } from '../../APIs/supplierApi';

const Suppliers = () => {
  const navigate = useNavigate();

  const [suppliers, setSuppliers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedSupplierId, setSelectedSupplierId] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const fetchSuppliers = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await supplierApi.getSupplires();

      setSuppliers(response.data || []);
    } catch (err) {
      setError(
        err.response?.data?.message ||
          'Failed to load suppliers from the server.'
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSuppliers();
  }, []);

  const handleDeleteClick = (supplierId) => {
    setSelectedSupplierId(supplierId);
    setDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!selectedSupplierId) return;

    setDeleting(true);
    setError(null);

    try {
      await supplierApi.deleteSupplier(selectedSupplierId);

      // Remove deleted supplier from table
      setSuppliers((prev) =>
        prev.filter(
          (supplier) => supplier.supplierId !== selectedSupplierId
        )
      );

      setDeleteDialogOpen(false);
    } catch (err) {
      setError(
        err.response?.data?.message ||
          'Failed to delete supplier.'
      );
    } finally {
      setDeleting(false);
      setSelectedSupplierId(null);
    }
  };

  const handleCloseDialog = () => {
    if (deleting) return;

    setDeleteDialogOpen(false);
    setSelectedSupplierId(null);
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Paper elevation={3} sx={{ p: 4, borderRadius: 2 }}>
        {/* Header */}
        <Box
          sx={{
            display: 'flex',
            alignItems: 'flex-start',
            mb: 3
          }}
        >
          <Box sx={{ flexGrow: 1 }}>
            <Typography
              variant="h5"
              component="h1"
              fontWeight="bold"
              color="primary"
            >
              Supplier Management
            </Typography>

            <Typography
              variant="body2"
              color="text.secondary"
            >
              View, add, edit, and delete suppliers.
            </Typography>
          </Box>

          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate('/suppliers/add')}
          >
            Add Supplier
          </Button>
        </Box>

        {/* Error */}
        {error && (
          <Alert
            severity="error"
            sx={{ mb: 3 }}
            onClose={() => setError(null)}
          >
            {error}
          </Alert>
        )}

        {/* Loading */}
        {loading ? (
          <Box
            sx={{
              display: 'flex',
              justifyContent: 'center',
              my: 6
            }}
          >
            <CircularProgress />
          </Box>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>
                    <strong>ID</strong>
                  </TableCell>

                  <TableCell>
                    <strong>Supplier Name</strong>
                  </TableCell>

                  <TableCell>
                    <strong>Phone</strong>
                  </TableCell>

                  <TableCell>
                    <strong>Email</strong>
                  </TableCell>

                  <TableCell>
                    <strong>Status</strong>
                  </TableCell>

                  <TableCell align="right">
                    <strong>Actions</strong>
                  </TableCell>
                </TableRow>
              </TableHead>

              <TableBody>
                {suppliers.length === 0 ? (
                  <TableRow>
                    <TableCell
                      colSpan={6}
                      align="center"
                      sx={{ py: 3 }}
                    >
                      <Typography
                        variant="body1"
                        color="text.secondary"
                      >
                        No suppliers found. Click{' '}
                        <strong>Add Supplier</strong> to create one.
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  suppliers.map((supplier) => (
                    <TableRow
                      key={supplier.supplierId}
                      hover
                    >
                      <TableCell>
                        {supplier.supplierId}
                      </TableCell>

                      <TableCell>
                        {supplier.supplierName}
                      </TableCell>

                      <TableCell>
                        {supplier.phone || 'N/A'}
                      </TableCell>

                      <TableCell>
                        {supplier.email || 'N/A'}
                      </TableCell>

                      <TableCell>
                        <Chip
                          label={
                            supplier.isActive
                              ? 'Active'
                              : 'Inactive'
                          }
                          size="small"
                          color={
                            supplier.isActive
                              ? 'success'
                              : 'default'
                          }
                        />
                      </TableCell>

                      <TableCell align="right">
                        <IconButton
                          color="primary"
                          onClick={() =>
                            navigate(
                              `/suppliers/edit/${supplier.supplierId}`
                            )
                          }
                        >
                          <EditIcon />
                        </IconButton>

                        <IconButton
                          color="error"
                          onClick={() =>
                            handleDeleteClick(
                              supplier.supplierId
                            )
                          }
                        >
                          <DeleteIcon />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>

      {/* Delete Dialog */}
      <Dialog
        open={deleteDialogOpen}
        onClose={handleCloseDialog}
      >
        <DialogTitle>
          Delete Supplier
        </DialogTitle>

        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete this supplier?
            This action cannot be undone.
          </DialogContentText>
        </DialogContent>

        <DialogActions sx={{ p: 2 }}>
          <Button
            onClick={handleCloseDialog}
            disabled={deleting}
          >
            Cancel
          </Button>

          <Button
            color="error"
            variant="contained"
            onClick={handleConfirmDelete}
            disabled={deleting}
            startIcon={
              deleting ? (
                <CircularProgress
                  size={18}
                  color="inherit"
                />
              ) : (
                <DeleteIcon />
              )
            }
          >
            {deleting ? 'Deleting...' : 'Delete'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default Suppliers;