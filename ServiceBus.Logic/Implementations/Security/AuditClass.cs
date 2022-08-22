using ServiceBus.Core.Model.Generic;
using ServiceBus.Data.ORM.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Implementations.Security
{
   public static class AuditClass
    {
        public static bool AuditLog(string username, string action, string module, string key, string description, string output)
        {
            try
            {
                using (AiroPayContext context=new AiroPayContext())
                {
                    AuditTrail audit = new AuditTrail();
                    audit.Action = action;
                    audit.Module = module;
                    audit.DateCommitted = DateTime.Now;
                    audit.Description = description;
                    audit.Name = username;
                    audit.Output = output;
                    audit.CustomerID = key;
                    context.AuditTrail.Add(audit);
                    context.SaveChanges();
                    return true; 
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"an error occurred {ex}; {ex.Message}; {ex.StackTrace}");
                return false;
            }
        }
    }
}
