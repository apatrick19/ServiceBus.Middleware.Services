using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Data.ORM.EntityFramework.Migration
{
    internal sealed class Configuration : DbMigrationsConfiguration<AiroPayContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ServiceBus.Data.ORM.EntityFramework.ARMPContext";

            //ContextKey = "ARMMiddleWare.Data.Implementation.ARMContext";
        }

        protected override void Seed(AiroPayContext context)
        {


        }
    }
}
