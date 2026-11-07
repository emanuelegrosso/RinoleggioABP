namespace Rinoleggio.Listings;

public enum ListingStatus { Draft, Active, Reserved, Closed, Suspended, Withdrawn, Deleted }
public enum ContributionType { None, Incentive, Compensation }
public enum FuelType { Petrol, Diesel, MildHybrid, PlugInHybrid, Electric, LPG, CNG }
public enum TransmissionType { Manual, Automatic }
public enum LeadStatus { New, Contacted, Closed }
public enum AccountType { Private, Business }
public enum CatalogEntryType { RentalCompany, Brand, Province }
public enum LeadOutcome { None, TransferCompleted, CustomerDeclined, DealerDeclined, NotEligible, ContractUnavailable, Other }
