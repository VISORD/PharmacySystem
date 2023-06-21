CREATE TABLE [order].[OrderHistory] (
    [Id]          INT                  IDENTITY (1, 1) NOT NULL,
    [OrderId]     INT                  NOT NULL,
    [Timestamp]   DATETIMEOFFSET (0)   CONSTRAINT [DF_OrderHistory_Timestamp] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [Event]       NVARCHAR (1024)      NOT NULL,
    CONSTRAINT [PK_OrderHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderHistory_Order] FOREIGN KEY ([OrderId]) REFERENCES [order].[Order] ([Id]) ON DELETE CASCADE,
);
GO;

CREATE NONCLUSTERED INDEX [IX_OrderHistory_OrderId]
    ON [order].[OrderHistory] ([OrderId] DESC);
GO;
