using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace StartupCentral.Code.Controllers
{
    public static class StudentsController
    {
        /// <summary>
        /// Gets the root
        /// </summary>
        /// <returns></returns>
        public static IContent GetRootFolder()
        {
            var node = StartupCentral.Helpers.Nodes.GetFirstNodeByType(-1, "studenter");
            if (node != null)
            {
                return ApplicationContext.Current.Services.ContentService.GetById(node.Id);
            }

            return null;
        }

        public static IContent GetOrCreateStudent(IMember member)
        {
            if (member != null)
            {
                IContent studentsRoot = GetRootFolder();
                if (studentsRoot != null)
                {
                    //IContent student = studentsRoot.Children().FirstOrDefault(itm => itm.Name == member.Email.ToLower());
                    IContent student = ApplicationContext.Current.Services.ContentService.GetPagedChildren(studentsRoot.Id, 0, int.MaxValue, out long total)
                        .FirstOrDefault(itm => itm.Name == member.Email.ToLower());
                    if (student == null)
                    {
                        // student = ApplicationContext.Current.Services.ContentService.CreateContentWithIdentity(member.Email, studentsRoot, "enkeltstudent");
                        student = ApplicationContext.Current.Services.ContentService.CreateAndSave(member.Email, studentsRoot, "enkeltstudent");
                        if (student != null)
                        {
                            student.SetValue("memberid", member.Id);
                            // ApplicationContext.Current.Services.ContentService.SaveAndPublishWithStatus(student);
                            ApplicationContext.Current.Services.ContentService.SaveAndPublish(student);
                        }
                    }

                    return student;
                }
            }

            return null;
        }
    }
}