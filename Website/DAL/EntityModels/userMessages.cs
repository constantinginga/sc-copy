//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StartupCentral.DAL.EntityModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class userMessages
    {
        public System.Guid Id { get; set; }
        public System.Guid FromUser { get; set; }
        public System.Guid ToUser { get; set; }
        public string Body { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Attachment { get; set; }
        public string Subject { get; set; }
        public bool Read { get; set; }
        public string PitchAttatchment { get; set; }
    }
}
