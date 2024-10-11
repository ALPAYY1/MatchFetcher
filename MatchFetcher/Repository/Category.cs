using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.Repository
{
    public class Category
    {
        public static List<string> Artillery = new List<string>() { "Hwei", "Jayce", "Lux", "Varus", "Vel'Koz", "Xerath", "Ziggs" };
        
        public static List<string> Assassin = new List<string>() { "Akali", "Akshan", "Aurora", "Diana", "Ekko", "Evelynn", "Fizz", "Kassadin",
        "Katarina", "Kha'Zix", "Naafiri", "Nocturne", "Pyke", "Qiyana", "Rengar", "Shaco", "Talon", "Yone", "Zed"};

        public static List<string> Battlemage = new List<string>() { "Anivia", "Cassiopeia", "Karthus", "Malzahar", "Rumble", "Ryze", "Swain",
        "Taliyah", "Viktor", "Vladimir"};

        public static List<string> Burst = new List<string>() { "Ahri", "Annie", "Brand", "Karma", "LeBlanc", "Lissandra", "Neeko", "Orianna",
        "Seraphine", "Sylas", "Syndra", "Twisted Fate", "Veigar", "Vex", "Zoe"};

        public static List<string> Catcher = new List<string>() { "Bard", "Blitzcrank", "Ivern", "Jhin", "Morgana", "Rakan", "Thresh", "Zyra" };

        public static List<string> Diver = new List<string>() { "Briar", "Camille", "Elise", "Hecarim", "Irelia", "JarvanIV", "Lee Sin",
        "Olaf", "Pantheon", "Rek'Sai", "Renekton", "Vi", "Warwick", "Wukong", "Xin Zhao"};

        public static List<string> Enchanter = new List<string>() { "Janna", "Karma", "Lulu", "Milio", "Nami", "Renata Glasc", "Senna",
        "Sona", "Soraka", "Taric", "Yuumi"};

        public static List<string> Juggernaut = new List<string>() { "Aatrox", "Darius", "Dr. Mundo", "Illaoi", "Mordekaiser", "Nasus", "Sett",
        "Shyvana", "Trundle", "Udyr", "Urgot", "Volibear", "Yorick"};

        public static List<string> Marksman = new List<string>() { "Aphelios", "Caitlyn", "Corki", "Draven", "Ezreal", "Jhin", "Jinx", "Kai'Sa",
        "Kalista", "Kindred", "Kog'Maw", "Lucian", "Miss Fortune", "Samira", "Senna", "Sivir", "Smolder", "Teemo", "Tristana", "Twitch", "Varus",
        "Vayne", "Xayah", "Zeri"};

        public static List<string> Skirmisher = new List<string>() { "Bel'Veth", "Fiora", "Gwen", "Jax", "K'Sante", "Kayn", "Kled", "Lillia",
        "Master Yi", "Nilah", "Riven", "Sylas", "Tryndamere", "Viego", "Yasuo", "Yone"};

        public static List<string> Specialist = new List<string>() { "Azir", "Cho'Gath", "Fiddlesticks", "Gangplank", "Gnar", "Graves", "Heimerdinger",
        "Kayle", "Kennen", "Nidalee", "Quinn", "Singed", "Zilean"};

        public static List<string> Vanguard = new List<string>() { "Alistar", "Amumu", "Gragas", "Leona", "Malphite", "Maokai", "Nautilus", "Nunu",
        "Ornn", "Rammus", "Rell", "Sejuani", "Sion", "Skarner", "Zac"};

        public static List<string> Warden = new List<string>() { "Braum", "Galio", "Poppy", "Shen", "Tahm Kench" };


        public static List<string> GetChampionsInCategory (string champName) 
        {
            if (Artillery.Contains(champName)) return Artillery;
            if (Assassin.Contains(champName)) return Assassin;
            if (Battlemage.Contains(champName)) return Battlemage;
            if (Burst.Contains(champName)) return Burst;
            if (Catcher.Contains(champName)) return Catcher;
            if (Diver.Contains(champName)) return Diver;
            if (Enchanter.Contains(champName)) return Enchanter;
            if (Juggernaut.Contains(champName)) return Juggernaut;
            if (Marksman.Contains(champName)) return Marksman;
            if (Skirmisher.Contains(champName)) return Skirmisher;
            if (Specialist.Contains(champName)) return Specialist;
            if (Vanguard.Contains(champName)) return Vanguard;
            if (Warden.Contains(champName)) return Warden;

            return new List<string>();
        }
    }
}