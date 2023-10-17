using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StartupCentral.Code.Model;
using Stripe;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.ApiControllers
{
    [Route("secret")]
    public class StripeApiController : UmbracoApiController
    {
        [HttpGet]
        public ApiResponse Get()
        {
            StripeConfiguration.ApiKey = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Stripe.SecretKey"];
            var options = new CustomerCreateOptions { };

            var service = new CustomerService();
            var customer = service.Create(options);

            var intentOptions = new SetupIntentCreateOptions
            {
                Customer = customer.Id,
                PaymentMethodTypes = new List<string> { "bancontact", "card", "ideal" },
            };
            var intentService = new SetupIntentService();
            var intent = intentService.Create(intentOptions);

            var clientSecretObj = new ClientSecretObj();
            clientSecretObj.client_secret = intent.ClientSecret;

            return new ApiResponse(true, JsonConvert.SerializeObject(clientSecretObj));
        }

        // Get Stripe customer
        [HttpGet("getcustomer/{id}")]
        public ApiResponse GetCustomer(string id)
        {
            StripeConfiguration.ApiKey = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Stripe.SecretKey"];
            var service = new SetupIntentService();
            var customer = service.Get(id).CustomerId;
            return new ApiResponse(true, JsonConvert.SerializeObject(customer));
        }

        // Create Stripe customer
        //[HttpPost("createcustomer")]
        //public ApiResponse CreateCustomer([FromBody] Customer customer)
        //{
        //    StripeConfiguration.ApiKey = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Stripe.SecretKey"];
        //    var options = new CustomerCreateOptions
        //    {
                
        //        PaymentMethod = customer.Token,
        //        Email = customer.UserId,
        //        InvoiceSettings = new CustomerInvoiceSettingsOptions
        //        {
        //            DefaultPaymentMethod = paylikeCreditCardResult.Token,
        //        },
        //    };
        //    var service = new CustomerService();
        //    var customer = service.Create(options);
        //    return new ApiResponse(true, JsonConvert.SerializeObject(customer));
        //}

    }

    public class ClientSecretObj
    {
        public string client_secret { get; set; }
    }
}