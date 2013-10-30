namespace FreeDB.Web.Models.Dto
{
    using System.Runtime.Serialization;
    using System.Web.Http.OData;

    [DataContract(Name = "Artist")]
    public class ArtistDto : ArtistSummaryDto
    {
        [DataMember]
        public PageResult<DiscDto> Discs { get; set; }
    }
}