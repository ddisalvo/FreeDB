CREATE TABLE [dbo].[Artists] (
    [ArtistId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_Artists] PRIMARY KEY CLUSTERED ([ArtistId] ASC),
    CONSTRAINT [UNQ_Artists_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

