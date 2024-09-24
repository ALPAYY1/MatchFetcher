using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class FramesTimeLineDTO
    {
        public List<EventsTimeLineDTO> Events { get; set; }
        public ParticipantFramesDTO ParticipantFrames { get; set; }
        public int Timestamp { get; set; }
    }
}
