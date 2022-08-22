using Newtonsoft.Json;
using Servicebus.SMS.Gateway.Integration;
using Servicebus.SMS.Gateway.Models;
using ServiceBus.SMS.Gateway.Migration;
using ServiceBus.SMS.Gateway.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Servicebus.SMS.Gateway.Service
{
    public class MessageService
    {
        public static ResponseModel SendSMS(SMSRequest request)
        {
            try
            {
                using (MessagingContext context=new MessagingContext())
                {
                    if (string.IsNullOrEmpty(request.MobileNo))
                    {
                        return new ResponseModel()
                        {
                            ResponseCode = "04",
                            ResponseMessage = "Invalid Mobile No"
                        };
                    }

                    if (request.MobileNo.Length<11)
                    {
                        return new ResponseModel()
                        {
                            ResponseCode = "04",
                            ResponseMessage = "Invalid Mobile No Length"
                        };
                    }

                    if (string.IsNullOrEmpty(request.Message))
                    {
                        return new ResponseModel()
                        {
                            ResponseCode = "04",
                            ResponseMessage = "Message cannot be empty"
                        };
                    }
                    var dbModel = new Messaging();
                    dbModel.MobileNo = request.MobileNo;
                    dbModel.Message = request.Message;
                    #region compose url
                    string Url = string.Concat(BaseService.GetAppSetting("BaseUrl"), $"?api_token={BaseService.GetAppSetting("Token")}",$"&from={BaseService.GetAppSetting("SenderID")}");
                    Url += $"&to={request.MobileNo}";
                    Url += $"&body={request.Message}";
                    Url += $"&dnd={BaseService.GetAppSetting("DndSetting")}";
                    #endregion

                    //call message
                    var response = MessageIntegration.UrlGet(Url);
                    if (string.IsNullOrEmpty(response))
                    {
                        dbModel.Status = 0;
                        dbModel.ThirdPartyResponse = "";
                        dbModel.SystemResponse = "No response from server";
                        dbModel.RetrialCount = 1;
                        //log request as pending  
                    }
                    var Jresponse = JsonConvert.DeserializeObject<MessageResponse>(response);
                    if (Jresponse==null)
                    {
                        dbModel.Status = 0;
                        dbModel.ThirdPartyResponse = "";
                        dbModel.SystemResponse = "Derialization failed";
                        dbModel.RetrialCount = 1;
                    }

                    if (Jresponse.data.status== "success" && Jresponse.data.message== "Message Sent")
                    {
                        dbModel.Status = 1;
                        dbModel.ThirdPartyResponse = Jresponse.data.status;
                        dbModel.SystemResponse = Jresponse.data.message;
                        dbModel.RetrialCount = 1;
                    }
                    else
                    {
                        dbModel.Status = 2;
                        dbModel.ThirdPartyResponse = Jresponse.data.status;
                        dbModel.SystemResponse = Jresponse.data.message;
                        dbModel.RetrialCount = 1;
                    }
                    context.Messaging.Add(dbModel);
                    context.SaveChanges();
                    return new ResponseModel()
                    {
                        ResponseCode = dbModel.Status==1?"00":"06",
                        ResponseMessage = dbModel.SystemResponse,
                         ResponseObject=dbModel.ThirdPartyResponse
                    };
                  
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("error" + ex.ToString());
                return new ResponseModel()
                {
                    ResponseCode = "06",
                    ResponseMessage = "An error occurred"
                };
            }
        }

        public static ResponseModel SendSMSTermii(SMSRequest request)
        {
            try
            {
                using (MessagingContext context = new MessagingContext())
                {
                    if (string.IsNullOrEmpty(request.MobileNo))
                    {
                        return new ResponseModel()
                        {
                            ResponseCode = "04",
                            ResponseMessage = "Invalid Mobile No"
                        };
                    }

                    if (request.MobileNo.Length <11)
                    {
                        return new ResponseModel()
                        {
                            ResponseCode = "04",
                            ResponseMessage = "Invalid Mobile No Length"
                        };
                    }

                    if (string.IsNullOrEmpty(request.Message))
                    {
                        return new ResponseModel()
                        {
                            ResponseCode = "04",
                            ResponseMessage = "Message cannot be empty"
                        };
                    }
                    if (request.MobileNo.StartsWith("0"))
                    {
                        request.MobileNo = "234" + request.MobileNo.Substring(1,10);
                    }

                    var dbModel = new Messaging();
                    dbModel.MobileNo = request.MobileNo;
                    dbModel.Message = request.Message;
                    #region compose url
                    string Url =BaseService.GetAppSetting("TermiBaseUrl");

                    var payload = new
                    {
                        to = request.MobileNo,
                        from = BaseService.GetAppSetting("TermiSenderID"),
                        sms = request.Message,
                        type = "plain",
                        channel = "dnd",
                        api_key = BaseService.GetAppSetting("TermiToken")
                    };

                    #endregion
                    //call message
                    var response = MessageIntegration.UrlPost<SMSResponse>(Url, payload);
                   
                    if (response == null)
                    {
                        dbModel.Status = 0;
                        dbModel.ThirdPartyResponse = "";
                        dbModel.SystemResponse = "Derialization failed";
                        dbModel.RetrialCount = 1;
                    }

                    if (response.code == "ok" && response.message == "Successfully Sent")
                    {
                        dbModel.Status = 1;
                        dbModel.ThirdPartyResponse = response.message;
                        dbModel.SystemResponse = response.message;
                        dbModel.RetrialCount = 1;
                    }
                    else
                    {
                        dbModel.Status = 2;
                        dbModel.ThirdPartyResponse = response.message;
                        dbModel.SystemResponse = response.message;
                        dbModel.RetrialCount = 1;
                    }
                    context.Messaging.Add(dbModel);
                    context.SaveChanges();
                    return new ResponseModel()
                    {
                        ResponseCode = dbModel.Status == 1 ? "00" : "06",
                        ResponseMessage = dbModel.SystemResponse,
                        ResponseObject = dbModel.ThirdPartyResponse
                    };

                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("error" + ex.ToString());
                return new ResponseModel()
                {
                    ResponseCode = "06",
                    ResponseMessage = "An error occurred"
                };
            }
        }
    }
}
