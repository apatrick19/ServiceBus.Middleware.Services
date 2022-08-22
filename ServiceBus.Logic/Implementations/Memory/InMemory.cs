using ServiceBus.Core.Model;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.CRM;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.Data.ORM.EntityFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Implementations.Memory
{
  public static class InMemory
    {

        public static string EmailTemplate;



        public static readonly Dictionary<string, string> ChannelSources = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> CustomerKYCCategoryDic = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> PencomeResponse = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> Descriptions = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> States = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> LGAs = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> Branches = new Dictionary<string, string>();

        public static readonly List<Branch> BranchList = new List<Branch>();

        public static readonly Dictionary<string, string> CaseCategory = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> CaseType = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> CaseSubCategory = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> BenefitRequestTypes = new Dictionary<string, string>();

        public static List<State> StateList = new List<State>();
        public static List<Lga> LgaList = new List<Lga>();
       

        public static readonly Dictionary<string, string> CRMStatusCode = new Dictionary<string, string>();

        public static readonly Dictionary<string, string> Gender = new Dictionary<string, string>();

        public static List<Gender> GenderList = new List<Gender>();
        public static List<AccountTier> AccountTier = new List<AccountTier>();

     
        public static List<ClientType> ClientTypeList = new List<ClientType>();

        public static List<Relationship> RelationshipList = new List<Relationship>();

        public static readonly Dictionary<string, string> Title = new Dictionary<string, string>();

        public static List<Title> TitleList = new List<Title>();

        public static readonly Dictionary<string, string> Country = new Dictionary<string, string>();

        public static List<Country> CountryList = new List<Country>();

        public static List<Bank> BankList = new List<Bank>();

        public static List<Sector> SectorList = new List<Sector>();

        public static List<StatementOption> StatementOptionList = new List<StatementOption>();

        public static readonly Dictionary<string, string> MaritalStatus = new Dictionary<string, string>();

        public static List<MaritalStatus> MaritalList = new List<MaritalStatus>();

      
        public static List<AccountStatus> AccountStatusList = new List<AccountStatus>();

       
        public static List<PaymentFrequency> PaymentFrequencyList = new List<PaymentFrequency>();

        public static List<ModeOfExit> ModeOfExitList = new List<ModeOfExit>();

   
        public static List<RemittancePattern> RemittancePatternList = new List<RemittancePattern>();

       
        public static List<Competitor> CompetitorList = new List<Competitor>();

      
         public static List<Industry> IndustryList = new List<Industry>();

      
        public static List<ChannelSource> ChannelSourceList = new List<ChannelSource>();

        public static List<Nationality> NationalityList = new List<Nationality>();

        public static List<Team> TeamList = new List<Team>();

      
        public static List<ChannelActivitiesMapping> ChannelActivitiesMappingList = new List<ChannelActivitiesMapping>();

        public static List<Channels> ChannelsList = new List<Channels>();

        public static List<CommunicationMethod> CommunicationMethodList = new List<CommunicationMethod>();

        public static List<Blocked> BlockedList = new List<Blocked>();

       

        public static string ConvertToPencomMaritalStatus(string status)
        {
            try
            {
               return InMemory.MaritalList.FirstOrDefault(x => x.GUID == status).PencomCode;               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                return string.Empty;
            }
        }

        public static string ConvertToPencomNationality(string nationality)
        {
            try
            {
                if (string.IsNullOrEmpty(nationality))
                {
                    return string.Empty;
                }
                try
                {
                    var  Nationailty = InMemory.NationalityList.Where(x => x.Name.Contains(nationality)).FirstOrDefault();
                    if (Nationailty!=null)
                    {
                        string NationailtyCode = Nationailty.Code;
                        return NationailtyCode;
                    }
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                    return string.Empty;
                }               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                return string.Empty;
            }
        }

        public static string GenerateSequence()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode()).ToString();
        }

        // public static  List<BenefitRequestType> BenefitRequestTypes = new List<BenefitRequestType>();
        public static void LoadEmailTemplate()
        {
            try
            {               
                string file = File.ReadAllText($@"{AppConfig.EmailTemplate}");
                file = file.Replace("\r\n", "<br>").Trim();
                for (int i = 0; i < 11; i++)
                {
                    file = file.Replace(i.ToString(),"");
                }
                EmailTemplate = file;
                Trace.TraceInformation("Email template successfully loaded");
               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        public static string LoadGenericTemplate(string filepath)
        {
            try
            {
                string file = File.ReadAllText($@"{filepath}");
                file = file.Replace("\r\n", "<br>").Trim();
                for (int i = 0; i < 11; i++)
                {
                    file = file.Replace(i.ToString(), "");
                }                
                Trace.TraceInformation("Email template successfully loaded");
                return file;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                return string.Empty;
            }
        }

        //public static void LoadTeams()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.Team;
        //            foreach (var item in result)
        //            {
        //                TeamList.Add(new Team() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status, AccountTeam= item.AccountTeam, LeadTeam=item.LeadTeam, CaseTeam=item.CaseTeam, DRTeam=item.DRTeam, BenefitTeam=item.BenefitTeam, RecordUpdateTeam=item.RecordUpdateTeam });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}
        public static void LoadStatementOption()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.StatementOption;
                    foreach (var item in result)
                    {
                        StatementOptionList.Add(new StatementOption() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        public static void LoadAccountTier()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.AccountTier;
                    foreach (var item in result)
                    {
                        AccountTier.Add(new ServiceBus.Core.Model.Bank.AccountTier() { Name=item.Name,Tier=item.Tier, MaximumAmount=item.MaximumAmount });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }



        //public static void LoadChannelActivitiesMappingList()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.ChannelActivitiesMapping;
        //            foreach (var item in result)
        //            {
        //                ChannelActivitiesMappingList.Add(new ChannelActivitiesMapping() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

        //public static void LoadChannelActivitiesList()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.Channels;
        //            foreach (var item in result)
        //            {
        //                ChannelsList.Add(new Channels() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

      

       
       
       
        
        //public static void LoadClientType()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.ClientType;
        //            foreach (var item in result)
        //            {
        //                ClientTypeList.Add(new ClientType() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

        public static void LoadAccountStatus()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.AccountStatus;
                    foreach (var item in result)
                    {
                        AccountStatusList.Add(new AccountStatus() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

       

        public static void LoadRealtionship()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Relationship;
                    foreach (var item in result)
                    {
                        RelationshipList.Add(new Relationship() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        //public static void LoadCustomerKYCCategory()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.CustomerKYCCategory;
        //            foreach (var item in result)
        //            {
        //                CustomerKYCCategoryDic.Add(item.Reference, item.GUID);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}


        //public static void LoadSector()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.Sector;
        //            foreach (var item in result)
        //            {
        //                SectorList.Add(new Sector() {  GUID=item.GUID, Name=item.Name, Reference=item.Reference, Status=item.Name});
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}



        /// <summary>
        /// Loads all response codes and descrition from the DB by default 
        /// </summary>
        public static void LoadChannelSources()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.ChannelSource;
                    foreach (var item in result)
                    {
                        ChannelSources.Add(item.Reference, item.GUID);
                        ChannelSourceList.Add(new ChannelSource() { Name = item.Name, GUID = item.GUID, Reference = item.Reference, Status = item.Status });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }


        public static void LoadBank()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Bank;
                    foreach (var item in result)
                    {
                       
                        BankList.Add(new ServiceBus.Core.Model.Generic.Bank()
                        {
                            Name = item.Name,
                            GUID = item.GUID,
                            Reference = item.Reference,
                            ID = item.ID
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }


        public static void LoadMaritalStatus()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.MaritalStatus;
                    foreach (var item in result)
                    {
                        MaritalStatus.Add(item.Reference, item.GUID);
                        MaritalList.Add(new ServiceBus.Core.Model.Generic.MaritalStatus()
                        {
                            Name = item.Name,
                            GUID = item.GUID,                             
                            Reference = item.Reference,
                            PencomCode=item.PencomCode,
                            ID = item.ID,
                            NavCode=item.NavCode
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        /// <summary>
        /// Loads all response codes and descrition from the DB by default 
        /// </summary>
        public static void LoadCodeDescription()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Response;
                    foreach (var item in result)
                    {
                        Descriptions.Add(item.ResponseCode, item.ResponseDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

       

        public static void LoadState()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.State;
                    foreach (var item in result)
                    {
                        States.Add(item.Code, item.GUID);
                        StateList.Add(new State() { Name = item.Name, Code = item.Code, GUID = item.GUID, Reference = item.Reference, Status = item.Status });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load state Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        public static void LoadLGA()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Lga;
                    foreach (var item in result)
                    {
                        LGAs.Add(item.Reference, item.GUID);
                        LgaList.Add(new Lga() { Name=item.Name, GUID=item.GUID, Reference=item.Reference, Code =item.Code, StateCode=item.StateCode, Status=item.Status});
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load state Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        public static void LoadBranch()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Branch;
                    foreach (var item in result)
                    {
                        Branches.Add(item.Reference, item.GUID);
                        BranchList.Add(new Branch() { Name = item.Name, GUID = item.GUID, Reference = item.Reference, TeamGUID = item.TeamGUID, StateCode = item.StateCode, Status = item.Status });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load state Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

       
      
      

       
        //public static void LoadCRMStatus()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.Status;

        //            foreach (var item in result)
        //            {
        //                CRMStatusCode.Add(item.Reference, item.GUID);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load state Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

        public static void LoadGender()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Gender;

                    foreach (var item in result)
                    {
                        GenderList.Add(new ServiceBus.Core.Model.Generic.Gender()
                        {
                          Name=item.Name, GUID=item.GUID, PencomCode=item.PencomCode, Reference=item.Reference, ID=item.ID, StatusName=item.StatusName
                        });
                        Gender.Add(item.Reference.Trim(), item.GUID.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load state Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        public static void LoadTitle()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Title;

                    foreach (var item in result)
                    {
                        Title.Add(item.Reference, item.GUID);
                        TitleList.Add(new Title() { Name=item.Name, Reference=item.Reference, GUID=item.GUID, Status=item.Status, NavID = item.NavID });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load state Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

        public static void LoadCountry()
        {
            try
            {
                
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Country;

                    foreach (var item in result)
                    {
                        Country.Add(item.Reference, item.GUID);
                        CountryList.Add(new ServiceBus.Core.Model.Generic.Country()
                        {
                            Name = item.Name,
                            GUID = item.GUID,
                            Code = item.Code,
                            Reference = item.Reference,
                            ID = item.ID
                        });
                    }


                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load state Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

       
        //public static void LoadPaymentFrequency()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.PaymentFrequency;
        //            foreach (var item in result)
        //            {
        //                PaymentFrequencyList.Add(new PaymentFrequency() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Name });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

        //public static void LoadModeOfExit()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.ModeOfExit;
        //            foreach (var item in result)
        //            {
        //                ModeOfExitList.Add(new ModeOfExit() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Name });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

        //public static void LoadDOBSource()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.DOBSource;
        //            foreach (var item in result)
        //            {
        //                DOBSourceList.Add(new DOBSource() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Name });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

       
       

        //public static void LoadRemittancePattern()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.RemittancePattern;
        //            foreach (var item in result)
        //            {
        //                RemittancePatternList.Add(new RemittancePattern() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Name });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

       

        //public static void LoadCompetitor()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.Competitor;
        //            foreach (var item in result)
        //            {
        //                CompetitorList.Add(new Competitor() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

       


       

        //public static void LoadIndustry()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.Industry;
        //            foreach (var item in result)
        //            {
        //                IndustryList.Add(new Industry() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status, SectorCode=item.SectorCode});
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

       


        public static void LoadNationality()
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    var result = context.Nationality;
                    foreach (var item in result)
                    {
                        NationalityList.Add(new Nationality() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status, Code=item.Code });
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
                throw ex;
            }
        }

      

      

        //public static void LoadCommunicationMethod()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.CommunicationMethod;
        //            foreach (var item in result)
        //            {
        //                CommunicationMethodList.Add(new CommunicationMethod() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status, NavID = item.NavID });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

        //public static void LoadBlocked()
        //{
        //    try
        //    {
        //        using (AiroPayContext context = new AiroPayContext())
        //        {
        //            var result = context.Blocked;
        //            foreach (var item in result)
        //            {
        //                BlockedList.Add(new Blocked() { GUID = item.GUID, Name = item.Name, Reference = item.Reference, Status = item.Status, NavID = item.NavID });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceInformation($"An error occurred, unable to load ResponseDescription Context {ex}; {ex.Message}; {ex?.InnerException?.StackTrace}");
        //        throw ex;
        //    }
        //}

       


       

      

        
    }
}
