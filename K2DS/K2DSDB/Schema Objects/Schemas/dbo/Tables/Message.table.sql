CREATE TABLE [dbo].[Message] (
    [Identity]      NVARCHAR (36)   NOT NULL,
    [AppSpecific]   BIGINT          NOT NULL,
    [AppState]      INT             NOT NULL,
    [AppType]       NVARCHAR (255)  NULL,
    [ClientID]      NVARCHAR (255)  NULL,
    [ClientSubID]   NVARCHAR (255)  NULL,
    [CorrelationID] NVARCHAR (255)  NULL,
    [CreationTime]  NVARCHAR (255)  NULL,
    [Data]          NVARCHAR (4000) NULL,
    [Format]        NVARCHAR (255)  NULL,
    [Label]         NVARCHAR (255)  NULL,
    [Tag]           NVARCHAR (512)  NULL,
    [TargetID]      NVARCHAR (255)  NULL,
    [TargetSubID]   NVARCHAR (255)  NULL,
    [UserID]        NVARCHAR (255)  NULL,
    [VenueCode]     NVARCHAR (255)  NULL
);

