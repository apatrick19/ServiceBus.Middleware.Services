using Servicebus.SMS.Gateway.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.SMS.Gateway.Migration
{
    public class MessagingContext : DbContext
    {
        public MessagingContext() : base("MessagingContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MessagingContext, Configuration>("MessagingContext"));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        }


      
        public DbSet<Messaging> Messaging { get; set; }
     
    }
}
