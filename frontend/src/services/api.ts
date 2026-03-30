import axios, { AxiosError } from 'axios';
import { useAuthStore } from '../store/authStore';
import type {
  TokenResponse,
  Usuario,
  Asistencia,
  Ausencia,
  MarcarRequest,
  MarcarSalidaRequest,
  MarcarResponse,
  CrearAusenciaRequest,
  AusenciaResponse,
  Ubicacion,
  Horario,
  LoginRequest,
} from '../types';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      useAuthStore.getState().logout();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  login: async (credentials: LoginRequest): Promise<TokenResponse> => {
    const response = await api.post<TokenResponse>('/auth/login', credentials);
    return response.data;
  },

  register: async (data: any): Promise<any> => {
    const response = await api.post('/auth/register', data);
    return response.data;
  },

  me: async (): Promise<Usuario> => {
    const response = await api.get<Usuario>('/auth/me');
    return response.data;
  },
};

export const asistenciaService = {
  marcar: async (data: MarcarRequest): Promise<MarcarResponse> => {
    const response = await api.post<MarcarResponse>('/asistencia/marcar', data);
    return response.data;
  },

  marcarSalida: async (id: number, data: MarcarSalidaRequest): Promise<MarcarResponse> => {
    const response = await api.post<MarcarResponse>(`/asistencia/marcar/${id}/salida`, data);
    return response.data;
  },

  historial: async (): Promise<Asistencia[]> => {
    const response = await api.get<Asistencia[]>('/asistencia/historial');
    return response.data;
  },

  getAll: async (): Promise<Asistencia[]> => {
    const response = await api.get<Asistencia[]>('/asistencia');
    return response.data;
  },
};

export const ausenciaService = {
  crear: async (data: CrearAusenciaRequest): Promise<AusenciaResponse> => {
    const response = await api.post<AusenciaResponse>('/ausencias', data);
    return response.data;
  },

  misSolicitudes: async (): Promise<Ausencia[]> => {
    const response = await api.get<Ausencia[]>('/ausencias/mis');
    return response.data;
  },

  pendientes: async (): Promise<Ausencia[]> => {
    const response = await api.get<Ausencia[]>('/ausencias/pendientes');
    return response.data;
  },

  aprobar: async (id: number): Promise<AusenciaResponse> => {
    const response = await api.put<AusenciaResponse>(`/ausencias/${id}/aprobar`);
    return response.data;
  },

  rechazar: async (id: number, motivo: string): Promise<AusenciaResponse> => {
    const response = await api.put<AusenciaResponse>(`/ausencias/${id}/rechazar`, { motivoRechazo: motivo });
    return response.data;
  },

  getAll: async (): Promise<Ausencia[]> => {
    const response = await api.get<Ausencia[]>('/ausencias');
    return response.data;
  },
};

export const adminService = {
  getUsuarios: async () => {
    const response = await api.get('/admin/usuarios');
    return response.data;
  },

  crearUsuario: async (data: any) => {
    const response = await api.post('/admin/usuarios', data);
    return response.data;
  },

  actualizarUsuario: async (id: number, data: any) => {
    const response = await api.put(`/admin/usuarios/${id}`, data);
    return response.data;
  },

  getUbicaciones: async (): Promise<Ubicacion[]> => {
    const response = await api.get<Ubicacion[]>('/admin/ubicaciones');
    return response.data;
  },

  crearUbicacion: async (data: Ubicacion) => {
    const response = await api.post('/admin/ubicaciones', data);
    return response.data;
  },

  actualizarUbicacion: async (id: number, data: Ubicacion) => {
    const response = await api.put(`/admin/ubicaciones/${id}`, data);
    return response.data;
  },

  eliminarUbicacion: async (id: number) => {
    const response = await api.delete(`/admin/ubicaciones/${id}`);
    return response.data;
  },

  getHorarios: async (): Promise<Horario[]> => {
    const response = await api.get<Horario[]>('/admin/horarios');
    return response.data;
  },

  crearHorario: async (data: Horario) => {
    const response = await api.post('/admin/horarios', data);
    return response.data;
  },

  actualizarHorario: async (id: number, data: Horario) => {
    const response = await api.put(`/admin/horarios/${id}`, data);
    return response.data;
  },

  eliminarHorario: async (id: number) => {
    const response = await api.delete(`/admin/horarios/${id}`);
    return response.data;
  },
};

export default api;
