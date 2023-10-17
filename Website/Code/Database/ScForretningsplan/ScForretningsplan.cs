using Newtonsoft.Json;
using System;
using System.Data.SqlClient;

namespace StartupCentral.Code.Database
{
    public class ScForretningsplan
    {
        public ScForretningsplan Apply(SqlDataReader dr)
        {
            Id = Convert.ToInt32(dr["Id"]);
            UniqueId = new Guid(Convert.ToString(dr["UniqueId"]));
            DocumentId = Convert.ToInt32(dr["DocumentId"]);
            MemberId = Convert.ToInt32(dr["MemberId"]);
            Seen = Convert.ToBoolean(dr["Seen"]);
            Downloaded = Convert.ToBoolean(dr["Downloaded"]);
            CreateDate = Convert.ToDateTime(dr["CreateDate"]);
            UpdateDate = Convert.ToDateTime(dr["UpdateDate"]);

            return this;
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("uniqueId")]
        public Guid UniqueId { get; set; }

        [JsonProperty("documentId")]
        public int DocumentId { get; set; }

        [JsonProperty("memberId")]
        public int MemberId { get; set; }

        [JsonProperty("seen")]
        public Boolean Seen { get; set; }

        [JsonProperty("downloaded")]
        public Boolean Downloaded { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }
    }
}