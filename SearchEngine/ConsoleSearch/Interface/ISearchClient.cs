using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSearch.Interface
{
    public interface ISearchClient
    {
        Task<List<string>> SearchAsync(string query, int maxWords);
    }
}
