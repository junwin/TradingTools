using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.CancelOrderRequest")]
    public class CancelOrderRequest : KaiTrade.Interfaces.ICancelOrderRequest
    {
        private string _mnemonic;
        private string _clOrdID;
        private string _origClOrdID;
        private int? _retryCount;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public string Mnemonic
        {
            get { return _mnemonic; }
            set { _mnemonic = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public string ClOrdID
        {
            get { return _clOrdID; }
            set { _clOrdID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public string OrigClOrdID
        {
            get { return _origClOrdID; }
            set { _origClOrdID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int? RetryCount
        {
            get { return _retryCount; }
            set { _retryCount = value; }
        }
    }
}
