CREATE TYPE [dbo].[Disc] AS TABLE (
    [DiscId]          BIGINT         NOT NULL,
    [Artist]          NVARCHAR (255) NULL,
    [Genre]           NVARCHAR (100) NULL,
    [Title]           NVARCHAR (255) NOT NULL,
    [Released]        DATE           NULL,
    [LengthInSeconds] INT            NOT NULL,
    [Language]        NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([DiscId] ASC));

