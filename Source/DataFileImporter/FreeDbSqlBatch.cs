namespace FreeDB.DataFileImporter
{
    using System.Data;
    using System.Linq;
    using Core.Common;
    using Core.Model;

    internal class FreeDbSqlBatch
    {
        public DataTable Discs { get; private set; }
        public DataTable Tracks { get; private set; }

        public FreeDbSqlBatch()
        {
            Discs = new DataTable();
            Discs.Columns.AddRange(
                new[] { "DiscId", "Artist", "Genre", "Title", "Released", "LengthInSeconds", "Language" }.Select(
                    s => new DataColumn(s)).ToArray());

            Tracks = new DataTable();
            Tracks.Columns.AddRange(
                new[] { "DiscId", "Title", "TrackNumber", "Offset" }.Select(s => new DataColumn(s)).ToArray());
        }

        public void Add(Disc disc)
        {
            if (Discs.AsEnumerable().Any(r => long.Parse(r["DiscId"].ToString()) == disc.Id))
                return;

            Discs.Rows.Add(disc.Id, disc.Artist != null ? disc.Artist.Name.TruncateAndAddEllipsis(255) : null,
                           disc.Genre != null ? disc.Genre.Title.TruncateAndAddEllipsis(100) : null,
                           disc.Title.TruncateAndAddEllipsis(255), disc.Released, disc.LengthInSeconds,
                           disc.Language != null ? disc.Language.TruncateAndAddEllipsis(100) : null);

            disc.Tracks.ToList()
                .ForEach(t => Tracks.Rows.Add(disc.Id, t.Title.TruncateAndAddEllipsis(255), t.TrackNumber, t.Offset));
        }
    }
}
