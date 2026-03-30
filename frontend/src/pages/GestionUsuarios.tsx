import { useState, useEffect } from 'react';
import { adminService } from '../services/api';

function GestionUsuarios() {
  const [usuarios, setUsuarios] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchUsuarios();
  }, []);

  const fetchUsuarios = async () => {
    try {
      const data = await adminService.getUsuarios();
      setUsuarios(data);
    } catch (error) {
      console.error('Error fetching usuarios:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading"><div className="spinner"></div></div>;
  }

  return (
    <div className="container">
      <h1 style={{ marginBottom: '2rem' }}>Gestión de Usuarios</h1>

      <div className="card">
        {usuarios.length === 0 ? (
          <p style={{ color: 'var(--text-secondary)' }}>No hay usuarios registrados</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Correo</th>
                <th>Rol</th>
                <th>Jefe</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              {usuarios.map((u) => (
                <tr key={u.idUsuario}>
                  <td>{u.nombreCompleto}</td>
                  <td>{u.correoInstitucional}</td>
                  <td>{u.rol}</td>
                  <td>{u.nombreJefe || '-'}</td>
                  <td>
                    <span className={`badge ${u.estado ? 'badge-success' : 'badge-danger'}`}>
                      {u.estado ? 'Activo' : 'Inactivo'}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}

export default GestionUsuarios;
