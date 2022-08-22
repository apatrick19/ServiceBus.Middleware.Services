using RestSharp;
using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Servicebus.SMS.Gateway.Integration
{
    public class MessageIntegration
    {

        static string className = "MessageIntegration";


            public static string UrlGet(string url)
            {           
                  try
                  {               
                      var client = new RestClient(url);
                      var webRequest = new RestRequest(Method.GET);
                      webRequest.AddParameter("Content-Type", "application/json", ParameterType.HttpHeader);
                      webRequest.AddParameter("Accept", "application/json", ParameterType.HttpHeader);
                      IRestResponse webResponse = client.Execute(webRequest);
                 
                      if (webResponse.StatusCode != HttpStatusCode.OK)
                      {
                          Trace.TraceInformation("error response not 200" + webResponse.StatusCode);
                      }
                      return webResponse.Content;
                  }
                  catch (Exception ex)
                  {
                      Trace.TraceInformation("error" + ex.ToString());
                      return string.Empty;
                  }          
        }

        public static T UrlPost<T>( string endPoint, object request)
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
                    Trace.TraceInformation(className+ " RestPostRequestIntegration client.Execute Request "+ JsonConvert.SerializeObject(webResponse));
                }

                var response = JsonConvert.DeserializeObject<T>(webResponse?.Content);

                Trace.TraceInformation(className+ methodName+ " RestPostRequestIntegration Response Details  \r\n" + JsonConvert.SerializeObject(response));

                return response;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(className+ methodName+ ""+ ex.ToString());
                return default(T);
            }


        }

    }
}