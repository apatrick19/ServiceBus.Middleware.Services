using ServiceBus.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
   public interface IMerchantProduct:IDropdown
    {
         string MercantCode { get; set; }
    }
}
