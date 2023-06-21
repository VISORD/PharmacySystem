CREATE TABLE [company].[Company] (
    [Id]         INT              IDENTITY (1, 1) NOT NULL,
    [Email]      NVARCHAR (225)   NOT NULL,
    [Name]       NVARCHAR (50)    NOT NULL,
    [Phone]      VARCHAR (32)     NULL,
    [Password]   NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([Id] ASC),
);
GO;
