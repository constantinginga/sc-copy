using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.WebSockets;
using Umbraco.Core.Logging;
using static StartupCentral.Code.Class.LiveConnections;

namespace StartupCentral.Code.Class
{
    public class SocketChannel : WebSocketHandler
    {
        public string ChannelName { get { return _channelName.ToString(); } }

        private ChannelsType _channelName;

        public Guid MemberKey { get; set; }

        public override void OnOpen()
        {
            LiveConnections.Channels[_channelName].Add(this);
        }


        public SocketChannel(ChannelsType channelName, Guid elementId)
        {
            //Token = token;
            _channelName = channelName;
            MemberKey = elementId;




            if (!LiveConnections.Channels.ContainsKey(channelName))
            {
                // Apparently ContainsKey doesn't always get it right.
                try
                {
                    LiveConnections.Channels.Add(channelName, new WebSocketCollection());
                }
                catch (System.Exception) { }
            }
        }

        public override void OnMessage(string message)
        {
            var envelope = new SocketEnvelope(this.MemberKey, message);

            LiveConnections.Channels[_channelName].Broadcast(
                envelope.ToJson());

            //LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            //              $"User: '{envelope.Sender.Username}' ({envelope.Sender.Id}) | Message: '{ message }' ");
        }

        public override void OnClose()
        {
            try
            {
                base.OnClose();
                LiveConnections.Channels[_channelName].Remove(this);
            }
            finally
            {
              
            }
        }

        public override void OnError()
        {
            base.OnError();
        }

    }
}