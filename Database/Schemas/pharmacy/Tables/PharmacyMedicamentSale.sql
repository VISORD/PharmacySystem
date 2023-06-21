CREATE TABLE [pharmacy].[PharmacyMedicamentSale] (
    [PharmacyId]     INT                  NOT NULL,
    [MedicamentId]   INT                  NOT NULL,
    [SoldAt]         DATETIMEOFFSET (0)   CONSTRAINT [DF_PharmacyMedicamentSale_SoldAt] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [SalePrice]      MONEY                NOT NULL,
    [UnitsSold]      INT                  NOT NULL,
    CONSTRAINT [PK_PharmacyMedicamentSale] PRIMARY KEY CLUSTERED ([PharmacyId] ASC, [MedicamentId] ASC, [SoldAt] ASC),
    CONSTRAINT [FK_PharmacyMedicamentSale_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [pharmacy].[Pharmacy] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PharmacyMedicamentSale_Medicament] FOREIGN KEY ([MedicamentId]) REFERENCES [medicament].[Medicament] ([Id]) ON DELETE CASCADE,
);
GO;
