﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
  public   class PlaceLienModel
    {
        public string AccountNo { get; set; }
        public string AuthenticationCode { get; set; }
        public string ReferenceID { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
    }
}
