using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class QueryProcessor
    {
        public static string[] ProcessQuery(string query, bool caseSensitive, int maxWords)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be empty.");
            }
            string[] queryArray = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!caseSensitive)
            {
                queryArray = queryArray.Select(term => term.ToLower()).ToArray();
            }
            // Apply maxWords limit if provided

            if (maxWords > 0 && maxWords < queryArray.Length)
            {
                queryArray = queryArray.Take(maxWords).ToArray();
            }
            return queryArray;
        }
    }
}
