# ESPOCH - Plataforma de Experiencia del Colaborador

## 🚀 Despliegue con Supabase (PostgreSQL)

### Prerrequisitos

1. Cuenta en [Railway](https://railway.app) o [Supabase](https://supabase.com)
2. Cuenta en [GitHub](https://github.com/thejhonmol/espoch-platform)
3. .NET 8 SDK
4. Node.js 18+

---

## 📦 Paso 1: Crear Base de Datos en Supabase

1. Ve a [supabase.com](https://supabase.com) y crea una cuenta
2. Crea un nuevo proyecto:
   - Name: `ESPOCH-DB`
   - Password: Guarda esta contraseña
   - Region: Selecciona la más cercana
3. En el dashboard del proyecto, ve a **Settings** → **API**
4. Copia la **Connection string** (格式: `postgresql://postgres:[PASSWORD]@db.[REF].supabase.co:5432/postgres`)

---

## 📁 Paso 2: Ejecutar Scripts SQL en Supabase

1. En Supabase, ve a **SQL Editor**
2. Ejecuta los scripts en orden:
   - `db/02_CreateTables.sql` 
   - `db/04_SeedData.sql`

---

## 🚂 Paso 3: Desplegar Backend en Railway

1. Ve a [railway.app](https://railway.app)
2. Crea un nuevo proyecto: **New Project** → **Deploy from GitHub**
3. Selecciona el repositorio `espoch-platform`
4. Configura el **Root Directory** como `backend`

### Variables de Entorno en Railway:
```
ConnectionStrings__DefaultConnection=postgresql://postgres:[TU_PASSWORD]@db.[TU_REF].supabase.co:5432/postgres
Jwt__Key=ESPOCH-Super-Secret-Key-That-Is-At-Least-32-Characters-Long
Jwt__Issuer=ESPOCH-API
Jwt__Audience=ESPOCH-FrontEnd
FrontendUrl=https://tu-frontend.railway.app
```

---

## 🌐 Paso 4: Desplegar Frontend en Railway

1. Crea otro proyecto: **New Project** → **Deploy from GitHub**
2. Selecciona el repositorio `espoch-platform`
3. Configura el **Root Directory** como `frontend`

### Variables de Entorno:
```
VITE_API_URL=https://tu-backend.railway.app/api
```

---

## 🔐 Sistema de Autenticación

**Login simple con email + contraseña (JWT)**

### Credenciales de Prueba (db/04_SeedData.sql):

| Rol | Email | Contraseña |
|-----|-------|------------|
| Admin | admin@espoch.edu.ec | admin123 |
| Jefe Directo | jefe.departamento@espoch.edu.ec | jefe123 |
| Colaborador | colaborador@espoch.edu.ec | colab123 |

---

## 📡 Endpoints API

### Login
```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@espoch.edu.ec","password":"admin123"}'
```

### Marcar Asistencia
```bash
curl -X POST "http://localhost:5000/api/asistencia/marcar" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tipo": "Ingreso",
    "latitud": -0.180653,
    "longitud": -78.467834,
    "modalidad": "Presencial",
    "idUbicacion": 1
  }'
```

### Crear Ausencia
```bash
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
```

### Aprobar/Rechazar Ausencia
```bash
# Aprobar
curl -X PUT "http://localhost:5000/api/ausencias/1/aprobar" \
  -H "Authorization: Bearer TU_JWT_TOKEN"

# Rechazar
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
│       └── ESPOCH.Infrastructure/  # EF Core (PostgreSQL), Repositories
├── frontend/                   # React 18+ + Vite
│   └── src/
│       ├── pages/              # Login, Dashboard, Ausencias, etc.
│       ├── services/           # API services
│       └── store/              # Zustand state
├── db/                         # Scripts PostgreSQL
└── railway/                    # Docker Compose (opcional)
```

---

## 🏛️ Reglas de Negocio

1. **Haversine (>100m)**: Rechaza marcación si distancia > 100 metros
2. **Límite 6h/día**: Máximo 6 horas de ausencia por día por colaborador
3. **Roles**: Admin (todo), JefeDirecto (sus colaboradores), Colaborador (básico)

---

## ⚙️ Desarrollo Local

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

## ⚠️ Notas Importantes

1. **Geolocalización**: Requiere HTTPS o localhost para funcionar
2. **Contraseñas**: Actualmente en texto plano (para producción usar BCrypt)
3. **Supabase Connection String**: Formato correcto:
   ```
   postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres
   ```
4. **Railway**: Backend y Frontend se despliegan como proyectos SEPARADOS
