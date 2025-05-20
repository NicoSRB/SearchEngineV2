using System;
using System.Collections.Generic;
using System.IO;
using indexer.Databases;
using indexer.Interfaces;
using Shared;

namespace Indexer
{
    public class App
    {
        public void Run(string backend){
            IDatabase db = backend switch
            {
                "sqlite" => new DatabaseSqlite(),
                "postgres" => new DatabasePostgres(),
                "mongo" => new DatabaseMongo(),
                _ => throw new ArgumentException("Unsupported backend: " + backend)
            };


            Crawler crawler = new Crawler(db);
            var root = new DirectoryInfo(Paths.FOLDERDB2);

            crawler.IndexFilesIn(root, new List<string> { ".txt" });


            DateTime start = DateTime.Now;
            TimeSpan used = DateTime.Now - start;
            Console.WriteLine("DONE! used " + used.TotalMilliseconds);

            var all = db.GetAllWords();

            Console.WriteLine($"Indexed {db.GetDocumentCounts()} documents");
            Console.WriteLine($"Number of different words: {all.Count}");
            int count = 10;
            Console.WriteLine($"The first {count} is:");
            foreach (var p in all) {
                Console.WriteLine("<" + p.Key + ", " + p.Value + ">");
                count--;
                if (count == 0) break;
            }
        }
        public void RunAll(IEnumerable<string> backends)
        {
            foreach (var backend in backends)
            {
                Console.WriteLine($"Starting indexing with backend: {backend}");
                Run(backend);
            }
        }
    }
}
