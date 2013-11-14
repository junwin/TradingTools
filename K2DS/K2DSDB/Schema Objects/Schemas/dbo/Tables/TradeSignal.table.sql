CREATE TABLE [dbo].[TradeSignal] (
    [SignalType]  INT            NOT NULL,
    [Identity]    NVARCHAR (36)  NOT NULL,
    [Name]        NVARCHAR (255) NULL,
    [Status]      INT            NOT NULL,
    [Origin]      NVARCHAR (255) NULL,
    [TimeCreated] DATETIME       NOT NULL,
    [TimeValid]   BIGINT         NOT NULL,
    [StrategyID]  NVARCHAR (255) NULL,
    [Set]         BIT            NOT NULL,
    [Mnemonic]    NVARCHAR (255) NULL,
    [OrdType]     NVARCHAR (255) NULL,
    [Side]        NVARCHAR (255) NULL,
    [OrdQty]      FLOAT          NOT NULL,
    [Price]       FLOAT          NOT NULL,
    [StopPrice]   FLOAT          NOT NULL,
    [ProfitPrice] FLOAT          NOT NULL,
    [Text]        NVARCHAR (512) NULL
);

