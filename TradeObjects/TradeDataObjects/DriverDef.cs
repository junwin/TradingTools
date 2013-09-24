using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2DataObjects
{
    public class DriverDef : KaiTrade.Interfaces.IDriverDef
    {
        private string _name = "";
        private string _routeCode = "";
        private string _configPath = "";
        private bool _manualStart = false;
        private bool _liveMarket = false;
        private bool _hideDriverUI = true;
        private bool _asyncPrices = true;
        private bool _queueReplaceRequests = true;

        public string RouteCode
        {
            get { return _routeCode; }
            set { _routeCode = value; }
        }
        

        public string ConfigPath
        {
            get { return _configPath; }
            set { _configPath = value; }
        }
        

        public bool ManualStart
        {
            get { return _manualStart; }
            set { _manualStart = value; }
        }
        

        public bool LiveMarket
        {
            get { return _liveMarket; }
            set { _liveMarket = value; }
        }
        

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

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _code = "";

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        private string _loadPath = "";

        public string LoadPath
        {
            get { return _loadPath; }
            set { _loadPath = value; }
        }
        private bool _enabled = false;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
    }
}
