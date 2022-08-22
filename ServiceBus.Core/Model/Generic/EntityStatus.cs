using ServiceBus.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    /// <summary>
    /// Generic Status Class
    /// </summary>
    public class EntityStatus : Entity, IEntityStatus
    {
        /// <summary>
        /// Status
        /// </summary>
        [StringLength(100)]
        public string Status { get; set; }
        public string StatusName { get; set; }
    }
}
