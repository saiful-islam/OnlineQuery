CREATE TABLE [dbo].[tblConnection] (
    [ConnId]         INT            NOT NULL,
    [ConnectionName] NVARCHAR (200) NULL,
    [DBServerId]     INT            NOT NULL,
    [DBId]           INT            NOT NULL,
    [DBSchemaId]     INT            NOT NULL,
    [DBTableId]      INT            NOT NULL,
    CONSTRAINT [PK_tblAuthentication] PRIMARY KEY CLUSTERED ([ConnId] ASC),
    CONSTRAINT [FK_tblAuthentication_tblDBName] FOREIGN KEY ([DBId]) REFERENCES [dbo].[tblDBName] ([Id]),
    CONSTRAINT [FK_tblAuthentication_tblDBSchema] FOREIGN KEY ([DBSchemaId]) REFERENCES [dbo].[tblDBSchema] ([Id]),
    CONSTRAINT [FK_tblAuthentication_tblDBServer] FOREIGN KEY ([DBServerId]) REFERENCES [dbo].[tblDBServer] ([Id]),
    CONSTRAINT [FK_tblAuthentication_tblDBTable] FOREIGN KEY ([DBTableId]) REFERENCES [dbo].[tblDBTable] ([Id])
);

