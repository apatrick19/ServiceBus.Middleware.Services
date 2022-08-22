using ServicBus.Logic.Implementations;
using ServicBus.Logic.Implementations.IO.Image;
using ServicBus.Logic.Implementations.Memory;
using ServicBus.Logic.Implementations.SharePoint;
using ServiceBus.Core.Model.CRM;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.CRM.Implementation;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using ServiceBus.Nodes.Contract;
using ServiceBus.Nodes.Destination;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Logic.Implementations.Model;
using System.Drawing;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace ServiceBus.Jobs.Implementation
{
    public class JobProcessor
    {
        IExternalNode externalNode;
        public JobProcessor(IExternalNode external)
        {
            externalNode = external;
        }

        public JobProcessor()
        {

        }
        private static string GenerateSMS(string FName, string LName, string RSAPIN)
        {

            string message = $"Dear Client,welcome to ARM Pensions. Your RSA Pin is {RSAPIN}. You will be contacted by our representative on 012715000. Thank you for partnering with us";
            return message;
        }
        
        public static ResponseModel FlagAndBlockAccountsProcessor()
        {
            try
            {
                Trace.TraceInformation("inside Flag And Block Accounts Processor");
                //service starts and tries to pull data 
                List<FlagBlockResponseMapping> accounts = new List<FlagBlockResponseMapping>();
                FlaggedAccount account = new FlaggedAccount();
                Trace.TraceInformation("about calling client connector to spool records using fetch xml");
                var accountToFlag = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector()).FetchFlagAndBlockAccounts();
                Trace.TraceInformation($"gotten response from client connector {accountToFlag.ResponseCode??""}");
                using (ARMPContext context = new ARMPContext())
                {
                    if (accountToFlag.ResponseCode == "00")
                    {
                        if (accountToFlag.ResultObject != null)
                        {
                            Trace.TraceInformation("about casting object to list");
                            accounts = (List<FlagBlockResponseMapping>)accountToFlag.ResultObject;
                            if (accounts != null)
                            {
                                Trace.TraceInformation("about calling Navision ");
                                foreach (var item in accounts)
                                {
                                    #region Casting
                                    var customerRecord = new Customer();
                                    customerRecord.RSAPIN = item.RSAPIN;
                                    if (!string.IsNullOrEmpty(item.Blocked))
                                    {
                                        customerRecord.BlockedCode = int.Parse(InMemory.BlockedList.FirstOrDefault(x => x.GUID == item.Blocked).NavID);
                                    }
                                    customerRecord.FlagCode = item.FlagCode ?? "";
                                    #endregion

                                    #region DatabaseCheck
                                    var existingRecord = context.FlaggedAccount.Where(x => x.RSAPIN == customerRecord.RSAPIN && x.DateFlagged==DateTime.Today && x.isSuccess==true).FirstOrDefault();
                                    if (existingRecord == null)
                                    {
                                        account.RSAPIN = customerRecord.RSAPIN;
                                        account.FlagCode = customerRecord.FlagCode;
                                        account.BlockedCode = customerRecord.BlockedCode;
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    #endregion


                                    var coresSystemResponse = new ExternalNode(new ApiPostAndGet(), new FileConverter()).FlagAndBlockOnCore(customerRecord);
                                    if (coresSystemResponse.ResponseCode == "00")
                                    {
                                        Trace.TraceInformation($"successfully updated navision with ...end of process for this account, running next account");
                                        account.isSuccess = true;
                                        account.DateFlagged = DateTime.Today;
                                        context.FlaggedAccount.Add(account);
                                    }
                                    Trace.TraceInformation($"could not update navision with employee flag and block code");
                                }
                                context.SaveChanges();
                                Trace.TraceInformation($"successfully iterated through all accounts in process and updated navision");
                                return ResponseDictionary.GetCodeDescription("00");

                            }

                        }
                        Trace.TraceInformation($"about returning with no record.");
                        return ResponseDictionary.GetCodeDescription("04");
                    }
                    
                }

                return ResponseDictionary.GetCodeDescription("96");
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
        public static ResponseModel PinGenerationEmail()
        {
            try
            {
                if (string.IsNullOrEmpty(InMemory.EmailTemplate))
                {
                    throw new Exception("Email template is missing");
                }

                using (ARMPContext context = new ARMPContext())
                {
                    ResponseModel emailObject = new ResponseModel(); List<Email> EMailList = new List<Email>();
                    string filePath = string.Empty, downloadedFile = string.Empty, fileConverted = string.Empty, messageToSend = string.Empty;
                    var accountsToProcess = context.Customer.Where(x => x.IsMembershipGenerated == false && x.IsAccountOnCore == true && x.MembershipCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No membership certificate to process");
                    }

                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrWhiteSpace(item.Email) || string.IsNullOrEmpty(item.RSAPIN))
                        {
                            continue;
                        }
                        filePath = $@"{AppConfig.FolderManager}\{item.RSAPIN}MembershipCertificate.pdf";
                        downloadedFile = DownloadManager.DownloadRemotesite($"{AppConfig.MembershipUrl}/{item.ID}", filePath);
                        if (string.IsNullOrWhiteSpace(downloadedFile))
                        {
                            continue;
                        }
                        fileConverted = FileConverter.ConvertFiletoBase64(downloadedFile);
                        if (string.IsNullOrEmpty(fileConverted))
                        {
                            continue;
                        }
                        messageToSend = InMemory.EmailTemplate.Replace("$", item.LastName); messageToSend = messageToSend.Replace("#", item.FirstName);
                        emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMailAndAttachement
                            (
                             item.Email, $"{item.LastName ?? ""} {item.FirstName ?? ""}"
                             , messageToSend, "Welcome to ARM Pension", fileConverted, $"{item.RSAPIN} Membership Certificate", "application/pdf"
                            );
                        if (emailObject.ResponseCode == "03" || emailObject.ResponseCode == "00")
                        {
                            var EmailResponse = (EmailResponse)emailObject.ResultObject;
                            item.IsSMSSent = true;
                            EMailList.Add(new Email() { Destination = item.Email, DateCreated = DateTime.UtcNow, EmailId = EmailResponse._id, Message = messageToSend, Status = EmailResponse.status, Subject = "Welcome to arm pension", RejectReason = "" });
                        }
                    }
                    context.Email.AddRange(EMailList);
                    context.SaveChangesAsync();
                    return emailObject;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
        public static ResponseModel PinGenerationSMS()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    var accountsToProcess = context.Customer.Where(x => x.IsSMSSent == false && x.IsAccountOnCore==true).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No membership certificate to process");
                    }
                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrWhiteSpace(item.MobileNumber))
                        {
                            continue;
                        }
                        if (item.MobileNumber.Length==11 && item.MobileNumber.StartsWith("0"))
                        {
                            item.MobileNumber = $"234{item.MobileNumber.Substring(1, 10)}";
                        }
                        var smsStatus= new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendSMS(item.MobileNumber, GenerateSMS(item.FirstName, item.LastName, item.RSAPIN));
                        if (smsStatus.ResponseCode=="00")
                        {
                            item.IsSMSSent = true;
                            context.SaveChangesAsync();
                        }
                    }
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
              Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel DataRecaptureSMS()
        {
            Trace.TraceInformation("inside data recapture sms generation");
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    var accountsToProcess = context.CustomerDatarecapture.Where(x => x.IsSMSSent == false && x.IsAccountOnCore == true).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No membership certificate to process");
                    }
                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrWhiteSpace(item.MobileNumber))
                        {
                            continue;
                        }
                        if (item.MobileNumber.Length == 11 && item.MobileNumber.StartsWith("0"))
                        {
                            item.MobileNumber = $"234{item.MobileNumber.Substring(1, 10)}";
                        }
                        var SmsMessage = File.ReadAllText($@"{AppConfig.DataRecaptureSMS}");                        
                        SmsMessage = SmsMessage.Replace("$", item.LastName);
                        SmsMessage = SmsMessage.Replace("#", item.FirstName);
                        Trace.TraceInformation($"about sending data recapture sms {SmsMessage}");
                        var smsStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendSMS(item.MobileNumber, SmsMessage);
                        if (smsStatus.ResponseCode == "00")
                        {
                            item.IsSMSSent = true;                          
                        }
                    }
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel RecordUpdateSMS()
        {
            Trace.TraceInformation("inside record update sms generation");
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    var accountsToProcess = context.CustomerRecordUpdate.Where(x => x.IsSMSSent == false && x.IsAccountOnCore == true).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No membership certificate to process");
                    }
                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrWhiteSpace(item.MobileNumber))
                        {
                            continue;
                        }
                        if (item.MobileNumber.Length == 11 && item.MobileNumber.StartsWith("0"))
                        {
                            item.MobileNumber = $"234{item.MobileNumber.Substring(1, 10)}";
                        }
                        var SmsMessage = File.ReadAllText($@"{AppConfig.RecordUpdateSMS}");                      
                        SmsMessage = SmsMessage.Replace("$", item.LastName);
                        SmsMessage = SmsMessage.Replace("#", item.FirstName);
                        Trace.TraceInformation($"about sending record update sms {SmsMessage}");
                        var smsStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendSMS(item.MobileNumber, SmsMessage);
                        if (smsStatus.ResponseCode == "00")
                        {
                            item.IsSMSSent = true;                           
                        }
                    }
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel RecordUpdateEmailNotification()
        {
            Trace.TraceInformation("inside record update email generation");
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    var accountsToProcess = context.CustomerRecordUpdate.Where(x => x.IsEmailGenerated == false && x.IsAccountOnCore == true).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No membership certificate to process");
                    }
                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrWhiteSpace(item.Email))
                        {
                            continue;
                        }
                        if (!item.Email.Contains('@'))
                        {
                            continue;
                        }
                        var EmailMessage = InMemory.LoadGenericTemplate(AppConfig.RecordUpdateEmail);
                        EmailMessage = EmailMessage.Replace("$", item.LastName);
                        EmailMessage = EmailMessage.Replace("#", item.FirstName);
                        Trace.TraceInformation($"about sending record update email {EmailMessage}");
                        var smsStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(item.Email,$"{item.FirstName} {item.LastName}", EmailMessage, "Record Update Request");
                        if (smsStatus.ResponseCode == "00")
                        {
                            item.IsEmailGenerated = true;                           
                        }
                    }
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel DataRecaptureEmailNotification()
        {
            Trace.TraceInformation("inside data recapture email and slip  generation");
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    var accountsToProcess = context.CustomerDatarecapture.Where(x => x.IsEmailGenerated == false && x.IsAccountOnCore == true).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No membership certificate to process");
                    }
                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrWhiteSpace(item.Email))
                        {
                            continue;
                        }
                        if (!item.Email.Contains('@'))
                        {
                            continue;
                        }
                        var EmailMessage = InMemory.LoadGenericTemplate(AppConfig.DataRecaptureEmail);                      
                        EmailMessage = EmailMessage.Replace("$", item.LastName);
                        EmailMessage = EmailMessage.Replace("#", item.FirstName);
                        var file = DataRecaptureslipGeneration(item);
                        if (string.IsNullOrEmpty(file))
                        {
                            continue;
                        }
                        Trace.TraceInformation($"about sending data recapture email {EmailMessage}");
                        var smsStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMailAndAttachement(item.Email, $"{item.FirstName} {item.LastName}", EmailMessage, "Approved Data Recapture", file,"Data Recapture slip", "application/pdf");
                        if (smsStatus.ResponseCode == "00")
                        {
                            item.IsEmailGenerated = true;
                        }
                    }
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
            
        public static ResponseModel GetECRSPinGenerationStatus()
        {
            try
            {
                List<ECRSResponse> MultipleECRS = new List<ECRSResponse>();
              Trace.TraceInformation("Entered pencome ECRS Status service ");
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                  Trace.TraceInformation("about looking for pending records  ");
                    var accountsToProcess = context.Customer.Where(x => x.IsPinGenerated == false && x.CustomerKycCategory != AppConfig.TpinCode && x.Status== "PENCOM_ACKNOWLEDGED" && x.StatusCount<AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                      Trace.TraceInformation("No record found for ECRS status");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                  Trace.TraceInformation($"Total record to  process for ECRS status {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                      Trace.TraceInformation($"Processing record in bits for ecrs generation {item.SetID} {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSPINStatus(item);
                                                 
                            customer = (Customer) pencomeStatus.ResultObject;
                            item.PencomResponseCode =string.IsNullOrEmpty(customer.PencomResponseCode)?"": customer.PencomResponseCode;
                            item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage)?"": customer.PencomResponseMessage;
                            
                            if (string.IsNullOrEmpty(customer.RSAPIN) || customer.RSAPIN=="null")
                            {                          
                            item.Status = "PENCOM_ACKNOWLEDGED";
                            item.RSAPIN = string.Empty;
                            }
                            else
                            {
                            item.RSAPIN =  customer.RSAPIN;
                            item.Status =  "PENCOM_APPROVED";
                            item.IsPinGenerated = true;
                            }
                          
                            item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN)? true:false;
                            if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                            {
                                var accountToUpdate = new CRMAccountResponseModel()
                                {
                                    CoreSystemResponse = "",
                                    PencomCode = item.PencomResponseCode,
                                    PencomResponse = item.PencomResponseMessage,
                                    RSAPIN = item.RSAPIN,
                                    CustomerID = item.CustomerID,
                                    MembershipStatus = false,
                                    SMSStatus = false,
                                    IsPinGenerated = item.IsPinGenerated,
                                    SetID=item.SetID
                                    
                                };                           
                                //CRM for updates
                                var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);
                            #region multiple ecrs 
                            var ecrsResponse = new ECRSResponse();
                            ecrsResponse.DateCreated = item.DateCreated;
                            ecrsResponse.DateReceived = DateTime.Now;
                            ecrsResponse.DateSent = DateTime.Now;
                            ecrsResponse.ReferenceNumber = item.CustomerID;
                            ecrsResponse.RequestType = "Pin Generation";
                            ecrsResponse.ResponseCode = item.PencomResponseCode;
                            ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                            ecrsResponse.RSAPIN = item.RSAPIN;
                            ecrsResponse.SetID = item.SetID;
                            MultipleECRS.Add(ecrsResponse);
                            #endregion
                        }
                        item.StatusCount = item.StatusCount + 1;
                        
                    }
                    context.ECRSResponse.AddRange(MultipleECRS);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
              Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GetPinGenerationStatus_BackUp()
        {
            try
            {
                List<ECRSResponse> MultipleECRS = new List<ECRSResponse>();
                Trace.TraceInformation("Entered pencome ECRS Status service ");
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("about looking for pending records  ");
                    var accountsToProcess = context.Customer.Where(x => x.IsPinGenerated == false && x.CustomerKycCategory != AppConfig.TpinCode && x.Status == "PENCOM_ACKNOWLEDGED").Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No record found for ECRS status");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"Total record to  process for ECRS status {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"Processing record in bits for ecrs generation {item.SetID} {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSPINStatus(item);

                        customer = (Customer)pencomeStatus.ResultObject;
                        item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                        item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                        if (string.IsNullOrEmpty(customer.RSAPIN) || customer.RSAPIN == "null")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                            item.RSAPIN = string.Empty;
                        }
                        else if (item.PencomResponseMessage != "Submitted for Processing" && item.PencomResponseMessage != "Account Acknowledged , Pending Pencom Confirmation" && item.PencomResponseMessage != "Accepted" && !string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            item.Status = "Open";
                        }      
                        else
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.Status = "PENCOM_APPROVED";
                            item.IsPinGenerated = true;
                        }

                        item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                        if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            var accountToUpdate = new CRMAccountResponseModel()
                            {
                                CoreSystemResponse = "",
                                PencomCode = item.PencomResponseCode,
                                PencomResponse = item.PencomResponseMessage,
                                RSAPIN = item.RSAPIN,
                                CustomerID = item.CustomerID,
                                MembershipStatus = false,
                                SMSStatus = false,
                                IsPinGenerated = item.IsPinGenerated,
                                SetID = item.SetID

                            };
                            //CRM for updates
                            var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);
                            #region multiple ecrs 
                            var ecrsResponse = new ECRSResponse();
                            ecrsResponse.DateCreated = item.DateCreated;
                            ecrsResponse.DateReceived = DateTime.Now;
                            ecrsResponse.DateSent = DateTime.Now;
                            ecrsResponse.ReferenceNumber = item.CustomerID;
                            ecrsResponse.RequestType = "Pin Generation";
                            ecrsResponse.ResponseCode = item.PencomResponseCode;
                            ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                            ecrsResponse.RSAPIN = item.RSAPIN;
                            ecrsResponse.SetID = item.SetID;
                            MultipleECRS.Add(ecrsResponse);
                            #endregion
                        }
                        item.StatusCount = item.StatusCount + 1;

                    }
                    context.ECRSResponse.AddRange(MultipleECRS);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel RealtimeECRSPinGenerationStatus(Customer item)
        {
            try
            {
                Trace.TraceInformation("Entered realtime pencome ECRS Status service ");
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    var ecrsResponse = new ECRSResponse();
                    ecrsResponse.DateCreated = item.DateCreated;
                    ecrsResponse.DateSent = DateTime.Now;
                    Trace.TraceInformation($"Processing record in bits for ecrs generation status customerid:{item.CustomerID}; setid: {item.SetID} ");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSPINStatus(item);

                        customer = (Customer)pencomeStatus.ResultObject;
                        item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                        item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                        if (string.IsNullOrEmpty(customer.RSAPIN) || customer.RSAPIN == "null")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                            item.RSAPIN = string.Empty;
                        }
                        else
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.Status = "PENCOM_APPROVED";
                            item.IsPinGenerated = true;
                        }

                        item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                        if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            var accountToUpdate = new CRMAccountResponseModel()
                            {
                                CoreSystemResponse = "",
                                PencomCode = item.PencomResponseCode,
                                PencomResponse = item.PencomResponseMessage,
                                RSAPIN = item.RSAPIN,
                                CustomerID = item.CustomerID,
                                MembershipStatus = false,
                                SMSStatus = false,
                                IsPinGenerated = item.IsPinGenerated,
                                SetID=item.SetID
                                
                            };
                            //CRM for updates
                            var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);
                        #region multiple ecrs 
                                       
                        ecrsResponse.DateReceived = DateTime.Now;                        
                        ecrsResponse.ReferenceNumber = item.CustomerID;
                        ecrsResponse.RequestType = "Pin Generation";
                        ecrsResponse.ResponseCode = item.PencomResponseCode;
                        ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                        ecrsResponse.RSAPIN = item.RSAPIN;
                        ecrsResponse.SetID = item.SetID;
                        context.ECRSResponse.Add(ecrsResponse);
                        #endregion
                    }
                    item.StatusCount = item.StatusCount + 1;
                      
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel BatchDataRecaptureECRSStatus()
        {
            try
            {
                List<ECRSResponse> MultipleECRS = new List<ECRSResponse>();
                Trace.TraceInformation("Entered pencome ECRS Status service ");
                CustomerDatarecapture customer = new CustomerDatarecapture();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("about looking for pending records  ");
                    var accountsToProcess = context.CustomerDatarecapture.Where(x => x.Status == "PENCOM_ACKNOWLEDGED" && x.StatusCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No record found for ECRS status");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"Total record to  process for ECRS status {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"Processing record in bits for ecrs generation {item.SetID} {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSDataRecaptureStatus(item);
                        if (pencomeStatus.ResponseCode == "01")
                        {
                            continue;
                        }
                        customer = (CustomerDatarecapture)pencomeStatus.ResultObject;
                        item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                        item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                        if (item.PencomResponseMessage == "PIN has already been recaptured;" || item.PencomResponseCode == "PD34U310SP04;")
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.Status = "PENCOM_APPROVED";
                            item.IsDataRecaptured = true;
                            //item.IsECRSProcessFailed = false;
                        }
                        else
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.Status = "PENCOM_ACKNOWLEGED";
                            item.IsDataRecaptured = false;
                           /// item.IsECRSProcessFailed = true;
                        }
                        item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                        if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            var accountToUpdate = new CRMAccountResponseModel()
                            {
                                CoreSystemResponse = "",
                                PencomCode = item.PencomResponseCode,
                                PencomResponse = item.PencomResponseMessage,
                                RSAPIN = item.RSAPIN,
                                CustomerID = item.CustomerID,
                                MembershipStatus = false,
                                SMSStatus = false,
                                IsDataRecaptured = item.IsDataRecaptured,
                                isERSProcessFailed = item.IsECRSProcessFailed,
                                SetID = item.SetID
                            };
                            //CRM for updates
                            var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateDataRecaptureWithResponse(accountToUpdate);
                            #region multiple ecrs 
                            var ecrsResponse = new ECRSResponse();
                            ecrsResponse.DateCreated = item.DateCreated;
                            ecrsResponse.DateReceived = DateTime.Now;
                            ecrsResponse.DateSent = DateTime.Now;
                            ecrsResponse.ReferenceNumber = item.CustomerID;
                            ecrsResponse.RequestType = "Data Recapture";
                            ecrsResponse.ResponseCode = item.PencomResponseCode;
                            ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                            ecrsResponse.RSAPIN = item.RSAPIN;
                            ecrsResponse.SetID = item.SetID;
                            MultipleECRS.Add(ecrsResponse);
                            #endregion
                        }
                        item.StatusCount = item.StatusCount + 1;

                    }
                    context.ECRSResponse.AddRange(MultipleECRS);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel RealtimeDataRecaptureStatus(CustomerDatarecapture item)
        {
            try
            {
                var ecrsResponse = new ECRSResponse();
                ecrsResponse.DateSent = DateTime.Now;
                ecrsResponse.DateCreated = DateTime.Now;
                Trace.TraceInformation("Entered realtime pencome ECRS Status service ");
                CustomerDatarecapture customer = new CustomerDatarecapture();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation($"Processing record in bits for ecrs generation status customerid:{item.CustomerID}; setid: {item.SetID} ");
                    var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSDataRecaptureStatus(item);
                   
                    customer = (CustomerDatarecapture)pencomeStatus.ResultObject;
                    item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                    item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                    if (item.PencomResponseMessage == "PIN has already been recaptured;" || item.PencomResponseCode == "PD34U310SP04;")
                    {
                        item.RSAPIN = customer.RSAPIN;
                        item.Status = "PENCOM_APPROVED";
                        item.IsDataRecaptured = true;
                        //item.IsECRSProcessFailed = false;
                    }
                    else
                    {
                        item.RSAPIN = customer.RSAPIN;
                        item.Status = "PENCOM_ACKNOWLEGED";
                        item.IsDataRecaptured = false;
                        //item.IsECRSProcessFailed = true;
                    }
                    item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                    if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                    {
                        var accountToUpdate = new CRMAccountResponseModel()
                        {
                            CoreSystemResponse = "",
                            PencomCode = item.PencomResponseCode,
                            PencomResponse = item.PencomResponseMessage,
                            RSAPIN = item.RSAPIN,
                            CustomerID = item.CustomerID,
                            MembershipStatus = false,
                            SMSStatus = false,
                            IsDataRecaptured = item.IsDataRecaptured,
                            isERSProcessFailed = item.IsECRSProcessFailed,
                            SetID = item.SetID
                        };
                        //CRM for updates
                        var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateDataRecaptureWithResponse(accountToUpdate);
                        #region multiple ecrs 
                        
                        ecrsResponse.DateReceived = DateTime.Now;
                       
                        ecrsResponse.ReferenceNumber = item.CustomerID;
                        ecrsResponse.RequestType = "Data Recapture";
                        ecrsResponse.ResponseCode = item.PencomResponseCode;
                        ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                        ecrsResponse.RSAPIN = item.RSAPIN;
                        ecrsResponse.SetID = item.SetID;
                        context.ECRSResponse.Add(ecrsResponse);
                        #endregion
                    }

                    item.StatusCount = item.StatusCount + 1;
                    
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel RealtimeRecordUpdateStatus(CustomerRecordUpdate item)
        {
            try
            {
                var ecrsResponse = new ECRSResponse();
                ecrsResponse.DateCreated = item.DateCreated;
                ecrsResponse.DateSent = DateTime.Now;
                Trace.TraceInformation("Entered realtime pencome ECRS Status service ");
                CustomerRecordUpdate customer = new CustomerRecordUpdate();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation($"Processing record in bits for ecrs generation status customerid:{item.CustomerID}; setid: {item.SetID} ");
                    var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSRecordUpdateStatus(item);

                    customer = (CustomerRecordUpdate)pencomeStatus.ResultObject;
                    item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                    item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                    if (item.PencomResponseMessage.Contains("successful"))
                    {
                        item.RSAPIN = customer.RSAPIN;
                        item.Status = "PENCOM_APPROVED";
                        item.IsRecordUpdateSuccessful = true;
                    }
                    else
                    {
                        item.RSAPIN = customer.RSAPIN;
                        item.Status = "PENCOM_ACKNOWLEGED";                       
                        //item.IsECRSProcessFailed = true;
                    }
                    item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                    if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                    {
                        var accountToUpdate = new CRMAccountResponseModel()
                        {
                            CoreSystemResponse = "",
                            PencomCode = item.PencomResponseCode,
                            PencomResponse = item.PencomResponseMessage,
                            RSAPIN = item.RSAPIN,
                            CustomerID = item.CustomerID,
                            MembershipStatus = false,
                            SMSStatus = false,
                            SetID = item.SetID,
                            IsRecordUpdateSuccessful = item.IsRecordUpdateSuccessful
                        };
                        //CRM for updates
                        var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateRecordUpdateWithResponse(accountToUpdate);
                        #region multiple ecrs 
                        
                        ecrsResponse.DateReceived = DateTime.Now;
                        ecrsResponse.ReferenceNumber = item.CustomerID;
                        ecrsResponse.RequestType = "Record Update";
                        ecrsResponse.ResponseCode = item.PencomResponseCode;
                        ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                        ecrsResponse.RSAPIN = item.RSAPIN;
                        ecrsResponse.SetID = item.SetID;
                        context.ECRSResponse.Add(ecrsResponse);
                        #endregion
                    }

                    item.StatusCount = item.StatusCount + 1;

                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel BatchRecordUpdateStatus()
        {
            try
            {
                List<ECRSResponse> MultipleECRS = new List<ECRSResponse>();
                Trace.TraceInformation("Entered pencome ECRS Status service for record update");
                CustomerRecordUpdate customer = new CustomerRecordUpdate();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("about looking for pending records  ");
                    var accountsToProcess = context.CustomerRecordUpdate.Where(x => x.Status == "PENCOM_ACKNOWLEDGED" && x.StatusCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No record found for ECRS status");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"Total record to  process for ECRS status {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"Processing record in bits for ecrs generation {item.SetID} {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSRecordUpdateStatus(item);
                        if (pencomeStatus.ResponseCode == "01")
                        {
                            continue;
                        }
                        customer = (CustomerRecordUpdate)pencomeStatus.ResultObject;
                        item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                        item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                        if (item.PencomResponseMessage.Contains("successful"))
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.Status = "PENCOM_APPROVED";
                            item.IsRecordUpdateSuccessful = true;
                        }
                        else
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.Status = "PENCOM_ACKNOWLEGED";
                            //item.IsECRSProcessFailed = true;
                        }
                        item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                        if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            var accountToUpdate = new CRMAccountResponseModel()
                            {
                                CoreSystemResponse = "",
                                PencomCode = item.PencomResponseCode,
                                PencomResponse = item.PencomResponseMessage,
                                RSAPIN = item.RSAPIN,
                                CustomerID = item.CustomerID,
                                MembershipStatus = false,
                                SMSStatus = false,
                                SetID = item.SetID,
                                IsRecordUpdateSuccessful = item.IsRecordUpdateSuccessful
                            };
                            //CRM for updates
                            var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateRecordUpdateWithResponse(accountToUpdate);
                            #region multiple ecrs 
                            var ecrsResponse = new ECRSResponse();
                            ecrsResponse.DateCreated = item.DateCreated;
                            ecrsResponse.DateReceived = DateTime.Now;
                            ecrsResponse.DateSent = DateTime.Now;
                            ecrsResponse.ReferenceNumber = item.CustomerID;
                            ecrsResponse.RequestType = "Record Update";
                            ecrsResponse.ResponseCode = item.PencomResponseCode;
                            ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                            ecrsResponse.RSAPIN = item.RSAPIN;
                            ecrsResponse.SetID = item.SetID;
                            context.ECRSResponse.Add(ecrsResponse);
                            #endregion
                        }

                        item.StatusCount = item.StatusCount + 1;
                    }
                    context.ECRSResponse.AddRange(MultipleECRS);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GetECRSTemoraryPinAccountStatus()
        {
            try
            {
                List<ECRSResponse> MultipleECRS = new List<ECRSResponse>();
                Trace.TraceInformation("Entered pencome ECRS TPIN Status service ");
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("about looking for pending records  ");
                    var accountsToProcess = context.Customer.Where(x => x.IsPinGenerated == false && x.CustomerKycCategory==AppConfig.TpinCode && x.Status == "PENCOM_ACKNOWLEDGED" && x.StatusCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No record found for ECRS status");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"Total record to  process for ECRS status {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"Processing record in bits for ecrs generation {item.SetID} {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSPINStatus(item);

                        customer = (Customer)pencomeStatus.ResultObject;
                        item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                        item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                        if (string.IsNullOrEmpty(customer.TPin) || customer.TPin == "null")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                            item.RSAPIN = string.Empty;
                        }
                        else
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.TPin = customer.TPin;
                            item.Status = "PENCOM_APPROVED";
                            item.IsPinGenerated = true;
                        }

                        item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                        if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            var accountToUpdate = new CRMAccountResponseModel()
                            {
                                CoreSystemResponse = "",
                                TPIN = item.TPin,
                                PencomCode = item.PencomResponseCode,
                                PencomResponse = item.PencomResponseMessage,
                                CustomerID = item.CustomerID,
                                MembershipStatus = false,
                                SMSStatus = false,
                                IsPinGenerated = item.IsPinGenerated,
                                isTpin = true,
                                SetID=item.SetID
                                
                            };
                            //CRM for updates
                            var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);
                            #region multiple ecrs 
                            var ecrsResponse = new ECRSResponse();
                            ecrsResponse.DateCreated = item.DateCreated;
                            ecrsResponse.DateReceived = DateTime.Now;
                            ecrsResponse.DateSent = DateTime.Now;
                            ecrsResponse.ReferenceNumber = item.CustomerID;
                            ecrsResponse.RequestType = "Temporary pin";
                            ecrsResponse.ResponseCode = item.PencomResponseCode;
                            ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                            ecrsResponse.RSAPIN = item.RSAPIN;
                            ecrsResponse.SetID = item.SetID;
                            MultipleECRS.Add(ecrsResponse);
                            #endregion
                        }
                        item.StatusCount = item.StatusCount + 1;
                    }
                    context.ECRSResponse.AddRange(MultipleECRS);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GetECRSTemoraryPinAccountStatus_Backup()
        {
            try
            {
                List<ECRSResponse> MultipleECRS = new List<ECRSResponse>();
                Trace.TraceInformation("Entered pencome ECRS TPIN Status service ");
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("about looking for pending records  ");
                    var accountsToProcess = context.Customer.Where(x => x.IsPinGenerated == false && x.CustomerKycCategory == AppConfig.TpinCode && x.Status == "PENCOM_ACKNOWLEDGED").Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No record found for ECRS status");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"Total record to  process for ECRS status {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"Processing record in bits for ecrs generation {item.SetID} {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).GetECRSPINStatus(item);

                        customer = (Customer)pencomeStatus.ResultObject;
                        item.PencomResponseCode = string.IsNullOrEmpty(customer.PencomResponseCode) ? "" : customer.PencomResponseCode;
                        item.PencomResponseMessage = string.IsNullOrEmpty(customer.PencomResponseMessage) ? "" : customer.PencomResponseMessage;

                        if (string.IsNullOrEmpty(customer.TPin) || customer.TPin == "null")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                            item.RSAPIN = string.Empty;
                        }
                        else if (item.PencomResponseMessage != "Submitted for Processing" && item.PencomResponseMessage != "Account Acknowledged , Pending Pencom Confirmation" && item.PencomResponseMessage != "Accepted" && !string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            item.Status = "Open";
                        }
                        else
                        {
                            item.RSAPIN = customer.RSAPIN;
                            item.TPin = customer.TPin;
                            item.Status = "PENCOM_APPROVED";
                            item.IsPinGenerated = true;
                        }

                        item.CRMNotified = !string.IsNullOrEmpty(customer.RSAPIN) ? true : false;
                        if (!string.IsNullOrEmpty(item.PencomResponseMessage))
                        {
                            var accountToUpdate = new CRMAccountResponseModel()
                            {
                                CoreSystemResponse = "",
                                TPIN = item.TPin,
                                PencomCode = item.PencomResponseCode,
                                PencomResponse = item.PencomResponseMessage,
                                CustomerID = item.CustomerID,
                                MembershipStatus = false,
                                SMSStatus = false,
                                IsPinGenerated = item.IsPinGenerated,
                                isTpin = true,
                                SetID = item.SetID

                            };
                            //CRM for updates
                            var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);
                            #region multiple ecrs 
                            var ecrsResponse = new ECRSResponse();
                            ecrsResponse.DateCreated = item.DateCreated;
                            ecrsResponse.DateReceived = DateTime.Now;
                            ecrsResponse.DateSent = DateTime.Now;
                            ecrsResponse.ReferenceNumber = item.CustomerID;
                            ecrsResponse.RequestType = "Temporary pin";
                            ecrsResponse.ResponseCode = item.PencomResponseCode;
                            ecrsResponse.ResponseMessage = item.PencomResponseMessage;
                            ecrsResponse.RSAPIN = item.RSAPIN;
                            ecrsResponse.SetID = item.SetID;
                            MultipleECRS.Add(ecrsResponse);
                            #endregion
                        }
                        item.StatusCount = item.StatusCount + 1;
                    }
                    context.ECRSResponse.AddRange(MultipleECRS);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }        

        public static ResponseModel GenerateAttestationandMoveKyc()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running customer attestation and base 64 copy  mechanism"); 
                    var accountsToProcess = context.Customer.Where(x => x.IsFileConverted == false && x.FileJobCount < AppConfig.TrialCount).Take(AppConfig.MinuteSequence).OrderByDescending(x => x.ID);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending record to copy or convert  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running ECRS file conversion schema total record to process {accountsToProcess.Count()}"); 
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"converting file  with form number {item.CustomerID}");

                        //CPS
                        if (item.CustomerKycCategory== "779560000")
                        {
                            string statename = "";
                            var accountdocument = context.AccountDocument.Where(x => x.Customerid == item.CustomerID).FirstOrDefault();
                            //  var clientdocument = context.Client.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                            if (accountdocument != null)
                            {
                                #region passport extraction
                                if (accountdocument.PassportUpload.Contains("data:image/png;base64,"))
                                {
                                    item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/png;base64,", "");
                                }
                                else if (accountdocument.PassportUpload.Contains("data:image/jpeg;base64,"))
                                {
                                    item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/jpeg;base64,", "");
                                }
                                #endregion
                                #region signature extraction
                                if (accountdocument.SignatureUpload.Contains("data:image/png;base64,"))
                                {
                                    item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/png;base64,", "");
                                }
                                else if (accountdocument.SignatureUpload.Contains("data:image/jpeg;base64,"))
                                {
                                    item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/jpeg;base64,", "");
                                }
                                #endregion
                                #region consent form conversion
                                if (!string.IsNullOrEmpty(item.StateOfResidence))
                                {
                                    statename = InMemory.StateList.FirstOrDefault(x => x.Code == item.StateOfResidence).Name;
                                }                                 
                                item.ConsentFormConverted = GenerateAttestationForm(accountdocument.SignatureUpload, item.CustomerID, $"{item.FirstName} {item.LastName} {item.OtherName}", $"{item.HouseNo}, {item.StreetName}, {statename}");
                                #endregion
                                if (!string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.ConsentFormConverted))
                                {
                                    Trace.TraceInformation("changing status for isfile  generated to true");
                                    item.IsFileConverted = true;
                                }
                            }
                        }
                        else if (item.CustomerKycCategory == "779560001")   // cross border employee
                        {
                            var accountdocument = context.CrossBorderAccountDocument.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                            //  var clientdocument = context.Client.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                            if (accountdocument != null)
                            {
                                #region passport extraction
                                if (accountdocument.PassportUpload.Contains("data:image/png;base64,"))
                                {
                                    item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/png;base64,", "");
                                }
                                else if (accountdocument.PassportUpload.Contains("data:image/jpeg;base64,"))
                                {
                                    item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/jpeg;base64,", "");
                                }
                                #endregion
                                #region signature extraction
                                if (accountdocument.SignatureUpload.Contains("data:image/png;base64,"))
                                {
                                    item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/png;base64,", "");
                                }
                                else if (accountdocument.SignatureUpload.Contains("data:image/jpeg;base64,"))
                                {
                                    item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/jpeg;base64,", "");
                                }
                                #endregion
                                #region consent form conversion
                                string statename = InMemory.StateList.FirstOrDefault(x => x.Code == item.StateOfResidence).Name;
                                item.ConsentFormConverted = GenerateAttestationForm(accountdocument.SignatureUpload, item.CustomerID, $"{item.FirstName} {item.LastName} {item.OtherName}", $"{item.HouseNo}, {item.StreetName}, {statename}");
                                #endregion
                                if (!string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.ConsentFormConverted))
                                {
                                    Trace.TraceInformation("changing status for isfile  generated to true");
                                    item.IsFileConverted = true;
                                }
                            }
                        }
                        else if (item.CustomerKycCategory == "779560002")
                        {
                            var accountdocument = context.MicroPensionsAccountDocument.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                            //  var clientdocument = context.Client.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                            if (accountdocument != null)
                            {
                                #region passport extraction
                                if (accountdocument.PassportUpload.Contains("data:image/png;base64,"))
                                {
                                    item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/png;base64,", "");
                                }
                                else if (accountdocument.PassportUpload.Contains("data:image/jpeg;base64,"))
                                {
                                    item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/jpeg;base64,", "");
                                }
                                #endregion
                                #region signature extraction
                                if (accountdocument.SignatureUpload.Contains("data:image/png;base64,"))
                                {
                                    item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/png;base64,", "");
                                }
                                else if (accountdocument.SignatureUpload.Contains("data:image/jpeg;base64,"))
                                {
                                    item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/jpeg;base64,", "");
                                }
                                #endregion
                                #region consent form conversion
                                string statename = InMemory.StateList.FirstOrDefault(x => x.Code == item.StateOfResidence).Name;
                                item.ConsentFormConverted = GenerateAttestationForm(accountdocument.SignatureUpload, item.CustomerID, $"{item.FirstName} {item.LastName} {item.OtherName}", $"{item.HouseNo}, {item.StreetName}, {statename}");
                                #endregion
                                if (!string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.ConsentFormConverted))
                                {
                                    Trace.TraceInformation("changing status for isfile  generated to true");
                                    item.IsFileConverted = true;
                                }
                            }
                        }


                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully converted all files");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GenerateAttestationandMoveDatarecaptureKyc()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running customer attestation for data recapture and base 64 copy  mechanism");
                    var accountsToProcess = context.CustomerDatarecapture.Where(x =>  x.IsFileConverted == false && x.FileJobCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence).OrderByDescending(x => x.ID);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending record to copy or convert  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running ECRS file conversion schema total record to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"converting file  with form number {item.CustomerID}");

                        var accountdocument = context.DataRecaptureDocument.Where(x => x.Rsapin == item.RSAPIN).FirstOrDefault();
                        //  var clientdocument = context.Client.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                        if (accountdocument != null)
                        {
                            #region passport extraction
                            if (accountdocument.PassportUpload.Contains("data:image/png;base64,"))
                            {
                                item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/png;base64,", "");
                            }
                            else if (accountdocument.PassportUpload.Contains("data:image/jpeg;base64,"))
                            {
                                item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/jpeg;base64,", "");
                            }
                            #endregion
                            #region signature extraction
                            if (accountdocument.SignatureUpload.Contains("data:image/png;base64,"))
                            {
                                item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/png;base64,", "");
                            }
                            else if (accountdocument.SignatureUpload.Contains("data:image/jpeg;base64,"))
                            {
                                item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/jpeg;base64,", "");
                            }
                            #endregion
                            #region consent form conversion
                            string statename = string.Empty;
                            if (!string.IsNullOrEmpty(item.StateOfResidence))
                            {
                                statename = InMemory.StateList.FirstOrDefault(x => x.Code == item.StateOfResidence).Name;
                            }                                                        
                            item.ConsentFormConverted = GenerateAttestationForm(accountdocument.SignatureUpload, item.CustomerID, $"{item.FirstName} {item.LastName} {item.OtherName}", $"{item.HouseNo}, {item.StreetName}, {statename}");
                            #endregion
                            if (!string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.ConsentFormConverted))
                            {
                                Trace.TraceInformation("changing status for isfile  generated to true");
                                item.IsFileConverted = true;
                            }

                        }
                        
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully converted all files");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// this method generates consent form 
        /// </summary>
        public static string GenerateAttestationForm(string SignatureFile, string CustomerID, string Name, string Address)
        {
            try
            {
                string htmlfile = System.IO.File.ReadAllText($@"{AppConfig.FolderManager}\Attestation.html");
                #region format html file
                htmlfile = htmlfile.Replace("{*}", CustomerID);
                htmlfile = htmlfile.Replace("{$}", Name);
                htmlfile = htmlfile.Replace("{#}", Address);
                htmlfile = htmlfile.Replace("{@}", SignatureFile);
                htmlfile = htmlfile.Replace("{%}", DateTime.UtcNow.ToString());
                #endregion
                System.Drawing.Image image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(htmlfile);
                string filepath = $@"{AppConfig.DocumentManager}\{CustomerID}.png";
                image.Save(filepath, ImageFormat.Png);
                string base64 = FileConverter.ConvertFiletoBase64(filepath);
                return base64;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred while generating consent form {ex.Message}; {ex.InnerException}");
                return string.Empty;
            }
        }

        public static string MembershipNoGeneration(MembershipModel model)
        {
            //DateTime NewDOB;

            Trace.TraceInformation("# 12 Job processor for membership certificate generation");
            try
            {
                if (model == null)
                {

                    Trace.TraceInformation("# 12 Job processor model is null, cannot pocess certificate");
                    return string.Empty;
                }
                //if (!string.IsNullOrEmpty(model.DOB))
                //{
                //    try
                //    {
                //        NewDOB = DateTime.Parse(model.DOB);
                //        NewDOB = NewDOB.AddDays(1);
                //        model.DOB = NewDOB.ToString();
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                //    }
                //}
                string pdfpath = $@"{AppConfig.DocumentManager}\{model.RSAPin} MembershipForm.pdf";
                Trace.TraceInformation("# 12 Job processor computing keywords and replacing");
                string registration = string.Empty;
                switch (model.IsChannelSource)
                {
                    case true:
                        registration = "DIGITAL";
                        break;
                    case false:
                        registration = "NON-DIGITAL";
                        break;                  
                }
                string htmlfile = System.IO.File.ReadAllText($@"{AppConfig.FolderManager}\CPSMembership.htm");
                #region format html file
                htmlfile = htmlfile.Replace("{!@}", model.Title);
                htmlfile = htmlfile.Replace("{!#}", model.FirstName);
                htmlfile = htmlfile.Replace("{!$}", model.SurName);
                htmlfile = htmlfile.Replace("{!%}", model.OtherName);
                htmlfile = htmlfile.Replace("{!^}", model.DOB);
                htmlfile = htmlfile.Replace("{!&}", model.MobileNo);
                htmlfile = htmlfile.Replace("{!*}", model.EmailAddress);
                htmlfile = htmlfile.Replace("{!0}", model.EmployerName);
                htmlfile = htmlfile.Replace("{!1}", model.EmployeeId);
                htmlfile = htmlfile.Replace("{!2}", model.RSAPin);
                htmlfile = htmlfile.Replace("{!3}", registration);
                htmlfile = htmlfile.Replace("{!4}", model.PassportUrl);
                #endregion
                Trace.TraceInformation("# 12 Job processor done with replacement about rendering as image");
          
               
                System.Drawing.Image image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(htmlfile);
                string imgpath = $@"{AppConfig.DocumentManager}\{model.RSAPin} MembershipForm.png";                
                image.Save(imgpath, ImageFormat.Jpeg);
                Trace.TraceInformation("# 12 Job processor image rendering done; about converting to pdf");
                Document document = new Document();
                using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var imageStream = new FileStream(imgpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var images = iTextSharp.text.Image.GetInstance(imageStream);
                        float maxWidth = document.PageSize.Width - 1- 1;
                        //float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        //float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;
                        float maxHeight = document.PageSize.Height - 1 - 1;
                        if (image.Height > maxHeight || image.Width > maxWidth)
                            images.ScaleToFit(maxWidth, maxHeight);
                        document.Add(images);
                    }
                    Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");
                    document.Close();
                }
                Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                return pdfpath;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for membership certificate generation {ex.Message} {ex}; {ex.StackTrace}");
                return string.Empty;
            }
        }

        public static string ReferenceLetterGeneration(ReferenceLettersModel model)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string pdfpath = string.Empty, htmlfile = string.Empty, filename=string.Empty;
            Trace.TraceInformation("# 14 Job processor for membership certificate generation");
            try
            {
                if (model == null)
                {
                    Trace.TraceInformation("# 14 Job processor model is null, cannot process letter");
                    return string.Empty;
                }
                string genderadjective = string.Empty;
                if (!string.IsNullOrEmpty(model.Gender))
                {
                    model.Gender = model.Gender.ToLower().Trim();
                    switch (model.Gender)
                    {
                        case "male":
                        case "Male":
                            model.Gender = "his";
                            genderadjective = "him";
                            break;
                        case "Female":
                        case "female":
                            model.Gender = "her";
                            genderadjective = "her";
                            break;
                        default:
                            model.Gender = "";
                            break;
                    }
                }
                if (model.isReferenceLetter==true)
                {
                    filename = $"{model.RSAPin} ReferenceLetter.pdf";
                    pdfpath = $@"{AppConfig.DocumentManager}\{filename}";     
                    htmlfile = System.IO.File.ReadAllText($@"{AppConfig.FolderManager}\Reference Letter.html");
                    #region format html file
                    htmlfile = htmlfile.Replace("{1}", model.ReferenceName??"");
                    htmlfile = htmlfile.Replace("{23}", model.RefCompanyName ?? "");
                    htmlfile = htmlfile.Replace("{2}", model.HouseNo??"");
                    htmlfile = htmlfile.Replace("{3}", model.Street??"");
                    htmlfile = htmlfile.Replace("{4}", model.City??"");
                    htmlfile = htmlfile.Replace("{5}", model.State??"");
                    htmlfile = htmlfile.Replace("{6}", model.Country??"");
                    htmlfile = htmlfile.Replace("{7}", model.SurName??"");
                    htmlfile = htmlfile.Replace("{8}", model.FirstName??"");
                    htmlfile = htmlfile.Replace("{9}", model.OtherName??"");
                    htmlfile = htmlfile.Replace("{10}",model.RSAPin??"");
                    htmlfile = htmlfile.Replace("{11}",model.Title??"");
                    htmlfile = htmlfile.Replace("{12}",model.UserName??"");
                    htmlfile = htmlfile.Replace("{13}",model.SupervisorName??"");
                    htmlfile = htmlfile.Replace("{14}", DateTime.Now.ToString("dd MMMM yyyy"));
                    htmlfile = htmlfile.Replace("{15}", model.UserSignatureUrl??"");
                    htmlfile = htmlfile.Replace("{16}", model.SupervisorSignature??"");
                    htmlfile = htmlfile.Replace("{16}", model.SupervisorSignature??"");
                    htmlfile = htmlfile.Replace("{17}", model.EmployerName??"");
                    htmlfile = htmlfile.Replace("{18}", model.Gender??"");
                    htmlfile = htmlfile.Replace("{19}", genderadjective ?? "");
                    if (!string.IsNullOrEmpty(model.SurName))
                    {
                        htmlfile = htmlfile.Replace("{20}", textInfo.ToTitleCase(model.SurName.ToLower() ?? "") ?? "");
                    }
                    if (!string.IsNullOrEmpty(model.FirstName))
                    {
                        htmlfile = htmlfile.Replace("{21}", textInfo.ToTitleCase(model.FirstName.ToLower() ?? "") ?? "");
                    }

                    if (!string.IsNullOrEmpty(model.OtherName))
                    {
                        htmlfile = htmlfile.Replace("{22}", textInfo.ToTitleCase(model.OtherName.ToLower() ?? "") ?? "");
                    }
                    else
                    {
                        htmlfile = htmlfile.Replace("{22}",  "");
                    }
                  
                    
                    #endregion
                }
                else if (model.isAccruedRightLetter==true)
                {
                    filename = $"{model.RSAPin} AccruedRightLetter.pdf";
                    pdfpath = $@"{AppConfig.DocumentManager}\{filename}";

                    htmlfile = System.IO.File.ReadAllText($@"{AppConfig.FolderManager}\Confirmation of Accrued Rights Letter.html");
                    #region format html file
                    htmlfile = htmlfile.Replace("{1}", DateTime.Now.ToString("dd MMMM yyyy"));
                    htmlfile = htmlfile.Replace("{2}", model.ReferenceName??"");
                    htmlfile = htmlfile.Replace("{3}", model.RefCompanyName??"");
                    htmlfile = htmlfile.Replace("{4}", model.HouseNo??"");
                    htmlfile = htmlfile.Replace("{5}", model.Street??"");
                    htmlfile = htmlfile.Replace("{6}", model.City??"");
                    htmlfile = htmlfile.Replace("{7}", model.State??"");
                    htmlfile = htmlfile.Replace("{8}", model.SurName??"");
                    htmlfile = htmlfile.Replace("{9}", model.FirstName??"");
                    htmlfile = htmlfile.Replace("{10}",model.OtherName??"");
                    htmlfile = htmlfile.Replace("{11}",model.RSAPin??"");
                    htmlfile = htmlfile.Replace("{12}",model.UserName??"");
                    htmlfile = htmlfile.Replace("{13}",model.SupervisorName??"");
                    htmlfile = htmlfile.Replace("{14}", model.UserSignatureUrl);
                    htmlfile = htmlfile.Replace("{15}", model.SupervisorSignature);
                    htmlfile = htmlfile.Replace("{16}", model.Gender ?? "");
                    #endregion
                }

                Trace.TraceInformation("# 12 Job processor done with replacement about rendering as image");
                System.Drawing.Image image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(htmlfile);
                string imgpath = pdfpath.Replace(".pdf","");
                 imgpath = $"{imgpath}.png";
                image.Save(imgpath, ImageFormat.Png);
                Trace.TraceInformation("# 12 Job processor image rendering done; about converting to pdf");
                Document document = new Document();
                using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var imageStream = new FileStream(imgpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var images = iTextSharp.text.Image.GetInstance(imageStream);
                       // float maxWidth = document.PageSize.Width - 2 - 2;
                        float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;
                        //float maxHeight = document.PageSize.Height - 2 - 2;
                        if (image.Height > maxHeight || image.Width > maxWidth)
                            images.ScaleToFit(maxWidth, maxHeight);
                        document.Add(images);
                    }
                    Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");
                    document.Close();
                }
                Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                return filename;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for membership certificate generation {ex.Message} {ex}; {ex.StackTrace}");
                throw ex;
            }
        }

        public static string NINAttestationGeneration(NINAttestationModel model)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string pdfpath = string.Empty, htmlfile = string.Empty, filename = string.Empty;
            Trace.TraceInformation("# 14 Job processor for nin certificate generation");
            try
            {
                if (model == null)
                {
                    Trace.TraceInformation("# 14 Job processor model is null, cannot process letter");
                    return string.Empty;
                }
                filename = $"{model.CustomerID}.pdf";
                pdfpath = $@"{AppConfig.DocumentManager}\{filename}";
                htmlfile = System.IO.File.ReadAllText($@"{AppConfig.FolderManager}\Attestation.html");
                #region format html file
                htmlfile = htmlfile.Replace("{*}", model.CustomerID);
                htmlfile = htmlfile.Replace("{$}", $"{model.FirstName} {model.LastName}");
                htmlfile = htmlfile.Replace("{#}", model.Address);
                htmlfile = htmlfile.Replace("{@}", model.Signature);
                htmlfile = htmlfile.Replace("{%}", DateTime.UtcNow.ToString());
                #endregion
                System.Drawing.Image image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(htmlfile);
                string filepath = $@"{AppConfig.DocumentManager}\{model.CustomerID}.png";
                string imgpath = pdfpath.Replace(".pdf", "");
                imgpath = $"{imgpath}.png";
                image.Save(filepath, ImageFormat.Png);
               
                Trace.TraceInformation("# 12 Job processor image rendering done; about converting to pdf");
                Document document = new Document();
                using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var imageStream = new FileStream(imgpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var images = iTextSharp.text.Image.GetInstance(imageStream);
                        // float maxWidth = document.PageSize.Width - 2 - 2;
                        float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;
                        //float maxHeight = document.PageSize.Height - 2 - 2;
                        if (image.Height > maxHeight || image.Width > maxWidth)
                            images.ScaleToFit(maxWidth, maxHeight);
                        document.Add(images);
                    }
                    Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");
                    document.Close();
                }
                Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                return filename;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for membership certificate generation {ex.Message} {ex}; {ex.StackTrace}");
                throw ex;
            }
        }

        public static string DataRecaptureslipGeneration(CustomerDatarecapture model)
        {
            //DateTime NewDOB;

            Trace.TraceInformation("# 12 Job processor for CustomerDatarecapture generation");
            try
            {
                if (model == null)
                {

                    Trace.TraceInformation("# 12 Job processor model is null, cannot pocess slip");
                    return string.Empty;
                }
                string Htmlpath = System.IO.File.ReadAllText($@"{AppConfig.DataRecaptureSlip}");
               string pdfpath = $@"{AppConfig.DocumentManager}\DataRecapture\{model.RSAPIN} DataRecapture Slip.pdf";  
               
                #region format html file
                //htmlfile = htmlfile.Replace("{!@}", model.Title);
                //htmlfile = htmlfile.Replace("{!#}", model.FirstName);
                //htmlfile = htmlfile.Replace("{!$}", model.SurName);
                //htmlfile = htmlfile.Replace("{!%}", model.OtherName);
                //htmlfile = htmlfile.Replace("{!^}", model.DOB);
                //htmlfile = htmlfile.Replace("{!&}", model.MobileNo);
                //htmlfile = htmlfile.Replace("{!*}", model.EmailAddress);
                //htmlfile = htmlfile.Replace("{!0}", model.EmployerName);
                //htmlfile = htmlfile.Replace("{!1}", model.EmployeeId);
                //htmlfile = htmlfile.Replace("{!2}", model.RSAPin);
                //htmlfile = htmlfile.Replace("{!3}", registration);
                //htmlfile = htmlfile.Replace("{!4}", model.PassportUrl);
                #endregion
                Trace.TraceInformation("# 12 Job processor done with replacement about rendering as image");


                System.Drawing.Image image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(Htmlpath);
                string imgpath = $@"{AppConfig.DocumentManager}\DataRecapture\{model.RSAPIN} DataRecapture Slip.jpg";
                image.Save(imgpath, ImageFormat.Jpeg);
                Trace.TraceInformation("# 12 Job processor image rendering done; about converting to pdf");
                Document document = new Document();
                using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var imageStream = new FileStream(imgpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var images = iTextSharp.text.Image.GetInstance(imageStream);
                        float maxWidth = document.PageSize.Width - 1 - 1;
                        //float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        //float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;
                        float maxHeight = document.PageSize.Height - 1 - 1;
                        if (image.Height > maxHeight || image.Width > maxWidth)
                            images.ScaleToFit(maxWidth, maxHeight);
                        document.Add(images);
                    }
                    Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");
                    document.Close();
                }
                Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                return pdfpath;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for membership certificate generation {ex.Message} {ex}; {ex.StackTrace}");
                return string.Empty;
            }
        }

        public ResponseModel ConvertDataRecapturefileUrlToBase64()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running DataRecapture file conversion mechanism"); Trace.TraceInformation("running customer file conversion mechanism");
                    var accountsToProcess = context.DataRecapture.Where(x => x.IsFileConverted == false).Take(5);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to convert  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running DataRecapture file conversion schema total record to process {accountsToProcess.Count()}"); Trace.TraceInformation($"running ECRS file conversion schema total record to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"converting DataRecapture   with form number {item.CustomerID}"); Trace.TraceInformation($"converting file  with form number  {item.CustomerID}");
                        //call anc convert files 
                        if (string.IsNullOrEmpty(item.SignatureConverted))
                        {
                            Trace.TraceInformation("converting signature ");
                            item.SignatureConverted = SharePointManager.DownLoadFileToBase64(item.SignatureUpload, $@"{AppConfig.FolderManager}\{item.FormNumber}_Signature.jpg");
                        }
                        if (string.IsNullOrEmpty(item.PassportConverted))
                        {
                            Trace.TraceInformation("converting passport ");
                            item.PassportConverted = SharePointManager.DownLoadFileToBase64(item.PassportUpload, $@"{AppConfig.FolderManager}\{item.FormNumber}_Passport.jpg");
                        }
                        if (string.IsNullOrEmpty(item.ConsentFormConverted))
                        {
                            Trace.TraceInformation("converting passport ");
                            item.ConsentFormConverted = SharePointManager.DownLoadFileToBase64(item.SignatureUpload, $@"{AppConfig.FolderManager}\{item.FormNumber}_Signature.jpg");
                        }
                        if (!string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.ConsentFormConverted))
                        {
                            Trace.TraceInformation("changing status for isfile  generated to true");
                            item.IsFileConverted = true;
                        }
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully converted all DataRecapture files");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"DataRecapture An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel ConvertemployerfileUrlToBase64()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                   Trace.TraceInformation("running employer file conversion mechanism");Trace.TraceInformation("running customer file conversion mechanism");
                    var accountsToProcess = context.Employer.Where(x => x.IsFileConverted == false).Take(5);
                    if (accountsToProcess.Count() <= 0)
                    {
                       Trace.TraceInformation("No pending file to convert  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                   Trace.TraceInformation($"running employer file conversion schema total record to process {accountsToProcess.Count()}");Trace.TraceInformation($"running employer file conversion schema total record to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                       Trace.TraceInformation($"converting file  with rc number {item.EmployerRCNumber}");Trace.TraceInformation($"converting file  with form number  {item.EmployerRCNumber}");
                        //call anc convert files 
                        if (string.IsNullOrEmpty(item.LetterOfRequestUpload))
                        {
                           Trace.TraceInformation("converting letter of request ");
                            item.ConvertedLetterOfRequestUpload = SharePointManager.DownLoadFileToBase64(item.LetterOfRequestUpload, $@"{AppConfig.FolderManager}\{item.EmployerRCNumber}_LetterOfRequestUpload.pdf");
                        }
                        if (string.IsNullOrEmpty(item.EvidenceOfTaxPayerUpload))
                        {
                           Trace.TraceInformation("converting passport ");
                            item.ConvertedEvidenceOfTaxPayerUpload = SharePointManager.DownLoadFileToBase64(item.EvidenceOfTaxPayerUpload, $@"{AppConfig.FolderManager}\{item.EmployerRCNumber}_EvidenceOfTaxPayerUpload.pdf");
                        }
                        if (string.IsNullOrEmpty(item.CertificateOfIncoporationUpload))
                        {
                           Trace.TraceInformation("converting passport ");
                            item.ConvertedCertificateOfIncoporationUpload = SharePointManager.DownLoadFileToBase64(item.CertificateOfIncoporationUpload, $@"{AppConfig.FolderManager}\{item.EmployerRCNumber}_CertificateOfIncoporationUpload.pdf");
                        }
                        if (!string.IsNullOrEmpty(item.ConvertedCertificateOfIncoporationUpload) && !string.IsNullOrEmpty(item.ConvertedEvidenceOfTaxPayerUpload) && !string.IsNullOrEmpty(item.ConvertedLetterOfRequestUpload))
                        {
                           Trace.TraceInformation("changing status for isfile  generated to true");
                            Trace.TraceInformation("changing status for isfile  generated to true");
                            item.IsFileConverted = true;
                        }
                    }
                    context.SaveChanges();
                   Trace.TraceInformation("successfully converted all files");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
               Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GenerateEmployerCSV()
        {
            try
            {
                //var tommorrow = DateTime.Today.AddDays(1);
                using (ARMPContext context = new ARMPContext())
                {
                   Trace.TraceInformation("running employer template generation");Trace.TraceInformation("running employer file mechanism");
                    var accountsToProcess = context.Employer.Where(x => x.Status == "Pending" && x.TrialCount<AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                       Trace.TraceInformation("No pending file to convert  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                   Trace.TraceInformation($"running employer file template  total record to process {accountsToProcess.Count()}");                    
                    List<EmployerModel> EmployerModelList = new List<EmployerModel>();
                    List<EmployerKYCModel> EmployerKYCModelList = new List<EmployerKYCModel>();

                    foreach (var item in accountsToProcess)
                    {
                       Trace.TraceInformation($"about generating template  with rc number {item.EmployerRCNumber}");

                        // file template generation 
                        EmployerModelList.Add(new EmployerModel()
                        {
                          EmployerEmail=item.EmployerEmail,
                           EmployerName=item.EmployerName,
                            EmployerNatureOfBusiness=item.EmployerNatureOfBusiness,
                             EmployerRCNumber=item.EmployerRCNumber,
                              EmployerSector=item.EmployerSector,
                               EmployerType=item.EmployerType,
                                RegisteredAddress=item.RegisteredAddress,
                                 TinNumber=item.TinNumber
                        });
                        EmployerKYCModelList.Add(new EmployerKYCModel()
                        {
                            EmployerName = item.EmployerName,                           
                            EmployerRCNumber = item.EmployerRCNumber,
                            LetterOfRequestUrl=item.LetterOfRequestUpload,
                            TaxIdentificationUrl=item.EvidenceOfTaxPayerUpload,
                            CertificateOfIncoprationUrl=item.CertificateOfIncoporationUpload
                        });

                        item.Status = "Approved";

                    }
                    string filePath = $@"{AppConfig.FolderManager}\EmployerGenFile_{DateTime.Now.ToString("dd-MM-yyyy hh-mm")}.csv";
                    string empKycPath = $@"{AppConfig.FolderManager}\EmployerGenKycFile_{DateTime.Now.ToString("dd-MM-yyyy hh-mm")}.csv";

                    Trace.TraceInformation($"4mtd calling employer file writer destination file {filePath ?? "empty"}");

                    var empResponse = new FileConverter().WriteToCsv<EmployerModel>(EmployerModelList, filePath);
                    var KycResponse = new FileConverter().KycWriteToCsv<EmployerKYCModel>(EmployerKYCModelList, empKycPath);
                    if (empResponse == true)
                    {
                        //convert file to base 64
                        string base64file = FileConverter.ConvertFiletoBase64(filePath);
                        string Kyc64File = FileConverter.ConvertFiletoBase64(empKycPath);
                       
                        Trace.TraceInformation("employer csv form generation successful about to send mail");

                        //sending mail
                        var messageToSend = "Hello All, <br/><br/> Please find attached our request for employer code issuance. <br/><br/> Kindly find attached the CSV file and corresponding share point links of new employer for your action. <br/><br/> Thank you.";

                        List<EmailFile> EmailFiles = new List<EmailFile>();
                        EmailFiles.Add(new EmailFile() {
                             File=base64file,
                             FileName ="EmployerCsvTemplate",
                             FileType ="application/csv"
                        });
                        EmailFiles.Add(new EmailFile()
                        {
                            File = Kyc64File,
                            FileName = "EmployerKycTemplate",
                            FileType = "application/csv"
                        });


                        var emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMultipleMailAndAttachement(AppConfig.EmployerDestEmail,"All", messageToSend, "CRM Employer Code Generation", EmailFiles);
                        if (emailObject.ResponseCode=="00")
                        {
                            Trace.TraceInformation("employer csv form generation mail has been send succesfully ");
                            context.SaveChanges();
                        }
                    }
                      return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
               Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GenerateEmployerCSV_Updated()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    string pdfpath = string.Empty;
                    Trace.TraceInformation("running employer template generation"); Trace.TraceInformation("running employer file mechanism");
                    var accountsToProcess = context.Employer.Where(x => x.Status == "Pending" && x.TrialCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to convert  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running employer file template  total record to process {accountsToProcess.Count()}");
                    List<EmployerModel> EmployerModelList = new List<EmployerModel>();
                    //List<EmployerKYCModel> EmployerKYCModelList = new List<EmployerKYCModel>();
                    string Directoryfolder = $@"{AppConfig.EmployerGenFolder}\{DateTime.Now.ToString("dd-MM-yyyy")}";
                    #region folder creation
                    //check if directory exist else create it 

                    if (Directory.Exists(Directoryfolder) == true)
                    {
                        //directory exists do nothin
                    }
                    else
                    {
                        //create directory
                        Directory.CreateDirectory(Directoryfolder);
                    }
                    #endregion
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"about generating template  with rc number {item.EmployerRCNumber}");
                        
                        #region pdf introduction                        
                        #region employer file check 
                        //if (string.IsNullOrEmpty(item.CertificateOfIncoporationUpload) || string.IsNullOrEmpty(item.EvidenceOfTaxPayerUpload) || string.IsNullOrEmpty(item.LetterOfRequestUpload))
                       // {
                            //copy files to another location
                            var filesFromDB = context.EmployerDocument.Where(x => x.CustomerID.Trim() == item.CustomerID.Trim()).FirstOrDefault();
                            if (filesFromDB==null)
                            {
                                continue;
                            }
                            else 
                            {
                                if (string.IsNullOrEmpty(filesFromDB.CertificateOfIncorporationUpload))
                                {
                                    Trace.TraceInformation($"skipping this employer record for file generation {item.CustomerID}");
                                    continue;
                                }
                                item.CertificateOfIncoporationUpload = filesFromDB.CertificateOfIncorporationUpload;
                                item.LetterOfRequestUpload = filesFromDB.EmployerRequestLetterUpload;
                                item.EvidenceOfTaxPayerUpload = filesFromDB.TINDocumentUpload;
                            }
                        //}
                        #endregion
                        pdfpath = $@"{Directoryfolder}\{item.CustomerID}_KycDocumentsform.pdf";

                     
                        string CACpath = FileConverter.ConverBase64toFile(item.CertificateOfIncoporationUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_CertificateOfIncoporationUpload");
                        string LORpath = FileConverter.ConverBase64toFile(item.LetterOfRequestUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_LetterOfRequestUpload");
                        string TINpath = FileConverter.ConverBase64toFile(item.EvidenceOfTaxPayerUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_EvidenceOfTaxPayerUpload");
                        Document document = new Document();
                        using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                            PdfWriter.GetInstance(document, stream);
                            document.Open();
                           
                                var CACimages = iTextSharp.text.Image.GetInstance(CACpath);
                                var LORimages = iTextSharp.text.Image.GetInstance(LORpath);
                                var TINimages = iTextSharp.text.Image.GetInstance(TINpath);
                            
                                float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                                float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;
                               
                                if (CACimages.Height > maxHeight || CACimages.Width > maxWidth) CACimages.ScaleToFit(maxWidth, maxHeight);
                                if (LORimages.Height > maxHeight || LORimages.Width > maxWidth) LORimages.ScaleToFit(maxWidth, maxHeight);
                                if (TINimages.Height > maxHeight || TINimages.Width > maxWidth) TINimages.ScaleToFit(maxWidth, maxHeight);
                                document.Add(CACimages);
                                document.Add(new Paragraph("Certificate Of Incoporation Upload", FontFactory.GetFont("Arial", 30)));
                                document.NewPage();
                                document.Add(LORimages);
                                document.Add(new Paragraph("Letter Of Request Upload", FontFactory.GetFont("Arial", 30)));
                                document.NewPage();
                                document.Add(TINimages);
                                document.Add(new Paragraph("Evidence of Tax Document Upload", FontFactory.GetFont("Arial", 30)));

                            Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");
                            document.Close();
                        }
                        Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                        //return pdfpath;


                        #endregion

                        // file template generation 
                        EmployerModelList.Add(new EmployerModel()
                        {
                            EmployerEmail = item.EmployerEmail,
                            EmployerName = item.EmployerName,
                            EmployerNatureOfBusiness = item.EmployerNatureOfBusiness,
                            EmployerRCNumber = item.EmployerRCNumber,
                            EmployerSector = item.EmployerSector,
                            EmployerType = item.EmployerType,
                            RegisteredAddress = item.RegisteredAddress,
                            TinNumber = item.TinNumber
                        });
                        item.Status = "Approved";

                    }
                    string filePath = $@"{Directoryfolder}\EmployerGenFile_{DateTime.Now.ToString("dd-MM-yyyy")}.csv";
                    //string empKycPath = $@"{AppConfig.FolderManager}\EmployerGenKycFile_{DateTime.Now.ToString("dd-MM-yyyy hh-mm")}.csv";

                    Trace.TraceInformation($"4mtd calling employer file writer destination file {filePath ?? "empty"}");

                    var empResponse = new FileConverter().WriteToCsv<EmployerModel>(EmployerModelList, filePath);
                    // var KycResponse = new FileConverter().KycWriteToCsv<EmployerKYCModel>(EmployerKYCModelList, empKycPath);
                    if (empResponse == true)
                    {
                        //convert file to base 64
                        string base64file = FileConverter.ConvertFiletoBase64(filePath);
                        // string Kyc64File = FileConverter.ConvertFiletoBase64(empKycPath);

                        Trace.TraceInformation("employer csv form generation successful about to send mail");
                        string mailUrl = $@"{AppConfig.KycUrl}EmployerGenFolder\{DateTime.Now.ToString("dd-MM-yyyy")}";

                        mailUrl = mailUrl.Substring(2, mailUrl.Length - 2);

                        mailUrl = $"file://///{mailUrl}";
                        //sending mail
                        //  var messageToSend = $"Hello Team, <br/><br/> Please find below Url path for today's employer code issuance request. <br/><br/><a href=\"///{mailUrl}\">Click Link</a><br/><br/> Thank you.";

                        var messageToSend = $"Hello All, <br/><br/> Please find attached our request for employer code issuance.<br/>The below URL contains the Location for CAC/TIN/Employer Form documents for your action.<br/><br/><a href=\"{mailUrl}\">Click Link</a><br/><br/>Kindly download the details attached<br/><br/><br/>Thank you."
                                             + "<br/><br/>ARM Pension Manager(PFA) Limited"
                                             + "<br/><br/>Tel: +234(1)2715005"
                                             + "<br/><br/>Email: dmulagos@armpension.com";

                        if (AppConfig.EmployerDestEmail.Contains(';'))
                        {
                            string[] recipients = AppConfig.EmployerDestEmail.Split(';');
                            if (recipients!=null)
                            {
                                foreach (var item in recipients.ToList())
                                {
                                    var emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(item, "All", messageToSend, "Request for employer code");
                                    if (emailObject.ResponseCode == "00")
                                    {
                                        Trace.TraceInformation("employer csv form generation mail has been send succesfully ");                                       
                                    }
                                }
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            var emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(AppConfig.EmployerDestEmail.Trim(), "All", messageToSend, "Request for employer code");
                            if (emailObject.ResponseCode == "00")
                            {
                                Trace.TraceInformation("employer csv form generation mail has been send succesfully ");
                                context.SaveChanges();
                            }
                        }
                       
                    }
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static string GenerateEmployerCPSPDF(string Directoryfolder, Employer item)
        {
            try
            {
                string pdfpath = $@"{Directoryfolder}\{item.CustomerID}_KycDocumentsform.pdf";
                string CACpath = FileConverter.ConverBase64toFile(item.CertificateOfIncoporationUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_CertificateOfIncoporationUpload");
                string LORpath = FileConverter.ConverBase64toFile(item.LetterOfRequestUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_LetterOfRequestUpload");
                string TINpath = FileConverter.ConverBase64toFile(item.EvidenceOfTaxPayerUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_EvidenceOfTaxPayerUpload");
                Document document = new Document();
                using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    var CACimages = iTextSharp.text.Image.GetInstance(CACpath);
                    var LORimages = iTextSharp.text.Image.GetInstance(LORpath);
                    var TINimages = iTextSharp.text.Image.GetInstance(TINpath);

                    float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                    float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;

                    if (CACimages.Height > maxHeight || CACimages.Width > maxWidth) CACimages.ScaleToFit(maxWidth, maxHeight);
                    if (LORimages.Height > maxHeight || LORimages.Width > maxWidth) LORimages.ScaleToFit(maxWidth, maxHeight);
                    if (TINimages.Height > maxHeight || TINimages.Width > maxWidth) TINimages.ScaleToFit(maxWidth, maxHeight);
                    document.Add(CACimages);
                    document.Add(new Paragraph("Certificate Of Incoporation Upload", FontFactory.GetFont("Arial", 30)));
                    document.NewPage();
                    document.Add(LORimages);
                    document.Add(new Paragraph("Letter Of Request Upload", FontFactory.GetFont("Arial", 30)));
                    document.NewPage();
                    document.Add(TINimages);
                    document.Add(new Paragraph("Evidence of Tax Document Upload", FontFactory.GetFont("Arial", 30)));

                    Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");
                    document.Close();
                }
                Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                return pdfpath;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occcurred on employer csv conversion {ex.Message}; {ex}");
                return string.Empty;
            }
        }
        public static string GenerateEmployerUpdatePDF(List<RecordUpdateDocumentRequest> Documents, EmployerRecordUpdate item, string Directoryfolder)
        {
            try
            {
                string pdfpath = $@"{Directoryfolder}\{item.CustomerID}_UpdateKycform.pdf";
                string path = string.Empty;
                Document document = new Document();
                using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    foreach (var doc in Documents)
                {
                        //extract the later path. 
                        path = $"{doc.ServerUrl.Substring(15, doc.ServerUrl.Length - 15)}";
                        path = $@"C:{path}";
                        Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                      
                        var Pathimages = iTextSharp.text.Image.GetInstance(path);
                       
                        float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;

                        if (Pathimages.Height > maxHeight || Pathimages.Width > maxWidth) Pathimages.ScaleToFit(maxWidth, maxHeight);
                        document.Add(Pathimages);
                        document.Add(new Paragraph(doc.DocumentName, FontFactory.GetFont("Arial", 30)));
                        document.NewPage(); 
                        Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");                        
                    }
                    document.Close();
                    Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                }
                return pdfpath;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occcurred on employer csv conversion {ex.Message}; {ex}");
                return string.Empty;
            }
        }
        public static string GenerateEmployerCPSCSV(string Directoryfolder, Employer item)
        {
            try
            {               
                var EmployerModelList = new List<EmployerModel>();
                var model=new EmployerModel()
                {
                    EmployerEmail = item.EmployerEmail,
                    EmployerName = item.EmployerName,
                    EmployerNatureOfBusiness = item.EmployerNatureOfBusiness,
                    EmployerRCNumber = item.EmployerRCNumber,
                    EmployerSector = item.EmployerSector,
                    EmployerType = item.EmployerType,
                    RegisteredAddress = item.RegisteredAddress,
                    TinNumber = item.TinNumber
                };
                EmployerModelList.Add(model);
                
                string filePath = $@"{Directoryfolder}\EmployerGenFile_{DateTime.Now.ToString("dd-MM-yyyy")}.csv";
               
                Trace.TraceInformation($"4mtd calling employer file writer destination file {filePath ?? "empty"}");

                var result= new FileConverter().WriteToCsv<EmployerModel>(EmployerModelList, filePath);
                if (result==true)
                {
                   return FileConverter.ConvertFiletoBase64(filePath);                   
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occcurred on employer csv generation {ex.Message}; {ex}");
                return string.Empty;
            }
        }
        public static string GenerateEmployerUpdateCSV(string Directoryfolder, EmployerRecordUpdate item)
        {
            try
            {
                var EmployerModelList = new List<EmployerUpdateModel>();
                var model = new EmployerUpdateModel()
                {
                    RCNumber = item.RCNumber,
                    EmployerCode = item.EmployerCode,
                    NewAddress = item.NewResidentialAddress,
                    OldAddress = item.OldResidentialAddress,
                    OldEmailAddress = item.OldEmailAddress,
                    NewEmailAddress = item.NewEmailAddress,
                    NewEmployerName = item.NewEmployerName,                   
                    OldEmployerName = item.OldEmployerName
                };
                EmployerModelList.Add(model);

                string filePath = $@"{Directoryfolder}\EmployerUpdateFile_{DateTime.Now.ToString("dd-MM-yyyy")}.csv";

                Trace.TraceInformation($"4mtd calling employer file writer destination file {filePath ?? "empty"}");

                var result = new FileConverter().WriteEmployerUpdateToCsv<EmployerUpdateModel>(EmployerModelList, filePath);
                if (result == true)
                {
                    return FileConverter.ConvertFiletoBase64(filePath);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occcurred on employer csv generation {ex.Message}; {ex}");
                return string.Empty;
            }
        }
        public static ResponseModel SendEmployerCSVEmail(string CustomerID)
        {
            try
            {
                //Send Email to DMU 
                Trace.TraceInformation("employer csv form generation successful about to send mail");
                string mailUrl = $@"{AppConfig.KycUrl}EmployerGenFolder\{DateTime.Today.ToString("dd-MM-yyyy")}\{CustomerID}";

                mailUrl = mailUrl.Substring(2, mailUrl.Length - 2);

                mailUrl = $"file://///{mailUrl}";
                //sending mail
                //  var messageToSend = $"Hello Team, <br/><br/> Please find below Url path for today's employer code issuance request. <br/><br/><a href=\"///{mailUrl}\">Click Link</a><br/><br/> Thank you.";

                var messageToSend = $"Hello All, <br/><br/> Please find attached our request for employer code issuance.<br/>The below URL contains the Location for CAC/TIN/Employer Form documents for your action.<br/><br/><a href=\"{mailUrl}\">Click Link</a><br/><br/>Kindly download the details attached<br/><br/><br/>Thank you."
                                     + "<br/><br/>ARM Pension Manager(PFA) Limited"
                                     + "<br/><br/>Tel: +234(1)2715005"
                                     + "<br/><br/>Email: dmulagos@armpension.com";

             
                if (AppConfig.EmployerDestEmail.Contains(';'))
                {
                    string[] recipients = AppConfig.EmployerDestEmail.Split(';');
                    if (recipients != null)
                    {
                        foreach (var recipient in recipients.ToList())
                        {
                            var emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(recipient, "All", messageToSend, "Request for employer code");
                            if (emailObject.ResponseCode == "00")
                            {
                                Trace.TraceInformation("employer csv form generation mail has been send succesfully ");
                            }
                        }
                        return ResponseDictionary.GetCodeDescription("00");
                    }
                }
                
                return new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(AppConfig.EmployerDestEmail.Trim(), "All", messageToSend, "Request for employer code");
                                 
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occcurred on employer csv generation {ex.Message}; {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SendEmployerUpdateRequestEmail(string CustomerID)
        {
            try
            {
                //Send Email to DMU 
                Trace.TraceInformation("employer update csv form generation successful about to send mail");
                string mailUrl = $@"{AppConfig.KycUrl}EmployerGenFolder\{DateTime.Today.ToString("dd-MM-yyyy")}\{CustomerID}";

                mailUrl = mailUrl.Substring(2, mailUrl.Length - 2);

                mailUrl = $"file://///{mailUrl}";
                //sending mail
                //  var messageToSend = $"Hello Team, <br/><br/> Please find below Url path for today's employer code issuance request. <br/><br/><a href=\"///{mailUrl}\">Click Link</a><br/><br/> Thank you.";

                var messageToSend = $"Hello All, <br/><br/> Please find attached our request for employer update request.<br/>The below URL contains the Location for the kyc Form documents for your action.<br/><br/><a href=\"{mailUrl}\">Click Link</a><br/><br/>Kindly download the details attached<br/><br/><br/>Thank you."
                                     + "<br/><br/>ARM Pension Manager(PFA) Limited"
                                     + "<br/><br/>Tel: +234(1)2715005"
                                     + "<br/><br/>Email: dmulagos@armpension.com";


                if (AppConfig.EmployerDestEmail.Contains(';'))
                {
                    string[] recipients = AppConfig.EmployerDestEmail.Split(';');
                    if (recipients != null)
                    {
                        foreach (var recipient in recipients.ToList())
                        {
                            var emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(recipient, "All", messageToSend, "CRM Employer Update Request");
                            if (emailObject.ResponseCode == "00")
                            {
                                Trace.TraceInformation("employer csv form generation mail has been send succesfully ");
                            }
                        }
                        return ResponseDictionary.GetCodeDescription("00");
                    }
                }

                return new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(AppConfig.EmployerDestEmail.Trim(), "All", messageToSend, "Request for employer code");

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occcurred on employer csv generation {ex.Message}; {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
        public static ResponseModel NewEmployerCreation_ECRS_CVS_RealTime(Employer item)
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    string pdfpath = string.Empty;       Trace.TraceInformation($"running employer template generation realtime from API call {item.CustomerID}");
                    List<EmployerModel> EmployerModelList = new List<EmployerModel>();
                    string Directoryfolder = $@"{AppConfig.EmployerGenFolder}\{DateTime.Today.ToString("dd-MM-yyyy")}\{item.CustomerID}";
                    #region folder creation
                    if (Directory.Exists(Directoryfolder) != true)
                    {
                        Directory.CreateDirectory(Directoryfolder);
                    }
                    
                    #endregion
                    #region employer file check 
                    //copy files to another location
                    var filesFromDB = context.EmployerDocument.Where(x => x.CustomerID.Trim() == item.CustomerID.Trim()).FirstOrDefault();
                    if (filesFromDB == null)
                    {
                        Trace.TraceInformation($"employer creation halted due to kyc unavailability {item.CustomerID}");
                        return ResponseDictionary.GetCodeDescription("03", "unable to process due to document unavailability");
                    }
                                       
                    if (string.IsNullOrEmpty(filesFromDB.CertificateOfIncorporationUpload))
                    {
                        //account is AES 
                        item.CustomerKYCCategory = "";
                    }
                    else
                    {
                        //account is CPS
                        item.CustomerKYCCategory = "";
                        item.CertificateOfIncoporationUpload = filesFromDB.CertificateOfIncorporationUpload;
                        item.LetterOfRequestUpload = filesFromDB.EmployerRequestLetterUpload;
                        item.EvidenceOfTaxPayerUpload = filesFromDB.TINDocumentUpload;
                    }

                    #endregion
                    //Convert kyc File into pdf
                    pdfpath = GenerateEmployerCPSPDF(Directoryfolder,item);
                    string CsvPath = GenerateEmployerCPSCSV(Directoryfolder, item);                  
                    if (!string.IsNullOrEmpty(CsvPath) && !string.IsNullOrEmpty(pdfpath))
                    {
                        return SendEmployerCSVEmail(item.CustomerID);
                    }                    
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
        public static ResponseModel EmployerRecordUpdate_ECRS_CVS_RealTime(EmployerRecordUpdate item)
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    string pdfpath = string.Empty; Trace.TraceInformation($"running employer template generation realtime from API call {item.CustomerID}");
                    List<EmployerModel> EmployerModelList = new List<EmployerModel>();
                    string Directoryfolder = $@"{AppConfig.DocumentManager}\EmployerUpdate\{DateTime.Today.ToString("dd-MM-yyyy")}\{item.CustomerID}";
                    #region folder creation
                    if (Directory.Exists(Directoryfolder) != true)
                    {
                        Directory.CreateDirectory(Directoryfolder);
                    }
                    #endregion
                    #region employer file check 
                    //copy files to another location
                    var filesFromDB = context.RecordUpdateDocumentRequest.Where(x => x.CustomerID.Trim() == item.CustomerID.Trim());
                    if (filesFromDB.Count() <= 0)
                    {
                        Trace.TraceInformation($"employer creation halted due to kyc unavailability {item.CustomerID}");
                        return ResponseDictionary.GetCodeDescription("03", "unable to process due to document unavailability");
                    }
                    pdfpath = GenerateEmployerUpdatePDF(filesFromDB.ToList(),item, Directoryfolder);
                    #endregion
                    //Convert kyc File into pdf

                    string CsvPath = GenerateEmployerUpdateCSV(Directoryfolder, item);
                    if (!string.IsNullOrEmpty(CsvPath) && !string.IsNullOrEmpty(pdfpath))
                    {
                        return SendEmployerUpdateRequestEmail(item.CustomerID);
                    }
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
        public static ResponseModel GenerateEmployerRecordUpdateCSV()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    string pdfpath = string.Empty;
                    Trace.TraceInformation("running employer record update template generation"); Trace.TraceInformation("running employer record update file mechanism");
                    var accountsToProcess = context.EmployerRecordUpdate.Where(x => x.Status == "Pending" && x.TrialCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to convert process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running employer record update file template total record to process {accountsToProcess.Count()}");
                    List<EmployerRecordUpdatePayload> EmployerRUModelList = new List<EmployerRecordUpdatePayload>();
                    //List<EmployerKYCModel> EmployerKYCModelList = new List<EmployerKYCModel>();
                    string Directoryfolder = $@"{AppConfig.EmployerGenFolder}\{DateTime.Now.ToString("dd-MM-yyyy")}";
                    #region folder creation
                    //check if directory exist else create it 

                    if (Directory.Exists(Directoryfolder) == true)
                    {
                        //directory exists, do nothing
                    }
                    else
                    {
                        //create directory
                        Directory.CreateDirectory(Directoryfolder);
                    }
                    #endregion
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"about generating template  with rc number {item.RCNumber}");

                        //#region pdf introduction                        
                        //#region employer file check 
                        ////if (string.IsNullOrEmpty(item.CertificateOfIncoporationUpload) || string.IsNullOrEmpty(item.EvidenceOfTaxPayerUpload) || string.IsNullOrEmpty(item.LetterOfRequestUpload))
                        //// {
                        ////copy files to another location
                        //var filesFromDB = context.EmployerDocument.Where(x => x.CustomerID.Trim() == item.CustomerID.Trim()).FirstOrDefault();
                        //if (filesFromDB == null)
                        //{
                        //    continue;
                        //}
                        //else
                        //{
                        //    if (string.IsNullOrEmpty(filesFromDB.CertificateOfIncorporationUpload))
                        //    {
                        //        Trace.TraceInformation($"skipping this employer record for file generation {item.CustomerID}");
                        //        continue;
                        //    }
                        //    item.CertificateOfIncoporationUpload = filesFromDB.CertificateOfIncorporationUpload;
                        //    item.LetterOfRequestUpload = filesFromDB.EmployerRequestLetterUpload;
                        //    item.EvidenceOfTaxPayerUpload = filesFromDB.TINDocumentUpload;
                        //}
                        ////}
                        //#endregion
                        //pdfpath = $@"{Directoryfolder}\{item.CustomerID}_KycDocumentsform.pdf";


                        //string CACpath = FileConverter.ConverBase64toFile(item.CertificateOfIncoporationUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_CertificateOfIncoporationUpload");
                        //string LORpath = FileConverter.ConverBase64toFile(item.LetterOfRequestUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_LetterOfRequestUpload");
                        //string TINpath = FileConverter.ConverBase64toFile(item.EvidenceOfTaxPayerUpload, $@"{AppConfig.FolderManager}\{item.CustomerID}_EvidenceOfTaxPayerUpload");
                        //Document document = new Document();
                        //using (var stream = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None))
                        //{
                        //    Trace.TraceInformation("# 12 Job processor pdf writer initialiazation and conversion running ");
                        //    PdfWriter.GetInstance(document, stream);
                        //    document.Open();

                        //    var CACimages = iTextSharp.text.Image.GetInstance(CACpath);
                        //    var LORimages = iTextSharp.text.Image.GetInstance(LORpath);
                        //    var TINimages = iTextSharp.text.Image.GetInstance(TINpath);

                        //    float maxWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        //    float maxHeight = document.PageSize.Height - document.TopMargin - document.BottomMargin;

                        //    if (CACimages.Height > maxHeight || CACimages.Width > maxWidth) CACimages.ScaleToFit(maxWidth, maxHeight);
                        //    if (LORimages.Height > maxHeight || LORimages.Width > maxWidth) LORimages.ScaleToFit(maxWidth, maxHeight);
                        //    if (TINimages.Height > maxHeight || TINimages.Width > maxWidth) TINimages.ScaleToFit(maxWidth, maxHeight);
                        //    document.Add(CACimages);
                        //    document.Add(new Paragraph("Certificate Of Incoporation Upload", FontFactory.GetFont("Arial", 30)));
                        //    document.NewPage();
                        //    document.Add(LORimages);
                        //    document.Add(new Paragraph("Letter Of Request Upload", FontFactory.GetFont("Arial", 30)));
                        //    document.NewPage();
                        //    document.Add(TINimages);
                        //    document.Add(new Paragraph("Evidence of Tax Document Upload", FontFactory.GetFont("Arial", 30)));

                        //    Trace.TraceInformation("# 12 Job processor model is null, successfully converted image to pdf returingin url");
                        //    document.Close();
                        //}
                        //Trace.TraceInformation("# 12 Job processor membership certificate generated, see file url");
                        ////return pdfpath;


                        //#endregion

                        // file template generation 
                        EmployerRUModelList.Add(new EmployerRecordUpdatePayload()
                        {
                            RCNumber = item.RCNumber,
                            EmployerCode = item.EmployerCode,
                            OldEmployerName = item.OldEmployerName,
                            OldResidentialAddress = item.OldResidentialAddress,
                            OldCountry = item.OldCountry,
                            OldState = item.OldState,
                            OldLGA = item.OldLGA,
                            OldEmailAddress = item.OldEmailAddress,
                            NewEmployerName = item.NewEmployerName,
                            NewResidentialAddress = item.NewResidentialAddress,
                            NewCountry = item.NewCountry,
                            NewState = item.NewState,
                            NewLGA = item.NewLGA,
                            NewEmailAddress = item.NewEmailAddress
                        });
                        item.Status = "Approved";

                    }
                    string filePath = $@"{Directoryfolder}\EmployerGenFile_{DateTime.Now.ToString("dd-MM-yyyy")}.csv";
                    //string empKycPath = $@"{AppConfig.FolderManager}\EmployerGenKycFile_{DateTime.Now.ToString("dd-MM-yyyy hh-mm")}.csv";

                    Trace.TraceInformation($"4mtd calling employer file writer destination file {filePath ?? "empty"}");

                    var empResponse = new FileConverter().WriteToCsv<EmployerRecordUpdatePayload>(EmployerRUModelList, filePath);
                    // var KycResponse = new FileConverter().KycWriteToCsv<EmployerKYCModel>(EmployerKYCModelList, empKycPath);
                    if (empResponse == true)
                    {
                        //convert file to base 64
                        string base64file = FileConverter.ConvertFiletoBase64(filePath);
                        // string Kyc64File = FileConverter.ConvertFiletoBase64(empKycPath);

                        Trace.TraceInformation("employer record update csv form generation successful about to send mail");
                        string mailUrl = $@"{AppConfig.KycUrl}EmployerGenFolder\{DateTime.Now.ToString("dd-MM-yyyy")}";

                        mailUrl = mailUrl.Substring(2, mailUrl.Length - 2);

                        mailUrl = $"file://///{mailUrl}";
                        //sending mail
                        //  var messageToSend = $"Hello Team, <br/><br/> Please find below Url path for today's employer code issuance request. <br/><br/><a href=\"///{mailUrl}\">Click Link</a><br/><br/> Thank you.";

                        var messageToSend = $"Hello All, <br/><br/> Please find attached details for an employer record update request.<br/>The below URL contains the location for the accompanying record update documents for your action.<br/><br/><a href=\"{mailUrl}\">Click Link</a><br/><br/>Kindly download the details attached<br/><br/><br/>Thank you."
                                             + "<br/><br/>ARM Pension Manager(PFA) Limited"
                                             + "<br/><br/>Tel: +234(1)2715005"
                                             + "<br/><br/>Email: dmulagos@armpension.com";

                        if (AppConfig.EmployerDestEmail.Contains(';'))
                        {
                            string[] recipients = AppConfig.EmployerDestEmail.Split(';');
                            if (recipients != null)
                            {
                                foreach (var item in recipients.ToList())
                                {
                                    var emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(item, "All", messageToSend, "Employer Record Update Request");
                                    if (emailObject.ResponseCode == "00")
                                    {
                                        Trace.TraceInformation("employer record update csv form generation mail has been sent succesfully ");
                                    }
                                }
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            var emailObject = new ExternalNode(new ApiPostAndGet(), new FileConverter()).SendMail(AppConfig.EmployerDestEmail.Trim(), "All", messageToSend, "Employer Record Update Request");
                            if (emailObject.ResponseCode == "00")
                            {
                                Trace.TraceInformation("employer record update csv form generation mail has been sent succesfully ");
                                context.SaveChanges();
                            }
                        }

                    }
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GenerateECRSPIN()
        {
            try
            {
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                  Trace.TraceInformation("running ECRS pin generation schema");                
                    var accountsToProcess = context.Customer.Where(x => x.Status ==  "Pending" && x.CustomerKycCategory != AppConfig.TpinCode && x.IsFileConverted==true && x.TrialCount<=AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                      Trace.TraceInformation("No pending ECRS account  to  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                  Trace.TraceInformation($"running ECRS pin generation schema total account to process {accountsToProcess.Count()}");          
                    foreach (var item in accountsToProcess)
                    {
                        if (item.CustomerKycCategory== "779560001")
                        {
                            item.NigeriaOrAbroad = "A";
                        }
                        else if (item.CustomerKycCategory == "779560002")
                        {
                            item.NigeriaOrAbroad = "N";
                            item.Sector = "MP";  
                            item.EmployerCode = item.EmployerIndustryCode;
                            item.EmployerAddress = item.Address;
                            item.EmployerPhone = item.MobileNumber;
                            item.LGAOfEmployer = item.LGAOfResidence;
                            item.EmployerCountry = item.CountryOfResidence;
                            item.StateOfEmployer = item.StateOfResidence;
                            item.EmployerCountry = item.CountryOfResidence;
                            item.EmployerTown = item.Town;
                            item.DateOfCurrentEmployment = null;
                            item.DateOfFirstEmployment = null;
                            if (!string.IsNullOrEmpty(item.EmployerIndustryCode))
                            {
                                var industry = InMemory.IndustryList.FirstOrDefault(x=>x.Reference==item.EmployerIndustryCode);
                                if (industry!=null)
                                {
                                    if (!string.IsNullOrEmpty(industry.Name))
                                    {
                                        if (industry.Name.Count() < 15)
                                        {
                                            item.EmployerIndustry = industry.Name;
                                        }
                                        else
                                        {
                                            item.EmployerIndustry = industry.Name.Substring(0,13);
                                        }
                                        
                                    }
                                  
                                }
                            }
                           
                        }
                        else
                        {
                            item.NigeriaOrAbroad = "N";
                        }
                      Trace.TraceInformation($"Processing account with form number {item.CustomerID}");                  
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).ECRSPencomPinGeneration(item);

                        if (pencomeStatus.ResponseCode=="06")
                        {
                            continue;
                        }
                        customer = (Customer)pencomeStatus.ResultObject;
                        item.PencomResponseCode = customer.PencomResponseCode;
                        item.PencomResponseMessage = customer.PencomResponseMessage;                     
                        item.SetID = customer.SetID;
                        item.IsPinGenerated = false;
                        if (item.PencomResponseCode=="0" && item.PencomResponseMessage== "Submitted for Processing")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                        }
                        else
                        {
                            item.Status = "Pending";  
                        }                       
                      Trace.TraceInformation($"pencom response for ECRS account form no {item.CustomerID};======> {item.PencomResponseMessage}======== {item.SetID??""}  status={item.Status}");          Trace.TraceInformation($"pencom response for ECRS account form no {item.CustomerID};======> {item.PencomResponseMessage}======== {item.SetID??""}");
                        var accountToUpdate = new CRMAccountResponseModel() {
                            CoreSystemResponse = "",
                            PencomCode = item.PencomResponseCode,
                            PencomResponse = item.PencomResponseMessage, RSAPIN = string.Empty,
                                CustomerID = item.CustomerID,
                                 MembershipStatus = false,
                                SMSStatus = false,
                               IsPinGenerated = false
                            };
                        //item.TrialCount = item.TrialCount + 1;
                        //CRM for updates
                      Trace.TraceInformation($"about updating CRM with ECRS pencom response pin generation for {item.CustomerID}");     
                        var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);
                        item.TrialCount = item.TrialCount + 1; 
                    }
                    context.SaveChanges();
                   Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
              Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");     Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel TPINAccountCreation()
        {
            try
            {
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running ECRS Tpin generation schema");
                    var accountsToProcess = context.Customer.Where(x => x.Status == "Pending"  && x.CustomerKycCategory == AppConfig.TpinCode && x.TrialCount <= AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending ECRS account  to  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running ECRS pin generation schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).TPinGeneration(item);

                        if (pencomeStatus.ResponseCode == "06")
                        {
                            continue;
                        }
                        customer = (Customer)pencomeStatus.ResultObject;
                        item.PencomResponseCode = customer.PencomResponseCode;
                        item.PencomResponseMessage = customer.PencomResponseMessage;
                        item.SetID = customer.SetID;
                        item.IsPinGenerated = false;
                        if (item.PencomResponseCode == "0" && item.PencomResponseMessage == "Submitted for Processing")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                        }
                        else
                        {
                            item.Status = "Pending";
                        }
                        Trace.TraceInformation($"pencom response for ECRS account form no {item.CustomerID};======> {item.PencomResponseMessage}======== {item.SetID ?? ""}  status={item.Status}"); Trace.TraceInformation($"pencom response for ECRS account form no {item.CustomerID};======> {item.PencomResponseMessage}======== {item.SetID ?? ""}");
                        var accountToUpdate = new CRMAccountResponseModel()
                        {
                            CoreSystemResponse = "",
                            PencomCode = item.PencomResponseCode,
                            PencomResponse = item.PencomResponseMessage,
                            RSAPIN = string.Empty,
                            CustomerID = item.CustomerID,
                            MembershipStatus = false,
                            SMSStatus = false,
                            IsPinGenerated = false
                        };
                        //item.TrialCount = item.TrialCount + 1;
                        //CRM for updates
                        Trace.TraceInformation($"about updating CRM with ECRS pencom response pin generation for {item.CustomerID}");
                        var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);
                        item.TrialCount = item.TrialCount + 1;
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel CreateEmployeeOnCore()
        {
            try
            {
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running core system employee creation "); Trace.TraceInformation("running core system employee creation");
                    var accountsToProcess = context.Customer.Where(x => x.Status == "PENCOM_APPROVED" && x.IsAccountOnCore == false && x.CoreSystemCount<AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending account  to  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process as account are either pending or already created");
                    }
                    Trace.TraceInformation($"running core system account creation schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrEmpty(item.RSAPIN))
                        {
                            Trace.TraceInformation($"Processing account with form number {item.CustomerID} rsa pin is empty, skipping this iteration...moving on to the next counter");
                            continue;
                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID} and rsapin{item.RSAPIN}");

                        try
                        {
                            Trace.TraceInformation($"inside try catch, checking all variables ");
                           item.MaritalStatus = InMemory.MaritalList.FirstOrDefault(x => x.GUID == item.MaritalStatus).NavCode ?? "";
                            item.Fundtype = InMemory.FundTypeList.FirstOrDefault(x => x.GUID == item.Fundtype).NavID ?? "";
                            //calling CRM to get data 
                            var CRMCall = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).FetchNewClient(item.MobileNumber).Result;
                            if (CRMCall.ResultObject != null && CRMCall.ResponseCode=="00")
                            {
                                var client = (Client)CRMCall.ResultObject;
                                Trace.TraceInformation($"CRM client call is not null, i will proceed ");
                                item.CorrAddress = client.CorrAddress??"";
                                item.CorrCountryOfResidence = client.CorrCountryName??"";
                                item.CorrHouseNo = client.CorrHouseNo??"";
                                item.CorrLGAOfResidence = client.CorrLGAOfResidence??"";
                                item.CorrTown = client.CorrTown??"";
                                item.CorrStateOfResidence = client.CorrStateOfResidence??"";
                                item.CorrStreetName = client.CorrStreetName??"";
                                item.AgentCode = client.RSAAgentCode??"";
                                item.NOKHouseNo = client.NOKHouseNo??"";
                                item.NOKStreet = client.NOKStreet??"";
                                item.MaidenName = client.MaidenName??"";
                                item.CommencementDate = client.CommencementDate;
                                item.LGAOfResidence = client.LGAOfResidence;
                                //item.Gender = client.Gender;
                                item.StaffNo = client.StaffNo;
                                //employer name 
                                item.EmployerCountry = client.EmployerCountry;
                                item.EmployerName = client.EmployerName;
                                
                            }

                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation($"AN error occurred while calling CRM for employee account creation on core see error below {ex.Message}; {ex}");
                        }
                        var coresystemStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).CreateEmployeeCore(item, item.IsAccountOnCore);
                        if (coresystemStatus.ResponseCode == "06")
                        {
                            Trace.TraceInformation($"core system response for ECRS account form no {item.RSAPIN};======> {coresystemStatus.ResultObject ?? ""}");                            
                            continue;
                        }                      
                        Trace.TraceInformation($"core system response for ECRS account form no {item.RSAPIN};======> {coresystemStatus.ResultObject??""}");
                        item.IsAccountOnCore = true;
                        var accountToUpdate = new CRMAccountResponseModel()
                        {
                            CoreSystemResponse = (string)coresystemStatus.ResultObject, 
                            CustomerID = item.CustomerID                                                     
                        };
                        
                        Trace.TraceInformation($"about updating CRM with core system  response  for {item.RSAPIN}"); 
                        var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);

                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        
        //public static ResponseModel DataRecaptureCoreSystemUpdateJob()
        //{
        //    try
        //    {
        //        Customer customer = new Customer();
        //        using (ARMPContext context = new ARMPContext())
        //        {
        //            Trace.TraceInformation("running core system employee creation "); Trace.TraceInformation("running core system employee creation");
        //            var accountsToProcess = context.CustomerDatarecapture.Where(x => x.Status == "PENCOM_APPROVED" && x.IsAccountOnCore == false && x.CoreSystemCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence);
        //            if (accountsToProcess.Count() <= 0)
        //            {
        //                Trace.TraceInformation("No pending account  to  process");
        //                return ResponseDictionary.GetCodeDescription("03", "No record found to process as account are either pending or already created");
        //            }
        //            Trace.TraceInformation($"running core system data recapture creation schema total account to process {accountsToProcess.Count()}");
        //            foreach (var item in accountsToProcess)
        //            {
        //                if (string.IsNullOrEmpty(item.RSAPIN))
        //                {
        //                    Trace.TraceInformation($"Processing account with form number {item.CustomerID} rsa pin is empty, skipping this iteration...moving on to the next counter");
        //                    continue;
        //                }
        //                Trace.TraceInformation($"Processing account with form number {item.CustomerID} and rsapin{item.RSAPIN}");

        //                try
        //                {
        //                    Trace.TraceInformation($"inside try catch, checking all variables ");
        //                    item.MaritalStatus = InMemory.MaritalList.FirstOrDefault(x => x.GUID == item.MaritalStatus).NavCode ?? "";
        //                    item.Fundtype = InMemory.FundTypeList.FirstOrDefault(x => x.GUID == item.Fundtype).NavID ?? "";
        //                    //calling CRM to get data 
        //                    var CRMCall = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).FetchNewClient(item.MobileNumber).Result;
        //                    if (CRMCall.ResultObject != null && CRMCall.ResponseCode == "00")
        //                    {
        //                        var client = (Client)CRMCall.ResultObject;
        //                        Trace.TraceInformation($"CRM client call is not null, i will proceed ");
        //                        item.CorrAddress = client.CorrAddress ?? "";
        //                        item.CorrCountryOfResidence = client.CorrCountryName ?? "";
        //                        item.CorrHouseNo = client.CorrHouseNo ?? "";
        //                        item.CorrLGAOfResidence = client.CorrLGAOfResidence ?? "";
        //                        item.CorrTown = client.CorrTown ?? "";
        //                        item.CorrStateOfResidence = client.CorrStateOfResidence ?? "";
        //                        item.CorrStreetName = client.CorrStreetName ?? "";
        //                        item.RSAAgent = client.RSAAgentCode ?? "";
        //                        item.NOKHouseNo = client.NOKHouseNo ?? "";
        //                        item.NOKStreet = client.NOKStreet ?? "";
        //                        item.MaidenName = client.MaidenName ?? "";
        //                        item.CommencementDate = client.CommencementDate;
        //                        item.LGAOfResidence = client.LGAOfResidence;
        //                        item.Gender = client.Gender;
        //                        item.StaffNo = client.StaffNo;
        //                        //employer name 
        //                        item.EmployerCountry = client.EmployerCountry;
        //                        item.EmployerName = client.EmployerName;

        //                    }

        //                }
        //                catch (Exception)
        //                {

        //                    throw;
        //                }
        //                var coresystemStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).RecaptureEmployeeCore(item);
        //                if (coresystemStatus.ResponseCode == "06")
        //                {
        //                    Trace.TraceInformation($"core system response for ECRS account form no {item.RSAPIN};======> {coresystemStatus.ResultObject ?? ""}");
        //                    continue;
        //                }
        //                Trace.TraceInformation($"core system response for ECRS account form no {item.RSAPIN};======> {coresystemStatus.ResultObject ?? ""}");
        //                item.IsAccountOnCore = true;
        //                var accountToUpdate = new CRMAccountResponseModel()
        //                {
        //                    CoreSystemResponse = (string)coresystemStatus.ResultObject,
        //                    CustomerID = item.CustomerID
        //                };

        //                Trace.TraceInformation($"about updating CRM with core system  response  for {item.RSAPIN}");
        //                var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithResponse(accountToUpdate);

        //            }
        //            context.SaveChanges();
        //            Trace.TraceInformation("successfully updated all changes ");
        //            return ResponseDictionary.GetCodeDescription("03");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
        //        return ResponseDictionary.GetCodeDescription("06");
        //    }
        //}

        public static ResponseModel DataRecaptureCoreSystemUpdateJob()
        {
            try
            {
                Trace.TraceInformation("data recapture core system update job has been triggered");
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("retrieving all data recapture records with approved status from the database");
                    var dataRecapture = context.CustomerDatarecapture.Where(x => x.IsAccountOnCore == false && x.Status == "PENCOM_APPROVED" && x.CoreSystemCount <= AppConfig.TrialCount).OrderByDescending(x => x.ID).Take(AppConfig.VolumeSequence);

                    Trace.TraceInformation("checking that at least 1 record was retrieved");
                    if (dataRecapture.Count() <= 0)
                    {
                        Trace.TraceInformation("ending method after no record found");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }

                    Trace.TraceInformation("looping through retrieved records");
                    foreach (var item in dataRecapture)
                    {
                        Trace.TraceInformation("parse the object as a type of Customer");
                        Customer recapture = new Customer();
                        recapture.RSAPIN = item.RSAPIN;
                        recapture.FirstName = item.FirstName ?? "";
                        recapture.LastName = item.LastName ?? "";
                        recapture.OtherName = item.OtherName ?? "";

                        recapture.Title = item.Title;

                        if (item.DOB != null && item.DOB != DateTime.Today)
                        {
                            recapture.DOB = item.DOB;
                        }
                        recapture.DateOfFirstEmployment = item.DateOfFirstEmployment;
                        recapture.Fundtype = item.Fundtype ?? "";
                        recapture.EmployerCode = item.EmployerCode ?? "";
                        if (item.CommencementDate != null && item.CommencementDate != DateTime.Today)
                        {
                            recapture.CommencementDate = item.CommencementDate;
                        }
                        recapture.CreatedBy = item.AccountOwner;
                        recapture.Nationality = item.Nationality ?? "";
                        recapture.StateOfOrigin = item.StateOfOrigin ?? "";
                        recapture.StateOfPosting = item.StateOfPosting ?? "";
                        recapture.MobileNumber = item.MobileNumber ?? "";
                        recapture.Email = item.Email ?? "";
                        recapture.Address = item.Address ?? "";
                        recapture.Town = item.Town ?? "";
                        recapture.StateOfResidence = item.StateOfResidence ?? "";
                        recapture.CountryOfResidence = item.CountryOfResidence ?? "";
                        recapture.Gender = item.Gender ?? "";
                        recapture.AgentCode = "";
                        recapture.BlockedCode = 0;
                        recapture.FlagCode = "";
                        recapture.StatementOption = item.StatementOption ?? "";
                        recapture.AccountOwner = item.AccountOwner ?? "";
                        if (item.EmployeeContribution > 0)
                        {
                            recapture.EmployeeContribution = item.EmployeeContribution;
                        }
                        if (item.EmployerContribution > 0)
                        {
                            recapture.EmployerContribution = item.EmployerContribution;
                        }

                        #region CRM details fetch
                        

                        #endregion

                        if (!string.IsNullOrEmpty(item.MaritalStatus))
                        {
                            recapture.MaritalStatus = InMemory.MaritalList.FirstOrDefault(x => x.GUID == item.MaritalStatus).NavCode ?? "";
                        }
                        if (!string.IsNullOrEmpty(item.Fundtype))
                        {
                            recapture.Fundtype = InMemory.FundTypeList.FirstOrDefault(x => x.GUID == item.Fundtype).NavID ?? "";
                        }


                        #region CRM details fetch
                        Trace.TraceInformation($"inside try catch, checking all variables ");
                        //calling CRM to get data 
                        string DUrl = $"{AppConfig.CrmUri}/api/data/v9.0/armp_updateaccounts(armp_referencenumber={item.CustomerID})";
                        var acc = new CrmEntityConnector(new CrmApiConnector()).GetEntityEntityName(DUrl).Result;
                        if (acc.ResponseCode != "00")
                        {
                            recapture.CorrAddress = "";
                            recapture.CorrCountryOfResidence = "";
                            recapture.CorrHouseNo = "";
                            recapture.CorrLGAOfResidence = "";
                            recapture.CorrTown = "";
                            recapture.CorrStateOfResidence = "";
                            recapture.CorrStreetName = "";
                            recapture.AgentCode = "";
                            recapture.NOKHouseNo = "";
                            recapture.NOKStreet = "";
                            recapture.MaidenName = "";
                            recapture.CommencementDate = DateTime.Now;
                            recapture.LGAOfResidence = "";
                            recapture.Gender = "0";
                            recapture.StaffNo = "";
                            recapture.EmployerCountry = "";
                            recapture.EmployerName = "";
                        }
                        else
                        {
                            JObject data = (JObject)acc.ResultObject;
                            if (data != null)
                            {
                                Trace.TraceInformation($"CRM client call is not null, i will proceed ");
                                item.CorrAddress = data.GetValue("armp_correspondenceaddress").ToString() ?? "";
                                if (!string.IsNullOrEmpty(data.GetValue("_armp_correspondencecountry_value").ToString()))
                                {
                                    item.CorrCountryOfResidence = InMemory.CountryList.Where(x => x.GUID == data.GetValue("_armp_correspondencecountry_value").ToString()).FirstOrDefault().Reference;
                                }
                                item.CorrHouseNo = data.GetValue("armp_correspondenceaddressbuildingnumber").ToString() ?? "";
                                item.CorrLGAOfResidence = data.GetValue("armp_correspondenceaddresslgacode").ToString() ?? "";
                                item.CorrTown = data.GetValue("armp_correspondencevillagetowncity").ToString() ?? "";
                                item.CorrStateOfResidence = data.GetValue("armp_stateofcorrespondenceaddresscode").ToString() ?? "";
                                item.CorrStreetName = data.GetValue("armp_correspondencestreetname").ToString() ?? "";
                                item.RSAAgent = data.GetValue("armp_rsaagentcode").ToString() ?? "";
                                item.NOKHouseNo = data.GetValue("armp_nokhousenumbername").ToString() ?? "";
                                item.NOKStreet = data.GetValue("armp_nokstreetname").ToString() ?? "";
                                item.MaidenName = data.GetValue("armp_mothersmaidenname").ToString() ?? "";
                                item.CommencementDate = DateTime.Parse(data.GetValue("armp_commencementdate").ToString());
                                item.LGAOfResidence = data.GetValue("armp_residenceaddresslgacode").ToString() ?? "";
                                if (!string.IsNullOrEmpty(data.GetValue("armp_gender").ToString()))
                                {
                                    item.Gender = InMemory.GenderList.Where(x => x.GUID == data.GetValue("armp_gender").ToString()).FirstOrDefault().Reference;
                                }
                                item.StaffNo = data.GetValue("armp_employeeidnumber").ToString() ?? "";
                                //employer name
                                if (!string.IsNullOrEmpty(item.EmployerGUID))
                                {
                                    var employer = new CrmEntityConnector(new CrmApiConnector()).GetEntityByGUID(item.EmployerGUID, "accounts").Result;
                                    item.EmployerName = employer.GetValue("name").ToString();
                                    if (!string.IsNullOrEmpty(employer.GetValue("_armp_country_value").ToString()))
                                    {
                                        item.EmployerCountry = InMemory.CountryList.Where(x => x.GUID == employer.GetValue("_armp_country_value").ToString()).FirstOrDefault().Reference;
                                    }
                                }
                            }
                        }


                        #endregion
                        


                        Trace.TraceInformation("send the resulting customer details retrieved from CRM to NAV");
                        var updateAccount = new ExternalNode(new ApiPostAndGet(), new FileConverter()).UpdateEmployeeCore(recapture);
                        Trace.TraceInformation("confirming method success");
                        if (updateAccount.ResponseCode == "00")
                        {
                            Trace.TraceInformation("update database");
                            item.IsAccountOnCore = true;
                        }

                        Trace.TraceInformation("update count");
                        item.TrialCount = item.TrialCount + 1;
                    }
                    Trace.TraceInformation("saving database");
                    context.SaveChanges();
                    Trace.TraceInformation("data recapture account core system update has been completed");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel DBA_ApprovalCheck()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running DBA approval check");
                    var document = context.DeathBenefitAccountDocument.Where(x => x.isAccountApproved == false && x.IsAllDocumentUploaded == true).OrderByDescending(x => x.ID).Take(10);
                    if (document.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running Share Point FileUpload schema total account to process {document.Count()}");
                    foreach (var item in document)
                    {

                        try
                        {
                            //check CRM for DBA account 
                            var Result = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).FetchAccountStatus(item.CustomerID);
                            if (Result.ResponseCode == "00")
                            {
                                //extract acount status 
                                if (Result.ResultObject != null)
                                {
                                    var status = (string)Result.ResultObject;
                                    if (status == "779560007")
                                    {
                                        item.isAccountApproved = true;
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                        }


                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        //process each file  

                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel Case_ResolutionCheck()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running case resolution check");
                    var document = context.CaseUpload.Where(x => x.IsCaseResolved == false).OrderByDescending(x => x.ID).Take(15);
                    if (document.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running case resolution to process {document.Count()}");
                    foreach (var item in document)
                    {

                        try
                        {
                            //check CRM for DBA account 
                            var Result = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).FetchCaseStatus(item.CaseNo);
                            if (Result.ResponseCode == "00")
                            {
                                //extract acount status 
                                if (Result.ResultObject != null)
                                {
                                    var status = (string)Result.ResultObject;
                                    if (status == "779560011" || status== "779560012")
                                    {
                                        item.IsCaseResolved = true;
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                        }


                        Trace.TraceInformation($"Processing account with form number {item.CaseNo}");
                        //process each file  

                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
        public static ResponseModel SharePointFileUploadWithMetaData_CPS()
        {
            try
            {

                string Directoryfolder = string.Empty, PassportFileName = string.Empty, SignaturefileName = string.Empty, ConsentFormFileName = string.Empty, POIFileName=string.Empty, POAFileName = string.Empty, EvidenceOfEmployementFileName = string.Empty, BirthCertificateFileName = string.Empty, NOKPassportFileName=string.Empty, RSAFormFileName=string.Empty ;
                byte[] Signature = null, Passport = null, Consent = null, POA = null, POI=null, Evidence=null,NOK=null, BirthCertificate=null, RsaForm =null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running Share Point FileUpload With MetaData schema"); 
                    var accountsToProcess = context.Customer.Where(x => x.Status == "PENCOM_APPROVED" && x.isSharePointUploaded == false && x.CustomerKycCategory== "779560000").Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running Share Point FileUpload schema total account to process {accountsToProcess.Count()}"); 
                    foreach (var item in accountsToProcess)
                    {
                         Directoryfolder = $@"{AppConfig.DocumentManager}\{item.CustomerID}";
                        #region document upload
                        var extraDocument = context.AccountDocument.Where(x => x.Customerid == item.CustomerID).FirstOrDefault();
                        if (extraDocument != null)
                        {
                            try
                            {
                                Signature = Convert.FromBase64String(item.SignatureConverted);
                                Passport = Convert.FromBase64String(item.PassportConverted);
                                Consent = Convert.FromBase64String(item.ConsentFormConverted);
                                #region birthcertificate
                                if (!string.IsNullOrEmpty(extraDocument.BirthCertificateUpload))
                                {
                                    if (extraDocument.BirthCertificateUpload.Contains("data:image/png;base64,"))
                                    {
                                        BirthCertificate = Convert.FromBase64String(extraDocument.BirthCertificateUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.BirthCertificateUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        BirthCertificate = Convert.FromBase64String(extraDocument.BirthCertificateUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region POI
                                if (!string.IsNullOrEmpty(extraDocument.ProofOfIdentityUpload))
                                {
                                    if (extraDocument.ProofOfIdentityUpload.Contains("data:image/png;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.ProofOfIdentityUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.BirthCertificateUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.ProofOfIdentityUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion                        }

                                #region POA
                                if (!string.IsNullOrEmpty(extraDocument.ProofOfAddressUpload))
                                {
                                    if (extraDocument.ProofOfAddressUpload.Contains("data:image/png;base64,"))
                                    {
                                        POA = Convert.FromBase64String(extraDocument.ProofOfAddressUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.ProofOfAddressUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        POA = Convert.FromBase64String(extraDocument.ProofOfAddressUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region NOK
                                if (!string.IsNullOrEmpty(extraDocument.NOKPassportUpload))
                                {
                                    if (extraDocument.NOKPassportUpload.Contains("data:image/png;base64,"))
                                    {
                                        NOK = Convert.FromBase64String(extraDocument.NOKPassportUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.NOKPassportUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        NOK = Convert.FromBase64String(extraDocument.NOKPassportUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region EvidenceOfEmployment
                                if (!string.IsNullOrEmpty(extraDocument.EvidenceOfEmploymentUpload))
                                {
                                    if (extraDocument.EvidenceOfEmploymentUpload.Contains("data:image/png;base64,"))
                                    {
                                        Evidence = Convert.FromBase64String(extraDocument.EvidenceOfEmploymentUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.EvidenceOfEmploymentUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        Evidence = Convert.FromBase64String(extraDocument.EvidenceOfEmploymentUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region RSAform
                                if (!string.IsNullOrEmpty(extraDocument.RSAFormUpload))
                                {
                                    if (extraDocument.RSAFormUpload.Contains("png"))
                                    {
                                        RsaForm = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}\rsaform.png"));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.png";
                                    }
                                    else if (extraDocument.RSAFormUpload.Contains("jpg"))
                                    {
                                        RsaForm = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}\rsaform.jpg"));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.jpg";
                                    }
                                    else if (extraDocument.RSAFormUpload.Contains("pdf"))
                                    {
                                        RsaForm = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}\rsaform.pdf"));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.pdf";
                                    }
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                            }
                          
                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");   
                        //process each file                         
                         PassportFileName = $"{item.RSAPIN}_Passport.jpg";
                         SignaturefileName = $"{item.RSAPIN}_ Signature.jpg";
                         ConsentFormFileName = $"{item.RSAPIN}_ConsentForm.jpg";
                         POIFileName = $"{item.RSAPIN}_ProofOfIdentity.jpg";
                         POAFileName = $"{item.RSAPIN}_ProofOfAddress.jpg";
                         EvidenceOfEmployementFileName = $"{item.RSAPIN}_EvidenceOfEmployment.jpg";
                         BirthCertificateFileName = $"{item.RSAPIN}_BirthCertificate.jpg";
                        
                       
                        //converting passportupload
                        bool isPassportOk = SharePointManager.UploadFilewithMetaData(Passport, PassportFileName, AppConfig.SPLibrary,item.RSAPIN,item.CustomerID,"Pin Generation","Accounts","ESB-CRM");
                        //converting signature upload 
                        bool isSignatureOk = SharePointManager.UploadFilewithMetaData(Signature, SignaturefileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //converting consent upload
                        bool isConsentOk = SharePointManager.UploadFilewithMetaData(Consent, ConsentFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //birthcertificate
                        bool isBirthCertificateOk = SharePointManager.UploadFilewithMetaData(BirthCertificate, BirthCertificateFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isEmploymentOk = SharePointManager.UploadFilewithMetaData(Evidence, EvidenceOfEmployementFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isRSAFormOk = SharePointManager.UploadFilewithMetaData(RsaForm, RSAFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isPOAk = SharePointManager.UploadFilewithMetaData(POA, POAFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isPOIok = SharePointManager.UploadFilewithMetaData(POI, POIFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        if (isPassportOk==true&&isSignatureOk==true&&isConsentOk==true)
                        {
                            Trace.TraceInformation($"completely converted all files");
                            item.PassportUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{PassportFileName}";
                            item.SignatureUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{SignaturefileName}";
                            item.ConsentFormConverted = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{ConsentFormFileName}";
                            item.isSharePointUploaded = true;
                        }

                        #endregion
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SharePointFileUploadWithMetaData_MPS()
        {
            try
            {

                string Directoryfolder = string.Empty, PassportFileName = string.Empty, SignaturefileName = string.Empty, ConsentFormFileName = string.Empty, POIFileName = string.Empty, MembershipName = string.Empty, RSAFormFileName = string.Empty;
                byte[] Signature = null, Passport = null, Consent = null, POI = null, Membership = null, RsaForm = null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running MPS Share Point FileUpload With MetaData schema");
                    var accountsToProcess = context.Customer.Where(x => x.Status == "PENCOM_APPROVED" && x.isSharePointUploaded == false && x.CustomerKycCategory == "779560002").Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running Share Point FileUpload schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Directoryfolder = $@"{AppConfig.DocumentManager}\{item.CustomerID}";
                        #region document upload
                        var extraDocument = context.MicroPensionsAccountDocument.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                        if (extraDocument != null)
                        {
                            try
                            {
                                Signature = Convert.FromBase64String(item.SignatureConverted);
                                Passport = Convert.FromBase64String(item.PassportConverted);
                                Consent = Convert.FromBase64String(item.ConsentFormConverted);
                                #region EvidenceOfAssociationMembershipUpload
                                if (!string.IsNullOrEmpty(extraDocument.EvidenceOfAssociationMembershipUpload))
                                {
                                    if (extraDocument.EvidenceOfAssociationMembershipUpload.Contains("data:image/png;base64,"))
                                    {
                                        Membership = Convert.FromBase64String(extraDocument.EvidenceOfAssociationMembershipUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.EvidenceOfAssociationMembershipUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        Membership = Convert.FromBase64String(extraDocument.EvidenceOfAssociationMembershipUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region MeansofIdentificationUpload
                                if (!string.IsNullOrEmpty(extraDocument.MeansofIdentificationUpload))
                                {
                                    if (extraDocument.MeansofIdentificationUpload.Contains("data:image/png;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.MeansofIdentificationUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.MeansofIdentificationUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.MeansofIdentificationUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region RSAForm
                                if (!string.IsNullOrEmpty(extraDocument.RSAFormUpload))
                                {
                                    if (extraDocument.RSAFormUpload.Contains("data:image/png;base64,"))
                                    {
                                        RsaForm = Convert.FromBase64String(extraDocument.RSAFormUpload.Replace("data:image/png;base64,", ""));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.jpg";
                                    }
                                    else if (extraDocument.RSAFormUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        RsaForm = Convert.FromBase64String(extraDocument.RSAFormUpload.Replace("data:image/jpeg;base64,", ""));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.jpg";
                                    }
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                            }

                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        //process each file                         
                        PassportFileName = $"{item.RSAPIN}_Passport.jpg";
                        SignaturefileName = $"{item.RSAPIN}_ Signature.jpg";
                        ConsentFormFileName = $"{item.RSAPIN}_ConsentForm.jpg";
                        POIFileName = $"{item.RSAPIN}_MeansOfIdentification.jpg";                       
                     

                        //converting passportupload
                        bool isPassportOk = SharePointManager.UploadFilewithMetaData(Passport, PassportFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //converting signature upload 
                        bool isSignatureOk = SharePointManager.UploadFilewithMetaData(Signature, SignaturefileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //converting consent upload
                        bool isConsentOk = SharePointManager.UploadFilewithMetaData(Consent, ConsentFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //birthcertificate
                        bool isRSAFormOk = SharePointManager.UploadFilewithMetaData(RsaForm, RSAFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isPOIok = SharePointManager.UploadFilewithMetaData(POI, POIFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        if (isPassportOk == true && isSignatureOk == true && isConsentOk == true)
                        {
                            Trace.TraceInformation($"completely converted all files");
                            item.PassportUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{PassportFileName}";
                            item.SignatureUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{SignaturefileName}";
                            item.ConsentFormConverted = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{ConsentFormFileName}";
                            item.isSharePointUploaded = true;
                        }

                        #endregion
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SharePointFileUploadWithMetaData_TPIN()
        {
            try
            {

                string Directoryfolder = string.Empty, IndemnityName = string.Empty, AppointmentName = string.Empty;
                byte[] Indemnity = null, Appointment = null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running TPIN Share Point FileUpload With MetaData schema");
                    var accountsToProcess = context.Customer.Where(x => x.Status == "PENCOM_APPROVED" && x.isSharePointUploaded == false && x.CustomerKycCategory == "779560003").Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running Share Point FileUpload schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Directoryfolder = $@"{AppConfig.DocumentManager}\{item.CustomerID}";
                        #region document upload
                        var extraDocument = context.TemporaryPINAccountDocument.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                        if (extraDocument != null)
                        {
                            try
                            {
                               
                                #region LetterOfAppointment
                                if (!string.IsNullOrEmpty(extraDocument.LetterOfAppointmentUpload))
                                {
                                    if (extraDocument.LetterOfAppointmentUpload.Contains("data:image/png;base64,"))
                                    {
                                        Appointment = Convert.FromBase64String(extraDocument.LetterOfAppointmentUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.LetterOfAppointmentUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        Appointment = Convert.FromBase64String(extraDocument.LetterOfAppointmentUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region Indemnity
                                if (!string.IsNullOrEmpty(extraDocument.LetterOfIndemnityUpload))
                                {
                                    if (extraDocument.LetterOfIndemnityUpload.Contains("data:image/png;base64,"))
                                    {
                                        Indemnity = Convert.FromBase64String(extraDocument.LetterOfIndemnityUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.LetterOfIndemnityUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        Indemnity = Convert.FromBase64String(extraDocument.LetterOfIndemnityUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion
                                                                
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                            }

                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        //process each file                         
                        AppointmentName = $"{item.RSAPIN}_LetterOfAppointment.jpg";
                        IndemnityName = $"{item.RSAPIN}_ LetterOfIndemnity.jpg";                       

                        //converting passportupload
                        bool isAppointmentOk = SharePointManager.UploadFilewithMetaData(Appointment, AppointmentName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts-TPIN", "ESB-CRM");
                        //converting signature upload 
                        bool isIndemnityOk = SharePointManager.UploadFilewithMetaData(Indemnity, IndemnityName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts-TPIN", "ESB-CRM");
                        //converting consent upload
                           if (isAppointmentOk == true && isIndemnityOk == true)
                        {
                            Trace.TraceInformation($"completely converted all files");                       
                            item.isSharePointUploaded = true;
                        }

                        #endregion
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SharePointFileUploadWithMetaData_CrossBorder()
        {
            try
            {

                string Directoryfolder = string.Empty, PassportFileName = string.Empty, SignaturefileName = string.Empty, ConsentFormFileName = string.Empty, POIFileName = string.Empty, LetterOfUndertakingFileName = string.Empty, EvidenceOfEmployementFileName = string.Empty, EvidenceOfNationalityFileName = string.Empty, WorkPermitUpFileName = string.Empty, RSAFormFileName = string.Empty;
                byte[] Signature = null, Passport = null, Consent = null, LetterOfUndertaking = null, POI = null, Evidence = null, WorkPermitUp = null, EvidenceOfNationality = null, RsaForm = null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running Cross border Share Point FileUpload With MetaData schema");
                    var accountsToProcess = context.Customer.Where(x => x.Status == "PENCOM_APPROVED" && x.isSharePointUploaded == false && x.CustomerKycCategory == "779560001").Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running cross border Share Point FileUpload schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Directoryfolder = $@"{AppConfig.DocumentManager}\{item.CustomerID}";
                        #region document upload
                        var extraDocument = context.CrossBorderAccountDocument.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                        if (extraDocument != null)
                        {
                            try
                            {
                                Signature = Convert.FromBase64String(item.SignatureConverted);
                                Passport = Convert.FromBase64String(item.PassportConverted);
                                Consent = Convert.FromBase64String(item.ConsentFormConverted);
                                #region LetterOfUndertakingUpload
                                if (!string.IsNullOrEmpty(extraDocument.LetterOfUndertakingUpload))
                                {
                                    if (extraDocument.LetterOfUndertakingUpload.Contains("data:image/png;base64,"))
                                    {
                                        LetterOfUndertaking = Convert.FromBase64String(extraDocument.LetterOfUndertakingUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.LetterOfUndertakingUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        LetterOfUndertaking = Convert.FromBase64String(extraDocument.LetterOfUndertakingUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region WorkPermitUpload
                                if (!string.IsNullOrEmpty(extraDocument.WorkPermitUpload))
                                {
                                    if (extraDocument.WorkPermitUpload.Contains("data:image/png;base64,"))
                                    {
                                        WorkPermitUp = Convert.FromBase64String(extraDocument.WorkPermitUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.WorkPermitUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        WorkPermitUp = Convert.FromBase64String(extraDocument.WorkPermitUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion                        

                                #region EvidenceOfNationalityUpload
                                if (!string.IsNullOrEmpty(extraDocument.EvidenceOfNationalityUpload))
                                {
                                    if (extraDocument.EvidenceOfNationalityUpload.Contains("data:image/png;base64,"))
                                    {
                                        EvidenceOfNationality = Convert.FromBase64String(extraDocument.EvidenceOfNationalityUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.EvidenceOfNationalityUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        EvidenceOfNationality = Convert.FromBase64String(extraDocument.EvidenceOfNationalityUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region MeansOfIdentitification
                                if (!string.IsNullOrEmpty(extraDocument.MeansOfIdentificationUpload))
                                {
                                    if (extraDocument.MeansOfIdentificationUpload.Contains("data:image/png;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.MeansOfIdentificationUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.MeansOfIdentificationUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.MeansOfIdentificationUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region EvidenceOfEmployment
                                if (!string.IsNullOrEmpty(extraDocument.EvidenceOfEmploymentUpload))
                                {
                                    if (extraDocument.EvidenceOfEmploymentUpload.Contains("data:image/png;base64,"))
                                    {
                                        Evidence = Convert.FromBase64String(extraDocument.EvidenceOfEmploymentUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.EvidenceOfEmploymentUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        Evidence = Convert.FromBase64String(extraDocument.EvidenceOfEmploymentUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region RSAForm
                                if (!string.IsNullOrEmpty(extraDocument.RSAFormUpload))
                                {
                                    if (extraDocument.RSAFormUpload.Contains("data:image/png;base64,"))
                                    {
                                        RsaForm = Convert.FromBase64String(extraDocument.RSAFormUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.RSAFormUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        RsaForm = Convert.FromBase64String(extraDocument.RSAFormUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                            }

                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        //process each file                         
                        PassportFileName = $"{item.RSAPIN}_Passport.jpg";
                        SignaturefileName = $"{item.RSAPIN}_ Signature.jpg";
                        ConsentFormFileName = $"{item.RSAPIN}_ConsentForm.jpg";
                        POIFileName = $"{item.RSAPIN}_MeansOfIdentification.jpg";
                        EvidenceOfEmployementFileName = $"{item.RSAPIN}_EvidenceOfEmployement.jpg";
                        WorkPermitUpFileName = $"{item.RSAPIN}_WorkPermit.jpg";
                        RSAFormFileName = $"{item.RSAPIN}_Rsaform.jpg";
                        EvidenceOfNationalityFileName = $"{item.RSAPIN}_EvidenceOfNationality.jpg";
                        LetterOfUndertakingFileName = $"{item.RSAPIN}_LetterOfUndertaking.jpg";


                        //converting passportupload
                        bool isPassportOk = SharePointManager.UploadFilewithMetaData(Passport, PassportFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //converting signature upload 
                        bool isSignatureOk = SharePointManager.UploadFilewithMetaData(Signature, SignaturefileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //converting consent upload
                        bool isConsentOk = SharePointManager.UploadFilewithMetaData(Consent, ConsentFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        //birthcertificate
                        bool isEvidenceOfEmployementOk = SharePointManager.UploadFilewithMetaData(Evidence, EvidenceOfEmployementFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isUndertakingOk = SharePointManager.UploadFilewithMetaData(LetterOfUndertaking, LetterOfUndertakingFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isRSAFormOk = SharePointManager.UploadFilewithMetaData(RsaForm, RSAFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isWorkPermitOk = SharePointManager.UploadFilewithMetaData(WorkPermitUp, WorkPermitUpFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isPOIok = SharePointManager.UploadFilewithMetaData(POI, POIFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        bool isNationalityok = SharePointManager.UploadFilewithMetaData(EvidenceOfNationality, EvidenceOfNationalityFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                        if (isPassportOk == true && isSignatureOk == true && isConsentOk == true)
                        {
                            Trace.TraceInformation($"completely converted all files");
                            item.PassportUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{PassportFileName}";
                            item.SignatureUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{SignaturefileName}";
                            item.ConsentFormConverted = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{ConsentFormFileName}";
                            item.isSharePointUploaded = true;
                        }

                        #endregion
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SharePointFileUploadWithMetaData_DBA()
        {
            try
            {

               string DeceasedPassportUploadName = string.Empty, DeathCertificateUploadName = string.Empty, BeneficiaryMeansOfIdentificationUploadName = string.Empty, LetterOfIntroductionUploadName = string.Empty, AccountOpeningFormUploadName = string.Empty;
                byte[] DeceasedPassportUpload = null, DeathCertificateUpload = null, BeneficiaryMeansOfIdentificationUpload = null, LetterOfIntroductionUpload = null, AccountOpeningFormUpload = null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running DBA Share Point FileUpload With MetaData schema");
                    var document = context.DeathBenefitAccountDocument.Where(x => x.isAccountApproved == true && x.isSharePointUploaded==false).Take(10);
                    if (document.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running Share Point FileUpload schema total account to process {document.Count()}");
                    foreach (var item in document)
                    {    
                      
                        
                            try
                            {
                                #region AccountOpeningFormUpload
                                if (!string.IsNullOrEmpty(item.AccountOpeningFormUpload))
                                {
                                    if (item.AccountOpeningFormUpload.Contains("data:image/png;base64,"))
                                    {
                                        AccountOpeningFormUpload = Convert.FromBase64String(item.AccountOpeningFormUpload.Replace("data:image/png;base64,", ""));
                                    AccountOpeningFormUploadName = $"{item.CustomerID}_ AccountOpeningFormUpload.jpg";
                                }
                                    else if (item.AccountOpeningFormUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        AccountOpeningFormUpload = Convert.FromBase64String(item.AccountOpeningFormUpload.Replace("data:image/jpeg;base64,", ""));
                                    AccountOpeningFormUploadName = $"{item.CustomerID}_ AccountOpeningFormUpload.jpg";
                                }
                                    else if (item.AccountOpeningFormUpload.Contains("data:application/pdf;base64,"))
                                    {
                                        AccountOpeningFormUpload = Convert.FromBase64String(item.AccountOpeningFormUpload.Replace("data:application/pdf;base64,", ""));
                                    AccountOpeningFormUploadName = $"{item.CustomerID}_ AccountOpeningFormUpload.pdf";
                                }
                                }
                                #endregion

                                #region BeneficiaryMeansOfIdentificationUpload
                                if (!string.IsNullOrEmpty(item.BeneficiaryMeansOfIdentificationUpload))
                                {
                                    if (item.AccountOpeningFormUpload.Contains("data:image/png;base64,"))
                                    {
                                        BeneficiaryMeansOfIdentificationUpload = Convert.FromBase64String(item.BeneficiaryMeansOfIdentificationUpload.Replace("data:image/png;base64,", ""));
                                    BeneficiaryMeansOfIdentificationUploadName = $"{item.CustomerID}_BeneficiaryMeansOfIdentificatio.jpg";
                                }
                                    else if (item.AccountOpeningFormUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        BeneficiaryMeansOfIdentificationUpload = Convert.FromBase64String(item.BeneficiaryMeansOfIdentificationUpload.Replace("data:image/jpeg;base64,", ""));
                                    BeneficiaryMeansOfIdentificationUploadName = $"{item.CustomerID}_BeneficiaryMeansOfIdentificatio.jpg";
                                }
                                    else if (item.AccountOpeningFormUpload.Contains("data:application/pdf;base64,"))
                                    {
                                        BeneficiaryMeansOfIdentificationUpload = Convert.FromBase64String(item.BeneficiaryMeansOfIdentificationUpload.Replace("data:application/pdf;base64,", ""));
                                    BeneficiaryMeansOfIdentificationUploadName = $"{item.CustomerID}_BeneficiaryMeansOfIdentificatio.pdf";
                                }
                                }
                                #endregion

                                #region DeathCertificateUpload
                                if (!string.IsNullOrEmpty(item.DeathCertificateUpload))
                                {
                                    if (item.DeathCertificateUpload.Contains("data:image/png;base64,"))
                                    {
                                        DeathCertificateUpload = Convert.FromBase64String(item.DeathCertificateUpload.Replace("data:image/png;base64,", ""));
                                    DeathCertificateUploadName = $"{item.CustomerID}_DeathCertificateUploadName.jpg";
                                }
                                    else if (item.DeathCertificateUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        DeathCertificateUpload = Convert.FromBase64String(item.DeathCertificateUpload.Replace("data:image/jpeg;base64,", ""));
                                    DeathCertificateUploadName = $"{item.CustomerID}_DeathCertificateUploadName.jpg";
                                }
                                    else if (item.DeathCertificateUpload.Contains("data:application/pdf;base64,"))
                                    {
                                        DeathCertificateUpload = Convert.FromBase64String(item.DeathCertificateUpload.Replace("data:application/pdf;base64,", ""));
                                    DeathCertificateUploadName = $"{item.CustomerID}_DeathCertificateUploadName.pdf";
                                }
                                }
                                #endregion

                                #region DeceasedPassportUpload
                                if (!string.IsNullOrEmpty(item.DeceasedPassportUpload))
                                {
                                    if (item.DeceasedPassportUpload.Contains("data:image/png;base64,"))
                                    {
                                        DeceasedPassportUpload = Convert.FromBase64String(item.DeceasedPassportUpload.Replace("data:image/png;base64,", ""));
                                    DeceasedPassportUploadName = $"{item.CustomerID}_ DeceasedPassportUpload.jpg";
                                }
                                    else if (item.DeceasedPassportUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        DeceasedPassportUpload = Convert.FromBase64String(item.DeceasedPassportUpload.Replace("data:image/jpeg;base64,", ""));
                                    DeceasedPassportUploadName = $"{item.CustomerID}_ DeceasedPassportUpload.jpg";
                                }
                                    else if (item.DeceasedPassportUpload.Contains("data:application/pdf;base64,"))
                                    {
                                        DeceasedPassportUpload = Convert.FromBase64String(item.DeceasedPassportUpload.Replace("data:application/pdf;base64,", ""));
                                    DeceasedPassportUploadName = $"{item.CustomerID}_ DeceasedPassportUpload.pdf";
                                }
                                }
                                #endregion

                                #region LetterOfIntroductionUpload
                                if (!string.IsNullOrEmpty(item.LetterOfIntroductionUpload))
                                {
                                    if (item.LetterOfIntroductionUpload.Contains("data:image/png;base64,"))
                                    {
                                        LetterOfIntroductionUpload = Convert.FromBase64String(item.LetterOfIntroductionUpload.Replace("data:image/png;base64,", ""));
                                    LetterOfIntroductionUploadName = $"{item.CustomerID}_LetterOfIntroductionUpload.jpg";
                                }
                                    else if (item.LetterOfIntroductionUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        LetterOfIntroductionUpload = Convert.FromBase64String(item.LetterOfIntroductionUpload.Replace("data:image/jpeg;base64,", ""));
                                    LetterOfIntroductionUploadName = $"{item.CustomerID}_LetterOfIntroductionUpload.jpg";
                                }
                                    else if (item.LetterOfIntroductionUpload.Contains("data:application/pdf;base64,"))
                                    {
                                        LetterOfIntroductionUpload = Convert.FromBase64String(item.LetterOfIntroductionUpload.Replace("data:application/pdf;base64,", ""));
                                    LetterOfIntroductionUploadName = $"{item.CustomerID}_LetterOfIntroductionUpload.pdf";
                                }
                                }
                                #endregion

                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                            }

                        
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        //process each file                         
                     

                        //converting passportupload
                        bool isDeathCertificateUploadOk = SharePointManager.UploadFilewithMetaData(DeathCertificateUpload, DeathCertificateUploadName, AppConfig.SPLibrary, item.CustomerID, item.CustomerID, "DBA Account", "Accounts", "ESB-CRM");
                        //converting signature upload 
                        bool isDeceasedPassportUploadOk = SharePointManager.UploadFilewithMetaData(DeceasedPassportUpload, DeceasedPassportUploadName, AppConfig.SPLibrary, item.CustomerID, item.CustomerID, "DBA Account", "Accounts", "ESB-CRM");
                        //converting consent upload
                        bool isLetterOfIntroductionUploadOk = SharePointManager.UploadFilewithMetaData(LetterOfIntroductionUpload, LetterOfIntroductionUploadName, AppConfig.SPLibrary, item.CustomerID, item.CustomerID, "DBA Account", "Accounts", "ESB-CRM");
                        //birthcertificate
                        bool isBENFormOk = SharePointManager.UploadFilewithMetaData(BeneficiaryMeansOfIdentificationUpload, BeneficiaryMeansOfIdentificationUploadName, AppConfig.SPLibrary, item.CustomerID, item.CustomerID, "DBA Account", "Accounts", "ESB-CRM");
                        bool isAODok = SharePointManager.UploadFilewithMetaData(AccountOpeningFormUpload, AccountOpeningFormUploadName, AppConfig.SPLibrary, item.CustomerID, item.CustomerID, "DBA Account", "Accounts", "ESB-CRM");
                        if (isAODok == true && isBENFormOk == true && isLetterOfIntroductionUploadOk == true && isDeathCertificateUploadOk==true && isDeceasedPassportUploadOk==true)
                        {
                            Trace.TraceInformation($"completely converted all files");                          
                            item.isSharePointUploaded = true;
                        }

                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SharePointFileUploadWithMetaData_DataRecapture()
        {
            try
            {

                string Directoryfolder = string.Empty, PassportFileName = string.Empty, SignaturefileName = string.Empty, ConsentFormFileName = string.Empty, POIFileName = string.Empty, POAFileName = string.Empty, EvidenceOfEmployementFileName = string.Empty, BirthCertificateFileName = string.Empty, NOKPassportFileName = string.Empty, RSAFormFileName = string.Empty;
                byte[] Signature = null, Passport = null, Consent = null, POA = null, POI = null, Evidence = null, NOK = null, BirthCertificate = null, RsaForm = null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running Share Point FileUpload With MetaData schema for data recapture");
                    var accountsToProcess = context.CustomerDatarecapture.Where(x => x.Status == "PENCOM_APPROVED" && x.isSharePointUploaded == false).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running data recapture Share Point FileUpload schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Directoryfolder = $@"{AppConfig.DocumentManager}\{item.CustomerID}";
                        #region document upload
                        var extraDocument = context.DataRecaptureDocument.Where(x => x.Rsapin == item.RSAPIN.Trim()).FirstOrDefault();
                        if (extraDocument != null)
                        {
                            try
                            {
                                Signature = Convert.FromBase64String(item.SignatureConverted);
                                Passport = Convert.FromBase64String(item.PassportConverted);
                                Consent = Convert.FromBase64String(item.ConsentFormConverted);
                                #region birthcertificate
                                if (!string.IsNullOrEmpty(extraDocument.BirthCertificateUpload))
                                {
                                    if (extraDocument.BirthCertificateUpload.Contains("data:image/png;base64,"))
                                    {
                                        BirthCertificate = Convert.FromBase64String(extraDocument.BirthCertificateUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.BirthCertificateUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        BirthCertificate = Convert.FromBase64String(extraDocument.BirthCertificateUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region POI
                                if (!string.IsNullOrEmpty(extraDocument.ProofOfIdentityUpload))
                                {
                                    if (extraDocument.ProofOfIdentityUpload.Contains("data:image/png;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.ProofOfIdentityUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.BirthCertificateUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        POI = Convert.FromBase64String(extraDocument.ProofOfIdentityUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion                        }

                                #region POA
                                if (!string.IsNullOrEmpty(extraDocument.ProofOfAddressUpload))
                                {
                                    if (extraDocument.ProofOfAddressUpload.Contains("data:image/png;base64,"))
                                    {
                                        POA = Convert.FromBase64String(extraDocument.ProofOfAddressUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.ProofOfAddressUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        POA = Convert.FromBase64String(extraDocument.ProofOfAddressUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region NOK
                                if (!string.IsNullOrEmpty(extraDocument.NOKPassportUpload))
                                {
                                    if (extraDocument.NOKPassportUpload.Contains("data:image/png;base64,"))
                                    {
                                        NOK = Convert.FromBase64String(extraDocument.NOKPassportUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.NOKPassportUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        NOK = Convert.FromBase64String(extraDocument.NOKPassportUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region EvidenceOfEmployment
                                if (!string.IsNullOrEmpty(extraDocument.EvidenceOfEmploymentUpload))
                                {
                                    if (extraDocument.EvidenceOfEmploymentUpload.Contains("data:image/png;base64,"))
                                    {
                                        Evidence = Convert.FromBase64String(extraDocument.EvidenceOfEmploymentUpload.Replace("data:image/png;base64,", ""));
                                    }
                                    else if (extraDocument.EvidenceOfEmploymentUpload.Contains("data:image/jpeg;base64,"))
                                    {
                                        Evidence = Convert.FromBase64String(extraDocument.EvidenceOfEmploymentUpload.Replace("data:image/jpeg;base64,", ""));
                                    }
                                }
                                #endregion

                                #region RSAform
                                if (!string.IsNullOrEmpty(extraDocument.RSAFormUpload))
                                {
                                    if (extraDocument.RSAFormUpload.Contains("png"))
                                    {
                                        RsaForm = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}\rsaform.png"));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.png";
                                    }
                                    else if (extraDocument.RSAFormUpload.Contains("jpg"))
                                    {
                                        RsaForm = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}\rsaform.jpg"));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.jpg";
                                    }
                                    else if (extraDocument.RSAFormUpload.Contains("pdf"))
                                    {
                                        RsaForm = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}\rsaform.pdf"));
                                        RSAFormFileName = $"{item.RSAPIN}_RsaForm.pdf";
                                    }
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                            }

                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        //process each file                         
                        PassportFileName = $"{item.RSAPIN}_Passport.jpg";
                        SignaturefileName = $"{item.RSAPIN}_ Signature.jpg";
                        ConsentFormFileName = $"{item.RSAPIN}_ConsentForm.jpg";
                        POIFileName = $"{item.RSAPIN}_ProofOfIdentity.jpg";
                        POAFileName = $"{item.RSAPIN}_ProofOfAddress.jpg";
                        EvidenceOfEmployementFileName = $"{item.RSAPIN}_EvidenceOfEmployment.jpg";
                        BirthCertificateFileName = $"{item.RSAPIN}_BirthCertificate.jpg";


                        //converting passportupload
                        bool isPassportOk = SharePointManager.UploadFilewithMetaData(Passport, PassportFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        //converting signature upload 
                        bool isSignatureOk = SharePointManager.UploadFilewithMetaData(Signature, SignaturefileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        //converting consent upload
                        bool isConsentOk = SharePointManager.UploadFilewithMetaData(Consent, ConsentFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        //birthcertificate
                        bool isBirthCertificateOk = SharePointManager.UploadFilewithMetaData(BirthCertificate, BirthCertificateFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        bool isEmploymentOk = SharePointManager.UploadFilewithMetaData(Evidence, EvidenceOfEmployementFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        bool isRSAFormOk = SharePointManager.UploadFilewithMetaData(RsaForm, RSAFormFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        bool isPOAk = SharePointManager.UploadFilewithMetaData(POA, POAFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        bool isPOIok = SharePointManager.UploadFilewithMetaData(POI, POIFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Data Recapture", "Accounts", "ESB-CRM");
                        if (isPassportOk == true && isSignatureOk == true && isConsentOk == true)
                        {
                            Trace.TraceInformation($"completely converted all files");
                            item.PassportUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{PassportFileName}";
                            item.SignatureUpload = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{SignaturefileName}";
                            item.ConsentFormConverted = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{ConsentFormFileName}";
                            item.isSharePointUploaded = true;
                        }

                        #endregion
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SharePointFileUploadWithMetaData_RecordUpdate()
        {
            try
            {

                string Directoryfolder = string.Empty, DocumentName = string.Empty;
                byte[] Document = null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running record update Share Point FileUpload With MetaData schema ");
                    var accountsToProcess = context.CustomerRecordUpdate.Where(x => x.Status == "PENCOM_APPROVED" && x.isSharePointUploaded == false).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running record update Share Point FileUpload schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Directoryfolder = $@"{AppConfig.DocumentManager}\RecordUpdate\{item.CustomerID}";
                        #region document upload
                        var extraDocument = context.RecordUpdateDocumentRequest.Where(x => x.CustomerID == item.CustomerID.Trim());
                        if (extraDocument != null)
                        {
                            foreach (var docs in extraDocument)
                            {
                                try
                                {
                                    #region Document
                                    if (!string.IsNullOrEmpty(docs.DocumentUrl))
                                    {
                                        if (docs.DocumentUrl.Contains("png"))
                                        {
                                            Document = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}/{docs.DocumentName}.png"));
                                            DocumentName = $"{docs.DocumentName}.png";
                                        }
                                        else if (docs.DocumentUrl.Contains("jpg"))
                                        {
                                            Document = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}/{docs.DocumentName}.png"));
                                            DocumentName = $"{docs.DocumentName}.jpg";
                                        }
                                        else if (docs.DocumentUrl.Contains("pdf"))
                                        {
                                            Document = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}/{docs.DocumentName}.png"));
                                            DocumentName = $"{docs.DocumentName}.pdf";
                                        }
                                    }
                                    #endregion

                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceInformation($"An error occurred on share point document upload see stack {ex.Message}; {ex?.StackTrace}");
                                }

                                //converting passportupload
                                bool isDocumentOk = SharePointManager.UploadFilewithMetaData(Document, DocumentName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Record Update", "Accounts", "ESB-CRM");
                                //converting signature upload 
                                if (isDocumentOk == true)
                                {                                   
                                    item.isSharePointUploaded = true;
                                }
                            }

                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                       
                        #endregion
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel SharePointFileUploadWithMetaData_Case()
        {
            try
            {

                string Directoryfolder = string.Empty, DocumentName = string.Empty;
                byte[] Document = null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running case upload Share Point FileUpload With MetaData schema ");
                    var accountsToProcess = context.CaseUpload.Where(x => x.IsCaseResolved==true && x.IsSharePointUploaded == false).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running record update Share Point FileUpload schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Directoryfolder = $@"{AppConfig.DocumentManager}\Cases\{item.CaseNo}";
                        #region document upload
                        if (!string.IsNullOrEmpty(item.FileName))
                        {   
                              Document = Convert.FromBase64String(FileConverter.ConvertFiletoBase64($"{Directoryfolder}/{item.FileName}"));
                                                
                              //converting passportupload
                              bool isDocumentOk = SharePointManager.UploadFilewithMetaData(Document, item.FileName, AppConfig.SPLibrary, item.CaseNo, item.CaseNo, "Case Logging", "Cases", "ESB-CRM");
                              //converting signature upload 
                              if (isDocumentOk == true)
                              {
                                  item.IsSharePointUploaded = true;
                              }                            
                        }
                        Trace.TraceInformation($"Processing case with case number {item.CaseNo}");
                        #endregion
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }



        public static ResponseModel SharePointFileUploadWithMetaData_ClientOnbaording()
        {
            try
            {
                string PassportFileName = string.Empty, SignaturefileName = string.Empty, BirthCertificateFileName = string.Empty, EvidenceOfEmploymentFileName=string.Empty, ProofOfIdentityFileName=string.Empty, ProofOfAddressFileName=string.Empty;
                byte[] Signature = null, Passport = null, BirthCertificate = null, EvidenceOfEmployment=null, ProofOfIdentity=null, ProofOfAddress=null;
                Customer customer = new Customer();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running Share Point FileUpload With MetaData schema for client onbaording");
                    var accountsToProcess = context.Client.Where(x => x.Status == "Approved" && x.IsDataComplete==true && x.CRMNotified==false).Take(10);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending file to push to share point ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running Share Point FileUpload schema total account to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        if (string.IsNullOrEmpty(item.CustomerID))
                        {
                            Trace.TraceInformation($"customer id  is empty for this account, skipping {item.MobileNumber}");
                            continue;
                        }
                        Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                       
                        PassportFileName = $"{item.CustomerID}_Passport.jpg";
                        SignaturefileName = $"{item.CustomerID}_Signature.jpg";
                        BirthCertificateFileName = $"{item.CustomerID}_BirthCertificate.jpg";

                        EvidenceOfEmploymentFileName = $"{item.CustomerID}_EvidenceOfEmployment.jpg";
                        ProofOfAddressFileName = $"{item.CustomerID}_ProofOfAddress.jpg";
                        ProofOfIdentityFileName = $"{item.CustomerID}_ProofOfIdentity.jpg";

                        //converting passportupload
                        if (!string.IsNullOrEmpty(item.PassportUpload))
                        {
                            Passport = Convert.FromBase64String(item.PassportUpload);
                            bool isPassportOk = SharePointManager.UploadFilewithMetaData(Passport, PassportFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                            item.PassportUrl = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{PassportFileName}";
                        }

                        if (!string.IsNullOrEmpty(item.SignatureUpload))
                        {
                            Signature = Convert.FromBase64String(item.SignatureUpload);
                            bool isSignatureOk = SharePointManager.UploadFilewithMetaData(Signature, SignaturefileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                            item.SignatureUrl = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{SignaturefileName}";
                        }

                        if (!string.IsNullOrEmpty(item.BirthCertificateUpload))
                        {
                            BirthCertificate = Convert.FromBase64String(item.BirthCertificateUpload);
                            bool isBirthCertificateOk = SharePointManager.UploadFilewithMetaData(BirthCertificate, BirthCertificateFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                            item.BirthCertificateUrl = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{BirthCertificateFileName}";
                        }

                        if (!string.IsNullOrEmpty(item.EvidenceOfEmploymentUpload))
                        {
                            //converting passportupload
                            EvidenceOfEmployment = Convert.FromBase64String(item.EvidenceOfEmploymentUpload);
                            bool isEmploymentOk = SharePointManager.UploadFilewithMetaData(EvidenceOfEmployment, EvidenceOfEmploymentFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                            item.ProofOfAddressUrl = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{ProofOfAddressFileName}";
                        }

                        if (!string.IsNullOrEmpty(item.ProofOfIdentityUpload))
                        {
                            //converting proof of identity upload 
                            ProofOfIdentity = Convert.FromBase64String(item.ProofOfIdentityUpload);
                            bool isIdentityOk = SharePointManager.UploadFilewithMetaData(ProofOfIdentity, ProofOfIdentityFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                            item.ProofOfIdentityUrl = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{ProofOfIdentityFileName}";
                        }

                        if (!string.IsNullOrEmpty(item.ProofOfAddressUpload))
                        {
                            //converting consent upload
                            ProofOfAddress = Convert.FromBase64String(item.ProofOfAddressUpload);
                            bool isAddressOk = SharePointManager.UploadFilewithMetaData(ProofOfAddress, ProofOfAddressFileName, AppConfig.SPLibrary, item.RSAPIN, item.CustomerID, "Pin Generation", "Accounts", "ESB-CRM");
                            item.EvidenceOfEmploymentUrl = $"{AppConfig.SPURL}/{AppConfig.SPLibrary}/{EvidenceOfEmploymentFileName}";
                        }
                           
                        var accountToUpdate = new CRMaccountKYCResponseModel()
                        {
                            RSAPIN = item.RSAPIN,
                            CustomerID = item.CustomerID,
                            PassportUrl = item.PassportUrl,
                            SignatureUrl = item.SignatureUrl,
                            BirthCertificateUrl = item.BirthCertificateUrl,
                            ProofOfAddressUrl=item.ProofOfAddressUrl,
                            ProofOfIdentityUrl=item.ProofOfIdentityUrl,
                            EvidenceOfEmploymentUrl=item.EvidenceOfEmploymentUrl
                        };
                        //CRM for updates
                        Trace.TraceInformation($"about updating CRM with ECRS pencom response pin generation for {item.CustomerID}"); Trace.TraceInformation("about updating CRM with ECRS pencom response");
                        var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateAccountWithKycUrls(accountToUpdate);
                        item.CRMNotified = true;
                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel GenerateDataECRSRecapture()
        {
            try
            {
                CustomerDatarecapture customer = new CustomerDatarecapture();
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running ECRS Data recapture generation schema"); 
                    var accountsToProcess = context.CustomerDatarecapture.Where(x => x.Status == "Pending" && x.IsFileConverted==true && x.TrialCount<=AppConfig.TrialCount).OrderByDescending(x=>x.ID).Take(AppConfig.VolumeSequence);
                    if (accountsToProcess.Count() <= 0 || accountsToProcess==null)
                    {
                        Trace.TraceInformation("No pending ECRS data recapture  to  process");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    Trace.TraceInformation($"running ECRS data recapture generation schema total account to process {accountsToProcess.Count()}"); 
                    foreach (var item in accountsToProcess)
                    {
                        
                       Trace.TraceInformation($"Processing account with form number {item.CustomerID}");
                        var pencomeStatus = new ExternalNode(new ApiPostAndGet(), new FileConverter()).PencomDataRecapture(item);

                        if (pencomeStatus.ResponseCode == "06")
                        {
                            continue;
                        }
                        customer = (CustomerDatarecapture)pencomeStatus.ResultObject;
                        item.PencomResponseCode = customer.PencomResponseCode;
                        item.PencomResponseMessage = customer.PencomResponseMessage;
                        item.SetID = customer.SetID;
                        item.IsDataRecaptured = false;
                        if (item.PencomResponseCode == "0" && item.PencomResponseMessage == "Submitted for Processing")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                            item.IsECRSProcessFailed = false;
                        }
                        else if (item.PencomResponseCode == "0" && item.PencomResponseMessage == "Accepted")
                        {
                            item.Status = "PENCOM_ACKNOWLEDGED";
                            item.IsECRSProcessFailed = false;
                        }
                        else if (item.PencomResponseMessage.Contains("Schema Validation error"))
                        {
                            item.IsECRSProcessFailed = true;
                        }
                       
                        Trace.TraceInformation($"pencom response for ECRS account form no {item.CustomerID};======> {item.PencomResponseMessage}======== {item.SetID ?? ""}  status={item.Status}"); Trace.TraceInformation($"pencom response for ECRS account form no {item.CustomerID};======> {item.PencomResponseMessage}======== {item.SetID ?? ""}");
                        var accountToUpdate = new CRMAccountResponseModel()
                        {
                            CoreSystemResponse = "",
                            PencomCode = item.PencomResponseCode,
                            PencomResponse = item.PencomResponseMessage,
                            CustomerID = item.CustomerID,
                            RSAPIN = item.RSAPIN,
                            isERSProcessFailed = item.IsECRSProcessFailed,
                            SetID = item.SetID
                        };
                        //CRM for updates
                        Trace.TraceInformation("about updating CRM with ECRS pencom response"); Trace.TraceInformation("about updating CRM with ECRS pencom response");
                        var CRMResult = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateDataRecaptureWithResponse(accountToUpdate);

                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully updated all changes ");
                    return ResponseDictionary.GetCodeDescription("03");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static bool ReturnDataRecaptureKycFiles()
        {
            try
            {
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("running customer attestation for data recapture and base 64 copy  mechanism");
                    var accountsToProcess = context.CustomerDatarecapture.Where(x => x.IsFileConverted == false && x.FileJobCount < AppConfig.TrialCount).Take(AppConfig.VolumeSequence).OrderByDescending(x => x.ID);
                    if (accountsToProcess.Count() <= 0)
                    {
                        Trace.TraceInformation("No pending record to copy or convert  process");
                        return false;
                    }
                    Trace.TraceInformation($"running ECRS file conversion schema total record to process {accountsToProcess.Count()}");
                    foreach (var item in accountsToProcess)
                    {
                        Trace.TraceInformation($"converting file  with form number {item.CustomerID}");

                        var accountdocument = context.DataRecaptureDocument.Where(x => x.Rsapin == item.RSAPIN).FirstOrDefault();
                        //  var clientdocument = context.Client.Where(x => x.CustomerID == item.CustomerID).FirstOrDefault();
                        if (accountdocument != null)
                        {
                            #region passport extraction
                            if (accountdocument.PassportUpload.Contains("data:image/png;base64,"))
                            {
                                item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/png;base64,", "");
                            }
                            else if (accountdocument.PassportUpload.Contains("data:image/jpeg;base64,"))
                            {
                                item.PassportConverted = accountdocument.PassportUpload.Replace("data:image/jpeg;base64,", "");
                            }
                            #endregion
                            #region signature extraction
                            if (accountdocument.SignatureUpload.Contains("data:image/png;base64,"))
                            {
                                item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/png;base64,", "");
                            }
                            else if (accountdocument.SignatureUpload.Contains("data:image/jpeg;base64,"))
                            {
                                item.SignatureConverted = accountdocument.SignatureUpload.Replace("data:image/jpeg;base64,", "");
                            }
                            #endregion
                            #region consent form conversion
                            string statename = string.Empty;
                            if (!string.IsNullOrEmpty(item.StateOfResidence))
                            {
                                statename = InMemory.StateList.FirstOrDefault(x => x.Code == item.StateOfResidence).Name;
                            }
                            item.ConsentFormConverted = GenerateAttestationForm(accountdocument.SignatureUpload, item.CustomerID, $"{item.FirstName} {item.LastName} {item.OtherName}", $"{item.HouseNo}, {item.StreetName}, {statename}");
                            #endregion
                            if (!string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.SignatureConverted) && !string.IsNullOrEmpty(item.ConsentFormConverted))
                            {
                                Trace.TraceInformation("changing status for isfile  generated to true");
                                item.IsFileConverted = true;
                            }

                        }

                    }
                    context.SaveChanges();
                    Trace.TraceInformation("successfully converted all files");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}"); Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return false;
            }
        }

        public static ResponseModel CreateLeadAsync()
        {
            try
            {
                Trace.TraceInformation("lead async job has been triggered");
                using (ARMPContext context = new ARMPContext())
                {
                    var leadToProcess = context.Lead.Where(x => x.Status == "Pending" && x.TrialCount <= AppConfig.TrialCount).OrderByDescending(x => x.ID).Take(AppConfig.VolumeSequence);
                    if (leadToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    foreach (var item in leadToProcess)
                    {
                        var result = new LeadConnector(new CrmApiConnector(),new CrmEntityConnector(new CrmApiConnector())).CreateLead_RealTime(item);
                        if (result.ResponseCode=="00")
                        {
                            item.Status = "Approved";
                          
                        }
                        item.TrialCount = item.TrialCount + 1;
                    }
                    context.SaveChangesAsync();
                    Trace.TraceInformation("lead job has been completed");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
              Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel CreateCaseAsync()
        {
            try
            {
                Trace.TraceInformation("case async job has been triggered");
                using (ARMPContext context = new ARMPContext())
                {
                    var leadToProcess = context.Case.Where(x => x.Status == "Pending" && x.TrialCount <= AppConfig.TrialCount).OrderByDescending(x=>x.ID).Take(AppConfig.VolumeSequence);
                    if (leadToProcess.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    foreach (var item in leadToProcess)
                    {
                        var result = new CaseConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).CreateCase_RealTime(item);
                        if (result.Result.ResponseCode == "00")
                        {
                            item.Status = "Approved";
                           
                        }
                        item.TrialCount = item.TrialCount + 1;
                    }
                    context.SaveChangesAsync();
                    Trace.TraceInformation("completed case job");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
              Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel BenefitDocumentConversionToServerUrl()
        {
            try
            {
                Trace.TraceInformation("firing Benefit Document Conversion  To  ServerUrl");
                string FilePath = string.Empty, ServerPath = string.Empty;
                using (ARMPContext context = new ARMPContext())
                {
                    var benefitDoc = context.ChannelBenefitRequestDocument.Where(x => x.isFileConverted==false && x.Count<=AppConfig.TrialCount).Take(AppConfig.VolumeSequence).OrderByDescending(x=>x.ID);
                    if (benefitDoc.Count() <= 0 || benefitDoc==null)
                    {
                        Trace.TraceInformation("No record found to process ");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }
                    foreach (var item in benefitDoc)
                    {
                        if (!string.IsNullOrEmpty(item.DocumentUrl))
                        {
                            FilePath = $@"{AppConfig.DocumentManager}\{item.BenefitNo}\{item.ID}_{item.DocumentName}";
                            string file = FileConverter.ConverBase64toFile(item.DocumentUrl, FilePath);
                            item.ServerUrl = $"{AppConfig.KycUrl}{Path.GetFileName(file)}";
                            Trace.TraceInformation($"converted file see folder and server ; folder={FilePath??"is empty"}; server url{item.ServerUrl??"is empty, conversion failed"}");
                            if (!string.IsNullOrEmpty(item.ServerUrl))
                            {                                
                                item.isFileConverted = true;
                                Trace.TraceInformation($"is file converted { item.isFileConverted}");
                            }                            
                        }
                        item.Count = item.Count + 1;
                    }
                    Trace.TraceInformation($"about saving context to database");
                    context.SaveChanges();
                    Trace.TraceInformation($"returning out of benefit document sequence");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel BenefitCoreSystemCreationJob()
        {
            try
            {
                Trace.TraceInformation("firing Benefit Core System Creation Job");               
                using (ARMPContext context = new ARMPContext())
                {
                    var benefitDoc = context.BenefitRequest.Where(x => x.Status=="Pending" &&  x.Count <= AppConfig.TrialCount).Take(AppConfig.VolumeSequence).OrderByDescending(x => x.ID);
                    if (benefitDoc.Count() <= 0 || benefitDoc == null)
                    {
                        Trace.TraceInformation("No record found to process for benefit record push");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }                   
                    foreach (var item in benefitDoc)
                    {
                        if (string.IsNullOrEmpty(item.BANumber))
                        {
                            item.BANumber = item.CRMRequestNumber;
                        }
                        var document = context.ChannelBenefitRequestDocument.Where(x => x.BenefitNo == item.CRMRequestNumber && x.isFileConverted==true).ToList();
                        if (document!=null)
                        {
                            var response = new ExternalNode(new ApiPostAndGet(), new FileConverter()).CreateBenefitRequest(item, document);
                            if (response.ResponseCode=="00")
                            {
                                item.Status = "Approved";
                                item.ResponseCode = "00";
                                item.ResponseMessage = "Benefit created";
                                item.NavRequestNumber = (string)response.ResultObject;
                                item.Count = item.Count + 1;
                            }
                            else
                            {
                                item.Status = "Pending";
                                item.ResponseCode = "06";
                                item.ResponseMessage = response.ResponseDescription;
                                item.Count = item.Count + 1;
                            }
                            new BenefitConnector(new CrmApiConnector(), new CrmEntityConnector()).UpdateBenefitRequestWithCoreSystem(item.CRMRequestNumber, item.ResponseMessage, item.NavRequestNumber);
                        }                      
                    }
                    Trace.TraceInformation($"about saving context to database");
                    context.SaveChanges();                   
                    Trace.TraceInformation($"returning out of benefit document sequence");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel BenefitRejectionRequestJob()
        {
            try
            {
                Trace.TraceInformation("benefit rejection request job has been triggered");
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("retrieving all pending benefit rejection requests from the database");
                    var benefitRejectionRequest = context.BenefitRejectionRequest.Where(x => x.Status == "Pending" && x.Trialcount <= AppConfig.TrialCount).OrderByDescending(x => x.ID).Take(AppConfig.VolumeSequence);

                    Trace.TraceInformation("checking that at least 1 record was retrieved");
                    if (benefitRejectionRequest.Count() <= 0)
                    {
                        Trace.TraceInformation("ending method after no record found");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }

                    Trace.TraceInformation("looping through retrieved records");
                    foreach (var item in benefitRejectionRequest)
                    {
                        Trace.TraceInformation("calling the update benefit request method on the retrieved information");
                        var result = new BenefitConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).UpdateBenefitRequest(item.BenefitNo, item.StatusCode, item.RejectionReason, item.RejectionComment, item.RejectedBy);

                        Trace.TraceInformation("confirming method success");
                        if (result.ResponseCode == "00")
                        {
                            Trace.TraceInformation("set database status");
                            item.Status = "Approved";

                        }
                        Trace.TraceInformation("update count");
                        item.Trialcount = item.Trialcount + 1;
                    }
                    Trace.TraceInformation("saving database");
                    context.SaveChanges();
                    Trace.TraceInformation("benefit rejection request job has been completed");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static ResponseModel EmployerCreationCoreSystemJob()
        {
            try
            {
                Trace.TraceInformation("employer core system creation job has been triggered");
                using (ARMPContext context = new ARMPContext())
                {
                    Trace.TraceInformation("retrieving all employer accounts yet to be sent to core from the database");
                    var employerAccount = context.Employer.Where(x => x.IsAccountOnCoreSystem == false && x.CoreTrialCount <= AppConfig.TrialCount).OrderByDescending(x => x.ID).Take(AppConfig.VolumeSequence);

                    Trace.TraceInformation("checking that at least 1 record was retrieved");
                    if (employerAccount.Count() <= 0)
                    {
                        Trace.TraceInformation("ending method after no record found");
                        return ResponseDictionary.GetCodeDescription("03", "No record found to process");
                    }

                    Trace.TraceInformation("looping through retrieved records");
                    foreach (var item in employerAccount)
                    {
                        Trace.TraceInformation("calling the employer account fetch by customer ID method on the retrieved information");
                        var result = new ClientConnector(new CrmApiConnector(), new CrmEntityConnector(new CrmApiConnector())).FetchEmployerByCustomerID(item.CustomerID);

                        Trace.TraceInformation("confirming method success");
                        if (result.Result.ResponseCode == "00")
                        {
                            Trace.TraceInformation("parse the object as a type of Employer");
                            var employer = (EmployerAccountModel)result.Result.ResultObject;
                            if (!string.IsNullOrEmpty(employer.EmployerCode) && !string.IsNullOrEmpty(employer.PencomEmployerName) && employer.Status == "06")
                            {                               
                                if (!string.IsNullOrEmpty(employer.EmployerIndustry))
                                {
                                    employer.EmployerIndustry = InMemory.IndustryList.Where(x => x.GUID == employer.EmployerIndustry).FirstOrDefault().Reference;
                                }
                                Trace.TraceInformation("send the resulting employer details retrieved from CRM to NAV");
                                var createEmployer = new ExternalNode(new ApiPostAndGet(), new FileConverter()).CreateCoperateEmployerCoreNew(employer, false);
                                Trace.TraceInformation("confirming method success");
                                if (createEmployer.ResponseCode == "00")
                                {
                                    Trace.TraceInformation("update database");
                                    item.EmployerCode = employer.EmployerCode;
                                    item.PencomEmployerName = employer.PencomEmployerName;
                                    item.IsAccountOnCoreSystem = true;
                                }
                            }


                        }
                        Trace.TraceInformation("update count");
                        item.CoreTrialCount = item.CoreTrialCount + 1;
                    }
                    Trace.TraceInformation("saving database");
                    context.SaveChanges();
                    Trace.TraceInformation("employer account core system creation has been completed");
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

      
    }
}
