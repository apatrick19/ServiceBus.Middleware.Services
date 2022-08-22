using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts
{
   public  interface ILoanIntegration
    {
        ResponseModel GetLoanProduct ();
        ResponseModel GetAllApprovedLoans ();
        ResponseModel GetCompletedLoan ();
        ResponseModel GetLoanBalance (string AccountNumber);
        ResponseModel LoanRequest(LoanRequestModel request);
    }
}
