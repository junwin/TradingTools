CREATE TABLE [dbo].[Message] (
    [Identity]      NVARCHAR (36)   NOT NULL,
    [AppSpecific]   BIGINT          NOT NULL,
    [AppState]      INT             NOT NULL,
    [AppType]       NVARCHAR (256)  NULL,
    [ClientID]      NVARCHAR (256)  NULL,
    [ClientSubID]   NVARCHAR (256)  NULL,
    [CorrelationID] NVARCHAR (256)  NULL,
    [CreationTime]  NVARCHAR (256)  NULL,
    [Data]          NVARCHAR (4000) NULL,
    [Format]        NVARCHAR (256)  NULL,
    [Label]         NVARCHAR (256)  NULL,
    [Tag]           NVARCHAR (512)  NULL,
    [TargetID]      NVARCHAR (256)  NULL,
    [TargetSubID]   NVARCHAR (256)  NULL,
    [UserID]        NVARCHAR (256)  NULL,
    [VenueCode]     NVARCHAR (256)  NULL
);

