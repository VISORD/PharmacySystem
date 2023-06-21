CREATE TABLE [pharmacy].[PharmacyMedicament] (
    [PharmacyId]       INT   NOT NULL,
    [MedicamentId]     INT   NOT NULL,
    [QuantityOnHand]   INT   CONSTRAINT [DF_PharmacyMedicament_QuantityOnHand] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_PharmacyMedicament] PRIMARY KEY CLUSTERED ([PharmacyId] ASC, [MedicamentId] ASC),
    CONSTRAINT [FK_PharmacyMedicament_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [pharmacy].[Pharmacy] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PharmacyMedicament_Medicament] FOREIGN KEY ([MedicamentId]) REFERENCES [medicament].[Medicament] ([Id]) ON DELETE CASCADE,
);
GO;
