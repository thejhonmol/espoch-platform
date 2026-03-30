-- 02_CreateTables.sql
-- Script para crear las tablas de la base de datos ESPOCH

USE ESPOCH;
GO

-- Tabla Roles
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        IdRol INT PRIMARY KEY IDENTITY(1,1),
        NombreRol NVARCHAR(50) NOT NULL,
        Descripcion NVARCHAR(200) NULL,
        Estado BIT NOT NULL DEFAULT 1
    );
    PRINT 'Tabla Roles creada correctamente';
END
GO

-- Tabla Horarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Horarios')
BEGIN
    CREATE TABLE Horarios (
        IdHorario INT PRIMARY KEY IDENTITY(1,1),
        NombreHorario NVARCHAR(50) NOT NULL,
        HorarioInicio TIME NOT NULL,
        HorarioFin TIME NOT NULL,
        Estado BIT NOT NULL DEFAULT 1
    );
    PRINT 'Tabla Horarios creada correctamente';
END
GO

-- Tabla Ubicaciones
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ubicaciones')
BEGIN
    CREATE TABLE Ubicaciones (
        IdUbicacion INT PRIMARY KEY IDENTITY(1,1),
        CodigoUbicacion NVARCHAR(20) NOT NULL,
        Direccion NVARCHAR(200) NOT NULL,
        Latitud FLOAT NOT NULL,
        Longitud FLOAT NOT NULL,
        RadioPermitidoSede INT NOT NULL DEFAULT 100,
        Estado BIT NOT NULL DEFAULT 1
    );
    PRINT 'Tabla Ubicaciones creada correctamente';
END
GO

-- Tabla Usuarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        IdUsuario INT PRIMARY KEY IDENTITY(1,1),
        NombreCompleto NVARCHAR(100) NOT NULL,
        CorreoInstitucional NVARCHAR(150) NOT NULL,
        azureOid NVARCHAR(100) NULL,
        IdRol INT NOT NULL,
        IdJefeDirecto INT NULL,
        IdHorario INT NULL,
        Estado BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol) REFERENCES Roles(IdRol),
        CONSTRAINT FK_Usuarios_Usuarios_Jefe FOREIGN KEY (IdJefeDirecto) REFERENCES Usuarios(IdUsuario),
        CONSTRAINT FK_Usuarios_Horarios FOREIGN KEY (IdHorario) REFERENCES Horarios(IdHorario),
        CONSTRAINT UQ_Usuarios_Correo UNIQUE (CorreoInstitucional)
    );
    CREATE INDEX IX_Usuarios_azureOid ON Usuarios(azureOid);
    PRINT 'Tabla Usuarios creada correctamente';
END
GO

-- Tabla Asistencias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Asistencias')
BEGIN
    CREATE TABLE Asistencias (
        IdAsistencia INT PRIMARY KEY IDENTITY(1,1),
        IdUsuario INT NOT NULL,
        FechaHoraIngreso DATETIME NULL,
        FechaHoraSalida DATETIME NULL,
        Modalidad NVARCHAR(20) NOT NULL DEFAULT 'Presencial',
        IdUbicacion INT NOT NULL,
        LatIngreso FLOAT NOT NULL,
        LonIngreso FLOAT NOT NULL,
        LatSalida FLOAT NULL,
        LonSalida FLOAT NULL,
        EstadoPuntualidad NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
        FechaCreacion DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Asistencias_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario),
        CONSTRAINT FK_Asistencias_Ubicaciones FOREIGN KEY (IdUbicacion) REFERENCES Ubicaciones(IdUbicacion)
    );
    CREATE INDEX IX_Asistencias_IdUsuario ON Asistencias(IdUsuario);
    CREATE INDEX IX_Asistencias_FechaHoraIngreso ON Asistencias(FechaHoraIngreso);
    PRINT 'Tabla Asistencias creada correctamente';
END
GO

-- Tabla Ausencias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ausencias')
BEGIN
    CREATE TABLE Ausencias (
        IdAusencia INT PRIMARY KEY IDENTITY(1,1),
        IdUsuario INT NOT NULL,
        FechaSolicitud DATETIME NOT NULL DEFAULT GETUTCDATE(),
        FechaAusencia DATE NOT NULL,
        HorarioInicio TIME NOT NULL,
        HorarioFin TIME NOT NULL,
        TotalHoras DECIMAL(5,2) NOT NULL,
        Motivo NVARCHAR(500) NOT NULL,
        TipoAusencia NVARCHAR(50) NOT NULL,
        EstadoAprobacion NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
        IdAprobador INT NULL,
        MotivoRechazo NVARCHAR(500) NULL,
        FechaAprobacion DATETIME NULL,
        CONSTRAINT FK_Ausencias_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario),
        CONSTRAINT FK_Ausencias_Aprobador FOREIGN KEY (IdAprobador) REFERENCES Usuarios(IdUsuario)
    );
    CREATE INDEX IX_Ausencias_IdUsuario ON Ausencias(IdUsuario);
    CREATE INDEX IX_Ausencias_FechaAusencia ON Ausencias(FechaAusencia);
    CREATE INDEX IX_Ausencias_EstadoAprobacion ON Ausencias(EstadoAprobacion);
    PRINT 'Tabla Ausencias creada correctamente';
END
GO

PRINT 'Todas las tablas fueron creadas correctamente';
GO
