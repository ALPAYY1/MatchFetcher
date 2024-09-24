using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class InfoDTO
    {
        public string endOfGameResult { get; set; }
        public long gameCreation { get; set; }
        public long gameDuration { get; set; }
        public long gameEndTimestamp { get; set; }
        public long gameId { get; set; }
        public string gameMode { get; set; }
        public string gameName { get; set; }
        public long gameStartTimestamp { get; set; }
        public string gameType { get; set; }
        public string gameVersion { get; set; }
        public int mapId { get; set; }
        public List<ParticipantDTO> participants { get; set; }
        public string platformId { get; set; }
        public int queueId { get; set; }
        public List<TeamDTO> teams { get; set; }
        public string tournamentCode { get; set; }
    }
}
