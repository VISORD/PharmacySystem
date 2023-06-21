CREATE TABLE [pharmacy].[PharmacyMedicamentRate] (
    [PharmacyId]     INT     NOT NULL,
    [MedicamentId]   INT     NOT NULL,
    [StartDate]      DATE    CONSTRAINT [DF_PharmacyMedicamentRate_StartDate] DEFAULT ('2000-01-01') NOT NULL,
    [StopDate]       DATE    CONSTRAINT [DF_PharmacyMedicamentRate_StopDate] DEFAULT ('2199-12-31') NOT NULL,
    [RetailPrice]    MONEY   NOT NULL,
    CONSTRAINT [PK_PharmacyMedicamentRate] PRIMARY KEY CLUSTERED ([PharmacyId] ASC, [MedicamentId] ASC, [StartDate] DESC, [StopDate] DESC),
    CONSTRAINT [FK_PharmacyMedicamentRate_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [pharmacy].[Pharmacy] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PharmacyMedicamentRate_Medicament] FOREIGN KEY ([MedicamentId]) REFERENCES [medicament].[Medicament] ([Id]) ON DELETE CASCADE,
);
GO;
