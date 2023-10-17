using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using StartupCentral.Code.Model.MobilePay;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using System.Text;
using Umbraco.Core;
using Umbraco.Web;
using umbraco.NodeFactory;

namespace StartupCentral.Code.Class
{
    public static class MobilePay
    {



        private static string certFile = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.MobilePay.Certificate.Filename"]; // System.IO.Path.Combine("C:\\", "Certifikater", "StartupCentral Sandbox cert", "Sandbox_MobilePay_StartupCentral.pfx");
        private static string baseUrl = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.MobilePay.ApiBaseUrl"]; // "https://api.sandbox.mobilepay.dk";

        private static string apiSecret = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.MobilePay.Secret"]; // "P2kJ5cP4jY4pH7cY8kX0sP3nN8hS4lY8cX0tJ4vB7sG5rA0vV7";
        private static string apiClientId = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.MobilePay.ClientId"]; //  "204bbb19-8ca4-4b6b-abbb-36390c602f84";

        private static string userRedirectUrl = "";
        private static string callbackUrl = "https://www.startupcentral.dk/umbraco/api/mobilepayapi/";

        public static string DataPath { get; set; }

        public static void Initialize()
        {
            UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            userRedirectUrl = string.Concat(umbracoHelper.TypedContent(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.BlivBruger.Id"])).UrlWithDomain(), "?callback=true");

            DataPath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.MobilePay.TransactionPath"]);

            if (!System.IO.Directory.Exists(DataPath))
            {
                System.IO.Directory.CreateDirectory(DataPath);
            }
        }


        public static AgreementResponse RequestPayment(IMember member)
        {
            Initialize();

            RemoveMemberStatus(member.Key);

            Node mobilePaySettings = StartupCentral.Helpers.Nodes.GetFirstNodeByType(-1, "mobilePay");


            var client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            AgreementRequest req = new AgreementRequest();
            req.ExternalId = member.Key.ToString();
            req.Amount = mobilePaySettings.GetProperty("wwmonthlyprice").Value.Replace(",", ".");
            req.Description = mobilePaySettings.GetProperty("wwtransactiontext").Value; // "Startup Central Abb";

            if (member.GetValue<string>("wwmobile") != null)
            {
                try
                {
                    req.MobilePhoneNumber = Convert.ToInt64("45" + member.GetValue<string>("wwmobile").Replace(" ", string.Empty).Replace("-", string.Empty).Trim());

                    //req.OneOffPayment = new OneOffPayment()
                    //{
                    //    Amount = mobilePaySettings.GetProperty("wwmonthlyprice").Value.Replace(",", "."),
                    //    ExternalId = member.Id.ToString(),
                    //    Description = "Første betaling"
                    //};

                }
                catch (System.Exception)
                {
                    req.MobilePhoneNumber = 0;
                }
            }
            //req.OneOffPayment = new OneOffPayment() { Amount = "0.50", Description = "First payment", ExternalId = "PMT0000001" };
            req.Links.Add(new Link() { Rel = "user-redirect", Href = userRedirectUrl + "&memberkey=" + member.Key.ToString() });
            req.Links.Add(new Link() { Rel = "success-callback", Href = callbackUrl + "PaymentSuccess?memberkey=" + member.Key.ToString() });
            req.Links.Add(new Link() { Rel = "cancel-callback", Href = callbackUrl + "PaymentRejected?memberkey=" + member.Key.ToString() });

            string requestString = Newtonsoft.Json.JsonConvert.SerializeObject(req);

            request.AddParameter("application/json", requestString, ParameterType.RequestBody);
            LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "MobilePay request: " + requestString);

            //request.AddParameter("application/json", "{\"external_id\":\"SUB0000001\",\"amount\":\"29.9\",\"currency\":\"DKK\",\"description\":\"Startup Central Abb\",\"next_payment_date\":\"2018-10-04T08:00:00.000Z\",\"frequency\":12,\"links\":[{\"rel\":\"user-redirect\",\"href\":\"" + callbackUrl + "\"}, {\"rel\":\"success-callback\",\"href\":\"" + callbackUrl + "&state=1\"},	{\"rel\":\"cancel-callback\",\"href\":\"" + callbackUrl + "&state=0\"}],\"country_code\":\"DK\",\"plan\":\"Basic\",\"expiration_timeout_minutes\":5,\"mobile_phone_number\":4500000000,\"one_off_payment\":{\"amount\":\"19.99\",\"external_id\":\"PMT0000001\",\"description\":\"First payment\"}}", ParameterType.RequestBody);


            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "MobilePay response: " + response.Content);
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    AgreementResponse item = Newtonsoft.Json.JsonConvert.DeserializeObject<AgreementResponse>(response.Content);

                    System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, item.Id + ".json"), response.Content);

