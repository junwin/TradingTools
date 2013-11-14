CREATE TABLE [dbo].[MQRoutingKey] (
    [Enabled]   BIT            NOT NULL,
    [Exchange]  NVARCHAR (256) NULL,
    [QueueName] NVARCHAR (256) NULL,
    [Key]       NVARCHAR (512) NULL,
    [Name]      NVARCHAR (256) NULL,
    [Type]      NVARCHAR (256) NULL
);

