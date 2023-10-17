using System;

namespace StartupCentral.Code.Model
{
    public class PaylikeCreditCardResult
    {
        public int MemberNodeId { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }

        public string PromoKode { get; set; }

        public int SubscriptionType { get; set; }

        public override string ToString()
        {
            return string.Format("PaylikeCreditCardResult: UserId {0} Token {1}", this.UserId.ToString(), this.Token);
        }
    }
}
