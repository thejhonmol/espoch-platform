# ESPOCH - Plataforma de Experiencia del Colaborador

## 🚀 Despliegue en Railway

Railway es una plataforma de hosting que permite desplegar contenedores Docker fácilmente. Sigue estos pasos:

### Prerrequisitos

1. Cuenta en [Railway](https://railway.app) (puedes registrarte con GitHub)
2. Git instalado
3. .NET 8 SDK
4. Node.js 18+
5. Azure AD Tenant y App Registration

### Paso 1: Registrar Aplicación en Azure AD

1. Ve a [Azure Portal](https://portal.azure.com) → **Microsoft Entra ID** → **App registrations**
2. Click en **New registration**
   - Name: `ESPOCH Platform`
   - Supported account types: `Accounts in this organizational directory only`
   - Redirect URI: `Web` → `https://tu-proyecto.up.railway.app/auth/callback`
3. Anota los valores:
   - **Application (client) ID**
   - **Directory (tenant) ID**

4. En **Certificates & secrets** → **New client secret** → Anota el valor

5. En **API permissions** → **Microsoft Graph** → añade:
   - `openid`
   - `profile`
   - `email`

### Paso 2: Subir Proyecto a GitHub

```bash
cd hackaton-espoch
git init
git add .
git commit -m "Initial commit - ESPOCH Platform"
git remote add origin https://github.com/TU-USUARIO/espoch-platform.git
git push -u origin main
```

### Paso 3: Crear Proyecto en Railway

1. Entra a [Railway](https://railway.app)
2. Click **New Project** → **Deploy from GitHub repo**
3. Selecciona tu repositorio `espoch-platform`

### Paso 4: Configurar Servicios (3 servicios separados)

Railway desplegará automáticamente lo que detecte. Necesitas **crear 3 servicios manualmente**:

#### 4.1 Servicio SQL Server

1. **New Service** → **Database** → **Add PostgreSQL** (Railway recomienda PostgreSQL)
2. **⚠️ IMPORTANTE**: Railway NO ofrece SQL Server managed. Tienes 2 opciones:

**Opción A: Usar PostgreSQL (RECOMENDADO)**
- Cambia el connection string a formato PostgreSQL
- El código Entity Framework funciona igual, solo cambia el provider

**Opción B: SQL Server en Nix Hosted Service**
- Railway permite servicios custom con Docker
- Usa la imagen `mcr.microsoft.com/mssql/server:2022-latest`

Para este ejemplo usamos **PostgreSQL** que es más fácil:

1. **New Service** → **Database** → **Add PostgreSQL**
2. Anota la variable `DATABASE_URL` (formato: `postgresql://user:pass@host:5432/db`)

#### 4.2 Servicio Backend (.NET)

1. **New Service** → **Deploy from Source** → selecciona el repo
2. Configure:
   - **Root Directory**: `backend`
   - **Build Command**: `dotnet publish -c Release -o ./publish`
   - **Start Command**: `cd publish && dotnet ESPOCH.API.dll`

3. **Variables de Entorno** (en Settings → Variables):
   ```
   ConnectionStrings__DefaultConnection=<tu-DATABASE_URL>
   AzureAd__ClientId=<tu-client-id>
   AzureAd__TenantId=<tu-tenant-id>
   AzureAd__Audience=api://<tu-client-id>
   Jwt__Key=<tu-jwt-secret-minimo-32-caracteres>
   Jwt__Issuer=ESPOCH-API
   Jwt__Audience=ESPOCH-FrontEnd
   FrontendUrl=https://tu-proyecto.up.railway.app
   ```

4. **Generar JWT Key**:
   ```bash
   openssl rand -base64 32
   ```

#### 4.3 Servicio Frontend (React)

1. **New Service** → **Deploy from Source** → selecciona el repo
2. Configure:
   - **Root Directory**: `frontend`
   - **Build Command**: `npm install && npm run build`
   - **Start Command**: `npx serve -s dist -l 3000`

3. **Variables de Entorno**:
   ```
   VITE_API_URL=https://tu-backend.up.railway.app/api
   VITE_AZURE_CLIENT_ID=<tu-client-id>
   VITE_AZURE_TENANT_ID=<tu-tenant-id>
   VITE_REDIRECT_URI=https://tu-proyecto.up.railway.app/auth/callback
   ```

### Paso 5: Configurar Redes entre Servicios

En Railway, los servicios se comunican usando variables de entorno:

1. Ve al servicio **Backend** → **Settings** → **Networking**
2. Crea un **Public Domain** (esto expone el backend)
3. Copia la URL (ej: `https://espoch-backend.up.railway.app`)

4. En el servicio **Frontend**, actualiza:
   ```
   VITE_API_URL=https://espoch-backend.up.railway.app/api
   ```

### Paso 6: Actualizar Azure AD Redirect URIs

1. Ve a Azure Portal → **App registrations** → tu app
2. **Authentication** → **Add URI**
3. Añade: `https://tu-proyecto.up.railway.app/auth/callback`

### Paso 7: Inicializar Base de Datos

1. Conéctate a tu PostgreSQL (puedes usar Railway's Data explorer o TablePlus)
2. Ejecuta los scripts SQL en orden:
   - `db/01_CreateDatabase.sql`
   - `db/02_CreateTables.sql` (cambia `IDENTITY` a serial para PostgreSQL si es necesario)
   - `db/04_SeedData.sql`

### Paso 8: Verificar Despliegue

1. Abre `https://tu-proyecto.up.railway.app`
2. Deberías ver la página de login
3. Click en "Sign in with Microsoft"
4. Serás redirigido a Azure AD para autenticarte
5. Después del login, verás el dashboard según tu rol

---

## 📋 Despliegue Local (Alternativo)

### Con Docker Compose

```bash
cd hackaton-espoch/railway
docker-compose up -d
```

Esto levantará:
- SQL Server en `localhost:1433`
- Backend en `localhost:5000`
- Frontend en `localhost:3000`

### Sin Docker

**Backend:**
```bash
cd backend/src/ESPOCH.API
dotnet run
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```

---

## 🔧 Configuración de Variables

### Backend (.NET)

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Connection string BD | `Server=localhost;Database=ESPOCH...` |
| `AzureAd__ClientId` | Client ID de Azure AD | `abc123...` |
| `AzureAd__TenantId` | Tenant ID de Azure AD | `xyz789...` |
| `AzureAd__Audience` | Audience API | `api://abc123...` |
| `Jwt__Key` | Clave secreta JWT (32+ chars) | `mi-super-secreto...` |
| `Jwt__Issuer` | Emisor JWT | `ESPOCH-API` |
| `Jwt__Audience` | Audiencia JWT | `ESPOCH-FrontEnd` |
| `FrontendUrl` | URL del frontend | `http://localhost:5173` |

### Frontend (React)

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `VITE_API_URL` | URL del API backend | `https://api.up.railway.app/api` |
| `VITE_AZURE_CLIENT_ID` | Client ID Azure AD | `abc123...` |
| `VITE_AZURE_TENANT_ID` | Tenant ID Azure AD | `xyz789...` |
| `VITE_REDIRECT_URI` | URI de callback | `https://app.up.railway.app/auth/callback` |

---

## 🧪 Probar Endpoints con cURL

### Login/Obtener Token

```bash
curl -X POST "https://tu-api.railway.app/api/auth/token" \
  -H "Content-Type: application/json" \
  -d '{"code": "código-desde-azure-ad"}'
```

### Marcar Asistencia (con JWT)

```bash
curl -X POST "https://tu-api.railway.app/api/asistencia/marcar" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tipo": "Ingreso",
    "latitud": -1.286389,
    "longitud": -78.586666,
    "modalidad": "Presencial"
  }'
```

### Crear Ausencia

```bash
curl -X POST "https://tu-api.railway.app/api/ausencias" \
  -H "Authorization: Bearer TU_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "fechaAusencia": "2026-04-01",
    "horarioInicio": "08:00",
    "horarioFin": "12:00",
    "motivo": "Cita médica",
    "tipoAusencia": "Permiso"
  }'
```

### Aprobar Ausencia (Jefe/Admin)

```bash
curl -X PUT "https://tu-api.railway.app/api/ausencias/1/aprobar" \
  -H "Authorization: Bearer TU_JWT_TOKEN"
```

### Rechazar Ausencia (Jefe/Admin)

```bash
curl -X PUT "https://tu-api.railway.app/api/ausencias/1/rechazar" \
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
│       ├── services/           # API y MSAL
│       └── store/              # Zustand state
├── db/                         # Scripts SQL
├── railway/                    # Docker Compose
└── docs/                       # Documentación API
```

---

## 🔐 Roles y Permisos

| Rol | Dashboard | Marcar | Mis Ausencias | Solicitar | Revisar | Admin |
|-----|-----------|--------|---------------|-----------|---------|-------|
| Admin | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Colaborador | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| Jefe Directo | ✅ | ✅ | ✅ | ✅ | ✅* | ❌ |

*Solo colaboradores que le reportan

---

## ⚠️ Notas Importantes

1. **SQL Server en Railway**: Railway no ofrece SQL Server managed. Considera usar PostgreSQL y cambiar el provider de EF Core de `Microsoft.EntityFrameworkCore.SqlServer` a `Npgsql.EntityFrameworkCore.PostgreSQL`.

2. **Variables de Entorno**: Nunca expongas secrets en el código. Usa las variables de entorno de Railway.

3. **CORS**: El backend está configurado para permitir el frontend. Asegúrate de que `FrontendUrl` esté correctamente configurado.

4. **Geolocalización**: El frontend debe estar served sobre HTTPS para que la API de geolocalización funcione correctamente.
