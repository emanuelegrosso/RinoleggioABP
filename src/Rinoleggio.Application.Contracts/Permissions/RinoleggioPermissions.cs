namespace Rinoleggio.Permissions;

public static class RinoleggioPermissions
{
    public const string GroupName = "Rinoleggio";

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

    public static class Contracts
    {
        public const string Default = GroupName + ".Contracts";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string ManageAll = Default + ".ManageAll";
        public const string Suspend = Default + ".Suspend";
        public const string Close = Default + ".Close";
        public const string Delete = Default + ".Delete";
        public const string Restore = Default + ".Restore";
    }

    public static class Leads
    {
        public const string Default = GroupName + ".Leads";
        public const string Create = Default + ".Create";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string ViewAll = Default + ".ViewAll";
        public const string Manage = Default + ".Manage";
    }

    public static class Profile
    {
        public const string Default = GroupName + ".Profile";
    }

    public static class Lookups
    {
        public const string Default = GroupName + ".Lookups";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
