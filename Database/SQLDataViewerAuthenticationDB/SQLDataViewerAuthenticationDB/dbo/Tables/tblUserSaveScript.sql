CREATE TABLE [dbo].[tblUserSaveScript] (
    [StateName]    NVARCHAR (50)  NOT NULL,
    [ConnectionID] INT            NOT NULL,
    [Script]       NVARCHAR (MAX) NULL,
    [IsActive]     BIT            NULL,
    CONSTRAINT [PK_tblUserSaveScript] PRIMARY KEY CLUSTERED ([StateName] ASC, [ConnectionID] ASC)
);

