using StartupCentral.Code.Class;
using StartupCentral.Code.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Web;

namespace StartupCentral.Hangfire
{
    public class InvitationJobs
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        bool isProd = Environment.MachineName.Equals("startupVM");

        public InvitationJobs(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        static HttpClient client = new HttpClient();

        static async Task<string> GetResAsync(string path)
        {
            string product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<string>();
            }
            return product;
        }

        static async Task<ApiResponse> GetRespAsync(string path)
        {
            try
            {
                ApiResponse product = new ApiResponse();
                client.Timeout = TimeSpan.FromHours(3);
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    product = await response.Content.ReadAsAsync<ApiResponse>();
                }
                return product;
            }
            catch (TaskCanceledException ex)
            {
                return new ApiResponse(false, ex.Message);
            }
        }

        public async Task<string> UpdateUnsubscribedUsers()
        {
            var url = "http://localhost:1111/umbraco/api/JobsApi/UnsubscribeUsers";
            if (isProd)
            {
                url = "https://www.startupcentral.dk/umbraco/api/JobsApi/UnsubscribeUsers";
            }

            string response = "";
            response = await GetResAsync(url);
            if (!int.TryParse(response, out int student))
            {
                return "NAN: " + response;
            }

            if (student == 0)
            {
                return "Unsubscribed users were update, no new unsubcribers";
            }
            else if (student > 0)
            {
                return $"Updates done, {student} students were updated";

            }
            else
            {
                return "something went wrong!";
            }
        }

        public async Task<string> UpdateLoginList()
        {
            var url = "http://localhost:1111/umbraco/api/JobsApi/DailyActivMembers";
            if (isProd)
            {
                url = "https://www.startupcentral.dk/umbraco/api/JobsApi/DailyActivMembers";
            }

            string response = "";
            response = await GetResAsync(url);
            if (!int.TryParse(response, out int student))
            {
                return "NAN: " + response;
            }

            if (student == 0)
            {
                return "No Logins for the day";
            }
            else if (student > 0)
            {
                return $"Updates done, {student} students were logged in last 24 hours";
            }
            else
            {
                return "something went wrong!";
            }
        }


        public async Task<string> CleanUpNewUnApprovedDuplicateMembers()
        {
            var url = "http://localhost:1111/umbraco/api/JobsApi/CleanUpNewUnApprovedDuplicateMembers";
            if (isProd)
            {
                url = "https://www.startupcentral.dk/umbraco/api/JobsApi/CleanUpNewUnApprovedDuplicateMembers";
            }

            string response = "";
            response = await GetResAsync(url);
            if (!int.TryParse(response, out int members))
            {
                return "NAN: " + response;
            }

            if (members == 0)
            {
                return "No duplicates for the day";
            }
            else if (members > 0)
            {
                return $"Updates done, {members} un-approved duplicates of approved members were deleted";
            }
            else
            {
                return "something went wrong!";
            }
        }


        public async Task<string> CleanUpDuplicateBasicMembers()
        {
            var url = "http://localhost:1111/umbraco/api/JobsApi/CleanUpDuplicateBasicMembers";
            if (isProd)
            {
                url = "https://www.startupcentral.dk/umbraco/api/JobsApi/CleanUpDuplicateBasicMembers";
            }

            string response = "";
            response = await GetResAsync(url);
            if (!int.TryParse(response, out int members))
            {
                return "NAN: " + response;
            }

            if (members == 0)
            {
                return "No duplicates for the day";
            }
            else if (members > 0)
            {
                return $"Updates done, {members} duplicates of basic members were deleted";
            }
            else
            {
                return "something went wrong!";
            }
        }
        public async Task<string> EmailingAllStudentsAsync()
        {
            var url = "http://localhost:1111/umbraco/api/beskedapi/EmailingAllStudents";

            if (isProd)
            {
                url = "https://www.startupcentral.dk/umbraco/api/BeskedApi/EmailingAllStudents";
            }
            ApiResponse response = null;
            response = await GetRespAsync(url);


            if (response.Success)
            {
                return response.Message;
            }

            else
            {
                return "something went wrong!";
            }

        }

        public async Task<string> EmailingAllCochesAsync()
        {
            var url = "http://localhost:1111/umbraco/api/beskedapi/EmailingAllCoaches";

            if (isProd)
            {
                url = "https://www.startupcentral.dk/umbraco/api/BeskedApi/EmailingAllCoaches";
            }
            ApiResponse response = null;
            response = await GetRespAsync(url);


            if (response.Success)
            {
                return response.Message;
            }

            else
            {
                return "something went wrong!";
            }

        }

        public async Task<string> MessageBadPayers()
        {
            var url = "http://localhost:1111/umbraco/api/beskedapi/BadPayersMessage";
            if (isProd)
            {
                url = "https://www.startupcentral.dk/umbraco/api/BeskedApi/BadPayersMessage";
            }
            ApiResponse response = null;
            response = await GetRespAsync(url);
            if (response.Success)
            {
                return response.Message;
            }

            else
            {
                return "something went wrong!";
            }
        }
    }
}