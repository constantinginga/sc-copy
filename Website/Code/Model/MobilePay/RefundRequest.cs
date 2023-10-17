using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model.MobilePay
{
    public class RefundRequest
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("status_callback_url")]
        public string Status_Callback_Url { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

    }
}