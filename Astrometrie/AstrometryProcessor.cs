using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Astrometrie
{
    class AstrometryProcessor
    {
        public static void UploadImage()
        {
            Properties.Settings.Default.message = "Uploading file.";

            Task<string> responseContent = UploadImage(ImgHelper.GetImgPath());

            if (responseContent == null)
            {
                return;
            }

            responseContent.Wait();

            string json = responseContent.Result;

            ResponseJson responseObj = JsonConvert.DeserializeObject<ResponseJson>(json);

            if (responseObj.Status == "success")
            {
                bool parsed = Int32.TryParse(responseObj.SubId, out int parseResult);

                if (parsed)
                {
                    ApiHelper.SubId = parseResult;
                }
            }
        }
        public static bool GetSubmissionStatus()
        {
            //int time = 0;
            //int sleep = 10000;

            DateTime time = DateTime.UtcNow;

            if (ApiHelper.SubId == 0) return false;

            string url = $"submissions/" + ApiHelper.SubId;

            DateTime waitTime = DateTime.UtcNow;

            while (true)
            {
                if (DateTime.UtcNow - waitTime < TimeSpan.FromSeconds(10))
                {
                    continue;
                }

                Properties.Settings.Default.message = DateTime.UtcNow.ToString("HH:mm:ss") 
                    + " Asking for submission results...";


                try
                {
                    Task<string> task = SendRequest(url);

                    task.Wait();

                    string jsonString = task.Result;

                    //Console.WriteLine(jsonString);


                    ResponseJson responseObj
                        = JsonConvert.DeserializeObject<ResponseJson>(jsonString);

                    if (responseObj.job_calibrations.Count() != 0)
                    {
                        Properties.Settings.Default.message = "Submission succesfull";
                        ApiHelper.JobId = responseObj.jobs[0].ToString();
                        return true;
                    }
                }
                catch
                {
                    Properties.Settings.Default.message = "Connection error. Retrying...";
                    //Thread.Sleep(2000);
                }

                if (DateTime.UtcNow - time > TimeSpan.FromMinutes(30))
                {
                    return false;
                }

                waitTime = DateTime.UtcNow;
            }
        }
        public static bool GeJobStatus()
        {
            string url = $"jobs/" + ApiHelper.JobId;

            Properties.Settings.Default.message = "Getting job status...";

            Task<string> task = SendRequest(url);

            task.Wait();

            ResponseJson responseObj
                    = JsonConvert.DeserializeObject<ResponseJson>(task.Result);

            if (responseObj.Status == "success")
            {
                Properties.Settings.Default.message = "Job succesfull";

                return true;
            }

            return false;
        }
        public static CoordinatesModel GetAstrometryResults()
        {
            string url = $"jobs/" + ApiHelper.JobId + "/calibration/";

            Properties.Settings.Default.message = "Getting astrometry result...";

            Task<string> task = SendRequest(url);

            task.Wait();

            CoordinatesModel responseObj
                    = JsonConvert.DeserializeObject<CoordinatesModel>(task.Result);

            Properties.Settings.Default.message = "Astrometry succesfull";

            return responseObj;
        }



        private static async Task<string> SendRequest(string url)
        {
            RequestJson requestJson = new RequestJson() { session = ApiHelper.SessionKey };
            string jsonString = JsonConvert.SerializeObject(requestJson);

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response
                    = await ApiHelper.ApiClient.PostAsync(url, content).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    return responseContent;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        private static async Task<string> UploadImage(string path)
        {
            string url = $"upload";

            using (var content = new MultipartFormDataContent())
            {
                //firstPart
                RequestJson fits = new RequestJson() { session = ApiHelper.SessionKey };
                string jsonString = JsonConvert.SerializeObject(fits);
                var firstPart = new StringContent(jsonString, Encoding.UTF8, "application/json");

                //secondPart
                byte[] img = null;
                try
                {
                    img = ImgHelper.ImgToByteArray(path);
                }
                catch
                {
                    Properties.Settings.Default.message = "File not found.";
                    return null;
                }


                content.Add(firstPart, "request-json");
                content.Add(new StreamContent(new MemoryStream(img)), "file", "file.fit");


                using (HttpResponseMessage response
                    = await ApiHelper.ApiClient.PostAsync(url, content).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        return responseContent;
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }
        }
    }
}
