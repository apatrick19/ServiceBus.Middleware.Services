using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Exceptions : Entity, IExceptions
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string InnerException { get; set; }
        public string Caller { get; set; }
        public string Method { get; set; }
        public string Class { get; set; }
        public DateTime Date { get; set; }
        public string Library { get; set; }
        public string ActionRequired { get; set; }
    }
}
