using Microsoft.Web.WebSockets;
using System.Collections.Generic;

namespace StartupCentral.Code.Class
{
    public static class LiveConnections
    {
        public enum ChannelsType
        {
            MobilePayApi
            
        }

        public static Dictionary<ChannelsType, WebSocketCollection> Channels = new Dictionary<ChannelsType, WebSocketCollection>();
    }
}