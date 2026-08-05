import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Box,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Typography,
  Divider
} from '@mui/material';

import DashboardIcon from '@mui/icons-material/Dashboard';
import PeopleIcon from '@mui/icons-material/People';
import CategoryIcon from '@mui/icons-material/Category';
import MedicalServicesIcon from '@mui/icons-material/MedicalServices';
import InventoryIcon from '@mui/icons-material/Inventory';
import LocalShippingIcon from '@mui/icons-material/LocalShipping';
import LogoutIcon from '@mui/icons-material/Logout'; 
import { authService } from '../services/authService';

const drawerWidth = 260;

const Sidebar = () => {
  const location = useLocation();
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

  const navItems = [
    { 
      title: 'Dashboard', 
      path: '/dashboard', 
      icon: <DashboardIcon />,
      allowedRoles: ['Admin', 'Store Manager', 'Pharmacist'] 
    },
    { 
      title: 'Medicines', 
      path: '/medicines', 
      icon: <MedicalServicesIcon />,
      allowedRoles: ['Admin', 'Store Manager', 'Pharmacist'] 
    },
    { 
      title: 'Categories', 
      path: '/categories', 
      icon: <CategoryIcon />,
      allowedRoles: ['Admin', 'Store Manager'] 
    },
    // { 
    //   title: 'Batches & Stock', 
    //   path: '/batches', 
    //   icon: <InventoryIcon />,
    //   allowedRoles: ['Admin', 'Store Manager', 'Pharmacist'] 
    // },
    // { 
    //   title: 'Suppliers', 
    //   path: '/suppliers', 
    //   icon: <LocalShippingIcon />,
    //   allowedRoles: ['Admin', 'Store Manager'] 
    // },
    { 
      title: 'User Management', 
      path: '/users', 
      icon: <PeopleIcon />,
      allowedRoles: ['Admin'] 
    }
  ];

  const visibleNavItems = navItems.filter(item => 
    !item.allowedRoles || item.allowedRoles.includes(userRole)
  );
const handleLogout = () => {
    authService.logout(); 
  };

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { 
          width: drawerWidth, 
          boxSizing: 'border-box',
          backgroundColor: '#f8f9fa',
          borderRight: '1px solid #e0e0e0'
        },
      }}
    >
      <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
        
        <Box sx={{ p: 3, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Typography variant="h5" color="primary" fontWeight="bold">
            PharmaTrack
          </Typography>
        </Box>

        <Divider />

        <Box sx={{ overflow: 'auto', flexGrow: 1, mt: 2 }}>
          <List>
            {visibleNavItems.map((item) => {
              const isActive = location.pathname.startsWith(item.path);

              return (
                <ListItem key={item.title} disablePadding sx={{ mb: 0.5, px: 2 }}>
                  <ListItemButton
                    onClick={() => navigate(item.path)}
                    selected={isActive}
                    sx={{
                      borderRadius: 2,
                      '&.Mui-selected': {
                        backgroundColor: 'primary.main',
                        color: 'primary.contrastText',
                        '& .MuiListItemIcon-root': {
                          color: 'primary.contrastText',
                        },
                        '&:hover': {
                          backgroundColor: 'primary.dark',
                        }
                      },
                    }}
                  >
                    <ListItemIcon 
                      sx={{ 
                        minWidth: 40,
                        color: isActive ? 'inherit' : 'text.secondary'
                      }}
                    >
                      {item.icon}
                    </ListItemIcon>
                    <ListItemText 
                      primary={item.title} 
                      slotProps={{
                        primary: { fontWeight: isActive ? 'bold' : 'medium' }
                      }}
                    />
                  </ListItemButton>
                </ListItem>
              );
            })}
          </List>
        </Box>

        {/* --- Bottom Logout Button --- */}
        <Divider />
        <Box sx={{ p: 2 }}>
          <List disablePadding>
            <ListItem disablePadding>
              <ListItemButton
                onClick={handleLogout}
                sx={{
                  borderRadius: 2,
                  color: 'error.main', 
                  '&:hover': {
                    backgroundColor: 'error.light',
                    color: 'error.dark',
                    '& .MuiListItemIcon-root': {
                      color: 'error.dark',
                    }
                  }
                }}
              >
                <ListItemIcon sx={{ minWidth: 40, color: 'error.main' }}>
                  <LogoutIcon />
                </ListItemIcon>
                <ListItemText 
                  primary="Logout" 
                  slotProps={{ primary: { fontWeight: 'bold' } }} 
                />
              </ListItemButton>
            </ListItem>
          </List>
        </Box>
        
      </Box>
    </Drawer>
  );
};

export default Sidebar;