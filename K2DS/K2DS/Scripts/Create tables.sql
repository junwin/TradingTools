/****** Object:  Table [dbo].[Account]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Account]') AND type in (N'U'))
DROP TABLE [dbo].[Account]
GO
/****** Object:  Table [dbo].[Exchange]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exchange]') AND type in (N'U'))
DROP TABLE [dbo].[Exchange]
GO
/****** Object:  Table [dbo].[Fill]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Fill]') AND type in (N'U'))
DROP TABLE [dbo].[Fill]
GO
/****** Object:  Table [dbo].[Firm]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firm]') AND type in (N'U'))
DROP TABLE [dbo].[Firm]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND type in (N'U'))
DROP TABLE [dbo].[Order]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
DROP TABLE [dbo].[Product]
GO
/****** Object:  Table [dbo].[Server]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server]') AND type in (N'U'))
DROP TABLE [dbo].[Server]
GO
/****** Object:  Table [dbo].[Session]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Session]') AND type in (N'U'))
DROP TABLE [dbo].[Session]
GO
/****** Object:  Table [dbo].[Strategy]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Strategy]') AND type in (N'U'))
DROP TABLE [dbo].[Strategy]
GO
/****** Object:  Table [dbo].[Trade]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trade]') AND type in (N'U'))
DROP TABLE [dbo].[Trade]
GO
/****** Object:  Table [dbo].[TradeVenueDest]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TradeVenueDest]') AND type in (N'U'))
DROP TABLE [dbo].[TradeVenueDest]
GO
/****** Object:  Table [dbo].[User]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
DROP TABLE [dbo].[User]
GO
/****** Object:  Table [dbo].[Venue]    Script Date: 10/20/2011 13:45:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Venue]') AND type in (N'U'))
DROP TABLE [dbo].[Venue]
GO
/****** Object:  Table [dbo].[Venue]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Venue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Venue](
	[AccountNumber] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BeginString] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CancelBag] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Code] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VenueCode] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[TargetVenue] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DataFeedVenue] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DriverCode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[NOSBag] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Name] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReplaceBag] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DefaultCurrencyCode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DefaultSecurityExchange] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DefaultCFICode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UseSymbol] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Venue] PRIMARY KEY CLUSTERED 
(
	[VenueCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[User]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[User](
	[Enabled] [bit] NOT NULL,
	[ID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UserPwd] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UserID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[K2Config] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_dbo.User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[TradeVenueDest]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TradeVenueDest]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TradeVenueDest](
	[ExDestination] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExchangeCode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PrimaryCFICode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VenueCode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
END
GO
/****** Object:  Table [dbo].[Trade]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Trade]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Trade](
	[Identity] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TradeID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[MatchID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TradingSessionID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TradeInputSource] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[HandlInst] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VenueType] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExecutingBrokerCode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ClearingBrokerCode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Trader] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ClearingDate] [datetime] NOT NULL,
	[BusinessDate] [datetime] NOT NULL,
	[SessionID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SessionSubID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VenueCode] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProductID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Src] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IDSrc] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Sym] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Exch] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[MMY] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Strk] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CFI] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Mnemonic] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Account] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ClOrdID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrigClOrdID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrderID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Side] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrdType] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Quantity] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Price] [float] NULL,
	[StopPx] [float] NULL,
	[OrdStatus] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LeavesQty] [float] NULL,
	[CumQty] [float] NULL,
	[LastPx] [float] NULL,
	[LastQty] [float] NULL,
	[AvgPx] [float] NULL,
	[Text] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TransactTime] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Tag] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CorrelationID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExecutionID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TimeInForce] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PositionEffect] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_dbo.Trade] PRIMARY KEY CLUSTERED 
(
	[TradeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Strategy]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Strategy]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Strategy](
	[LastOrdIdentity] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UserID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Identity] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[AutoCreate] [bit] NOT NULL,
	[ProductID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Mnemonic] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DataMnemonic] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Side] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ShortSaleLocate] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrdType] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TimeInForce] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TIFDateTime] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[QtyLimit] [float] NOT NULL,
	[MaxIterations] [int] NOT NULL,
	[MaxEntries] [int] NOT NULL,
	[MaxPrice] [float] NOT NULL,
	[MinPrice] [float] NOT NULL,
	[Qty] [float] NOT NULL,
	[MaxFloor] [float] NULL,
	[Price] [float] NOT NULL,
	[FlattenOnExit] [bit] NOT NULL,
	[CancelOnExit] [bit] NOT NULL,
	[UseStrategyTimes] [bit] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[UserName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StopPx] [float] NOT NULL,
	[Account] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Type] [int] NOT NULL,
	[PXAlgorithmName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TriggerName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EntryTriggerName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AutoConnectTrg] [bit] NOT NULL,
	[ConditionInterval] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[ORAlgorithmName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ParameterBag] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[RunIdentifier] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CorrelationID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TSQueryGroupPath] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Info] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[State] [int] NOT NULL,
	[Initialized] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Strategy] PRIMARY KEY CLUSTERED 
(
	[Identity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Session]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Session]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Session](
	[Identity] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UserID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CorrelationID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
END
GO
/****** Object:  Table [dbo].[Server]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Server]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Server](
	[Enabled] [bit] NOT NULL,
	[ID] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[InstanceNumber] [bigint] NOT NULL,
	[MachineName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Name] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ServerRole] [int] NOT NULL,
	[Running] [bit] NOT NULL,
	[startTimeTicks] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Server] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Product]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Product](
	[CFICode] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Currency] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DriverID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Exchange] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IDSource] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Identity] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LongName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Commodity] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[MMY] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[GenericName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Mnemonic] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[QtyIncrement] [int] NOT NULL,
	[SecurityID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StrikePrice] [decimal](29, 4) NULL,
	[Symbol] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Tag] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TickSize] [decimal](29, 4) NULL,
	[TickValue] [decimal](29, 4) NULL,
	[NumberDecimalPlaces] [int] NOT NULL,
	[PriceFeedQuantityMultiplier] [int] NOT NULL,
	[ContractSize] [decimal](29, 4) NULL,
	[TradeVenue] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BrokerService] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExDestination] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DepthLevelCount] [int] NOT NULL,
	[SyntheticPriceCalcName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_dbo.Product] PRIMARY KEY CLUSTERED 
(
	[Mnemonic] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Order]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Order](
	[UserID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CorrelationID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Account] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AutoTradeProcessCount] [int] NOT NULL,
	[AvgPx] [float] NOT NULL,
	[ClOrdID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CumQty] [float] NOT NULL,
	[ExtendedOrdType] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[HandlInst] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Identity] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IsAutoTrade] [bit] NOT NULL,
	[LastPx] [float] NOT NULL,
	[LastQty] [float] NOT NULL,
	[LeavesQty] [float] NOT NULL,
	[LocateReqd] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LocationID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[MaxFloor] [bigint] NOT NULL,
	[Mnemonic] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OCAGroupName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrdStatus] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrdType] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrderID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrderQty] [bigint] NOT NULL,
	[OrigClOrdID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ParentIdentity] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Price] [float] NOT NULL,
	[QuantityLimit] [float] NOT NULL,
	[ShortSaleLocate] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Side] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StopPx] [float] NOT NULL,
	[StrategyName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AlgoName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Tag] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Text] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TimeInForce] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TradeVenue] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TransactTime] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TriggerOrderID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_dbo.Order] PRIMARY KEY CLUSTERED 
(
	[Identity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Firm]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Firm]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Firm](
	[ID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FirmCode] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FirmName] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FirmType] [int] NOT NULL,
	[External] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Firm] PRIMARY KEY CLUSTERED 
(
	[FirmCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Fill]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Fill]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Fill](
	[OrderStatus] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AvgPx] [float] NOT NULL,
	[LeavesQty] [float] NOT NULL,
	[CumQty] [float] NOT NULL,
	[Ticks] [bigint] NOT NULL,
	[ExecReport] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FillQty] [float] NOT NULL,
	[LastPx] [float] NOT NULL,
	[OrderID] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Identity] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_dbo.Fill] PRIMARY KEY CLUSTERED 
(
	[Identity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Exchange]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exchange]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Exchange](
	[ExchangeCode] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Name] [nvarchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_dbo.Exchange] PRIMARY KEY CLUSTERED 
(
	[ExchangeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[Account]    Script Date: 10/20/2011 13:45:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Account]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Account](
	[Code] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LongName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FirmCode] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Identity] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[VenueCode] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[AccountType] [int] NOT NULL,
	[InitialMargin] [decimal](29, 4) NOT NULL,
	[MaintMargin] [decimal](29, 4) NOT NULL,
	[NetLiquidity] [decimal](29, 4) NOT NULL,
	[AvailableFunds] [decimal](29, 4) NOT NULL,
	[ExcessFunds] [decimal](29, 4) NOT NULL,
 CONSTRAINT [PK_dbo.Account] PRIMARY KEY CLUSTERED 
(
	[Identity] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
