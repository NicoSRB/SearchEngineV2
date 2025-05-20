using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class LoadBalancerConfig
    {
        public List<string> BackendServers { get; set; } = new();
    }

}
