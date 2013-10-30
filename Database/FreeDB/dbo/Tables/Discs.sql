CREATE TABLE [dbo].[Discs] (
    [DiscId]          BIGINT         NOT NULL,
    [ArtistId]        INT            NULL,
    [GenreId]         INT            NULL,
    [Title]           NVARCHAR (255) NOT NULL,
    [Released]        DATE           NULL,
    [LengthInSeconds] INT            NOT NULL,
    [Language]        NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Discs] PRIMARY KEY CLUSTERED ([DiscId] ASC),
    CONSTRAINT [FK_Discs_Artists] FOREIGN KEY ([ArtistId]) REFERENCES [dbo].[Artists] ([ArtistId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Discs_Genres] FOREIGN KEY ([GenreId]) REFERENCES [dbo].[Genres] ([GenreId])
);












GO
CREATE NONCLUSTERED INDEX [IDX_Disc_Name]
    ON [dbo].[Discs]([Title] ASC)
    INCLUDE([ArtistId]);


GO
CREATE NONCLUSTERED INDEX [IDX_Disc_ArtistId]
    ON [dbo].[Discs]([ArtistId] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_Disc_Released]
    ON [dbo].[Discs]([Released] ASC)
    INCLUDE([ArtistId]);

