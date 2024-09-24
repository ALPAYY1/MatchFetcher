using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class PerksDTO
    {
        public PerkStatsDTO statPerks { get; set; }
        public List<PerkStyleDTO> styles { get; set; }
    }
}
