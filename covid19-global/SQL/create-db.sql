IF NOT EXISTS 
   (
     SELECT name FROM master.dbo.sysdatabases 
     WHERE name = N'covid_data'
    )
CREATE DATABASE [covid_data]
GO

USE covid_data;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='covid_data.dbo.daily_global' and xtype='U')
	CREATE TABLE covid_data.dbo.daily_global (
	    ts bigint NULL,
	    country nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	    geoId nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	    cases int NULL,
	    deaths int NULL,
	    ID bigint IDENTITY(0,1) NOT NULL,
	    CONSTRAINT daily_global_PK PRIMARY KEY (ID)
	);
GO