using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace StartupCentral.Member_Pipelining
{
    // https://our.umbraco.com/Documentation/Reference/Routing/Request-Pipeline/IContentFinder-v8#adding-and-removing-icontentfinders
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class UpdateContentFindersComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // Insert my MemberProfileContentFinder before ContentFinderByNiceUrl
            composition.ContentFinders().InsertBefore<ContentFinderByUrl, MemberProfileContentFinder>();
        }
    }
}