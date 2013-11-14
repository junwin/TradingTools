CREATE TABLE [dbo].[Firm] (
    [ID]       NVARCHAR (255) NULL,
    [FirmCode] NVARCHAR (255) NOT NULL,
    [FirmName] NVARCHAR (512) NULL,
    [FirmType] INT            NOT NULL,
    [External] BIT            NOT NULL
);

