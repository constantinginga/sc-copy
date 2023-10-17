using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model.MobilePay
{
    public class AgreementStatus
    {
        [JsonProperty("agreement_id")]
        public string AgreementId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_text")]
        public string StatusText { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

    }
}