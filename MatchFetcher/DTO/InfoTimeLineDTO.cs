using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class InfoTimeLineDTO
    {
        public string EndOfGameResult { get; set; }
        public long FrameInterval { get; set; }
        public long GameId { get; set; }
        public List<ParticipantTimeLineDTO> Participants { get; set; }
        public List<FramesTimeLineDTO> Frames { get; set; }
    }
}
