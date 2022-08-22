using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class TransactionHistoryBaseRequest
    {
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public int NumberOfItems { get; set; }
    }


    public class TransactionRequest:Request
    {
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
         public string Email { get; set; }
      
    }

   
}
