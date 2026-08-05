import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

const ProtectedRoute = ({ allowedRoles }) => {
  const token = localStorage.getItem('token');

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  try {
    const decodedToken = jwtDecode(token);
    const currentTime = Date.now() / 1000;

    if (decodedToken.exp < currentTime) {
      localStorage.removeItem('token');
      return <Navigate to="/login" replace />;
    }

    if (allowedRoles && allowedRoles.length > 0) {

      const userRole = 
        decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 
        decodedToken.role;

      if (!allowedRoles.includes(userRole)) {
        return <Navigate to="/" replace />;
      }
    }

    return <Outlet />;
    
  } catch (error) {
    localStorage.removeItem('token');
    return <Navigate to="/login" replace />;
  }
};

export default ProtectedRoute;