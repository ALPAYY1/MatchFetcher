using MatchFetcher.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.Repository
{
    public class TrainData
    {
        public string participantId { get; set; }
        public string championName { get; set; }
        public string position { get; set; }
        public string rank { get; set; }
        public string tier { get; set; }
        public List<ParticipantFrameDTO> frames { get; set; }
        public Dictionary<int, ParticipantFrameDTO> timeStampAndFrames { get; set; } = new Dictionary<int, ParticipantFrameDTO>();
    }
}
