using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model
{
    public class Coach2StudMsgsReport
    {
        public int CoachId { get; set; }
        public string CoachName { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int NumberOfMessages { get; set; }
        public DateTime LastMsgDate { get; set; }
    }
}