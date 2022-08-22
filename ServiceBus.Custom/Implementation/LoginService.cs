using Hangfire;
using ServicBus.Logic.Contracts;
using ServicBus.Logic.Implementations.Security;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.Custom.Contract;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Implementation
{
    public class LoginService : ILoginService
    {
        string classname = "LoginService";
        IAccountValidationService accountValidation;
        public LoginService(IAccountValidationService service)
        {
            accountValidation = service;
        }

        public LoginService()
        {

        }
        public ResponseModel AuthenticateAccount(string username, string password,string DeviceId)
        {
            try
            {
                string methodname = "AuthenticateAccount";
                LogMachine.LogInformation(classname, methodname, $"entered authicate account");
                //compare users details 
                using (AiroPayContext context = new AiroPayContext())
                {
                    LogMachine.LogInformation(classname, methodname, $"checking against DB");
                    var account = context.Account.Where(x => x.Email == username || x.AccountNumber==username).First();

                    if (account == null)
                    {
                        LogMachine.LogInformation(classname, methodname, $"no record found");
                        return ResponseDictionary.GetCodeDescription("04", "invalid username");
                    }
                    if (account.Status=="1" && !string.IsNullOrEmpty(account.CustomerId))
                    {
                        LogMachine.LogInformation(classname, methodname, $"account is yet to be created, customer has been created");
                        return ResponseDictionary.GetCodeDescription("04", "Account creation in progress, please try again in five minutes");
                    }
                    if (account.Password == password)
                    {
                        LogMachine.LogInformation(classname, methodname, $"matching password");
                        if (account.DeviceID== DeviceId)
                        {
                            LogMachine.LogInformation(classname, methodname, $"device id matched, calling corebanking to get all accounts");
                            return accountValidation.GetAccountByAccountNo(account.AccountNumber);
                        }
                        ///device not recognized
                        return ResponseDictionary.GetCodeDescription("100", $"New Device Recognized, Last device was a(n) {account.DeviceName}");

                    }
                    return ResponseDictionary.GetCodeDescription("04", "invalid password");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Malfunction");
            }
        }

        public ResponseModel RefreshAccount(string accountNo)
        {
            try
            {
                string methodname = "AuthenticateAccount";
                LogMachine.LogInformation(classname, methodname, $"entered authicate account");
                //compare users details 
                
              return accountValidation.GetAllAccountsByAccountNo(accountNo, "");
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
        public ResponseModel AuthenticateUser(string username, string password)
        {
            try
            {
                //compare users details 
                using (AiroPayContext context=new AiroPayContext())
                {
                    var account = context.User.Where(x=>x.UserName==username || x.Email==username).FirstOrDefault();

                    if (account==null)
                    {
                        return ResponseDictionary.GetCodeDescription("04","invalid username");
                    }                   
                    if (account.Password==password)
                    {
                        
                        return ResponseDictionary.GetCodeDescription("00","login succesful", account);
                    }
                    return ResponseDictionary.GetCodeDescription("04","invalid password");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel ChangePassword(ChangePassword model)
        {
            try
            {
                //compare users details 
                using (AiroPayContext context = new AiroPayContext())
                {
                    var account = context.Account.Where(x => x.AccountNumber == model.username || x.Email==model.username).FirstOrDefault();

                    if (account == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "invalid username");
                    }
                    if (account.Password != model.oldpassword)
                    {
                        return ResponseDictionary.GetCodeDescription("06", "invalid old password");
                    }
                    
                    //check for token 
                        account.LastPassword = account.Password;
                        account.Password = model.newpassword;
                        context.SaveChanges();
                        return ResponseDictionary.GetCodeDescription("00", "Password change successsful");
                    

                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel ChangePin(ChangePinModel model)
        {
            try
            {
                //compare users details 
                using (AiroPayContext context = new AiroPayContext())
                {
                    var account = context.Account.Where(x => x.AccountNumber == model.username || x.Email==model.username).FirstOrDefault();

                    if (account == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "unable to chnage pin, internal error.username no not linked");
                    }
                    if (account.PIN != model.oldpin)
                    {
                        return ResponseDictionary.GetCodeDescription("06", "invalid old pin");
                    }
                    
                    //check for token
                        account.LastPIN = account.PIN;
                        account.PIN = model.newpin;
                        context.SaveChanges();
                        return ResponseDictionary.GetCodeDescription("00", "Password change successsful");
                    

                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel PasswordUpdate(PasswordUpdateModel model)
        {
            try
            {
                //compare users details 
                using (AiroPayContext context = new AiroPayContext())
                {
                    var account = context.Account.Where(x => x.AccountNumber == model.AccountNumber && x.MobileNo == model.MobileNumber).FirstOrDefault();

                    if (account == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "unable to chnage password, account no or mobile does not exists");
                    }
                    //check for token
                    account.Password = model.Password;                   
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00", "Password changed successsful");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                //compare users details 
                using (AiroPayContext context = new AiroPayContext())
                {
                    var account = context.Account.Where(x => x.AccountNumber == model.AccountNumber).FirstOrDefault();

                    if (account == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "unable to activate password reset, account number not found or registered");
                    }
                    //generate and save record
                    string tempPassword = Guid.NewGuid().ToString();
                    tempPassword = tempPassword.Substring(0, 6);
                    account.LastPassword = account.Password;
                    account.Password = new Cryptography().Encrypt(tempPassword);
                    var message = new Messaging();
                    message.Message = $"Dear Customer, Your temporary password is { tempPassword} , kindly login and change it immediatetely";
                    message.Subject = "Password Reset";
                    message.Email = account.Email;
                    message.Status = 0;

                    BackgroundJob.Enqueue(() => Messenger.SendEmail(message.Subject, message.Message, account.Email));

                    context.Messaging.Add(message);
                    context.SaveChanges();
                    //change password
                    return ResponseDictionary.GetCodeDescription("00","password reset activated, kindly check your email for details ");
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
    }
}
