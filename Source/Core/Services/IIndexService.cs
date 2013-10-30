namespace FreeDB.Core.Services
{
    using System.Collections.Generic;

    public interface IIndexService
    {
        void Add(IEnumerable<dynamic> data);
    }
}
