using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Class
{
    public class SocketEnvelope
    {
        public SocketEnvelope(Guid memberId, string message)
        {
            this.Message = message;
        }

        public object Data { get; set; }

        public string Message { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}