using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using StartupCentral.Models;
using StartupCentral.Code.Class;
using System.Threading.Tasks;
using Stripe;

namespace StartupCentral.Controllers
{
    public class RegisterUserPaymentController : SurfaceController
    {

        [HttpPost]
        public ActionResult Submit(RegisterUserPayment model)
        {
            return Redirect("/");
        }

        [HttpPost]
        public async Task<JsonResult> ApplyPromocode(string promocode)
        {
            var upodiWrapperService = new UpodiWrapperService();

            var isValidDiscount = await upodiWrapperService.ValidateDiscountCode(promocode);

            if (!isValidDiscount)
                return Json("Den indtastede rabatkode er forkert. Prøv igen.");

            var discountPercentage = await upodiWrapperService.GetDiscountPercentage(promocode);

            return Json(discountPercentage);
        }

        //[HttpPost]
        //public JsonResult ValidateCc(string Kortnummer)
        //{
        //    // Luhn check
        //    if (string.IsNullOrEmpty(Kortnummer)) return Json("Invalid cc");
        //    int nDigits = Kortnummer.Length;

        //    int nSum = 0;
        //    bool isSecond = false;
        //    for (int i = nDigits - 1; i >= 0; i--)
        //    {

        //        int d = Kortnummer[i] - '0';

        //        if (isSecond == true)
        //            d = d * 2;

        //        // We add two digits to handle
        //        // cases that make two digits
        //        // after doubling
        //        nSum += d / 10;
        //        nSum += d % 10;

        //        isSecond = !isSecond;
        //    }
        //    return (nSum % 10 == 0) ? Json(true) : Json("Invalid Cc");
        //}
    }
}