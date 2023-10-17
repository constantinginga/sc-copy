using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using umbraco.cms.businesslogic.member;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Web.Routing;

namespace StartupCentral.Member_Pipelining
{
    public class MemberProfileContentFinder : IContentFinder
    {
        public bool TryFindContent(PublishedRequest contentRequest)
        {
            var urlParts = contentRequest.Uri.GetAbsolutePathDecoded().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Manual handling of /en/coaches/Name
            if (urlParts.Length == 3 && urlParts[0].ToLowerInvariant() == "en")
            {
                urlParts = urlParts.Skip(1).ToArray();
            }

            //Check if the Url Parts
            // Starts with /member/*
            //if (urlParts.Length > 1 && urlParts[0].ToLower() == "member") 
                if (urlParts.Length > 1 && ( false
                    || urlParts[0].ToLower() == "coaches" 
                    || urlParts[0].ToLower() == "partnere"
                    || urlParts[0].ToLower() == "partners"
                    ))
            {
                //Lets try & find the member
                var memberName = urlParts[1];

                //Try and find a member where the property matches the memberName
                List<IMember> tryFindMember = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue("urlProfile", memberName).ToList();

                if (!tryFindMember.Any()) // try a partial match just in case
                {
                    tryFindMember = ApplicationContext.Current.Services.MemberService.GetMembersByPropertyValue("urlProfile", memberName, StringPropertyMatchType.Contains).ToList();
                }

                //See if tryFindMember is not null
                 if (tryFindMember.Any())
                {
                    //Need to set the member ID or pass member object to published content
                    HttpContext.Current.Items["memberProfile"] = tryFindMember.FirstOrDefault();

                    //Add in string to items - for profile name user was looking for
                    HttpContext.Current.Items["memberName"] = memberName;

                    //Set the Published Content Node to be the /Profile node - can get properties off it & my member profile in the view
                    contentRequest.PublishedContent = contentRequest.UmbracoContext.ContentCache.GetByRoute("/MemberProfile");
                }

                //Return true to say found something & stop pipeline & other contentFinder's from running
                return true;
            }

            //Not found any content node to display/match - so run next ContentFinder in Pipeline
            return false;
        }
    }
}