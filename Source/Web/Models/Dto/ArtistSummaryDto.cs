namespace FreeDB.Web.Models.Dto
{
    using System.Runtime.Serialization;

    [DataContract(Name = "ArtistSummary")]
    public class ArtistSummaryDto : LinkedResource
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int NumberOfDiscs { get; set; }
    }
}