using Newtonsoft.Json;
using OfficeOpenXml;
using StartupCentral.Code.Class;
using StartupCentral.Code.Model;
using StartupCentral.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;
using Core = Umbraco.Core;
using static StartupCentral.Code.Model.CvrOpslag;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR.Json;

namespace StartupCentral.Code.ApiControllers
{
    public class MemberApiController : UmbracoApiController
    {

        [HttpPost]
        public bool LoginByUsername()
        {
            
            string username = HttpContext.Current.Request.Form["username"];
            string clearTextPassword = HttpContext.Current.Request.Form["password"];

            // Check if username and password are provided
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(clearTextPassword))
            {
                return false;
            }

            var member = Membership.GetUser(username);

            if (member == null)
            {
                return false; // User not found
            }

            bool result;

            /*
             * OLD CODE, in case smth breaks again :(
            var member = Services.MemberService.GetByEmail(username);
            if (member != null)
            {
                return true;
            }

            */

            // work-around for validating username & pasword of basic users
            // (Membership.ValidateUser only works for approved members)
            bool originalApprovalStatus = member.IsApproved;
            member.IsApproved = true;
            Membership.UpdateUser(member);

            try
            {
                // Validate the user's credentials
                result = Membership.ValidateUser(username, clearTextPassword);
            }
            finally
            {
                // Revert the user's approval status
                member.IsApproved = originalApprovalStatus;
                Membership.UpdateUser(member);
            }

            return result;
        }

        [HttpPost]
        public int LoginAttemptsCount()
        {
            string username = HttpContext.Current.Request.Form["username"];
            IMember member = StartupCentral.Code.Controllers.UserController.GetByEmail(username);
            int leftAttempts = 5- member.FailedPasswordAttempts;
            return leftAttempts;
        }

        [HttpPost]
        public bool IsLockedOut()
        {
            string username = HttpContext.Current.Request.Form["username"];
            var member = System.Web.Security.Membership.GetUser(username);
            return member.IsLockedOut;
        }


