CREATE TABLE [dbo].[Tracks] (
    [TrackId]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [DiscId]      BIGINT         NOT NULL,
    [Title]       NVARCHAR (255) NOT NULL,
    [TrackNumber] INT            NOT NULL,
    [Offset]      INT            NOT NULL,
    CONSTRAINT [PK_Tracks] PRIMARY KEY CLUSTERED ([TrackId] ASC),
    CONSTRAINT [FK_Tracks_Discs] FOREIGN KEY ([DiscId]) REFERENCES [dbo].[Discs] ([DiscId]) ON DELETE CASCADE,
    CONSTRAINT [UNQ_Tracks_Disc_TrackNumber] UNIQUE NONCLUSTERED ([DiscId] ASC, [TrackNumber] ASC)
);



