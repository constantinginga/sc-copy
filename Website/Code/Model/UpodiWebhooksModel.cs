using System;

namespace StartupCentral.Code.Model
{
    public class UpodiWebhooksModel
    {
        public Guid ID { get; set; }

        public DateTime Time { get; set; }

        public string Signature { get; set; }

        public string Action { get; set; }

        public Issuer Issuer { get; set; }

        public DateTime Data { get; set; }

        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("UpodiWebhooks: Type: {0}, Action:{1}, Url:{2} Signature: {3}", Type, Action, Issuer.Url, Signature);
        }
    }

    public class Issuer
    {
        public string Url { get; set; }

        public Guid Identifier { get; set; }
    }
}
