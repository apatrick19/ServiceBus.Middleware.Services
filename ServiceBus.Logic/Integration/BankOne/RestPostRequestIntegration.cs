using Newtonsoft.Json;
using RestSharp;
using ServiceBus.Logic.Implementations.Logger;
using System;
using System.Net;

namespace ServiceBus.Logic.Integration
{
    public class RestPostRequestIntegration
    {
        private static string className = "RestPostRequestIntegration";

        public static IRestResponse PostRequestIntegration(object request, string endPoint)
        {
            string methodName = "PostRequestIntegration";
            var response = new object();

            try
            {
                var client = new RestClient(endPoint);
                var webRequest = new RestRequest(Method.POST);
                webRequest.AddHeader("content-type", "application/json");               
                string param = JsonConvert.SerializeObject(request);
                webRequest.AddParameter("application/json", param, ParameterType.RequestBody);
                webRequest.RequestFormat = DataFormat.Json;

                IRestResponse webResponse = client.Execute(webRequest);

                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    LogMachine.LogInformation(className, "RestPostRequestIntegration client.Execute Request", JsonConvert.SerializeObject(webResponse));
                }

                response = JsonConvert.DeserializeObject<object>(webResponse?.Content);

                LogMachine.LogInformation( className, methodName, "RestPostRequestIntegration Response Details \r\n" + JsonConvert.SerializeObject(response));

                return webResponse;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(className, methodName,"", ex);
            }

            return null;
        }

        public static T APICall<T>(object request, string endPoint)
        {
            string methodName = "APICall";
            //var response = new object();

            try
            {
                var client = new RestClient(endPoint);
                var webRequest = new RestRequest(Method.POST);
                webRequest.AddHeader("content-type", "application/json");
                string param = JsonConvert.SerializeObject(request);
                webRequest.AddParameter("application/json", param, ParameterType.RequestBody);
                webRequest.RequestFormat = DataFormat.Json;

                IRestResponse webResponse = client.Execute(webRequest);

                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    LogMachine.LogInformation( className, "RestPostRequestIntegration client.Execute Request", JsonConvert.SerializeObject(webResponse));
                }

               var response = JsonConvert.DeserializeObject<T>(webResponse?.Content);

                LogMachine.LogInformation(className, methodName, "RestPostRequestIntegration Response Details \r\n" + JsonConvert.SerializeObject(response));

                return response;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(className, methodName, "",ex);
                 return default(T);
            }

           
        }

        public static T APICallGet<T>(string request, string endPoint)
        {
            string methodName = "APICall";
            //var response = new object();

            try
            {
                var client = new RestClient(endPoint);
                var webRequest = new RestRequest(Method.GET);
                webRequest.AddHeader("content-type", "application/json");               
                webRequest.AddParameter("application/json", request, ParameterType.RequestBody);
                webRequest.RequestFormat = DataFormat.Json;

                IRestResponse webResponse = client.Execute(webRequest);

                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    LogMachine.LogInformation(className, "RestPostRequestIntegration client.Execute Request", JsonConvert.SerializeObject(webResponse));
                }

                var response = JsonConvert.DeserializeObject<T>(webResponse?.Content);

                LogMachine.LogInformation(className, methodName, "RestPostRequestIntegration Response Details \r\n" + JsonConvert.SerializeObject(response));

                return response;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(className, methodName, "", ex);
                return default(T);
            }


        }

    }
}
