using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model
{
    public class BeskedWithPlane
    {
        public Guid planId { get; set; }

        public Guid pitchId { get; set; }

        public int investorId { get; set; }
        public string besked { get; set; }
    }
}