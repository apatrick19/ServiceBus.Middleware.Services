using ServicBus.Logic.Contracts;
using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Core.Contracts.Generic.Interface;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Data.ORM.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Implementations
{
   public static class ResponseDictionary
    {
          
         /// <summary>
         /// Gets description of a partivular response code 
         /// </summary>
         /// <param name="Code"></param>
        public static ResponseModel GetCodeDescription(string Code)
        {
            try
            {
                if (string.IsNullOrEmpty(Code))
                {
                    return new ResponseModel() { ResponseCode = "96", ResponseDescription = InMemory.Descriptions.FirstOrDefault(x => x.Key == Code).Value };
                }
                return new ResponseModel() {ResponseCode=Code,ResponseDescription= InMemory.Descriptions.FirstOrDefault(x => x.Key == Code).Value };
                    
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        /// <summary>
        /// gets description and binds object as well
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static ResponseModel GetCodeDescription(string Code, string Obj)
        {
            try
            {
                if (string.IsNullOrEmpty(Obj))
                {
                    return new ResponseModel() { ResponseCode = "96", ResponseDescription = $"{InMemory.Descriptions.FirstOrDefault(x => x.Key == Code).Value??""}"};
                }                
                return new ResponseModel() { ResponseCode = Code, ResponseDescription = $"{Obj}" };
              //  return new ResponseModel() { ResponseCode = Code, ResponseDescription = $"{InMemory.Descriptions.FirstOrDefault(x => x.Key == Code).Value ?? ""} ", ResultObject=Obj };

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }
        
        public static ResponseModel GetCodeDescription(string Code, object Obj)
        {
            try
            {
                if (string.IsNullOrEmpty(Code))
                {
                    return new ResponseModel() { ResponseCode = "96", ResponseDescription = InMemory.Descriptions.FirstOrDefault(x => x.Key == Code).Value, ResultObject = Obj };
                }
                return new ResponseModel() { ResponseCode = Code, ResponseDescription = InMemory.Descriptions.FirstOrDefault(x => x.Key == Code).Value, ResultObject = Obj };

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        public static ResponseModel GetCodeDescription(string Code, string message, object Obj)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    return new ResponseModel() { ResponseCode = "96", ResponseDescription = InMemory.Descriptions.FirstOrDefault(x => x.Key == Code).Value, ResultObject = Obj };
                }
                return new ResponseModel() { ResponseCode = Code, ResponseDescription = message, ResultObject = Obj };

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }
    }


    public class ResponseModel:Response
    {
        public  object ResultObject { get; set; }
    }
}
