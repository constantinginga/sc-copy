using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using RestSharp;
using StartupCentral.Code.Class;
using StartupCentral.Code.Model.MobilePay;
using umbraco.NodeFactory;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.ApiControllers
{
    public class MobilePayApiController : UmbracoSocketApiController
    {
        public override LiveConnections.ChannelsType ChannelType { get; set; } = LiveConnections.ChannelsType.MobilePayApi;

        public string pathOfData = "/App_Data/MobilePay/Data";


        [HttpPost]
        public void PaymentSuccess(Guid memberKey)
        {
            AgreementStatus status = MobilePay.HandleCallback();
            string DataPath = HttpContext.Current.Server.MapPath(pathOfData);

            if (memberKey != null)
            {
                IMember member = Services.MemberService.GetByKey(memberKey);
                if (member != null)
                {
                    member.SetValue("wwMobilePayAbonnementsDato", DateTime.Now);
                    member.SetValue("wwmobileSubscriptionId", status.AgreementId);
                    //member.IsApproved = true;

                    StartupCentral.Code.Class.MobilePay.RemoveMemberStatus(memberKey);

                    System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, memberKey + ".accept.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(status));

                    Services.MemberService.Save(member);

                    Node mobilePaySettings = StartupCentral.Helpers.Nodes.GetFirstNodeByType(-1, "mobilePay");
                    //Class.MobilePay.RequestPaymentRequest(status.AgreementId, mobilePaySettings.GetProperty("wwmonthlyprice").Value.Replace(",", "."), member);

                    PaymentResponse paymentResponse = Class.MobilePay.RequestOneOffPayment(status.AgreementId, mobilePaySettings.GetProperty("wwmonthlyprice").Value.Replace(",", "."), member, $"https://www.startupcentral.dk/mobilepaycompleted?memberkey={memberKey}&completed=true&agreementid={status.AgreementId}");
                    if (paymentResponse != null)
                    {
                        paymentResponse.Status = "redirect";
                        System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, memberKey + ".redirect.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(paymentResponse));
                        SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, paymentResponse,
                               memberKey);
                    }
                    //SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, status,
                    //     memberKey);
                }
            }

            try
            {
                if (status != null)
                {
                    //StartupCentral.Code.Class.MobilePay.UpdateMerch(memberKey);
                }
            }
            catch (System.Exception)
            {

            }
        }

        [HttpPost]
        public void CallbackStatus()
        {

            var statusList = MobilePay.HandleStatusCallback();

            if (statusList != null)
            {
                foreach (var st in statusList)
                {
                   if (!string.IsNullOrEmpty(st.ExternalId))
                    {
                        IMember member = Services.MemberService.GetById(Convert.ToInt32(st.ExternalId));
                        if (member != null)
                        {
                            if (st.Status.ToLower() == "reserved" || st.Status.ToLower() == "accepted")
                            {
                                member.IsApproved = true;
                                Services.MemberService.Save(member);

                                string password = Passwords.GenerateNewPassword();
                                Services.MemberService.SavePassword(member, password);
                                Emailing.SendApprovvalEmail(member, password);

                            }
                        }
                    }
                }
            }

            string DataPath = HttpContext.Current.Server.MapPath(pathOfData);
            System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, Guid.NewGuid().ToString().Replace("-", "") + ".status1.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(statusList));
            HttpContext.Current.Request.SaveAs(System.IO.Path.Combine(DataPath, Guid.NewGuid().ToString().Replace("-", "") + ".status.callback.json"), true);
        }


        [HttpPost]
        public void OneoffPaymentSuccess(Guid memberKey)
        {

            string DataPath = HttpContext.Current.Server.MapPath(pathOfData);
            HttpContext.Current.Request.SaveAs(System.IO.Path.Combine(DataPath, memberKey + ".random.callback.json"), true);

            AgreementStatus status = MobilePay.HandleCallback();

            if (memberKey != null)
            {
                IMember member = Services.MemberService.GetByKey(memberKey);
                if (member != null)
                {
                    member.SetValue("wwlatestMobilePayId", status.ExternalId);
                    member.IsApproved = true;

                    StartupCentral.Code.Class.MobilePay.RemoveMemberStatus(memberKey);

                    System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, memberKey + ".oneoffaccept.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(status));

                    Services.MemberService.Save(member);



                    //PaymentResponse paymentResponse = Class.MobilePay.RequestOneOffPayment(status.AgreementId, "149", member, $"https://www.startupcentral.dk/mobilepay?memberkey={memberKey}");
                    //if (paymentResponse != null)
                    //{

                    //    paymentResponse.Status = "redirect";

                    //    System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, memberKey + ".redirect.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(paymentResponse));

                    //    SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, paymentResponse,
                    //           memberKey);

                    //}

                }
            }

            try
            {
                if (status != null)
                {
                    //StartupCentral.Code.Class.MobilePay.UpdateMerch(memberKey);
                }
            }
            catch (System.Exception)
            {

            }



        }



        [HttpPost]
        public void PaymentRejected(Guid memberKey)
        {
            AgreementStatus status = MobilePay.HandleCallback();
            string DataPath = HttpContext.Current.Server.MapPath(pathOfData);

            if (memberKey != null)
            {
                IMember member = Services.MemberService.GetByKey(memberKey);
                if (member != null)
                {
                    member.SetValue("wwMobilePayAbonnementsDato", null);
                    member.IsApproved = false;

                    StartupCentral.Code.Class.MobilePay.RemoveMemberStatus(memberKey);

                    System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, memberKey + ".reject.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(status));
                    Services.MemberService.Save(member);
                }
            }

            try
            {
                if (status != null)
                {
                    StartupCentral.Code.Class.MobilePay.UpdateMerch(memberKey);
                }
            }
            catch (System.Exception)
            {

            }
            SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, status,
                   memberKey);

        }


        [HttpPost]
        public void OneoffPaymentFailure(Guid memberKey)
        {
            string DataPath = HttpContext.Current.Server.MapPath(pathOfData);
            HttpContext.Current.Request.SaveAs(System.IO.Path.Combine(DataPath, memberKey + ".randomfailure.callback.json"), true);
            AgreementStatus status = MobilePay.HandleCallback();

            if (memberKey != null)
            {
                IMember member = Services.MemberService.GetByKey(memberKey);
                if (member != null)
                {
                    member.SetValue("wwMobilePayAbonnementsDato", null);
                    member.IsApproved = false;

                    StartupCentral.Code.Class.MobilePay.RemoveMemberStatus(memberKey);

                    System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, memberKey + ".oneoffreject.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(status));
                    Services.MemberService.Save(member);
                }
            }

            try
            {
                if (status != null)
                {
                    StartupCentral.Code.Class.MobilePay.UpdateMerch(memberKey);
                }
            }
            catch (System.Exception)
            {

            }
            SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, status,
                   memberKey);

        }


        [HttpPost]
        public void PaymentRefund(Guid memberKey)
        {
            AgreementStatus status = MobilePay.HandleCallback();
            string DataPath = HttpContext.Current.Server.MapPath(pathOfData);

            if (memberKey != null)
            {
                IMember member = Services.MemberService.GetByKey(memberKey);
                if (member != null)
                {
                    member.SetValue("wwMobilePayAbonnementsDato", null);
                    member.IsApproved = false;

                    StartupCentral.Code.Class.MobilePay.RemoveMemberStatus(memberKey);

                    System.IO.File.WriteAllText(System.IO.Path.Combine(DataPath, memberKey + ".refund.callback.json"), Newtonsoft.Json.JsonConvert.SerializeObject(status));
                    Services.MemberService.Save(member);
                }
            }

            try
            {
                if (status != null)
                {
                    StartupCentral.Code.Class.MobilePay.UpdateMerch(memberKey);
                }
            }
            catch (System.Exception)
            {

            }
            SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, status,
                   memberKey);

        }


        /// <summary>
        /// Callback method that MobilePay calls back to, when the user either accepts or declines the subscription in the app.
        /// </summary>
        /// <param name="memberKey"></param>
        [HttpGet]
        public void RequestStatus(Guid memberKey)
        {
            string DataPath = HttpContext.Current.Server.MapPath(pathOfData);

            object status = null;


            if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".oneoffaccept.callback.json")))
            {
                SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, Newtonsoft.Json.JsonConvert.DeserializeObject<object>(System.IO.File.ReadAllText(System.IO.Path.Combine(DataPath, memberKey + ".oneoffaccept.callback.json"))), memberKey);
            }
            if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".redirect.callback.json")))
            {
                status = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentResponse>(System.IO.File.ReadAllText(System.IO.Path.Combine(DataPath, memberKey + ".redirect.callback.json")));
            }
            //if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".accept.callback.json")))
            //{
            //    status = Newtonsoft.Json.JsonConvert.DeserializeObject<AgreementStatus>(System.IO.File.ReadAllText(System.IO.Path.Combine(DataPath, memberKey + ".accept.callback.json")));
            //}
            //if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".oneoffaccept.callback.json")))
            //{
            //    SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, Newtonsoft.Json.JsonConvert.DeserializeObject<object>(System.IO.File.ReadAllText(System.IO.Path.Combine(DataPath, memberKey + ".oneoffaccept.callback.json"))), memberKey);
            //}
            //if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".redirect.callback.json")))
            //{
            //    status = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentResponse>(System.IO.File.ReadAllText(System.IO.Path.Combine(DataPath, memberKey + ".redirect.callback.json")));
            //}
            if (System.IO.File.Exists(System.IO.Path.Combine(DataPath, memberKey + ".reject.callback.json")))
            {
                status = Newtonsoft.Json.JsonConvert.DeserializeObject<AgreementStatus>(System.IO.File.ReadAllText(System.IO.Path.Combine(DataPath, memberKey + ".reject.callback.json")));
            }

            SocketTransmitter.Broadcast(LiveConnections.ChannelsType.MobilePayApi, status,
                 memberKey);

            try
            {
                if (status != null)
                {
                    StartupCentral.Code.Class.MobilePay.UpdateMerch(memberKey);
                }
            }
            catch (System.Exception)
            {

            }
        }





        [HttpGet]
        public string Accept()
        {

            string certFile = System.IO.Path.Combine("C:\\", "Certifikater", "StartupCentral Sandbox cert", "Sandbox_MobilePay_StartupCentral.pfx");
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");
            var client = new RestClient("https://api.sandbox.mobilepay.dk/recurringpayments-restapi/api/agreements/v1/" + StartupCentral.Code.Class.MobilePay.GetAgreementList().First().Id);
            client.ClientCertificates = new X509CertificateCollection();
            client.ClientCertificates.Add(oCert);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("authenticateduser", "e3ad5035-67b1-4114-a5c3-73f07882e24b");
            request.AddHeader("x-ibm-client-secret", "P2kJ5cP4jY4pH7cY8kX0sP3nN8hS4lY8cX0tJ4vB7sG5rA0vV7");
            request.AddHeader("x-ibm-client-id", "204bbb19-8ca4-4b6b-abbb-36390c602f84");
            request.AddParameter("application/json", "{\"status\":\"Accepted\"}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            return response.Content;

        }

        [HttpGet]
        public string Reject()
        {

            string certFile = System.IO.Path.Combine("C:\\", "Certifikater", "StartupCentral Sandbox cert", "Sandbox_MobilePay_StartupCentral.pfx");
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");
            var client = new RestClient("https://api.sandbox.mobilepay.dk/recurringpayments-restapi/api/agreements/v1/" + StartupCentral.Code.Class.MobilePay.GetAgreementList().First().Id);
            client.ClientCertificates = new X509CertificateCollection();
            client.ClientCertificates.Add(oCert);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("authenticateduser", "e3ad5035-67b1-4114-a5c3-73f07882e24b");
            request.AddHeader("x-ibm-client-secret", "P2kJ5cP4jY4pH7cY8kX0sP3nN8hS4lY8cX0tJ4vB7sG5rA0vV7");
            request.AddHeader("x-ibm-client-id", "204bbb19-8ca4-4b6b-abbb-36390c602f84");
            request.AddParameter("application/json", "{\"status\":\"Rejected\"}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            return response.Content;

        }

        [HttpGet]
        public string Decline()
        {

            string certFile = System.IO.Path.Combine("C:\\", "Certifikater", "StartupCentral Sandbox cert", "Sandbox_MobilePay_StartupCentral.pfx");
            X509Certificate2 oCert = new X509Certificate2(certFile, "Attluc2018#");
            var client = new RestClient("https://api.sandbox.mobilepay.dk/recurringpayments-restapi/api/agreements/v1/" + StartupCentral.Code.Class.MobilePay.GetAgreementList().First().Id);
            client.ClientCertificates = new X509CertificateCollection();
            client.ClientCertificates.Add(oCert);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("authenticateduser", "e3ad5035-67b1-4114-a5c3-73f07882e24b");
            request.AddHeader("x-ibm-client-secret", "P2kJ5cP4jY4pH7cY8kX0sP3nN8hS4lY8cX0tJ4vB7sG5rA0vV7");
            request.AddHeader("x-ibm-client-id", "204bbb19-8ca4-4b6b-abbb-36390c602f84");
            request.AddParameter("application/json", "{\"status\":\"Declined\"}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            return response.Content;

        }
        [HttpGet]
        public string List()
        {

            return StartupCentral.Code.Class.MobilePay.GetAgreementListToString();

        }

        //[HttpGet]
        //public string Testing()
        //{
        //    IMember member = Services.MemberService.GetByKey(new Guid("72ad901d-fe6a-4e52-8f8e-3de008f9c51d"));
        //    return Class.MobilePay.RequestPaymentRequest("b3431db0-281f-4307-a6ec-da3a35e54aa1", 0.01, member);
        //}

    }
}