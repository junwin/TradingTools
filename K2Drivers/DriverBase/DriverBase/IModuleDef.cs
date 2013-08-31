using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public interface IModuleDef
    {
         string Name {get; set;}
         string PackageFullName {get; set;}
         string PackageVersion { get; set; }
         int Instance { get; set; }
         string HostName {get; set;}
         string Server {get; set;}
         string HostModule { get; set; }
         string HostFileName { get; set; }
         string HostVersionInfo { get; set; }
         string Tag { get; set; }

    }
}
