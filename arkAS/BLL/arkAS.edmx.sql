
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/23/2015 16:04:41
-- Generated from EDMX file: D:\Проекты\Rudensoft\arkAS\arkAS\BLL\arkAS.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [arkAS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_as_categories_as_categories1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_categories] DROP CONSTRAINT [FK_as_categories_as_categories1];
GO
IF OBJECT_ID(N'[dbo].[FK_as_settings_as_dataTypes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_settings] DROP CONSTRAINT [FK_as_settings_as_dataTypes];
GO
IF OBJECT_ID(N'[dbo].[FK_as_menu_as_menu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_menu] DROP CONSTRAINT [FK_as_menu_as_menu];
GO
IF OBJECT_ID(N'[dbo].[FK_as_menuRoles_as_menu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_menuRoles] DROP CONSTRAINT [FK_as_menuRoles_as_menu];
GO
IF OBJECT_ID(N'[dbo].[FK_as_mt_metrics_as_mt_metrics]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_mt_metrics] DROP CONSTRAINT [FK_as_mt_metrics_as_mt_metrics];
GO
IF OBJECT_ID(N'[dbo].[FK_as_mt_metrics_as_mt_metricTypes]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_mt_metrics] DROP CONSTRAINT [FK_as_mt_metrics_as_mt_metricTypes];
GO
IF OBJECT_ID(N'[dbo].[FK_as_profilePropertyValues_as_profileProperties]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_profilePropertyValues] DROP CONSTRAINT [FK_as_profilePropertyValues_as_profileProperties];
GO
IF OBJECT_ID(N'[dbo].[FK_as_settingAvailableValues_as_settings]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_settingAvailableValues] DROP CONSTRAINT [FK_as_settingAvailableValues_as_settings];
GO
IF OBJECT_ID(N'[dbo].[FK_as_settings_as_settingCategories]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_settings] DROP CONSTRAINT [FK_as_settings_as_settingCategories];
GO
IF OBJECT_ID(N'[dbo].[FK_as_rightsRoles_as_rights]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[as_rightsRoles] DROP CONSTRAINT [FK_as_rightsRoles_as_rights];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[as_categories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_categories];
GO
IF OBJECT_ID(N'[dbo].[as_dataTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_dataTypes];
GO
IF OBJECT_ID(N'[dbo].[as_menu]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_menu];
GO
IF OBJECT_ID(N'[dbo].[as_menuRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_menuRoles];
GO
IF OBJECT_ID(N'[dbo].[as_mt_metrics]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_mt_metrics];
GO
IF OBJECT_ID(N'[dbo].[as_mt_metricTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_mt_metricTypes];
GO
IF OBJECT_ID(N'[dbo].[as_profileProperties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_profileProperties];
GO
IF OBJECT_ID(N'[dbo].[as_profilePropertyValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_profilePropertyValues];
GO
IF OBJECT_ID(N'[dbo].[as_rights]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_rights];
GO
IF OBJECT_ID(N'[dbo].[as_rightsRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_rightsRoles];
GO
IF OBJECT_ID(N'[dbo].[as_settingAvailableValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_settingAvailableValues];
GO
IF OBJECT_ID(N'[dbo].[as_settingCategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_settingCategories];
GO
IF OBJECT_ID(N'[dbo].[as_settings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_settings];
GO
IF OBJECT_ID(N'[dbo].[as_statuses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_statuses];
GO
IF OBJECT_ID(N'[dbo].[as_statusLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_statusLog];
GO
IF OBJECT_ID(N'[dbo].[as_trace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[as_trace];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'as_categories'
CREATE TABLE [dbo].[as_categories] (
    [id] int IDENTITY(1,1) NOT NULL,
    [typeCode] nvarchar(20)  NULL,
    [name] nvarchar(128)  NULL,
    [parentID] int  NULL,
    [desc] nvarchar(256)  NULL
);
GO

-- Creating table 'as_dataTypes'
CREATE TABLE [dbo].[as_dataTypes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(32)  NULL,
    [code] nvarchar(20)  NULL
);
GO

-- Creating table 'as_menu'
CREATE TABLE [dbo].[as_menu] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(256)  NULL,
    [url] nvarchar(256)  NULL,
    [pattern] nvarchar(256)  NULL,
    [parentID] int  NULL
);
GO

-- Creating table 'as_menuRoles'
CREATE TABLE [dbo].[as_menuRoles] (
    [id] int  NOT NULL,
    [itemID] int  NULL,
    [role] nvarchar(32)  NULL,
    [ord] int  NULL
);
GO

-- Creating table 'as_mt_metrics'
CREATE TABLE [dbo].[as_mt_metrics] (
    [id] int IDENTITY(1,1) NOT NULL,
    [title] nvarchar(256)  NULL,
    [subtitle] nvarchar(512)  NULL,
    [sql] nvarchar(max)  NULL,
    [parName] nvarchar(128)  NULL,
    [parentID] int  NULL,
    [ord] int  NULL,
    [typeID] int  NULL
);
GO

-- Creating table 'as_mt_metricTypes'
CREATE TABLE [dbo].[as_mt_metricTypes] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(128)  NULL,
    [code] nvarchar(20)  NULL
);
GO

-- Creating table 'as_profileProperties'
CREATE TABLE [dbo].[as_profileProperties] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(64)  NULL,
    [code] nvarchar(64)  NULL
);
GO

-- Creating table 'as_profilePropertyValues'
CREATE TABLE [dbo].[as_profilePropertyValues] (
    [id] int IDENTITY(1,1) NOT NULL,
    [propertyID] int  NULL,
    [value] nvarchar(512)  NULL,
    [userGuid] uniqueidentifier  NULL
);
GO

-- Creating table 'as_rights'
CREATE TABLE [dbo].[as_rights] (
    [id] int IDENTITY(1,1) NOT NULL,
    [code] nvarchar(64)  NULL,
    [name] nvarchar(128)  NULL
);
GO

-- Creating table 'as_rightsRoles'
CREATE TABLE [dbo].[as_rightsRoles] (
    [id] int IDENTITY(1,1) NOT NULL,
    [rightID] int  NULL,
    [role] nvarchar(64)  NULL
);
GO

-- Creating table 'as_settingAvailableValues'
CREATE TABLE [dbo].[as_settingAvailableValues] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(128)  NULL,
    [value] nvarchar(256)  NULL,
    [settingID] int  NULL
);
GO

-- Creating table 'as_settingCategories'
CREATE TABLE [dbo].[as_settingCategories] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(128)  NULL,
    [ord] int  NULL
);
GO

-- Creating table 'as_settings'
CREATE TABLE [dbo].[as_settings] (
    [id] int IDENTITY(1,1) NOT NULL,
    [code] nvarchar(32)  NULL,
    [value] nvarchar(max)  NULL,
    [categoryID] int  NULL,
    [typeID] int  NULL,
    [value2] nvarchar(256)  NULL,
    [name] nvarchar(256)  NULL
);
GO

-- Creating table 'as_statuses'
CREATE TABLE [dbo].[as_statuses] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(128)  NULL,
    [color] nvarchar(12)  NULL,
    [code] nvarchar(24)  NULL,
    [desc] nvarchar(256)  NULL,
    [typeCode] nvarchar(16)  NULL
);
GO

-- Creating table 'as_statusLog'
CREATE TABLE [dbo].[as_statusLog] (
    [id] int IDENTITY(1,1) NOT NULL,
    [statusID] int  NULL,
    [itemID] int  NULL,
    [created] datetime  NULL,
    [username] nvarchar(128)  NULL,
    [typeCode] nvarchar(16)  NULL
);
GO

-- Creating table 'as_trace'
CREATE TABLE [dbo].[as_trace] (
    [id] int IDENTITY(1,1) NOT NULL,
    [header] nvarchar(256)  NULL,
    [text] nvarchar(256)  NULL,
    [code] nvarchar(64)  NULL,
    [created] datetime  NULL,
    [itemID] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'as_categories'
ALTER TABLE [dbo].[as_categories]
ADD CONSTRAINT [PK_as_categories]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_dataTypes'
ALTER TABLE [dbo].[as_dataTypes]
ADD CONSTRAINT [PK_as_dataTypes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_menu'
ALTER TABLE [dbo].[as_menu]
ADD CONSTRAINT [PK_as_menu]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_menuRoles'
ALTER TABLE [dbo].[as_menuRoles]
ADD CONSTRAINT [PK_as_menuRoles]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_mt_metrics'
ALTER TABLE [dbo].[as_mt_metrics]
ADD CONSTRAINT [PK_as_mt_metrics]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_mt_metricTypes'
ALTER TABLE [dbo].[as_mt_metricTypes]
ADD CONSTRAINT [PK_as_mt_metricTypes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_profileProperties'
ALTER TABLE [dbo].[as_profileProperties]
ADD CONSTRAINT [PK_as_profileProperties]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_profilePropertyValues'
ALTER TABLE [dbo].[as_profilePropertyValues]
ADD CONSTRAINT [PK_as_profilePropertyValues]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_rights'
ALTER TABLE [dbo].[as_rights]
ADD CONSTRAINT [PK_as_rights]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_rightsRoles'
ALTER TABLE [dbo].[as_rightsRoles]
ADD CONSTRAINT [PK_as_rightsRoles]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_settingAvailableValues'
ALTER TABLE [dbo].[as_settingAvailableValues]
ADD CONSTRAINT [PK_as_settingAvailableValues]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_settingCategories'
ALTER TABLE [dbo].[as_settingCategories]
ADD CONSTRAINT [PK_as_settingCategories]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_settings'
ALTER TABLE [dbo].[as_settings]
ADD CONSTRAINT [PK_as_settings]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_statuses'
ALTER TABLE [dbo].[as_statuses]
ADD CONSTRAINT [PK_as_statuses]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_statusLog'
ALTER TABLE [dbo].[as_statusLog]
ADD CONSTRAINT [PK_as_statusLog]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'as_trace'
ALTER TABLE [dbo].[as_trace]
ADD CONSTRAINT [PK_as_trace]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [parentID] in table 'as_categories'
ALTER TABLE [dbo].[as_categories]
ADD CONSTRAINT [FK_as_categories_as_categories1]
    FOREIGN KEY ([parentID])
    REFERENCES [dbo].[as_categories]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_categories_as_categories1'
CREATE INDEX [IX_FK_as_categories_as_categories1]
ON [dbo].[as_categories]
    ([parentID]);
GO

-- Creating foreign key on [typeID] in table 'as_settings'
ALTER TABLE [dbo].[as_settings]
ADD CONSTRAINT [FK_as_settings_as_dataTypes]
    FOREIGN KEY ([typeID])
    REFERENCES [dbo].[as_dataTypes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_settings_as_dataTypes'
CREATE INDEX [IX_FK_as_settings_as_dataTypes]
ON [dbo].[as_settings]
    ([typeID]);
GO

-- Creating foreign key on [parentID] in table 'as_menu'
ALTER TABLE [dbo].[as_menu]
ADD CONSTRAINT [FK_as_menu_as_menu]
    FOREIGN KEY ([parentID])
    REFERENCES [dbo].[as_menu]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_menu_as_menu'
CREATE INDEX [IX_FK_as_menu_as_menu]
ON [dbo].[as_menu]
    ([parentID]);
GO

-- Creating foreign key on [itemID] in table 'as_menuRoles'
ALTER TABLE [dbo].[as_menuRoles]
ADD CONSTRAINT [FK_as_menuRoles_as_menu]
    FOREIGN KEY ([itemID])
    REFERENCES [dbo].[as_menu]
        ([id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_menuRoles_as_menu'
CREATE INDEX [IX_FK_as_menuRoles_as_menu]
ON [dbo].[as_menuRoles]
    ([itemID]);
GO

-- Creating foreign key on [parentID] in table 'as_mt_metrics'
ALTER TABLE [dbo].[as_mt_metrics]
ADD CONSTRAINT [FK_as_mt_metrics_as_mt_metrics]
    FOREIGN KEY ([parentID])
    REFERENCES [dbo].[as_mt_metrics]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_mt_metrics_as_mt_metrics'
CREATE INDEX [IX_FK_as_mt_metrics_as_mt_metrics]
ON [dbo].[as_mt_metrics]
    ([parentID]);
GO

-- Creating foreign key on [typeID] in table 'as_mt_metrics'
ALTER TABLE [dbo].[as_mt_metrics]
ADD CONSTRAINT [FK_as_mt_metrics_as_mt_metricTypes]
    FOREIGN KEY ([typeID])
    REFERENCES [dbo].[as_mt_metricTypes]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_mt_metrics_as_mt_metricTypes'
CREATE INDEX [IX_FK_as_mt_metrics_as_mt_metricTypes]
ON [dbo].[as_mt_metrics]
    ([typeID]);
GO

-- Creating foreign key on [propertyID] in table 'as_profilePropertyValues'
ALTER TABLE [dbo].[as_profilePropertyValues]
ADD CONSTRAINT [FK_as_profilePropertyValues_as_profileProperties]
    FOREIGN KEY ([propertyID])
    REFERENCES [dbo].[as_profileProperties]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_profilePropertyValues_as_profileProperties'
CREATE INDEX [IX_FK_as_profilePropertyValues_as_profileProperties]
ON [dbo].[as_profilePropertyValues]
    ([propertyID]);
GO

-- Creating foreign key on [settingID] in table 'as_settingAvailableValues'
ALTER TABLE [dbo].[as_settingAvailableValues]
ADD CONSTRAINT [FK_as_settingAvailableValues_as_settings]
    FOREIGN KEY ([settingID])
    REFERENCES [dbo].[as_settings]
        ([id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_settingAvailableValues_as_settings'
CREATE INDEX [IX_FK_as_settingAvailableValues_as_settings]
ON [dbo].[as_settingAvailableValues]
    ([settingID]);
GO

-- Creating foreign key on [categoryID] in table 'as_settings'
ALTER TABLE [dbo].[as_settings]
ADD CONSTRAINT [FK_as_settings_as_settingCategories]
    FOREIGN KEY ([categoryID])
    REFERENCES [dbo].[as_settingCategories]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_settings_as_settingCategories'
CREATE INDEX [IX_FK_as_settings_as_settingCategories]
ON [dbo].[as_settings]
    ([categoryID]);
GO

-- Creating foreign key on [rightID] in table 'as_rightsRoles'
ALTER TABLE [dbo].[as_rightsRoles]
ADD CONSTRAINT [FK_as_rightsRoles_as_rights]
    FOREIGN KEY ([rightID])
    REFERENCES [dbo].[as_rights]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_as_rightsRoles_as_rights'
CREATE INDEX [IX_FK_as_rightsRoles_as_rights]
ON [dbo].[as_rightsRoles]
    ([rightID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------