        [HttpPost]
        public Model.ApiResponse Create()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                if (HttpContext.Current.Request.ContentLength > 0)
                {
                    System.IO.Stream s = HttpContext.Current.Request.InputStream;
                    System.IO.StreamReader reader = new System.IO.StreamReader(s);
                    System.Diagnostics.Debug.Write(reader.ReadToEnd());

                    string usertype = HttpContext.Current.Request.Form["user"];
                    string name = HttpContext.Current.Request.Form["name"];
                    string username = HttpContext.Current.Request.Form["username"];
                    string shorttxt = HttpContext.Current.Request.Form["shorttxt"];
                    string desc = HttpContext.Current.Request.Form["desc"];
                    string tags = HttpContext.Current.Request.Form["tags"];
                    bool unavailable = HttpContext.Current.Request.Form["unavailable"].ParseBoolean();
                    System.DateTime? datefrom = null;
                    System.DateTime? dateTo = null;

                    try
                    {
                        datefrom = System.Convert.ToDateTime(HttpContext.Current.Request.Form["datefrom"]);
                        dateTo = System.Convert.ToDateTime(HttpContext.Current.Request.Form["dateto"]);
                    }
                    catch (System.Exception)
                    {
                    }

                    if (string.IsNullOrEmpty(name) || name.Length <= 2)
                    {
                        return new Model.ApiResponse() { Success = false, Message = "Navn skal udfyldes korrekt." };
                    }

                    if (Services.MemberService.Exists(username))
                    {
                        return new Model.ApiResponse() { Success = false, Message = "Brugernavnet/e-mail findes allerede." };
                    }

                    if (!username.IsEmailAddress())
                    {
                        return new Model.ApiResponse() { Success = false, Message = "E-mail adressen er ikke gyldig." };
                    }

                    IMember newMember = Services.MemberService.CreateMemberWithIdentity(username, username, name, usertype);
                    newMember.SetValue("wwname", name);
                    newMember.SetValue("wwemail", username);

                    // Tags
                    if (newMember.HasProperty("wwtags") && !string.IsNullOrEmpty(tags))
                    {
                        int[] idlist = tags.CsvToInt();
                        if (idlist.Length > 0)
                        {
                            System.Text.StringBuilder tagBuilder = new System.Text.StringBuilder();

                            foreach (int tagid in idlist)
                            {
                                IContent tagContent = Services.ContentService.GetById(tagid);
                                if (tagContent != null)
                                {
                                    tagBuilder.Append($"umb://document/{tagContent.Key.ToString().Replace("-", string.Empty)}{ (tagid != idlist[idlist.Length - 1] ? "," : "")}");
                                }
                            }

                            newMember.SetValue("wwtags", tagBuilder.ToString());
                        }
                    }

                    // Short description
                    if (!string.IsNullOrEmpty(shorttxt))
                    {
                        if (newMember.HasProperty("wwshorttxt"))
                        {
                            newMember.SetValue("wwshorttxt", shorttxt);
                        }
                    }

                    // Description / bodyText
                    if (!string.IsNullOrEmpty(desc))
                    {
                        if (newMember.HasProperty("bodyText"))
                        {
                            newMember.SetValue("bodyText", desc);
                        }
                    }

                    // Unavailable
                    if (newMember.HasProperty("wwunavailable"))
                    {
                        newMember.SetValue("wwunavailable", unavailable);
                    }

                    // datefrom
                    if (newMember.HasProperty("wwbooketfra") && datefrom != null)
                    {
                        newMember.SetValue("wwbooketfra", datefrom.Value);
                    }

                    // dateTo
                    if (newMember.HasProperty("wwbookettil") && dateTo != null)
                    {
                        newMember.SetValue("wwbookettil", dateTo.Value);
                    }

                    IMedia mediaFolder = StartupCentral.Code.Controllers.UserController.GetProfileImageFolder(newMember);


                    if (newMember.HasProperty("wwavatar") && HttpContext.Current.Request.Files["avatar"].ContentLength > 0)
                    {
                        IMedia mediaFile = Services.MediaService.CreateMedia(HttpContext.Current.Request.Files["avatar"].FileName, mediaFolder, "Image");
                        System.IO.Stream avatarStream = HttpContext.Current.Request.Files["avatar"].InputStream;
                        mediaFile.SetValue(Core.Composing.Current.Services.ContentTypeBaseServices, "umbracoFile", HttpContext.Current.Request.Files["avatar"].FileName, avatarStream);//TODO:Verify
                        avatarStream.Close();
                        Services.MediaService.Save(mediaFile);
                        newMember.SetValue("wwavatar", mediaFile.Id);
                    }

                    if (newMember.HasProperty("wwnda") && HttpContext.Current.Request.Files["nda"].ContentLength > 0)
                    {
                        IMedia mediaFile = Services.MediaService.CreateMedia(HttpContext.Current.Request.Files["nda"].FileName, mediaFolder, "File");
                        System.IO.Stream ndaStream = HttpContext.Current.Request.Files["nda"].InputStream;
                        mediaFile.SetValue(Core.Composing.Current.Services.ContentTypeBaseServices, "umbracoFile", HttpContext.Current.Request.Files["nda"].FileName, ndaStream);//TODO:Verify
                        ndaStream.Close();
                        Services.MediaService.Save(mediaFile);
                        newMember.SetValue("wwnda", mediaFile.Id);
                    }

                    if (newMember.HasProperty("wwcv") && HttpContext.Current.Request.Files["cv"].ContentLength > 0)
                    {
                        IMedia mediaFile = Services.MediaService.CreateMedia(HttpContext.Current.Request.Files["cv"].FileName, mediaFolder, "File");
                        System.IO.Stream cvStream = HttpContext.Current.Request.Files["cv"].InputStream;
                        mediaFile.SetValue(Core.Composing.Current.Services.ContentTypeBaseServices, "umbracoFile", HttpContext.Current.Request.Files["cv"].FileName, cvStream); //TODO:Verify
                        cvStream.Close();
                        Services.MediaService.Save(mediaFile);
                        newMember.SetValue("wwcv", mediaFile.Id);
                    }

                    Services.MemberService.Save(newMember);

                    Services.MemberService.AssignRole(username, usertype);
                    Services.MemberService.Save(newMember);

                    //Services.MemberService.SavePassword(newMember, newpassword);

                    //Emailing.SendCreationEmail(newMember, newpassword);

                    SendApprovvalEmail(newMember);

                    return new Model.ApiResponse() { Success = true, Message = "OK" };
                }
            }

            return new Model.ApiResponse() { Success = false, Message = "Token not authorized." };
        }

        [HttpGet]
        public Model.ApiResponse Deactivate(int memberId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IMember mem = Services.MemberService.GetById(memberId);
                    if (mem != null)
                    {
                        mem.IsApproved = false;
                        Services.MemberService.Save(mem, true);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member: {member.Name} ({member.Id}) has deactivated member: {mem.Name} ({mem.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to deactivated member ID: {memberId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to deactivated member ID: {memberId} ");
                return new Model.ApiResponse(false, "Your identity is unknown! For safety purposes, this is added to the log.");
            }
        }

        [HttpGet]
        public Model.ApiResponse Activate(int memberId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IMember mem = Services.MemberService.GetById(memberId);
                    if (mem != null)
                    {
                        mem.IsApproved = true;
                        Services.MemberService.Save(mem, true);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member: {member.Name} ({member.Id}) has activated member: {mem.Name} ({mem.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to deactivated member ID: {memberId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to activate member ID: {memberId} ");
                return new Model.ApiResponse(false, "Your identity is unknown! For safety purposes, this is added to the log.");
            }
        }

        [HttpGet]
        public Model.Member GetMemberById(int memberId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                if (memberId > 0)
                {
                    IMember mem = Services.MemberService.GetById(memberId);
                    if (mem != null)
                    {
                        return new StartupCentral.Code.Model.Member()
                        {
                            Id = mem.Id,
                            Name = mem.Name,
                            Username = mem.Username,
                            Mobile = mem.GetValue<string>("wwmobile"),
                            Key = mem.Key,
                            ContentTypeAlias = mem.ContentTypeAlias,
                            Tags = mem.GetValue("wwtags")?.ToString()?.UdiCSVToIntList()?.ToList() ?? null,
                            AvatarImageUrl = mem.GetValue<string>("wwavatar")?.UmbToSingleImageUrl()?.Url ?? null,
                            ShortText = mem.HasProperty("wwshorttxt") ? mem.GetValue<string>("wwshorttxt").ToString() : "",
                            Description = mem.HasProperty("bodyText") ? mem.GetValue<string>("bodyText").ToString() : "",
                            Unavailable = mem.GetValue<bool>("wwunavailable"),
                            DateFrom = mem.GetValue<DateTime?>("wwbooketfra"),
                            DateTo = mem.GetValue<DateTime?>("wwbookettil"),
                            NDAUrl = mem.GetValue<string>("wwnda")?.UmbToSingleImageUrl()?.Url ?? null,
                            CVUrl = mem.GetValue<string>("wwcv")?.UmbToSingleImageUrl()?.Url ?? null
                        };
                    }
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to GetMemberById member ID: {memberId} ");
            }

            return null;
        }

        [HttpGet]
        public Model.ApiResponse Delete(int memberId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IMember mem = Services.MemberService.GetById(memberId);
                    if (mem != null)
                    {
                        Services.MemberService.Delete(mem);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member: {member.Name} ({member.Id}) has delete member: {mem.Name} ({mem.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to delete member ID: {memberId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to delete member ID: {memberId} ");
                return new Model.ApiResponse(false, "Your identity is unknown! For safety purposes, this is added to the log.");
            }
        }

        //[HttpGet]
        //public HttpResponseMessage DownloadUsersListFile()
        //{
        //    IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
        //    if (member == null && member.ContentType.Alias.ToLower() != "admin")
        //        return new HttpResponseMessage(HttpStatusCode.Forbidden);
        //    //create new xls file 
        //    string fileName = string.Format("users {0}.xlsx", DateTime.Now.ToString("dd-MM-yyyy"));
        //    using (var excel = new ExcelPackage())
        //    {
        //        var workSheet = excel.Workbook.Worksheets.Add("Users");
        //        workSheet.Cells["A1"].Value = "Name";
        //        workSheet.Cells["B1"].Value = "Mobile";
        //        workSheet.Cells["C1"].Value = "Email";
        //        workSheet.Cells["D1"].Value = "Create Date";

        //        int i = 2;
        //        Services.MemberService.GetAll(0, int.MaxValue, out long totalRecords).Where(x => x.ContentType.Alias.ToLower() == "student").ToList()
        //            .ForEach(item =>
        //            {
        //                workSheet.Cells["A" + i].Value = item.Name;
        //                workSheet.Cells["B" + i].Value = item.GetValue<string>("wwmobile");
        //                workSheet.Cells["C" + i].Value = item.Email;
        //                workSheet.Cells["D" + i].Value = item.CreateDate.ToString();
        //                i++;
        //            });
        //        workSheet.Column(1).AutoFit();
        //        workSheet.Column(2).AutoFit();
        //        workSheet.Column(3).AutoFit();
        //        workSheet.Column(4).AutoFit();
        //        int finalrows = workSheet.Dimension.End.Row;
        //        string ColumnString = "A1:D" + finalrows.ToString();
        //        var rang = workSheet.Cells[ColumnString];
        //        rang.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        rang.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            excel.SaveAs(ms);
        //            var result = new HttpResponseMessage(HttpStatusCode.OK)
        //            {
        //                Content = new ByteArrayContent(ms.ToArray())
        //            };
        //            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //            {
        //                FileName = fileName
        //            };
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        //            return result;
        //        }
        //    }
        //}


        [HttpPost]
        public Guid CreateUserFromLounge()
        {
            string name = HttpContext.Current.Request.Form["name"];
            string email = HttpContext.Current.Request.Form["email"];
            string password = HttpContext.Current.Request.Form["password"];
            string phoneNumber = HttpContext.Current.Request.Form["phoneNumber"];
            string cvr = HttpContext.Current.Request.Form["cvr"];

            
            IMember member = Services.MemberService.CreateMemberWithIdentity(email, email, name, "student");
            if (member != null)
            {
                member.SetValue("wwmobile", phoneNumber);
                member.SetValue("wwMobilePayAbonnementsDato", null);
                member.SetValue("wwemail", email);

                if (!string.IsNullOrWhiteSpace(cvr))
                {
                    member.SetValue("cVR", cvr);
                }

                member.SetValue("wwname", (!string.IsNullOrEmpty(name) ? name : email));
                //member.SetValue("umbracoNewsletterSubscriber", model.Newsletter);
                member.SetValue("isFreeLoungeUser", true);
                member.IsApproved = false;
                Services.MemberService.Save(member);
                Services.MemberService.AssignRole(email, "student");
                Services.MemberService.SavePassword(member, password);
                var UpodiWrapperService = new UpodiWrapperService();
                UpodiWrapperService.CreateNewCustomer(member.Id, member);
            }

            return member.Key;

        }

        [HttpPost]
        public IMember UpdateUserFromLounge(){
            int memberId = int.Parse(HttpContext.Current.Request.Form["memberId"]);
            string name = HttpContext.Current.Request.Form["name"];
            string phoneNumber = HttpContext.Current.Request.Form["phoneNumber"];

            IMember member = Services.MemberService.GetById(memberId);

            if (member != null)
            {
                member.Name = name;
                member.SetValue("wwname", name);
                member.SetValue("wwmobile", phoneNumber);
            }

            if (member.IsDirty())
            {
                UpodiWrapperService upodiWrapperService = new UpodiWrapperService();
                upodiWrapperService.updateCustomerPrivateInfo(member);

                ApplicationContext.Current.Services.MemberService.Save(member);
            }
            return member; 
        }

        [HttpPost]
        public async Task UpdateUserOnLounge(IMember member)
        {
            string endPoint = $"https://localhost:7297/Profiles/SyncUserFromUmbraco";

            using (HttpClient httClient = new HttpClient())
            {
                try
                {
                    var ContactsSection = new
                    {
                        MemberId = member.Id,
                        PhoneNumber = member.GetValue<string>("wwmobile"),
                    };
                    var data = new
                    {
                        MemberId = member.Id,
                        Name = member.Name.ToString(),
                        ContactsSection = ContactsSection,
                    };
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httClient.PostAsync(endPoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Request was successful.");
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response Content: " + responseContent);
                    }
                    else
                    {
                        Console.WriteLine("Request failed with status code: " + response.StatusCode);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Update user on Lounge failed with exception: " + e.Message);
                }
            }
        }

        [HttpGet]
        public List<LoungeFreeUserInfo> LoungeFreeUsers()
        {
            var loungeMembers = new List<LoungeFreeUserInfo>();
            var students = Services.MemberService.GetMembersByMemberType("student");
            var members =  students.Where(x => x.IsApproved == false && x.GetValue<bool>("isFreeLoungeUser") == true);

            foreach (var member in members)
            {
                var loungeMember = new LoungeFreeUserInfo();
                loungeMember.Id = member.Id;
                loungeMember.Name = member.Name;
                loungeMember.Email = member.Email;
                loungeMember.PhoneNumber = member.GetValue<string>("wwmobile");
                loungeMember.CreateDate = member.CreateDate;
                loungeMember.IsApproved = member.IsApproved;
                loungeMember.CreatedFromLounge = member.GetValue<bool>("isFreeLoungeUser");
                loungeMembers.Add(loungeMember);
            }
            return loungeMembers;
        }

        [HttpGet]
        public List<BasicMembersInfo> BasicMembershipUsers()
        {
            var BasicMembers = new List<BasicMembersInfo>();
            var students = Services.MemberService.GetMembersByMemberType("student");
            var members = students.Where(x => x.IsApproved == false && x.GetValue<bool>("isFreeLoungeUser") == false && x.GetValue<bool>("basicMembershipUser") == true);

            foreach (var member in members)
            {
                var basicmember = new BasicMembersInfo();
                basicmember.Id = member.Id;
                basicmember.Name = member.Name;
                basicmember.Email = member.Email;
                basicmember.PhoneNumber = member.GetValue<string>("wwmobile");
                basicmember.CreateDate = member.CreateDate;
                basicmember.IsApproved = member.IsApproved;
                basicmember.BasicMemberShip = member.GetValue<bool>("basicMembershipUser");
                BasicMembers.Add(basicmember);
            }
            return BasicMembers;
        }

        [HttpGet]
        public async Task<List<VirksomhedData>> GetMemberCVRData()
        {
            var members = Services.MemberService.GetMembersByMemberType("student");
            var membersWithCvr = members.Where(m => m.GetValue("cVR") != null).ToList();
            List<VirksomhedData> apiResult = new List<VirksomhedData>();
            CvrController cvrController = new CvrController();

            foreach (var member in membersWithCvr)
            {
                if (member.GetValue("cVR") is string cvr && !string.IsNullOrEmpty(cvr))
                {
                    ApiResponse memberCvrData = await cvrController.GetCompanyInfo(cvr);

                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(memberCvrData.Message);

                    JToken hitsToken = jsonObject["hits"]["hits"];
                    if (hitsToken != null && hitsToken.HasValues)
                    {
                        JToken sourceToken = hitsToken[0]?["_source"]["Vrvirksomhed"];
                        if (sourceToken != null)
                        {
                            VirksomhedData virksomhedData = new VirksomhedData
                            {
                                CompanyName = sourceToken["navne"]?.FirstOrDefault()?["navn"]?.ToString(),
                                RegistrationDate = sourceToken["navne"]?.FirstOrDefault()?["periode"]?["gyldigFra"]?.ToObject<DateTime>(),
                                RegisteredAddress = sourceToken["beliggenhedsadresse"]?.FirstOrDefault()?["vejnavn"]?.ToString() + " " +
                                                    sourceToken["beliggenhedsadresse"]?.FirstOrDefault()?["husnummerFra"]?.ToString() + ", " +
                                                    sourceToken["beliggenhedsadresse"]?.FirstOrDefault()?["postdistrikt"]?.ToString() + ", " +
                                                    sourceToken["beliggenhedsadresse"]?.FirstOrDefault()?["landekode"]?.ToString(),
                                PhoneNumber = sourceToken["telefonNummer"]?.FirstOrDefault()?["kontaktoplysning"]?.ToString(),
                                EmailAddresses = sourceToken["elektroniskPost"]?.Select(email => email?["kontaktoplysning"]?.ToString()).ToList(),
                                Website = sourceToken["hjemmeside"]?.Select(website => website?["kontaktoplysning"]?.ToString()).ToList(),
                                CompanyForm = sourceToken["virksomhedsform"]?.FirstOrDefault()?["langBeskrivelse"]?.ToString(),
                                MainBranch = sourceToken["hovedbranche"]?.FirstOrDefault()?["branchetekst"]?.ToString(),
                                NumberOfEmployees = sourceToken["aarsbeskaeftigelse"]?.FirstOrDefault()?["antalAnsatte"]?.ToObject<int>()
                            };

                            apiResult.Add(virksomhedData);
                        }
                    }
                }
            }

            return apiResult;
        }






        //[HttpGet]
        //public List<BasicMembersInfo> LastTwoWeeksSignups()
        //{
        //    var now = DateTime.Now;
        //    var twoWeeks = now.AddDays(-12);
        //    var BasicMembers = new List<BasicMembersInfo>();
        //    var students = Services.MemberService.GetMembersByMemberType("student");
        //    var members = students.Where(x => x.IsApproved == false && x.GetValue<bool>("isFreeLoungeUser") == false && x.CreateDate >= twoWeeks);

        //    foreach (var member in members)
        //    {
        //        var basicmember = new BasicMembersInfo();
        //        basicmember.Id = member.Id;
        //        basicmember.Name = member.Name;
        //        basicmember.Email = member.Email;
        //        basicmember.PhoneNumber = member.GetValue<string>("wwmobile");
        //        basicmember.CreateDate = member.CreateDate;
        //        basicmember.IsApproved = member.IsApproved;
        //        BasicMembers.Add(basicmember);
        //    }
        //    return BasicMembers;
        //}

        [HttpPost]
        public void ChangeCoachAsStudent(bool res, int contentId)
        {
            try
            {

                var contentService = Services.ContentService;

                // Get the page using the GUID you've defined
                var content = contentService.GetById(contentId);

                //var isCoach = content.GetValue("isCoach");
                content.SetValue("isStudent", res);
                contentService.Save(content);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #region Methods

        public void SendApprovvalEmail(IMember member)
        {
            if (member != null && member.ContentType.Alias.ToLower() == "student")
            {
                string password = Passwords.GenerateNewPassword();
                Services.MemberService.SavePassword(member, password);
                Emailing.SendApprovvalEmail(member, password, false);
            }
        }

        [HttpGet]
        public ApiResponse SendOfferEmail(int userId, int nodeId)
        {
            try
            {
                Emailing.SendExtraOffer(userId, nodeId);
                return new ApiResponse(true, "Extra offer sent");
            }
            catch
            {
                return new ApiResponse(false, "Error sending the offer");
            }
        }
        [HttpGet]
        public async Task<ApiResponse> SendFriendInvitation(string friendEmail, int userId)
        {
            try
            {
                await Emailing.SendFriendInvite(friendEmail, userId);
                return new ApiResponse(true, "Invite sent");
            }
            catch
            {
                return new ApiResponse(false, "Error sending the invitation");
            }
        }

        [HttpGet]
        public async Task<ApiResponse> SendNotificationPartner(int userId, int partnerId)
        {
            try
            {
                await Emailing.SendInterestNotificationPartner(userId, partnerId);
                return new ApiResponse(true, "Proceed to partner link.");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"SendNotificationPartner: {e.Message}, Stack trace: {e.StackTrace}");
            }
        }

        [HttpGet]
        public async Task<ApiResponse> RequestPromoCode(bool p, int userId, int partnerId)
        {
            if (p)
            {
                try
                {
                    var part = Services.MemberService.GetById(partnerId);
                    //Umbraco.Member(partnerId);

                    if (part != null)
                    {
                        var promoCode = part.GetValue<string>("wwwpromoCode");

                        if (!string.IsNullOrEmpty(promoCode))
                        {
                            var notResult = await SendNotificationPartner(userId, partnerId);

                            if (notResult.Success)
                            {
                                return new ApiResponse(true, "Promocode provided.", promoCode);
                            }
                            else
                            {
                                return notResult;
                            }
                        }

                        return new ApiResponse(false, "Promocode empty.");
                    }

                    return new ApiResponse(false, "Partner not found.");
                }
                catch (Exception e)
                {
                    return new ApiResponse(false, $"MemberAPIController: {e.Message}");
                }
            }
            return new ApiResponse(true, "Promocode provided with no notification.");


            //return new ApiResponse(false, "Pointer false.");
        }
        #endregion

        [HttpGet]
        public HttpResponseMessage GetLoungeMembers()
        {
            var maxTicks = DateTime.MinValue.Ticks;
            if (Request.GetQueryNameValuePairs().FastAny())
            {
                var maxTicksString = Request.GetQueryNameValuePairs().SingleOrDefault(x => x.Key == "maxTicks").Value;
                long.TryParse(maxTicksString, out maxTicks);
            }
            var maxDt = new DateTime(maxTicks);

            try
            {
                var allMembers = Services.MemberService
                    .GetAll(0, int.MaxValue, out long totalRecords)
                    .Where(x => x.UpdateDate > maxDt);

                var result = new List<MemberInfo>();
                foreach (var x in allMembers)
                {
                    var dto = new MemberInfo()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Url = (x.GetValue<string>("urlProfile") != null ? x.GetValue<string>("urlProfile") : x.Name.Replace(" ", "-")),
                        Login = x.Username,
                        Email = x.Email,
                        CreateDate = x.CreateDate,
                        UpdateDate = x.UpdateDate,
                        RawPasswordValue = x.GetValue<string>("userToken"),
                        ContentType = x.ContentType.Alias,
                        IsApproved = x.IsApproved,
                        Key = x.Key,
                    };
                    
                    var z = 0;
                    if (x.ContentType.Alias != "partner")
                    {
                        if (x.GetValue<string>("wwavatar").IsNumeric())
                        {
                            z = Convert.ToInt32(x.GetValue<string>("wwavatar"));
                        }
                        else if (!string.IsNullOrEmpty(x.GetValue<string>("wwavatar")))
                        {
                            z = x.GetValue<string>("wwavatar").GetIdByUdi();
                        }
                    }
                    else
                    {
                        if (x.GetValue<string>("wwpartnerAvatar").IsNumeric())
                        {
                            z = Convert.ToInt32(x.GetValue<string>("wwpartnerAvatar"));
                        }
                        else if (!string.IsNullOrEmpty(x.GetValue<string>("wwpartnerAvatar")))
                        {
                            z = x.GetValue<string>("wwpartnerAvatar").GetIdByUdi();
                        }
                    }

                    var zMedia = Umbraco.Media(z);
                    if (zMedia != null)
                    {
                        dto.Avatar = zMedia.Url;
                    }

                    var roles = Services.MemberService.GetAllRoles(x.Id);                        
                    dto.IsAdmin = roles.Any(r => r.Equals("Admin"));                    
                    
                    result.Add(dto);
                }


                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result);
                return response;
            }
            catch (Exception ex)
            {
                this.Logger.Error(this.GetType(), ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }            
        }
        static int exCount = 0;

    }

    [System.Runtime.Serialization.DataContract]
    public class MemberInfo
    {
        [System.Runtime.Serialization.DataMember]
        public int Id { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string Name { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string Url { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string Login { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string Email { get; set; }
        [System.Runtime.Serialization.DataMember]
        public DateTime CreateDate { get; set; }
        [System.Runtime.Serialization.DataMember]
        public DateTime UpdateDate { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string RawPasswordValue { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string ContentType { get; set; }
        [System.Runtime.Serialization.DataMember]
        public bool IsApproved { get; set; }
        [System.Runtime.Serialization.DataMember]
        public string Avatar { get; set; }
        [System.Runtime.Serialization.DataMember]
        public bool IsAdmin { get; set; }

        [System.Runtime.Serialization.DataMember]
        public Guid Key { get; set; }
    }


    public class LoungeFreeUserInfo
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsApproved { get; set; }
        public bool CreatedFromLounge { get; set; }
    }
    public class BasicMembersInfo
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsApproved { get; set; }
        public bool? BasicMemberShip { get; set; }
    }

    public class VirksomhedData
    {
        public string CompanyName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegisteredAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> EmailAddresses { get; set; }
        public List<string> Website { get; set; }
        public string CompanyForm { get; set; }
        public string MainBranch { get; set; }
        public int? NumberOfEmployees { get; set; }
    }

}