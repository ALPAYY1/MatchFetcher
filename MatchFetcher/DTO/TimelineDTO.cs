using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class TimelineDTO
    {
        public MetadataTimeLineDTO Metadata { get; set; }
        public InfoTimeLineDTO Info { get; set; }
    }
}
