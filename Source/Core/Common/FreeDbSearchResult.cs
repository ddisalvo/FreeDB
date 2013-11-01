namespace FreeDB.Core.Common
{
    using System;

    public class FreeDbSearchResult
    {
        public long? DiscId { get; set; }
        public string DiscTitle { get; set; }
        public int DiscLengthInSeconds { get; set; }
        public DateTime? DiscReleaseDate { get; set; }
        public int? ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string[] Tracks { get; set; }

        public FreeDbSearchResult()
        {
            Tracks = new string[0];
        }
    }
}
