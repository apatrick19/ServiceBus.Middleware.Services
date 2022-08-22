using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicBus.Logic.Implementations.SharePoint
{

    public class Metadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
    }

    public class Deferred
    {
        public string uri { get; set; }
    }

    public class Author
    {
        public Deferred __deferred { get; set; }
    }

    public class Deferred2
    {
        public string uri { get; set; }
    }

    public class CheckedOutByUser
    {
        public Deferred2 __deferred { get; set; }
    }

    public class Deferred3
    {
        public string uri { get; set; }
    }

    public class EffectiveInformationRightsManagementSettings
    {
        public Deferred3 __deferred { get; set; }
    }

    public class Deferred4
    {
        public string uri { get; set; }
    }

    public class InformationRightsManagementSettings
    {
        public Deferred4 __deferred { get; set; }
    }

    public class Deferred5
    {
        public string uri { get; set; }
    }

    public class ListItemAllFields
    {
        public Deferred5 __deferred { get; set; }
    }

    public class Deferred6
    {
        public string uri { get; set; }
    }

    public class LockedByUser
    {
        public Deferred6 __deferred { get; set; }
    }

    public class Deferred7
    {
        public string uri { get; set; }
    }

    public class ModifiedBy
    {
        public Deferred7 __deferred { get; set; }
    }

    public class Deferred8
    {
        public string uri { get; set; }
    }

    public class Properties
    {
        public Deferred8 __deferred { get; set; }
    }

    public class Deferred9
    {
        public string uri { get; set; }
    }

    public class Versions
    {
        public Deferred9 __deferred { get; set; }
    }

    public class Result
    {
        public Metadata __metadata { get; set; }
        public Author Author { get; set; }
        public CheckedOutByUser CheckedOutByUser { get; set; }
        public EffectiveInformationRightsManagementSettings EffectiveInformationRightsManagementSettings { get; set; }
        public InformationRightsManagementSettings InformationRightsManagementSettings { get; set; }
        public ListItemAllFields ListItemAllFields { get; set; }
        public LockedByUser LockedByUser { get; set; }
        public ModifiedBy ModifiedBy { get; set; }
        public Properties Properties { get; set; }
        public Versions Versions { get; set; }
        public string CheckInComment { get; set; }
        public int CheckOutType { get; set; }
        public string ContentTag { get; set; }
        public int CustomizedPageStatus { get; set; }
        public string ETag { get; set; }
        public bool Exists { get; set; }
        public bool IrmEnabled { get; set; }
        public string Length { get; set; }
        public int Level { get; set; }
        public string LinkingUrl { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public string Name { get; set; }
        public string ServerRelativeUrl { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeLastModified { get; set; }
        public object Title { get; set; }
        public int UIVersion { get; set; }
        public string UIVersionLabel { get; set; }
        public string UniqueId { get; set; }
    }

    public class D
    {
        public List<Result> results { get; set; }
    }

    public class RootObject
    {
        public D d { get; set; }
    }

}
