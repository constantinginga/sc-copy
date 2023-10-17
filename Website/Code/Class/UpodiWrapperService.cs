using System;
using System.Configuration;
using System.Net;
using System.Text;
using Umbraco.Core.Models;
using Upodi.SDK;
using Upodi.SDK.Objects;
using Upodi.SDK.Objects.Enums;
using Upodi.SDK.Managers;
using System.Linq;
using StartupCentral.Code.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using StartupCentral.Extensions;
using System.Net.Http;
using RestSharp;
using System.Web.Mvc;
using Upodi.SDK.Objects.Request;
using Microsoft.AspNet.SignalR.Messaging;
using Upodi.SDK.PaymentProviders;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;

namespace StartupCentral.Code.Class
{
    public class UpodiWrapperService
    {
        private static string AccessKey = ConfigurationManager.AppSettings["StartupCentral.Upodi.AccessKey"];
        //private static string ProductPlanId = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId"];

        private static readonly string ProductPlanId1 = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId1"];

        
        
        private UpodiClient upodi;

        public UpodiWrapperService()
        {
            this.upodi = new UpodiClient(UpodiWrapperService.AccessKey);
        }

        public bool IsCustomerExist(int memberNodeId)
        {
            //return false;

            var memberId = Task.Run(() => this.GetByAccountNumber(memberNodeId)).GetAwaiter().GetResult();
            return memberId != null;
            //return this.GetByAccountNumber(memberNodeId) != null;
        }

        public async Task<Customer> GetByAccountNumber(int memberNodeId)
        {
            try
            {
                //Customer result = null;
                //var outerDelay = Task.Delay(1557);
                //var resTask = Task.Factory.StartNew(async () =>
                //{
                //    //result = this.upodi.Customers.GetByAccountNumber(memberNodeId.ToString("D"));


                //    // var queryResult = await upodi.Customers.Query(c => c.AccountNumber == memberNodeId.ToString("D"));
                //    var customerList = await upodi.Customers.ListAsync();
                //    result = customerList.Results.Where(c => c.AccountNumber == memberNodeId.ToString("D")).FirstOrDefault();
                //    //result = queryResult.Results.First();
                //    /*if (result == null && System.Diagnostics.Debugger.IsAttached)
                //    {
                //        System.Diagnostics.Debugger.Break();
                //    }*/
                //    if (result == null)
                //    {
                //        Task.Delay(123).GetAwaiter().GetResult();
                //        //result = queryResult.Results.FirstOrDefault();
                //    }
                //});

                //Task.WaitAny(resTask, outerDelay);

                var queryResult = await upodi.Customers.Query(c => c.AccountNumber == memberNodeId.ToString("D"));
                var result = queryResult.Results.First();
                return result;
            }
            catch (Exception)
            {
                return (Customer)null;
            }
        }

        public void CreateNewCustomer(int memberNodeId, string fullName, string primaryEmail, string currencyCode = "DKK")
        { 

            if (this.IsCustomerExist(memberNodeId))
                throw new Exception("Make sure you are not running an old DB against Upodi PROD !");

            //Customer customer = new()
            //{
            //    AccountNumber = memberNodeId.ToString("D"),
            //    FullName = fullName,
            //    CurrencyCode = currencyCode,
            //    PrimaryEmail = primaryEmail,
            //    AutoBill = true,
            //    Country = "Denmark"
            //};

            CreateCustomer customer = new CreateCustomer(null)
            {
                FullName = fullName,
                PrimaryEmail = primaryEmail,
                CurrencyCode = currencyCode,
                AccountNumber = memberNodeId.ToString("D"),
                AutoBill = true,
                Country = "Denmark"
            };


            // var res = Guid.Empty;
            // Step 1 https://docs.upodi.com/page/sign-up-flow#step-1---to-create-a-new-customer-use-the-api-or-our-c-sdk

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var outerDelay = Task.Delay(3757);
            while(!this.IsCustomerExist(memberNodeId))
            {
                var resTask = Task.Factory.StartNew(async () =>
                {
                    await this.upodi.Customers.CreateAsync(customer);
                }).ConfigureAwait(false);
                var innerDelay = Task.Delay(300);
                Task.WaitAny(resTask.GetAwaiter().GetResult(), innerDelay);
                if (outerDelay.IsCompleted) break;
            }

            System.Diagnostics.Debug.WriteLine("CreateNewCustomer stopwatch ms: " + stopwatch.ElapsedMilliseconds);
        }

