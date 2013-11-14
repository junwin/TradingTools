CREATE TABLE [dbo].[Fill] (
    [OrderStatus] NVARCHAR (255)  NULL,
    [AvgPx]       FLOAT           NOT NULL,
    [LeavesQty]   FLOAT           NOT NULL,
    [CumQty]      FLOAT           NOT NULL,
    [Ticks]       BIGINT          NOT NULL,
    [ExecReport]  NVARCHAR (4000) NULL,
    [FillQty]     FLOAT           NOT NULL,
    [LastPx]      FLOAT           NOT NULL,
    [OrderID]     NVARCHAR (512)  NULL,
    [Identity]    NVARCHAR (255)  NOT NULL
);

