using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using StartupCentral.Code.Model;
using StartupCentral.DAL.EntityModels;
using StartupCentral.DAL.Repositories;

namespace StartupCentral.Code.Class
{
    public class ForretningsPlanService
    {

        private readonly IUnitOfWork _unitOfWork;

        public ForretningsPlanService()
        {
            _unitOfWork = new UnitOfWork();
        }

        //Get list
        public async Task<List<userForretningsPlan>> GetListOfAllBusinessPlans(Guid userId)
        {
            var res = await _unitOfWork.ForretningsPlanRepository.WhereAsync(x => x.Owner == userId);
            return res.OrderBy(x => x.CreateDate).ToList();
        }

        //Get list drafts
        public async Task<List<userForretningsPlan>> GetListOfDraftBusinessPlans(Guid userId)
        {
            var res = await _unitOfWork.ForretningsPlanRepository.WhereAsync(x => (x.Owner == userId && x.Completed == false));
            return res.ToList();
        }

        //Get list finished
        public async Task<List<userForretningsPlan>> GetListOfCompletedBusinessPlans(Guid userId)
        {
            var res = await _unitOfWork.ForretningsPlanRepository.WhereAsync(x => (x.Owner == userId && x.Completed == true));
            return res.ToList();
        }

        //Get BussinessPlan with ID 
        public async Task<List<userForretningsPlan>> GetBusinessPlanById(Guid userId, Guid forretningsPlanId)
        {
            var res = await _unitOfWork.ForretningsPlanRepository.WhereAsync(x => (x.Owner == userId && x.Id == forretningsPlanId));
            return res.ToList();
            
        }

        //Get BusinessPlanPDF with ID 
        public List<userForretningsPlan> GetBusinessPlanByIdPDF(Guid userId, Guid forretningsPlanId)
        {
            var res = _unitOfWork.ForretningsPlanRepository.Get(x => (x.Owner == userId && x.Id == forretningsPlanId));
            return res.ToList();
        }

        //Get BussinessPlan with ID for a coach/investor
        public async Task<List<userForretningsPlan>> GetBusinessPlanByIdCoach(Guid forretningsPlanId)
        {
            var res = await _unitOfWork.ForretningsPlanRepository.WhereAsync(x => x.Id == forretningsPlanId);
            return res.ToList();
        }

        //Create
        public async Task<Guid> Create(Guid userId, BusinessPlan plan)
        {
            userForretningsPlan forretningsPlanDb = new userForretningsPlan();

            forretningsPlanDb.Id = Guid.NewGuid();
            forretningsPlanDb.Owner = userId;
            forretningsPlanDb.CreateDate = DateTime.Now;
            forretningsPlanDb.UpdateDate = DateTime.Now;
            forretningsPlanDb.Completed = false;

            //Introduktion
            forretningsPlanDb.ProjektNavn = plan.ProjektNavn;
            forretningsPlanDb.Kategori = plan.Kategori;
            forretningsPlanDb.English = plan.English;
            forretningsPlanDb.IdeDesc = plan.IdeDesc;

            _unitOfWork.ForretningsPlanRepository.Insert(forretningsPlanDb);
            await _unitOfWork.SaveAsync();
            return forretningsPlanDb.Id;
        }

