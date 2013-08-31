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
