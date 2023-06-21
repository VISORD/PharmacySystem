CREATE TABLE [pharmacy].[PharmacyWorkingHours] (
    [PharmacyId]   INT        NOT NULL,
    [Weekday]      TINYINT    NOT NULL,
    [StartTime]    TIME (0)   NOT NULL,
    [StopTime]     TIME (0)   NOT NULL,
    CONSTRAINT [PK_PharmacyWorkingHours] PRIMARY KEY CLUSTERED ([PharmacyId] ASC, [Weekday] ASC),
    CONSTRAINT [FK_PharmacyWorkingHours_PharmacyId] FOREIGN KEY ([PharmacyId]) REFERENCES [pharmacy].[Pharmacy] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_PharmacyWorkingHours_Weekday] CHECK ([Weekday] >= 0 AND [Weekday] <= 6),
);
GO;
