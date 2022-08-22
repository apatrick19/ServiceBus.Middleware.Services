using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface INotificationLogger : IEntity
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Message { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        DateTime DateCommitted { get; set; }
        DateTime DateSent { get; set; }
        Entity_Status Status { get; set; }
    }
}
