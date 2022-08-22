using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Logic.Contracts.BankOne;
using ServiceBus.Logic.Contracts.Service_Contracts;
using ServiceBus.Logic.Integration.Strategy;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Service
{
   public  class AccountGenericService: IAccountGenericService
    {
        string className = "AccountGenericService";
        IAccountByAccountNoIntegration accountByAccountNoIntegration;       
        IBankOneAccountCreationIntegration bankOneAccountCreationIntegration;
        IBankOneBVNValidationIntegration bankOneBVNValidation;
        public AccountGenericService(
            IAccountByAccountNoIntegration accountByAccountNoIntegration, 
            IBankOneAccountCreationIntegration bankOneAccountCreationIntegration, 
            IBankOneBVNValidationIntegration bankOneBVNVal)
        {
            this.accountByAccountNoIntegration = accountByAccountNoIntegration;
            this.bankOneAccountCreationIntegration = bankOneAccountCreationIntegration;
            this.bankOneBVNValidation = bankOneBVNVal;
        }


        public GetAccountByAccountNoResponse GetAccountByAccountNo(GetAccountByAccountNoRequest request)
        {
            string methodName = "GetAccountByAccountNo";
            try
            {
                if (string.IsNullOrEmpty(request.OperatorId))
                {
                    return new GetAccountByAccountNoResponse()
                    {
                        ResponseCode = "01",
                        ResponseMessage = "Operator Id is required",
                        OperatorId = request.OperatorId,
                        BankId = request.BankCode
                    };
                }

                if (string.IsNullOrEmpty(request.BankCode))
                {
                    return new GetAccountByAccountNoResponse()
                    {
                        ResponseCode = "01",
                        ResponseMessage = "BankCode is required",
                        OperatorId = request.OperatorId,
                        BankId = request.BankCode
                    };
                }


                if (string.IsNullOrEmpty(request.AccountNumber))
                {
                    return new GetAccountByAccountNoResponse()
                    {
                        ResponseCode = "01", 
                        ResponseMessage = "Account Number is Required ",
                        OperatorId = request.OperatorId,
                        BankId = request.BankCode 
                    };
                }
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
                        return accountByAccountNoIntegration.GetAccountByAccountNo(request);                   
                    case Core.enums.OperatorEnum.Others:
                        return new GetAccountByAccountNoResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new GetAccountByAccountNoResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new GetAccountByAccountNoResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {

                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetAccountByAccountNoResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }
              

        public AccountCreationResponse CreateAccount(AccountRequest request)
        {
            string methodName = "CreateAccount";
            try
            {
                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         AccountCreationResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
                var account = new Account();
                #region Mapping
              //  account.Nationality = request.Nationality;
                account.Title = request.Title;
                account.Gender = request.Gender;
                account.MaritalStatus = request.MaritalStatus;
                account.FirstName = request.FirstName;
                account.LastName = request.LastName;
                account.MiddleName = request.MiddleName;
                account.DOB = request.DOB;
                account.Country = request.Country;
                account.ResidentialState = request.State;
                account.ResidentialLGA = request.LGA;
                account.Email = request.Email;
                account.MobileNo = request.MobileNo;
                account.ResidentialAddress = request.Address;
                //account.UserName = request.UserName;
                //account.PIN = request.PIN;
                //account.Password = request.Password;
                //account.Signature = request.Signature;
                //account.Passport = request.Passport;
                //account.IdentityCard = request.IdentityCard;
                //account.DeviceName = request.DeviceName;
                //account.DeviceID = request.DeviceId;
                account.AccountTier = 1;
                //account.NIN = request.NIN;
                //account.AccountOfficerCode = request.AccountOfficerCode;
                //account.ProductCode = request.ProductCode;
                //account.NOKName = request.NOKName;
                //account.NOKNo = request.NOKPhone;
                //account.BVN = request.BVN;
                //account.ReferralPhoneNo = request.ReferralPhoneNo;
                //account.ReferralName = request.ReferralName;


                #endregion
                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return bankOneAccountCreationIntegration.CreateNewAccount(account);
                    case Core.enums.OperatorEnum.Others:
                        return new AccountCreationResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new AccountCreationResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new AccountCreationResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {

                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    AccountCreationResponse()
                    { ResponseCode = "96", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }

        public BVNValidationResponse ValidateBVN(BVNValidationRequest request)
        {
            string methodName = "ValidateBVN";
            try
            {
                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         BVNValidationResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return bankOneBVNValidation.ValidateBVN(request);
                    case Core.enums.OperatorEnum.Others:
                        return new BVNValidationResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new BVNValidationResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new BVNValidationResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {

                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    BVNValidationResponse()
                    { ResponseCode = "96", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }

        public AccountEnquiryResponse AccountEnquiry(GetAccountByAccountNoRequest request)
        {
            string methodName = "AccountEnquiry";
            try
            {
                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         AccountEnquiryResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return accountByAccountNoIntegration.AccountEnquiry(request);
                    case Core.enums.OperatorEnum.Others:
                        return new AccountEnquiryResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new AccountEnquiryResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new AccountEnquiryResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {

                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    AccountEnquiryResponse()
                    { ResponseCode = "96", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }


        public GetAccountBalanceResponse GetBalance(GetAccountBalanceRequest request)
        {
            string methodName = "GetBalance";
            try
            {
                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         GetAccountBalanceResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return accountByAccountNoIntegration.GetBalance(request);
                    case Core.enums.OperatorEnum.Gemini:
                        return accountByAccountNoIntegration.GetBalance(request);
                    case Core.enums.OperatorEnum.Others:
                        return new GetAccountBalanceResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new GetAccountBalanceResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new GetAccountBalanceResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {

                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetAccountBalanceResponse()
                    { ResponseCode = "96", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }

    }
}
 