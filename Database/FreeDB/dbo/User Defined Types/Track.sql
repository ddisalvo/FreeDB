CREATE TYPE [dbo].[Track] AS TABLE (
    [DiscId]      BIGINT        NOT NULL,
    [Title]       VARCHAR (255) NOT NULL,
    [TrackNumber] INT           NOT NULL,
    [Offset]      INT           NOT NULL);

