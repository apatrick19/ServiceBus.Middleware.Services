using ServiceBus.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Entity : IEntity
    {
        /// <summary>
        /// table ID
        /// </summary>
        [Key]
        public long ID { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.Now;

        public Entity()
        {
            this.DateCreated = DateTime.UtcNow;
        }

    }
}
