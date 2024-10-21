using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class LeagueEntryDTO
    {
        public string LeagueId { get; set; }
        public string SummonerId { get; set; }
        public string QueueType { get; set; }
        public string Tier { get; set; }
        public string Rank { get; set; }
        public int LeaguePoints { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public bool HotStreak { get; set; }
        public bool Veteran { get; set; }
        public bool FreshBlood { get; set; }
        public bool Inactive { get; set; }
        public MiniSeriesDTO MiniSeries { get; set; }
    }
}
