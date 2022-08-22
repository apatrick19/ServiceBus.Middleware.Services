namespace ServiceBus.Logic.Model
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class CoreBankingAuthModel
    {
        [JsonProperty("$id")]
       
        public string Id { get; set; }

        [JsonProperty("Payload")]
        public Payload Payload { get; set; }

        [JsonProperty("ErrorDetails")]
        public string ErrorDetails { get; set; }

        [JsonProperty("ResponseCode")]
        public int ResponseCode { get; set; }
    }

    public partial class Payload
    {
        [JsonProperty("$id")]
       
        public int Id { get; set; }

        [JsonProperty("SessionId")]
        public string SessionId { get; set; }

        [JsonProperty("CustomerId")]
        public int CustomerId { get; set; }

        [JsonProperty("IsTemporaryPassword")]
        public bool IsTemporaryPassword { get; set; }

        [JsonProperty("PersonId")]
        public long PersonId { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Roles")]
        public Role[] Roles { get; set; }

        [JsonProperty("LicenceDaysRemaining")]
        public object LicenceDaysRemaining { get; set; }
    }

    public partial class Role
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("RoleID")]
        public long RoleId { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("roleDescription")]
        public string RoleDescription { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("Permissions")]
        public Permission[] Permissions { get; set; }

        [JsonProperty("USER")]
        public User[] User { get; set; }

        [JsonProperty("ChangeTracker")]
        public RoleChangeTracker ChangeTracker { get; set; }
    }

    public partial class RoleChangeTracker
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("State")]
        public long State { get; set; }

        [JsonProperty("ObjectsRemovedFromCollectionProperties")]
        public ExtendedProperties ObjectsRemovedFromCollectionProperties { get; set; }

        [JsonProperty("OriginalValues")]
        public ExtendedProperties OriginalValues { get; set; }

        [JsonProperty("ExtendedProperties")]
        public ExtendedProperties ExtendedProperties { get; set; }

        [JsonProperty("ObjectsAddedToCollectionProperties")]
        public ExtendedProperties ObjectsAddedToCollectionProperties { get; set; }
    }

    public partial class ExtendedProperties
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }
    }

    public partial class Permission
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("permissionID")]
        public long PermissionId { get; set; }

        [JsonProperty("permissionName")]
        public string PermissionName { get; set; }

        [JsonProperty("ROLE")]
        public ROLE[] ROLE { get; set; }

        [JsonProperty("ChangeTracker")]
        public RoleChangeTracker ChangeTracker { get; set; }
    }

    public partial class ROLE
    {
        [JsonProperty("$ref")]
       
        public long Ref { get; set; }
    }

    public partial class User
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("IsTemporaryPassword")]
        public bool IsTemporaryPassword { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty("MaximumDepositPost")]
        public long MaximumDepositPost { get; set; }

        [JsonProperty("MaximumLoanApproval")]
        public long MaximumLoanApproval { get; set; }

        [JsonProperty("ExtField1")]
        public long ExtField1 { get; set; }

        [JsonProperty("ExtField2")]
        public long ExtField2 { get; set; }

        [JsonProperty("ExtField3")]
        public long ExtField3 { get; set; }

        [JsonProperty("ExtField4")]
        public long ExtField4 { get; set; }

        [JsonProperty("ExtField5")]
        public long ExtField5 { get; set; }

        [JsonProperty("ExtField6")]
        public long ExtField6 { get; set; }

        [JsonProperty("UserRoles")]
        public ROLE[] UserRoles { get; set; }

        [JsonProperty("TRANSACTION")]
        public object[] Transaction { get; set; }

        [JsonProperty("LOCKS")]
        public object[] Locks { get; set; }

        [JsonProperty("CashAccount")]
        public object CashAccount { get; set; }

        [JsonProperty("DAILYCASHANALYSIS")]
        public object[] Dailycashanalysis { get; set; }

        [JsonProperty("T_SYSTEMUSER")]
        public object TSystemuser { get; set; }

        [JsonProperty("staffNumber")]
       
        public long StaffNumber { get; set; }

        [JsonProperty("employeeSince")]
        public DateTimeOffset EmployeeSince { get; set; }

        [JsonProperty("retirementDate")]
        public object RetirementDate { get; set; }

        [JsonProperty("approved")]
        public bool Approved { get; set; }

        [JsonProperty("MaritalStatus")]
        public long MaritalStatus { get; set; }

        [JsonProperty("Category")]
        public long Category { get; set; }

        [JsonProperty("Status")]
        public long Status { get; set; }

        [JsonProperty("stateOfOrigin")]
        public long StateOfOrigin { get; set; }

        [JsonProperty("nyscNumber")]
        public string NyscNumber { get; set; }

        [JsonProperty("personalizedSms")]
        public bool PersonalizedSms { get; set; }

        [JsonProperty("departmentId")]
        public long DepartmentId { get; set; }

        [JsonProperty("managerId")]
        public object ManagerId { get; set; }

        [JsonProperty("gradeScale")]
        public object GradeScale { get; set; }

        [JsonProperty("promotionDate")]
        public object PromotionDate { get; set; }

        [JsonProperty("lga")]
        public string Lga { get; set; }

        [JsonProperty("otherComments")]
        public string OtherComments { get; set; }

        [JsonProperty("HomeBranch")]
        public HomeBranch HomeBranch { get; set; }

        [JsonProperty("SalaryInfo")]
        public SalaryInfo SalaryInfo { get; set; }

        [JsonProperty("Degrees")]
        public object[] Degrees { get; set; }

        [JsonProperty("T_ACCOUNT")]
        public object[] TAccount { get; set; }

        [JsonProperty("STAFFSTATUSHISTORY")]
        public object[] Staffstatushistory { get; set; }

        [JsonProperty("LinkedAccount")]
        public object LinkedAccount { get; set; }

        [JsonProperty("StaffPayroll")]
        public object StaffPayroll { get; set; }

        [JsonProperty("AdditionalInformation")]
        public object AdditionalInformation { get; set; }

        [JsonProperty("personID")]
        public long PersonId { get; set; }

        [JsonProperty("salutation")]
        public string Salutation { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTimeOffset DateOfBirth { get; set; }

        [JsonProperty("occupation")]
        public string Occupation { get; set; }

        [JsonProperty("homeTelephone")]
        public string HomeTelephone { get; set; }

        [JsonProperty("mobileTelephone")]
        public string MobileTelephone { get; set; }

        [JsonProperty("workTelephone")]
        public string WorkTelephone { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("nameOfSpouse")]
        public string NameOfSpouse { get; set; }

        [JsonProperty("nameOfNextOfKin")]
        public string NameOfNextOfKin { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("NameOfCurrentEmployer")]
        public string NameOfCurrentEmployer { get; set; }

        [JsonProperty("addressOfCurrentEmployer")]
        public object AddressOfCurrentEmployer { get; set; }

        [JsonProperty("motherMaidenName")]
        public object MotherMaidenName { get; set; }

        [JsonProperty("relationship")]
        public object Relationship { get; set; }

        [JsonProperty("employerInfo")]
        public object EmployerInfo { get; set; }

        [JsonProperty("Customer")]
        public object[] Customer { get; set; }

        [JsonProperty("CorpInfo")]
        public object[] CorpInfo { get; set; }

        [JsonProperty("Addresses")]
        public object[] Addresses { get; set; }

        [JsonProperty("ACCOUNT")]
        public object[] Account { get; set; }

        [JsonProperty("SHAREACCOUNT")]
        public object[] Shareaccount { get; set; }

        [JsonProperty("personSpecimenImage")]
        public object PersonSpecimenImage { get; set; }

        [JsonProperty("T_RecordUpdateApprovedBy")]
        public object[] TRecordUpdateApprovedBy { get; set; }

        [JsonProperty("T_RecordUpdateApprovalRequest")]
        public object[] TRecordUpdateApprovalRequest { get; set; }

        [JsonProperty("ChangeTracker")]
        public UserChangeTracker ChangeTracker { get; set; }
    }

    public partial class UserChangeTracker
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("State")]
        public long State { get; set; }

        [JsonProperty("ObjectsRemovedFromCollectionProperties")]
        public ExtendedProperties ObjectsRemovedFromCollectionProperties { get; set; }

        [JsonProperty("OriginalValues")]
        public ExtendedProperties OriginalValues { get; set; }

        [JsonProperty("ExtendedProperties")]
        public PurpleExtendedProperties ExtendedProperties { get; set; }

        [JsonProperty("ObjectsAddedToCollectionProperties")]
        public ExtendedProperties ObjectsAddedToCollectionProperties { get; set; }
    }

    public partial class PurpleExtendedProperties
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("HomeBranch.branchID")]
        public long HomeBranchBranchId { get; set; }

        [JsonProperty("SalaryInfo.salaryInfoID")]
        public long SalaryInfoSalaryInfoId { get; set; }
    }

    public partial class HomeBranch
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("branchID")]
        public long BranchId { get; set; }

        [JsonProperty("branchName")]
        public string BranchName { get; set; }

        [JsonProperty("branchNumber")]
       
        public long BranchNumber { get; set; }

        [JsonProperty("isMainBranch")]
        public bool IsMainBranch { get; set; }

        [JsonProperty("ACCOUNT")]
        public object[] Account { get; set; }

        [JsonProperty("branchAddress")]
        public BranchAddress BranchAddress { get; set; }

        [JsonProperty("localCurrency")]
        public LocalCurrency LocalCurrency { get; set; }

        [JsonProperty("CUSTOMER")]
        public object[] Customer { get; set; }

        [JsonProperty("FIXEDASSET")]
        public object[] Fixedasset { get; set; }

        [JsonProperty("STAFF")]
        public ROLE[] Staff { get; set; }

        [JsonProperty("SHAREACCOUNT")]
        public object[] Shareaccount { get; set; }

        [JsonProperty("BANK")]
        public object[] Bank { get; set; }

        [JsonProperty("FISCALPERIODBRANCH")]
        public object[] Fiscalperiodbranch { get; set; }

        [JsonProperty("GLACCOUNT")]
        public object[] Glaccount { get; set; }

        [JsonProperty("BRANCHCHARTACCOUNT")]
        public object[] Branchchartaccount { get; set; }

        [JsonProperty("FOREXCUSTOMER")]
        public object[] Forexcustomer { get; set; }

        [JsonProperty("FOREXRATES")]
        public object[] Forexrates { get; set; }

        [JsonProperty("FOREXSERVICES")]
        public object[] Forexservices { get; set; }

        [JsonProperty("T_TRANSACTIONPATTERN")]
        public object[] TTransactionpattern { get; set; }

        [JsonProperty("ChangeTracker")]
        public HomeBranchChangeTracker ChangeTracker { get; set; }
    }

    public partial class BranchAddress
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("addressID")]
        public long AddressId { get; set; }

        [JsonProperty("poBox")]
        public string PoBox { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [JsonProperty("addressType")]
        public AddressType AddressType { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("BRANCH")]
        public ROLE[] Branch { get; set; }

        [JsonProperty("COMPANY")]
        public object[] Company { get; set; }

        [JsonProperty("CORPORATION")]
        public object[] Corporation { get; set; }

        [JsonProperty("PERSON")]
        public object[] Person { get; set; }

        [JsonProperty("CUSTOMEROTHERBANKACCOUNT")]
        public object[] Customerotherbankaccount { get; set; }

        [JsonProperty("BANK")]
        public object[] Bank { get; set; }

        [JsonProperty("FOREXCUSTOMERS")]
        public object[] Forexcustomers { get; set; }

        [JsonProperty("ChangeTracker")]
        public BranchAddressChangeTracker ChangeTracker { get; set; }
    }

    public partial class AddressType
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("addressTypeID")]
        public long AddressTypeId { get; set; }

        [JsonProperty("adressTypeName")]
        public string AdressTypeName { get; set; }

        [JsonProperty("ADDRESS")]
        public ROLE[] Address { get; set; }

        [JsonProperty("ChangeTracker")]
        public RoleChangeTracker ChangeTracker { get; set; }
    }

    public partial class BranchAddressChangeTracker
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("State")]
        public long State { get; set; }

        [JsonProperty("ObjectsRemovedFromCollectionProperties")]
        public ExtendedProperties ObjectsRemovedFromCollectionProperties { get; set; }

        [JsonProperty("OriginalValues")]
        public ExtendedProperties OriginalValues { get; set; }

        [JsonProperty("ExtendedProperties")]
        public TentacledExtendedProperties ExtendedProperties { get; set; }

        [JsonProperty("ObjectsAddedToCollectionProperties")]
        public ExtendedProperties ObjectsAddedToCollectionProperties { get; set; }
    }

    public partial class TentacledExtendedProperties
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("addressType.addressTypeID")]
        public long AddressTypeAddressTypeId { get; set; }

        [JsonProperty("state.stateID")]
        public long StateStateId { get; set; }
    }

    public partial class State
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("stateID")]
        public long StateId { get; set; }

        [JsonProperty("stateName")]
        public string StateName { get; set; }

        [JsonProperty("RegionId")]
        public long RegionId { get; set; }

        [JsonProperty("ADDRESS")]
        public ROLE[] Address { get; set; }

        [JsonProperty("ChangeTracker")]
        public RoleChangeTracker ChangeTracker { get; set; }
    }

    public partial class HomeBranchChangeTracker
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("State")]
        public long State { get; set; }

        [JsonProperty("ObjectsRemovedFromCollectionProperties")]
        public ExtendedProperties ObjectsRemovedFromCollectionProperties { get; set; }

        [JsonProperty("OriginalValues")]
        public ExtendedProperties OriginalValues { get; set; }

        [JsonProperty("ExtendedProperties")]
        public FluffyExtendedProperties ExtendedProperties { get; set; }

        [JsonProperty("ObjectsAddedToCollectionProperties")]
        public ExtendedProperties ObjectsAddedToCollectionProperties { get; set; }
    }

    public partial class FluffyExtendedProperties
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("branchAddress.addressID")]
        public long BranchAddressAddressId { get; set; }

        [JsonProperty("localCurrency.currencyID")]
        public long LocalCurrencyCurrencyId { get; set; }
    }

    public partial class LocalCurrency
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("currencyID")]
        public long CurrencyId { get; set; }

        [JsonProperty("currencyDescription")]
        public string CurrencyDescription { get; set; }

        [JsonProperty("currencySymbol")]
        public string CurrencySymbol { get; set; }

        [JsonProperty("BRANCH")]
        public ROLE[] Branch { get; set; }

        [JsonProperty("FOREXACCOUNT")]
        public object[] Forexaccount { get; set; }

        [JsonProperty("FOREXRATES")]
        public object[] Forexrates { get; set; }

        [JsonProperty("ChangeTracker")]
        public RoleChangeTracker ChangeTracker { get; set; }
    }

    public partial class SalaryInfo
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("salaryInfoID")]
        public long SalaryInfoId { get; set; }

        [JsonProperty("basicMonthlySalary")]
        public long BasicMonthlySalary { get; set; }

        [JsonProperty("monthlyBenefits")]
        public long MonthlyBenefits { get; set; }

        [JsonProperty("STAFF")]
        public ROLE[] Staff { get; set; }

        [JsonProperty("ChangeTracker")]
        public RoleChangeTracker ChangeTracker { get; set; }
    }
}
