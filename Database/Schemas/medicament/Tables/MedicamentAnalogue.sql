CREATE TABLE [medicament].[MedicamentAnalogue] (
    [OriginalId]      INT   NOT NULL,
    [AnalogueId]      INT   NOT NULL,
    CONSTRAINT [PK_MedicamentAnalogue] PRIMARY KEY CLUSTERED ([OriginalId] ASC, [AnalogueId] ASC),
    CONSTRAINT [FK_MedicamentAnalogue_OriginalMedicament] FOREIGN KEY ([OriginalId]) REFERENCES [medicament].[Medicament] ([Id]),
    CONSTRAINT [FK_MedicamentAnalogue_AnalogueMedicament] FOREIGN KEY ([AnalogueId]) REFERENCES [medicament].[Medicament] ([Id]),
    CONSTRAINT [CK_MedicamentAnalogue_Medicaments] CHECK ([OriginalId] <> [AnalogueId]),
);
GO;
