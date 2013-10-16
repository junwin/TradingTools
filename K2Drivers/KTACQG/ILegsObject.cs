using System;
namespace KTACQG
{
    interface ILegsObject
    {
        string Duration { get; set; }
        int IgnoredQty { get; set; }
        bool IsPrimary { get; set; }
        string Version { get; set; }
    }
}