        //Update
        public async Task<Guid> Update(Guid userId, BusinessPlan plan)
        {
            userForretningsPlan forretningsPlanDb = new userForretningsPlan();

            forretningsPlanDb.Id = plan.Id;
            forretningsPlanDb.Owner = userId;
            forretningsPlanDb.CreateDate = plan.CreateDate;
            forretningsPlanDb.UpdateDate = DateTime.Now;
            forretningsPlanDb.Completed = plan.Completed;

            //Introduktion
            forretningsPlanDb.ProjektNavn = plan.ProjektNavn;
            forretningsPlanDb.Kategori = plan.Kategori;
            forretningsPlanDb.English = plan.English;
            forretningsPlanDb.IdeDesc = plan.IdeDesc;

            //Idegrundlag
            forretningsPlanDb.Uddyb = plan.Uddyb;
            forretningsPlanDb.Loser = plan.Loser;
            forretningsPlanDb.Videreudvik = plan.Videreudvik;
            forretningsPlanDb.Brugprod = plan.Brugprod;

            //Målsætning
            forretningsPlanDb.Splitaktiviteter = plan.Splitaktiviteter;
            forretningsPlanDb.Splitaktiviteter3ar = plan.Splitaktiviteter3ar;
            forretningsPlanDb.Splitaktiviteter5ar = plan.Splitaktiviteter5ar;

            //Værdier
            forretningsPlanDb.FirmImportant = plan.FirmImportant;
            forretningsPlanDb.FirmBehov = plan.FirmBehov;
            forretningsPlanDb.FirmReach = plan.FirmReach;
            forretningsPlanDb.FirmIssues = plan.FirmIssues;
            forretningsPlanDb.FirmValues = plan.FirmValues;
            forretningsPlanDb.Firmvinkel = plan.Firmvinkel;

            //Team og ressourcer
            forretningsPlanDb.FirmRoller = plan.FirmRoller;
            forretningsPlanDb.FirmOkonomi = plan.FirmOkonomi;
            forretningsPlanDb.FirmRes = plan.FirmRes;
            forretningsPlanDb.Firmres2 = plan.Firmres2;
            forretningsPlanDb.FirmDriv = plan.FirmDriv;
            forretningsPlanDb.FirmVwork = plan.FirmVwork;

            //Produktbeskrivelse
            forretningsPlanDb.ProdDesc = plan.ProdDesc;
            forretningsPlanDb.ProdVers = plan.ProdVers;
            forretningsPlanDb.ProdFunkt = plan.ProdFunkt;
            forretningsPlanDb.ProdMake = plan.ProdMake;
            forretningsPlanDb.ProdReady = plan.ProdReady;

            //Markedsanalyse
            forretningsPlanDb.CusWho = plan.CusWho;
            forretningsPlanDb.CusFind = plan.CusFind;
            forretningsPlanDb.CusNeeds = plan.CusNeeds;
            forretningsPlanDb.CusKon = plan.CusKon;
            forretningsPlanDb.CusStrong = plan.CusStrong;

            //Økonomi
            forretningsPlanDb.ProdSale = plan.ProdSale;
            forretningsPlanDb.ProdStart = plan.ProdStart;

            if (plan.BudgetFile != null)
            {
                forretningsPlanDb.BudgetFile = $"{plan.BudgetFile}";
            }

            forretningsPlanDb.ProdFinance = plan.ProdFinance;

            //Salgsmarkedsføring
            forretningsPlanDb.ProdPrice = plan.ProdPrice;
            forretningsPlanDb.ProdUdregn = plan.ProdUdregn;
            forretningsPlanDb.ProdSaleplace = plan.ProdSaleplace;
            forretningsPlanDb.ProdEksi = plan.ProdEksi;
            forretningsPlanDb.ProdMarkedsf = plan.ProdMarkedsf;

            //Juridisk
            forretningsPlanDb.JurAftale = plan.JurAftale;
            forretningsPlanDb.JurLeverand = plan.JurLeverand;
            forretningsPlanDb.JurContact = plan.JurContact;
            forretningsPlanDb.JurCvr = plan.JurCvr;

            if (plan.AndetChk == true)
            {
                forretningsPlanDb.AndetChk = plan.AndetChk;
                forretningsPlanDb.AndetHeadline = plan.AndetHeadline;
                forretningsPlanDb.Andet = plan.Andet;
            }

            forretningsPlanDb.Completed = plan.Completed;

            _unitOfWork.ForretningsPlanRepository.Update(forretningsPlanDb);

            await _unitOfWork.SaveAsync();
            return forretningsPlanDb.Id;
        }


        //Delete BussinessPlan with ID
        public async Task<bool> Delete(Guid forretningsPlanId)
        {
            try
            {
                _unitOfWork.ForretningsPlanRepository.Delete(forretningsPlanId);
                await _unitOfWork.SaveAsync();
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }

            return true;
        }

        //Migrate
        public async Task<bool> Migrate(List<userForretningsPlan> list)
        {
            try
            {     
                list = list.Where(x => x != null).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    _unitOfWork.ForretningsPlanRepository.Insert(list[i]);
                }
                await _unitOfWork.SaveAsync();
            }
            catch (DbEntityValidationException ex)
            {

                throw ex;
            }

            return true;
        }

        public List<BPMonthly> GetAllPerMonth()
        {
            var currentDate = DateTime.Now;
            var allBP = _unitOfWork.ForretningsPlanRepository.GetAll();

            var monthlyCounts = allBP
                .Where(bp => bp.CreateDate <= currentDate) // Exclude future months
                .GroupBy(bp => new { Year = bp.CreateDate.Year, Month = bp.CreateDate.Month })
                .Select(group => new BPMonthly
                {
                    Date = $"{group.Key.Year}-{group.Key.Month:00}", // Format year and month as "yyyy-MM"
                    Count = group.Count()
                })
                .OrderBy(monthly => monthly.Date) // Order by year and month
                .ToList();

            return monthlyCounts;
        }


        public class BPMonthly
        { 
            public string Date { get; set; }
            public int Count { get; set; }
        }

    }
}