        public void CreateNewCustomer(int memberNodeId, IMember member)
        {
            this.CreateNewCustomer(memberNodeId, (string)((IContentBase)member).GetValue<string>("wwname"), (string)((IContentBase)member).GetValue<string>("wwemail"), "DKK");
        }

        public Guid АssginProductPlanId(int sub)
        {
            string ProductPlanId1 = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId1"];
            string ProductPlanId3 = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId3"];
            string ProductPlanId8 = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId8"];
            string ProductPlanId12 = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId12"];
            string ProductPlanId16 = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId16"];
            string ProductPlanIdS = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanIdS"];
            string ProductPlanIdP = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanIdP"];
            string ProductPlanIdPM = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanIdPM"];
            string ProductPlanIdBasic = ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanIdBasic"];

            Guid ProductPlanId = new Guid(ProductPlanId1);

            switch (sub)
            {
                case 0:
                    ProductPlanId = new Guid(ProductPlanIdS);
                    break;
                case 3:
                    ProductPlanId = new Guid(ProductPlanId3);
                    break;
                case 8:
                    ProductPlanId = new Guid(ProductPlanId8);
                    break;
                case 12:
                    ProductPlanId = new Guid(ProductPlanId12);
                    break;
                case 16:
                    ProductPlanId = new Guid(ProductPlanId16);
                    break;
                case 18:
                    ProductPlanId = new Guid(ProductPlanIdP);
                    break;
                case 20:
                    ProductPlanId = new Guid(ProductPlanIdPM);
                    break;
                case 22:
                    // basic user
                    ProductPlanId = new Guid(ProductPlanIdBasic);
                    break;
            }

            //Guid ProductPlanId = sub switch
            //{
            //    3 => new Guid(ProductPlanId12),
            //    12 => new Guid(ProductPlanId3),
            //    _ => new Guid(ProductPlanId1)
            //};

            return ProductPlanId;
        }

        public async Task ActivateSubscription(Guid SubscriptionId)
        {
            try
            {
                // Step 5 https://docs.upodi.com/page/sign-up-flow#step-5---activate-the-subscription-to-begin-billing
                await upodi.Subscriptions.ActivateAsync(SubscriptionId, new ActivateSubscriptionRequest());
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Activate", ex);
            }
        }

        public async Task<int> ReActivateSubscription(PaylikeCreditCardResult model, IMember member)
        {
            Customer customer = await GetByAccountNumber(model.MemberNodeId);

            if (customer != null)
            {
                Guid pPlanId = АssginProductPlanId(model.SubscriptionType);

                //Subscription[] subscriptions = this.upodi.Subscriptions.GetByCustomerId(customer.ID);
                // get by 
                var subscriptionList = await upodi.Subscriptions.ListAsync();
                var subscription = subscriptionList.Results.Where(s => s.CustomerID == customer.ID).First();
                //var sortedSubscriptions = subscriptions.Where(x => x.Status == SubscriptionStatusEnum.Draft && x.ProductPlanID == pPlanId).OrderByDescending(x => x.CreatedDate).ToList();

                if (subscription != null)
                {
                    //if (!string.IsNullOrEmpty(model.PromoKode))
                    //    AddPromoKode(subscription.ID, model.PromoKode);

                    await ActivateSubscription(subscription.ID);
                    await SavePaymentMethod(customer, model.Token);

                    return 1;
                }
                else
                {
                    Guid subscriptionId = await Subscribe(customer.ID, pPlanId);

                    //if (!string.IsNullOrEmpty(model.PromoKode))
                    //    AddPromoKode(subscriptionId, model.PromoKode);

                    await ActivateSubscription(subscriptionId);
                    await SavePaymentMethod(customer, model.Token);

                    return 2;
                }

                //if (sortedSubscriptions.Count > 0)
                //{
                //    var inactiveSubscription = sortedSubscriptions[0];
                //    Guid subscriptionId = inactiveSubscription.ID;

                //    if (!string.IsNullOrEmpty(model.PromoKode))
                //        AddPromoKode(subscriptionId, model.PromoKode);

                //    ActivateSubscription(subscriptionId);
                //    SavePaymentMethod(customer, model.Token);

                //    return 1;
                //}
                //else
                //{
                //    Guid subscriptionId = Subscribe(customer.ID, pPlanId);

                //    if (!string.IsNullOrEmpty(model.PromoKode))
                //        AddPromoKode(subscriptionId, model.PromoKode);

                //    ActivateSubscription(subscriptionId);
                //    SavePaymentMethod(customer, model.Token);

                //    return 2;
                //}
            }
            else
            {
                Guid pPlanId = АssginProductPlanId(model.SubscriptionType);
                CreateNewCustomer(model.MemberNodeId, member);
                await ReActivateSubscription(model, member);

                return 0;
            }
        }

