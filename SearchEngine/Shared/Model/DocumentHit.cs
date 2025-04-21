using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shared.Model
{
    public class DocumentHit
    {
        [JsonProperty("document")]
        public BEDocument Document { get; set; }

        [JsonProperty("noOfHits")]
        public int NoOfHits { get; set; }

        [JsonProperty("missing")]
        public List<string> Missing { get; set; }
        public string IdxTime { get; set; }

        [JsonIgnore] // By default, exclude the idxTime from JSON serialization
        public bool IncludeTimeStamp { get; set; }

        public DocumentHit() { }

        public DocumentHit(BEDocument doc, int noOfHits, List<string> missing, bool includeTimeStamp)
        {
            Document = doc;
            NoOfHits = noOfHits;
            Missing = missing;

            IncludeTimeStamp = includeTimeStamp;

            // Set IdxTime only if IncludeTimeStamp is true, otherwise don't set it
            IdxTime = includeTimeStamp ? doc.mIdxTime : null;
        }
    }

}
