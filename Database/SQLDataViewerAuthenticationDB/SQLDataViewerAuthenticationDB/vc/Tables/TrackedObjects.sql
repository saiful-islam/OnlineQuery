CREATE TABLE [vc].[TrackedObjects] (
    [ObjectId]       INT            IDENTITY (10000, 1) NOT NULL,
    [SchemaName]     VARCHAR (100)  NOT NULL,
    [ObjectName]     VARCHAR (1000) NOT NULL,
    [LastUpdateDate] DATETIME       NOT NULL,
    CONSTRAINT [PK__TrackedO__9A61929157DD0BE4] PRIMARY KEY CLUSTERED ([ObjectId] ASC)
);

