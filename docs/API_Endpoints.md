# API Endpoints - ESPOCH

## Autenticación

### POST /api/auth/token
Intercambia código de Azure AD por JWT

**Request Body:**
```json
{
  "code": "string",
  "redirectUri": "string"
}
```

**Response:**
```json
{
  "token": "string",
  "expiration": "datetime",
  "usuario": {
    "idUsuario": 1,
    "nombreCompleto": "string",
    "correoInstitucional": "string",
    "rol": "string",
    "idRol": 1
  }
}
```

### GET /api/auth/me
Obtiene el usuario actual autenticado

**Headers:** `Authorization: Bearer {token}`

**Response:**
```json
{
  "idUsuario": 1,
  "nombreCompleto": "string",
  "correoInstitucional": "string",
  "rol": "string",
  "idRol": 1
}
```

---

## Asistencia

### POST /api/asistencia/marcar
Registra entrada de asistencia

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "latitud": -0.180653,
  "longitud": -78.467834,
  "idUbicacion": 1,
  "modalidad": "Presencial"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Entrada registrada correctamente",
  "asistencia": {
    "idAsistencia": 1,
    "idUsuario": 1,
    "nombreUsuario": "string",
    "fechaHoraIngreso": "datetime",
    "modalidad": "Presencial",
    "ubicacion": "string",
    "estadoPuntualidad": "Presente"
  }
}
```

### POST /api/asistencia/marcar/{id}/salida
Registra salida de asistencia

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "latitud": -0.180653,
  "longitud": -78.467834
}
```

### GET /api/asistencia/historial
Obtiene historial de asistencia del usuario

**Headers:** `Authorization: Bearer {token}`

### GET /api/asistencia
Obtiene todas las asistencia (solo Admin)

**Headers:** `Authorization: Bearer {token}`

---

## Ausencias

### POST /api/ausencias
Crea nueva solicitud de ausencia

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "fechaAusencia": "2026-04-01",
  "horarioInicio": "08:00",
  "horarioFin": "12:00",
  "motivo": "Cita médica",
  "tipoAusencia": "Médica"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Solicitud creada correctamente",
  "ausencia": {
    "idAusencia": 1,
    "idUsuario": 1,
    "nombreUsuario": "string",
    "fechaSolicitud": "datetime",
    "fechaAusencia": "date",
    "horarioInicio": "time",
    "horarioFin": "time",
    "totalHoras": 4.0,
    "motivo": "string",
    "tipoAusencia": "Médica",
    "estadoAprobacion": "Pendiente"
  }
}
```

### GET /api/ausencias/mis
Obtiene solicitudes del usuario actual

**Headers:** `Authorization: Bearer {token}`

### GET /api/ausencias/pendientes
Obtiene solicitudes pendientes de aprobación (Jefe/Admin)

**Headers:** `Authorization: Bearer {token}`

### PUT /api/ausencias/{id}/aprobar
Aprueba solicitud de ausencia

**Headers:** `Authorization: Bearer {token}`

### PUT /api/ausencias/{id}/rechazar
Rechaza solicitud de ausencia

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "motivoRechazo": "No hay justificación suficiente"
}
```

### GET /api/ausencias
Obtiene todas las ausencias (solo Admin)

**Headers:** `Authorization: Bearer {token}`

---

## Administración (solo Admin)

### GET /api/admin/usuarios
Lista todos los usuarios

### POST /api/admin/usuarios
Crea nuevo usuario

### PUT /api/admin/usuarios/{id}
Actualiza usuario

### GET /api/admin/ubicaciones
Lista ubicaciones/sedes

### POST /api/admin/ubicaciones
Crea ubicación

### PUT /api/admin/ubicaciones/{id}
Actualiza ubicación

### DELETE /api/admin/ubicaciones/{id}
Elimina ubicación

### GET /api/admin/horarios
Lista horarios

### POST /api/admin/horarios
Crea horario

### PUT /api/admin/horarios/{id}
Actualiza horario

### DELETE /api/admin/horarios/{id}
Elimina horario

---

## Códigos de Error

| Código | Descripción |
|--------|-------------|
| 400 | Bad Request - Datos inválidos |
| 401 | Unauthorized - No autenticado |
| 403 | Forbidden - Sin permisos |
| 404 | Not Found - Recurso no encontrado |
| 500 | Internal Server Error - Error del servidor |

---

## Reglas de Negocio

### Geolocalización
- Radio máximo permitido: 100 metros
- Fórmula: Haversine
- Si distancia > 100m → Rechazo

### Límite de Ausencias
- Máximo: 6 horas por día
- Incluye horas ya aprobadas + nueva solicitud
- Si excede → Rechazo con mensaje
