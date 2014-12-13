
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/13/2014 16:01:29
-- Generated from EDMX file: C:\Users\Chris\FH WN\Projekte\Master\NumberRecognizer\NumberRecognizerCloud.Data\NetworkDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [NumberRecognizerDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_NetworkTrainingImage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainingImageDataSet] DROP CONSTRAINT [FK_NetworkTrainingImage];
GO
IF OBJECT_ID(N'[dbo].[FK_NetworkTrainLog]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrainLogSet] DROP CONSTRAINT [FK_NetworkTrainLog];
GO
IF OBJECT_ID(N'[dbo].[FK_TrainLogPatternFitness]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PatternFitnessSet] DROP CONSTRAINT [FK_TrainLogPatternFitness];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[NetworkSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NetworkSet];
GO
IF OBJECT_ID(N'[dbo].[TrainingImageDataSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainingImageDataSet];
GO
IF OBJECT_ID(N'[dbo].[TrainLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrainLogSet];
GO
IF OBJECT_ID(N'[dbo].[PatternFitnessSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PatternFitnessSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'NetworkSet'
CREATE TABLE [dbo].[NetworkSet] (
    [NetworkId] int IDENTITY(1,1) NOT NULL,
    [NetworkName] nvarchar(max)  NOT NULL,
    [Calculated] int  NOT NULL,
    [Fitness] float  NOT NULL,
    [NetworkData] varbinary(max)  NULL,
    [CalculationStart] datetime  NULL,
    [CalculationEnd] datetime  NULL,
    [Username] nvarchar(max)  NOT NULL,
    [ErrorStatus] nvarchar(max)  NULL
);
GO

-- Creating table 'TrainingImageDataSet'
CREATE TABLE [dbo].[TrainingImageDataSet] (
    [TrainImageId] int IDENTITY(1,1) NOT NULL,
    [ImageData] varbinary(max)  NOT NULL,
    [Pattern] nvarchar(max)  NOT NULL,
    [Network_NetworkId] int  NOT NULL
);
GO

-- Creating table 'TrainLogSet'
CREATE TABLE [dbo].[TrainLogSet] (
    [TrainLogId] int IDENTITY(1,1) NOT NULL,
    [GenerationNr] int  NOT NULL,
    [MultipleGenPoolIdentifier] nvarchar(max)  NOT NULL,
    [Fitness] float  NOT NULL,
    [Network_NetworkId] int  NOT NULL
);
GO

-- Creating table 'PatternFitnessSet'
CREATE TABLE [dbo].[PatternFitnessSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Pattern] nvarchar(max)  NOT NULL,
    [Fitness] float  NOT NULL,
    [TrainLog_TrainLogId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [NetworkId] in table 'NetworkSet'
ALTER TABLE [dbo].[NetworkSet]
ADD CONSTRAINT [PK_NetworkSet]
    PRIMARY KEY CLUSTERED ([NetworkId] ASC);
GO

-- Creating primary key on [TrainImageId] in table 'TrainingImageDataSet'
ALTER TABLE [dbo].[TrainingImageDataSet]
ADD CONSTRAINT [PK_TrainingImageDataSet]
    PRIMARY KEY CLUSTERED ([TrainImageId] ASC);
GO

-- Creating primary key on [TrainLogId] in table 'TrainLogSet'
ALTER TABLE [dbo].[TrainLogSet]
ADD CONSTRAINT [PK_TrainLogSet]
    PRIMARY KEY CLUSTERED ([TrainLogId] ASC);
GO

-- Creating primary key on [Id] in table 'PatternFitnessSet'
ALTER TABLE [dbo].[PatternFitnessSet]
ADD CONSTRAINT [PK_PatternFitnessSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Network_NetworkId] in table 'TrainingImageDataSet'
ALTER TABLE [dbo].[TrainingImageDataSet]
ADD CONSTRAINT [FK_NetworkTrainingImage]
    FOREIGN KEY ([Network_NetworkId])
    REFERENCES [dbo].[NetworkSet]
        ([NetworkId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NetworkTrainingImage'
CREATE INDEX [IX_FK_NetworkTrainingImage]
ON [dbo].[TrainingImageDataSet]
    ([Network_NetworkId]);
GO

-- Creating foreign key on [Network_NetworkId] in table 'TrainLogSet'
ALTER TABLE [dbo].[TrainLogSet]
ADD CONSTRAINT [FK_NetworkTrainLog]
    FOREIGN KEY ([Network_NetworkId])
    REFERENCES [dbo].[NetworkSet]
        ([NetworkId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NetworkTrainLog'
CREATE INDEX [IX_FK_NetworkTrainLog]
ON [dbo].[TrainLogSet]
    ([Network_NetworkId]);
GO

-- Creating foreign key on [TrainLog_TrainLogId] in table 'PatternFitnessSet'
ALTER TABLE [dbo].[PatternFitnessSet]
ADD CONSTRAINT [FK_TrainLogPatternFitness]
    FOREIGN KEY ([TrainLog_TrainLogId])
    REFERENCES [dbo].[TrainLogSet]
        ([TrainLogId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainLogPatternFitness'
CREATE INDEX [IX_FK_TrainLogPatternFitness]
ON [dbo].[PatternFitnessSet]
    ([TrainLog_TrainLogId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------