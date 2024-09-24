using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class DamageStatsDTO
    {
        public int MagicDamageDone { get; set; }
        public int MagicDamageDoneToChampions { get; set; }
        public int MagicDamageTaken { get; set; }
        public int PhysicalDamageDone { get; set; }
        public int PhysicalDamageDoneToChampions { get; set; }
        public int PhysicalDamageTaken { get; set; }
        public int TotalDamageDone { get; set; }
        public int TotalDamageDoneToChampions { get; set; }
        public int TotalDamageTaken { get; set; }
        public int TrueDamageDone { get; set; }
        public int TrueDamageDoneToChampions { get; set; }
        public int TrueDamageTaken { get; set; }
    }
}
