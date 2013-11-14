CREATE TABLE [dbo].[Fill] (
    [OrderStatus] NVARCHAR (1)    NULL,
    [AvgPx]       FLOAT           NOT NULL,
    [LeavesQty]   FLOAT           NOT NULL,
    [CumQty]      FLOAT           NOT NULL,
    [Ticks]       BIGINT          NOT NULL,
    [Sequence]    BIGINT          IDENTITY (1, 1) NOT NULL,
    [ClOrdID]     NVARCHAR (256)  NULL,
    [OrigClOrdID] NVARCHAR (256)  NULL,
    [ExecType]    NVARCHAR (32)   NULL,
    [ExecRefID]   NVARCHAR (256)  NULL,
    [ExecReport]  NVARCHAR (4000) NULL,
    [FillQty]     FLOAT           NOT NULL,
    [LastPx]      FLOAT           NOT NULL,
    [OrderID]     NVARCHAR (256)  NULL,
    [Identity]    NVARCHAR (256)  NOT NULL
);

