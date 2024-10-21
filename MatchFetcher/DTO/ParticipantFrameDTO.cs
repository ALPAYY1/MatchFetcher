using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class ParticipantFrameDTO
    {
        public ChampionStatsDTO ChampionStats { get; set; }
        public int CurrentGold { get; set; }
        public DamageStatsDTO DamageStats { get; set; }
        public int GoldPerSecond { get; set; }
        public int JungleMinionsKilled { get; set; }
        public int Level { get; set; }
        public int MinionsKilled { get; set; }
        public int ParticipantId { get; set; }
        public PositionDTO Position { get; set; }
        public int TimeEnemySpentControlled { get; set; }
        public int TotalGold { get; set; }
        public int Xp { get; set; }
        public int TimeStamp { get; set; }
    }
}
