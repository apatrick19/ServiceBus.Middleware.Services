using ServicBus.Logic.Contracts;
using ServiceBus.Core;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Contracts.BankOne;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class BankOneBVNValidationIntegration: IBankOneBVNValidationIntegration
    {
        string className = "BankOneBVNValidationIntegration";
        IBankOneCoreBankingAuthentication coreBankingService;
        IApiPostAndGet apiservice;

        public BankOneBVNValidationIntegration(IBankOneCoreBankingAuthentication corebanking, IApiPostAndGet service)
        {
            coreBankingService = corebanking;
            apiservice = service;
        }

        public BVNValidationResponse ValidateBVN(BVNValidationRequest request)
        {
            string methodName = "ValidateBVN";
            try
            {
                var bvnRequest = new BVNRequest() { BVN = request.BVN, Token = BaseService.GetAppSetting("AuthToken") };

                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}account/bvn/getbvndetails";
                var bvnResult = apiservice.UrlPost<BVNResponse>(Url, bvnRequest);
                if (bvnResult == null)
                {
                   return new BVNValidationResponse()
                   { ResponseCode = "01", ResponseMessage = "No response from server", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
                if (bvnResult.isBvnValid == false)
                {
                   return new BVNValidationResponse()
                   { ResponseCode = "04", ResponseMessage = "No record found / invalid bvn", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
                return new BVNValidationResponse()
                {
                    ResponseCode = "00",
                    ResponseMessage = bvnResult.ResponseMessage,
                    OperatorId = request.OperatorId,
                    BankId = request.BankCode,
                    BVN = bvnResult.bvnDetails.BVN,
                    MobileNumber = bvnResult.bvnDetails.phoneNumber,
                    DOB = bvnResult.bvnDetails.DOB,
                    FirstName = bvnResult.bvnDetails.FirstName,
                    LastName = bvnResult.bvnDetails.LastName,
                    OtherNames = bvnResult.bvnDetails.OtherNames,
                    RequestId = request.RequestId
                };
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return new BVNValidationResponse()
                { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }
    }
}
