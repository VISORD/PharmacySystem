CREATE TABLE [medicament].[Medicament] (
    [Id]            INT               IDENTITY (1, 1) NOT NULL,
    [CompanyId]     INT               NOT NULL,
    [Name]          NVARCHAR (100)    NOT NULL,
    [Description]   NVARCHAR (1024)   NULL,
    [VendorPrice]   MONEY             NOT NULL,
    CONSTRAINT [PK_Medicament] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Medicament_Company] FOREIGN KEY ([CompanyId]) REFERENCES [company].[Company] ([Id]),
);
GO;

CREATE NONCLUSTERED INDEX [IX_Medicament_CompanyId]
    ON [medicament].[Medicament] ([CompanyId] ASC);
GO;
