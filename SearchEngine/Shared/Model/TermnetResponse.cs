using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class TermnetResponse
    {
        public Dictionary<string, List<SynonymTermer>> ExpandedTerms { get; set; }
        public List<string> NotExpandedTerms { get; set; } = new();

        public TermnetResponse()
        {
            ExpandedTerms = new Dictionary<string, List<SynonymTermer>>();
        }
    }
}
