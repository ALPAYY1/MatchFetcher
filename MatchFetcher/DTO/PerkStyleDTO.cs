using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class PerkStyleDTO
    {
        public string description { get; set; }
        public List<PerkStyleSelectionDTO> selections { get; set; }
        public int style { get; set; }
    }
}
