CREATE TABLE [dbo].[tblDBTable] (
    [Id]            INT            NOT NULL,
    [DBTableOrView] NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_tblDBTable] PRIMARY KEY CLUSTERED ([Id] ASC)
);

