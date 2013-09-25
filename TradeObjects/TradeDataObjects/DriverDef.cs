using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaiTrade.Interfaces;

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
        private List<KaiTrade.Interfaces.IParameter> _userParameters;
        private IPEndpoint _IPEndPoint;
        private UserCredential _userCredential;
        private List<IMQExchange> _MQExchanges;
        private List<IMQRoutingKey> _MQRoutingKeys;

        public IIPEndpoint IPEndPoint
        {
            get { return _IPEndPoint; }
            set { _IPEndPoint = value as IPEndpoint; }
        }


        public IUserCredential UserCredential
        {
            get { return _userCredential; }
            set { _userCredential = value as UserCredential; }
        }


        public List<IMQExchange> MQExchanges
        {
            get { return _MQExchanges; }
            set { _MQExchanges = value; }
        }


        public List<IMQRoutingKey> MQRoutingKeys
        {
            get { return _MQRoutingKeys; }
            set { _MQRoutingKeys = value; }
        }

        public List<KaiTrade.Interfaces.IParameter> UserParameters
        {
            get { return _userParameters; }
            set { _userParameters = value; }
        }

        public DriverDef()
        {
            _userParameters = new List<KaiTrade.Interfaces.IParameter>();
            _MQExchanges = new List<IMQExchange>();
            _MQRoutingKeys = new List<IMQRoutingKey>();
        }

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


    public class UserCredential : IUserCredential
    {
        private string _userId = "";

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        private string _pwd = "";

        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }
    }

    public class IPEndpoint : IIPEndpoint
    {

        string _server = "";

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        int _port = 0;

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }


        string _name = "";

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        string _path = "";

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

    }
}
