using ServiceBus.Core.Model;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.CRM;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.Data.ORM.EntityFramework.Migration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Data.ORM.EntityFramework
{
    public class AiroPayContext : DbContext
    {
        public AiroPayContext() : base("AiroPayContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AiroPayContext, Configuration>("AiroPayContext"));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        }


      
        public DbSet<UserRoleModules> UserRoleModules { get; set; }
        public DbSet<LienLog> LienLog { get; set; }
        public DbSet<StatementRequestLog> StatementRequestLog { get; set; }
        public DbSet<CardHotListLog> CardHotListLog { get; set; }
        public DbSet<CardRequest> CardRequest { get; set; }
        public DbSet<ServiceType> ServiceType { get; set; }
        public DbSet<BillsPaymentTransaction> BillsPaymentTransaction { get; set; }
        public DbSet<Transfer> Transfer { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Messaging> Messaging { get; set; }
        public DbSet<Beneficiary> Beneficiary { get; set; }
        public DbSet<AccountTier> AccountTier { get; set; }
        public DbSet<IPAddresses> IPAddresses { get; set; }
        public DbSet<Reversal> Reversal { get; set; }      
        public DbSet<Transactions> Transactions { get; set; }     
        public DbSet<Billers> Billers { get; set; } 
        public DbSet<BillingCategory> BillingCategory { get; set; }
        public DbSet<FAQS> FAQS { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<Account> Account { get; set; }
         public DbSet<Branch> Branch { get; set; }       
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Relationship> Relationship { get; set; }
        public DbSet<Religion> Religion { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Lga> Lga { get; set; }
        public DbSet<Title> Title { get; set; }
        public DbSet<ChannelSource> ChannelSource { get; set; }    
        public DbSet<MaritalStatus> MaritalStatus { get; set; }
        public DbSet<Nationality> Nationality { get; set; }      
        public DbSet<Response> Response { get; set; }          
        public DbSet<User> User { get; set; }      
        public DbSet<ChannelActivities> ChannelActivities { get; set; }       
        public DbSet<StatementOption> StatementOption { get; set; }
        public DbSet<ProofOfIdentity> ProofOfIdentity { get; set; }
        public DbSet<ProofOfAddress> ProofOfAddress { get; set; }      
        public DbSet<AccountStatus> AccountStatus { get; set; }    
        public DbSet<AuditTrail> AuditTrail { get; set; }
       
    }
}
