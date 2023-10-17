using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model.MobilePay
{
    public class PaymentResponseLink
    {

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class PaymentResponse
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("links")]
        public List<PaymentResponseLink> Links { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

    }
}