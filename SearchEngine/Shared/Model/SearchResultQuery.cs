using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Shared.Model
{
    /*
     * A data class representing the result of a search.
     * Hits is the total number of documents containing at least one word from the query.
     * DocumentHits is the documents and the number of words from the query contained in the document - see
     * the class DocumentHit
     * Ignored contains words from the query not present in the document base.
     * TimeUsed is the timespan used to perform the search.
     */
    public class SearchResultQuery
    {
        public SearchResultQuery(string[] query, int hits, List<DocumentHit> documents, List<string> ignored, TimeSpan timeUsed)
        {
            Query = query;
            Hits = hits;
            DocumentHits = documents;
            Ignored = ignored;
            TimeUsed = timeUsed;
        }
        
        public SearchResultQuery() { }

        // The query that was used to search
        [JsonProperty("query")]
        public string[] Query { get; set; }
        [JsonProperty("hits")]
        public int Hits { get; set; }
        [JsonProperty("documentHits")]
        public List<DocumentHit> DocumentHits { get; set;  }
        [JsonProperty("ignored")]
        public List<string> Ignored { get; set; }
        [JsonProperty("timeUsed")]
        public TimeSpan TimeUsed { get; set; }

        [JsonProperty("unexpandedTerms")]
        public List<string> UnexpandedTerms { get; set; } = new();

    }
}