        private DateTime DayOneNextMonth(DateTime start)
        {
            DateTime dt = start;
            DateTime dayone = new DateTime(dt.AddMonths(1).Year, dt.AddMonths(1).Month, 1);

            return dayone;
        }

        private async Task<Guid> BuildSubscription(Guid NewCustomersId, Guid ProductPlanId)
        {
            Guid planS = new Guid(ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanIdS"]);
            Guid pland = new Guid(ConfigurationManager.AppSettings["StartupCentral.Upodi.ProductPlanId1"]);

            CreateSubscription subscription = new CreateSubscription(NewCustomersId, ProductPlanId, DateTime.UtcNow)
            {
                SubscriptionNumber = Guid.NewGuid().ToString(),
                AutoRenew = true,
                InitialTermPeriod = SubscriptionTermEnum.years,
                InitialTermInterval = 1,
                RenewalTermPeriod = SubscriptionTermEnum.years,
                RenewalTermInterval = 1
                //Initial = (Term)new AnnualTerm(1),
                //Renewal = (Term)new AnnualTerm(1),
            };

            try
            {
                // Step 3 https://docs.upodi.com/page/sign-up-flow
                //var result = upodi.Subscriptions.Create(subscription);
                var result = await upodi.Subscriptions.CreateAsync(subscription);
                return result.ID;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Create Subscription", ex);
            }
        }

        public async Task<List<Upodi.SDK.Objects.Subscription>> getActiveSubscriptions(int custNumber)
        {
            Customer customer = await GetByAccountNumber(custNumber);
            if (customer != null)
            {
                //Upodi.SDK.Objects.Subscription[] subscriptions = upodi.Subscriptions.List().Results.Where(s => s.CustomerID.CompareTo(customer.ID) == 0).ToArray();
                Upodi.SDK.Objects.Subscription[] subscriptions = upodi.Subscriptions.List().Results.Where(s => s.CustomerID == customer.ID).ToArray();
                //Subscription[] subscriptions = this.upodi.Subscriptions.GetByCustomerId(customer.ID);
                var ActiveSubscription = subscriptions.Where(x => x.Status == StatusEnum.Active).ToList();
                return ActiveSubscription;
            }
            else
            {
                return null;
            }
        }

        public async Task<Upodi.SDK.Objects.Subscription[]> GetByCustomerId(Guid customerId)
        {
            var res = await upodi.Subscriptions.ListAsync();
            //return res.Results.Where(s => s.CustomerID.CompareTo(customerId) == 0).ToArray();
            return res.Results.Where(s => s.CustomerID == customerId).ToArray();
            //return await upodi.Subscriptions.GetByCustomerIdAsync(customerId);
        }

        public async Task<Upodi.SDK.Objects.Subscription[]> DeactivateSubscription(IMember member)
        {
            //Customer customer = GetByAccountNumber(member.Id);
            //if (customer != null)
            //{
            //    Subscription[] subscriptions = await GetByCustomerId(customer.ID);
            //    var ActiveSubscription = subscriptions.Where(x => x.Status == SubscriptionStatusEnum.Active).ToList();
            //    if (ActiveSubscription.Count > 0)
            //    {
            //        ActiveSubscription.ForEach(x => DeleteSubscription(x));
            //    }
            //    return subscriptions;
            //}
            //return null;

            return null;
        }

        public void DeleteSubscription(Upodi.SDK.Objects.Subscription subscription)
        {
            this.upodi.Subscriptions.Cancel(subscription.ID, null);
        }

        public async Task<Guid> Subscribe(Guid NewCustomersId, Guid ProductPlanId)
        {
            Func<DateTime, DateTime> func = (Func<DateTime, DateTime>)(x =>
            {
                DateTime dateTime = new DateTime(x.Year, x.Month, 1);
                dateTime = dateTime.AddMonths(1);
                return dateTime.AddDays(-1.0);
            });

            return await BuildSubscription(NewCustomersId, ProductPlanId);
        }

        private async Task<Guid> SavePaymentMethod(Customer Customer, string Token)
        {
            CreatePaymentMethodRequest paymentMethodRequest = new CreatePaymentMethodRequest("paylike", Token, Customer.ID, true)
            {
                MakeDefault = true
            };

            try
            {
                // Step 2b https://docs.upodi.com/page/sign-up-flow#step-2b---assign-a-default-payment-provider-using-the-returned-token
                //var result = this.upodi.PaymentMethods.CreatePaymentMethod(Customer.ID, paymentMethodRequest);
                var result = await upodi.PaymentMethods.CreateAsync(paymentMethodRequest);
                return result.ID;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to CreatePaymentMethod", ex);
            }
        }

        public async Task<Guid> SavePaymentMethodTest(Customer Customer, string Token)
        {
            CreatePaymentMethodRequest paymentMethodRequest = new CreatePaymentMethodRequest("test","{ \"card\" : { \"number\" : \"4111 1111 1111 1111\", \"expiry\" : \"03/2030\", \"cvc\" : 737 }}", Customer.ID, true)
            {
                MakeDefault = true
            };

            try
            {
                // Step 2b https://docs.upodi.com/page/sign-up-flow#step-2b---assign-a-default-payment-provider-using-the-returned-token
                //var result = this.upodi.PaymentMethods.CreatePaymentMethod(Customer.ID, paymentMethodRequest);
                var result = await upodi.PaymentMethods.CreateAsync(paymentMethodRequest);
                return result.ID;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to CreatePaymentMethod", ex);
            }
        }

        public async Task AddSubToBasic(int memberNodeId)
        {
            var queryResult = await upodi.Customers.Query(c => c.AccountNumber == memberNodeId.ToString("D"));
            var customer = queryResult.Results.First();

            // add basic product plan id
            var basicPlanId = АssginProductPlanId(22);
            Guid SubscriptionId = await Subscribe(customer.ID, basicPlanId);
            await ActivateSubscription(SubscriptionId);
        }

        public async Task<Guid> SavePaymentMethod(int memberNodeId, string Token, string PromoKode, Guid ProductPlanId)
        {
            var queryResult = await upodi.Customers.Query(c => c.AccountNumber == memberNodeId.ToString("D"));
            var customer = queryResult.Results.First();

            Guid SubscriptionId = await Subscribe(customer.ID, ProductPlanId);
            if (!string.IsNullOrEmpty(PromoKode))
                await AddPromoKode(SubscriptionId, PromoKode);
            await ActivateSubscription(SubscriptionId);
            return await SavePaymentMethod(customer, Token);
        }

        public async Task<Guid> SavePaymentMethodFromLounge(int memberNodeId, string Token, string PromoKode, Guid ProductPlanId, IMember member)
        {
            var queryResult = await upodi.Customers.Query(c => c.AccountNumber == memberNodeId.ToString("D"));
            var customer = queryResult.Results.First();
            //Customer customer = this.upodi.Customers.GetByAccountNumber(memberNodeId.ToString("D"));
            if (customer == null)
            {
                CreateNewCustomer(memberNodeId, member);
                return await SavePaymentMethod(memberNodeId, Token, null, ProductPlanId);
            }
            else
            {

                Guid SubscriptionId = await Subscribe(customer.ID, ProductPlanId);
                if (!string.IsNullOrEmpty(PromoKode))
                    await AddPromoKode(SubscriptionId, PromoKode);
                await ActivateSubscription(SubscriptionId);
                return await SavePaymentMethod(customer, Token);
            }
        }

        //public async Task<Guid> ReSavePaymentMethod(int memberNodeId, string Token, IMember member, Guid ProductPlanId)
        //{
        //    var res = await upodi.Customers.Query(c => c.AccountNumber == memberNodeId.ToString("D"));
        //    var customer = res.Results.FirstOrDefault();
        //    if (customer == null)
        //    {
        //        CreateNewCustomer(memberNodeId, member);
        //        return await SavePaymentMethod(memberNodeId, Token, null, ProductPlanId);
        //    }
        //    else
        //    {
        //        return await SavePaymentMethod(customer, Token);
        //    }
        //}

        public async Task<Guid> ReSavePaymentMethod(int memberNodeId, string Token, string PromoKode, Guid ProductPlanId)
        {
            var queryResult = await upodi.Customers.Query(c => c.AccountNumber == memberNodeId.ToString("D"));
            var customer = queryResult.Results.First();

            
            return await SavePaymentMethod(customer, Token);
        }

        public async Task AddPromoKode(Guid SubscriptionId, string PromoKode)
        {
            try
            {
                //this.upodi.Discounts.ApplyDiscountCodeToSubscription(SubscriptionId, PromoKode);
                var sub = await upodi.Subscriptions.GetAsync(SubscriptionId);
                await upodi.Discounts.ApplyDiscountCode(PromoKode, sub);
            }
            catch (Exception)
            {

            }
        }

        public async Task<bool> ValidateDiscountCode(string promo)
        {
            var DiscountCollection = await upodi.Discounts.ListAsync();
            var DiscountList = DiscountCollection.Results;
            List<DiscountCode> discountCodeList = new List<DiscountCode>();
            for (int i = 0; i < DiscountList.Length; i++)
            {
                var discount = await upodi.Discounts.GetAsync(DiscountList[i].ID);
                var discountCodesCollection =  await upodi.Discounts.ListDiscountCodes(discount);
                var discountCodesList = discountCodesCollection.Results;
                foreach (var d in discountCodesList)
                {
                    discountCodeList.Add(d);
                }
                //this.upodi.Discounts.FetchDiscountCodes(DiscountList[i].ID).ToList().ForEach(x => discountCodeList.Add(x));
            }
            var ActiveCode = discountCodeList.Where(x => x.State == DiscountCodeStateEnum.Active).Select(
                //x => x.CompleteCode.ToLower());
                x => (x.CodePrefix + (string.IsNullOrEmpty(x.Code) ? "" : "-" + x.Code)).ToLowerInvariant());
            return !string.IsNullOrEmpty(ActiveCode.FirstOrDefault(x => x == promo.ToLowerInvariant()));
        }

        public async Task<(decimal, int?, string)> GetDiscountPercentage(string PromoKode)
        {
            
            var discountId = Guid.Empty;
            var DiscountCollection = await upodi.Discounts.ListAsync();
            var DiscountList = DiscountCollection.Results;
            List<DiscountCode> discountCodeList = new List<DiscountCode>();
            // fill discount list
            for (int i = 0; i < DiscountList.Count(); i++)
            {
                var discount = await upodi.Discounts.GetAsync(DiscountList[i].ID);
                var discountCodesCollection = await upodi.Discounts.ListDiscountCodes(discount);
                var discountCodesList = discountCodesCollection.Results;
                foreach (var d in discountCodesList)
                {
                    discountCodeList.Add(d);
                }
                //this.upodi.Discounts.FetchDiscountCodes(DiscountList[i].ID).ToList().ForEach(x => discountCodeList.Add(x));
            }

            //foreach (var dc in DiscountList)
            //{
            //    foreach (var d in dc.DiscountCodes)
            //    {
            //        d.DiscountId
            //        if (d.CompleteCode.Contains(PromoKode))
            //        {
            //            return (dc.Amount, dc.ExpiryPeriodInterval, dc.ExpiryPeriod.ToString());
            //        }
            //    }
            //}

            foreach (var dc in discountCodeList)
            {
                if (dc.CompleteCode.Contains(PromoKode))
                {
                    var discount = await upodi.Discounts.GetAsync(dc.DiscountId);
                    return (discount.Amount, discount.ExpiryPeriodInterval, discount.ExpiryPeriod.ToString());
                }
            }

            return (-1, -1, null);
        }

        public Invoice GetInvoic(Guid InvoicesId)
        {
            try
            {
                return this.upodi.Invoices.Get(InvoicesId);
            }
            catch (Exception)
            {
                return (Invoice)null;
            }
        }

        public async Task<string> GetPaymentCardNumber(Guid upodiCustomerId)
        {
            string result = null;
            PaymentMethod activePaymentMethod = null;
            var cust = await upodi.Customers.GetAsync(upodiCustomerId);
            var pmID = cust.PaymentMethod;
            if (pmID.HasValue)
            {
                activePaymentMethod = await upodi.PaymentMethods.GetAsync(pmID.Value);
            }

            if (activePaymentMethod != null)
            {
                result = activePaymentMethod.Issuer + " " + activePaymentMethod.FullName;
            }

            return result;
        }

        public async Task<DateTime?> GetCardExpiryDate(Guid upodiCustomerId)
        {
            try
            {
                DateTime? result = null;
                //var delay = Task.Delay(1734);
                //var task = Task.Factory.StartNew(() =>
                //{
                //    PaymentMethod activePaymentMethod = null;
                //    var cust = this.upodi.Customers.Get(upodiCustomerId);
                //    //var pmID = cust.PaymentMethodID;
                //    var pmID = cust.PaymentMethod;
                //    if (pmID.HasValue)
                //    {
                //        activePaymentMethod = this.upodi.PaymentMethods.Get(pmID.Value);
                //    }
                //    else
                //    {
                //        //var paymentMethods = this.upodi.PaymentMethods.GetByCustomer(upodiCustomerId);
                //        //var paymentMethods = upodi.PaymentMethods.List().Results.Where(p => p.CustomerID.CompareTo(upodiCustomerId) == 0);
                //        var paymentMethods = upodi.PaymentMethods.List().Results.Where(p => p.CustomerID == upodiCustomerId);
                //        activePaymentMethod = paymentMethods.OrderByDescending(x => x.ModifiedDate).FirstOrDefault();
                //    }
                //    if (activePaymentMethod != null)
                //    {
                //        result = activePaymentMethod?.ExpiryDate;
                //    }
                //});
                //task.ConfigureAwait(false);

                //Task.WaitAny(delay, task);
                PaymentMethod activePaymentMethod = null;
                var cust = await upodi.Customers.GetAsync(upodiCustomerId);
                var pmID = cust.PaymentMethod;
                if (pmID.HasValue)
                {
                    activePaymentMethod = await upodi.PaymentMethods.GetAsync(pmID.Value);
                }
                else
                {
                    //var paymentMethods = this.upodi.PaymentMethods.GetByCustomer(upodiCustomerId);
                    //var paymentMethods = upodi.PaymentMethods.List().Results.Where(p => p.CustomerID.CompareTo(upodiCustomerId) == 0);
                    var paymentMethodsList = await upodi.PaymentMethods.ListAsync();
                    var paymentMethods = paymentMethodsList.Results.Where(p => p.CustomerID == upodiCustomerId);
                    activePaymentMethod = paymentMethods.OrderByDescending(x => x.ModifiedDate).FirstOrDefault();
                }
                if (activePaymentMethod != null)
                {
                    result = activePaymentMethod?.ExpiryDate;
                }


                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DateTime?> GetNextPaymentDate(Guid upodiCustomerId)
        {
           var result = await upodi.Helpers.GetUpcomingBillingDateByCustomer(upodiCustomerId);
            return result;
        }

        public async Task<string> GetPlanType(Guid upodiCustomerId)
        {
            try
            {
                string result = null;
                var query = await upodi.Subscriptions.Query(s => s.CustomerID == upodiCustomerId);
                var subscriptions = query.Results;
                var firstActiveSub = subscriptions.FirstOrDefault(s => s.Status == StatusEnum.Active);
                if (firstActiveSub != null)
                {
                    var planId = firstActiveSub.ProductPlanID;

                    var plan = await upodi.ProductPlans.GetAsync(planId);
                    result = plan.FullName;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Invoice[]> GetListOfInvoices(Guid MemberId)
        {
            try
            {
                var queryResult = await upodi.Invoices.Query(i => i.CustomerID == MemberId);
                var result = queryResult.Results;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Customer GetCustomerById(Guid ID)
        {
            try
            {
                return this.upodi.Customers.Get(ID);
            }
            catch (Exception)
            {
                return (Customer)null;
            }
        }
        
        public void BookInvoic(Guid ID)
        {
            using (WebClient webClient = new WebClient())
            {
                string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(UpodiWrapperService.AccessKey));
                webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + base64String);
                webClient.UploadData("https://api.upodi.io/v2/invoices/" + ID.ToString() + "/book", "PUT", new byte[0]);
            }
        }

        public CreditCard GetCreditCard(int memberNodeId)
        {
            //try
            //{
            //    var customer = GetByAccountNumber(memberNodeId);

            //    var paymentMethod = this.upodi.PaymentMethods.GetByCustomer(customer.ID).FirstOrDefault();
            //    //if (paymentMethod == null) return new CreditCard("fake", "Customer Not Found", DateTime.Now);
            //    return new CreditCard(paymentMethod.Issuer, paymentMethod.FullName, paymentMethod.ExpiryDate);
            //}
            //catch (Exception)
            //{
            //    return null;
            //}

            return null;
        }

        public bool updateCustomerPrivateInfo(IMember member)
        {
            //var outerDelay = Task.Delay(1517);
            //var result = false;
            //var task = Task.Factory.StartNew(() =>
            //{
            //    Customer cus = GetByAccountNumber(member.Id);

            //    if (cus != null)
            //    {
            //        cus.FullName = member.Name;
            //        cus.MobilePhone = member.GetValue<string>("wwmobile");
            //        cus.PostalCode = member.GetValue<string>("wwpostnummer");
            //        cus.City = member.GetValue<string>("wwcity");
            //        cus.AddressLine1 = member.GetValue<string>("wwaddress");

            //        var resTask = this.upodi.Customers.UpdateAsync(cus, Guid.NewGuid());
            //        var innerDelay = Task.Delay(817);
            //        Task.WaitAny(resTask, innerDelay);
            //        if (resTask.IsCompleted)
            //            result = resTask.GetAwaiter().GetResult();
            //    }
            //    else
            //    {
            //        result = false;
            //    }
            //});
            //Task.WaitAny(outerDelay, task);

            //return result;

            return false;
        }


        public bool updateCustomerCompanyName(IMember member, string companyName, string cvr)
        {
            //Customer cus = GetByAccountNumber(member.Id);

            //if (cus != null)
            //{
            //    cus.CompanyName = companyName;
            //    cus.CompanyVAT = cvr;

            //    var res = false;
            //    var delay = Task.Delay(3521);
            //    var resTask = Task.Factory.StartNew(() => res = this.upodi.Customers.Update(cus, Guid.NewGuid()));
            //    Task.WaitAny(resTask, delay);
            //    if (resTask.IsCompleted)
            //        res = resTask.Result;
            //    return res;
            //}
            //else
            //{
            //    return false;
            //}

            return false;
        }


        public bool resumeSubscription(IMember member)
        {
            //Customer customer = GetByAccountNumber(member.Id);
            //Subscription[] subscriptions = this.upodi.Subscriptions.GetByCustomerId(customer.ID);

            //var pausedSubscription = subscriptions.Where(x => x.StateCode == SubscriptionStateEnum.Hold).ToList();

            //if (pausedSubscription.Count > 0)
            //{
            //    return upodi.Subscriptions.Resume(pausedSubscription[0].ID);
            //}
            //else
            //{
            //    return false;
            //}

            return false;

        }
    }
}