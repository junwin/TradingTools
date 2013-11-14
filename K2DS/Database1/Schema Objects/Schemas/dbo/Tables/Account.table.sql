CREATE TABLE [dbo].[Account] (
    [Code]           NVARCHAR (255)  NOT NULL,
    [LongName]       NVARCHAR (255)  NULL,
    [FirmCode]       NVARCHAR (255)  NULL,
    [Identity]       NVARCHAR (255)  NOT NULL,
    [VenueCode]      NVARCHAR (255)  NOT NULL,
    [AccountType]    INT             NOT NULL,
    [InitialMargin]  DECIMAL (29, 4) NOT NULL,
    [MaintMargin]    DECIMAL (29, 4) NOT NULL,
    [NetLiquidity]   DECIMAL (29, 4) NOT NULL,
    [AvailableFunds] DECIMAL (29, 4) NOT NULL,
    [ExcessFunds]    DECIMAL (29, 4) NOT NULL
);

