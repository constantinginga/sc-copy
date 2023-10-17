using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StartupCentral.Code.Model
{
    [JsonObject]
    public class ValidationResponse
    {
        public ValidationResponse()
        {
        }

        public ValidationResponse(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

}