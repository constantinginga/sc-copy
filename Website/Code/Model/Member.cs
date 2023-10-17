using System;
using System.Collections.Generic;

namespace StartupCentral.Code.Model
{
    public class Member
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }
        public Guid Key { get; internal set; }
        public string ContentTypeAlias { get; internal set; }

        public List<int> Tags { get; set; }

        public string AvatarImageUrl { get; set; }

        public string Description { get; set; }

        public string ShortText { get; set; }

        public bool Unavailable { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string NDAUrl { get; set; }

        public string CVUrl { get; set; }
    }
}