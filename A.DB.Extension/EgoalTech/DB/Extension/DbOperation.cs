namespace EgoalTech.DB.Extension
{
    using System;

    [Flags]
    public enum DbOperation : byte
    {
        Create = 1,
        CU = 3,
        CUD = 11,
        Delete = 8,
        None = 0,
        Read = 4,
        Update = 2
    }
}

