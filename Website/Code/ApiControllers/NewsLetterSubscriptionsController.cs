using StartupCentral.Code.Class;
using Umbraco.Web.WebApi;
using System.Collections.Generic;
using System.Linq;
using StartupCentral.DAL.EntityModels;
using System.Web.Http;
using System;

namespace StartupCentral.Code.ApiControllers
{
    public class NewsLetterSubscriptionsController : UmbracoApiController
    {
        private readonly NewsLetterSubscriptionsService _newsLetterSubscriptionsService;

        public NewsLetterSubscriptionsController()
        {
            _newsLetterSubscriptionsService = new NewsLetterSubscriptionsService();
        }

        [HttpGet]
        public IEnumerable<SubscriptionDTO> GetAll()
        {
            return _newsLetterSubscriptionsService.GetAll().Select(x => SubscriptionDTO.FromDB(x));
        }

        [HttpGet]
        public void Unsubscribe(string email) => _newsLetterSubscriptionsService.Unsubscribe(email);

    }

    public class SubscriptionDTO
    {
        public readonly string Name;
        public readonly string Email;

        private SubscriptionDTO(newsLetterSubscriptions dbModel)
        {
            this.Name = dbModel.Name;
            this.Email = dbModel.Email;
        }

        public static SubscriptionDTO FromDB(newsLetterSubscriptions dbModel) => new SubscriptionDTO(dbModel);
    }
}