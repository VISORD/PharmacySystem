CREATE TYPE [pharmacy].[PharmacyMedicamentRate] AS TABLE (
    [PharmacyId]     INT     NOT NULL,
    [MedicamentId]   INT     NOT NULL,
    [StartDate]      DATE    NOT NULL,
    [StopDate]       DATE    NOT NULL,
    [RetailPrice]    MONEY   NOT NULL
);
GO;
