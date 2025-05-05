using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class TermnetRequest
    {
        public string Query { get; set; }
        public List<string> Domains { get; set; }
    }
}
