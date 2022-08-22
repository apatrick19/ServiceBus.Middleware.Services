using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface IExceptions : IEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        string InnerException { get; set; }
        string Caller { get; set; }
        string Method { get; set; }
        string Class { get; set; }
        DateTime Date { get; set; }
        string Library { get; set; }
        string ActionRequired { get; set; }
    }
}
