using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface IRelayLogger : IEntity
    {
        string URL { get; set; }
        string ControllerName { get; set; }
        string ActionName { get; set; }
        string Request { get; set; }
        string Response { get; set; }
        DateTime Date { get; set; }
        int ResponseRate { get; set; }
    }
}
