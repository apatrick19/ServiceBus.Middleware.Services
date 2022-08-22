using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServicBus.Logic.Contracts;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class BankOneCoreBankingAuthentication : IBankOneCoreBankingAuthentication
    {
        static string className = "CoreBankingAuthentication";
        IApiPostAndGet apiservice;
        public BankOneCoreBankingAuthentication(IApiPostAndGet service)
        {
            apiservice = service;
        }
        public string GetCoreBankingSessionID()
        {
            string MethodName = "GetCoreBankingSessionID";
            LogMachine.LogInformation(className, MethodName, $"entered new method");
            try
            {
                var authRequest = new
                {
                    Username = AppConfig.AuthUsername,
                    Password = AppConfig.AuthPassword,
                    AppMode = AppConfig.AppMode,
                    BranchId = AppConfig.BranchId
                };
                string Url=$"{AppConfig.CoreBankingBaseUrl}login";

                LogMachine.LogInformation(className, MethodName, $"calling corebanking {Url}; {JsonConvert.SerializeObject(authRequest)}");
                var authResult = apiservice.UrlPost(Url, authRequest);

                if (string.IsNullOrEmpty(authResult))
                {
                    return string.Empty;
                }

                //LogMachine.LogInformation(className, MethodName, $"result {authResult}");
                object dec = JsonConvert.DeserializeObject(authResult);     // deserializing Json string (it will deserialize Json string)
                JObject obj = JObject.Parse(dec.ToString());        // it will parse deserialize Json object
                                                           
                var SessionId = obj["Payload"]["SessionId"];
                LogMachine.LogInformation(className, MethodName, $"sessionid {SessionId}");
                return SessionId.ToString();                           
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                LogMachine.LogInformation(className, MethodName, $"error {ex.ToString()}");
                return string.Empty;
            }
        }
    }
}
