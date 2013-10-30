CREATE PROCEDURE [dbo].[InsertDisc] 
	@Discs	Disc Readonly,
	@Tracks Track Readonly
AS
BEGIN

	SET NOCOUNT ON;

	insert into dbo.Genres(Title)
		select distinct(d.Genre)
		from @Discs as d
			left join dbo.Genres as g on g.Title=d.Genre
		where g.GenreId is null and d.Genre is not null
	
	insert into dbo.Artists(Name)
		select distinct(d.Artist)
		from @Discs as d
			left join dbo.Artists as a on a.Name=d.Artist
		where a.ArtistId is null and d.Artist is not null

	insert into dbo.Discs(DiscId, ArtistId, GenreId, Title, Released, LengthInSeconds, Language)
		select
			d1.DiscId,
			a.ArtistId,
			g.GenreId,
			d1.Title,
			d1.Released,
			d1.LengthInSeconds,
			d1.Language
		from @Discs as d1
			left join dbo.Discs as d2 on d2.DiscId=d1.DiscId
			left join dbo.Artists as a on a.Name=d1.Artist
			left join dbo.Genres as g on g.Title=d1.Genre
		where d2.DiscId is null
	
	insert into Tracks(DiscId, Title, TrackNumber, Offset)
		select
			t1.DiscId, 
			t1.Title, 
			t1.TrackNumber, 
			t1.Offset
		from @Tracks as t1
			left join Tracks as t2 on t2.DiscId=t1.DiscId and t2.TrackNumber=t1.TrackNumber
		where t2.TrackId is null
	
END