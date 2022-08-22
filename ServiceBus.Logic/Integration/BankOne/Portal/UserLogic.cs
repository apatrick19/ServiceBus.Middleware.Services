using ServiceBus.Core;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Logic.Portal
{
    public class UserLogic
    {
        static string classname = "UserLogic";
        static string countrycode = BaseService.GetAppSetting("countrycode");
        public static BaseResponse CreateRegion(RegionCreationRequest request)
        {
            string methodname = "CreateRegion";
            try
            {
                var region = new Region()
                {
                    Name = request.Name,
                    Description = request.Description
                };
                using (AiroPayContext context = new AiroPayContext())
                {
                    context.Region.Add(region);
                    context.SaveChanges();
                    return new BaseResponse() { ResponseCode = "00", ResponseMessage = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BaseResponse() { ResponseCode = "06", ResponseMessage = "An error occurred while creating region, please contact admin" };
            }
        }

        public static BaseResponse CreateUserType(UserTypeCreationRequest request)
        {
            string methodname = "CreateUserType";
            try
            {
                var region = new UserType()
                {
                    Name = request.Name,
                    Description = request.Description,
                    IsAdmin = string.IsNullOrEmpty(request.IsAdmin) ? false : true
                };
                using (AiroPayContext context = new AiroPayContext())
                {
                    context.UserType.Add(region);
                    context.SaveChanges();
                    return new BaseResponse() { ResponseCode = "00", ResponseMessage = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BaseResponse() { ResponseCode = "06", ResponseMessage = "An error occurred while creating region, please contact admin" };
            }
        }

        public static BaseResponse CreateRoleModupeMapping(UserRoleModules request)
        {
            string methodname = "CreateRoleModupeMapping";
            try
            {               
                using (AiroPayContext context = new AiroPayContext())
                {
                    context.UserRoleModules.Add(request);
                    context.SaveChanges();
                    return new BaseResponse() { ResponseCode = "00", ResponseMessage = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BaseResponse() { ResponseCode = "06", ResponseMessage = "An error occurred while creating region, please contact admin" };
            }
        }

        public static BaseResponse CreateUser(UserCreationRequest request)
        {
            string methodname = "CreateUser";
            try
            {
                var user = new User()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email.ToLower().Trim(),
                    Gender = int.Parse(request.Gender),
                    MobileNo = request.MobileNo,
                    PassportUrl = request.PassportUrl,
                    Region = int.Parse(request.Region),
                    Role = int.Parse(request.Role),
                    IsFirstLogon = true,
                    IsPasswordChanged = false,
                    Password = Guid.NewGuid().ToString().Substring(0, 6),
                    IsRegistrationEmailSent = false
                    //  Manager = int.Parse(request.Manager)
                };
                using (AiroPayContext context = new AiroPayContext())
                {
                    var checkrecords = context.User.Where(x => x.Email == request.Email.ToLower().Trim()).FirstOrDefault();
                    if (checkrecords!=null)
                    {
                        return new BaseResponse() { ResponseCode = "04", ResponseMessage = "A user exists with this email id" };
                    }
                    context.User.Add(user);
                    context.SaveChanges();
                    return new BaseResponse() { ResponseCode = "00", ResponseMessage = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BaseResponse() { ResponseCode = "06", ResponseMessage = "An error occurred while creating region, please contact admin" };
            }
        }

        public static BaseResponse UpdateUser(UserCreationRequest request)
        {
            string methodname = "CreateUser";
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {
                    var oldRecord = context.User.Where(x => x.ID == request.ID).FirstOrDefault();

                    oldRecord.FirstName = request.FirstName;
                    oldRecord.LastName = request.LastName;
                    oldRecord.Email = request.Email;
                    oldRecord.Gender = int.Parse(request.Gender=="Male"?"1":"0");
                    oldRecord.MobileNo = request.MobileNo;
                    oldRecord.PassportUrl = string.IsNullOrEmpty(request.PassportUrl)?oldRecord.PassportUrl:request.PassportUrl;
                    oldRecord.Region = int.Parse(request.Region);
                    oldRecord.Role = int.Parse(request.Role);

                    context.SaveChanges();

                    return new BaseResponse() { ResponseCode = "00", ResponseMessage = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BaseResponse() { ResponseCode = "06", ResponseMessage = "An error occurred while updating user, please contact admin" };
            }
        }

        public static BaseResponse PasswordChange (PasswordChangeRequest request)
        {
            string methodname = "PasswordChange";
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {
                    var oldRecord = context.User.Where(x => x.ID == request.UserId).FirstOrDefault();

                    if (oldRecord==null)
                    {
                        return new BaseResponse() { ResponseCode = "04", ResponseMessage = "System Error, no record associated" };
                    }

                    oldRecord.OldPassword = oldRecord.Password;
                    oldRecord.Password = request.NewPassword;
                    oldRecord.IsFirstLogon = false;
                    oldRecord.IsPasswordChanged = true;
                    context.SaveChanges();

                    return new BaseResponse() { ResponseCode = "00", ResponseMessage = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BaseResponse() { ResponseCode = "06", ResponseMessage = "An error occurred while updating user, please contact admin" };
            }
        }

        public static List<DropdownResponse> FetchRegion()
        {
            string methodname = "FetchRegion";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.Region.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchUserType()
        {
            string methodname = "FetchUserType";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.UserType.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID, isAdmin = item.IsAdmin, Description = item.Description });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchUserRoles()
        {
            string methodname = "FetchUserRoles";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.UserRoleModules.OrderByDescending(x=>x.ID);
                    if (result.Count() > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.Name, Id = item.ID, isAdmin = item.IsAdmin, Description = item.Description });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<DropdownResponse> FetchUser4DropDown()
        {
            string methodname = "FetchUser4DropDown";
            List<DropdownResponse> responses = new List<DropdownResponse>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.User.ToList();
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            responses.Add(new DropdownResponse() { Name = item.FirstName + " " + item.LastName, Id = item.ID });
                        }
                    }
                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static List<User> FetchUser()
        {
            string methodname = "FetchUser";
            List<User> responses = new List<User>();
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    responses = context.User.ToList();

                    return responses;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return responses;
            }
        }

        public static User FetchUserByEmail(string Email)
        {
            string methodname = "FetchUserByEmail";
          
            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.User.Where(x=>x.Email.Trim().ToLower()== Email.Trim().ToLower()).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return null;
            }
        }

        public static UserRoleModules FetchUserRoleByUserId(int ID)
        {
            string methodname = "FetchUserByEmail";

            try
            {

                using (AiroPayContext context = new AiroPayContext())
                {

                    var result = context.UserRoleModules.Where(x => x.ID == ID).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return null;
            }
        }

        public static BaseResponse CreateAccountTier(AccountTier request)
        {
            string methodname = "CreateAccountTier";
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    context.AccountTier.Add(request);
                    context.SaveChanges();
                    return new BaseResponse() { ResponseCode = "00", ResponseMessage = "Request completed successfully" };
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(countrycode, classname, methodname, ex);
                return new BaseResponse() { ResponseCode = "06", ResponseMessage = "An error occurred while creating tier, please contact admin" };
            }
        }

            public static List<AccountTier> FetchAllAccountTier()
            {
                string methodname = "CreateAccountTier";
                try
                {
                    using (AiroPayContext context = new AiroPayContext())
                    {
                        var result=context.AccountTier.ToList();
                    return result;
                    }
                }
                catch (Exception ex)
                {
                    LogMachine.LogInformation(countrycode, classname, methodname, ex);
                    return null;
                }
            }
    }
}