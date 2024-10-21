using MatchFetcher.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.Repository
{
    public class Ranks
    {
        public enum Rank 
        {
            IRON_IV = 0,
            IRON_III = 1,
            IRON_II = 2,
            IRON_I = 3,
            BRONZE_IV = 4,
            BRONZE_III = 5,
            BRONZE_II = 6,
            BRONZE_I = 7,
            SILVER_IV = 8,
            SILVER_III = 9,
            SILVER_II = 10,
            SILVER_I = 11,
            GOLD_IV = 12,
            GOLD_III = 13,
            GOLD_II = 14,
            GOLD_I = 15,
            PLATINUM_IV = 16,
            PLATINUM_III = 17,
            PLATINUM_II = 18,
            PLATINUM_I = 19,
            EMERALD_IV = 20,
            EMERALD_III = 21,
            EMERALD_II = 22,
            EMERALD_I = 23,
            DIAMOND_IV = 24,
            DIAMOND_III = 25,
            DIAMOND_II = 26,
            DIAMOND_I = 27,
            MASTER = 28,
            GRANDMASTER = 29,
            CHALLENGER = 30
        }

        public string Tier { get; set; }
        public string Division { get; set; }
        public static Ranks CalculateMatchRank(List<string> ranks) 
        {
            List<int> points = new List<int>();

            foreach (string rank in ranks) 
            {
                int value = (int)Enum.Parse(typeof(Ranks.Rank), rank);
                points.Add(value);
            }

            double averageRank = points.Average();
            double roundedRank = Math.Round(averageRank);

            Rank translatedRank = (Rank)roundedRank;
            string stringedRank = translatedRank.ToString();

            string[] splitRank = stringedRank.Split('_');

            Ranks rnk = new Ranks()
            {
                Tier = splitRank[0],
                Division = splitRank[1]
            };

            return rnk;
        }
    }
}
