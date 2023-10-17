using System;

namespace StartupCentral.Code.Model
{
    public class TemplateVideo
    {
        public int Id { get; set; }

        public Guid Key { get; set; }

        public string Name { get; set; }

        public string ContentTypeAlias { get; set; }
    }
}