using StartupCentral.Code.Class;
using StartupCentral.Code.Controllers;
using StartupCentral.Code.Model;
using StartupCentral.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;
using Core = Umbraco.Core;
using Umbraco.Core;
using System.Data.Entity.Validation;
using System.Linq;
using Umbraco.Web;
using StartupCentral.DAL.EntityModels;

namespace StartupCentral.Code.ApiControllers
{
    public class ForretningsplanApiController : UmbracoApiController
    {
        readonly ForretningsPlanService forretningsPlanService;
        public List<FormToUmbraco> Properties { get; set; } = new List<FormToUmbraco>();

        public bool IsReusable => throw new NotImplementedException();

        public ForretningsplanApiController()
        {
            forretningsPlanService = new ForretningsPlanService();
        }

        #region new implementation for the business plan 27/10/2020
        private IMember GetMember() => TokenValidator.GetCurrentMember(HttpContext.Current.Request);

        [HttpPost]
        public async Task<ApiResponse> CreateOrUpdateBusinessPlan(BusinessPlan model)
        {
            try
            {
                IMember member = GetMember();

                if (member != null)
                {
                    //create
                    if(model.Id == Guid.Empty)
                    {
                        Guid planId = await forretningsPlanService.Create(member.Key, model);
                        LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"ForretninsPlan ok");

                        return new ApiResponse(true, string.Format("{0} plan created.", model.ProjektNavn), planId);
                    }
                    //update
                    else
                    {
                        Guid planId = await forretningsPlanService.Update(member.Key, model);
                        return new ApiResponse(true, string.Format("{0} plan updated.", model.ProjektNavn), planId);
                    }                   
                }
                else
                {
                    return new ApiResponse(false, $"User not found - bsp cannot be created.");
                }
            }
            catch (DbEntityValidationException ex)
            {
                string err = string.Empty;
                foreach (var eve in ex.EntityValidationErrors)
                {
                    err = err + string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err = err + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }

                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Businessplan encountered an error {err}");               
                return new ApiResponse(false, $"Businessplan encountered an error {err}");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"Businessplan encountered an error {e.Message}");
            }         
        }

        [HttpGet]
        public async Task<ApiResponse> getBusinessPlanById(string sBusId)
        {
            IMember member = GetMember();

            if (member != null)
            {
                if (member.ContentTypeAlias == "coach1")
                {
                    var res = await forretningsPlanService.GetBusinessPlanByIdCoach(new Guid(sBusId));

                    if (res.Count() > 0)
                    {
                        userForretningsPlan plan = res[0];
                        return new ApiResponse(true, "Business plan retrived.", plan);
                    }                  
                }
                else if(member.ContentTypeAlias == "student")
                {
                    var res = await forretningsPlanService.GetBusinessPlanById(member.Key, new Guid(sBusId));

                    if (res.Count() > 0)
                    {
                        userForretningsPlan plan = res[0];
                        return new ApiResponse(true, "Business plan retrived.", plan);
                    }
                }
              
                return new ApiResponse(false, "Dusiness plan not found.");              
            }

            return new ApiResponse(false, "User doesnt have access to this business plan.");
        }

        public userForretningsPlan getBusinessPlanPDF(Guid sBusId, Guid memberKey)
        {
            var res = forretningsPlanService.GetBusinessPlanByIdPDF(memberKey, sBusId);
            //var res = await forretningsPlanService.GetBusinessPlanById(memberKey, sBusId);

            if (res.Count() > 0)
            {
                userForretningsPlan plan = res[0];
                return plan;
            }
               
            return null;
        }

        [HttpGet]
        public async Task<ApiResponse> getDrafts()
        {
            IMember member = GetMember();

            if (member != null)
            {
                var listOfDrafts = await forretningsPlanService.GetListOfDraftBusinessPlans(member.Key);
                return new ApiResponse(true, "List of draft plans retrived.", listOfDrafts);
            }

            return new ApiResponse(false, "User doesnt have access to this list.");

        } 

        [HttpGet]
        public async Task<ApiResponse> getCompleted()
        {
            IMember member = GetMember();

            if (member != null)
            {
                var listOfCompleted = await forretningsPlanService.GetListOfCompletedBusinessPlans(member.Key);
                return new ApiResponse(true, "List of completed plans retrived.", listOfCompleted);
            }

            return new ApiResponse(false, "User doesnt have access to this list.");
             
        }

        [HttpDelete]
        public async Task<ApiResponse> delete(Guid id)
        {
            IMember member = GetMember();

            if (member != null)
            {
                //Guid gid = new Guid(id);
                var res = await forretningsPlanService.Delete(id);

                if(res)
                    return new ApiResponse(true, "Business plan deleted.");
                else
                    return new ApiResponse(false, "Business plan couldn't be deleted.");
            }

            return new ApiResponse(false, "Business plan couldn't be deleted.");
        }

        #endregion




        [HttpPost]
        public ApiResponse AutoSave(string values)
        {
            bool isDirty = false;
            values = HttpContext.Current.Request.Form[0];
            List<FormToUmbracoWithValue> list = null;
            IContent currentForretningsplan = null;

            try
            {
                // New instance of the ForretningsplanController
                ForretningsplanController forretningsplanController = new ForretningsplanController();

                // Deserialize of fields and values
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FormToUmbracoWithValue>>(values);

                // Load of the current forretningsplan
                if (HttpContext.Current.Request.QueryString["Id"] != null)
                {
                    if (HttpContext.Current.Request.QueryString["id"].IsNumeric())
                    {
                        currentForretningsplan = Services.ContentService.GetById(Convert.ToInt32(HttpContext.Current.Request.QueryString["id"]));
                    }
                }

                if (currentForretningsplan == null)
                {
                    currentForretningsplan = forretningsplanController.GetOrCreateForretningsplan("forretningsplan");
                }

                if (list != null)
                {
                    foreach (var key in list)
                    {
                        if (currentForretningsplan.HasProperty(key.UmbracoFieldName))
                        {
                            switch (currentForretningsplan.Properties[key.UmbracoFieldName].PropertyType.PropertyEditorAlias)
                            {
                                case "Umbraco.MediaPicker2":
                                    break;

                                case "Umbraco.TrueFalse":
                                    if (key.Value != null)
                                    {
                                        currentForretningsplan.SetValue(key.UmbracoFieldName, key.Value.ParseBoolean());
                                        isDirty = true;
                                    }
                                    else
                                    {
                                        currentForretningsplan.SetValue(key.UmbracoFieldName, false);
                                        isDirty = true;
                                    }

                                    break;

                                default:
                                    if (key.Value != null)
                                    {
                                        currentForretningsplan.SetValue(key.UmbracoFieldName, Convert.ToString(key.Value));
                                        isDirty = true;
                                    }
                                    break;
                            }
                        }
                    }

                    if (isDirty)
                    {
                        // Saves the changes just applied.
                        Services.ContentService.SaveAndPublish(currentForretningsplan);
                        return new ApiResponse() { Success = true, Message = "OK", Data = list };
                    }
                }
            }
            catch (System.Exception ex)
            {
                return new ApiResponse() { Success = false, Message = ex.ToString(), Data = values };
            }

            return new ApiResponse() { Success = false, Message = "Some didn't succeed based on the values/data provided.", Data = values };
        }

        [HttpGet]
        public ApiResponse SetDownloaded(int documentId)
        {
            try
            {
                IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
                if (member != null)
                {
                    if (member.ContentTypeAlias == "investor")
                    {
                        Database.ScForretningsplan scForretningsplan = Database.ScForretningsplanRepository.GetByDocumentIdAndMemberId(documentId, member.Id);
                        if (scForretningsplan != null)
                        {
                            scForretningsplan.Downloaded = true;

                            if (Database.ScForretningsplanRepository.Save(scForretningsplan) != null)
                            {
                                return new ApiResponse(true, "OK");
                            }
                        }
                    }
                    else
                    {
                        return new ApiResponse(false, "Only membertype 'investor' allowed.");
                    }
                }

                return new ApiResponse(false, "Something went wrong?!");
            }
            catch (System.Exception ex)
            {
                return new ApiResponse(false, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public ApiResponse UploadFile()
        {
            var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            if (file != null && file.ContentLength > 0)
            {
                IMember member = GetMember();
                var memberEmail = member.Email;

                int studentMediaRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Student.RootFolder.Id"]);

                IMedia studentFolder = null;
                IMedia mediaRoot = ApplicationContext.Current.Services.MediaService.GetById(studentMediaRootFolderId);

                if (mediaRoot != null)
                {
                    // studentFolder = mediaRoot.Children().Where(itm => itm.Name == memberEmail).FirstOrDefault();
                    // https://our.umbraco.com/forum/using-umbraco-and-getting-started/101941-get-imedia-children-in-umbraco-8
                    var children = ApplicationContext.Current.Services.MediaService.GetPagedChildren(studentMediaRootFolderId, 0, int.MaxValue, out long total);
                    studentFolder = children.Where(itm => itm.Name == memberEmail).FirstOrDefault();
                }

                if (studentFolder == null)
                {
                    studentFolder = ApplicationContext.Current.Services.MediaService.CreateMedia(memberEmail, studentMediaRootFolderId, "Folder");
                    studentFolder.SetValue("memberId", member.Id);
                    ApplicationContext.Current.Services.MediaService.Save(studentFolder);
                }

                if (studentFolder != null)
                {             
                    // Creating and saving file.
                    IMedia mediaFile = ApplicationContext.Current.Services.MediaService.CreateMedia(file.FileName, studentFolder, "File");
                    System.IO.Stream s = file.InputStream; 
                    // mediaFile.SetValue("umbracoFile", file.FileName, s);
                    mediaFile.SetValue(Core.Composing.Current.Services.ContentTypeBaseServices, "umbracoFile", file.FileName, s); //TODO:Verify
                    s.Close();                    
                    ApplicationContext.Current.Services.MediaService.Save(mediaFile);

                    //var fileUrl = Umbraco.TypedMedia(mediaFile.Id).Url();
                    var fileUrl = Umbraco.Media(mediaFile.Id).Url(); //TODO:Verify

                    // Updating file link on forretnings plan.
                    return new ApiResponse(true, "File uploaded.", fileUrl) ; //mediaFile.GetUdi().ToString());
                }
                else
                {
                    return new ApiResponse(false, "Folder coudn't be created.");
                }
            }
            else
            {
                return new ApiResponse(false, "Please select a file.");
            }
        }
    }
}