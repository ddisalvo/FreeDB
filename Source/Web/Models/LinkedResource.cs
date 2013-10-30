
namespace FreeDB.Web.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class LinkedResource
    {
        [DataMember]
        public string HRef { get; set; }
    }
}