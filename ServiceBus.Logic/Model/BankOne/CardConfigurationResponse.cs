using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
  
    public class CardProfile
    {
        public string DisplayName { get; set; }
        public string BIN { get; set; }
        public string Description { get; set; }
        public string SponsorBank { get; set; }
    }

    public class DeliveryOption
    {
        public string OptionName { get; set; }
        public string Status { get; set; }
        public string DeliveryDescription { get; set; }
    }

    public class CardConfigurationResponse
    {
        public bool IsSuccessful { get; set; }
        public string ResponseDescription { get; set; }
        public List<CardProfile> CardProfiles { get; set; }
        public List<DeliveryOption> DeliveryOptions { get; set; }
        public List<string> RequestType { get; set; }
    }

}
