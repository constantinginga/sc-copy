using StartupCentral.DAL.EntityModels;
using StartupCentral.DAL.Repositories;
using StartupCentral.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StartupCentral.Code.Class
{
    public class NewsLetterSubscriptionsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsLetterSubscriptionsService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public Guid Create(string name, string email)
        {
            if (!email.IsEmailAddress())
                return Guid.Empty;

            if (GetByEmail(email) != null)
                return Guid.Empty;

            var newsLetterSubscriptionsDb = new newsLetterSubscriptions()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email.ToLower()
            };

            _unitOfWork.NewsLetterSubscriptionsRepository.Insert(newsLetterSubscriptionsDb);
            _unitOfWork.Save();
            Emailing.SendNewsLetterSubscriptions(name, email);
            return newsLetterSubscriptionsDb.Id;
        }

        public newsLetterSubscriptions GetByEmail(string email) => _unitOfWork.NewsLetterSubscriptionsRepository.FirstOrDefault(x => x.Email == email.ToLower());

        public void Unsubscribe(string email)
        {
            var subscribe = GetByEmail(email);
            (subscribe != null ?
                (Action<newsLetterSubscriptions>)((s) => Delete(s.Id)) :
                (Action<newsLetterSubscriptions>)((s) => { }))
                (subscribe);
        }

        public void Delete(Guid SubscriptionId)
        {
            _unitOfWork.NewsLetterSubscriptionsRepository.Delete(SubscriptionId);
            _unitOfWork.Save();
        }

        public IEnumerable<newsLetterSubscriptions> GetAll() => _unitOfWork.NewsLetterSubscriptionsRepository.GetAll();

    }
}