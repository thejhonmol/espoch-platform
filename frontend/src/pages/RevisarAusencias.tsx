import { useState, useEffect } from 'react';
import { ausenciaService } from '../services/api';
import type { Ausencia } from '../types';

function RevisarAusencias() {
  const [ausencias, setAusencias] = useState<Ausencia[]>([]);
  const [loading, setLoading] = useState(true);
  const [rechazoMotivo, setRechazoMotivo] = useState<{ [key: number]: string }>({});

  useEffect(() => {
    fetchAusencias();
  }, []);

  const fetchAusencias = async () => {
    try {
      const data = await ausenciaService.pendientes();
      setAusencias(data);
    } catch (error) {
      console.error('Error fetching ausencias:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAprobar = async (id: number) => {
    try {
      await ausenciaService.aprobar(id);
      fetchAusencias();
      alert('Ausencia aprobada');
    } catch (error) {
      alert('Error al aprobar');
    }
  };

  const handleRechazar = async (id: number) => {
    const motivo = rechazoMotivo[id];
    if (!motivo) {
      alert('Ingrese el motivo de rechazo');
      return;
    }
    try {
      await ausenciaService.rechazar(id, motivo);
      fetchAusencias();
      alert('Ausencia rechazada');
    } catch (error) {
      alert('Error al rechazar');
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-EC', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  if (loading) {
    return <div className="loading"><div className="spinner"></div></div>;
  }

  return (
    <div className="container">
      <h1 style={{ marginBottom: '2rem' }}>Revisar Ausencias</h1>

      <div className="card">
        {ausencias.length === 0 ? (
          <p style={{ color: 'var(--text-secondary)' }}>No hay solicitudes pendientes</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>Colaborador</th>
                <th>Fecha</th>
                <th>Tipo</th>
                <th>Horario</th>
                <th>Horas</th>
                <th>Motivo</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {ausencias.map((a) => (
                <tr key={a.idAusencia}>
                  <td>{a.nombreUsuario}</td>
                  <td>{formatDate(a.fechaAusencia)}</td>
                  <td>{a.tipoAusencia}</td>
                  <td>{a.horarioInicio} - {a.horarioFin}</td>
                  <td>{a.totalHoras}h</td>
                  <td>{a.motivo}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.5rem' }}>
                      <button className="btn btn-success" onClick={() => handleAprobar(a.idAusencia)}>
                        ✓
                      </button>
                      <button className="btn btn-danger" onClick={() => handleRechazar(a.idAusencia)}>
                        ✗
                      </button>
                    </div>
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

export default RevisarAusencias;
