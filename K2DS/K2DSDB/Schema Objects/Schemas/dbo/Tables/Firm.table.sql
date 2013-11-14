CREATE TABLE [dbo].[Firm] (
    [ID]       NVARCHAR (36)  NULL,
    [FirmCode] NVARCHAR (256) NOT NULL,
    [FirmName] NVARCHAR (256) NULL,
    [FirmType] INT            NOT NULL,
    [External] BIT            NOT NULL
);

