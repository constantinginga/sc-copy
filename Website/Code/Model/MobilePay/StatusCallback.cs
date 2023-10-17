using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model.MobilePay
{
    public class StatusCallback
    {

        [JsonProperty("agreement_id")]
        public string AgreementId { get; set; }

        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("payment_date")]
        public string PaymentDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_text")]
        public string StatusText { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("payment_type")]
        public string PaymentType { get; set; }

    }
}