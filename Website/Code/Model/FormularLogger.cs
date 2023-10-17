using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model
{
    public class FormularLogger
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid Owner { get; set; }
        public string CVRNumber { get; set; }
        public string Industry { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Notes { get; set; }
        public bool Contacted { get; set; }
        public bool MailFlow { get; set; }
        public bool Read { get; set; }
    }
}