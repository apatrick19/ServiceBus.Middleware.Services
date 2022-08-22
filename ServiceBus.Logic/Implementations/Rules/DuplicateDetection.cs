using ServicBus.Logic.Contracts;
using ServiceBus.Core.Model.CRM;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Implementations.Rules
{
    public class DuplicateDetection : IDuplicateDetection
    {
        dynamic expando = new ExpandoObject();

        public dynamic GetDuplicateRule(string key)
        {
            string[] arrayofKeys = key.Split(';');

            dynamic expando = new ExpandoObject();
           
            foreach (var item in arrayofKeys)
            {
                AddProperty(expando, item, "Unknown");
            }

            return expando;
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
        
        public bool CheckForDuplicate(dynamic account)
        {
            bool IsDuplicate = false;  
            var dupkeys = GetDuplicateRule("FirstName;LastName;Email");
            var expandoDict = account as IDictionary<string, object>;
            foreach (var item in dupkeys)
            {
                if (expandoDict.ContainsKey(item))
                {
                    if (expandoDict[item] == null)
                    {

                    }
                }
            }

            return true;
        }


    }
}
