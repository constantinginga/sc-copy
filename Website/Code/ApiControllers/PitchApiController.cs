using StartupCentral.Code.Class;
using StartupCentral.Code.Controllers;
using StartupCentral.Code.Model;
using StartupCentral.DAL.EntityModels;
using StartupCentral.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Core = Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;
using Umbraco.Core;
using System.IO;
using System.Text;

namespace StartupCentral.Code.ApiControllers
{
    public class PitchApiController : UmbracoApiController
    {

        readonly UserPitchDeckService pitchService;
        public List<FormToUmbraco> Properties { get; set; } = new List<FormToUmbraco>();

        public bool IsReusable => throw new NotImplementedException();

        public PitchApiController()
        {
            pitchService = new UserPitchDeckService();
        }


        private IMember GetMember() => TokenValidator.GetCurrentMember(HttpContext.Current.Request);

        [HttpPost]
        public async Task<ApiResponse> CreateOrUpdatePitch(PitchDeck model)
        {
            try
            {
                IMember member = GetMember();

                if (member != null)
                {
                    //create
                    if (model.Id == Guid.Empty)
                    {
                        Guid pitchId = await pitchService.Create(member.Key, model);
                        LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Pitch deck ok");

                        return new ApiResponse(true, string.Format("{0} pitch created.", model.PitchName), pitchId);
                    }
                    //update
                    else
                    {
                        Guid pitchId = await pitchService.Update(member.Key, model);
                        return new ApiResponse(true, string.Format("{0} pitch updated.", model.PitchName), pitchId);
                    }
                }
                else
                {
                    return new ApiResponse(false, $"User not found - pitch cannot be created.");
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

                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Pitch deck encountered an error {err}");
                return new ApiResponse(false, $"Pitch deck encountered an error {err}");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"Pitch deck encountered an error {e.Message}");
            }
        }

        [HttpGet]
        public async Task<ApiResponse> getPitchById(string sBusId)
        {
            IMember member = GetMember();

            if (member != null)
            {
                if (member.ContentTypeAlias == "coach1")
                {
                    var res = await pitchService.GetPitchByIdCoach(new Guid(sBusId));

                    if (res.Count() > 0)
                    {
                        userPitch pitch = res[0];
                        return new ApiResponse(true, "Pitch retrived.", pitch);
                    }
                }
                else if (member.ContentTypeAlias == "student")
                {
                    var res = await pitchService.GetPitchById(member.Key, new Guid(sBusId));

                    if (res.Count() > 0)
                    {
                        userPitch pitch = res[0];
                        return new ApiResponse(true, "Pitch retrived.", pitch);
                    }
                }

                return new ApiResponse(false, "Pitch not found.");
            }

            return new ApiResponse(false, "User doesnt have access to this Pitch.");
        }

        public userPitch getPitchPDF(Guid sBusId, Guid memberKey)
        {
            var res = pitchService.GetPitchByIdPDF(memberKey, sBusId);
            //var res = await forretningsPlanService.GetBusinessPlanById(memberKey, sBusId);

            if (res.Count() > 0)
            {
                userPitch pitch = res[0];
                return pitch;
            }

            return null;
        }


        [HttpGet]
        public HttpResponseMessage GetPdf()
        {
            var html = System.IO.File.ReadAllText(@"C:/github/StartupCentral18/SCUmbraco/Website/bin/Lars.html");
            var css = System.IO.File.ReadAllText(@"C:\github\StartupCentral18\SCUmbraco\Website\css\V4 Winter 2021\sc-v4-main.css");

            html.Replace(" </head>", "<style>" + css + "</style></head>");


            Byte[] res = null;
            MemoryStream ms = new MemoryStream();
            { 
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();

                System.IO.File.WriteAllBytes(@"C:/github/StartupCentral18/SCUmbraco/Website/bin/Lars.pdf", res);
            }
            //Response.ContentType("")
            //return res;            
            // https://stackoverflow.com/a/23381647/1156301
            var result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(ms);
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            return result;
        }


