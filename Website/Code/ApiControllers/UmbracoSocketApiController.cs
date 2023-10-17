using Microsoft.IdentityModel.Logging;
using Microsoft.Web.WebSockets;
using StartupCentral.Code.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Umbraco.Web.WebApi;
using Umbraco.Core.Logging;
//using LogHelper = Umbraco.Core.Logging.LogHelper;

namespace StartupCentral.Code.ApiControllers
{
    public abstract class UmbracoSocketApiController : UmbracoApiController
    {

        public abstract Class.LiveConnections.ChannelsType ChannelType { get; set; }


        [AllowAnonymous]
        public HttpResponseMessage Get(string memberKey)
        {
            LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Get");

            try
            {
                HttpContext.Current.AcceptWebSocketRequest(new SocketChannel(
                    this.ChannelType,
                    new Guid(memberKey)
                ));
            }
            catch (System.Exception ex)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ex.ToString());

            }
            finally
            {

            }

            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        #region WebSocket

        //[HttpGet]
        //public ApiResponse SubscribeTo(Guid memberKey)
        //{

        //    try
        //    {
        //        if (LiveConnections.Channels != null)
        //        {
        //            if (LiveConnections.Channels.ContainsKey(this.ChannelType))
        //            {
        //                var item = LiveConnections.Channels[this.ChannelType]?.Where(itm => ((SocketChannel)itm).MemberKey == memberKey).FirstOrDefault();
        //                if (item != null)
        //                {
        //                    try
        //                    {
        //                        ((SocketChannel)item).ElementIdList.Add(elementId);
        //                    }
        //                    catch (System.Exception ex)
        //                    {
        //                        System.Diagnostics.Debug.Write(ex.ToString());
        //                    }
        //                    return new ApiResponse(true, "Subscription added");
        //                }
        //                else
        //                {
        //                    return new ApiResponse(false, "X: Subscription to unknown subscriber could not be added. You must open websocket before subscribing.");
        //                }
        //            }
        //            else
        //            {
        //                return new ApiResponse(false, $"No channel name {this.ChannelType.ToString()}. You must open websocket before subscribing.");
        //            }
        //        }

        //        return new ApiResponse(false, "No channels created. Channel must be establish first. You must open websocket before subscribing.");
        //    }
        //    finally
        //    {

        //    }
        //}

        #endregion WebSocket

    }
}