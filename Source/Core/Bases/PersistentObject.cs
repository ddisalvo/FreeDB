namespace FreeDB.Core.Bases
{
    using System;

    [Serializable]
    public abstract class PersistentObject<T> : PersistentObject
    {
        public new virtual T Id
        {
            get { return (T)base.Id; }
            set { base.Id = value; }
        }

        protected override object GetEmptyId()
        {
            return default(T);
        }
    }

    [Serializable]
    public abstract class PersistentObject
    {
        public const string ID = "Id";
        public virtual object Id { get; set; }

        protected PersistentObject()
        {
            Id = GetEmptyId();
        }

        public virtual bool IsPersistent
        {
            get { return IsPersistentObject(); }
        }

        public override bool Equals(object obj)
        {
            if (IsPersistentObject())
            {
                var persistentObject = obj as PersistentObject;
                return (persistentObject != null) && (IdsAreEqual(persistentObject));
            }

            return base.Equals(obj);
        }

        protected bool IdsAreEqual(PersistentObject persistentObject)
        {
            return Equals(Id, persistentObject.Id);
        }

        public override int GetHashCode()
        {
            return IsPersistentObject() ? Id.GetHashCode() : base.GetHashCode();
        }

        private bool IsPersistentObject()
        {
            return !Equals(Id, GetEmptyId());
        }

        protected abstract object GetEmptyId();
    }
}
