//using Microsoft.Crm.Sdk.Samples.HelperCode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using ServicBus.Logic.Contracts;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Implementations
{
    public class ApiPostAndGet : IApiPostAndGet
    {

       // Start with base version and update with actual version later.
        private Version webAPIVersion = new Version(8, 0);

       // Provides a persistent client-to-CRM server communication channel.
        public HttpClient httpClient;

        string classname = "ApiPostAndGet";
        public ApiPostAndGet()
        {

        }

        public T UrlGet<T>(string url, string parameters)
        {
            string result = "";
            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var http = HttpConnectionManager.Instance;
                //  http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(headertype, password);
                using (var resp = http.Client.GetAsync(url).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;
                    Trace.TraceInformation($"Getting  URL {url} Status: Successful {result}");
                }
                Trace.TraceInformation($"Done Getting from URL {url}");

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }

        //public string UrlGet(string url, string parameters)
        //{
        //    string result = "";
        //    try
        //    {
        //        Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
        //        var http = HttpConnectionManager.Instance;
        //        //  http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(headertype, password);
        //        using (var resp = http.Client.GetAsync(url).Result)
        //        {
        //            if (!resp.IsSuccessStatusCode)
        //            {
        //                Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
        //                throw new Exception(resp.Content.ReadAsStringAsync().Result);
        //            }
        //            result = resp.Content.ReadAsStringAsync().Result;
        //            Trace.TraceInformation($"Getting  URL {url} Status: Successful {result}");
        //        }
        //        Trace.TraceInformation($"Done Getting from URL {url}");

        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
        //        return result;
        //    }
        //}

        /// <summary>
        /// this method sends a gets request with specified URL
        /// </summary>
        /// <param name = "url" ></ param >
        /// < param name="parameters"></param>
        /// <returns></returns>
        public async void DownloadFile(string url, string parameters,string path)
        {
            string result = "";

            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var http = HttpConnectionManager.Instance;
                using (var resp = http.Client.GetAsync(url).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");                        
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;

                    using (var file = System.IO.File.Create(path))
                    { // create a new file to write to
                        var contentStream = await resp.Content.ReadAsStreamAsync(); // get the actual content stream
                        await contentStream.CopyToAsync(file); // copy that stream to the file stream
                        await file.FlushAsync(); // flush back to disk before disposing
                    }

                    Trace.TraceInformation($"Getting  URL {url} Status: Successful created file {path}");

                }

                Trace.TraceInformation($"Done Getting from URL {url}");
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Get Request {ex.Message} \n {ex.StackTrace} ");
                throw ex;
            }

        }



        /// <summary>
        /// this method sends a gets request with specified URL
        /// </summary>
        /// <param name = "url" ></ param >
        /// < param name="parameters"></param>
        /// <returns></returns>
        public string UrlGet(string url, string parameters)
        {
            string result = "";

            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var http = HttpConnectionManager.Instance;
                using (var resp = http.Client.GetAsync(url).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                       
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    //using (AiroPayContext context = new AiroPayContext())
                    //{
                    //    RelayLogger requestAndResponse = new RelayLogger() { Date = DateTime.UtcNow, ActionName = "APIPostAndGet", ControllerName = "HTTPClient", Request = parameters, Response = resp?.Content?.ReadAsStringAsync()?.Result ?? "", URL = url };
                    //    context.RelayLogger.Add(requestAndResponse);
                    //    context.RelayLogger.Add(requestAndResponse); context.SaveChanges();
                    //}

                    result = resp.Content.ReadAsStringAsync().Result;

                    Trace.TraceInformation($"Getting  URL {url} Status: Successful {result}");

                }

                Trace.TraceInformation($"Done Getting from URL {url}");

                return result;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Get Request {ex.Message} \n {ex.StackTrace} ");
                throw ex;
            }

        }

        public  byte[] DownloadFile(string url, string parameters, string username, string password, string domain, string path)
        {
            string result = "";

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                NetworkCredential credentials = new System.Net.NetworkCredential(username, password, domain);
                request.Credentials = credentials;
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("accept-encoding", "gzip, deflate");
                request.AddHeader("cookie", "WSS_KeepSessionAuthenticated={18c3790d-8faf-4784-b8e7-7fc3098c786b}; WSS_KeepSessionAuthenticated={18c3790d-8faf-4784-b8e7-7fc3098c786b}; WSS_KeepSessionAuthenticated={18c3790d-8faf-4784-b8e7-7fc3098c786b}");
                request.AddHeader("Host", "crmhub.armpension.com");
                request.AddHeader("Postman-Token", "8c17191d-4ab6-48fe-8eb1-8221dd852471,e3e540bc-9a34-4bbb-be2a-1e64a3aa6eb9");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("User-Agent", "PostmanRuntime/7.13.0");
                request.AddHeader("Accept", "application/json;odata=verbose");
                request.AddHeader("Content-Type", "application/json");
                Trace.TraceInformation($"Done Getting from URL {url}");
                var response = client.DownloadData(request);                
                File.WriteAllBytes(path, response);
                return response;
               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Get Request {ex.Message} \n {ex.StackTrace} ");
                throw ex;
            }

        }
        public string UrlGet(string url, string parameters,string username,string password,string domain)
        {           
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                NetworkCredential credentials = new System.Net.NetworkCredential(username, password, domain);
                request.Credentials = credentials;
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("accept-encoding", "gzip, deflate");
                request.AddHeader("cookie", "WSS_KeepSessionAuthenticated={18c3790d-8faf-4784-b8e7-7fc3098c786b}; WSS_KeepSessionAuthenticated={18c3790d-8faf-4784-b8e7-7fc3098c786b}; WSS_KeepSessionAuthenticated={18c3790d-8faf-4784-b8e7-7fc3098c786b}");
                request.AddHeader("Host", "crmhub.armpension.com");
                request.AddHeader("Postman-Token", "8c17191d-4ab6-48fe-8eb1-8221dd852471,e3e540bc-9a34-4bbb-be2a-1e64a3aa6eb9");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("User-Agent", "PostmanRuntime/7.13.0");
                request.AddHeader("Accept", "application/json;odata=verbose");
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return response.Content;                   
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Get Request {ex.Message} \n {ex.StackTrace} ");
                throw ex;
            }

        }

        /// <summary>
        /// This method initiates a post request to specified url
        /// It has no authorization in its header
        /// </summary>
        /// <param name = "url" ></ param >
        /// < param name="theObject"></param>
        /// <returns></returns>
        public string UrlPost(string url, object theObject)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string result = string.Empty;
            string obj = "";
            try
            {

                var http = HttpConnectionManager.Instance;
                obj = JsonConvert.SerializeObject(theObject);
                StringContent content = new StringContent(obj, Encoding.UTF8, "application/json");
                using (var resp = http.Client.PostAsync(url, content).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }                    
                    result = resp.Content.ReadAsStringAsync().Result;

                }

                Trace.TraceInformation($"Done with UrlPost result =  {result}; returning");

                return result;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Post Request {ex.Message} \n {ex.StackTrace} ");
                
                return ex.ToString();
            }
        }

        /// <summary>
        /// This method initiates a post request to specified url
        /// it passes username and password in the request header
        /// </summary>
        /// <param name = "url" ></ param >
        /// < param name="theObject"></param>
        /// <param name = "userName" ></ param >
        /// < param name="password"></param>
        /// <returns></returns>
        public string UrlPost(string url, object theObject, string userName, string password)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string obj = "";
            try
            {

                string result = string.Empty;
                var http = HttpConnectionManager.Instance;
                obj = JsonConvert.SerializeObject(theObject);
                StringContent content = new StringContent(obj, Encoding.UTF8, "application/json");
                var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
                http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


                using (var resp = http.Client.PostAsync(url, content).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }


                    result = resp.Content.ReadAsStringAsync().Result;
                    
                }

                Trace.TraceInformation($"Done with UrlPost result =  {result}; returning");

                return result;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Post Request {ex.Message} \n {ex.StackTrace} ");
               
                throw ex;
            }
        }

        /// <summary>
        /// This method initiates a post request to specified url
        /// Its passes a token for authorization
        /// </summary>
        /// <param name = "url" ></ param >
        /// < param name="theObject"></param>
        /// <param name = "Token" ></ param >
        /// < returns ></ returns >
        public string UrlPost(string url, object theObject, string Token)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string obj = "";
            try
            {

                string result = string.Empty;
                var http = HttpConnectionManager.Instance;
                obj = JsonConvert.SerializeObject(theObject);
                StringContent content = new StringContent(obj, Encoding.UTF8, "application/json");
               // var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
                http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Token);


                using (var resp = http.Client.PostAsync(url, content).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }


                    result = resp.Content.ReadAsStringAsync().Result;
                    

                }

                Trace.TraceInformation($"Done with UrlPost result =  {result}; returning");

                return result;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Post Request {ex.Message} \n {ex.StackTrace} ");                
                throw ex;
            }
        }

        /// <summary>
        /// This method initiates a post request to specified url
        /// Its passes a token for authorization
        /// </summary>
        /// <param name = "url" ></ param >
        /// < param name="theObject"></param>
        /// <param name = "Token" ></ param >
        /// < returns ></ returns >
        public T UrlPost<T>(string url, object theObject, bool IsCustomKey, out string message)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string MySerializedObject = "";
            try
            {
                MySerializedObject = JsonConvert.SerializeObject(theObject);
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.Parameters.Clear();
                request.AddHeader("content-type", "application/json");
                request.AddHeader("Authorization", Base64Encode(TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time")).ToString()));
                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("application/json", MySerializedObject, ParameterType.QueryString);
                request.AddParameter("RequestSource", "Web", "application/json", ParameterType.QueryString);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    Trace.TraceInformation("Payday update succesfull; 200 returned");
                    var GenericObject = JsonConvert.DeserializeObject<T>(response.Content);
                    message = "Transaction successful";
                    return GenericObject;
                }
                Trace.TraceInformation($"http response not successful {response.Content}; throwing this exception ");
                throw new Exception(response.Content);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Post Request {ex.Message} \n {ex.StackTrace} ");
                message = ex.Message;
                return default(T);
            }
        }
                     
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        /// <summary>
        /// this method sends a gets request with specified URL
        /// It passes a token in its header 
        /// </summary>
        /// <param name = "url" ></ param >
        /// < param name="parameters"></param>
        /// <param name = "token" ></ param >
        /// < returns ></ returns >
        public string UrlGet(string url, string parameters, string token)
        {
            string result = "";

            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var http = HttpConnectionManager.Instance;
                http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                using (var resp = http.Client.GetAsync(url).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }


                    result = resp.Content.ReadAsStringAsync().Result;
                    Trace.TraceInformation($"Getting  URL {url} Status: Successful {result}");

                }

                Trace.TraceInformation($"Done Getting from URL {url}");

                return result;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An Error Occured on Get Request {ex.Message} \n {ex.StackTrace} ");
                Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }

        }

        public T UrlGet<T>(string url, string parameters, T newItem)
        {
            string result = "";
            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var http = HttpConnectionManager.Instance;
                using (var resp = http.Client.GetAsync(url).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;
                    Trace.TraceInformation($"Getting  URL {url} Status: Successful {result}");
                }

                Trace.TraceInformation($"Done Getting from URL {url}");
                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;
            }
            catch (Exception ex)
            {                
                return default(T);
            }
        }

        public T UrlGet<T>(string url, string parameters, string token, T newItem)
        {
            string result = "";
            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var http = HttpConnectionManager.Instance;
                http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                using (var resp = http.Client.GetAsync(url).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;
                    Trace.TraceInformation($"Getting  URL {url} Status: Successful {result}");
                }
                Trace.TraceInformation($"Done Getting from URL {url}");

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }

        public T UrlGet<T>(string url, string parameters, string headertype, string password)
        {
            string result = "";
            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var http = HttpConnectionManager.Instance;
                http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(headertype, password);
                using (var resp = http.Client.GetAsync(url).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;
                    Trace.TraceInformation($"Getting  URL {url} Status: Successful {result}");
                }
                Trace.TraceInformation($"Done Getting from URL {url}");

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }

        public T UrlGetWithRestSharp<T>(string url, string parameters, string key)
        {
            string result = "";
            try
            {
                Trace.TraceInformation($"insideUrlGet method URL= {url}; parameters={parameters} ");
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", key);
                request.AddParameter("text/plain", "", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                result = response.Content;               

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }

        public T UrlPostWithRestSharp<T>(string url, string parameters, string key)
        {
            string result = "";
            try
            {
                Trace.TraceInformation($"inside UrlPost method URL= {url}; parameters={parameters} ");
               
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", key);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", parameters, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                result = response.Content;

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }

        public T UrlPost<T>(string url, object theObject, T newItem)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string obj = "", result = string.Empty;
            try
            {

                var http = HttpConnectionManager.Instance;
                obj = JsonConvert.SerializeObject(theObject);
                StringContent content = new StringContent(obj, Encoding.UTF8, "application/json");
                Trace.TraceInformation($"Inside method URLPOST; url ={url}; payload{obj}");
                using (var resp = http.Client.PostAsync(url, content).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }

                    result = resp.Content.ReadAsStringAsync().Result;                   

                }

                Trace.TraceInformation($"Done with UrlPost result =  {result}; returning");

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An erorr occurred {ex?.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }

        public T UrlPost<T>(string url, object theObject, string Token, T newItem)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string obj = "";
            try
            {
                string result = string.Empty;
                var http = HttpConnectionManager.Instance;
                obj = JsonConvert.SerializeObject(theObject);
                StringContent content = new StringContent(obj, Encoding.UTF8, "application/json");
                Trace.TraceInformation($"Inside method URLPOST; url ={url}; payload{obj}");
                http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Token);
                using (var resp = http.Client.PostAsync(url, content).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;                    
                }

                Trace.TraceInformation($"Done with UrlPost result =  {result}; returning");

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                return GenericObject;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"an error occurred {ex.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }

        public T UrlPost<T>(string url, object theObject, string Token, T newItem, out string message)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string obj = "";
            try
            {
                string result = string.Empty;
                var http = HttpConnectionManager.Instance;
                obj = JsonConvert.SerializeObject(theObject);
                Trace.TraceInformation($"Inside method URLPOST; url ={url}; payload{obj}");
                StringContent content = new StringContent(obj, Encoding.UTF8, "application/json");
                http.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Token);
                using (var resp = http.Client.PostAsync(url, content).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;                    
                }

                Trace.TraceInformation($"Done with UrlPost result =  {result}; returning");

                var GenericObject = JsonConvert.DeserializeObject<T>(result);
                message = "";
                message = "Account creation successful";
                return GenericObject;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"an error occurred {ex.Message}; {ex?.InnerException?.StackTrace}");
                message = ex.Message;
                return default(T);
            }
        }

        public T UrlPost<T>(string url, object theObject)
        {
            Trace.TraceInformation($"Inside method URLPOST; url ={url}; request{theObject}");
            string obj = "", result = string.Empty;
            string methodname = "UrlPost";
            try
            {

                var http = HttpConnectionManager.Instance;
                obj = JsonConvert.SerializeObject(theObject);
                LogService.LogInfo("", classname, methodname, $"Request: URL {url}  payload \r\n {obj}");
                StringContent content = new StringContent(obj, Encoding.UTF8, "application/json");
                Trace.TraceInformation($"Inside method URLPOST; url ={url}; payload{obj}");
                using (var resp = http.Client.PostAsync(url, content).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Trace.TraceInformation($"http response not successful {resp.Content.ReadAsStringAsync().Result}; throwing this exception ");
                        
                        throw new Exception(resp.Content.ReadAsStringAsync().Result);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;
                    LogService.LogInfo("", classname, methodname, $"Response: URL {url}  Result \r\n {result}");
                }

                Trace.TraceInformation($"Done with UrlPost result =  {result}; returning");

                var GenericObject = JsonConvert.DeserializeObject<T>(result);

                return GenericObject;

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"an error occurred {ex.Message}; {ex?.InnerException?.StackTrace}");
                return default(T);
            }
        }


        /// <summary>
        /// Http Connection Manager Class
        /// </summary>
        public sealed class HttpConnectionManager
        {

            private HttpConnectionManager()
            {
                Client = new HttpClient();
            }

            /// <summary>
            /// Singleton Instance of<see cref="HttpConnectionManager"/>
            /// </summary>
            public static HttpConnectionManager Instance
            {
                get
                {
                    return Nested.instance;
                }
            }

            private class Nested
            {
            //    Explicit static constructor to tell C# compiler
            //     not to mark type as beforefieldinit
                static Nested() { }
                internal static readonly HttpConnectionManager instance = new HttpConnectionManager();
            }

            /// <summary>
            /// Gets session client
            /// </summary>
            public HttpClient Client { get;  set; }
            /// <summary>
            /// Disposes connection class
            /// </summary>
            public void Shutdown()
            {
                if (Client != null)
                {
                    Client.CancelPendingRequests();
                    Client.Dispose();
                    Client = null;
                }
            }


        }

        ///// <summary>
        ///// Obtains the connection information from the application's configuration file, then 
        ///// uses this info to connect to the specified CRM service.
        ///// </summary>
        ///// <param name = "args" > Command line arguments. The first specifies the name of the
        /////  connection string setting. </param>
        //public HttpClient ConnectToCRM()
        //{

        //    try
        //    {
        //        //Create a helper object to read app.config for service URL and application
        //        // registration settings.
        //        Configuration config = null;
        //        config = new FileConfiguration(null);
        //        //Create a helper object to authenticate the user with this connection info.
        //        Authentication auth = new Authentication(config);
        //        //Next use a HttpClient object to connect to specified CRM Web service.
        //        HttpClient httpClient = new HttpClient(auth.ClientHandler, true);
        //        //Define the Web API base address, the max period of execute time, the
        //        // default OData version, and the default response payload format.
        //        httpClient.BaseAddress = new Uri(config.ServiceUrl + "api/data/");
        //        httpClient.Timeout = new TimeSpan(0, 2, 0);
        //        httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
        //        httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
        //        httpClient.DefaultRequestHeaders.Accept.Add(
        //            new MediaTypeWithQualityHeaderValue("application/json"));

        //        return httpClient;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //public HttpClient ConnectToCRM(string[] cmdargs)
        //{
        //    // Create a helper object to read app.config for service URL and application
        //    //  registration settings
        //    Configuration config = null;
        //    if (cmdargs.Length > 0)
        //    { config = new FileConfiguration(cmdargs[0]); }
        //    else
        //    { config = new FileConfiguration(null); }
        //    // Create a helper object to authenticate the user with this connection info.
        //    Authentication auth = new Authentication(config);
        //    //   Next use a HttpClient object to connect to specified CRM Web service.
        //    httpClient = new HttpClient(auth.ClientHandler, true);
        //    //  Define the Web API base address, the max period of execute time, the
        //   // default OData version, and the default response payload format.
        //   httpClient.BaseAddress = new Uri(config.ServiceUrl + "api/data/");
        //    httpClient.Timeout = new TimeSpan(0, 2, 0);
        //    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
        //    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
        //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    return httpClient;
        //}


        //private string getVersionedWebAPIPath()
        //{
        //    return string.Format("v{0}/", webAPIVersion.ToString(2));
        //}

        /////// <summary>
        /////// Gets the Api version of CRM Manager Service
        /////// </summary>
        /////// <returns></returns>
        //public async Task<Version> getWebAPIVersion()
        //{
        //    try
        //    {
        //        HttpRequestMessage RetrieveVersionRequest =
        //     new HttpRequestMessage(HttpMethod.Get, getVersionedWebAPIPath() + "RetrieveVersion");

        //        HttpResponseMessage RetrieveVersionResponse =
        //            await httpClient.SendAsync(RetrieveVersionRequest);
        //        if (RetrieveVersionResponse.StatusCode == HttpStatusCode.OK)  //200
        //        {
        //            JObject RetrievedVersion = JsonConvert.DeserializeObject<JObject>(
        //                await RetrieveVersionResponse.Content.ReadAsStringAsync());
        //            //Capture the actual version available in this organization
        //            webAPIVersion = Version.Parse((string)RetrievedVersion.GetValue("Version"));
        //        }
        //        else
        //        {
        //            Trace.TraceInformation("Failed to retrieve the version for reason: {0}",
        //                 RetrieveVersionResponse.ReasonPhrase);
        //            throw new CrmHttpResponseException(RetrieveVersionResponse.Content);
        //        }

        //        return webAPIVersion;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}

    }
}
