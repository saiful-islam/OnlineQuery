CREATE TABLE [dbo].[MapUserConnection] (
    [Id]     INT NOT NULL,
    [ConnID] INT NOT NULL,
    [UserID] INT NOT NULL,
    CONSTRAINT [PK_MapUserConnection] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MapUserConnection_MapConnection] FOREIGN KEY ([ConnID]) REFERENCES [dbo].[tblConnection] ([ConnId]),
    CONSTRAINT [FK_MapUserConnection_tblUserInfo] FOREIGN KEY ([UserID]) REFERENCES [dbo].[UserProfile] ([UserId])
);



