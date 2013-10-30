namespace FreeDB.Core.Model
{
    using Bases;

    public class Track : PersistentObject<long>
    {
        public virtual string Title { get; set; }
        public virtual int TrackNumber { get; set; }
        public virtual int Offset { get; set; }
    }
}
