CREATE TABLE [vc].[ObjectDropedPeriodRange] (
    [Id]            INT      IDENTITY (1, 1) NOT NULL,
    [ObjectId]      INT      NOT NULL,
    [StartDatetime] DATETIME NOT NULL,
    [EndDatetime]   DATETIME NULL,
    CONSTRAINT [PK__ObjectDr__3214EC0733AA9866] PRIMARY KEY CLUSTERED ([Id] ASC)
);

