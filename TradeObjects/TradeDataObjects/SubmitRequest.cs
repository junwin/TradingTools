using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class SubmitRequest : KaiTrade.Interfaces.ISubmitRequest
    {
        private long _orderQty;

        [DataMember]
        [JsonProperty]
        public long OrderQty
        {
            get { return _orderQty; }
            set { _orderQty = value; }
        }

        private string _ordType;

        [DataMember]
        [JsonProperty]
        public string OrdType
        {
            get { return _ordType; }
            set { _ordType = value; }
        }

        private decimal? _price;

        [DataMember]
        [JsonProperty]
        public decimal? Price
        {
            get { return _price; }
            set { _price = value; }
        }


        private decimal? _stopPx;

        [DataMember]
        [JsonProperty]
        public decimal? StopPx
        {
            get { return _stopPx; }
            set { _stopPx = value; }
        }

        private string _clOrdID;

        [DataMember]
        [JsonProperty]
        public string ClOrdID
        {
            get { return _clOrdID; }
            set { _clOrdID = value; }
        }

        private string _side;

        [DataMember]
        [JsonProperty]
        public string Side
        {
            get { return _side; }
            set { _side = value; }
        }

        private string _mnemonic;

        [DataMember]
        [JsonProperty]
        public string Mnemonic
        {
            get { return _mnemonic; }
            set { _mnemonic = value; }
        }

        private string _securityID;

        [DataMember]
        [JsonProperty]
        public string SecurityID
        {
            get { return _securityID; }
            set { _securityID = value; }
        }

        private string _account;

        [DataMember]
        [JsonProperty]
        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }
    }
}
