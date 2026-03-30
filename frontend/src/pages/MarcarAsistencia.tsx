import { useState, useEffect } from 'react';
import { asistenciaService, adminService } from '../services/api';
import type { Ubicacion, MarcarRequest } from '../types';

function MarcarAsistencia() {
  const [ubicaciones, setUbicaciones] = useState<Ubicacion[]>([]);
  const [selectedUbicacion, setSelectedUbicacion] = useState<number>(0);
  const [modalidad, setModalidad] = useState<string>('Presencial');
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);
  const [currentPosition, setCurrentPosition] = useState<{ lat: number; lon: number } | null>(null);

  useEffect(() => {
    const fetchUbicaciones = async () => {
      try {
        const data = await adminService.getUbicaciones();
        setUbicaciones(data);
        if (data.length > 0) {
          setSelectedUbicacion(data[0].idUbicacion);
        }
      } catch (error) {
        console.error('Error fetching ubicaciones:', error);
      }
    };

    fetchUbicaciones();
    getCurrentPosition();
  }, []);

  const getCurrentPosition = () => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setCurrentPosition({
            lat: position.coords.latitude,
            lon: position.coords.longitude,
          });
        },
        (error) => {
          console.error('Error getting position:', error);
          setMessage({ type: 'error', text: 'No se pudo obtener la ubicación' });
        }
      );
    }
  };

  const handleMarcarEntrada = async () => {
    if (!currentPosition) {
      setMessage({ type: 'error', text: 'Primero active la ubicación' });
      return;
    }

    setLoading(true);
    try {
      const data: MarcarRequest = {
        tipo: 'Ingreso',
        latitud: currentPosition.lat,
        longitud: currentPosition.lon,
        idUbicacion: selectedUbicacion,
        modalidad,
      };
      const response = await asistenciaService.marcar(data);
      setMessage({ type: 'success', text: response.message });
    } catch (error as any) {
      setMessage({ type: 'error', text: error.response?.data?.message || 'Error al marcar' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container">
      <h1 style={{ marginBottom: '2rem' }}>Marcar Asistencia</h1>

      {message && (
        <div className={`alert alert-${message.type}`}>
          {message.text}
        </div>
      )}

      <div className="card">
        <h3 className="card-title">Registrar Entrada</h3>
        
        <div className="form-group">
          <label className="form-label">Sede</label>
          <select
            className="form-input"
            value={selectedUbicacion}
            onChange={(e) => setSelectedUbicacion(Number(e.target.value))}
          >
            {ubicaciones.map((u) => (
              <option key={u.idUbicacion} value={u.idUbicacion}>
                {u.direccion}
              </option>
            ))}
          </select>
        </div>

        <div className="form-group">
          <label className="form-label">Modalidad</label>
          <select
            className="form-input"
            value={modalidad}
            onChange={(e) => setModalidad(e.target.value)}
          >
            <option value="Presencial">Presencial</option>
            <option value="Remoto">Remoto</option>
            <option value="Híbrido">Híbrido</option>
          </select>
        </div>

        <div className="form-group">
          <label className="form-label">Tu Ubicación</label>
          {currentPosition ? (
            <div className="alert alert-success">
              📍 Lat: {currentPosition.lat.toFixed(6)}, Lon: {currentPosition.lon.toFixed(6)}
            </div>
          ) : (
            <div className="alert alert-warning">
              Obteniendo ubicación...
            </div>
          )}
        </div>

        <button
          className="btn btn-primary"
          onClick={handleMarcarEntrada}
          disabled={loading || !currentPosition}
          style={{ width: '100%' }}
        >
          {loading ? 'Registrando...' : 'Registrar Entrada'}
        </button>
      </div>
    </div>
  );
}

export default MarcarAsistencia;
