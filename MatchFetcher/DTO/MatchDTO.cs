using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class MatchDTO
    {
        public MetadataDTO metadata { get; set; }  
        public InfoDTO info { get; set; }
    }
}
