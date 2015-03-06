CREATE TABLE [vc].[ObjectVersions] (
    [VersionId]       INT             IDENTITY (1, 1) NOT NULL,
    [FirstVersionId]  INT             NULL,
    [PrevVersionId]   INT             NULL,
    [NextVersionId]   INT             NULL,
    [VersionDatetime] DATETIME        NOT NULL,
    [RevisionNo]      INT             CONSTRAINT [DF_ObjectVersions_RevisionNo] DEFAULT ((1)) NOT NULL,
    [Comment]         NVARCHAR (MAX)  NULL,
    [ObjectId]        INT             NOT NULL,
    [ObjectType]      VARCHAR (200)   NOT NULL,
    [SchemaName]      VARCHAR (100)   NOT NULL,
    [ObjectName]      VARCHAR (1000)  NOT NULL,
    [ObjectScript]    NVARCHAR (MAX)  NOT NULL,
    [ObjectScriptXml] AS              ([vc].[GetXmlFromScript]([ObjectScript])),
    [HostName]        NVARCHAR (128)  NOT NULL,
    [LoginName]       NVARCHAR (128)  NOT NULL,
    [ProgramName]     NVARCHAR (1000) NOT NULL,
    [NetAddress]      NCHAR (12)      NOT NULL,
    [NetLibrary]      NCHAR (12)      NOT NULL,
    [SpId]            SMALLINT        NOT NULL,
    CONSTRAINT [PK__ObjectVe__16C6400F5BAD9CC8] PRIMARY KEY CLUSTERED ([VersionId] ASC, [ObjectId] ASC)
);

