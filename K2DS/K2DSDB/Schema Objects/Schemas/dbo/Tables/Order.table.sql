﻿CREATE TABLE [dbo].[Order] (
    [UserID]                NVARCHAR (36)   NULL,
    [SessionID]             NVARCHAR (36)   NULL,
    [CorrelationID]         NVARCHAR (256)  NULL,
    [Account]               NVARCHAR (256)  NULL,
    [AutoTradeProcessCount] INT             NOT NULL,
    [AvgPx]                 FLOAT           NOT NULL,
    [ClOrdID]               NVARCHAR (512)  NULL,
    [CumQty]                FLOAT           NOT NULL,
    [Description]           NVARCHAR (512)  NULL,
    [Expiration]            BIGINT          NOT NULL,
    [ExpireDate]            NVARCHAR (4000) NULL,
    [ExtendedOrdType]       NVARCHAR (255)  NULL,
    [HandlInst]             NVARCHAR (32)   NULL,
    [Identity]              NVARCHAR (36)   NOT NULL,
    [IsAutoTrade]           BIT             NOT NULL,
    [LastPx]                FLOAT           NOT NULL,
    [LastQty]               FLOAT           NOT NULL,
    [LeavesQty]             FLOAT           NOT NULL,
    [LocateReqd]            NVARCHAR (255)  NULL,
    [LocationID]            NVARCHAR (255)  NULL,
    [MaxFloor]              BIGINT          NOT NULL,
    [Mnemonic]              NVARCHAR (255)  NULL,
    [OCAGroupName]          NVARCHAR (256)  NULL,
    [OrdStatus]             NVARCHAR (32)   NULL,
    [OrdType]               NVARCHAR (32)   NULL,
    [OrderID]               NVARCHAR (256)  NULL,
    [OrderQty]              BIGINT          NOT NULL,
    [OrigClOrdID]           NVARCHAR (256)  NULL,
    [ParentIdentity]        NVARCHAR (36)   NULL,
    [Price]                 FLOAT           NOT NULL,
    [QuantityLimit]         FLOAT           NOT NULL,
    [ShortSaleLocate]       NVARCHAR (256)  NULL,
    [Side]                  NVARCHAR (32)   NULL,
    [StopPx]                FLOAT           NOT NULL,
    [StrategyName]          NVARCHAR (256)  NULL,
    [AlgoName]              NVARCHAR (256)  NULL,
    [Tag]                   NVARCHAR (512)  NULL,
    [Text]                  NVARCHAR (512)  NULL,
    [TimeInForce]           NVARCHAR (26)   NULL,
    [TradeVenue]            NVARCHAR (256)  NULL,
    [TransactTime]          NVARCHAR (26)   NULL,
    [TriggerOrderID]        NVARCHAR (36)   NULL,
    [SystemDate]            DATETIME        NOT NULL
);

