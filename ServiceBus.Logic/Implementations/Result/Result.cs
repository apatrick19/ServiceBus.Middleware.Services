using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Implementations.Result
{
    public class Result
    {
        /// <summary>
        /// Set Generic Response by passing through a dictionary
        /// </summary>
        /// <param name="ResponseCode"></param>
        /// <returns></returns>
        public static ResponseModel SetResponse(string ResponseCode)
        {
            ResponseModel response = new ResponseModel();
            response.ResponseCode = ResponseCode;
            response.ResponseDescription = InMemory.Descriptions.FirstOrDefault(x => x.Key == ResponseCode).Value;
            return response;
        }

        public static ResponseModel SetResponse(string ResponseCode, string Message)
        {
            ResponseModel response = new ResponseModel();
            response.ResponseCode = ResponseCode;
            response.ResponseDescription = Message;
            return response;
        }

        public static ResponseModel SetResponse(string ResponseCode, string Message, dynamic input)
        {
            ResponseModel response = new ResponseModel();
            response.ResponseCode = ResponseCode;
            response.ResponseDescription = Message;
            response.ResultObject = input;
            return response;
        }

        /// <summary>
        /// Set Generic Response  and returns an dynamic objext 
        /// </summary>
        /// <param name="ResponseCode"></param>
        /// <returns></returns>
        //public static Response SetResponse(string ResponseCode, string entity)
        //{
        //    Response response = new Response();
        //    response.ResponseCode = ResponseCode;
        //    response.ResponseDescription = ResponseDictionary.Descriptions.FirstOrDefault(x => x.Key == ResponseCode).Value;
        //    response.ResponseObject = entity;
        //    return response;
        //}
    }
}
