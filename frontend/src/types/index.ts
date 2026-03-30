export interface Usuario {
  idUsuario: number;
  nombreCompleto: string;
  correoInstitucional: string;
  rol: string;
  idRol: number;
  azureOid?: string;
}

export interface TokenResponse {
  token: string;
  expiration: string;
  usuario: Usuario;
}

export interface Asistencia {
  idAsistencia: number;
  idUsuario: number;
  nombreUsuario: string;
  fechaHoraIngreso?: string;
  fechaHoraSalida?: string;
  modalidad: string;
  idUbicacion: number;
  ubicacion: string;
  estadoPuntualidad: string;
}

export interface MarcarRequest {
  latitud: number;
  longitud: number;
  idUbicacion: number;
  modalidad: string;
}

export interface MarcarSalidaRequest {
  latitud: number;
  longitud: number;
}

export interface MarcarResponse {
  success: boolean;
  message: string;
  asistencia?: Asistencia;
}

export interface Ausencia {
  idAusencia: number;
  idUsuario: number;
  nombreUsuario: string;
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
  fechaAprobacion?: string;
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

export interface ApiError {
  message: string;
}
