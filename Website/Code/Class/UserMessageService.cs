using StartupCentral.Code.Model;
using StartupCentral.DAL.EntityModels;
using StartupCentral.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Core.Composing;

namespace StartupCentral.Code.Class
{
    public class UserMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserMessageService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public List<userMessages> GetUserMessags(Guid userId)
        {
           return  _unitOfWork.UserMessagRepository.Get(x => x.FromUser == userId || x.ToUser == userId).ToList();
        }

        public async Task<Guid> Save(Guid fromUser, Guid toUser, Besked message)
        {
            var userMessagesDb = new userMessages()
            {
                Id = Guid.NewGuid(),
                Body = message.besked,
                CreateDate = DateTime.Now,
                FromUser = fromUser,
                ToUser = toUser,
                Subject = message.emne,
                Read = false
            };

            _unitOfWork.UserMessagRepository.Insert(userMessagesDb);
            await _unitOfWork.SaveAsync();
            return userMessagesDb.Id;
        }

        public async Task<Guid> SaveMsgWithPlan(Guid fromUser, Guid toUser, BeskedWithPlane message, Guid plan)
        {
            var userMessagesDb = new userMessages();
            userMessagesDb.Id = Guid.NewGuid();
            userMessagesDb.CreateDate = DateTime.Now;
            //userMessagesDb.Subject = $"Ny forretningsplan: {plan.GetValue<string>("wwprojektnavn")}";
            userMessagesDb.Body = message.besked ?? string.Empty;
            userMessagesDb.FromUser = fromUser;
            userMessagesDb.ToUser = toUser;
            userMessagesDb.Read = false;

            userMessagesDb.Attachment = $"umb://document/{plan.ToString().Replace("-", string.Empty)}";

            userMessagesDb.PitchAttatchment = null;


            _unitOfWork.UserMessagRepository.Insert(userMessagesDb);
            await _unitOfWork.SaveAsync();
            return userMessagesDb.Id;
        }
        public async Task<Guid> SaveMsgWithPitch(Guid fromUser, Guid toUser, BeskedWithPlane message,Guid pitch)
        {
            var userMessagesDb = new userMessages();
            userMessagesDb.Id = Guid.NewGuid();
            userMessagesDb.CreateDate = DateTime.Now;
            //userMessagesDb.Subject = $"Ny forretningsplan: {plan.GetValue<string>("wwprojektnavn")}";
            userMessagesDb.Body = message.besked ?? string.Empty;
            userMessagesDb.FromUser = fromUser;
            userMessagesDb.ToUser = toUser;
            userMessagesDb.Read = false;

            userMessagesDb.Attachment = null;

            userMessagesDb.PitchAttatchment = $"umb://document/{pitch.ToString().Replace("-", string.Empty)}";

            _unitOfWork.UserMessagRepository.Insert(userMessagesDb);
            await _unitOfWork.SaveAsync();
            return userMessagesDb.Id;
        }
        public async Task<Guid> Save(Guid fromUser, Guid toUser, BeskedWithPlane message, Guid plan, Guid pitch)
        {
            var userMessagesDb = new userMessages();
            userMessagesDb.Id = Guid.NewGuid();
            userMessagesDb.CreateDate = DateTime.Now;
            //userMessagesDb.Subject = $"Ny forretningsplan: {plan.GetValue<string>("wwprojektnavn")}";
            userMessagesDb.Body = message.besked ?? string.Empty;
            userMessagesDb.FromUser = fromUser;
            userMessagesDb.ToUser = toUser;
            userMessagesDb.Read = false;

                userMessagesDb.Attachment = $"umb://document/{plan.ToString().Replace("-", string.Empty)}";

                userMessagesDb.PitchAttatchment = $"umb://document/{pitch.ToString().Replace("-", string.Empty)}";
            
            _unitOfWork.UserMessagRepository.Insert(userMessagesDb);
            await _unitOfWork.SaveAsync();
            return userMessagesDb.Id;
        }




        public async Task<List<Guid>> GetUnreadMessages(Guid userId)
        {
            var unreadMessage = await _unitOfWork.UserMessagRepository.WhereAsync(x => x.ToUser == userId && !x.Read);
            return unreadMessage.Select(x => x.Id).ToList();
        }

