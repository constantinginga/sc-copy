using StartupCentral.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace StartupCentral.Code.Model
{
    public class BusinessPlan
    {
        public System.Guid Id { get; set; }

        public System.Guid Owner { get; set; }

        public System.DateTime CreateDate { get; set; }

        public System.DateTime UpdateDate { get; set; }

        public bool Completed { get; set; }

        public string ProjektNavn { get; set; }

        public string Kategori { get; set; }

        public bool English { get; set; }

        public string IdeDesc { get; set; }

        public string Uddyb { get; set; }

        public string Loser { get; set; }

        public string Videreudvik { get; set; }

        public string Brugprod { get; set; }

        public string Splitaktiviteter { get; set; }

        public string FirmImportant { get; set; }

        public string FirmBehov { get; set; }

        public string FirmReach { get; set; }

        public string FirmIssues { get; set; }

        public string FirmValues { get; set; }

        public string FirmRoller { get; set; }

        public string FirmOkonomi { get; set; }

        public string FirmRes { get; set; }

        public string FirmDriv { get; set; }

        public string FirmVwork { get; set; }

        public string ProdDesc { get; set; }

        public string ProdVers { get; set; }

        public string ProdFunkt { get; set; }

        public string ProdMake { get; set; }

        public string ProdReady { get; set; }

        public string CusWho { get; set; }

        public string CusFind { get; set; }

        public string CusNeeds { get; set; }

        public string CusKon { get; set; }

        public string CusStrong { get; set; }

        public string ProdSale { get; set; }

        public string ProdStart { get; set; }

        public string BudgetFile { get; set; }

        public string ProdFinance { get; set; }

        public string ProdPrice { get; set; }

        public string ProdUdregn { get; set; }

        public string ProdSaleplace { get; set; }

        public string ProdEksi { get; set; }

        public string ProdMarkedsf { get; set; }

        public string JurAftale { get; set; }

        public string JurLeverand { get; set; }

        public string JurContact { get; set; }

        public string JurCvr { get; set; }

        public bool AndetChk { get; set; }

        public string AndetHeadline { get; set; }

        public string Andet { get; set; }

        public string Splitaktiviteter3ar { get; set; }

        public string Splitaktiviteter5ar { get; set; }

        public string Firmres2 { get; set; }

        public string Firmvinkel { get; set; }


        //public string BudgetName { get; set; }
        //public System.IO.Stream BudgetStream { get; set; }
    }

    public class Forretningsplan
    {
        private IMember _currentMember = null;

        public Forretningsplan(IContent forretningsplan, IMember currentMember)
        {
            this._currentMember = currentMember;

            this.Content = forretningsplan;
            if (Content != null)
            {
                var parent = ApplicationContext.Current.Services.ContentService.GetParent(Content);

                if (parent != null)
                {
                    if (parent.GetValue("memberid") != null)
                    {
                        if (parent.GetValue<string>("memberid").IsNumeric())
                        {
                            this.Member = ApplicationContext.Current.Services.MemberService.GetById(parent.GetValue<int>("memberid"));
                        }
                    }
                }

                this.Status = Database.ScForretningsplanRepository.GetByDocumentIdAndMemberId(Content.Id, currentMember.Id);
            }
        }

        public IContent Content { get; set; }

        public IMember Member { get; set; }

        private Database.ScForretningsplan Status { get; set; }

        public bool Seen
        {
            get
            {
                if (this.Status != null)
                {
                    if (this.Status.Seen)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool Downloaded
        {
            get
            {
                if (this.Status != null)
                {
                    if (this.Status.Downloaded)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool SetSeen()
        {
            Database.ScForretningsplan forretningsplan = Database.ScForretningsplanRepository.GetByDocumentIdAndMemberId(this.Content.Id, _currentMember.Id);
            if (forretningsplan == null)
            {
                forretningsplan = new Database.ScForretningsplan() { DocumentId = this.Content.Id, MemberId = this._currentMember.Id, Seen = true };
            }
            else
            {
                forretningsplan.Seen = true;
            }

            return Database.ScForretningsplanRepository.Save(forretningsplan) != null;
        }

        public bool SetDownloaded()
        {
            Database.ScForretningsplan forretningsplan = Database.ScForretningsplanRepository.GetByDocumentIdAndMemberId(this.Content.Id, _currentMember.Id);
            if (forretningsplan == null)
            {
                forretningsplan = new Database.ScForretningsplan() { DocumentId = this.Content.Id, MemberId = this._currentMember.Id, Seen = true, Downloaded = true };
            }
            else
            {
                forretningsplan.Downloaded = true;
            }

            return Database.ScForretningsplanRepository.Save(forretningsplan) != null;
        }
    }
}