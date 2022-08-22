using Newtonsoft.Json;
using ServiceBus.Core;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Custom.Contract;
using ServiceBus.Custom.Model;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.Validation;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Implementation
{
    public class AccountService : IAccountService
    {
        string classname = "AccountService";
        IAccountCreationService accountService;
        IAccountValidationService validationService;
        
        public AccountService(IAccountCreationService creationService, IAccountValidationService service)
        {
            accountService = creationService;
            validationService = service;
        }
        public ResponseModel CreateNewAccount(AccountRequest account)
        {
            string methodname = "CreateNewAccount";
            LogMachine.LogInformation(classname,methodname,"inside method about processing ");
            try
            {
                LogMachine.LogInformation(classname, methodname, "mapping models ");
                var Mainacct = new Account();
                #region Mapping
                //Mainacct.Nationality = account.Nationality;
                Mainacct.Title = account.Title;
                Mainacct.Gender = account.Gender;
                Mainacct.MaritalStatus = account.MaritalStatus;
                Mainacct.FirstName = account.FirstName;
                Mainacct.LastName = account.LastName;
                Mainacct.MiddleName = account.MiddleName;
                Mainacct.DOB = account.DOB;
                Mainacct.Country = account.Country;
                Mainacct.ResidentialState = account.State;
                Mainacct.ResidentialLGA = account.LGA;
                Mainacct.Email = account.Email;
                Mainacct.MobileNo = account.MobileNo;
                Mainacct.ResidentialAddress = account.Address;
                //Mainacct.UserName = account.UserName;
                //Mainacct.PIN = account.PIN;
                //Mainacct.Password = account.Password;
                //Mainacct.Signature = account.Signature;
                //Mainacct.Passport = account.Passport;
                //Mainacct.IdentityCard = account.IdentityCard;
                //Mainacct.DeviceName = account.DeviceName;
                //Mainacct.DeviceID = account.DeviceId;
                Mainacct.AccountTier = 1;
                //Mainacct.NIN = account.NIN;
                //Mainacct.AccountOfficerCode = account.AccountOfficerCode;
                //Mainacct.ProductCode = account.ProductCode;
                //Mainacct.NOKName = account.NOKName;
                //Mainacct.NOKNo = account.NOKPhone;
                //Mainacct.BVN = account.BVN;
                //Mainacct.ReferralPhoneNo = account.ReferralPhoneNo;
                //Mainacct.ReferralName = account.ReferralName;


                #endregion
                using (AiroPayContext context=new AiroPayContext())
                {
                    LogMachine.LogInformation(classname, methodname, $"checking db to see if account exists {account.MobileNo}");
                    var existing = context.Account.Where(x => x.MobileNo == account.MobileNo).FirstOrDefault();                   
                    if (existing==null)
                    {
                        //call corebanking service 
                        LogMachine.LogInformation(classname, methodname, "acount does not exists, about parsing to corebaking methods for acocunt creaton ");
                        var AcctResult = accountService.CreateNewAccount(Mainacct);
                        if (AcctResult.ResponseCode=="00")
                        {
                            Mainacct.Status = "0";
                            Mainacct.StatusName = "Account Approved";
                            context.Account.Add(Mainacct);
                        }                       
                        else
                        {
                            return AcctResult;
                        }                       
                        context.SaveChanges();
                        return AcctResult;
                    }                           
                    return  ResponseDictionary.GetCodeDescription("44","Account exists with mobile no, kindly login");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}");
               return ResponseDictionary.GetCodeDescription("06", ex);
            }
        }

        public ResponseModel GetAccountByAccountNo(string accountNo)
        {
            string methodname = "GetAccountByAccountNo";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");

                var result = validationService.GetAccountByAccountNo(accountNo);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel AccountEnquiry(string accountNo)
        {
            string methodname = "AccountEnquiry";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");

                var result = validationService.AccountEnquiry(accountNo);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel FreezeAccount(FreezeAccountRequest request)
        {
            string methodname = "FreezeAccount";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.ReferenceID))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Reference ID");
                }
                if (string.IsNullOrEmpty(request.Reason))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Please specify reason");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode!=BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.FreezeAccount(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel UnFreezeAccount(FreezeAccountRequest request)
        {
            string methodname = "UnFreezeAccount";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.ReferenceID))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Reference ID");
                }
                if (string.IsNullOrEmpty(request.Reason))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Please specify reason");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.UnFreezeAccount(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel CheckFreeze(FreezeStatus request)
        {
            string methodname = "CheckFreeze";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.CheckFreeze(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel CheckLienStatus(FreezeStatus request)
        {
            string methodname = "CheckLienStatus";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.CheckLienStatus(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }
        public ResponseModel PlaceLien(PlaceLienModel request)
        {
            string methodname = "PlaceLien";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.ReferenceID))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Reference ID");
                }
                if (string.IsNullOrEmpty(request.Reason))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Please specify reason");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.PlaceLien(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel UnPlaceLien(FreezeAccountRequest request)
        {
            string methodname = "UnPlaceLien";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.ReferenceID))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Reference ID");
                }
                if (string.IsNullOrEmpty(request.Reason))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Please specify reason");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.UnPlaceLien(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel ValidateBVN(string BVN)
        {
            string methodname = "ValidateBVN";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");

                var result = validationService.ValidateBVN(BVN);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }
        public ResponseModel GetAccounts(string accountNo)
        {
            throw new NotImplementedException();
        }

        public ResponseModel RegisterExistingAccount(ExistingAccountModel account)
        {
            string methodname = "RegisterExistingAccount";
            LogMachine.LogInformation(classname, methodname, "inside method about processing ");
            try
            {
                LogMachine.LogInformation(classname, methodname, "mapping models ");
                var Mainacct = new Account();
                #region Mapping
               
                Mainacct.AccountNumber = account.AccountNo;               
                Mainacct.MobileNo = account.MobileNo;              
                Mainacct.PIN = account.PIN;
                Mainacct.Password = account.Password;              
                Mainacct.DeviceName = account.DeviceName;
                Mainacct.DeviceID = account.DeviceID;

                #endregion
                using (AiroPayContext context = new AiroPayContext())
                {
                    LogMachine.LogInformation(classname, methodname, $"checking db to see if account exists {account.MobileNo}");
                    var existing = context.Account.Where(x => x.MobileNo == account.MobileNo && x.AccountNumber==account.AccountNo).FirstOrDefault();
                    if (existing == null)
                    {
                        //validate account no  
                        LogMachine.LogInformation(classname, methodname, "account has not been registered, about parsing to corebaking methods for account validation ");
                        var AcctResult = validationService.GetAccountByAccountNo(Mainacct);
                        if (AcctResult.ResponseCode == "00")
                        {                           
                            Mainacct.Status = "0";
                            Mainacct.StatusName = "Account Registered";
                            context.Account.Add(Mainacct);
                            context.SaveChanges();
                            return AcctResult;
                        }                       
                        else
                        {
                            return AcctResult;
                        }
                       
                    }
                    return ResponseDictionary.GetCodeDescription("44", "Account exists with mobile number, kindly login");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", ex);
            }
        }

        public ResponseModel UpdateDevice(DeviceUpdate request)
        {
            string methodname = "UpdateDevice";
            LogMachine.LogInformation(classname, methodname, "inside method about processing ");
            try
            {

                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("01", "invalid account number");
                }
                if (string.IsNullOrEmpty(request.DeviceName) || string.IsNullOrEmpty(request.DeviceId))
                {
                    return ResponseDictionary.GetCodeDescription("01", "device id and name must not be empty");
                }
                if (string.IsNullOrEmpty(request.PIN))
                {
                    return ResponseDictionary.GetCodeDescription("01", "invalid PIN");
                }
                using (AiroPayContext context = new AiroPayContext())
                {
                    LogMachine.LogInformation(classname, methodname, $"checking db to see if account exists {request.MobileNo}");
                    var existing = context.Account.Where(x => x.AccountNumber == request.AccountNo).FirstOrDefault();
                    if (existing == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "account does not exists");
                    }
                    //map account and phone no
                    if (existing.PIN != request.PIN)
                    {
                        return ResponseDictionary.GetCodeDescription("06", "PIN mismatch");
                    }
                    existing.DeviceID = request.DeviceId;
                    existing.DeviceName = request.DeviceName;
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00", "Device update successful");

                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", ex);
            }
        }

        public ResponseModel GetTransaction(TransactionHistoryBaseRequest request)
        {
            string methodname = "GetTransaction";
            LogMachine.LogInformation(classname, methodname, "inside method about processing ");
            try
            {
                if (string.IsNullOrEmpty(request.AccountNumber))
                {
                    return ResponseDictionary.GetCodeDescription("01", "please select an account number");
                }

                if (request.StartDate==null)
                {
                    return ResponseDictionary.GetCodeDescription("01", "please select a start date");
                }

                if (request.EndDate==null)
                {
                    return ResponseDictionary.GetCodeDescription("01", "please select an end date");
                }

                return validationService.GetTransaction(request);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", ex);
            }
        }

        public StatementBaseResponse GetStatement(TransactionRequest request)
        {
            string methodname = "GetTransaction";
            LogMachine.LogInformation(classname, methodname, "inside method about processing ");
            try
            {
                if (string.IsNullOrEmpty(request.AccountNumber))
                {
                    return new StatementBaseResponse { ResponseCode="01", ResponseMessage= "please select an account number" };
                }

                if (string.IsNullOrEmpty(request.StartDate))
                {
                    return new StatementBaseResponse { ResponseCode = "01", ResponseMessage = "please select a start date" };
                 
                }

                if (string.IsNullOrEmpty(request.EndDate))
                {
                    return new StatementBaseResponse { ResponseCode = "01", ResponseMessage = "please select an end date" };
                   
                }               

                 return validationService.GetStatement(request);
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, classname, methodname, ex);
                return new StatementBaseResponse { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }

        public ResponseModel ActivatePND(FreezeStatus request)
        {
            string methodname = "ActivatePND";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.ActivatePND(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel DeactivatePND(FreezeStatus request)
        {
            string methodname = "DeactivatePND";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.DeactivatePND(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }

        public ResponseModel CheckPNDStatus(FreezeStatus request)
        {
            string methodname = "CheckPNDStatus";
            try
            {
                LogMachine.LogInformation(classname, methodname, "inside method about processing ");
                if (string.IsNullOrEmpty(request.AccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Account No");
                }
                if (string.IsNullOrEmpty(request.AuthenticationCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Invalid Authentication Code");
                }
                if (request.AuthenticationCode != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("06", "Authentication Code MisMatch");
                }

                var result = validationService.CheckLienStatus(request);

                LogMachine.LogInformation(classname, methodname, $"response {JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, $"An error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "An error occurred, please contact admin");
            }
        }
    }
}
