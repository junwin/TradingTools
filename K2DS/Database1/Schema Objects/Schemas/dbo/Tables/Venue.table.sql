CREATE TABLE [dbo].[Venue] (
    [AccountNumber]           NVARCHAR (255)  NULL,
    [BeginString]             NVARCHAR (255)  NULL,
    [CancelBag]               NVARCHAR (2048) NULL,
    [Code]                    NVARCHAR (255)  NULL,
    [VenueCode]               NVARCHAR (255)  NOT NULL,
    [TargetVenue]             NVARCHAR (255)  NULL,
    [DataFeedVenue]           NVARCHAR (255)  NULL,
    [DriverCode]              NVARCHAR (255)  NULL,
    [NOSBag]                  NVARCHAR (2048) NULL,
    [Name]                    NVARCHAR (255)  NULL,
    [ReplaceBag]              NVARCHAR (2048) NULL,
    [SID]                     NVARCHAR (255)  NULL,
    [TID]                     NVARCHAR (255)  NULL,
    [DefaultCurrencyCode]     NVARCHAR (3)    NULL,
    [DefaultSecurityExchange] NVARCHAR (255)  NULL,
    [DefaultCFICode]          NVARCHAR (16)   NULL,
    [UseSymbol]               BIT             NOT NULL
);

