import React from 'react';
import { Routes, Route, Navigate, Outlet } from 'react-router-dom';
import { Box } from '@mui/material';

import Login from '../pages/Auth/Login';
import ProtectedRoute from './ProtectedRoute';
import PublicRoute from './PublicRoute';
import Sidebar from '../components/Sidebar';

import AddUser from './../pages/Users/AddUser';
import Users from '../pages/Users/Users';
import Category from '../pages/Categories/Categories';
import AddCategory from '../pages/Categories/AddCategory';
import EditCategory from '../pages/Categories/EditCategory';
import Medicine from '../pages/Medicines/Medicines';
import AddMedicine from '../pages/Medicines/AddMedicine';
import EditMedicine from '../pages/Medicines/EditMedicine';
import Suppliers from '../pages/Suppliers/Suppliers';
import AddSupplier from '../pages/Suppliers/AddSuppliers';

const Dashboard = () => <div>Dashboard View</div>;

const MainLayout = () => {
  return (
    <Box sx={{ display: 'flex' }}>
      <Sidebar />
      <Box 
        component="main" 
        sx={{ 
          flexGrow: 1, 
          p: 3, 
          width: `calc(100% - 260px)`,
          minHeight: '100vh',
          backgroundColor: '#f4f6f8' 
        }}
      >
        <Outlet /> 
      </Box>
    </Box>
  );
};

const AppRoutes = () => {
  return (
    <Routes>
      <Route element={<PublicRoute />}>
        <Route path="/login" element={<Login />} />
      </Route>

      <Route element={<MainLayout />}>
        
        <Route element={<ProtectedRoute allowedRoles={['Admin', 'Store Manager', 'Pharmacist']} />}>
          <Route path="/medicines" element={<Medicine />} />
          <Route path="/" element={<Dashboard />} />
        </Route>

        <Route element={<ProtectedRoute allowedRoles={['Admin', 'Store Manager']} />}>
          <Route path="/categories" element={<Category />} />
          <Route path="/categories/add" element={<AddCategory />} />
          <Route path="/categories/edit/:id" element={<EditCategory />} />
          <Route path="/medicines/add" element={<AddMedicine />} />
          <Route path="/medicines/edit/:id" element={<EditMedicine />} />
          <Route path="/suppliers" element={<Suppliers />} />
          <Route path="/suppliers/add" element={<AddSupplier />} />
        </Route>

        <Route element={<ProtectedRoute allowedRoles={['Admin']} />}>
          <Route path="/users" element={<Users />} />
          <Route path="/users/add" element={<AddUser />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default AppRoutes;