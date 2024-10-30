using CsvHelper;
using CsvHelper.Configuration;
using MatchFetcher.DTO;
using MatchFetcher.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MatchFetcher
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            List<string> leagues = new List<string> { "iron", "bronze", "silver", "gold", "platinum", "emerald", "diamond" };
            List<string> divisions = new List<string> { "iv", "i" };
            List<string> pages = new List<string> { "1" };

            string apiKey = "";


            
            //-----------------DO NOT EDIT BELOW THIS LINE, UNLESS YOU KNOW WHAT YOU'RE DOING----------------------//

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
                    tempSummoner.Rank = match.Rank;
                    tempSummoner.Tier = match.Tier;
                    if (!summoners.Any(x => x.id == tempSummoner.id)) summoners.Add(tempSummoner);

                    Thread.Sleep(1500);

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
                        Thread.Sleep(1500);

                        if (match.metadata != null && match.info != null && match.info.gameMode == "CLASSIC" && match.info.gameType == "MATCHED_GAME" && match.info.mapId == 11) 
                        {
                            url = url + "/timeline";

                            HttpResponseMessage responseTimeLine = await client.GetAsync(url);
                            responseTimeLine.EnsureSuccessStatusCode();

                            string responseBodyTimeLine = await responseTimeLine.Content.ReadAsStringAsync();
                            TimelineDTO timeLine = JsonConvert.DeserializeObject<TimelineDTO>(responseBodyTimeLine);

                            Thread.Sleep(1500);

                            // Calculate match rank
                            List<string> summonerIds = match.info.participants.Select(x => x.SummonerId).ToList();
                            List<string> ranks = new List<string>();

                            foreach (string summonerid in summonerIds) 
                            {
                                string sUrl = $"https://euw1.api.riotgames.com/lol/league/v4/entries/by-summoner/{summonerid}";

                                HttpResponseMessage sResponseMatch = await client.GetAsync(sUrl);
                                sResponseMatch.EnsureSuccessStatusCode();

                                string sResponseBodyMatch = await sResponseMatch.Content.ReadAsStringAsync();
                                var summonerEntryList = JsonConvert.DeserializeObject<List<LeagueEntryDTO>>(sResponseBodyMatch);
                                LeagueEntryDTO summonerEntry = summonerEntryList.Where(x => x.QueueType == "RANKED_SOLO_5x5").FirstOrDefault();

                                if (summonerEntry != null) ranks.Add(summonerEntry.Tier + "_" + summonerEntry.Rank);
                                
                                Thread.Sleep(1500);
                            }

                            if (!ranks.Any()) 
                            {
                                match.Tier = summonermatch.Key.Tier;
                                match.Rank = summonermatch.Key.Rank;
                            }
                            else 
                            {
                                // Calculate average match rank
                                Ranks rank = Ranks.CalculateMatchRank(ranks);
                                match.Tier = rank.Tier;
                                match.Rank = rank.Division;
                            }

                            // Create folder matchid som navn.
                            string dPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            Directory.CreateDirectory(dPath + "\\Matches"); // Create root directory

                            Directory.CreateDirectory(dPath + "\\Matches\\" + $"{match.Tier + "_" + match.Rank}"); // Create rank folder

                            Directory.CreateDirectory(dPath + "\\Matches" + "\\" + $"{match.Tier + "_" + match.Rank}" + "\\" + $"{matchid}");

                            // Make match into json
                            string matchJsoned = JsonConvert.SerializeObject(match);
                            string timelineJsoned = JsonConvert.SerializeObject(timeLine);

                            File.WriteAllText(dPath + "\\Matches" + "\\" + $"{match.Tier + "_" + match.Rank}" + "\\" + $"{matchid}" + "\\" + "match.json", matchJsoned);
                            File.WriteAllText(dPath + "\\Matches" + "\\" + $"{match.Tier + "_" + match.Rank}" + "\\" + $"{matchid}" + "\\" + "timeline.json", timelineJsoned);

                            matches.Add(match, timeLine);

                            ConsoleCommands.ClearCurrentLine();
                            Console.WriteLine($"Ranked matches found and processed: {matches.Count()}");
                        }
                    }
                    catch (HttpRequestException e)
                    { 
                        Console.WriteLine($"Request error: {e.Message}");
                    }
                } 
            }

            List<TrainData> trainData = new List<TrainData>();

            foreach (var element in matches) 
            {
                // Key == match
                // Value == timeline

                List<ParticipantDTO> champions = element.Key.info.participants.ToList();

                foreach (ParticipantDTO champion in champions) 
                {
                    TrainData tempTrainData = new TrainData()
                    {
                        participantId = champion.Puuid,
                        championName = champion.ChampionName,
                        position = champion.IndividualPosition,
                        rank = element.Key.Rank,
                        tier = element.Key.Tier
                    };

                    int participantFrameNumber = element.Value.Info.Participants.FindIndex(x => x.Puuid == champion.Puuid);
                    List<ParticipantFrameDTO> frames = new List<ParticipantFrameDTO>();
                    List<int> timeStamps = element.Value.Info.Frames.Select(x => x.Timestamp).ToList();

                    // Get all the frames of said champion.
                    switch (participantFrameNumber)
                    {
                        case 0:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._1).ToList();
                            break;
                        case 1:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._2).ToList();
                            break;
                        case 2:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._3).ToList();
                            break;
                        case 3:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._4).ToList();
                            break;
                        case 4:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._5).ToList();
                            break;
                        case 5:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._6).ToList();
                            break;
                        case 6:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._7).ToList();
                            break;
                        case 7:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._8).ToList();
                            break;
                        case 8:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._9).ToList();
                            break;
                        case 9:
                            frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._10).ToList();
                            break;
                    }

                    for (int i = 0; i < timeStamps.Count; i++) 
                    {
                        int timeStamp = timeStamps.ElementAt(i);
                        ParticipantFrameDTO frame = frames.ElementAt(i);

                        frame.TimeStamp = timeStamp;

                        tempTrainData.timeStampAndFrames.Add(timeStamp, frame);
                    }

                    tempTrainData.frames = frames;

                    trainData.Add(tempTrainData);
                }
            }

            // Time to start generating the CSV.
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Directory.CreateDirectory(desktopPath + "\\Data"); // Create root directory
            
            foreach (TrainData td in trainData) 
            {
                string rootPath = desktopPath + "\\Data";
                Directory.CreateDirectory(rootPath + $"\\{td.championName}"); // Create champion folder, with its corresponding name.

                Directory.CreateDirectory(rootPath + $"\\{td.championName}" + $"\\{td.position}"); // Create position folder inside champion.
                Directory.CreateDirectory(rootPath + $"\\{td.championName}" + $"\\{td.position}" + $"\\{td.tier + "_" + td.rank}"); // Rank folder in position folder.

                bool exists = File.Exists(rootPath + $"\\{td.championName}" + $"\\{td.position}" + $"\\{td.tier + "_" + td.rank}" + "\\" + "data.csv");

                if (exists) 
                {
                    // Append to file
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };
                    using (var stream = File.Open(rootPath + $"\\{td.championName}" + $"\\{td.position}" + $"\\{td.tier + "_" + td.rank}" + "\\" + "data.csv", FileMode.Append))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            using (var csv = new CsvWriter(writer, config))
                            {
                                csv.WriteRecords(td.frames);
                            }
                        }
                    }
                }
                else 
                {
                    // Create file
                    using (var writer = new StreamWriter(@rootPath + $"\\{td.championName}" + $"\\{td.position}" + $"\\{td.tier + "_" + td.rank}" + "\\" + "data.csv"))
                    {
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(td.frames);
                        }
                    }
                }
            }
        }
    }
}
