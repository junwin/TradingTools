using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;

namespace K2DataObjects
{
    /// <summary>
    /// Models some financial instritution - for example an FCM or broker
    /// </summary>
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.ModifyOrderRequest")]
    public class ModifyOrderRequest : KaiTrade.Interfaces.IModifyOrderRequst
    {
        private double? _price;
        private int? _retryCount;
        private double? _stopPrice;
        private long? _qty;
        private string _clOrdID;
        private string _mnemonic;
        private string _origClOrdID;


        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public double? Price
        {
            get { return _price; }
            set { _price = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public double? StopPrice
        {
            get { return _stopPrice; }
            set { _stopPrice = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public long? Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public string Mnemonic
        {
            get { return _mnemonic; }
            set { _mnemonic = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public string ClOrdID
        {
            get { return _clOrdID; }
            set { _clOrdID = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public string OrigClOrdID
        {
            get { return _origClOrdID; }
            set { _origClOrdID = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public int? RetryCount
        {
            get { return _retryCount; }
            set { _retryCount = value; }
        }
    }
}
