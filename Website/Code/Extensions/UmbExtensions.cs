using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace StartupCentral.Extensions

{
    public static partial class StringExtensions
    {
        public static List<int> UmbToFileIds(this string value)
        {
            List<int> resList = null;
            if (!string.IsNullOrEmpty(value))
            {
                string[] items = value.ToString().Split(',');
                if (items.Length > 0)
                {
                    resList = new List<int>();

                    foreach (string itm in items)
                    {
                        try
                        {
                            GuidUdi imageGuidUdi = GuidUdi.Parse(itm);
                            // Get the ID of the node!
                            var imageNode = ApplicationContext.Current.Services.EntityService.Get(imageGuidUdi.Guid, (UmbracoObjectTypes)Enum.Parse(typeof(UmbracoObjectTypes), imageGuidUdi.EntityType, true));
                            var imageNodeId = imageNode.Id;
                            // Finally, get the node.
                            //resList.Add(Convert.ToInt32(imageNodeId.Result));
                            resList.Add(imageNodeId);
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                    }
                }
            }

            return resList;
        }

        public static IPublishedContent UmbToSingleImageUrl(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            UmbracoHelper helper = Umbraco.Web.Composing.Current.UmbracoHelper;

            if (value.IsNumeric())
            {
                return helper.Media(Convert.ToInt32(value));
            }

            List<int> list = UmbToFileIds(value);
            if (list != null)
            {
                return helper.Media(list.First());
            }

            return null;
        }
    }
}