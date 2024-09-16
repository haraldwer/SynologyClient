using Newtonsoft.Json;
using System.Diagnostics;

namespace Synology
{
    public class Client
    {
        public API API;
        public FileStation FileStation;

        HttpClient HttpClient;

        public Client(string InAddress, int InPort, TimeSpan InTimeout)
        {
            API = new(this);
            FileStation = new(this);

            HttpClient = new()
            {
                BaseAddress = new Uri(InAddress + ":" + InPort),
                Timeout = InTimeout,
            };
            Trace.WriteLine("HTTP Client created - addr: " + InAddress + ":" + InPort + " timeout:" + InTimeout.Seconds);
        }

        internal string ConstructRequest(string InRequestType, string InAPI, int InVersion, string InRequest)
        {
            return
                "/webapi/" + InRequestType +
                "?api=" + InAPI +
                "&version=" + InVersion.ToString() +
                "" + InRequest;
        }

        internal Response<T> Request<T>(string InRequest)
        {
            Response<string> jsonResponse = Request(InRequest);
            if (jsonResponse.success && jsonResponse.data != null)
            {
                Response<T>? response = JsonConvert.DeserializeObject<Response<T>>(jsonResponse.data);
                if (response != null)
                {
                    response.success = jsonResponse.success;
                    response.error = jsonResponse.error;
                    return response;
                }
            }
            return new()
            {
                success = jsonResponse.success,
                error = jsonResponse.error
            };
        }

        internal Response<string> Request(string InRequest)
        {
            Trace.WriteLine("Request: " + InRequest);

            try
            {
                using HttpResponseMessage response = HttpClient.GetAsync(InRequest).Result;
                if (!response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("Error response: " + ErrorHandling.GetMessageFromCode((int)response.StatusCode));
                    return new()
                    {
                        success = false,
                        error = new Error()
                        {
                            code = (int)response.StatusCode
                        }
                    };
                }

                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                Trace.WriteLine("Response: " + jsonResponse);
                return new()
                {
                    success = true,
                    data = jsonResponse
                };
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error response: " + ErrorHandling.GetMessageFromCode(1));
                return new()
                {
                    success = false,
                    error = new Error()
                    {
                        code = 1
                    }
                };
            }
        }
    }
}
