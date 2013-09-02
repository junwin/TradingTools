using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DriverBase
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DriverState : IDriverState
    {
        private string _configPath = "";
        private bool _asyncPrices = true;
        private bool _queueReplaceRequests = true;
        private bool _hideDriverUI = true;

        public bool HideDriverUI
        {
            get { return _hideDriverUI; }
            set { _hideDriverUI = value; }
        }

        public bool AsyncPrices
        {
            get { return _asyncPrices; }
            set { _asyncPrices = value; }
        }

        public bool QueueReplaceRequests
        {
            get { return _queueReplaceRequests; }
            set { _queueReplaceRequests = value; }
        }

        public string ConfigPath
        {
            get { return _configPath; }
            set { _configPath = value; }
        }
       
                       

    }
}
