using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class TokenGen : EntityStatus
    {
        [StringLength(200)]
        public string Token { get; set; }
        public bool IsExpired { get; set; }
        public DateTime DateGenerated { get; set; }
        public int DurationInSeconds { get; set; }
        [StringLength(100)]

        public string AccountNumber { get; set; }
        [StringLength(100)]

        public string MobileNumber { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        public bool MessageStatus { get; set; }
        public bool isUtilized { get; set; }
        public DateTime DateUtilized { get; set; }
        [StringLength(200)]
        public string Purpose { get; set; }
    }
}
