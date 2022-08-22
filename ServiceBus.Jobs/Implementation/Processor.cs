//using Hangfire;
using Hangfire;
using ServicBus.Logic.Implementations;
using ServicBus.Logic.Implementations.IO.Image;
using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Core.Settings;
using ServiceBus.Nodes.Destination;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Jobs.Implementation
{
    public static class Processor
    {
        private static BackgroundJobServer _server;

        public static bool InitiateSequence()
        {
            Trace.TraceInformation("Loading all dependency into memory");
            InMemory.LoadFundingCategory();
            InMemory.LoadIndustry();
            InMemory.LoadBenefitRejectionType();
            InMemory.LoadChannelActivitiesList();
            InMemory.LoadChannelActivitiesMappingList();
            InMemory.LoadSalaryGL();
            InMemory.LoadClientType();
            InMemory.LoadSalaryStep();
            InMemory.LoadHarmonizedSalary();
            InMemory.LoadConsolidatedSalary();
            InMemory.LoadAccountStatus();
            InMemory.LoadRealtionship();
            InMemory.LoadEmailTemplate();
            InMemory.LoadCustomerKYCCategory();
            InMemory.LoadChannelSources();
            InMemory.LoadCodeDescription();
            InMemory.LoadPecomeCodeDescription();
            InMemory.LoadSector();
            InMemory.LoadLeadTypeList();
            InMemory.LoadState();
            InMemory.LoadCountry();
            InMemory.LoadLGA();
            InMemory.LoadBranch();
            InMemory.LoadCaseType();
            InMemory.LoadCaseCategory();
            InMemory.LoadCaseSubCategory();
            InMemory.LoadBenefitTypes();
            InMemory.LoadCRMStatus();
            InMemory.LoadGender();
            InMemory.LoadMaritalStatus();
            InMemory.LoadTitle();
            InMemory.LoadBank();
            InMemory.LoadStatementOption();
            //InMemory.LoadDOBSource();
            InMemory.LoadPaymentFrequency();
            InMemory.LoadLumpsumOption();
            InMemory.LoadModeOfExit();
            InMemory.LoadSalaryType();
            InMemory.LoadRemittancePattern();
            InMemory.LoadSourceOfFunding();
            InMemory.LoadCaseStatus();
            InMemory.LoadFundType();
            InMemory.LoadCompetitor();
            InMemory.LoadBenefitStatus();
            InMemory.LoadNationality();
            InMemory.LoadTeams();
            InMemory.LoadEsteemCategory();
            InMemory.LoadRecordUpdateType();
            InMemory.LoadCommunicationMethod();
            InMemory.LoadBlocked();
            InMemory.LoadFlagDescription();


            GlobalConfiguration.Configuration.UseSqlServerStorage("ARMPContext");

            Trace.TraceInformation("All types successfully loaded, MQ service running jobs ");
            // Hangfire.GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));


            return true;
        }


        public static bool InitiateJobs(string[] args)
        {
            _server = new BackgroundJobServer();
            Trace.TraceInformation("Initiating servers and running all processes");

            JobProcessor processor = new JobProcessor();

            Trace.TraceInformation("starting Benefit Document Conversion To ServerUrl ");
            RecurringJob.AddOrUpdate(() => JobProcessor.BenefitDocumentConversionToServerUrl(), Cron.MinuteInterval(AppConfig.MinuteSequence));

            Trace.TraceInformation("starting Benefit TPIN Account Creation");
            RecurringJob.AddOrUpdate(() => JobProcessor.TPINAccountCreation(), Cron.MinuteInterval(AppConfig.TimeSequence));


            Trace.TraceInformation("starting Benefit Core System Creation Job ");
            RecurringJob.AddOrUpdate(() => JobProcessor.BenefitCoreSystemCreationJob(), Cron.MinuteInterval(AppConfig.TimeSequence));


            Trace.TraceInformation("starting Benefit Rejection Request Job ");
            RecurringJob.AddOrUpdate(() => JobProcessor.BenefitRejectionRequestJob(), Cron.MinuteInterval(AppConfig.MinuteSequence));


            Trace.TraceInformation("starting Employer Core System Creation Job ");
            RecurringJob.AddOrUpdate(() => JobProcessor.EmployerCreationCoreSystemJob(), Cron.MinuteInterval(AppConfig.TimeSequence));


            Trace.TraceInformation("starting Flag and Block Accounts Processor Job ");
            RecurringJob.AddOrUpdate(() => JobProcessor.FlagAndBlockAccountsProcessor(), Cron.MinuteInterval(AppConfig.MinuteSequence));


            Trace.TraceInformation("starting data recapture core system update Job ");
            RecurringJob.AddOrUpdate(() => JobProcessor.DataRecaptureCoreSystemUpdateJob(), Cron.MinuteInterval(AppConfig.TimeSequence));


            Trace.TraceInformation("starting service Generate Employer CSV ");
            RecurringJob.AddOrUpdate(() => JobProcessor.GenerateEmployerCSV_Updated(), Cron.MinuteInterval(AppConfig.MinuteSequence));


           // Trace.TraceInformation("starting service Generate Employer Record Update CSV ");
            //RecurringJob.AddOrUpdate(() => JobProcessor.GenerateEmployerRecordUpdateCSV(), Cron.MinuteInterval(AppConfig.MinuteSequence));


            Trace.TraceInformation("starting service Generate And Send SMS ");
            RecurringJob.AddOrUpdate(() => JobProcessor.PinGenerationSMS(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service Generate Membership Certificate ");
            RecurringJob.AddOrUpdate(() => JobProcessor.PinGenerationEmail(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("Get ECRS Pin Generation Status ");
            RecurringJob.AddOrUpdate(() => JobProcessor.GetECRSPinGenerationStatus(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("Get ECRS Pin Generation Status ");
            RecurringJob.AddOrUpdate(() => JobProcessor.GetPinGenerationStatus_BackUp(), Cron.MinuteInterval(20));

            Trace.TraceInformation("Get ECRS Temporary Pin account Status ");
            RecurringJob.AddOrUpdate(() => JobProcessor.GetECRSTemoraryPinAccountStatus(), Cron.MinuteInterval(AppConfig.MinuteSequence));


            Trace.TraceInformation("starting Get DataRecaptureECRSStatus");
            RecurringJob.AddOrUpdate(() => JobProcessor.BatchDataRecaptureECRSStatus(), Cron.MinuteInterval(AppConfig.MinuteSequence));

            Trace.TraceInformation("starting Generate Attestation and Move Kyc");
            RecurringJob.AddOrUpdate(() => JobProcessor.GenerateAttestationandMoveKyc(), Cron.MinuteInterval(AppConfig.MinuteSequence));


            Trace.TraceInformation("starting ECRS pin generation schema");
            RecurringJob.AddOrUpdate(() => JobProcessor.GenerateECRSPIN(), Cron.MinuteInterval(AppConfig.MinuteSequence));

            Trace.TraceInformation("starting Create Employee On Core");
            RecurringJob.AddOrUpdate(() => JobProcessor.CreateEmployeeOnCore(), Cron.MinuteInterval(AppConfig.TimeSequence));


            Trace.TraceInformation("starting Generate Data ECRS Recapture");
            RecurringJob.AddOrUpdate(() => JobProcessor.GenerateDataECRSRecapture(), Cron.MinuteInterval(AppConfig.MinuteSequence));


            Trace.TraceInformation("starting Generate Attestation and Move Data recapture Kyc");
            RecurringJob.AddOrUpdate(() => JobProcessor.GenerateAttestationandMoveDatarecaptureKyc(), Cron.MinuteInterval(AppConfig.MinuteSequence));

            Trace.TraceInformation("starting Data recapture SMS");
            RecurringJob.AddOrUpdate(() => JobProcessor.DataRecaptureSMS(), Cron.MinuteInterval(AppConfig.MinuteSequence));

            Trace.TraceInformation("starting Record update SMS");
            RecurringJob.AddOrUpdate(() => JobProcessor.RecordUpdateSMS(), Cron.MinuteInterval(AppConfig.MinuteSequence));

            Trace.TraceInformation("starting Record update Email");
            RecurringJob.AddOrUpdate(() => JobProcessor.RecordUpdateEmailNotification(), Cron.MinuteInterval(AppConfig.MinuteSequence));

            Trace.TraceInformation("starting service data recapture email and slip generation ");
            RecurringJob.AddOrUpdate(() => JobProcessor.DataRecaptureEmailNotification(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service record update status ");
            RecurringJob.AddOrUpdate(() => JobProcessor.BatchRecordUpdateStatus(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service DBA approval check ");
            RecurringJob.AddOrUpdate(() => JobProcessor.DBA_ApprovalCheck(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service sharepoint-DBA record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_DBA(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service sharepoint-CPS record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_CPS(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service sharepoint-MPS record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_MPS(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service sharepoint-TPIN record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_TPIN(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service sharepoint-Crossborder record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_CrossBorder(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service sharepoint-DataRecapture record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_DataRecapture(), Cron.MinuteInterval(AppConfig.TimeSequence));

            Trace.TraceInformation("starting service sharepoint-RecordUpdate record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_RecordUpdate(), Cron.MinuteInterval(AppConfig.TimeSequence));


            Trace.TraceInformation("starting service sharepoint-case record ");
            RecurringJob.AddOrUpdate(() => JobProcessor.SharePointFileUploadWithMetaData_Case(), Cron.MinuteInterval(AppConfig.TimeSequence));



            Trace.TraceInformation("Procesors running....");
            return true;
        }

        public static bool PowerDown()
        {
            Trace.TraceInformation("disposing global configs and stopping all processes");
            _server.Dispose();
            return true;
        }

        
    }
 }
