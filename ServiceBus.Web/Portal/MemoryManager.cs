using ServiceBus.Core.Model.Generic;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Integration;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceBus.Web.Portal
{
    public class MemoryManager
    {
     public static  List<SelectListItem> AccountTier = new List<SelectListItem>();
     public static  List<SelectListItem> States = new List<SelectListItem>();
     public static  List<SelectListItem> Lga = new List<SelectListItem>();
     public static  List<DropdownResponse> Lgalist = new List<DropdownResponse>();
     public static  List<SelectListItem> AccountOfficer = new List<SelectListItem>();
     public static  List<SelectListItem> Nationality = new List<SelectListItem>();
     public static  List<SelectListItem> Products = new List<SelectListItem>();
     public static  List<SelectListItem> Region = new List<SelectListItem>();
     public static  List<SelectListItem> Banks = new List<SelectListItem>();
     public static  List<SelectListItem> BillsCategories = new List<SelectListItem>();
     public static  List<SelectListItem> Billers = new List<SelectListItem>();
     public static  List<SelectListItem> PaymentItems = new List<SelectListItem>();
     public static  List<DropdownResponse> PaymentItemsDropDown = new List<DropdownResponse>();

        public static List<DropdownResponse> BillersDropDown = new List<DropdownResponse>();

        public static List<SelectListItem> FetchState()
        {
            string methodname = "FetchState";
           
            try
            {
                var result = GenericLogic.FetchState();
                foreach (var item in result)
                {
                    States.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                }

                return States;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }
       

        public static List<SelectListItem> FetchLga()
        {
            string methodname = "FetchLga";

            try
            {
                Lgalist = GenericLogic.FetchLGA();
                foreach (var item in Lgalist)
                {
                    Lga.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                }

                return Lga;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }

        public static List<SelectListItem> FetchNationality()
        {
            string methodname = "FetchNationality";

            try
            {
                var result = GenericLogic.FetchNationality();
                foreach (var item in result)
                {
                    Nationality.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                }

                return Lga;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }

        public static List<SelectListItem> FetchProducts()
        {
            string methodname = "FetchProducts";

            try
            {
                var result = GenericLogic.FetchProducts();
                foreach (var item in result)
                {
                    Products.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                }

                return Lga;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }

        public static List<SelectListItem> FetchOfficer()
        {
            string methodname = "FetchOfficer";

            try
            {
                var result = GenericLogic.FetchAllAccountOfficer();
                foreach (var item in result)
                {
                    AccountOfficer.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                }

                return Lga;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }

        public static List<SelectListItem> FetchBanks()
        {
            string methodname = "FetchBanks";

            try
            {
                var result = GenericLogic.FetchBanks();
                foreach (var item in result)
                {
                    Banks.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                }

                return Lga;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }

        public static List<SelectListItem> FetchBillerCategories()
        {
            string methodname = "FetchBillerCategories";

            try
            {
                var result = GenericLogic.FetchBillsCategory();
                BillsCategories.Add(new SelectListItem() { Text = "--Select Category--", Value = "-1" });
                foreach (var item in result)
                {
                    BillsCategories.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                }

                return States;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }

        public static List<SelectListItem> FetchBillers()
        {
            string methodname = "FetchBillers";

            try
            {
                var result = GenericLogic.FetchBillers();
                Billers.Add(new SelectListItem() { Text = "Select Billers", Value = "-1" });
                BillersDropDown.Add(new DropdownResponse() { Name = "Select Billers", Code = "-1", ParentCode = "-1" });
                foreach (var item in result)
                {
                    Billers.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                    BillersDropDown.Add(new DropdownResponse() { Name = item.Name, Code = item.Code, ParentCode=item.ParentCode });
                }

                return States;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }

        public static List<SelectListItem> FetchPaymentItems()
        {
            string methodname = "FetchPaymentItems";

            try
            {
                var result = GenericLogic.FetchPaymentItems();
                PaymentItems.Add(new SelectListItem() { Text = "Select Products", Value = "-1" });
                PaymentItemsDropDown.Add(new DropdownResponse() { Name = "Select Products", Code = "-1", Amount = -1, ParentCode = "-1" });
                foreach (var item in result)
                {
                    PaymentItems.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
                    PaymentItemsDropDown.Add(new DropdownResponse() { Name = item.Name, Code = item.Code, Amount=item.Amount, ParentCode=item.ParentCode });
                }

                return States;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }


        public static List<SelectListItem> FetchAccountTiers()
        {
            string methodname = "FetchAccountTiers";

            try
            {
                using (AiroPayContext context=new AiroPayContext())
                {
                    var result = context.AccountTier.ToList();
                    AccountTier.Add(new SelectListItem() { Text = "Select Tier", Value = "-1" });                   
                    foreach (var item in result)
                    {
                        AccountTier.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString() });                        
                    }

                    return AccountTier;
                }
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", "MemoryManager", methodname, ex);
                return null;
            }
        }
    }
}