CREATE TABLE [dbo].[Server] (
    [Enabled]        BIT            NOT NULL,
    [ID]             NVARCHAR (36)  NOT NULL,
    [InstanceNumber] BIGINT         NOT NULL,
    [MachineName]    NVARCHAR (255) NULL,
    [Name]           NVARCHAR (255) NULL,
    [ServerRole]     NVARCHAR (255) NULL,
    [Running]        BIT            NOT NULL,
    [startTimeTicks] BIGINT         NOT NULL
);

