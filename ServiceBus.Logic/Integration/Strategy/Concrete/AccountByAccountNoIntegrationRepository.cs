using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.enums;
using ServiceBus.Logic.Contracts.BankOne;
using ServiceBus.Logic.Service;

namespace ServiceBus.Logic.Integration.Strategy
{
    public  class AccountByAccountNoIntegrationRepository: IAccountByAccountNoIntegrationRepository
    {
        private readonly Func<string, IAccountByAccountNoIntegration> accountByAccountNoIntegration;
        public AccountByAccountNoIntegrationRepository(Func<string, IAccountByAccountNoIntegration> accountByAccountNoIntegration)
        {
            this.accountByAccountNoIntegration = accountByAccountNoIntegration;
        }

        public AccountEnquiryResponse AccountEnquiry(GetAccountByAccountNoRequest request)
        {
            throw new NotImplementedException();
        }

        public GetAccountByAccountNoResponse GetAccountByAccountNo(GetAccountByAccountNoRequest request)
        {
            if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
            {
                return
                     new
                     GetAccountByAccountNoResponse()
                     { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
            }

            switch (OperatorService.GetOperator(request.OperatorId))
            {
                case Core.enums.OperatorEnum.BankOne:
                    return accountByAccountNoIntegration(OperatorEnum.BankOne.ToString()).GetAccountByAccountNo(request);
                case Core.enums.OperatorEnum.Gemini:
                    return accountByAccountNoIntegration(OperatorEnum.Gemini.ToString()).GetAccountByAccountNo(request);
                case Core.enums.OperatorEnum.Others:
                    return new GetAccountByAccountNoResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                case Core.enums.OperatorEnum.None:
                    return new GetAccountByAccountNoResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                default:
                    return new GetAccountByAccountNoResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }

        public GetAccountBalanceResponse GetBalance(GetAccountBalanceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
