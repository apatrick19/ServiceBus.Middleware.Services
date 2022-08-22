using ServiceBus.Core.Contracts;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class State : Dropdown, IState
    {
        [StringLength(50)]
        public string Code { get; set; }
    }
}
