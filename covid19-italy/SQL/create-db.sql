CREATE DATABASE [covid_data]
GO

USE covid_data;
GO

CREATE TABLE covid_data.dbo.pcm_dpc_italy (
	ts bigint NOT NULL,
	hospitalisedWithSymptoms int NULL,
	intensiveCare int NULL,
	totalHospitalised int NULL,
	homeConfinement int NULL,
	currentPositiveCases int NULL,
	newCurrentPositiveCases int NULL,
	newPositiveCases int NULL,
	recovered int NULL,
	deaths int NULL,
	totalPositiveCases int NULL,
	testsPerformed int NULL,
	CONSTRAINT pcm_dpc_italy_PK PRIMARY KEY (ts)
);
GO

CREATE TABLE covid_data.dbo.pcm_dpc_regions (
	ts bigint NOT NULL,
    regionCode int,
    regionName nvarchar(100),
	hospitalisedWithSymptoms int NULL,
	intensiveCare int NULL,
	totalHospitalised int NULL,
	homeConfinement int NULL,
	currentPositiveCases int NULL,
	newCurrentPositiveCases int NULL,
	newPositiveCases int NULL,
	recovered int NULL,
	deaths int NULL,
	totalPositiveCases int NULL,
	testsPerformed int NULL,
	CONSTRAINT pcm_dpc_regions_PK PRIMARY KEY (ts,regionCode)
);
GO

CREATE TABLE covid_data.dbo.pcm_dpc_provinces (
	ts bigint NOT NULL,
    provinceCode int,
    provinceName nvarchar(100),
    totalPositiveCases int NULL,
	CONSTRAINT pcm_dpc_provinces_PK PRIMARY KEY (ts,provinceCode)
);
GO