                    return item;
                }
            }

            return null;
        }

        public static void UpdateMerch(Guid memberKey)
        {
            Initialize();

            LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "UpdateMerch");

            var client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me");
            var request = new RestRequest(Method.PATCH);

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            request.AddHeader("Accept", "application/json");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);
            request.AddParameter("application/json", "[{\"value\":\"https://www.startupcentral.dk/umbraco/api/mobilepayapi/CallbackStatus\",\"path\":\"/payment_status_callback_url\",\"op\":\"replace\"}]", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response != null)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "MobilePay response: " + response.Content);
            }

        }



        public static AgreementStatus HandleCallback()
        {
            AgreementStatus agreementStatus = null;
            try
            {
                agreementStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<AgreementStatus>(GetDocumentContents(HttpContext.Current.Request));
            }
            catch (Exception)
            {

            }

            return agreementStatus;
        }

        public static List<StatusCallback> HandleStatusCallback()
        {
            List<StatusCallback> agreementStatus = null;
            try
            {
                agreementStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StatusCallback>>(GetDocumentContents(HttpContext.Current.Request));
            }
            catch (Exception)
            {

            }

            return agreementStatus;
        }


        private static string GetDocumentContents(System.Web.HttpRequest Request)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }

        public static void RemoveMemberStatus(Guid memberKey)
        {

            try
            {
                string DataPath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.MobilePay.TransactionPath"]);

                if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".accept.callback.json")))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(DataPath, memberKey + ".accept.callback.json"));
                }
                if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".reject.callback.json")))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(DataPath, memberKey + ".reject.callback.json"));
                }
            }
            catch (System.Exception)
            {

            }
        }


        #region Sandbox Testing

        public static List<Agreement> GetAgreementList()
        {
            Initialize();

            var client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Agreement>>(response.Content);
                }
            }

            return null;
        }


        public static Agreement GetAgreement(string agreementId)
        {
            Initialize();

            var client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Agreement>(response.Content);
                }
            }

            return null;
        }


        public enum AgreementStatusType
        {
            All,
            Accepted,
            Pending,
            Rejected,
            Expired,
            Canceled
        }


        public static List<Agreement> GetAgreementList(AgreementStatusType status)
        {
            Initialize();

            RestClient client = null;

            if (status != AgreementStatusType.All)
            {
                client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements");
            }
            else
            {
                client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements?status={status.ToString().ToLower()}");
            }

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Agreement>>(response.Content);
                }
            }

            return null;
        }

        public static string GetAgreementListToString()
        {
            Initialize();

            var client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return response.Content;
                }
            }

            return null;
        }


        public static void DeleteOneOffPayment(string agreementId, string paymentId)
        {
            Initialize();

            var client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/oneoffpayments/{paymentId}");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.DELETE);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {

                }
            }


        }


        public static List<OneOffPaymentResponse> GetOneOffPayments(string agreementId)
        {
            Initialize();

            RestClient client = null;

            client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/oneoffpayments");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<OneOffPaymentResponse>>(response.Content);
                }
            }

            return null;
        }


        public static List<OneOffPaymentResponse> GetOneOffPaymentRequests(string agreementId)
        {
            Initialize();

            RestClient client = null;

            client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/oneoffpayments");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<OneOffPaymentResponse>>(response.Content);
                }
            }

            return null;
        }


        public static PaymentResponse RequestOneOffPayment(string agreementId, string amount, IMember member, string redirectUrl)
        {
            Initialize();

            RestClient client = null;

            client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/oneoffpayments");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            PaymentRequest req = new PaymentRequest();
            req.Amount = amount.ToString();
            req.ExternalId = member.Id.ToString();
            req.Description = "Månedlig betaling";
            req.Links.Add(new PaymentRequestLink()
            {
                Href = redirectUrl,
                Rel = "user-redirect"
            });
            req.Links.Add(new PaymentRequestLink()
            {
                Href = callbackUrl + "OneoffPaymentSuccess?memberkey=" + member.Key.ToString(),
                Rel = "success-callback"
            });
            req.Links.Add(new PaymentRequestLink()
            {
                Href = callbackUrl + "OneoffPaymentFailure?memberkey=" + member.Key.ToString(),
                Rel = "cancel-callback"
            });

            string requestString = Newtonsoft.Json.JsonConvert.SerializeObject(req);
            request.AddParameter("application/json", requestString, ParameterType.RequestBody);



            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentResponse>(response.Content);
                }
            }

            return null;
        }

        public static string RequestPaymentRequest(string agreementId, string amount, IMember member)
        {

            try
            {

                Initialize();

                RestClient client = null;

                client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/paymentrequests");

                client.ClientCertificates = new X509CertificateCollection();
                X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

                client.ClientCertificates.Add(oCert);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("correlationid", "");
                request.AddHeader("x-ibm-client-secret", apiSecret);
                request.AddHeader("x-ibm-client-id", apiClientId);

                List<RequestPaymentRequest> list = new List<RequestPaymentRequest>();

                RequestPaymentRequest req = new RequestPaymentRequest();
                req.AgreementId = agreementId;
                req.Amount = amount.ToString();
                req.ExternalId = member.Id.ToString();
                req.Description = "Månedlig betaling";
                req.DueDate = DateTime.Now.AddDays(8).ToString("yyyy-MM-dd");
                req.NextPaymentDate = DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd");
                list.Add(req);

                string requestString = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                request.AddParameter("application/json", requestString, ParameterType.RequestBody);

                //DumpToFile("PayReq", Newtonsoft.Json.JsonConvert.SerializeObject(list));


                IRestResponse response = client.Execute(request);
                if (response != null)
                {
                    if (response.ResponseStatus == ResponseStatus.Completed)
                    {
                        DumpToFile("RPRResp", response.Content);
                        return response.Content;
                    }
                }


                UpdateMerch(member.Key);

            } catch (System.Exception ex)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "RequestPaymentRequest", ex);
                return ex.ToString();
            }

            return string.Empty;
            //return null;
        }


        public static void DumpToFile(string clue, string content)
        {

            System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, "dump." + clue + Guid.NewGuid().ToString().Replace("-", string.Empty) + ".txt"), content);

        }

        public static List<OneOffPaymentResponse> GetPaymentRequests(string agreementId)
        {
            Initialize();

            RestClient client = null;

            client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/paymentrequests");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<OneOffPaymentResponse>>(response.Content);
                }
            }

            return null;
        }


        public static OneOffPaymentResponse GetOneOffPayment(string agreementId, string paymentId)
        {
            Initialize();

            RestClient client = null;

            client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/oneoffpayments/{paymentId}");

            client.ClientCertificates = new X509CertificateCollection();
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

            client.ClientCertificates.Add(oCert);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("correlationid", "");
            request.AddHeader("x-ibm-client-secret", apiSecret);
            request.AddHeader("x-ibm-client-id", apiClientId);

            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<OneOffPaymentResponse>(response.Content);
                }
            }

            return null;
        }


        public static void CapturePayment(string agreementId)
        {
            var list = Class.MobilePay.GetOneOffPaymentRequests(agreementId);
            if (list != null)
            {
                foreach(var itm in list)
                {
                    CatpurePayment(agreementId, itm.Id);
                }
            }
        }

        public static string CatpurePayment(string agreementId, string paymentId)
        {
            try
            {
                Initialize();

                RestClient client = null;

                client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/oneoffpayments/{paymentId}/capture");

                client.ClientCertificates = new X509CertificateCollection();
                X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

                client.ClientCertificates.Add(oCert);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("x-ibm-client-secret", apiSecret);
                request.AddHeader("x-ibm-client-id", apiClientId);

                IRestResponse response = client.Execute(request);
                if (response != null)
                {
                    if (response.ResponseStatus == ResponseStatus.Completed)
                    {
                        return response.Content;
                    }
                }

                return null;

            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }

        }

        public static string RefundPayment(string agreementId, string paymentId, IMember member, double amount)
        {
            try
            {
                Initialize();

                RestClient client = null;

                client = new RestClient($"{baseUrl}/subscriptions/api/merchants/me/agreements/{agreementId}/payments/{paymentId}/refunds");

                client.ClientCertificates = new X509CertificateCollection();
                X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");

                client.ClientCertificates.Add(oCert);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("x-ibm-client-secret", apiSecret);
                request.AddHeader("x-ibm-client-id", apiClientId);

                RefundRequest req = new RefundRequest();
                req.Amount = amount;
                req.ExternalId = member.Key.ToString();
                req.Status_Callback_Url = callbackUrl + "PaymentRefund?memberkey=" + member.Key.ToString();
                string requestString = Newtonsoft.Json.JsonConvert.SerializeObject(req);
                request.AddParameter("application/json", requestString, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                if (response != null)
                {
                    if (response.ResponseStatus == ResponseStatus.Completed)
                    {
                        return response.Content;
                    }
                }

                return null;

            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }

        }




        #endregion



    }
}