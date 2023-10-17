using Umbraco.Core.Models;

namespace StartupCentral.Code.Model
{
    public class Fagomraade
    {
        public Fagomraade()
        {
        }

        public Fagomraade(IContent content)
        {
            if (content != null)
            {
                this.Id = content.Id;
                this.Name = content.Name;
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}