using StartupCentral.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace StartupCentral.Code.Model
{
    public class PitchDeck
    {
        public System.Guid Id { get; set; }
        public System.Guid Owner { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool Completed { get; set; }
        public string PitchName { get; set; }
        public bool English { get; set; }
        public string Picture { get; set; }
        public string Agenda { get; set; }
        public string LookFor { get; set; }
        public string MoneyUsage { get; set; }
        public string CompanyDesc { get; set; }
        public string TeamDesc { get; set; }
        public string SatisfyDesc { get; set; }
        public string MarketDesc { get; set; }
        public string Traction { get; set; }
        public string Collaborations { get; set; }
        public string DevelopmentDesc { get; set; }
        public string proofBusiness { get; set; }
        public string proofMarket { get; set; }
        public string proofSales { get; set; }
        public string proofConceptPayment { get; set; }
        public string finBudget { get; set; }
        public string finVariety { get; set; }
        public string finGrowth { get; set; }
        public string finFacts { get; set; }
        public string riskInvest { get; set; }
        public string riskGoal { get; set; }
        public string Owners { get; set; }
        public string Summary { get; set; }
        public string ProofScale { get; set; }

        public string LookFor2 { get; set; }
    }
}