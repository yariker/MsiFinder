using System;

namespace MsiFinder.Model
{
    [Flags]
    public enum InstallContext
    {
        None = 0,
        UserManaged = 1,
        UserUnmanaged = 2,
        Machine = 4,
        All = UserManaged | UserUnmanaged | Machine,
        AllUserManaged = 8,
    }
}