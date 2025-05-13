using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class SearchResponseDTO
    {
        public List<DocumentHit> Results { get; set; } = new ();
        public Dictionary<string, List<SynonymTermer>> Synonyms { get; set; }
    }   
}
