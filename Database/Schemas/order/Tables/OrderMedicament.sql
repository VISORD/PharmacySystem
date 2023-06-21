CREATE TABLE [order].[OrderMedicament] (
    [OrderId]          INT   NOT NULL,
    [MedicamentId]     INT   NOT NULL,
    [RequestedCount]   INT   NOT NULL,
    [ApprovedCount]    INT   NULL,
    [IsApproved]       BIT   CONSTRAINT [DF_OrderMedicament_IsApproved] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_OrderMedicament] PRIMARY KEY CLUSTERED ([OrderId] ASC, [MedicamentId] ASC),
    CONSTRAINT [FK_OrderMedicament_Order] FOREIGN KEY ([OrderId]) REFERENCES [order].[Order] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderMedicament_Medicament] FOREIGN KEY ([MedicamentId]) REFERENCES [medicament].[Medicament] ([Id]) ON DELETE CASCADE,
);
GO;
