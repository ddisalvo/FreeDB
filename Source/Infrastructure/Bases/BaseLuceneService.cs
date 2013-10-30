namespace FreeDB.Infrastructure.Bases
{
    using System;
    using System.IO;
    using Lucene.Net.Store;
    using Properties;

    public abstract class BaseLuceneService : IDisposable
    {
        protected FSDirectory Directory { get; private set; }
        private bool _disposed;

        protected BaseLuceneService()
        {
            Directory = FSDirectory.Open(new DirectoryInfo(Settings.Default.IndexDirectory));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (Directory != null)
                        Directory.Dispose();
                }

                Directory = null;
                _disposed = true;
            }
        }
    }
}
