using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Contract;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Implementation
{
    /// <summary>
    /// This method adds the inputted username and password to the database
    /// </summary>
    public class UserService : IUserService
    {
        public ResponseModel CreateUser(User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName)|| string.IsNullOrEmpty(model.Password))
                {
                    return ResponseDictionary.GetCodeDescription("01", "username / password is missing");
                }
                using (AiroPayContext context=new AiroPayContext())
                {
                    context.User.Add(model);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
