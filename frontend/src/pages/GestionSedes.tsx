import { useState, useEffect } from 'react';
import { adminService } from '../services/api';
import type { Ubicacion } from '../types';

function GestionSedes() {
  const [sedes, setSedes] = useState<Ubicacion[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchSedes();
  }, []);

  const fetchSedes = async () => {
    try {
      const data = await adminService.getUbicaciones();
      setSedes(data);
    } catch (error) {
      console.error('Error fetching sedes:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading"><div className="spinner"></div></div>;
  }

  return (
    <div className="container">
      <h1 style={{ marginBottom: '2rem' }}>Gestión de Sedes</h1>

      <div className="card">
        {sedes.length === 0 ? (
          <p style={{ color: 'var(--text-secondary)' }}>No hay sedes registradas</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>Código</th>
                <th>Dirección</th>
                <th>Latitud</th>
                <th>Longitud</th>
                <th>Radio (m)</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              {sedes.map((s) => (
                <tr key={s.idUbicacion}>
                  <td>{s.codigoUbicacion}</td>
                  <td>{s.direccion}</td>
                  <td>{s.latitud.toFixed(6)}</td>
                  <td>{s.longitud.toFixed(6)}</td>
                  <td>{s.radioPermitidoSede}</td>
                  <td>
                    <span className={`badge ${s.estado ? 'badge-success' : 'badge-danger'}`}>
                      {s.estado ? 'Activa' : 'Inactiva'}
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

export default GestionSedes;
