CREATE TABLE [order].[Order] (
    [Id]           INT                  IDENTITY (1, 1) NOT NULL,
    [PharmacyId]   INT                  NOT NULL,
    [StatusId]     TINYINT              CONSTRAINT [DF_Order_StatusId] DEFAULT (0) NOT NULL,
    [UpdatedAt]    DATETIMEOFFSET (0)   CONSTRAINT [DF_Order_UpdatedAt] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [OrderedAt]    DATETIMEOFFSET (0)   NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Order_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [pharmacy].[Pharmacy] ([Id]) ON DELETE CASCADE,
);
GO;

CREATE NONCLUSTERED INDEX [IX_Order_PharmacyId]
    ON [order].[Order] ([PharmacyId] ASC);
GO;
