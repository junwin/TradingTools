using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public interface IDriverSession
    {
        string SessionName {get;set;}
        string FixSessionID {get;set;}
        string BeginString {get;set;}
        string SID {get;set;}
        string TID {get;set;}
        string UserName { get; set; }
        KaiTrade.Interfaces.Status State { get; set; }
        string StatusText { get; set; }
        string Text { get; set; }
        string Key { get; set; }
    }
}
