//using Hangfire;
using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Core.Model;
using ServiceBus.Core.Model.CRM;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.Custom.Contract;
using ServiceBus.Data.Contracts;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Implementation
{
    /// <summary>
    /// esb generic base service
    /// </summary>
    public class GenericBaseService : IGenericBaseService
    {
        IServiceDapper dapperConn;
        IAccountValidationService accountValidation;

        /// <summary>
        /// initializing the generic base service
        /// </summary>
        /// <param name="serviceDapper"></param>
        public GenericBaseService(IServiceDapper serviceDapper, IAccountValidationService accountValidationService)
        {
            dapperConn = serviceDapper;
            accountValidation = accountValidationService;
        }

        /// <summary>
        /// This method retrieves all countries from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllCountry()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<Country>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all titles from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllTitle()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<Title>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }                
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }                
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all marital status options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllMaritalStatus()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<MaritalStatus>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all gender options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllGender()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<Gender>();
                if (result.Count()<=0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                   Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);              
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all client types from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllClientType()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<ClientType>();
                if (result.Count()<=0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                   Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);              
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all states from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllState()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<State>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all sectors from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllSector()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<Sector>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all relationship options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllRelationship()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                using (AiroPayContext context = new AiroPayContext())
                {
                   // var result = dapperConn.GetAllGenericEnitities<Relationship>();
                    var result = context.Relationship.ToList();
                    if (result.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("04");
                    }
                    foreach (var item in result)
                    {
                        Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference, ThirdBind = item.genderCode });
                    }
                    return ResponseDictionary.GetCodeDescription("00", Model);
                }
                
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetAuditTrail(string key)
        {
            try
            {
                List<AuditTrail> audit = new List<AuditTrail>();
                using (AiroPayContext context = new AiroPayContext())
                {
                    // var result = dapperConn.GetAllGenericEnitities<Relationship>();
                    var result = context.AuditTrail.Where(x=>x.CustomerID==key.Trim());
                    if (result.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("04");
                    }
                    
                    return ResponseDictionary.GetCodeDescription("00", result.ToList());
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all banks from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllBank()
        {
            try
            {
                return accountValidation.GetBankOneBanks();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all armp branches from the database based on the state code provided
        /// </summary>
        /// <param name="StateCode"></param>
        /// <returns></returns>
        public ResponseModel GetBranchByState(string StateCode)
        {
            try
            {
                List<BranchResponseModel> Model = new List<BranchResponseModel>();
                using (AiroPayContext context = new AiroPayContext())
                {
                    var states = context.Branch.Where(x=>x.StateCode== StateCode).ToList();
                    if (states.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("04");
                    }
                    foreach (var item in states)
                    {
                        Model.Add(new BranchResponseModel() { Name = item.Name, Code = item.Reference, StateCode = StateCode, Address = item.Address, Latitude = item.Latitude, Longitude = item.Longitude });
                    }
                    return ResponseDictionary.GetCodeDescription("00", Model);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all channel sources from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllChannelSource()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<ChannelSource>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

       

     

        /// <summary>
        /// This method retrieves all local government areas from the database based on the state code provided
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public ResponseModel GetLGAByState(string stateCode)
        {
            try
            {
               
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                using (AiroPayContext context = new AiroPayContext())
                {
                    var LGAs = context.Lga.Where(x => x.StateCode == stateCode).ToList();
                    if (LGAs.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("04");
                    }  
                    foreach (var item in LGAs)
                    {
                        Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                    }
                    return ResponseDictionary.GetCodeDescription("00", Model);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all local government areas from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetLGA()
        {
            try
            {

                  using (AiroPayContext context = new AiroPayContext())
                {
                    var LGAs = context.Lga.ToList();
                    if (LGAs.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("04");
                    }
                    
                    return ResponseDictionary.GetCodeDescription("00", LGAs);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all statement options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllStatementOption()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<StatementOption>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all proof of identity options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllProofOfIdentity()
        {
            try
            {

                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<ProofOfIdentity>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all proof of address options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllProofOfAddress()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<ProofOfAddress>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all qualification options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllQualification()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<Qualification>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all response descriptions from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllResponseDecription()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                using (AiroPayContext context = new AiroPayContext())
                {
                    var states = context.Response.ToList();
                    if (states.Count() <= 0)
                    {
                        return ResponseDictionary.GetCodeDescription("04");
                    }
                    foreach (var item in states)
                    {
                        Model.Add(new GenericResponseModel() { Name = item.ResponseDescription, Code = item.ResponseCode });
                    }
                    return ResponseDictionary.GetCodeDescription("00", Model);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all customer kyc categories from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllCustomerKYCCategory()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<CustomerKYCCategory>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        /// <summary>
        /// This method retrieves all account status options from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllAccountStatus()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<AccountStatus>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

     

        /// <summary>
        /// This method retrieves all competitors from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllCompetitor()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<Competitor>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

       

        /// <summary>
        /// This method retrieves all nationalities from the database
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetAllNationality()
        {
            try
            {
                List<GenericResponseModel> Model = new List<GenericResponseModel>();
                var result = dapperConn.GetAllGenericEnitities<Nationality>();
                if (result.Count() <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04");
                }
                foreach (var item in result)
                {
                    Model.Add(new GenericResponseModel() { Name = item.Name, Code = item.Reference });
                }
                return ResponseDictionary.GetCodeDescription("00", Model);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel LogComplaints(Complaints issue)
        {
            try
            {
                using (AiroPayContext context=new AiroPayContext())
                {
                    context.Complaints.Add(issue);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00","issue/request has been logged successfully");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetFAQs()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var Faqs = dapperConn.GetAllRecordsByCount<FAQS>(10).ToList();
                    if (Faqs != null)
                    {
                        return ResponseDictionary.GetCodeDescription("04");
                    }
                    return ResponseDictionary.GetCodeDescription("00", Faqs);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetProducts()
        {
            try
            {
                return accountValidation.GetProducts();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
    }
}
