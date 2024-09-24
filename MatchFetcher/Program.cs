using MatchFetcher.DTO;
using MatchFetcher.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MatchFetcher
{
    public class Program
    {
        /// <summary>
        /// Fetches matches and groups those matches in the amount of champions that exist in league of legends.
        /// 150 champions means 150 groups of matches i.e 1 group of tryndamere games, 1 group for aatrox games etc.
        /// 1 big CSV will be generated with everything grouped together and then another X amount will be made for champion specific. (defined in
        /// champions to extract)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            // , "bronze", "silver", "gold", "platinum", "emerald", "diamond", "master", "grandmaster", "challenger"
            List<string> leagues = new List<string> { "iron" };
            List<string> divisions = new List<string> { "i" };
            List<string> pages = new List<string> { "1" };

            string apiKey = "RGAPI-e75b46d3-5e51-4829-b978-68790c3ebf56";

            string outputPath = @"../../";

            List<string> championsToExtract = new List<string>() {"Tryndamere" };
            //---------------------------------------//

            
            List<MatchEntryDTO> matchEntries = new List<MatchEntryDTO>();

            foreach (string league in leagues) 
            {
                foreach (string division in divisions) 
                {
                    foreach (string page in pages) 
                    {
                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);

                        try
                        {
                            string url = $"https://euw1.api.riotgames.com/lol/league/v4/entries/RANKED_SOLO_5x5/{league.ToUpper()}/{division.ToUpper()}?page={page}";

                            HttpResponseMessage response = await client.GetAsync(url);
                            response.EnsureSuccessStatusCode();

                            string responseBody = await response.Content.ReadAsStringAsync();
                            List<MatchEntryDTO> tempMatches = JsonConvert.DeserializeObject<List<MatchEntryDTO>>(responseBody);

                            foreach (MatchEntryDTO tempMatch in tempMatches) matchEntries.Add(tempMatch);

                            Thread.Sleep(1500);

                            Console.WriteLine($"Matches found in {league},{division}: {tempMatches.Count}");
                            Console.WriteLine($"Total matches: {matchEntries.Count}");
                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine($"Request error: {e.Message}");
                        }
                    }
                }
            }

            List<SummonerDTO> summoners = new List<SummonerDTO>();

            foreach (MatchEntryDTO match in matchEntries)
            {
                // Get all the summoners in the matches
                string url = $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/{match.SummonerId}";

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    SummonerDTO tempSummoner = JsonConvert.DeserializeObject<SummonerDTO>(responseBody);
                    if (!summoners.Any(x => x.id == tempSummoner.id)) summoners.Add(tempSummoner);

                    Thread.Sleep(1500);

                    if (summoners.Count > 10) break; // REMOVE

                    ConsoleCommands.ClearCurrentLine();
                    Console.WriteLine($"Summoners found: {summoners.Count}");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }

            Console.WriteLine();

            Dictionary<SummonerDTO, string[]> summonerMatches = new Dictionary<SummonerDTO, string[]>();
            int totalSummonersMatchesCount = 0;

            foreach (SummonerDTO summoner in summoners)
            {
                string url = $"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/{summoner.puuid}/ids";

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    summonerMatches.Add(summoner, responseBody.Split(',').Select(x => x.Trim('[', ']', '"')).ToArray());

                    Thread.Sleep(1500);
                    ConsoleCommands.ClearCurrentLine();
                    Console.WriteLine($"Match ids found: {totalSummonersMatchesCount += summonerMatches.Last().Value.Length}");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }

            Console.WriteLine();

            // Holds actual matches.
            Dictionary<MatchDTO, TimelineDTO> matches = new Dictionary<MatchDTO, TimelineDTO>();

            foreach (var summonermatch in summonerMatches) 
            {
                foreach (var matchid in summonermatch.Value) 
                {
                    string url = $"https://europe.api.riotgames.com/lol/match/v5/matches/{matchid}";

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);

                    try
                    {
                        HttpResponseMessage responseMatch = await client.GetAsync(url);
                        responseMatch.EnsureSuccessStatusCode();

                        string responseBodyMatch = await responseMatch.Content.ReadAsStringAsync();
                        MatchDTO match = JsonConvert.DeserializeObject<MatchDTO>(responseBodyMatch);
                        if (match.metadata != null && match.info != null
                            && match.info.gameMode == "CLASSIC" && match.info.gameType == "MATCHED_GAME"
                            && match.info.mapId == 11) 
                        {
                            Thread.Sleep(1500);

                            url = url + "/timeline";

                            HttpResponseMessage responseTimeLine = await client.GetAsync(url);
                            responseTimeLine.EnsureSuccessStatusCode();

                            string responseBodyTimeLine = await responseTimeLine.Content.ReadAsStringAsync();
                            TimelineDTO timeLine = JsonConvert.DeserializeObject<TimelineDTO>(responseBodyTimeLine);

                            matches.Add(match, timeLine);

                            Thread.Sleep(1500);

                            ConsoleCommands.ClearCurrentLine();
                            Console.WriteLine($"Ranked matches found: {matches.Count()}");
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine($"Request error: {e.Message}");
                    }
                }
            }

            Dictionary<string, List<MatchDTO>> organizedMatches = new Dictionary<string, List<MatchDTO>>();

            foreach (MatchDTO match in matches.Keys) 
            {
                foreach(ParticipantDTO participant in match.info.participants) 
                {
                    if (organizedMatches.Keys.Contains(participant.ChampionName)) 
                    {
                        foreach (var element in organizedMatches) 
                        {
                            if (element.Key == participant.ChampionName) 
                            {
                                element.Value.Add(match);
                                break; 
                            }
                        }
                    }
                    else 
                    {
                        organizedMatches.Add(participant.ChampionName, new List<MatchDTO>() { match });
                    }
                }
                // Iterate through matches and group them into <key,value> where key is champion and value is the match.
            }
            
            // Træn modellen på 3 variabler. Tid, skade, koordinater. Binært klassifikations problem. Aggressiv/defensiv.
            // Du skal have flere modeller. 1 model til hver champion. Også selve modellen der er trænet på ALT data.
            // Derefter skal der sammenlignes, tryndamere modellen op mod den store model og se hvordan den performer.
            // Tag champion kategorier, såsom skirmishers (tryndamere) også fokuser på de champions i den kategori.





        }
    }
}
