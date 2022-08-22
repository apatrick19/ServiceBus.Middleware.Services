using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.SMS.Gateway.Migration
{
    internal sealed class Configuration : DbMigrationsConfiguration<MessagingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ServiceBus.SMS.Gateway.MessagingContext";
        }

        protected override void Seed(MessagingContext context)
        {


        }
    }
}
