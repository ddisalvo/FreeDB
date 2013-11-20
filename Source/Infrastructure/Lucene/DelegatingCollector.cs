//from https://github.com/devhost/Corelicious

namespace FreeDB.Infrastructure.Lucene
{
    using System;
    using global::Lucene.Net.Index;
    using global::Lucene.Net.Search;

    public class DelegatingCollector : Collector
    {
        private readonly Action<IndexReader, Int32, Scorer> _method;
        private IndexReader _reader;
        private Scorer _scorer;

        public DelegatingCollector(Action<IndexReader, Int32> method)
        {
            _method = (reader, doc, scorer) => method(reader, doc);
        }

        public DelegatingCollector(Action<IndexReader, Int32, Scorer> method)
        {
            _method = method;
        }

        public override void SetScorer(Scorer scorer)
        {
            _scorer = scorer;
        }

        public override void Collect(Int32 doc)
        {
            _method(_reader, doc, _scorer);
        }

        public override void SetNextReader(IndexReader reader, Int32 docBase)
        {
            _reader = reader;
        }

        public override Boolean AcceptsDocsOutOfOrder
        {
            get { return true; }
        }
    }
}
