using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Class
{
    public class ActiveCampaignAPIResult
    {
        [JsonProperty("result_code")]
        public int Code { get; set; }

        [JsonProperty("result_message")]
        public string Message { get; set; }

        [JsonProperty("result_output")]
        public string Output { get; set; }

        public string Data { get; set; }

        public bool IsSuccessful => Code == 1;
    }
}