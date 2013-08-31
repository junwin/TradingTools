
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
    public class RequestCancelRejectResponse : KaiTrade.Interfaces.IRequestCancelRejectResponse
    {
        private string _orderID = "";
        private string _clOrdID = "";
        private string _origClOrdID = "";
        private string _ordStatus;
        private KaiTrade.Interfaces.CxlRejResponseTo _cxlRejResponseTo;

        [DataMember]
        [JsonProperty]
        public string OrderID
        {
            get { return _orderID; }
            set { _orderID = value; }
        }

        [DataMember]
        [JsonProperty]
        public string ClOrdID
        {
            get { return _clOrdID; }
            set { _clOrdID = value; }
        }

        [DataMember]
        [JsonProperty]
        public string OrigClOrdID
        {
            get { return _origClOrdID; }
            set { _origClOrdID = value; }
        }

        [DataMember]
        [JsonProperty]
        public string OrdStatus
        {
            get { return _ordStatus; }
            set { _ordStatus = value; }
        }

        [DataMember]
        [JsonProperty]
        public KaiTrade.Interfaces.CxlRejResponseTo CxlRejResponseTo
        {
            get { return _cxlRejResponseTo; }
            set { _cxlRejResponseTo = value; }
        }
        private DateTime _transactTime = DateTime.Now;

        [DataMember]
        [JsonProperty]
        public DateTime TransactTime
        {
            get { return _transactTime; }
            set { _transactTime = value; }
        }
        private string _reasonText = "";

      
        private KaiTrade.Interfaces.CxlRejReason _cxlRejReason;

        [DataMember]
        [JsonProperty]
        public KaiTrade.Interfaces.CxlRejReason CxlRejReason
        {
            get { return _cxlRejReason; }
            set { _cxlRejReason = value; }
        }

        [DataMember]
        [JsonProperty]
        public string ReasonText
        {
            get { return _reasonText; }
            set { _reasonText = value; }
        }
    }
}
