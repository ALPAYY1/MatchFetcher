using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class TeamDTO
    {
        public List<BanDTO> bans { get; set; }
        //public List<ObjectivesDTO> objectives { get; set; }
        public int teamId { get; set; }
        public bool win { get; set; }
    }
}
