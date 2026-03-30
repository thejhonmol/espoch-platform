import { useState, useEffect } from 'react';
import { ausenciaService } from '../services/api';
import type { Ausencia, CrearAusenciaRequest } from '../types';

function MisAusencias() {
  const [ausencias, setAusencias] = useState<Ausencia[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState<CrearAusenciaRequest>({
    fechaAusencia: '',
    horarioInicio: '08:00',
    horarioFin: '12:00',
    motivo: '',
    tipoAusencia: 'Personal',
  });

  useEffect(() => {
    fetchAusencias();
  }, []);

  const fetchAusencias = async () => {
    try {
      const data = await ausenciaService.misSolicitudes();
      setAusencias(data);
    } catch (error) {
      console.error('Error fetching ausencias:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await ausenciaService.crear(formData);
      if (response.success) {
        setShowForm(false);
        setFormData({
          fechaAusencia: '',
          horarioInicio: '08:00',
          horarioFin: '12:00',
          motivo: '',
          tipoAusencia: 'Personal',
        });
        fetchAusencias();
      }
      alert(response.message);
    } catch (error: any) {
      alert(error.response?.data?.message || 'Error al crear ausencia');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-EC', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  if (loading && ausencias.length === 0) {
    return <div className="loading"><div className="spinner"></div></div>;
  }

  return (
    <div className="container">
      <div className="card-header">
        <h1>Mis Ausencias</h1>
        <button className="btn btn-primary" onClick={() => setShowForm(!showForm)}>
          {showForm ? 'Cancelar' : '+ Nueva Solicitud'}
        </button>
      </div>

      {showForm && (
        <div className="card" style={{ marginBottom: '1rem' }}>
          <h3 className="card-title">Nueva Solicitud de Ausencia</h3>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-cols-2">
              <div className="form-group">
                <label className="form-label">Fecha</label>
                <input
                  type="date"
                  className="form-input"
                  value={formData.fechaAusencia}
                  onChange={(e) => setFormData({ ...formData, fechaAusencia: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label className="form-label">Tipo</label>
                <select
                  className="form-input"
                  value={formData.tipoAusencia}
                  onChange={(e) => setFormData({ ...formData, tipoAusencia: e.target.value })}
                >
                  <option value="Personal">Personal</option>
                  <option value="Médica">Médica</option>
                  <option value="Familiar">Familiar</option>
                  <option value="Otro">Otro</option>
                </select>
              </div>
              <div className="form-group">
                <label className="form-label">Hora Inicio</label>
                <input
                  type="time"
                  className="form-input"
                  value={formData.horarioInicio}
                  onChange={(e) => setFormData({ ...formData, horarioInicio: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label className="form-label">Hora Fin</label>
                <input
                  type="time"
                  className="form-input"
                  value={formData.horarioFin}
                  onChange={(e) => setFormData({ ...formData, horarioFin: e.target.value })}
                  required
                />
              </div>
            </div>
            <div className="form-group">
              <label className="form-label">Motivo</label>
              <textarea
                className="form-input"
                rows={3}
                value={formData.motivo}
                onChange={(e) => setFormData({ ...formData, motivo: e.target.value })}
                required
              />
            </div>
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Guardando...' : 'Guardar Solicitud'}
            </button>
          </form>
        </div>
      )}

      <div className="card">
        {ausencias.length === 0 ? (
          <p style={{ color: 'var(--text-secondary)' }}>No hay solicitudes de ausencia</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Tipo</th>
                <th>Horario</th>
                <th>Horas</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              {ausencias.map((a) => (
                <tr key={a.idAusencia}>
                  <td>{formatDate(a.fechaAusencia)}</td>
                  <td>{a.tipoAusencia}</td>
                  <td>{a.horarioInicio} - {a.horarioFin}</td>
                  <td>{a.totalHoras}h</td>
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
    </div>
  );
}

export default MisAusencias;
