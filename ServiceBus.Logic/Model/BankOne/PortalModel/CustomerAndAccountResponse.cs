using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PortalModel.CustomerSpace
{
    public class CustomerAndAccountResponse
    {
        public CustomerDetails CustomerDetails { get; set; }
        public List<Account> Accounts { get; set; }
    }

    public class CustomerDetails
    {
        public string Address { get; set; }
        public string Age { get; set; }
        public string BVN { get; set; }
        public string CustomerID { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string LocalGovernmentArea { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
    }

    public class Account
    {
        public string AccessLevel { get; set; }
        public string AccountNumber { get; set; }
        public string AccountStatus { get; set; }
        public string AccountType { get; set; }
        public string AccountBalance { get; set; }
        public string Branch { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string DateCreated { get; set; }
        public string LastActivityDate { get; set; }
        public string NUBAN { get; set; }
        public string Refree1CustomerID { get; set; }
        public string Refree2CustomerID { get; set; }
        public string ReferenceNo { get; set; }
    }
}
