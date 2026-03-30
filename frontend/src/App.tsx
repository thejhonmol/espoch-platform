import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuthStore } from './store/authStore';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import MarcarAsistencia from './pages/MarcarAsistencia';
import MisAusencias from './pages/MisAusencias';
import RevisarAusencias from './pages/RevisarAusencias';
import GestionUsuarios from './pages/GestionUsuarios';
import GestionSedes from './pages/GestionSedes';
import Navbar from './components/Navbar';

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  return <>{children}</>;
}

function App() {
  return (
    <div className="app">
      <Routes>
        <Route path="/login" element={<Login />} />
        
        <Route
          path="/"
          element={
            <PrivateRoute>
              <Navbar />
            </>
          }
        >
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="marcar" element={<MarcarAsistencia />} />
          <Route path="mis-ausencias" element={<MisAusencias />} />
          <Route path="revisar-ausencias" element={<RevisarAusencias />} />
          <Route path="gestion-usuarios" element={<GestionUsuarios />} />
          <Route path="gestion-sedes" element={<GestionSedes />} />
        </Route>
      </Routes>
    </div>
  );
}

export default App;
