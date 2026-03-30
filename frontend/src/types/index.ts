export interface LoginRequest {
  email: string;
  password: string;
}

export interface TokenResponse {
  token: string;
  expiration: string;
  usuario: Usuario;
}

export interface Usuario {
  idUsuario: number;
  nombreCompleto: string;
  correoInstitucional: string;
  rol: string;
  idRol: number;
}

export interface Asistencia {
  idAsistencia: number;
  idUsuario: number;
  fechaHoraIngreso: string;
  fechaHoraSalida?: string;
  modalidad: string;
  idUbicacion: number;
  latIngreso: number;
  lonIngreso: number;
  estadoPuntualidad: string;
  nombreUsuario?: string;
  nombreUbicacion?: string;
}

export interface MarcarRequest {
  tipo: 'Ingreso' | 'Salida';
  latitud: number;
  longitud: number;
  modalidad: string;
  idUbicacion?: number;
}

export interface MarcarSalidaRequest {
  latitud: number;
  longitud: number;
}

export interface MarcarResponse {
  success: boolean;
  message: string;
  asistencia?: Asistencia;
  distancia?: number;
}

export interface Ausencia {
  idAusencia: number;
  idUsuario: number;
  fechaSolicitud: string;
  fechaAusencia: string;
  horarioInicio: string;
  horarioFin: string;
  totalHoras: number;
  motivo: string;
  tipoAusencia: string;
  estadoAprobacion: string;
  idAprobador?: number;
  nombreAprobador?: string;
  motivoRechazo?: string;
  nombreUsuario?: string;
}

export interface CrearAusenciaRequest {
  fechaAusencia: string;
  horarioInicio: string;
  horarioFin: string;
  motivo: string;
  tipoAusencia: string;
}

export interface AusenciaResponse {
  success: boolean;
  message: string;
  ausencia?: Ausencia;
  horasUsadas?: number;
  horasDisponibles?: number;
}

export interface Ubicacion {
  idUbicacion: number;
  codigoUbicacion: string;
  direccion: string;
  latitud: number;
  longitud: number;
  radioPermitidoSede: number;
  estado: boolean;
}

export interface Horario {
  idHorario: number;
  nombreHorario: string;
  horarioInicio: string;
  horarioFin: string;
  estado: boolean;
}
