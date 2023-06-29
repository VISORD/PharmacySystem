CREATE TYPE [pharmacy].[PharmacyWorkingHours] AS TABLE (
    [PharmacyId]   INT        NOT NULL,
    [Weekday]      TINYINT    NOT NULL,
    [StartTime]    TIME (0)   NOT NULL,
    [StopTime]     TIME (0)   NOT NULL
);
GO;
