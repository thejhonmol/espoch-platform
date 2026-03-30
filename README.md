# ESPOCH - Plataforma de Experiencia del Colaborador

## 🚀 Despliegue en Railway

### Prerrequisitos

1. Cuenta en [Railway](https://railway.app) (registro con GitHub)
2. Git instalado
3. .NET 8 SDK
4. Node.js 18+

### Paso 1: Desplegar desde GitHub

1. Ve a [Railway](https://railway.app)
2. Click **New Project** → **Deploy from GitHub repo**
3. Selecciona el repositorio `espoch-platform`

### Paso 2: Crear Base de Datos

Railway no ofrece SQL Server managed. Usa **PostgreSQL** y cambia el provider de EF Core.

1. **New Service** → **Database** → **Add PostgreSQL**
2. Anota la variable `DATABASE_URL`

### Paso 3: Configurar Variables de Entorno

#### Backend
```
ConnectionStrings__DefaultConnection=<tu-postgres-url>
Jwt__Key=<genera-una-clave-32-chars>
Jwt__Issuer=ESPOCH-API
Jwt__Audience=ESPOCH-FrontEnd
FrontendUrl=<tu-url-de-railway>
```

#### Frontend
```
VITE_API_URL=https://tu-backend.up.railway.app/api
```

### Paso 4: Ejecutar Scripts SQL

1. Conéctate a PostgreSQL desde Railway
2. Ejecuta los scripts en orden:
   - `db/01_CreateDatabase.sql`
   - `db/02_CreateTables.sql`
   - `db/04_SeedData.sql`

---

## 🔐 Sistema de Autenticación

**Ya NO usa Azure AD**. Ahora usa autenticación simple con JWT:

- Login con **email + contraseña**
- Tokens JWT con expiración de 8 horas

### Credenciales de Prueba

| Rol | Email | Contraseña |
|-----|-------|------------|
| Admin | admin@espoch.edu.ec | admin123 |
| Jefe Directo | jefe.departamento@espoch.edu.ec | jefe123 |
| Colaborador | colaborador@espoch.edu.ec | colab123 |

---

## 📋 Endpoints API

### Autenticación

```bash
# Login
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@espoch.edu.ec","password":"admin123"}'

# Obtener usuario actual
curl -X GET "http://localhost:5000/api/auth/me" \
  -H "Authorization: Bearer TU_JWT_TOKEN"
```

### Marcar Asistencia

```bash
curl -X POST "http://localhost:5000/api/asistencia/marcar" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tipo": "Ingreso",
    "latitud": -1.286389,
    "longitud": -78.586666,
    "modalidad": "Presencial",
    "idUbicacion": 1
  }'
```

### Ausencias

```bash
# Crear ausencia
curl -X POST "http://localhost:5000/api/ausencias" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "fechaAusencia": "2026-04-01",
    "horarioInicio": "08:00",
    "horarioFin": "12:00",
    "motivo": "Cita médica",
    "tipoAusencia": "Médica"
  }'

# Aprobar ausencia (Jefe/Admin)
curl -X PUT "http://localhost:5000/api/ausencias/1/aprobar" \
  -H "Authorization: Bearer TU_JWT_TOKEN"

# Rechazar ausencia (Jefe/Admin)
curl -X PUT "http://localhost:5000/api/ausencias/1/rechazar" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"motivoRechazo": "Fechas no disponibles"}'
```

---

## 📁 Estructura del Proyecto

```
hackaton-espoch/
├── backend/                    # API ASP.NET Core 8
│   └── src/
│       ├── ESPOCH.API/         # Controllers, Program.cs
│       ├── ESPOCH.Core/        # Entities, DTOs, Services
│       └── ESPOCH.Infrastructure/  # EF Core, Repositories
├── frontend/                   # React 18+ + Vite
│   └── src/
│       ├── pages/              # Componentes de página
│       ├── services/           # API services
│       └── store/              # Zustand state
├── db/                         # Scripts SQL
├── railway/                    # Docker Compose
└── docs/                       # Documentación API
```

---

## 🏛️ Reglas de Negocio Implementadas

1. **Haversine (>100m)**: [`GeolocationService.cs`](backend/src/ESPOCH.Core/Services/GeolocationService.cs) - Rechaza marcación si distancia > 100m
2. **Límite 6h/día**: [`AusenciaService.cs`](backend/src/ESPOCH.Core/Services/AusenciaService.cs) - Máximo 6 horas de ausencia por día
3. **Roles**: Admin, JefeDirecto, Colaborador con políticas de autorización

---

## ⚙️ Configuración Local

### Backend
```bash
cd backend/src/ESPOCH.API
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```

---

## 📝 Notas

- La geolocalización requiere **HTTPS** o **localhost** para funcionar
- En producción, usar **BCrypt** para hashear contraseñas (actualmente en texto plano para demo)
- Railway no tiene SQL Server; usar PostgreSQL y cambiar provider de EF Core
