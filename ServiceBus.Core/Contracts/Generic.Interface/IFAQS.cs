using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
   public interface IFAQS:IEntity
    {
         string Question { get; set; }
         string Answer { get; set; }
    }
}
