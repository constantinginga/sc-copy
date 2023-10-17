using StartupCentral.Code.Model;
using StartupCentral.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Security;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace StartupCentral.Code.Controllers
{
    public class ForretningsplanController
    {
        private IMember currentMember = null;

        private IContent currentForretningsplan = null;

        public ForretningsplanController()
        {
            UmbracoHelper helper = Umbraco.Web.Composing.Current.UmbracoHelper;
            MembershipHelper membershipHelper = helper.MembershipHelper;
            currentMember = ApplicationContext.Current.Services.MemberService.GetByUsername(membershipHelper.CurrentUserName);

            // https://our.umbraco.com/forum/using-umbraco-and-getting-started/100868-get-current-user 
            /*
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            var identity = wrapper.GetCurrentIdentity(true);
            currentMember = ApplicationContext.Current.Services.MemberService.GetByProviderKey(identity.Id);
            */
        }

        #region Forretningsplans Creation

        public List<FormToUmbraco> Properties { get; set; } = new List<FormToUmbraco>();

        public void Add(string formFieldName, string umbracoFieldName)
        {
            this.Properties.Add(new FormToUmbraco() { FormFieldName = formFieldName, UmbracoFieldName = umbracoFieldName });
        }

        public bool Save(HttpRequest httpRequest, string contentTypeAlias)
        {
            bool isDirty = false;

            if (httpRequest.QueryString["Id"] != null)
            {
                if (httpRequest.QueryString["id"].IsNumeric())
                {
                    currentForretningsplan = ApplicationContext.Current.Services.ContentService.GetById(Convert.ToInt32(httpRequest.QueryString["id"]));
                }
            }

            if (currentForretningsplan == null)
            {
                currentForretningsplan = GetOrCreateForretningsplan(contentTypeAlias);
            }

            string memberEmail = string.Empty;
            if (!string.IsNullOrEmpty(currentMember.Email))
            {
                memberEmail = currentMember.Email;
            }
            else if (currentMember.GetValue("wwemail") != null)
            {
                memberEmail = currentMember.GetValue<string>("wwemail");
            }

            if (currentForretningsplan != null)
            {
                foreach (var key in Properties)
                {
                    if (currentForretningsplan.HasProperty(key.UmbracoFieldName))
                    {
                        switch (currentForretningsplan.Properties[key.UmbracoFieldName].PropertyType.PropertyEditorAlias)
                        {
                            case "Umbraco.TrueFalse":
                                if (httpRequest[key.FormFieldName] != null)
                                {
                                    currentForretningsplan.SetValue(key.UmbracoFieldName, httpRequest[key.FormFieldName].ParseBoolean());
                                    isDirty = true;
                                }
                                else
                                {
                                    currentForretningsplan.SetValue(key.UmbracoFieldName, false);
                                    isDirty = true;
                                }

                                break;

                            default:
                                if (httpRequest[key.FormFieldName] != null)
                                {
                                    currentForretningsplan.SetValue(key.UmbracoFieldName, Convert.ToString(httpRequest[key.FormFieldName]));
                                    isDirty = true;
                                }
                                break;
                        }
                    }
                }

                // Handling uploads
                if (httpRequest.Files != null && httpRequest.Files.Count > 0)
                {
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
                        studentFolder.SetValue("memberId", currentMember.Id);
                        ApplicationContext.Current.Services.MediaService.Save(studentFolder);
                    }

                    if (studentFolder != null)
                    {
                        int index = 0;
                        foreach (var file in httpRequest.Files)
                        {
                            if (httpRequest.Files[index].ContentLength > 0)
                            {
                                FormToUmbraco formToUmbraco = this.Properties.Where(itm => itm.FormFieldName == httpRequest.Files.Keys[index].ToString()).FirstOrDefault();
                                if (formToUmbraco != null)
                                {
                                    // Only if a field in the forretningsplan exists will the file be saved.
                                    if (currentForretningsplan.HasProperty(formToUmbraco.UmbracoFieldName))
                                    {
                                        // Creating and saving file.
                                        IMedia mediaFile = ApplicationContext.Current.Services.MediaService.CreateMedia(httpRequest.Files[index].FileName, studentFolder, "File");
                                        System.IO.Stream s = httpRequest.Files[index].InputStream;
                                        mediaFile.SetValue(Umbraco.Core.Composing.Current.Services.ContentTypeBaseServices, "umbracoFile", httpRequest.Files[index].FileName, s); //TODO:Verify
                                        s.Close();
                                        ApplicationContext.Current.Services.MediaService.Save(mediaFile);

                                        // Updating file link on forretnings plan.
                                        currentForretningsplan.SetValue(formToUmbraco.UmbracoFieldName, mediaFile.Id);
                                        isDirty = true;
                                    }
                                }
                                index++;
                            }
                        }
                    }
                }

                if (isDirty)
                {
                    ApplicationContext.Current.Services.ContentService.Save(currentForretningsplan);
                }
            }

            return true;
        }

        public void Load(string contentTypeAlias)
        {
            if (HttpContext.Current.Request.QueryString["Id"] != null)
            {
                if (HttpContext.Current.Request.QueryString["id"].IsNumeric())
                {
                    currentForretningsplan = ApplicationContext.Current.Services.ContentService.GetById(Convert.ToInt32(HttpContext.Current.Request.QueryString["id"]));
                }
            }

            if (currentForretningsplan == null)
            {
                currentForretningsplan = GetOrCreateForretningsplan(contentTypeAlias);
            }
        }

        public IContent GetOrCreateForretningsplan(string contentTypeAlias)
        {
            IContent student = StudentsController.GetOrCreateStudent(currentMember);
            if (student != null)
            {
                //IContent forretningsplan = student.Children().Where(itm => itm.ContentType.Alias == contentTypeAlias && itm.GetValue<bool>("wwcompleted") != true).FirstOrDefault();
                IContent forretningsplan = ApplicationContext.Current.Services.ContentService.GetPagedChildren(student.Id, 0, int.MaxValue, out long totalRecords)
                    .Where(itm => itm.ContentType.Alias == contentTypeAlias && itm.GetValue<bool>("wwcompleted") != true).FirstOrDefault();
                if (forretningsplan == null)
                {
                    //forretningsplan = ApplicationContext.Current.Services.ContentService.CreateContentWithIdentity("Forretningsplan", student, contentTypeAlias);
                    // https://github.com/umbraco/Umbraco-CMS/issues/4452#issuecomment-505664486
                    forretningsplan = ApplicationContext.Current.Services.ContentService.CreateAndSave("Forretningsplan", student, contentTypeAlias);
                }

                return forretningsplan;
            }

            return null;
        }

        public string GetValue(string propertyName)
        {
            var key = Properties.Where(itm => itm.FormFieldName == propertyName).FirstOrDefault();
            if (key != null)
            {
                if (currentForretningsplan != null)
                {
                    if (currentForretningsplan.HasProperty(key.UmbracoFieldName))
                    {
                        return currentForretningsplan.GetValue<string>(key.UmbracoFieldName);
                    }

                    if (currentForretningsplan.HasProperty(propertyName))
                    {
                        return currentForretningsplan.GetValue<string>(propertyName);
                    }
                }
            }

            return string.Empty;
        }

        public string BuildAutoSaveScript(string planIdParam)
        {
            if (string.IsNullOrEmpty(planIdParam))
            {
                if (this.currentForretningsplan != null)
                    planIdParam = this.currentForretningsplan.Id.ToString();
            }

            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.AppendLine("<script>");

            List<FormToUmbracoWithValue> propertiesWithValue = new List<FormToUmbracoWithValue>();
            foreach (var field in this.Properties)
            {
                propertiesWithValue.Add(new FormToUmbracoWithValue() { FormFieldName = field.FormFieldName, UmbracoFieldName = field.UmbracoFieldName, Value = string.Empty });
            }

            output.AppendLine($" var formFields = { Newtonsoft.Json.JsonConvert.SerializeObject(propertiesWithValue) };");
            output.AppendLine($" var planId = {  Convert.ToInt32(planIdParam) };");
            output.AppendLine("</script>");
            output.AppendLine("<script src=\"/scripts/startupcentral.fp-autosave.js?v=4\"></script>");

            return output.ToString();
        }

        public string GetNextPageUrl(int currentPageId)
        {
            if (HttpContext.Current.Request.Form["noredirect"] == null)
            {
                IContent content = ApplicationContext.Current.Services.ContentService.GetById(currentPageId);
                if (content != null)
                {
                    //IEnumerable<IContent> children = content.Parent().Children().OrderBy(itm => itm.SortOrder);
                    var parent = ApplicationContext.Current.Services.ContentService.GetParent(content);
                    IEnumerable<IContent> children = ApplicationContext.Current.Services.ContentService.GetPagedChildren(parent.Id, 0, int.MaxValue, out long total).OrderBy(itm => itm.SortOrder);

                    if (children != null)
                    {
                        IContent nextContent = null;
                        bool found = false;
                        foreach (IContent page in children)
                        {
                            if (found == true)
                            {
                                nextContent = page;
                                found = false;
                            }

                            if (page.Id == currentPageId && nextContent == null)
                            {
                                found = true;
                            }
                        }

                        if (nextContent != null)
                        {
                            if (HttpContext.Current.Request.QueryString["Id"] != null)
                            {
                                if (HttpContext.Current.Request.QueryString["id"].IsNumeric())
                                {
                                    //return string.Concat(umbraco.library.NiceUrl(nextContent.Id), "?id=", HttpContext.Current.Request.QueryString["id"]);
                                    return string.Concat(Umbraco.Web.Composing.Current.UmbracoHelper.Content(nextContent.Id).Url, "?id=", HttpContext.Current.Request.QueryString["id"]);
                                }
                            }
                            //return umbraco.library.NiceUrl(nextContent.Id);
                            return Umbraco.Web.Composing.Current.UmbracoHelper.Content(nextContent.Id).Url;
                        }
                    }
                }
            }

            return string.Empty;
        }

        public int GetCompletionPercent()
        {
            if (HttpContext.Current.Request.QueryString["Id"] != null)
            {
                if (HttpContext.Current.Request.QueryString["id"].IsNumeric())
                {
                    currentForretningsplan = ApplicationContext.Current.Services.ContentService.GetById(Convert.ToInt32(HttpContext.Current.Request.QueryString["id"]));
                }
            }

            if (currentForretningsplan == null)
            {
                currentForretningsplan = GetOrCreateForretningsplan("forretningsplan");
            }

            if (currentForretningsplan != null)
            {
                int propertyCount = currentForretningsplan.Properties.Count - 1; // Completed is not to include.
                int fillinCount = 0;

                foreach (var property in currentForretningsplan.Properties)
                {
                    if (property.GetValue() != null && !string.IsNullOrEmpty(property.GetValue().ToString()))
                    {
                        fillinCount++;
                    }
                }

                if (fillinCount > 0)
                {
                    fillinCount--; // Completed is not to include.
                }

                if (fillinCount > 0)
                {
                    return Convert.ToInt32(Math.Round(Convert.ToDouble(fillinCount) / Convert.ToDouble(propertyCount) * 100));
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }

        public void FinalizeForretningsplan(HttpRequest httpRequest, string contentTypeAlias)
        {
            if (httpRequest.QueryString["Id"] != null)
            {
                if (httpRequest.QueryString["id"].IsNumeric())
                {
                    currentForretningsplan = ApplicationContext.Current.Services.ContentService.GetById(Convert.ToInt32(httpRequest.QueryString["id"]));
                }
            }

            if (currentForretningsplan == null)
            {
                currentForretningsplan = GetOrCreateForretningsplan(contentTypeAlias);
            }

            if (currentForretningsplan != null && !currentForretningsplan.GetValue<bool>("wwcompleted"))
            {
                currentForretningsplan.SetValue("wwcompleted", true);
                ApplicationContext.Current.Services.ContentService.SaveAndPublish(currentForretningsplan);
            }
        }

        public string GetMediaFilename(string value)
        {
            if (value != null)
            {
                try
                {
                    Umbraco.Web.UmbracoHelper umbracoHelper = Umbraco.Web.Composing.Current.UmbracoHelper;

                    if (value.IsNumeric())
                    {
                        return umbracoHelper.Media(Convert.ToInt32(value)).Name;
                    }
                    else if (value.Contains("umb://"))
                    {
                        int id = value.GetIdByUdi();
                        if (id > 0)
                        {
                            return umbracoHelper.Media(id).Name;
                        }
                    }
                }
                catch (System.Exception) { }
            }

            return string.Empty;
        }

        #endregion Forretningsplans Creation

        public List<Forretningsplan> GetFinalizedForretningsplaner()
        {
            List<Forretningsplan> outputList = null;
            var nodes = Helpers.Nodes.GetAllNodesByType(-1, "forretningsplan");
            if (nodes != null)
            {
                List<int> idlist = new List<int>();
                foreach (var n in nodes)
                {
                    if (n.GetProperty("wwcompleted") != null && n.GetProperty("wwcompleted").GetValue().ParseBoolean())
                    {
                        idlist.Add(n.Id);
                    }
                }

                if (idlist != null && idlist.Count() > 0)
                {
                    outputList = new List<Forretningsplan>();
                    var list = ApplicationContext.Current.Services.ContentService.GetByIds(idlist);
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            outputList.Add(new Forretningsplan(item, currentMember));
                        }
                    }
                }
            }

            return outputList.Where(itm => itm.Member != null).ToList();
        }

        public List<Forretningsplan> GetMyFinalizedForretningsplaner()
        {
            List<Forretningsplan> outputList = null;
            var nodes = Helpers.Nodes.GetAllNodesByType(-1, "forretningsplan");
            if (nodes != null)
            {
                List<int> idlist = new List<int>();
                foreach (var n in nodes)
                {
                    if (n.GetProperty("wwcompleted") != null && n.GetProperty("wwcompleted").GetValue().ParseBoolean())
                    {
                        idlist.Add(n.Id);
                    }
                }

                if (idlist != null && idlist.Count() > 0)
                {
                    var list = ApplicationContext.Current.Services.ContentService.GetByIds(idlist);
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var parent = ApplicationContext.Current.Services.ContentService.GetParent(item);

                            if (parent != null && parent.GetValue("memberId") != null && parent.GetValue<int>("memberId") == currentMember.Id)
                            {
                                if (outputList == null) outputList = new List<Forretningsplan>();
                                outputList.Add(new Forretningsplan(item, currentMember));
                            }
                        }
                    }
                }
            }

            return outputList.Where(itm => itm.Member != null).ToList();
        }

        public Forretningsplan GetForretningsPlan(int id)
        {
            return new Forretningsplan(ApplicationContext.Current.Services.ContentService.GetById(id), currentMember);
        }

        public List<Database.ScForretningsplan> GetNotifications(int documentId)
        {
            return Database.ScForretningsplanRepository.GetListByDocumentId(documentId);
        }

        public bool AnyMessages(IMember member)
        {
            List<StartupCentral.Code.Model.Forretningsplan> myForretningsplaner = GetMyFinalizedForretningsplaner();
            if (myForretningsplaner != null)
            {
                foreach (var forretningsplan in myForretningsplaner)
                {
                    // Forespørger efter data/visninger/downloads om forretningsplanen.
                    var status = GetNotifications(forretningsplan.Content.Id);
                    if (status != null)
                    {
                        if (status.Count > 0) return true;
                    }
                }
            }

            return false;
        }
    }
}