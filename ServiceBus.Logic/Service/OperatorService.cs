using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.enums;
using ServiceBus.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Service
{
    public class OperatorService
    {
        static string className = "OperatorService";
        public static Response ValidateOperator(string OperatorId)
        {
            string methodName = "ValidateOperator";
            try
            {
                var operatorResult=OperatorRepo.GetOperatorById(OperatorId);
                if (operatorResult==null)
                {
                    return new Response() { ResponseCode="01", ResponseMessage="Invalid Operator Id"};
                }
                return new Response() { ResponseCode = "00", ResponseMessage = "Operator validaion successful" };
            }
            catch (Exception ex)
            {

                LogService.LogError(OperatorId, className, methodName, ex);
                return new Response() { OperatorId= OperatorId, ResponseCode="06", ResponseMessage="System Malfunction, Operator validation failed" };
            }
        }

        public static OperatorEnum GetOperator(string OperatorId)
        {
            switch(OperatorId)
            {
                case "0001":
                    return OperatorEnum.BankOne;
                case "0002":
                    return OperatorEnum.Gemini;
                case "0003":
                    return OperatorEnum.Others;
                default:
                    return OperatorEnum.None;
            }
        }
    }
}
