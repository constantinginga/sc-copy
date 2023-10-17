using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using umbraco.NodeFactory;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace StartupCentral.Helpers
{
    public static class Nodes
    {
        #region --- Nodes ---
        /*
        public static int GetSiteId(int nodeId, string rootAlias = "")
        {
            if (nodeId > 0)
            {
                IContent content = ApplicationContext.Current.Services.ContentService.GetById(nodeId);
                int siteId = nodeId;

                if (content != null)
                {
                    if (!string.IsNullOrEmpty(rootAlias))
                    {
                        if (content.ContentType != null)
                        {
                            if (content.ContentType.Alias == rootAlias)
                            {
                                return content.Id;
                            }
                        }

                        content = FindPreviousCloudSiteNode(content.Parent(), rootAlias);
                        if (content != null)
                        {
                            siteId = content.Id;
                        }
                    }
                    else
                    {
                        if (content.Ancestors().Count() > 0)
                        {
                            siteId = content.Ancestors().First().Id;
                        }
                    }

                    return siteId;
                }
            }
            return -1;
        }
        */

        /*
        internal static IContent FindPreviousCloudSiteNode(IContent currentNode, string rootAlias)
        {
            if (currentNode != null)
            {
                if (currentNode.ContentType != null)
                {
                    if (currentNode.ContentType.Alias == rootAlias) return currentNode;
                }
            }
            else
            {
                return null;
            }

            if (currentNode.Parent() != null)
            {
                return FindPreviousCloudSiteNode(currentNode.Parent(), rootAlias);
            }
            else
            {
                return null;
            }
        }
        */

        /*
        public static int GetMediaRootId(int memberId)
        {
            if (memberId > 0)
            {
                IMember member = ApplicationContext.Current.Services.MemberService.GetById(memberId);
                if (member != null)
                {
                    return member.Id;
                }
            }

            return -1;
        }

        public static IPublishedContent GetNodeByAlias(string alias, int siteId)
        {
            var umbracoHelper = new Umbraco.Web.UmbracoHelper(UmbracoContext.Current);
            var contentNode = umbracoHelper.TypedContentAtXPath(String.Format("//{0}", alias)).Where(itm => itm.Ancestors().First().Id == siteId).First();

            return contentNode;
        }
        
        public static IPublishedContent GetNodeByAlias(string alias)
        {
            var umbracoHelper = new Umbraco.Web.UmbracoHelper(UmbracoContext.Current);
            var contentNode = umbracoHelper.TypedContentSingleAtXPath(String.Format("//{0}", alias));

            return contentNode;
        }
        */

        /*
        public static Node GetNodeById(int nodeId)
        {
            return new Node(nodeId);
        }
        */

        /*
        private static IPublishedContent GetFirstNodeByType(IPublishedContent contentNode, string typeName)
        {
            foreach(var node in contentNode.Children())
            {
                if (node.ContentType.Alias == typeName) 
                    return node;

                if (node.Children().Count() > 0)
                {
                    var found = GetFirstNodeByType(node, typeName);
                    if (found != null) return found;
                }
            }

            return null;
        } */

        /// <summary>
        /// Get all nodes by type, put -1 to start at the root of the website
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static IPublishedContent GetFirstNodeByType(int nodeId, string typeName)
        {
            var umbracoContextFactory = DependencyResolver.Current.GetService<IUmbracoContextFactory>();
            var umbracoContextAccessor = DependencyResolver.Current.GetService<IUmbracoContextAccessor>();

            using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
            {
                var nodes = new List<IPublishedContent>();
                var nodeIdNode = umbracoContextAccessor.UmbracoContext.Content.GetById(nodeId);
                nodes.Add(nodeIdNode);
                if (nodeIdNode == null && nodeId == -1)
                {
                    // https://our.umbraco.com/forum/umbraco-8/98702-get-root-node
                    nodes = umbracoContextAccessor.UmbracoContext.Content.GetAtRoot().ToList();
                }

                foreach (var node in nodes)
                {
                    var found = node.DescendantOfType(typeName);
                    if (found != null)
                        return found;
                }
                return null;
            }
        }


        /*        
            var node = Umbraco.Web.Composing.Current.UmbracoHelper.Content(nodeId);

            var firstTypeNode = node.Descendants().Where(x => x.IsDocumentType(typeName)).SingleOrDefault();

            return firstTypeNode;           

            return node.DescendantOfType(typeName);
            /*
            List<Node> nodes = new List<Node>();
            var currentNode = new Node(nodeId);
            foreach (Node node in currentNode.Children)
            {
                if (node.NodeTypeAlias == typeName)
                {
                    return node;
                }

                if (node.Children.Count > 0)
                {
                    Node found = GetFirstNodeByType(node, typeName);
                    if (found != null) return found;
                }
            }

            return null;
            */

        /// <summary>
        /// Recursive method to get the node from the child passed in as a parameter
        /// </summary>
        /// <param name="node"></param>
        /// <param name="typeName"></param>
        private static IPublishedContent GetFirstNodeByType(IPublishedContent node, string typeName)
        {
            return node.DescendantOfType(typeName);
            /*
            foreach (Node childNode in node.Children)
            {
                if (childNode.NodeTypeAlias == typeName)
                {
                    return childNode;
                }

                if (childNode.Children.Count > 0)
                {
                    Node found = GetFirstNodeByType(childNode, typeName);
                    if (found != null) return found;
                }
            }

            return null;
            */
        }

        /// <summary>
        /// Get all nodes by type, put -1 to start at the root of the website
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static List<IPublishedContent> GetAllNodesByType(int nodeId, string typeName)
        {
            if (nodeId == -1)
                System.Diagnostics.Debugger.Break();
            var node = Umbraco.Web.Composing.Current.UmbracoHelper.Content(nodeId);
            return node.DescendantsOfType(typeName).ToList();
            /*
            List<Node> nodes = new List<Node>();
            var currentNode = new Node(nodeId);
            foreach (Node node in currentNode.Children)
            {
                if (node.NodeTypeAlias == typeName)
                {
                    nodes.Add(node);
                }

                if (node.Children.Count > 0)
                    GetAllNodesByType(ref nodes, node, typeName);
            }

            return nodes.ToList();
            */
        }

        /*
        /// <summary>
        /// Recursive method to get the nodes from the child passed in as a parameter
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="node"></param>
        /// <param name="typeName"></param>
        private static void GetAllNodesByType(ref List<Node> nodeList, Node node, string typeName)
        {          
            foreach (Node childNode in node.Children)
            {
                if (childNode.NodeTypeAlias == typeName)
                {
                    nodeList.Add(childNode);
                }

                if (childNode.Children.Count > 0)
                    GetAllNodesByType(ref nodeList, childNode, typeName);
            }
        }
        */

        #endregion --- Nodes ---
    }
}