import { useEffect, useState } from 'react';
import { useAuthStore } from '../store/authStore';
import { asistenciaService, ausenciaService } from '../services/api';
import type { Asistencia, Ausencia } from '../types';

function Dashboard() {
  const usuario = useAuthStore((state) => state.usuario);
  const [asistencias, setAsistencias] = useState<Asistencia[]>([]);
  const [ausencias, setAusencias] = useState<Ausencia[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [asistenciasData, ausenciasData] = await Promise.all([
          asistenciaService.historial(),
          ausenciaService.misSolicitudes(),
        ]);
        setAsistencias(asistenciasData.slice(0, 5));
        setAusencias(ausenciasData.slice(0, 5));
      } catch (error) {
        console.error('Error fetching data:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString('es-EC', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  const formatTime = (dateString?: string) => {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleTimeString('es-EC', {
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (loading) {
    return (
      <div className="loading">
        <div className="spinner"></div>
      </div>
    );
  }

  return (
    <div className="container">
      <h1 style={{ marginBottom: '2rem' }}>
        Bienvenido, {usuario?.nombreCompleto}
        <span className="badge badge-info" style={{ marginLeft: '0.5rem' }}>
          {usuario?.rol}
        </span>
      </h1>

      <div className="grid grid-cols-3">
        <div className="card">
          <h3 className="card-title">Asistencias Recientes</h3>
          {asistencias.length === 0 ? (
            <p style={{ color: 'var(--text-secondary)' }}>No hay registros</p>
          ) : (
            <table className="table">
              <thead>
                <tr>
                  <th>Fecha</th>
                  <th>Entrada</th>
                  <th>Estado</th>
                </tr>
              </thead>
              <tbody>
                {asistencias.map((a) => (
                  <tr key={a.idAsistencia}>
                    <td>{formatDate(a.fechaHoraIngreso)}</td>
                    <td>{formatTime(a.fechaHoraIngreso)}</td>
                    <td>
                      <span className={`badge ${
                        a.estadoPuntualidad === 'Presente' ? 'badge-success' : 'badge-warning'
                      }`}>
                        {a.estadoPuntualidad}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        <div className="card">
          <h3 className="card-title">Mis Ausencias</h3>
          {ausencias.length === 0 ? (
            <p style={{ color: 'var(--text-secondary)' }}>No hay solicitudes</p>
          ) : (
            <table className="table">
              <thead>
                <tr>
                  <th>Fecha</th>
                  <th>Tipo</th>
                  <th>Estado</th>
                </tr>
              </thead>
              <tbody>
                {ausencias.map((a) => (
                  <tr key={a.idAusencia}>
                    <td>{formatDate(a.fechaAusencia)}</td>
                    <td>{a.tipoAusencia}</td>
                    <td>
                      <span className={`badge ${
                        a.estadoAprobacion === 'Aprobada' ? 'badge-success' :
                        a.estadoAprobacion === 'Rechazada' ? 'badge-danger' : 'badge-warning'
                      }`}>
                        {a.estadoAprobacion}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        <div className="card">
          <h3 className="card-title">Acciones Rápidas</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
            <a href="/marcar" className="btn btn-primary">
              📍 Marcar Asistencia
            </a>
            <a href="/mis-ausencias" className="btn btn-outline">
              📋 Mis Ausencias
            </a>
            {(usuario?.rol === 'Admin' || usuario?.rol === 'JefeDirecto') && (
              <a href="/revisar-ausencias" className="btn btn-outline">
                ✅ Revisar Ausencias
              </a>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
