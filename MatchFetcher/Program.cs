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

            KeyValuePair<string, string> championToExtract = new KeyValuePair<string, string>("Tryndamere", "TOP"); // Key = champname. Value = position.
            //-----------------DO NOT EDIT BELOW THIS LINE----------------------//

            List<string> championsInCategory = Category.GetChampionsInCategory(championToExtract.Key);
            if (!championsInCategory.Any()) throw new Exception($"Invalid champion provided ( {championToExtract.Key} ). Check spelling.");

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

                    if (summoners.Count > 20) break; // REMOVE

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
                            
                            // break; // REMOVE

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

            // Finalize the train data
            Dictionary<string, List<TrainData>> specializedTrainData = new Dictionary<string, List<TrainData>>(); // This is the desired champion.
            List<TrainData> otherTrainData = new List<TrainData>(); // This is everyone else, that is NOT that champion.

            foreach (var element in matches) 
            {
                // Key == match
                // Value == timeline

                // Find the unique champion we want to train the data on, specified in championToExtract.
                ParticipantDTO participant = element.Key.info.participants.Where(x => x.ChampionName == championToExtract.Key).FirstOrDefault();

                if (participant != null && participant.IndividualPosition == championToExtract.Value) 
                {
                    // Desired champion EXISTS in this match AND has the correct position.

                    TrainData tempTrainData = new TrainData()
                    {
                        participantId = participant.Puuid,
                        participantFrameNumber = element.Value.Info.Participants.FindIndex(x => x.Puuid == participant.Puuid),
                        championName = participant.ChampionName
                    };

                    // Get all the frames of said champion.
                    switch (tempTrainData.participantFrameNumber) 
                    {
                        case 0:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._1).ToList();
                            break;
                        case 1:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._2).ToList();
                            break;
                        case 2:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._3).ToList();
                            break;
                        case 3:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._4).ToList();
                            break;
                        case 4:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._5).ToList();
                            break;
                        case 5:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._6).ToList();
                            break;
                        case 6:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._7).ToList();
                            break;
                        case 7:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._8).ToList();
                            break;
                        case 8:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._9).ToList();
                            break;
                        case 9:
                            tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._10).ToList();
                            break;
                    }

                    if (specializedTrainData.ContainsKey(championToExtract.Key)) 
                    {
                        List<TrainData> trainData = specializedTrainData[championToExtract.Key];
                        trainData.Add(tempTrainData);
                    }
                    else 
                    {
                        specializedTrainData.Add(championToExtract.Key, new List<TrainData>() { tempTrainData });
                    }
                }
 
                // Get every champion that is NOT our desired champion.
                List<ParticipantDTO> tempParticipants = element.Key.info.participants.Where(x => x.ChampionName != championToExtract.Key).ToList();
                    
                foreach (ParticipantDTO tparticipant in tempParticipants) 
                {
                    // Is the champion we have here the SAME category as our desired champion AND SAME position?
                    if (championsInCategory.Contains(tparticipant.ChampionName) && tparticipant.IndividualPosition == championToExtract.Value) 
                    {
                        // The champion that is NOT our desired champion, is in the SAME category as the desired champion.
                        TrainData tempTrainData = new TrainData()
                        {
                            participantId = tparticipant.Puuid,
                            participantFrameNumber = element.Value.Info.Participants.FindIndex(x => x.Puuid == tparticipant.Puuid),
                            championName = tparticipant.ChampionName
                        };

                        // Get all the frames of said champion.
                        switch (tempTrainData.participantFrameNumber)
                        {
                            case 0:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._1).ToList();
                                break;
                            case 1:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._2).ToList();
                                break;
                            case 2:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._3).ToList();
                                break;
                            case 3:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._4).ToList();
                                break;
                            case 4:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._5).ToList();
                                break;
                            case 5:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._6).ToList();
                                break;
                            case 6:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._7).ToList();
                                break;
                            case 7:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._8).ToList();
                                break;
                            case 8:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._9).ToList();
                                break;
                            case 9:
                                tempTrainData.frames = element.Value.Info.Frames.Select(x => x.ParticipantFrames._10).ToList();
                                break;
                        }

                        otherTrainData.Add(tempTrainData);
                    }
                    // We don't care about champions that don't fall into the same category & position - because comparing vayne top to tryndamere top
                    // will draw undesired conclusions, when training the model. 
                }
            }

             if (!specializedTrainData.Any()) throw new Exception($"No champion with name {championToExtract.Key} was found. Get more games.");
            
             // Generate the CSV.


            // Train object, som kommer til at være et udsnit af timelapse af X champion.
            // Så har man rækker, som hver især repræsentere en frame.
            // En CSV med tryndamere, og en med alle andre end tryndamere, men i hans kategori.

            // Træn modellen på 3 variabler. Tid, skade, koordinater. Binært klassifikations problem. Aggressiv/defensiv.
            // Du skal have flere modeller. 1 model til hver champion. Også selve modellen der er trænet på ALT data.
            // Derefter skal der sammenlignes, tryndamere modellen op mod den store model og se hvordan den performer.
            // Tag champion kategorier, såsom skirmishers (tryndamere) også fokuser på de champions i den kategori.
        }
    }
}
