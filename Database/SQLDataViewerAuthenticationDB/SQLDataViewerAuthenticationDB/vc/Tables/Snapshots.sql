CREATE TABLE [vc].[Snapshots] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (200) NOT NULL,
    [Description]      VARCHAR (400) NOT NULL,
    [SnapshotDatetime] DATETIME      NOT NULL,
    [MaxVersionId]     INT           NOT NULL,
    CONSTRAINT [PK__Snapshot__3214EC072F4FF79D] PRIMARY KEY CLUSTERED ([Id] ASC)
);

