using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.ApiControllers
{
    public class MobilePayOperationsController : UmbracoApiController
    {
        [HttpGet]
        public string[] CheckFolder()
        {
            return System.IO.Directory.GetFiles(@"C:\Certifikater\StartupCentral prod cert\");


        }

        Guid memberKey = new Guid("dd128869-e26f-44ea-846a-779e46f9e6be");

        [HttpGet]
        public List<Model.MobilePay.Agreement> GetAgreement()
        {

            List<Model.MobilePay.Agreement> acceptedAgreements = Class.MobilePay.GetAgreementList(status: Class.MobilePay.AgreementStatusType.Accepted);
            if (acceptedAgreements != null)
            {
                return acceptedAgreements.Where(itm => itm.Status == "Accepted" && itm.DateCanceled == null).OrderByDescending(itm => itm.DateAccepted).ToList();
            }

            return null;

        }


        [HttpGet]
        public List<Model.MobilePay.OneOffPaymentResponse> GetAgreements()
        {

            List<Model.MobilePay.Agreement> acceptedAgreements = Class.MobilePay.GetAgreementList(status: Class.MobilePay.AgreementStatusType.Accepted);
            if (acceptedAgreements != null)
            {
                Model.MobilePay.Agreement theAgreement = acceptedAgreements.Where(itm => itm.ExternalId == memberKey.ToString() && itm.Status == "Accepted").FirstOrDefault();
                if (theAgreement != null)
                {
                    List<Model.MobilePay.OneOffPaymentResponse> offPaymentResponses = Class.MobilePay.GetOneOffPayments(theAgreement.Id);

                    return offPaymentResponses;

                }
            }

            return null;

        }




        [HttpGet]
        public List<Model.MobilePay.OneOffPaymentResponse> GetPayments()
        {
            List<Model.MobilePay.Agreement> acceptedAgreements = Class.MobilePay.GetAgreementList(status: Class.MobilePay.AgreementStatusType.Accepted);
            if (acceptedAgreements != null)
            {

                List<Model.MobilePay.OneOffPaymentResponse> list = new List<Model.MobilePay.OneOffPaymentResponse>();

                foreach (var ag in acceptedAgreements)
                {

                    var payments = Class.MobilePay.GetOneOffPayments(ag.Id);
                    if (payments != null)
                    {
                        foreach (var p in payments)
                        {
                            list.Add(p);

                            //var agg = Class.MobilePay.GetAgreement(p.AgreementId)

                            Guid? guid = null;
                            try
                            {
                                guid = new Guid(ag.ExternalId);

                                Umbraco.Core.Models.IMember member = Services.MemberService.GetByKey(guid.Value);
                                if (member != null)
                                {
                                    Class.MobilePay.RefundPayment(ag.Id, p.Id, member, Convert.ToDouble(p.Amount));
                                }

                            }
                            catch (System.Exception)
                            {

                            }






                        }
                    }

                }

                return list;

            }

            return null;
        }


        [HttpGet]
        public List<Model.MobilePay.OneOffPaymentResponse> GetAgreementss()
        {

            List<Model.MobilePay.Agreement> acceptedAgreements = Class.MobilePay.GetAgreementList();
            if (acceptedAgreements != null)
            {
                List<Model.MobilePay.OneOffPaymentResponse> offPaymentResponses = new List<Model.MobilePay.OneOffPaymentResponse>();

                foreach (var ag in acceptedAgreements)
                {
                    var list = Class.MobilePay.GetOneOffPaymentRequests(ag.Id);
                    if (list != null)
                    {
                        foreach (var itm in list)
                        {
                            offPaymentResponses.Add(itm);

                            //var lort = Class.MobilePay.GetOneOffPayment(itm.AgreementId, itm.Id);

                            //Class.MobilePay.DeleteOneOffPayment(itm.AgreementId, itm.Id);

                            //CapturePayment(itm.AgreementId, itm.Id);
                        }
                    }
                }


                foreach (var ag in acceptedAgreements)
                {
                    var list = Class.MobilePay.GetPaymentRequests(ag.Id);
                    if (list != null)
                    {
                        foreach (var itm in list)
                        {
                            offPaymentResponses.Add(itm);
                            //Class.MobilePay.DeleteOneOffPayment(itm.AgreementId, itm.Id);

                            //CapturePayment(itm.AgreementId, itm.ExternalId);

                        }
                    }
                }

                return offPaymentResponses;

            }

            return null;
        }

        [HttpGet]
        public string ReqeustPayment()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            var list = Class.MobilePay.GetOneOffPayments("2e059bd1-f332-43bb-adff-3371fb0db29b");
            if (list != null)
            {

                IMember member = Services.MemberService.GetByKey(new Guid("d3ae9b99-d6ed-49b8-884f-fb3dd43b8e39"));


                foreach (var itm in list)
                {
                    str.Append(Class.MobilePay.RefundPayment(itm.AgreementId, itm.Id, member, 0.01));
                }
            }

            return str.ToString();
        }

        [HttpGet]
        public string CapturePayment(string agreementId, string paymentId)
        {
            return Class.MobilePay.CatpurePayment(agreementId, paymentId);

        }

        [HttpGet]
        public string Refund()
        {

            IMember member = Services.MemberService.GetByUsername("sl@lukic.nu");
            return StartupCentral.Code.Class.MobilePay.RefundPayment("88e73a69-4790-4034-9f2b-9c083632693e", "b6e4d9e4-aad5-438d-b772-68cda3562f76", member, 149);

        }


    }
}