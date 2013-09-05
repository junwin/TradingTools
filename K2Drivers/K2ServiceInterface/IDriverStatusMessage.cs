using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2ServiceInterface
{
    public interface IDriverStatusMessage
    {
        string Text {get;set;}
        string DriverCode {get;set;}
        string Module { get; set; }
        List<IDriverSession> Sessions { get; set; }
        int State { get; set; }
    }
}
