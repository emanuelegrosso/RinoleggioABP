namespace Rinoleggio.Permissions;

public static class RinoleggioPermissions
{
    public const string GroupName = "Rinoleggio";

    public static class Customer
    {
        public const string Favorites = GroupName + ".Customer.Favorites";
        public const string ManageFavorites = Favorites + ".Manage";
        public const string Leads = GroupName + ".Customer.Leads";
        public const string CreateLead = Leads + ".Create";
        public const string ViewOwnLeads = Leads + ".ViewOwn";
    }

    public static class Dealer
    {
        public const string Listings = GroupName + ".Dealer.Listings";
        public const string CreateListing = Listings + ".Create";
        public const string ViewOwnListings = Listings + ".ViewOwn";
        public const string ManageOwnListings = Listings + ".ManageOwn";
        public const string Leads = GroupName + ".Dealer.Leads";
        public const string ViewReceivedLeads = Leads + ".ViewReceived";
    }

    public static class Books
    {
        public const string Default = GroupName + ".Books";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Authors
    {
        public const string Default = GroupName + ".Authors";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class BackOffice
    {
        public const string Listings = GroupName + ".BackOffice.Listings";
        public const string ManageListings = GroupName + ".BackOffice.ManageListings";
        public const string Leads = GroupName + ".BackOffice.Leads";
        public const string ManageLeads = GroupName + ".BackOffice.ManageLeads";
    }

    public static class Administrator
    {
        public const string Default = GroupName + ".Administrator";
        public const string ManageCatalogs = Default + ".ManageCatalogs";
        public const string RestoreListings = Default + ".RestoreListings";
    }
    
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
