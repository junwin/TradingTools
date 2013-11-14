CREATE TABLE [dbo].[MQRoutingKey] (
    [Enabled]  BIT            NOT NULL,
    [Exchange] NVARCHAR (255) NULL,
    [Key]      NVARCHAR (512) NULL,
    [Name]     NVARCHAR (255) NULL,
    [Type]     NVARCHAR (255) NULL
);

