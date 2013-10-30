namespace FreeDB.Core.Model
{
    using Bases;

    public class Artist : PersistentObject<int>
    {
        public virtual string Name { get; set; }
    }
}
