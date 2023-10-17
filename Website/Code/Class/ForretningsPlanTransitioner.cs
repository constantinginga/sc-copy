using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Extra namespaces
using umbraco.cms.businesslogic.member;
using Umbraco.Web.UI.Controls;
using Umbraco.Web;
using StartupCentral.DAL.EntityModels;
using OfficeOpenXml.FormulaParsing.Utilities;
using StartupCentral.Extensions;
using StartupCentral.Code.Model;
using System.Threading.Tasks;
using Umbraco.Core.Models;

using System.Diagnostics;

namespace StartupCentral.Code.Class
{
    public class ForretningsPlanTransitioner : Umbraco.Web.UI.Controls.UmbracoUserControl
    {
        readonly ForretningsPlanService forretningsPlanService;

        public ForretningsPlanTransitioner()
        {
            forretningsPlanService = new ForretningsPlanService();
        }

        public async Task<ApiResponse> Read_ForrentningsPlanNodes()
        {
            var driverParentNode = Umbraco.TypedContent(1606);
            var studentFolders = driverParentNode.Descendants().Where(x => x.DocumentTypeAlias == "enkeltstudent").OrderBy(n => n.Name);

            List<userForretningsPlan> _userForretningsPlan = new List<userForretningsPlan>();

            foreach (var folder in studentFolders)
            {
                var memberEmail = folder.Name;
                var memberId = int.Parse(folder.GetPropertyValue("memberid").ToString());

                try
                {
                    IMember m = null;
                    m = Services.MemberService.GetById(memberId);                   

                    if (m != null)
                    {
                        var contentId = folder.Id;

                        var businessPlans = folder.Descendants();//.Where(x => x.DocumentTypeAlias == "forretningsplan");

                        foreach (var plan in businessPlans)
                        {
                            if (!string.IsNullOrEmpty(plan.GetPropertyValue<string>("wwprojektnavn")))
                            {
                                var userForretningsPlan = new userForretningsPlan();
                                UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                                userForretningsPlan.ProjektNavn = plan.GetPropertyValue<string>("wwprojektnavn").ToString();
                                userForretningsPlan.Id = plan.GetKey();
                                userForretningsPlan.Owner = new Guid(m.Key.ToString());
                                userForretningsPlan.CreateDate = plan.CreateDate;
                                userForretningsPlan.UpdateDate = plan.UpdateDate;

                                var completed = false;
                                if (!string.IsNullOrEmpty(plan.GetPropertyValue<string>("wwcompleted")))
                                    completed = plan.GetPropertyValue<string>("wwcompleted").ToString().ParseBoolean();
                                userForretningsPlan.Completed = completed;

                                //Introduktion
                                userForretningsPlan.Kategori = plan.GetPropertyValue<string>("wwkategori").ToString();

                                var eng = false;
                                if (!string.IsNullOrEmpty(plan.GetPropertyValue<string>("wweng")))
                                    eng = plan.GetPropertyValue<string>("wweng").ToString().ParseBoolean();
                                userForretningsPlan.English = eng;

                                userForretningsPlan.IdeDesc = plan.GetPropertyValue<string>("wwidedesc").ToString();

                                //Idegrundlag
                                userForretningsPlan.Uddyb = plan.GetPropertyValue<string>("wwuddyb").ToString();
                                userForretningsPlan.Loser = plan.GetPropertyValue<string>("wwloser").ToString();
                                userForretningsPlan.Videreudvik = plan.GetPropertyValue<string>("wwvidereudvik").ToString();
                                userForretningsPlan.Brugprod = plan.GetPropertyValue<string>("wwbrugprod").ToString();

                                //Målsætning
                                userForretningsPlan.Splitaktiviteter = plan.GetPropertyValue<string>("wwsplitaktiviteter").ToString();

                                //Værdier
                                userForretningsPlan.FirmImportant = plan.GetPropertyValue<string>("wwfirmimportant").ToString();
                                userForretningsPlan.FirmBehov = plan.GetPropertyValue<string>("wwfirmbehov").ToString();
                                userForretningsPlan.FirmReach = plan.GetPropertyValue<string>("wwfirmreach").ToString();
                                userForretningsPlan.FirmIssues = plan.GetPropertyValue<string>("wwfirmissues").ToString();
                                userForretningsPlan.FirmValues = plan.GetPropertyValue<string>("wwfirmvalues").ToString();

                                //Team og ressourcer
                                userForretningsPlan.FirmRoller = plan.GetPropertyValue<string>("wwfirmroller").ToString();
                                userForretningsPlan.FirmOkonomi = plan.GetPropertyValue<string>("wwfirmokonomi").ToString();
                                userForretningsPlan.FirmRes = plan.GetPropertyValue<string>("wwfirmres").ToString();
                                userForretningsPlan.FirmDriv = plan.GetPropertyValue<string>("wwfirmdriv").ToString();
                                userForretningsPlan.FirmVwork = plan.GetPropertyValue<string>("wwfirmvwork").ToString();

                                //Produktbeskrivelse
                                userForretningsPlan.ProdDesc = plan.GetPropertyValue<string>("wwproddesc").ToString();
                                userForretningsPlan.ProdVers = plan.GetPropertyValue<string>("wwprodvers").ToString();
                                userForretningsPlan.ProdFunkt = plan.GetPropertyValue<string>("wwprodfunkt").ToString();
                                userForretningsPlan.ProdMake = plan.GetPropertyValue<string>("wwprodmake").ToString();
                                userForretningsPlan.ProdReady = plan.GetPropertyValue<string>("wwprodready").ToString();

                                //Markedsanalyse
                                userForretningsPlan.CusWho = plan.GetPropertyValue<string>("wwcuswho").ToString();
                                userForretningsPlan.CusFind = plan.GetPropertyValue<string>("wwcusfind").ToString();
                                userForretningsPlan.CusNeeds = plan.GetPropertyValue<string>("wwcusneeds").ToString();
                                userForretningsPlan.CusKon = plan.GetPropertyValue<string>("wwcuskon").ToString();
                                userForretningsPlan.CusStrong = plan.GetPropertyValue<string>("wwcusstrong").ToString();

                                //Økonomi
                                userForretningsPlan.ProdSale = plan.GetPropertyValue<string>("wwprodsale").ToString();
                                userForretningsPlan.ProdStart = plan.GetPropertyValue<string>("wwprodstart").ToString();
                              
                                //int budgetfile = 0;
                                string budgetfileName = string.Empty;
                                try
                                {
                                    if (plan.GetPropertyValue<IPublishedContent>("wwbudgetfile") != null)  //plan.HasValue("wwbudgetfile") && 
                                    {
                                        var file = plan.GetPropertyValue<IPublishedContent>("wwbudgetfile");
                                        //Services.ContentService.GetById(plan.GetPropertyValue<int>("wwbudgetfile"));

                                        if (file != null)
                                        {
                                            budgetfileName = $"{file.Url}";
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                
                                userForretningsPlan.BudgetFile = budgetfileName;

                                userForretningsPlan.ProdFinance = plan.GetPropertyValue<string>("wwprodfinance").ToString();

                                //Salgsmarkedsføring
                                userForretningsPlan.ProdPrice = plan.GetPropertyValue<string>("wwprodprice").ToString();
                                userForretningsPlan.ProdUdregn = plan.GetPropertyValue<string>("wwprodudregn").ToString();
                                userForretningsPlan.ProdSaleplace = plan.GetPropertyValue<string>("wwprodsaleplace").ToString();
                                userForretningsPlan.ProdEksi = plan.GetPropertyValue<string>("wwprodeksi").ToString();
                                userForretningsPlan.ProdMarkedsf = plan.GetPropertyValue<string>("wwprodmarkedsf").ToString();

                                //Juridisk
                                userForretningsPlan.JurAftale = plan.GetPropertyValue<string>("wwjuraftale").ToString();
                                userForretningsPlan.JurLeverand = plan.GetPropertyValue<string>("wwjurleverand").ToString();
                                userForretningsPlan.JurContact = plan.GetPropertyValue<string>("wwjurcontact").ToString();
                                userForretningsPlan.JurCvr = plan.GetPropertyValue<string>("wwjurcvr").ToString();

                                var andetchk = false;
                                var andetheadline = string.Empty;
                                var andet = string.Empty;

                                if (!string.IsNullOrEmpty(plan.GetPropertyValue<string>("wwandetchk")))
                                {
                                    andetchk = plan.GetPropertyValue<string>("wwandetchk").ToString().ParseBoolean();
                                    andetheadline = plan.GetPropertyValue<string>("wwandetheadline").ToString();
                                    andet = plan.GetPropertyValue<string>("wwandet").ToString();
                                }             

                                userForretningsPlan.AndetChk = andetchk;
                                userForretningsPlan.AndetHeadline = andetheadline;
                                userForretningsPlan.Andet = andet;

                                //Debug.WriteLine("Start await with projectName {0}", userForretningsPlan.ProjektNavn);
                                //await forretningsPlanService.Migrate(userForretningsPlan);
                                //Debug.WriteLine("Done await projectName {0}", userForretningsPlan.ProjektNavn);

                                _userForretningsPlan.Add(userForretningsPlan);
                            }
                            else
                            {
                                Debug.WriteLine("Bsp has no title");
                                continue;
                            }                  
                        }
                    }
                }
                catch
                {
                    Debug.WriteLine("Member doesnt exsits naymore");
                    continue;
                }
            }

            await forretningsPlanService.Migrate(_userForretningsPlan);

            return new ApiResponse(true, "moved");
        }

        public string GetMediaFilename(string value)
        {
            if (value != null)
            {
                try
                {
                    Umbraco.Web.UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                    if (value.IsNumeric())
                    {
                        return umbracoHelper.TypedMedia(Convert.ToInt32(value)).Name;
                    }
                    else if (value.Contains("umb://"))
                    {
                        int id = value.GetIdByUdi();
                        if (id > 0)
                        {
                            return umbracoHelper.TypedMedia(id).Name;
                        }
                    }
                }
                catch (System.Exception) { }
            }

            return string.Empty;
        }
    }
}