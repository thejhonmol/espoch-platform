-- 02_CreateTables.sql
-- Script para crear tablas en PostgreSQL

-- Tabla Roles
CREATE TABLE IF NOT EXISTS "Roles" (
    "IdRol" SERIAL PRIMARY KEY,
    "NombreRol" VARCHAR(50) NOT NULL,
    "Descripcion" VARCHAR(200),
    "Estado" BOOLEAN NOT NULL DEFAULT TRUE
);

-- Tabla Horarios
CREATE TABLE IF NOT EXISTS "Horarios" (
    "IdHorario" SERIAL PRIMARY KEY,
    "NombreHorario" VARCHAR(50) NOT NULL,
    "HorarioInicio" TIME NOT NULL,
    "HorarioFin" TIME NOT NULL,
    "Estado" BOOLEAN NOT NULL DEFAULT TRUE
);

-- Tabla Ubicaciones
CREATE TABLE IF NOT EXISTS "Ubicaciones" (
    "IdUbicacion" SERIAL PRIMARY KEY,
    "CodigoUbicacion" VARCHAR(20) NOT NULL,
    "Direccion" VARCHAR(200) NOT NULL,
    "Latitud" DECIMAL(10, 8) NOT NULL,
    "Longitud" DECIMAL(11, 8) NOT NULL,
    "RadioPermitidoSede" INTEGER NOT NULL DEFAULT 100,
    "Estado" BOOLEAN NOT NULL DEFAULT TRUE
);

-- Tabla Usuarios
CREATE TABLE IF NOT EXISTS "Usuarios" (
    "IdUsuario" SERIAL PRIMARY KEY,
    "NombreCompleto" VARCHAR(200) NOT NULL,
    "CorreoInstitucional" VARCHAR(200) NOT NULL UNIQUE,
    "ContrasenaHash" VARCHAR(500),
    "IdRol" INTEGER NOT NULL REFERENCES "Roles"("IdRol"),
    "IdJefeDirecto" INTEGER REFERENCES "Usuarios"("IdUsuario"),
    "IdHorario" INTEGER REFERENCES "Horarios"("IdHorario"),
    "Estado" BOOLEAN NOT NULL DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS "IX_Usuarios_CorreoInstitucional" ON "Usuarios"("CorreoInstitutional");
CREATE INDEX IF NOT EXISTS "IX_Usuarios_IdRol" ON "Usuarios"("IdRol");

-- Tabla Asistencias
CREATE TABLE IF NOT EXISTS "Asistencias" (
    "IdAsistencia" SERIAL PRIMARY KEY,
    "IdUsuario" INTEGER NOT NULL REFERENCES "Usuarios"("IdUsuario"),
    "FechaHoraIngreso" TIMESTAMP NOT NULL,
    "FechaHoraSalida" TIMESTAMP,
    "Modalidad" VARCHAR(20) NOT NULL,
    "IdUbicacion" INTEGER NOT NULL REFERENCES "Ubicaciones"("IdUbicacion"),
    "LatIngreso" DECIMAL(10, 8) NOT NULL,
    "LonIngreso" DECIMAL(11, 8) NOT NULL,
    "EstadoPuntualidad" VARCHAR(20) NOT NULL,
    "FechaCreacion" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS "IX_Asistencias_IdUsuario" ON "Asistencias"("IdUsuario");
CREATE INDEX IF NOT EXISTS "IX_Asistencias_FechaHoraIngreso" ON "Asistencias"("FechaHoraIngreso");

-- Tabla Ausencias
CREATE TABLE IF NOT EXISTS "Ausencias" (
    "IdAusencia" SERIAL PRIMARY KEY,
    "IdUsuario" INTEGER NOT NULL REFERENCES "Usuarios"("IdUsuario"),
    "FechaSolicitud" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "FechaAusencia" DATE NOT NULL,
    "HorarioInicio" TIME NOT NULL,
    "HorarioFin" TIME NOT NULL,
    "TotalHoras" DECIMAL(4, 2) NOT NULL,
    "Motivo" VARCHAR(500) NOT NULL,
    "TipoAusencia" VARCHAR(50) NOT NULL,
    "EstadoAprobacion" VARCHAR(20) NOT NULL,
    "IdAprobador" INTEGER REFERENCES "Usuarios"("IdUsuario"),
    "MotivoRechazo" VARCHAR(500),
    "FechaAprobacion" TIMESTAMP,
    "FechaCreacion" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS "IX_Ausencias_IdUsuario" ON "Ausencias"("IdUsuario");
CREATE INDEX IF NOT EXISTS "IX_Ausencias_FechaAusencia" ON "Ausencias"("FechaAusencia");
CREATE INDEX IF NOT EXISTS "IX_Ausencias_EstadoAprobacion" ON "Ausencias"("EstadoAprobacion");

PRINT 'Tablas creadas exitosamente';
