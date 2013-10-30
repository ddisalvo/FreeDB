//based on https://github.com/ayende/XmcdParser

namespace FreeDB.DataFileImporter
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Core.Model;
    using EncodingHelpers;
    using ICSharpCode.SharpZipLib.BZip2;
    using ICSharpCode.SharpZipLib.Tar;
    using Infrastructure;
    using Infrastructure.EntityFramework;
    using Properties;

    public class Program
    {
        private const int SqlBatchSize = 100;
        private const int IndexBatchSize = 10000;

        public static void Main(string[] args)
        {
            var batch = new FreeDbSqlBatch();
            var stopwatch = ParseFile((disc, flush) => Persist(disc, batch, flush));

            Console.WriteLine();
            Console.WriteLine("Done sql persisting in {0}", stopwatch);

            stopwatch = IndexDatabase();

            Console.WriteLine();
            Console.WriteLine("Done indexing in {0}", stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static Stopwatch IndexDatabase()
        {
            var stopwatch = Stopwatch.StartNew();

            using (var indexService = new IndexService())
            {
                using (var context = new FreeDbDataContext())
                {
                    var total = context.Set<Disc>().LongCount();
                    Console.WriteLine("{0} total records, {1} batches of {2}", total, (total/IndexBatchSize) + 1, IndexBatchSize);
                    for (var i = 0; i <= total/IndexBatchSize; i++)
                    {
                        var batchStart = stopwatch.Elapsed;
                        var batch =
                            context.Set<Disc>()
                                   .AsNoTracking()
                                   .Include(d => d.Artist)
                                   .Include(d => d.Genre)
                                   .Include(d => d.Tracks)
                                   .OrderBy(d => d.Id)
                                   .Skip(i*IndexBatchSize)
                                   .Take(IndexBatchSize)
                                   .Select(
                                       d =>
                                       new
                                           {
                                               DiscId = d.Id,
                                               DiscTitle = d.Title,
                                               ArtistId = d.Artist != null ? d.Artist.Id : new int?(),
                                               ArtistName = d.Artist != null ? d.Artist.Name : null,
                                               Genre = d.Genre != null ? d.Genre.Title : null,
                                               Tracks = d.Tracks.Select(t => t.Title)
                                           });

                        indexService.Add(batch);
                        Console.WriteLine("Index batch {0} time: {1}", i, stopwatch.Elapsed - batchStart);
                    }
                }
            }

            return stopwatch;
        }

        private static void Persist(Disc discToAdd, FreeDbSqlBatch batch, bool flush = false)
        {
            using (var context = new FreeDbDataContext())
            {
                if (discToAdd != null)
                    batch.Add(discToAdd);

                if (batch.Discs.Rows.Count < SqlBatchSize && !flush)
                    return;

                context.Execute("InsertDisc", new[]
                    {
                        new SqlParameter("Discs", SqlDbType.Structured)
                            {
                                Value = batch.Discs,
                                TypeName = "dbo.Disc"
                            },
                        new SqlParameter("Tracks", SqlDbType.Structured)
                            {
                                Value = batch.Tracks,
                                TypeName = "dbo.Track"
                            }
                    });

                batch.Discs.Clear();
                batch.Tracks.Clear();
            }
        }

        private static Stopwatch ParseFile(Action<Disc, bool> persist)
        {
            var i = 0;
            var parser = new FileParser();
            var buffer = new byte[1024 * 1024];

            var stopwatch = Stopwatch.StartNew();
            using (var bz2 = new BZip2InputStream(File.Open(Settings.Default.FilePath, FileMode.Open)))
            {
                using (var tar = new TarInputStream(bz2))
                {
                    TarEntry entry;
                    while ((entry = tar.GetNextEntry()) != null)
                    {
                        if (entry.Size == 0 || entry.Name == "README" || entry.Name == "COPYING")
                            continue;
                        var readSoFar = 0;
                        while (true)
                        {
                            var read = tar.Read(buffer, readSoFar, ((int)entry.Size) - readSoFar);
                            if (read == 0)
                                break;

                            readSoFar += read;
                        }

                        var stream = new MemoryStream(buffer, 0, readSoFar);
                        var fileText =
                            new StreamReader(stream, EncodingExtenstions.DetectTextEncoding(stream.ToArray())).ReadToEnd();
                        try
                        {
                            var disc = parser.Parse(fileText);
                            persist(disc, false);
                            if (i++ % SqlBatchSize == 0)
                                Console.Write("\r{0} {1:#,#}  {2}         ", entry.Name, i, stopwatch.Elapsed);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine();
                            Console.WriteLine(entry.Name);
                            Console.WriteLine(e);
                        }
                    }
                    //flush remaining
                    persist(null, true);
                }
            }

            return stopwatch;
        }
    }
}
