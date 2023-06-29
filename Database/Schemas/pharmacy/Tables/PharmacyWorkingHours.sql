CREATE TABLE [pharmacy].[PharmacyWorkingHours] (
    [PharmacyId]   INT        NOT NULL,
    [Weekday]      TINYINT    NOT NULL,
    [StartTime]    TIME (0)   CONSTRAINT [DF_PharmacyWorkingHours_StartTime] DEFAULT ('00:00:00') NOT NULL,
    [StopTime]     TIME (0)   CONSTRAINT [DF_PharmacyWorkingHours_StopTime] DEFAULT ('00:00:00') NOT NULL,
    CONSTRAINT [PK_PharmacyWorkingHours] PRIMARY KEY CLUSTERED ([PharmacyId] ASC, [Weekday] ASC),
    CONSTRAINT [FK_PharmacyWorkingHours_Pharmacy] FOREIGN KEY ([PharmacyId]) REFERENCES [pharmacy].[Pharmacy] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_PharmacyWorkingHours_Weekday] CHECK ([Weekday] >= 0 AND [Weekday] <= 6),
);
GO;
