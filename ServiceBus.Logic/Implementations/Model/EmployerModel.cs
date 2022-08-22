using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Implementations.Model
{
    public class EmployerModel
    {
        public string EmployerRCNumber { get; set; }
        public string EmployerType { get; set; }
        public string EmployerName { get; set; }
        public string RegisteredAddress { get; set; }
        public string EmployerSector { get; set; }
        public string EmployerNatureOfBusiness { get; set; }
        public string EmployerEmail { get; set; }
        public string TinNumber { get; set; }
    }

    public class EmployerKYCModel
    {
        public string EmployerRCNumber { get; set; }
        public string EmployerName { get; set; }
        public string LetterOfRequestUrl { get; set; }
        public string TaxIdentificationUrl { get; set; }
        public string CertificateOfIncoprationUrl { get; set; }
    }

    public class EmployerKYCMapping : ClassMap<EmployerKYCModel>
    {

        public EmployerKYCMapping()
        {
            Map(m => m.EmployerRCNumber).Name("Registration Code");
            Map(m => m.EmployerName).Name("NAME");
            Map(m => m.LetterOfRequestUrl).Name("Letter Of RequestUrl");
            Map(m => m.TaxIdentificationUrl).Name("Tax Identification Url");
            Map(m => m.CertificateOfIncoprationUrl).Name("Certificate Of IncoprationUrl");
        }

        public string EmployerRCNumber { get; set; }
        public string EmployerName { get; set; }
        public string LetterOfRequestUrl { get; set; }
        public string TaxIdentificationUrl { get; set; }
        public string CertificateOfIncoprationUrl { get; set; }

    }
    public class EmployerCSVMapping : ClassMap<EmployerModel>
    {

        public EmployerCSVMapping()
        {
            Map(m => m.EmployerRCNumber).Name("Registration Code");
            Map(m => m.EmployerType).Name("EMPLOYER TYPE");
            Map(m => m.EmployerName).Name("NAME");
            Map(m => m.RegisteredAddress).Name("ADDRESS");
            Map(m => m.EmployerSector).Name("SECTOR Code");
            Map(m => m.EmployerNatureOfBusiness).Name("Nature of Business");
            Map(m => m.EmployerEmail).Name("EMAIL");
            Map(m => m.TinNumber).Name("Tax Id / COA No");
        }

        public string EmployerRCNumber { get; set; }
        public string EmployerType { get; set; }
        public string EmployerName { get; set; }
        public string RegisteredAddress { get; set; }
        public string EmployerSector { get; set; }
        public string EmployerNatureOfBusiness { get; set; }
        public string EmployerEmail { get; set; }
        public string TinNumber { get; set; }

    }

    public class EmployerRecordUpdatePayload
    {
        public string RCNumber { get; set; }
        public string EmployerCode { get; set; }
        public string OldEmployerName { get; set; }
        public string OldResidentialAddress { get; set; }
        public string OldCountry { get; set; }
        public string OldState { get; set; }
        public string OldLGA { get; set; }
        public string OldEmailAddress { get; set; }
        public string NewEmployerName { get; set; }
        public string NewResidentialAddress { get; set; }
        public string NewCountry { get; set; }
        public string NewState { get; set; }
        public string NewLGA { get; set; }
        public string NewEmailAddress { get; set; }
    }

    public class EmployerUpdateModel 
      {
        public string RCNumber { get; set; }
        public string EmployerCode { get; set; }
        public string OldEmployerName { get; set; }
        public string OldAddress { get; set; }       
        public string OldEmailAddress { get; set; }
        public string NewEmployerName { get; set; }
        public string NewAddress { get; set; }
        public string NewEmailAddress { get; set; }
    }

    public class EmployerUpdateMapping: ClassMap<EmployerUpdateModel>
    {
        public EmployerUpdateMapping()
        {
            Map(m => m.RCNumber).Name("RC/BN/COA/STATE ID");
            Map(m => m.EmployerCode).Name("EMPLOYER CODE");
            Map(m => m.OldEmployerName).Name("OLD EMPLOYER NAME");
            Map(m => m.OldAddress).Name("OLD ADDRESS");

            Map(m => m.OldEmailAddress).Name("OLD EMAIL ADDRESS OF EMPLOYER'S CONTACT");
            Map(m => m.NewEmployerName).Name("NEW EMPLOYER NAME");
            Map(m => m.NewAddress).Name("NEW ADDRESS");
            Map(m => m.NewEmailAddress).Name("NEW EMAIL ADDRESS OF EMPLOYER'S CONTACT");
        }
        public string RCNumber { get; set; }
        public string EmployerCode { get; set; }
        public string OldEmployerName { get; set; }
        public string OldAddress { get; set; }
        public string OldEmailAddress { get; set; }
        public string NewEmployerName { get; set; }
        public string NewAddress { get; set; }
        public string NewEmailAddress { get; set; }
    }
}
