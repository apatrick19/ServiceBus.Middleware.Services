using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Implementations
{
  public   class Messenger
    {
        public static bool SendEmail(string Subject, string Message, string Email)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("patrickabiodun@gmail.com", "L@g0s123$%"),
                    EnableSsl = true
                };
                client.Send("patrickabiodun@gmail.com", Email, Subject, Message);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred"+ex);
                return false;
            }
        }
    }
}
