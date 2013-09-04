using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;


namespace KTASimulator
{

    [JsonObject(MemberSerialization.OptIn)]
    public class SimulatorProduct
    {
        string _mnemonic;
        decimal _lowPrice = 0M;
        decimal _highPrice = 0M;
        bool _isAutoFill;
        bool _isCannedData = false;
        bool _runAsMarket = false;

        CannedData _cannedData;

        [JsonProperty]
        public CannedData CannedData
        {
            get { return _cannedData; }
            set { _cannedData = value; }
        }

        [JsonProperty]
        public bool RunAsMarket
        {
            get { return _runAsMarket; }
            set { _runAsMarket = value; }
        }

        [JsonProperty]
        public bool IsCannedData
        {
            get { return _isCannedData; }
            set { _isCannedData = value; }
        }

        [JsonProperty]
        public decimal LowPrice
        {
            get { return _lowPrice; }
            set { _lowPrice = value; }
        }

        [JsonProperty]
        public decimal HighPrice
        {
            get { return _highPrice; }
            set { _highPrice = value; }
        }

        [JsonProperty]
        public string Mnemonic
        {
            get { return _mnemonic; }
            set { _mnemonic = value; }
        }

        [JsonProperty]
        public bool IsAutoFill
        {
            get { return _isAutoFill; }
            set { _isAutoFill = value; }
        }
    }
}
