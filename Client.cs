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
                InRequest;
        }

        internal async Task<Response<T>> Request<T>(string InRequest, HttpContent? InContent = null)
        {
            Response<HttpResponseMessage> httpResponse = await RequestHttp(InRequest, InContent);
            int code = httpResponse.error != null ? httpResponse.error.code : 0;

            if (httpResponse.success && httpResponse.data != null)
            {
                try
                {
                    string content = await httpResponse.data.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<Response<T>>(content);
                    if (response != null)
                        return response; 
                }
                catch (Exception ex)
                {
                    code = 2;
                    Trace.WriteLine(ex.ToString());
                    Trace.WriteLine("Error response: " + ErrorHandling.GetMessageFromCode(code));
                }
            }

            return new()
            {
                success = false,
                error = new()
                {
                    code = code
                }
            };
        }

        internal async Task<Response<byte[]>> RequestBytes(string InRequest)
        {
            byte[]? bytes = null;
            var httpResponse = await RequestHttp(InRequest);
            if (httpResponse.success && httpResponse.data != null)
                bytes = httpResponse.data.Content.ReadAsByteArrayAsync().Result;
            return new()
            {
                success = httpResponse.success,
                error = httpResponse.error,
                data = bytes
            };
        }

        internal async Task<Response<HttpResponseMessage>> RequestHttp(string InRequest, HttpContent? InContent = null)
        {
            Trace.WriteLine("Request: " + InRequest);

            int code = 1;
            try
            {
                HttpResponseMessage? response = null;
                if (InContent != null)
                    response = await HttpClient.PostAsync(InRequest, InContent);
                else
                    response = await HttpClient.GetAsync(InRequest);

                code = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("Success response");
                    return new()
                    {
                        success = true,
                        data = response
                    };
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            return new()
            {
                success = false,
                error = new Error()
                {
                    code = code
                }
            };
        }
    }
}
