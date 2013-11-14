CREATE TABLE [dbo].[Driver] (
    [m_ManualStart]        BIT             NOT NULL,
    [m_RouteCode]          NVARCHAR (64)   NULL,
    [AsyncPrices]          BIT             NOT NULL,
    [Code]                 NVARCHAR (64)   NULL,
    [Config]               NVARCHAR (1024) NULL,
    [ConfigPath]           NVARCHAR (512)  NULL,
    [Enabled]              BIT             NOT NULL,
    [HideDriverUI]         BIT             NOT NULL,
    [Identity]             NVARCHAR (36)   NOT NULL,
    [LiveMarket]           BIT             NOT NULL,
    [Name]                 NVARCHAR (256)  NULL,
    [Password]             NVARCHAR (256)  NULL,
    [Path]                 NVARCHAR (512)  NULL,
    [QueueReplaceRequests] BIT             NOT NULL,
    [ServerName]           NVARCHAR (256)  NULL,
    [ServerPort]           NVARCHAR (6)    NULL,
    [UserID]               NVARCHAR (256)  NULL,
    [Vendor]               NVARCHAR (256)  NULL
);

