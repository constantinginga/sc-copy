using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace StartupCentral.Extensions

{
    public static class UdiExtensions
    {
        public static GuidUdi GetUdiById(this int id)
        {
            var node = ApplicationContext.Current.Services.ContentService.GetById(id);
            return node?.GetUdi();
        }

        public static int GetIdByUdi(this string udi)
        {
            // throw new Exception("//TODO: Handle missing context in src code");

            //https://our.umbraco.com/forum/umbraco-8/97891-how-to-get-content-or-media-by-udi-from-iumbracocontextfactory
            var udiAsString = udi.ToString();
            var guid = Guid.ParseExact(udiAsString.Substring(udiAsString.Length - 32), "N");

            if (udiAsString.StartsWith(@"umb://media/"))
            {
                var mediaNode = Umbraco.Web.Composing.Current.UmbracoContext.MediaCache.GetById(guid);
                if (mediaNode == null)
                    mediaNode = Umbraco.Web.Composing.Current.UmbracoContext.Media.GetById(guid);
                if (mediaNode == null) 
                    return 0;
                
                return mediaNode.Id;
            }

            var node = Umbraco.Web.Composing.Current.UmbracoContext.ContentCache.GetById(guid);
            if (node == null)
                node = Umbraco.Web.Composing.Current.UmbracoContext.Content.GetById(guid);
            if (node == null) 
                return 0;

            return node.Id;
        }

        public static List<int> UdiCSVToIntList(this string udis)
        {
            List<int> resultList = null;
            if (string.IsNullOrEmpty(udis)) return null;

            if (udis.Contains(","))
            {
                if (resultList == null) resultList = new List<int>();
                foreach (string udi in udis.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrEmpty(udi) && udi.ToLower().StartsWith("umb://"))
                    {
                        resultList.Add(udi.GetIdByUdi());
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(udis) && udis.ToLower().StartsWith("umb://"))
                {
                    if (resultList == null) resultList = new List<int>();
                    resultList.Add(udis.GetIdByUdi());
                }
            }

            return resultList;
        }

        public static string IntListToUdiString(List<int> list)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            foreach (int item in list)
            {
                if (output.ToString() == string.Empty)
                {
                    output.Append(item.GetUdiById());
                }
                else
                {
                    output.Append(string.Concat(",", item.GetUdiById()));
                }
            }

            return output.ToString();
        }
    }
}