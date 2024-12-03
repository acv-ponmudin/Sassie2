using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sassie2
{
    class SassieApi
    {
        HttpClient client;

        public SassieApi()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://uat.sassieshop.com");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("2sgstrans", "1.0"));
        }

        public void Authenticate(string jsonData)
        {
            string url = "https://uat.sassieshop.com/2sgstrans/sapi/api/token";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "*/*";
            //request.UserAgent = "PostmanRuntime/7.42.0";
            request.UserAgent = "sgstrans/1.0";

            // Write the JSON data to the request stream
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(jsonData);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Status Code: {response.StatusCode}");

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseText = reader.ReadToEnd();
                        Console.WriteLine($"Response: {responseText}");
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse errorResponse)
                {
                    Console.WriteLine($"Error: {errorResponse.StatusCode}");
                    using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        Console.WriteLine($"Error Details: {errorText}");
                    }
                }
                else
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }

        public async Task<string> AuthenticateAsync(string jsonData)
        {
            string token = "";
            try
            {
                //string jsonData = JsonConvert.SerializeObject(data); // Serialize to JSON
                StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("2sgstrans/sapi/api/token", content);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(responseData);

                    var result = JsonConvert.DeserializeObject<dynamic>(responseData);
                    token = result.access_token;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return token;
        }

        public async Task PostDataAsync(string jsonData, string token)
        {
            try
            {
                //string jsonData = JsonConvert.SerializeObject(data); // Serialize to JSON
                StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                //client.DefaultRequestHeaders.Add("Custom-Header", "HeaderValue");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.PostAsync("2sgstrans/sapi/api/job_import", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseData);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
