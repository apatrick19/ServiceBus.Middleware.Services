using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
   public class UserType:EntityStatus
    {
        [StringLength(200), MaxLength(200)]
        public string Name { get; set; }
        [StringLength(200), MaxLength(500)]
        public string Description { get; set; }
        public bool IsAdmin { get; set; }
    }
}
