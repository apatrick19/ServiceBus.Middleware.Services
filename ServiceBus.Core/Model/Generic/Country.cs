using ServiceBus.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Country : Dropdown, ICountry
    {
       public string Code { get; set; }

       public string NationalityCode { get; set; }
    }
}
