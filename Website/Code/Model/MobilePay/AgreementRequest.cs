using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model.MobilePay
{
    public class AgreementRequest
    {
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = "DKK";

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("next_payment_date")]
        public string NextPaymentDate { get; set; } = $"{FirstWeekDayOfNextMonth().ToString("yyyy-MM-dd")}T{FirstWeekDayOfNextMonth().ToString("HH:mm")}:00.000Z";

        [JsonProperty("frequency")]
        public int Frequency { get; set; } = 12;

        [JsonProperty("links")]
        public List<Link> Links { get; set; } = new List<Link>();

        [JsonProperty("country_code")]
        public string CountryCode { get; set; } = "DK";

        [JsonProperty("plan")]
        public string Plan { get; set; } = "Basic";

        [JsonProperty("expiration_timeout_minutes")]
        public int ExpirationTimeoutMinutes { get
            {
                return 20160;
            }
        }

        [JsonProperty("mobile_phone_number")]
        public long MobilePhoneNumber { get; set; }

        [JsonProperty("one_off_payment")]
        public OneOffPayment OneOffPayment { get; set; }

        private static DateTime FirstWeekDayOfNextMonth()
        {
            return DateTime.Now.AddMonths(1);

            //DateTime dato = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1);
            //while (dato.DayOfWeek == DayOfWeek.Saturday || dato.DayOfWeek == DayOfWeek.Sunday)
            //{
            //    dato = dato.AddDays(1);
            //}

            //if ((dato - DateTime.Now).Days <= 8)
            //{
            //    dato = DateTime.Now.AddDays(8);
            //}

            //return dato;
        }


    }

    public class OneOffPayment
    {

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class OneOffPaymentResponse
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("agreement_id")]
        public string AgreementId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }


    }


}