namespace FreeDB.Core.Model
{
    using Bases;

    public class Genre : PersistentObject<int>
    {
        public virtual string Title { get; set; }
    }
}
