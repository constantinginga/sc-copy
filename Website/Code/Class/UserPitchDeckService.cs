using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using StartupCentral.Code.Model;
using StartupCentral.DAL.EntityModels;
using StartupCentral.DAL.Repositories;
using Umbraco.Core.Composing.CompositionExtensions;
using Umbraco.Core.Models;
using Umbraco.Web.Composing;

namespace StartupCentral.Code.Class
{
    public class UserPitchDeckService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserPitchDeckService()
        {
            _unitOfWork = new UnitOfWork();
        }

        //Get list
        public async Task<List<userPitch>> GetListOfAllPitchs(Guid userId)
        {
            var res = await _unitOfWork.PitchDeckRepository.WhereAsync(x => x.owner == userId);
            return res.OrderBy(x => x.CreateDate).ToList();
        }

        //Get list drafts
        public async Task<List<userPitch>> GetListOfDraftPitchs(Guid userId)
        {
            var res = await _unitOfWork.PitchDeckRepository.WhereAsync(x => (x.owner == userId && x.Completed == false));
            return res.ToList();
        }
        //Get list finished
        public async Task<List<userPitch>> GetListOfCompletedPitchs(Guid userId)
        {
            var res = await _unitOfWork.PitchDeckRepository.WhereAsync(x => (x.owner == userId && x.Completed == true));
            return res.ToList();
        }


        //Get pitch with ID 
        public async Task<List<userPitch>> GetPitchById(Guid userId, Guid pitchId)
        {
            var res = await _unitOfWork.PitchDeckRepository.WhereAsync(x => (x.owner == userId && x.Id == pitchId));
            return res.ToList();

        }


        //Get Pitch deck with ID for a coach/investor
        public async Task<List<userPitch>> GetPitchByIdCoach(Guid pitchId)
        {
            var res = await _unitOfWork.PitchDeckRepository.WhereAsync(x => x.Id == pitchId);
            return res.ToList();
        }

        //Get pitchPDF with ID 
        public List<userPitch> GetPitchByIdPDF(Guid userId, Guid pitchId)
        {
            var res = _unitOfWork.PitchDeckRepository.Get(x => (x.owner == userId && x.Id == pitchId));
            return res.ToList();
        }



        //Create
        public async Task<Guid> Create(Guid userId, PitchDeck pitchDeck)
        {
            userPitch pitchDb = new userPitch();

            pitchDb.Id = Guid.NewGuid();
            pitchDb.owner = userId;
            pitchDb.CreateDate = DateTime.Now;
            pitchDb.UpdateDate = DateTime.Now;
            pitchDb.Completed = false;

            //Intro
            pitchDb.PitchName = pitchDeck.PitchName;
            pitchDb.English = pitchDeck.English;
            pitchDb.Picture = pitchDeck.Picture;
            pitchDb.Agenda = pitchDeck.Agenda;


            _unitOfWork.PitchDeckRepository.Insert(pitchDb);
            await _unitOfWork.SaveAsync();
            return pitchDb.Id;

        }


        //Update
        public async Task<Guid> Update(Guid userId, PitchDeck model)
        {
            userPitch pitchDb = new userPitch();

            pitchDb.Id = model.Id;
            pitchDb.owner = userId;
            pitchDb.CreateDate = model.CreateDate;
            pitchDb.UpdateDate = DateTime.Now;
            pitchDb.Completed = model.Completed;

            //Intro
            pitchDb.PitchName = model.PitchName;
            pitchDb.English = model.English;
            pitchDb.Picture = model.Picture;
            pitchDb.Agenda = model.Agenda;

            //Reason
            pitchDb.LookFor = model.LookFor;
            pitchDb.MoneyUsage = model.MoneyUsage;

            //Company Info
            pitchDb.CompanyDesc = model.CompanyDesc;
            pitchDb.TeamDesc = model.TeamDesc;
            pitchDb.SatisfyDesc = model.SatisfyDesc;
            pitchDb.MarketDesc = model.MarketDesc;
            pitchDb.Traction = model.Traction;
            pitchDb.Collaborations = model.Collaborations;
            pitchDb.DevelopmentDesc = model.DevelopmentDesc;

            //Proof
            pitchDb.proofBusiness = model.proofBusiness;
            pitchDb.proofMarket = model.proofMarket;
            pitchDb.proofSales = model.proofSales;
            pitchDb.ProofScale = model.ProofScale;
            pitchDb.proofConceptPayment = model.proofConceptPayment;


            //Financial

            pitchDb.finBudget = model.finBudget;
            pitchDb.finVariety = model.finVariety;
            pitchDb.finGrowth = model.finGrowth;
            pitchDb.finFacts = model.finFacts;


            //Risk
            pitchDb.riskInvest = model.riskInvest;
            pitchDb.riskGoal = model.riskGoal;

            //looking for
            pitchDb.LookFor2 = model.LookFor2;
            pitchDb.Owners = model.Owners;

            //Follow-up
            pitchDb.Summary = model.Summary;

            _unitOfWork.PitchDeckRepository.Update(pitchDb);

            await _unitOfWork.SaveAsync();
            return pitchDb.Id;

        }







        //Delete Pitch with ID and file
        public async Task<bool> DeleteWithFile(Guid pitchId, string filePath)
        {

            
            try
            {
                ApplicationContext.Current.MediaService.DeleteMediaFile(filePath);
                _unitOfWork.PitchDeckRepository.Delete(pitchId);
                await _unitOfWork.SaveAsync();
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }

            return true;
        }
        //Delete Pitch with ID
        public async Task<bool> Delete(Guid pitchId)
        {


            try
            {
                
                _unitOfWork.PitchDeckRepository.Delete(pitchId);
                await _unitOfWork.SaveAsync();
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
            catch(NullReferenceException ex)
            {
                throw ex;
            }

            return true;
        }



        //public void Delete(int mediaId)
        //{
        //    try
        //    {

        //        IMedia media = ApplicationContext.Current.MediaService.GetById(mediaId);
        //        if (media != null)
        //            ApplicationContext.Current.MediaService.Delete(media);



        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        //Migrate
        public async Task<bool> Migrate(List<userPitch> list)
        {
            try
            {
                list = list.Where(x => x != null).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    _unitOfWork.PitchDeckRepository.Insert(list[i]);
                }
                await _unitOfWork.SaveAsync();
            }
            catch (DbEntityValidationException ex)
            {

                throw ex;
            }

            return true;
        }
    }

}