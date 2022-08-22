using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts
{
    public class Dropdown : EntityStatus, IDropdown
    {
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string GUID { get; set; }
        [StringLength(200)]
        public string Reference { get; set; }
    }
}
