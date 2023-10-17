using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using static StartupCentral.Code.Class.LiveConnections;

namespace StartupCentral.Code.Class
{
    public static class SocketTransmitter
    {
        /// <summary>
        /// Method that determins who will recieve this transmission.
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="data"></param>
        public static void Broadcast(ChannelsType channelName, object data, Guid elementId)
        {
            DoBroadcast(channelName, data, elementId);
        }
 

        public static void DoBroadcast(ChannelsType channelName, object data, Guid memberKey)
        {
            if (LiveConnections.Channels.ContainsKey(channelName))
            {
                // Anyone listening?
                if (LiveConnections.Channels[channelName].Count > 0)
                {
                    string transmissionData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                    foreach (SocketChannel reciever in LiveConnections.Channels[channelName])
                    {
                        if (reciever.MemberKey == memberKey)
                        {
                            System.Diagnostics.Debug.WriteLine($"Sending message to: {memberKey} on channel {channelName}");
                            ThreadStart starter = () => reciever.Send(transmissionData);
                            Thread thread = new Thread(starter);
                            thread.Start();
                        }
                    }
                }
            }

        }
    }
}