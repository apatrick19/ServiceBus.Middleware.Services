using ServiceBus.Core.DataTransferObject;
using ServiceBus.Logic.Contracts.BankOne;
using ServiceBus.Logic.Contracts.Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Service
{
   public  class TransactionGenericService: ITransactionGenericService
    {
        string className = "AccountGenericService";
        IBankOneTransactionIntegration bankOneTransactionIntegration;
       
        public TransactionGenericService(
            IBankOneTransactionIntegration TransactionIntegration    )
        {
            this.bankOneTransactionIntegration = TransactionIntegration;
          
        }


        public GetTransactionHistoryResponse GetTransaction(GetTransactionHistoryRequest request)
        {
            string methodName = "GetAccountByAccountNo";
            try
            {
                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         GetTransactionHistoryResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return bankOneTransactionIntegration.GetTransaction(request);
                    case Core.enums.OperatorEnum.Others:
                        return new GetTransactionHistoryResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new GetTransactionHistoryResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new GetTransactionHistoryResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {

                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetTransactionHistoryResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }

        public GetMiniStatementResponse GetMiniStatetment(GetMiniStatementRequest request)
        {
            string methodName = "GetMiniStatetment";
            try
            {
                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         GetMiniStatementResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return bankOneTransactionIntegration.GetMiniStatement(request);
                    case Core.enums.OperatorEnum.Others:
                        return new GetMiniStatementResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new GetMiniStatementResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new GetMiniStatementResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {

                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetMiniStatementResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }
    }
}
