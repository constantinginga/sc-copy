using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using StartupCentral.Code.Model;
using Umbraco.Web.WebApi;

using static StartupCentral.Code.Model.CvrOpslag;
using System.Threading.Tasks;

namespace StartupCentral.Code.ApiControllers
{
    public class CvrController : UmbracoApiController
    {
        [HttpGet]
        public async Task<ApiResponse> GetCompanyInfo(string name)
        {
            if(!string.IsNullOrEmpty(name))
            {
                try
                {
                    string username = "Startup_Central_CVR_I_SKYEN";
                    string password = "e4b6a195-5d3f-4c19-81da-05a7eb2fb328";
                    string url = "http://distribution.virk.dk/cvr-permanent/virksomhed/_search";

                    // Create the JSON body with the 'name' parameter
                    string jsonBody = $@"{{
                        ""query"": {{
                            ""bool"": {{
                                ""must"": [
                                    {{
                                        ""term"": {{
                                            ""Vrvirksomhed.cvrNummer"": ""{name}""
                                        }}
                                    }}
                                ]
                            }}
                        }}
                    }}";

                    using (HttpClient httpClient = new HttpClient())
                    {
                        // Set the Basic Authorization header
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"))
                        );

                        // Set the Content-Type header to indicate JSON
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Send the POST request
                        HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read and return the response data as a string
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            // Process the JSON response here, if needed.
                            // For example, you can deserialize the JSON into a model class.
                            // For demonstration purposes, I'll just return the JSON response as a string.
                            return new ApiResponse(true, jsonResponse);
                        }
                        else
                        {
                            // Handle the error if the request was not successful
                            string errorResponse = await response.Content.ReadAsStringAsync();
                            return new ApiResponse(false, $"Request failed with status code: {response.StatusCode}\nResponse: {errorResponse}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    //    string.Format("UpodiApiController.DownloadInvoice Exception {0}", ex.Message));
                    //throw ex;
                    return new ApiResponse(false, ex.Message);
                }
            }
            return new ApiResponse(false, "No record found.");
        }
    }
}