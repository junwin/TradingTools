CREATE TABLE [dbo].[User] (
    [Enabled]    BIT             NOT NULL,
    [ID]         NVARCHAR (255)  NOT NULL,
    [UserPwd]    NVARCHAR (255)  NOT NULL,
    [UserID]     NVARCHAR (255)  NOT NULL,
    [K2Config]   NVARCHAR (4000) NULL,
    [LastSignIn] DATETIME        NOT NULL,
    [LastIP]     NVARCHAR (32)   NULL,
    [IsSignedIn] BIT             NOT NULL,
    [Email]      NVARCHAR (255)  NULL
);

