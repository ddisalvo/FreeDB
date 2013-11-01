namespace FreeDB.Web.Models.Dto
{
    using System.Runtime.Serialization;

    [DataContract(Name = "DiscSummary")]
    public class DiscSummaryDto : LinkedResource
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string ArtistName { get; set; }

        [DataMember]
        public string ArtistHRef { get; set; }

        [DataMember]
        public int? ArtistId { get; set; }

        [DataMember]
        public int NumberOfTracks { get; set; }

        [DataMember]
        public string Runtime { get; set; }

        [DataMember]
        public string Released { get; set; }

        [DataMember]
        public string Genre { get; set; }
    }
}