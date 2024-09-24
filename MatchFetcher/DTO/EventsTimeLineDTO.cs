using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class EventsTimeLineDTO
    {
        public long Timestamp { get; set; }
        public long RealTimestamp { get; set; }
        public string Type { get; set; }
    }
}
