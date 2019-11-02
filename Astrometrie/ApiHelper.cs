using Newtonsoft.Json;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Astrometrie
{
    static class ApiHelper
    {
        public static HttpClient ApiClient { get; set; }
        public static string SessionKey { get; set; }
        public static int SubId { get; set; }

        public static string JobId { get; set; }

        public static CoordinatesModel AstrometryResult { get; set; }



        public static void InitializeClient()
        {
            ApiClient = new HttpClient();

            ApiClient.DefaultRequestHeaders.Accept.Clear();

            ApiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("multipart/form-data")); //x-www-form-urlencoded

            ApiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/text"));

            ApiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

            ApiClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ApiClient.BaseAddress = new Uri($"http://nova.astrometry.net/api/");

        }
        public static bool InitializeSession()
        {
            Properties.Settings.Default.message = "Initializing session/Logging in...";

            Task<string> session = Login();

            SaveSession(session);

            if (SessionKey != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private static async Task<string> Login()
        {
            string url = $"login";

            //string apikey = ConfigurationManager.AppSettings["apikey"];

            string apikey = ConfigurationManager.AppSettings.Get("apikey");

            string json = "{\"apikey\": \"" + apikey + "\"}";

            var requestJson = new Dictionary<string, string>
            {
                {"request-json", json}
            };

            var request = new FormUrlEncodedContent(requestJson);

            using (HttpResponseMessage response
                = await ApiClient.PostAsync(url, request).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    string sessionStatus = await response.Content.ReadAsStringAsync();

                    return sessionStatus;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        private static void SaveSession(Task<string> sessionStatus)
        {
            string json = sessionStatus.Result;

            ResponseJson responseObj = JsonConvert.DeserializeObject<ResponseJson>(json);

            if (responseObj.Status == "success")
            {
                SessionKey = responseObj.Session;
            }
        }

    }
}
