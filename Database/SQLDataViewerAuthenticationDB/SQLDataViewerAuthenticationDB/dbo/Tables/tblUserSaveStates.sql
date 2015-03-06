CREATE TABLE [dbo].[tblUserSaveStates] (
    [StateName]    NVARCHAR (50)  NOT NULL,
    [ConnectionID] INT            NOT NULL,
    [HTMLDOC]      NVARCHAR (MAX) NOT NULL,
    [Attributes]   NVARCHAR (MAX) NOT NULL,
    [Columns]      NVARCHAR (MAX) NOT NULL,
    [TableName]    NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_tblUserSaveStates_1] PRIMARY KEY CLUSTERED ([StateName] ASC, [ConnectionID] ASC)
);

