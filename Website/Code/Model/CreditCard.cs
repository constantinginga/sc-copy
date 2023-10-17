using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model
{
    public class CreditCard
    {
        public string Issuer { get; set; }
        public string FullName { get; set; }
        public string ExpiryDate { get; set; }

        public CreditCard(string Issuer, string FullName, DateTime? ExpiryDate)
        {
            this.Issuer = Issuer.ToUpper();
            this.FullName= FullName;
            if (ExpiryDate.HasValue)
            {
                this.ExpiryDate = ExpiryDate.Value.ToString("MM/yyyy");
            }

        }
    }
}