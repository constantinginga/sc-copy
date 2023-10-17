using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model
{
    public class CvrOpslag
    {
        public class ApiOwners
        {
            public string Name { get; set; }
        }

        public class ApipPoductionunits
        {
            public string Pno { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Zipcode { get; set; }
            public string City { get; set; }
            public bool @protected { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Fax { get; set; }
            public string Startdate { get; set; }
            public string Enddate { get; set; }
            public string Employees { get; set; }
            public string Addressco { get; set; }
            public int Industrycode { get; set; }
            public string Industrydesc { get; set; }
            public int Companycode { get; set; }
            public string Companydesc { get; set; }
            public string Creditstartdate { get; set; }
            public int? Creditstatus { get; set; }
            public bool Creditbankrupt { get; set; }
        }

        public class ApiResult
        {
            public string VAT { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Zipcode { get; set; }
            public string City { get; set; }
            public bool @protected { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Fax { get; set; }
            public string Startdate { get; set; }
            public string Enddate { get; set; }
            public string Employees { get; set; }
            public string Addressco { get; set; }
            public int Industrycode { get; set; }
            public string Industrydesc { get; set; }
            public int Companycode { get; set; }
            public string Companydesc { get; set; }
            public string Creditstartdate { get; set; }
            public int? Creditstatus { get; set; }
            public bool Creditbankrupt { get; set; }
            public ApiOwners[] Owners { get; set; }
            public ApipPoductionunits[] Productionunits { get; set; }
            public int T { get; set; }
            public int Version { get; set; }

        }
    }
}