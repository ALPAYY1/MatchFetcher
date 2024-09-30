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
        public int participantFrameNumber { get; set; }
        public string championName { get; set; }
        public List<ParticipantFrameDTO> frames { get; set; }

    }
}
