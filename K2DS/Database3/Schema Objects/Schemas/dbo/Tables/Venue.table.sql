CREATE TABLE [dbo].[Venue] (
    [AccountNumber]           NVARCHAR (256)  NULL,
    [BeginString]             NVARCHAR (256)  NULL,
    [CancelBag]               NVARCHAR (2048) NULL,
    [Code]                    NVARCHAR (256)  NOT NULL,
    [TargetVenue]             NVARCHAR (256)  NULL,
    [DataFeedVenue]           NVARCHAR (256)  NULL,
    [DriverCode]              NVARCHAR (256)  NULL,
    [NOSBag]                  NVARCHAR (2048) NULL,
    [Name]                    NVARCHAR (256)  NULL,
    [ReplaceBag]              NVARCHAR (2048) NULL,
    [SID]                     NVARCHAR (256)  NULL,
    [TID]                     NVARCHAR (256)  NULL,
    [DefaultCurrencyCode]     NVARCHAR (3)    NULL,
    [DefaultSecurityExchange] NVARCHAR (256)  NULL,
    [DefaultCFICode]          NVARCHAR (8)    NULL,
    [UseSymbol]               BIT             NOT NULL
);

