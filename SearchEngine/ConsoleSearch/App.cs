using ConsoleSearch.Client;
using ConsoleSearch.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleSearch
{
    public class App
    {
        public App()
        {
        }

        private readonly ISearchClient _searchClient;

        public App(ISearchClient searchClient)
        {
            _searchClient = searchClient;
        }

        public async Task Run()
        {
            Console.Write("Enter search query: ");
            string query = Console.ReadLine();

            Console.Write("Enter max words (e.g., 100): ");
            int maxWords;
            while (!int.TryParse(Console.ReadLine(), out maxWords) || maxWords <= 0)
            {
                Console.WriteLine("Please enter a valid positive number for max words.");
            }

            try
            {
                List<string> results = await _searchClient.SearchAsync(query, maxWords);

                Console.WriteLine("\nSearch Results:");
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        //public void Run()
        //{
        //    SearchLogic mSearchLogic = new SearchLogic(new DatabaseSqlite());


        //    Console.WriteLine("Console Search");

        //    while (true)
        //    {
        //        Console.WriteLine("enter search terms - q for quit");
        //        string input = Console.ReadLine();
        //        if (input.Equals("q")) break;

        //        var query = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);


        //        var result = mSearchLogic.Search(query, 10);

        //        if (result.Ignored.Count > 0) {
        //            Console.WriteLine($"Ignored: {string.Join(',', result.Ignored)}");
        //        }

        //        int idx = 1;
        //        foreach (var doc in result.DocumentHits) {
        //            Console.WriteLine($"{idx} : {doc.Document.mUrl} -- contains {doc.NoOfHits} search terms");
        //            Console.WriteLine("Index time: " + doc.Document.mIdxTime);
        //            Console.WriteLine($"Missing: {ArrayAsString(doc.Missing.ToArray())}");
        //            idx++;
        //        }
        //        Console.WriteLine("Documents: " + result.Hits + ". Time: " + result.TimeUsed.TotalMilliseconds);
        //    }

        //    static async Task Main()
        //    {
        //        Console.Write("Enter search query: ");
        //        string query = Console.ReadLine();

        //        ISearchProxy searchProxy = SearchProxyFactory.Create();
        //        var results = await searchProxy.SearchAsync(query);

        //        Console.WriteLine("Search Results:");
        //        foreach (var result in results)
        //        {
        //            Console.WriteLine(result);
        //        }
        //    }
        //}

        string ArrayAsString(string[] s) {
            return s.Length == 0?"[]":$"[{String.Join(',', s)}]";
            //foreach (var str in s)
            //    res += str + ", ";
            //return res.Substring(0, res.Length - 2) + "]";
        }
    }
}
