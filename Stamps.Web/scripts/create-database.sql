-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StampsDB')
BEGIN
    CREATE DATABASE StampsDB;
END
GO

USE StampsDB;
GO

-- Note: Entity Framework will create all tables when you run migrations
-- This script is just to ensure the database exists

