using ConsoleSearch.Interface;
using SeachEngineAPI.Factories;
using SeachEngineAPI.Interfaces;
using System;
using System.Threading.Tasks;

namespace ConsoleSearch
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    new App().Run();
        //}

        static async Task Main()
        {
            ISearchClient searchClient = SearchClientFactory.Create();

            var app = new App(searchClient);

            await app.Run();
        }
    }
}
