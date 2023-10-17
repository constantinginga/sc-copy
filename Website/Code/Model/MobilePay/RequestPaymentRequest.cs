using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model.MobilePay
{
    public class RequestPaymentRequest
    {
        [JsonProperty("agreement_id")]
        public string AgreementId { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("due_date")]
        public string DueDate { get; set; }
        [JsonProperty("next_payment_date")]
        public string NextPaymentDate { get; set; }
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

}