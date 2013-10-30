namespace FreeDB.Core.Model
{
    using System;
    using System.Collections.Generic;
    using Bases;

    public class Disc : PersistentObject<long>
    {
        public virtual Artist Artist { get; set; }
        public virtual string Title { get; set; }
        public virtual ICollection<Track> Tracks { get; private set; }
        public virtual Genre Genre { get; set; }
        public virtual DateTime? Released { get; set; }
        public virtual int LengthInSeconds { get; set; }
        public virtual string Language { get; set; }

        public Disc()
        {
            Tracks = new List<Track>();
        }
    }
}
