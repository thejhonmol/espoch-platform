import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import { useMsal } from '@azure/msal-react';

function Navbar() {
  const { instance } = useMsal();
  const { usuario, logout } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await instance.logoutPopup();
    logout();
    navigate('/login');
  };

  const isAdmin = usuario?.rol === 'Admin';
  const isJefe = usuario?.rol === 'JefeDirecto' || isAdmin;

  return (
    <>
      <nav className="navbar">
        <a href="/dashboard" className="navbar-brand">
          🎓 ESPOCH
        </a>
        
        <div className="navbar-menu">
          <NavLink to="/dashboard" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            Dashboard
          </NavLink>
          <NavLink to="/marcar" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            Marcar
          </NavLink>
          <NavLink to="/mis-ausencias" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            Mis Ausencias
          </NavLink>
          
          {isJefe && (
            <NavLink to="/revisar-ausencias" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
              Revisar
            </NavLink>
          )}
          
          {isAdmin && (
            <>
              <NavLink to="/gestion-usuarios" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
                Usuarios
              </NavLink>
              <NavLink to="/gestion-sedes" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
                Sedes
              </NavLink>
            </>
          )}
          
          <span style={{ marginLeft: '1rem', color: 'var(--text-secondary)' }}>
            {usuario?.nombreCompleto}
          </span>
          
          <button className="btn btn-outline" onClick={handleLogout}>
            Cerrar
          </button>
        </div>
      </nav>
      
      <main>
        <Outlet />
      </main>
    </>
  );
}

export default Navbar;
