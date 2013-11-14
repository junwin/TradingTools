CREATE TABLE [dbo].[PlugIn] (
    [AddAssemblyInfo] BIT             NOT NULL,
    [Config]          NVARCHAR (4000) NULL,
    [Enabled]         BIT             NOT NULL,
    [Identity]        NVARCHAR (36)   NOT NULL,
    [Name]            NVARCHAR (256)  NULL,
    [Path]            NVARCHAR (512)  NULL,
    [Vendor]          NVARCHAR (256)  NULL
);

