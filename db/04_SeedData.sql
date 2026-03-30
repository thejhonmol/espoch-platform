-- 04_SeedData.sql
-- Script para insertar datos iniciales en la base de datos ESPOCH

USE ESPOCH;
GO

-- Agregar columna ContrasenaHash si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'ContrasenaHash')
BEGIN
    ALTER TABLE Usuarios ADD ContrasenaHash NVARCHAR(500) NULL;
    PRINT 'Columna ContrasenaHash agregada';
END
GO

-- Insertar Roles
IF NOT EXISTS (SELECT * FROM Roles)
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Estado) VALUES
    ('Admin', 'Administrador del sistema con acceso completo', 1),
    ('JefeDirecto', 'Jefe de departamento con acceso a gestionar colaboradores', 1),
    ('Colaborador', 'Colaborador regular con acceso básico', 1);
    PRINT 'Roles insertados correctamente';
END
GO

-- Insertar Horarios
IF NOT EXISTS (SELECT * FROM Horarios)
BEGIN
    INSERT INTO Horarios (NombreHorario, HorarioInicio, HorarioFin, Estado) VALUES
    ('Matutino', '08:00:00', '12:00:00', 1),
    ('Vespertino', '14:00:00', '18:00:00', 1),
    ('Continuo', '09:00:00', '17:00:00', 1);
    PRINT 'Horarios insertados correctamente';
END
GO

-- Insertar Ubicaciones
IF NOT EXISTS (SELECT * FROM Ubicaciones)
BEGIN
    INSERT INTO Ubicaciones (CodigoUbicacion, Direccion, Latitud, Longitud, RadioPermitidoSede, Estado) VALUES
    ('SEDE-001', 'Av. Principal 123, Quito', -0.180653, -78.467834, 100, 1),
    ('SEDE-002', 'Calle Secundaria 456, Guayaquil', -2.189427, -79.889041, 100, 1),
    ('SEDE-003', 'Av. Universitaria 789, Cuenca', -2.900564, -79.005905, 100, 1);
    PRINT 'Ubicaciones insertadas correctamente';
END
GO

-- Insertar Usuarios de prueba (CON CONTRASEÑAS)
IF NOT EXISTS (SELECT * FROM Usuarios WHERE CorreoInstitucional = 'admin@espoch.edu.ec')
BEGIN
    DECLARE @idHorario1 INT = (SELECT IdHorario FROM Horarios WHERE NombreHorario = 'Continuo');
    
    INSERT INTO Usuarios (NombreCompleto, CorreoInstitucional, ContrasenaHash, IdRol, IdHorario, Estado, FechaCreacion) VALUES
    ('Administrador Sistema', 'admin@espoch.edu.ec', 'admin123', 1, @idHorario1, 1, GETUTCDATE());
    
    PRINT 'Usuario admin insertado correctamente (email: admin@espoch.edu.ec, pass: admin123)';
END
GO

IF NOT EXISTS (SELECT * FROM Usuarios WHERE CorreoInstitucional = 'jefe.departamento@espoch.edu.ec')
BEGIN
    DECLARE @idJefeAdmin INT = (SELECT IdUsuario FROM Usuarios WHERE CorreoInstitucional = 'admin@espoch.edu.ec');
    DECLARE @idHorario2 INT = (SELECT IdHorario FROM Horarios WHERE NombreHorario = 'Matutino');
    
    INSERT INTO Usuarios (NombreCompleto, CorreoInstitucional, ContrasenaHash, IdRol, IdJefeDirecto, IdHorario, Estado, FechaCreacion) VALUES
    ('Juan Pérez López', 'jefe.departamento@espoch.edu.ec', 'jefe123', 2, @idJefeAdmin, @idHorario2, 1, GETUTCDATE());
    
    PRINT 'Usuario jefe directo insertado correctamente (email: jefe.departamento@espoch.edu.ec, pass: jefe123)';
END
GO

IF NOT EXISTS (SELECT * FROM Usuarios WHERE CorreoInstitucional = 'colaborador@espoch.edu.ec')
BEGIN
    DECLARE @idJefe INT = (SELECT IdUsuario FROM Usuarios WHERE CorreoInstitucional = 'jefe.departamento@espoch.edu.ec');
    DECLARE @idHorario3 INT = (SELECT IdHorario FROM Horarios WHERE NombreHorario = 'Matutino');
    
    INSERT INTO Usuarios (NombreCompleto, CorreoInstitucional, ContrasenaHash, IdRol, IdJefeDirecto, IdHorario, Estado, FechaCreacion) VALUES
    ('María García Rodríguez', 'colaborador@espoch.edu.ec', 'colab123', 3, @idJefe, @idHorario3, 1, GETUTCDATE());
    
    PRINT 'Usuario colaborador insertado correctamente (email: colaborador@espoch.edu.ec, pass: colab123)';
END
GO

-- Insertar algunos datos de prueba para Asistencias
IF NOT EXISTS (SELECT * FROM Asistencias)
BEGIN
    DECLARE @idUsuario INT = (SELECT IdUsuario FROM Usuarios WHERE CorreoInstitucional = 'colaborador@espoch.edu.ec');
    DECLARE @idUbicacion INT = (SELECT IdUbicacion FROM Ubicaciones WHERE CodigoUbicacion = 'SEDE-001');
    
    INSERT INTO Asistencias (IdUsuario, FechaHoraIngreso, Modalidad, IdUbicacion, LatIngreso, LonIngreso, EstadoPuntualidad) VALUES
    (@idUsuario, DATEADD(DAY, -1, GETUTCDATE()), 'Presencial', @idUbicacion, -0.180653, -78.467834, 'Presente'),
    (@idUsuario, DATEADD(DAY, -2, GETUTCDATE()), 'Presencial', @idUbicacion, -0.180653, -78.467834, 'Tardanza');
    
    PRINT 'Asistencias de prueba insertadas correctamente';
END
GO

-- Insertar algunos datos de prueba para Ausencias
IF NOT EXISTS (SELECT * FROM Ausencias)
BEGIN
    DECLARE @idUsuario INT = (SELECT IdUsuario FROM Usuarios WHERE CorreoInstitucional = 'colaborador@espoch.edu.ec');
    DECLARE @idJefe INT = (SELECT IdUsuario FROM Usuarios WHERE CorreoInstitucional = 'jefe.departamento@espoch.edu.ec');
    
    INSERT INTO Ausencias (IdUsuario, FechaSolicitud, FechaAusencia, HorarioInicio, HorarioFin, TotalHoras, Motivo, TipoAusencia, EstadoAprobacion, IdAprobador) VALUES
    (@idUsuario, DATEADD(DAY, -5, GETUTCDATE()), DATEADD(DAY, 3, GETUTCDATE()), '08:00:00', '12:00:00', 4.00, 'Cita médica', 'Médica', 'Aprobada', @idJefe),
    (@idUsuario, GETUTCDATE(), DATEADD(DAY, 7, GETUTCDATE()), '14:00:00', '18:00:00', 4.00, 'Trámite personal', 'Personal', 'Pendiente', NULL);
    
    PRINT 'Ausencias de prueba insertadas correctamente';
END
GO

PRINT '=== DATOS DE PRUEBA ===';
PRINT 'Admin: admin@espoch.edu.ec / admin123';
PRINT 'Jefe: jefe.departamento@espoch.edu.ec / jefe123';
PRINT 'Colaborador: colaborador@espoch.edu.ec / colab123';
PRINT '========================';
GO
