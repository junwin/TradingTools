CREATE TABLE [dbo].[Account] (
    [Code]           NVARCHAR (256)  NOT NULL,
    [LongName]       NVARCHAR (256)  NULL,
    [FirmCode]       NVARCHAR (256)  NULL,
    [Identity]       NVARCHAR (36)   NOT NULL,
    [VenueCode]      NVARCHAR (256)  NOT NULL,
    [AccountType]    INT             NOT NULL,
    [InitialMargin]  DECIMAL (29, 4) NOT NULL,
    [MaintMargin]    DECIMAL (29, 4) NOT NULL,
    [NetLiquidity]   DECIMAL (29, 4) NOT NULL,
    [AvailableFunds] DECIMAL (29, 4) NOT NULL,
    [ExcessFunds]    DECIMAL (29, 4) NOT NULL
);

