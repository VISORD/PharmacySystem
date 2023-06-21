CREATE TABLE [pharmacy].[Pharmacy] (
    [Id]            INT               IDENTITY (1, 1) NOT NULL,
    [CompanyId]     INT               NOT NULL,
    [Name]          NVARCHAR (50)     NOT NULL,
    [Email]         NVARCHAR (255)    NULL,
    [Phone]         VARCHAR (32)      NULL,
    [Address]       NVARCHAR (512)    NOT NULL,
    [Latitude]      DECIMAL (9, 6)    NOT NULL,
    [Longitude]     DECIMAL (9, 6)    NOT NULL,
    [Description]   NVARCHAR (1024)   NULL,
    CONSTRAINT [PK_Pharmacy] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pharmacy_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [company].[Company] ([Id]) ON DELETE CASCADE
);
GO;