        [HttpGet]
        public async Task<ApiResponse> getDrafts()
        {
            IMember member = GetMember();

            if (member != null)
            {
                var listOfDrafts = await pitchService.GetListOfDraftPitchs(member.Key);
                return new ApiResponse(true, "List of draft pitchs retrived.", listOfDrafts);
            }

            return new ApiResponse(false, "User doesnt have access to this list.");

        }

        [HttpGet]
        public async Task<ApiResponse> getCompleted()
        {
            IMember member = GetMember();

            if (member != null)
            {
                var listOfCompleted = await pitchService.GetListOfCompletedPitchs(member.Key);
                return new ApiResponse(true, "List of completed pitchs retrived.", listOfCompleted);
            }

            return new ApiResponse(false, "User doesnt have access to this list.");

        }

        [HttpDelete]
        public async Task<ApiResponse> deleteWithFile(Guid id, string filePath)
        {
            IMember member = GetMember();

            if (member != null)
            {
                //Guid gid = new Guid(id);
                var res = await pitchService.DeleteWithFile(id, filePath);


                if (res)
                    return new ApiResponse(true, "Picth deleted.");
                else
                    return new ApiResponse(false, "Pitch couldn't be deleted.");
            }

            return new ApiResponse(false, "Picth couldn't be deleted.");
        }


        [HttpDelete]
        public async Task<ApiResponse> delete(Guid id)
        {
            IMember member = GetMember();

            if (member != null)
            {
                //Guid gid = new Guid(id);
                var res = await pitchService.Delete(id);


                if (res)
                    return new ApiResponse(true, "Picth deleted.");
                else
                    return new ApiResponse(false, "Pitch couldn't be deleted.");
            }

            return new ApiResponse(false, "Picth couldn't be deleted.");
        }





        [HttpPost]
        public ApiResponse AutoSave(string values)
        {
            bool isDirty = false;
            values = HttpContext.Current.Request.Form[0];
            List<FormToUmbracoWithValue> list = null;
            IContent currentPitch = null;

            try
            {
                // New instance of the PitchController
                PitchController pitchController = new PitchController();

                // Deserialize of fields and values
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FormToUmbracoWithValue>>(values);

                // Load of the current pitch
                if (HttpContext.Current.Request.QueryString["Id"] != null)
                {
                    if (HttpContext.Current.Request.QueryString["id"].IsNumeric())
                    {
                        currentPitch = Services.ContentService.GetById(Convert.ToInt32(HttpContext.Current.Request.QueryString["id"]));
                    }
                }

                if (currentPitch == null)
                {
                    currentPitch = pitchController.GetOrCreatePitch("pitch");
                }

                if (list != null)
                {
                    foreach (var key in list)
                    {
                        if (currentPitch.HasProperty(key.UmbracoFieldName))
                        {
                            switch (currentPitch.Properties[key.UmbracoFieldName].PropertyType.PropertyEditorAlias)
                            {
                                case "Umbraco.MediaPicker2":
                                    break;

                                case "Umbraco.TrueFalse":
                                    if (key.Value != null)
                                    {
                                        currentPitch.SetValue(key.UmbracoFieldName, key.Value.ParseBoolean());
                                        isDirty = true;
                                    }
                                    else
                                    {
                                        currentPitch.SetValue(key.UmbracoFieldName, false);
                                        isDirty = true;
                                    }

                                    break;

                                default:
                                    if (key.Value != null)
                                    {
                                        currentPitch.SetValue(key.UmbracoFieldName, Convert.ToString(key.Value));
                                        isDirty = true;
                                    }
                                    break;
                            }
                        }
                    }

                    if (isDirty)
                    {
                        // Saves the changes just applied.
                        Services.ContentService.SaveAndPublish(currentPitch);
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

                return new ApiResponse(false, "Method not implemented yet");

        }

        [HttpPost]
        public ApiResponse UploadPicture()
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
                    return new ApiResponse(true, "File uploaded.", fileUrl); //mediaFile.GetUdi().ToString());
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