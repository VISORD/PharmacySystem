CREATE TABLE [medicament].[Medicament] (
    [Id]            INT               IDENTITY (1, 1) NOT NULL,
    [CompanyId]     INT               NOT NULL,
    [Name]          NVARCHAR (100)    NOT NULL,
    [Description]   NVARCHAR (1024)   NULL,
    [Price]         MONEY             NOT NULL,
    CONSTRAINT [PK_Medicament] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Medicament_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [company].[Company] ([Id]) ON DELETE CASCADE
);
GO;
