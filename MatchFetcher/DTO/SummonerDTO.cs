﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class SummonerDTO
    {
        public string id { get; set; }
        public string accountId { get; set; }
        public string puuid { get; set; }
        public int profileIconId { get; set; }
        public long revisionDate { get; set; }
        public int summonerLevel { get; set; }
    }
}