namespace FreeDB.Web.Models.Dto
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Track")]
    public class TrackDto
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public int TrackNumber { get; set; }

        [DataMember]
        public int Offset { get; set; }

        [DataMember]
        public string Runtime { get; set; }
    }
}