using Hangfire;
using ServicBus.Logic.Implementations;
using ServicBus.Logic.Implementations.IO.Image;
using ServiceBus.Core;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.PaymentItemSpace;
using ServiceBus.Logic.Model.PortalModel;
using ServiceBus.Logic.Model.Quickteller;
using ServiceBus.Logic.Model.Quickteller.bills;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class GenericLogic
    {
        static string classname = "GenericLogic";
        static string countrycode = BaseService.GetAppSetting("countrycode");

        public static List<DropdownResponse> FetchState()
        {
            string methodname = "FetchState";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.State.ToList();
                    if (result.Count > 0)
                    {
                        responses.Add(new DropdownResponse() { Name = "Select State", Id = -1, Code = "00" });
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID , Code=item.Code});
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchNationality()
        {
            string methodname = "FetchNationality";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.Nationality.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID, Code = item.ID.ToString() });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchProducts()
        {
            string methodname = "FetchProducts";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {
                string Url = $"{AppConfig.CoreBankingBaseUrl}Product/Get/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&mfbCode={BaseService.GetAppSetting("BankOneCode")}";
                var prdResult = RestPostRequestIntegration.APICallGet<List<ProductModel>>("", Url);
                if (prdResult == null)
                {
                    return responses;
                }
                if (prdResult.Count <= 0)
                {
                    return responses;
                }
                foreach (var item in prdResult)
                {
                    responses.Add(new DropdownResponse() { Name = item.ProductName,  Code = item.ProductCode });
                }
                return responses;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchBanks()
        {
            string methodname = "FetchBanks";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}BillsPayment/GetCommercialBanks/{BaseService.GetAppSetting("AuthToken")}";               
                var prdResult = RestPostRequestIntegration.APICallGet<List<BankModel>>("", Url);
                if (prdResult == null)
                {
                    return responses;
                }
                if (prdResult.Count <= 0)
                {
                    return responses;
                }
                foreach (var item in prdResult)
                {
                    responses.Add(new DropdownResponse() { Name = item.Name, Code = item.Code });
                }
                return responses;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchBillsCategory()
        {
            string methodname = "FetchBillsCategory";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {
               
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/GetBillerCategories/" + BaseService.GetAppSetting("AuthToken");
                var prdResult = RestPostRequestIntegration.APICallGet<List<CategoryResponse>>("", Url);
                if (prdResult == null)
                {
                    return responses;
                }
                if (prdResult.Count <= 0)
                {
                    return responses;
                }
                foreach (var item in prdResult)
                {
                    responses.Add(new DropdownResponse() { Name = item.Name, Code = item.ID });
                }
                return responses;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchBillers()
        {
            string methodname = "FetchBillsCategory";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {          
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/GetBillers/" + BaseService.GetAppSetting("AuthToken");
                var prdResult = RestPostRequestIntegration.APICallGet<List<BillersResponse>>("", Url);
                if (prdResult == null)
                {
                    return responses;
                }
                if (prdResult.Count <= 0)
                {
                    return responses;
                }
                foreach (var item in prdResult)
                {
                    responses.Add(new DropdownResponse() { Name = item.Name, Code = item.ID, ParentCode=item.CategoryId });
                }
                return responses;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchPaymentItems()
        {
            string methodname = "FetchBillsCategory";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {             

                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/GetPaymentItems/" + BaseService.GetAppSetting("AuthToken");
                var prdResult = RestPostRequestIntegration.APICallGet<List<PaymentItemsResponse>>("", Url);
                if (prdResult == null)
                {
                    return responses;
                }
                if (prdResult.Count <= 0)
                {
                    return responses;
                }
                foreach (var item in prdResult)
                {
                    responses.Add(new DropdownResponse() { Name = item.Name, Code = item.Code, Amount=item.Amount , ParentCode=item.BillerId});
                }
                return responses;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchAllAccountOfficer()
        {
            string methodname = "FetchOfficer";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {
                string Url = $"{AppConfig.CoreBankingBaseUrl}AccountOfficer/Get/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&mfbCode={BaseService.GetAppSetting("BankOneCode")}";
                var prdResult = RestPostRequestIntegration.APICallGet<List<BankModel>>("", Url);
                if (prdResult == null)
                {
                    return responses;
                }
                if (prdResult.Count <= 0)
                {
                    return responses;
                }
                foreach (var item in prdResult)
                {
                    responses.Add(new DropdownResponse() { Name = item.Name, Code = item.Code });
                }
                return responses;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static BVNBaseResponse FetchBVN(string BVN)
        {
            string methodname = "FetchBVN";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {
                if (string.IsNullOrEmpty(BVN))
                {
                    return new BVNBaseResponse() { ResponseCode = "04", ResponseMessage = "please enter BVN", bvnDetails = new BvnDetails() { BVN = "", FirstName = "", DOB = "", LastName = "", OtherNames = "", phoneNumber = "" } };

                }
                var bvnRequest = new BVNRequest() { BVN = BVN, Token = BaseService.GetAppSetting("AuthToken") };

                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}account/bvn/getbvndetails";
                var bvnResult = RestPostRequestIntegration.APICall<BVNBaseResponse>(bvnRequest, Url);
                if (bvnResult == null)
                {
                     
                    return new BVNBaseResponse() { ResponseCode = "04", ResponseMessage = "no record found", bvnDetails = new BvnDetails() { BVN = "", FirstName = "", DOB = "", LastName = "", OtherNames = "", phoneNumber = "" } };
                }
                if (bvnResult.isBvnValid == false)
                {
                    bvnResult.ResponseCode = "04";
                    bvnResult.ResponseMessage = string.IsNullOrEmpty(bvnResult.ResponseMessage)?"Oppps! Validation Failed": bvnResult.ResponseMessage;
                    return bvnResult;
                }
                bvnResult.ResponseCode = "00";
                bvnResult.ResponseMessage = string.IsNullOrEmpty(bvnResult.ResponseMessage) ? "Successful" : bvnResult.ResponseMessage;
                return bvnResult;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                  return new BVNBaseResponse() { ResponseCode = "06", ResponseMessage = "System Error", bvnDetails = new BvnDetails() { BVN = "", FirstName = "", DOB = "", LastName = "", OtherNames = "", phoneNumber = "" } };

            }
        }
        public static List<DropdownResponse> FetchLGA()
        {
            string methodname = "FetchLGA";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.Lga.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID, Code = item.Code });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchLGAByState(string stateCode)
        {
            string methodname = "FetchLGA";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.Lga.Where(x => x.StateCode == stateCode).ToList();
                    if (result.Count > 0)
                    {
                        
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID, Code = item.Code });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static AccountListResponse FetchAccountByAccountNumber(string AccountNo)
        {
            string methodname = "FetchAccountByAccountNumber";            
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.Account.Where(x=>x.AccountNumber==AccountNo).FirstOrDefault();
                    //if (result.Count()<= 0)
                    //{
                        //call bankone
                        List<Account> accounts = new List<Account>();
                        Account acctModel = new Account();

                       // var acctModel = new AccountDetailsModel();

                        string Url = $"{AppConfig.CoreBankingBaseUrl}Customer/GetByAccountNo/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={AccountNo}&institutionCode={BaseService.GetAppSetting("BankOneCode")}";
                        var acctResult = RestPostRequestIntegration.APICallGet<Model.PortalModel.CustomerSpace.CustomerAndAccountResponse>("", Url);
                        if (acctResult == null)
                        {
                            return new AccountListResponse() { ResponseCode = "04", ResponseMessage = "No record" };
                        }

                    //db call
                    acctModel.Title = result?.Title ?? "";
                    acctModel.MiddleName = result?.MiddleName??"";
                    acctModel.MaritalStatus = result?.MaritalStatus??"";
                    acctModel.NIN = result?.NIN??"";
                    acctModel.IdentityNumber = result?.IdentityNumber??"";
                    acctModel.NOKName = result?.NOKName ?? "";
                    acctModel.NOKNo = result?.NOKNo ?? "";
                    acctModel.ReferralName = result?.ReferralName ?? "";
                    acctModel.SecurityQuestion = result?.SecurityQuestion ?? "";
                    acctModel.SecurityAnswer = result?.SecurityAnswer ?? "";

                    
                    //api call
                        acctModel.AccountName = acctResult.CustomerDetails.Name;
                        acctModel.AccountNumber = acctResult.Accounts[0].NUBAN;
                        acctModel.ProductCode = acctResult.Accounts[0].AccountType;                       
                        acctModel.MobileNo = acctResult.CustomerDetails.PhoneNumber;
                        acctModel.Status = acctResult.Accounts[0].AccountStatus;                                            
                        acctModel.Email = acctResult.CustomerDetails.Email;                     
                        acctModel.ResidentialAddress = acctResult.CustomerDetails.Address;
                        acctModel.BVN = acctResult.CustomerDetails.BVN;
                        acctModel.DOB = DateTime.Parse(acctResult.CustomerDetails.DateOfBirth);
                        acctModel.ResidentialLGA = acctResult.CustomerDetails.LocalGovernmentArea;
                        acctModel.ResidentialState = acctResult.CustomerDetails.State;
                        acctModel.AccountTier = int.Parse(acctResult?.Accounts[0]?.AccessLevel);
                        acctModel.AccountNumber = AccountNo;
                        acctModel.ReferralName = acctResult?.Accounts[0]?.Refree1CustomerID;
                        try
                        {
                            acctModel.Gender = acctResult.CustomerDetails.Gender == "Male" ? 1 : 0;
                            acctModel.FirstName = acctResult?.CustomerDetails?.Name.Split(',')[0] ?? "";
                            acctModel.LastName = acctResult?.CustomerDetails?.Name.Split(',')[1] ?? "";
                            acctModel.DateCreated = DateTime.Parse(acctResult?.Accounts[0]?.DateCreated);
                        }
                        catch (Exception)
                        {

                        }
                      

                        accounts.Add(acctModel);

                        return new AccountListResponse() { ResponseCode = "00", ResponseMessage = "Successful", Accounts = accounts };
                    //}
                    //return new AccountListResponse() { ResponseCode = "00", ResponseMessage = "Successful", Accounts=result.ToList()};

                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new AccountListResponse() { ResponseCode = "06", ResponseMessage = "System Error" };
            }
        }

        public static AccountListResponse FetchAllAccounts()
        {
            string methodname = "FetchAccounts";          
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.Account.OrderByDescending(x=>x.ID).Take(1000).ToList();
                    if (result.Count > 0)
                    {
                        return new AccountListResponse() { ResponseCode = "00", ResponseMessage = "Accounts Loaded Successfully", Accounts=result };
                    }
                    return new AccountListResponse() { ResponseCode = "04", ResponseMessage = "No record" };

                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new AccountListResponse() { ResponseCode = "06", ResponseMessage = "System Error" };
            }
        }            

        public static Response CreateRegion(RegionCreationRequest request)
        {
            string methodname = "CreateRegion";
            try
            {
                var region = new Region()
                {
                    Name = request.Name,
                    Description = request.Description
                };
                using (AiroPayContext context = new AiroPayContext())
                {
                    context.Region.Add(region);
                    context.SaveChanges();
                    return new Response() { ResponseCode = "00", ResponseDescription = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new Response() { ResponseCode = "06", ResponseDescription = "An error occurred while creating region, please contact admin" };
            }
        }

        public static Response CreateUserType(UserTypeCreationRequest request)
        {
            string methodname = "CreateUserType";
            try
            {
                var region = new UserType()
                {
                    Name = request.Name,
                    Description = request.Description,
                    IsAdmin = string.IsNullOrEmpty(request.IsAdmin) ? false : true
                };
                using (AiroPayContext context = new AiroPayContext())
                {
                    context.UserType.Add(region);
                    context.SaveChanges();
                    return new Response() { ResponseCode = "00", ResponseDescription = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new Response() { ResponseCode = "06", ResponseDescription = "An error occurred while creating region, please contact admin" };
            }
        }

        public static Response CreateUser(UserCreationRequest request)
        {
            string methodname = "CreateUser";
            try
            {
                var user = new Core.Model.Generic.User()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Gender = int.Parse(request.Gender),
                    MobileNo = request.MobileNo,
                    PassportUrl = request.PassportUrl,
                    Region = int.Parse(request.Region),
                    Role = int.Parse(request.Role),
                    //  Manager = int.Parse(request.Manager)
                };
                using (AiroPayContext context = new AiroPayContext())
                {
                    context.User.Add(user);
                    context.SaveChanges();
                    return new Response() { ResponseCode = "00", ResponseDescription = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new Response() { ResponseCode = "06", ResponseDescription = "An error occurred while creating region, please contact admin" };
            }
        }

        public static List<DropdownResponse> FetchRegion()
        {
            string methodname = "FetchRegion";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.Region.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchUserRole()
        {
            string methodname = "FetchUserRole";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.UserType.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID, Description = item.Description });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchUser4DropDown()
        {
            string methodname = "FetchUser4DropDown";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.User.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.FirstName + " " + item.LastName, Id = item.ID });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<Core.Model.Generic.User> FetchUser()
        {
            string methodname = "FetchUser";
            List<Core.Model.Generic.User> responses = new List<Core.Model.Generic.User>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    responses = context.User.ToList();

                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static BillsPaymentResponse SendBillsPayment(BillsPaymentRequest request)
        {
            string methodname = "FetchBillsCategory";
          
            try
            {
                using (AiroPayContext context=new AiroPayContext())
                {
                    var trxLog = new BillsPaymentTransaction();
                    trxLog.Amount = request.Amount;
                    trxLog.TransactionDate = DateTime.Now;
                    trxLog.TransactionType = request.BillerName;
                    trxLog.SourceAccount = request.AccountNumber;
                    trxLog.Beneficiary = request.CustomerID;

                    request.CustomerPhone = "08123232323";
                    request.Token = BaseService.GetAppSetting("AuthToken");
                    string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/Payment";
                    var billingResult = new ApiPostAndGet().UrlPost<BillsPaymentResponse>(Url, request);
                    if (billingResult == null)
                    {
                        trxLog.Status = "Failed";
                        trxLog.ThirdPartyResponseCode = "06";
                        trxLog.ThirdPartyResponseMessage = "Communication Error";
                        trxLog.isSuccessful = false;
                        context.BillsPaymentTransaction.Add(trxLog);
                        context.SaveChanges();
                        return new BillsPaymentResponse() { IsSuccessful = false, ResponseCode = "06", ResponseMessage = "Communication Error" };
                    }
                    if (billingResult.IsSuccessful == false)
                    {
                        trxLog.Status = "Failed";
                        trxLog.ThirdPartyResponseCode = billingResult.ResponseCode;
                        trxLog.ThirdPartyResponseMessage = billingResult.ResponseDescription;
                        trxLog.isSuccessful =  false;
                        context.BillsPaymentTransaction.Add(trxLog);
                        context.SaveChanges();
                        return new BillsPaymentResponse() { IsSuccessful = false, ResponseCode = "06", ResponseMessage = billingResult.ResponseMessage };
                    }
                    trxLog.Status = billingResult.ResponseMessage??"Successful";
                    billingResult.ResponseCode = string.IsNullOrEmpty(billingResult.ResponseCode) ? "00" : billingResult.ResponseCode;
                    trxLog.ThirdPartyResponseCode = billingResult.ResponseCode;
                    trxLog.ThirdPartyResponseMessage = billingResult.ResponseDescription;
                    trxLog.isSuccessful = billingResult.ResponseCode == "00" ? true : false;
                    context.BillsPaymentTransaction.Add(trxLog);
                    context.SaveChanges();
                    return billingResult;
                }
              
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BillsPaymentResponse() { IsSuccessful = false, ResponseCode = "06", ResponseMessage = "System Error" };
            }
        }

        public static List<BillsPaymentTransaction> GetBillsPaymentTrx()
        {
            string methodname = "GetBillsPaymentTrx";

            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.BillsPaymentTransaction.Take(500).OrderByDescending(x => x.ID);
                    return result.ToList();
                }

            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return null;
            }
        }

        public static ResponseModel StatementLog(TransactionRequest request)
        {
            try
            {
                var Log = new StatementRequestLog();
                using (AiroPayContext context = new AiroPayContext())
                {
                    Log.AccountNumber=request.AccountNumber;
                    Log.StartDate=request.StartDate;
                    Log.EndDate = request.EndDate;

                    var accountCheck = FetchAccountByAccountNumber(request.AccountNumber);
                    if (accountCheck.ResponseCode != "00")
                    {
                        Log.Status = "Failed";
                        Log.StatusName = "No Record Found";
                        context.StatementRequestLog.Add(Log);  context.SaveChanges();
                        return ResponseDictionary.GetCodeDescription(accountCheck.ResponseCode, accountCheck.ResponseMessage);
                    }
                    if (accountCheck.Accounts.Count() <= 0)
                    {
                        Log.Status = "Failed";
                        Log.StatusName = "Account Lookup Error"; context.StatementRequestLog.Add(Log); context.SaveChanges();
                        return ResponseDictionary.GetCodeDescription("06", "Account lookup error");
                    }
                    if (string.IsNullOrEmpty(accountCheck.Accounts.FirstOrDefault().Email))
                    {
                        Log.Status = "Failed";
                        Log.StatusName = "No Email Registered"; context.StatementRequestLog.Add(Log); context.SaveChanges();
                        return ResponseDictionary.GetCodeDescription("04", "Email not attached to account, please update record and retry this process again");
                    }
                    Log.Status = "Success";
                    Log.StatusName = "Logged for Notification"; context.StatementRequestLog.Add(Log); context.SaveChanges();
                    request.Email = accountCheck.Accounts.FirstOrDefault().Email;
                     BackgroundJob.Enqueue(() => GenerateAndSendStatement(request));
                    return ResponseDictionary.GetCodeDescription("00", "Request has been stored for processing, statement will be sent to your email in 10 seconds");
                }             

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GenerateAndSendStatement(TransactionRequest request)
        {
            try
            {
                TransactionBaseResponse transactionBaseResponse = new TransactionBaseResponse();
                //Get Account Details              
                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GenerateAccountStatement2/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={request.AccountNumber}&fromDate={request.StartDate}&toDate={request.EndDate}&isPdf=true&arrangeAsc=true&showSerialNumber=true&showTransactionDate=true&showReversedTransactions=false&showInstrumentNumber=true&institutionCode={BaseService.GetAppSetting("BankOneCode")}";
                var acctResult = new ApiPostAndGet().UrlGet<TransactionBaseResponse>(Url, "");
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.Message);
                }
                if (string.IsNullOrEmpty(acctResult.Message))
                {
                    return ResponseDictionary.GetCodeDescription("04", "no statement result");
                }
                string statementPath = $@"{BaseService.GetAppSetting("StatementPath")}\{request.AccountNumber}_Statement.pdf";

                File.WriteAllBytes(statementPath, Convert.FromBase64String(acctResult.Message));

                var isEmailSent = SendEmail(statementPath, request.Email);

                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static bool SendEmail(string path, string email)
        {
           
            try
            {
                SmtpClient mailServer = new SmtpClient("smtp.gmail.com", 587);
                mailServer.EnableSsl = true;

                mailServer.Credentials = new System.Net.NetworkCredential(BaseService.GetAppSetting("AiroEmail"), BaseService.GetAppSetting("AiroEmailPwd"));

                string from = BaseService.GetAppSetting("AiroEmail");
                string to = email;
                MailMessage msg = new MailMessage(from, to);
                msg.Subject = "AIROPAY STATEMENT REQUEST";
                msg.Body = "<strong> Dear Customer </strong>,<br/>  <br/> Thank you for banking with us. Please find attached a copy of your statement request. <br/><br/> Kindly use any of our digital channels for your transaction.";
                //msg.Body += "<br/><br/> <strong>USSD: *7046#</strong>";
                msg.Body += "<br/><br/> <strong>Download our AiroPay App on Android store and IOS strore</strong>";
                msg.Body += "<br/><br/> contact mailto:helpdesk@airopay.com for further enquiries";
                msg.Body += "<br/><br/> Warm Regards ";
                msg.Attachments.Add(new System.Net.Mail.Attachment(path));
                msg.IsBodyHtml = true;
                mailServer.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return false;
            }
        }

        //public static List<DropdownResponse> FetchAccountByAccountNo(string AccountNo)
        //{
        //    string methodname = "FetchAccountByAccountNo";
        //    List<DropdownResponse> responses = new List<DropdownResponse>();
        //    try
        //    {
        //        string Url = $"{AppConfig.CoreBankingBaseUrl}AccountOfficer/Get/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&mfbCode={BaseService.GetAppSetting("BankOneCode")}";
        //        var prdResult = RestPostRequestIntegration.APICallGet<List<BankModel>>("", Url);
        //        if (prdResult == null)
        //        {
        //            return responses;
        //        }
        //        if (prdResult.Count <= 0)
        //        {
        //            return responses;
        //        }
        //        foreach (var item in prdResult)
        //        {
        //            responses.Add(new DropdownResponse() { Name = item.Name, Code = item.Code });
        //        }
        //        return responses;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMachine.LogInformation(countrycode, classname, methodname, ex);
        //        return responses;
        //    }
        //}
    }
}
