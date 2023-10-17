using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model.MobilePay
{
        
    public class AgreementResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("one_off_payment_id")]
        public string OneOffPaymentId { get; set; }

        [JsonProperty("links")]
        public IList<Link> Links { get; set; }
    }

    public class Link
    {

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }

}