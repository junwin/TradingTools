using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using K2ServiceInterface;

namespace DriverBase
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DriverStatusMessage : IDriverStatusMessage
    {
        public List<IDriverSession> Sessions {get; set;}        
        public string Text {get; set;}
        public string DriverCode {get; set;}
        public string Module  {get; set;}
        public int State { get; set; } 
    }
}