        //depricated
        public async System.Threading.Tasks.Task MarkRead(Guid toUser, Guid fromUser)
        {
            var unreadMessage = await _unitOfWork.UserMessagRepository.WhereAsync(x => x.ToUser == toUser && x.FromUser == fromUser  && !x.Read);
            unreadMessage.ToList().ForEach(x => {
                x.Read = true;
                _unitOfWork.UserMessagRepository.Update(x);
                });
            await _unitOfWork.SaveAsync();
        }

        public async System.Threading.Tasks.Task MarkRead(List<userMessages> unreadMessage)
        {
            unreadMessage.ToList().ForEach(x => {
                x.Read = true;
                _unitOfWork.UserMessagRepository.Update(x);
            });

            await _unitOfWork.SaveAsync();
        }

        public async Task<List<Guid>> GetRecipientKeys(Guid User)
        {
            List<userMessages> mes = (await _unitOfWork.UserMessagRepository.WhereAsync(x => x.ToUser == User || x.FromUser == User)).ToList();
            mes = mes.OrderByDescending(x => x.CreateDate).ToList();
            var res = new List<Guid>();
            for (int i = 0; i < mes.Count(); i++)
            {
                if (mes[i].FromUser != User && !res.Contains(mes[i].FromUser))
                    res.Add(mes[i].FromUser);

                if (mes[i].ToUser!= User && !res.Contains(mes[i].ToUser))
                    res.Add(mes[i].ToUser);
            };
            res.Remove(User);
            return res;
        }

        public async Task<List<userMessages>> GeMessages(Guid User1, Guid User2)
        {
            var res = await _unitOfWork.UserMessagRepository.WhereAsync(x => (x.ToUser == User1 && x.FromUser == User2)
            || (x.ToUser == User2 && x.FromUser == User1));

            return res.OrderBy(x => x.CreateDate).ToList();
        }

        //public async Task<userMessages> GetLatestMsgFromConversation(Guid User1, Guid User2)
        //{
        //    var res = await _unitOfWork.UserMessagRepository.WhereAsync(x => (x.ToUser == User1 && x.FromUser == User2)
        //    || (x.ToUser == User2 && x.FromUser == User1));

        //    res.OrderByDescending(x => x.CreateDate).ToList();

        //    return res.First();
        //}

        public async Task<List<Coach2StudMsgsReport>> GetAllMsgsFromCoach(Guid coachKey)
        {
            var res = new List<Coach2StudMsgsReport>();
            var memberService = Current.Services.MemberService;
            var coach = memberService.GetByKey(coachKey);
            DateTime nowDt = DateTime.Now;
            DateTime lastMonth = nowDt.AddMonths(-1);
            var coachMsgs = _unitOfWork.UserMessagRepository.Where(x => x.FromUser == coachKey && x.CreateDate >= lastMonth);
            
            foreach (var coachMessage in coachMsgs)
            {
                

                var student = memberService.GetByKey(coachMessage.ToUser);
                if (student != null)
                {

                    bool msgExists = res.Any(x => x.StudentId == student.Id);

                        if(!msgExists && student != null && coach != null)
                        {
                            var msgs =  await GeMessages(coachKey, student.Key);

                            res.Add(new Coach2StudMsgsReport
                            {
                               CoachId = coach.Id,
                               CoachName = coach.Name,
                               StudentId = student.Id,
                               StudentName = student.Name,
                               NumberOfMessages = msgs.Where(x => x.FromUser==coachKey).Count(),
                               LastMsgDate = msgs.Where(x => x.FromUser == coachKey).Last().CreateDate
                            });

                        }
                }



                
            }
            return res;
        }

        public async Task<bool> migrate(List<userMessages> list )
        {
            try
            {
                list = list.Where(x => x != null).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    _unitOfWork.UserMessagRepository.Insert(list[i]);
                }
                await _unitOfWork.SaveAsync();
            }
            catch (DbEntityValidationException  ex)
            {

                throw ex;
            }

            return true;
        }
    }
}