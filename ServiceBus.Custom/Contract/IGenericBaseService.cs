using ServiceBus.Core.Model.CRM;
using ServiceBus.Core.Model.Generic;

using ServiceBus.Logic.Implementations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServiceBus.Custom.Contract
{
    public interface IGenericBaseService
    {
        ResponseModel GetAllGender();
        ResponseModel GetAllClientType();
        ResponseModel GetAllResponseDecription();
        ResponseModel GetAllState();
        ResponseModel GetAllSector();
        ResponseModel GetAllCountry();
        ResponseModel GetLGAByState(string stateCode);
        ResponseModel GetLGA();
        ResponseModel GetAllTitle();
        ResponseModel GetAllMaritalStatus();
        ResponseModel GetAllRelationship();
        ResponseModel GetAllBank();
        ResponseModel GetBranchByState(string StateCode);
        ResponseModel GetAllChannelSource();
              
        ResponseModel GetAllStatementOption();
        ResponseModel GetAllProofOfIdentity();
        ResponseModel GetAllProofOfAddress();
        ResponseModel GetAllQualification();
        ResponseModel GetAllCustomerKYCCategory();
        ResponseModel GetAllAccountStatus();
         ResponseModel GetAllCompetitor();
    
        ResponseModel GetAllNationality();
        ResponseModel LogComplaints(Complaints issue);
        ResponseModel GetFAQs();
        ResponseModel GetProducts();

    }
}
