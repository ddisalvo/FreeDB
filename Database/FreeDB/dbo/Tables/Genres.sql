CREATE TABLE [dbo].[Genres] (
    [GenreId] INT            IDENTITY (1, 1) NOT NULL,
    [Title]   NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Genres] PRIMARY KEY CLUSTERED ([GenreId] ASC),
    CONSTRAINT [UNQ_Genres_Title] UNIQUE NONCLUSTERED ([Title] ASC)
);



