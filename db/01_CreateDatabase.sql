-- 01_CreateDatabase.sql
-- Script para crear la base de datos ESPOCH

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ESPOCH')
BEGIN
    CREATE DATABASE ESPOCH;
    PRINT 'Base de datos ESPOCH creada correctamente';
END
ELSE
BEGIN
    PRINT 'La base de datos ESPOCH ya existe';
END
GO

USE ESPOCH;
GO
