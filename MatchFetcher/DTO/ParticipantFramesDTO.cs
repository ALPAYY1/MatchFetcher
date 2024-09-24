using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchFetcher.DTO
{
    public class ParticipantFramesDTO
    {
        [JsonProperty("1")]
        public ParticipantFrameDTO _1 { get; set; }
        [JsonProperty("2")]
        public ParticipantFrameDTO _2 { get; set; }
        [JsonProperty("3")]
        public ParticipantFrameDTO _3 { get; set; }
        [JsonProperty("4")]
        public ParticipantFrameDTO _4 { get; set; }
        [JsonProperty("5")]
        public ParticipantFrameDTO _5 { get; set; }
        [JsonProperty("6")]
        public ParticipantFrameDTO _6 { get; set; }
        [JsonProperty("7")]
        public ParticipantFrameDTO _7 { get; set; }
        [JsonProperty("8")]
        public ParticipantFrameDTO _8 { get; set; }
        [JsonProperty("9")]
        public ParticipantFrameDTO _9 { get; set; }
        [JsonProperty("10")]
        public ParticipantFrameDTO _10 { get; set; }
    }
}
