namespace FreeDB.Infrastructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Bases;
    using Core.Services;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Version = Lucene.Net.Util.Version;

    public class IndexService : BaseLuceneService, IIndexService
    {
        public void Add(IEnumerable<dynamic> data)
        {
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    foreach (var item in data)
                    {
                        AddToIndex(item, writer);
                    }
                }
            }
        }

        private static void AddToIndex(dynamic item, IndexWriter writer)
        {
            var propertyInfo = item.GetType().GetProperties();

            var doc = new Document();

            foreach (PropertyInfo property in propertyInfo)
            {
                var value = property.GetValue(item);
                if (value == null) 
                    continue;

                if (property.PropertyType != typeof (string) &&
                    typeof (IEnumerable).IsAssignableFrom(property.PropertyType))
                    value = String.Join("`~", value);

                doc.Add(new Field(property.Name, value.ToString(), Field.Store.YES,
                                  property.Name.ToUpper().Contains("ID")
                                      ? Field.Index.NOT_ANALYZED
                                      : Field.Index.ANALYZED));
            }

            writer.AddDocument(doc);
        }
    }
}
