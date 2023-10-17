using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartupCentral.Code.Model;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace StartupCentral.Code.Class
{
    public class ActiveCampaignClient
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["StartupCentral.ActiveCampaign.ApiKey"];
        private readonly string _baseUrl = ConfigurationManager.AppSettings["StartupCentral.ActiveCampaign.BaseUrl"];
        
        private static readonly HttpClient HttpClient = new HttpClient();

        public ActiveCampaignClient()
        {
            if (string.IsNullOrEmpty(_apiKey))
                throw new ArgumentNullException(nameof(_apiKey));

            if (string.IsNullOrEmpty(_baseUrl))
                throw new ArgumentNullException(nameof(_baseUrl));
        }

        private string CreateBaseUrl(string apiAction)
        {
            return $"{_baseUrl}/admin/api.php?api_action={apiAction}&api_key={_apiKey}&api_output=json";
        }

        public async Task<ApiResponse> ApiAsync(string apiAction, Dictionary<string, string> parameters)
        {
            var uri = CreateBaseUrl(apiAction);

            using (var postContent = new FormUrlEncodedContent(parameters))
            {
                using (HttpResponseMessage response = await HttpClient.PostAsync(uri, postContent))
                {
                    response.EnsureSuccessStatusCode(); //throw if httpcode is an error

                    using (HttpContent content = response.Content)
                    {
                        string rawData = await content.ReadAsStringAsync();
                        //JObject json = JObject.Parse(rawData);
                        
                        var result = JsonConvert.DeserializeObject<ApiResponse>(rawData);
                        result.Data = rawData;                     
                        result.Success = (bool)JObject.Parse(result.Data.ToString())["result_code"];
                        result.Message = (string)JObject.Parse(result.Data.ToString())["result_message"];
                       
                        return result;
                    }
                }
            }
        }
    }
}