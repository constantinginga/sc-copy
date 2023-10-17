using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model
{
	public class Besked
	{
        public string emne { get; set; }
        public int toMemberId { get; set; }

        public string besked { get; set; }

    }

    public class Message
    {
        public string Subject { get; set; }
        public int ToMemberId { get; set; }
        public string Body { get; set; }
    }

}