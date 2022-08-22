using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class FAQS : Entity, IFAQS
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
