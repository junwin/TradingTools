/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
 *                     All Rights Reserved Worldwide
 *
 * STRICTLY PROPRIETARY and CONFIDENTIAL
 *
 * WARNING:  This file is the confidential property of KaiTrade LLC For
 * use only by those with the express written permission and license from
 * KaiTrade LLC.  Unauthorized reproduction, distribution, use or disclosure
 * of this file or any program (or document) is prohibited.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using K2ServiceInterface;

namespace L1PriceSupport
{
    public class Item<T>
    {
    }
    /// <summary>
    /// Used to publish real time prices from KaiTrade drivers
    /// </summary>
    public class PXPublisher : KaiTrade.Interfaces.IPublisher, IL1PX
    {
        /// <summary>
        /// used for lock()
        /// </summary>
        private Object m_Token1 = new Object();

        private System.Collections.Hashtable m_Image;
        private System.Collections.Generic.List<KaiTrade.Interfaces.IField> m_UpdateFields;
        private System.Collections.Generic.List<KaiTrade.Interfaces.IField> m_Fields;
        private System.Collections.Generic.List<KaiTrade.Interfaces.ISubscriber> m_Subscribers;

        private System.Collections.Generic.List<KaiTrade.Interfaces.IField> m_StatusInfo;
        public log4net.ILog m_Log = log4net.LogManager.GetLogger("KaiTrade");
        private string m_Key;

        private string m_Name;

        /// <summary>
        /// Product that this PX publisher is publishing data for
        /// </summary>
        private KaiTrade.Interfaces.IProduct m_Product;

        private string m_PublisherType = "PXPublisher";
        /// <summary>
        /// User defined string tag
        /// </summary>
        private string m_Tag;

        /// <summary>
        /// Is the suject changed
        /// </summary>
        //NOT USED? private bool m_isChanged = false;

        KaiTrade.Interfaces.Status m_PubStatus;

        private KaiTrade.Interfaces.IField m_Status;

        private DateTime m_APIUpdateTime = DateTime.Now;

        /// <summary>
        /// Get/Set the product (if defined)M that we are publishing MD info for
        /// </summary>
        public KaiTrade.Interfaces.IProduct Product
        {
            get
            {
                return m_Product;
            }
            set
            {
                m_Product = value;
            }
        }
        /// <summary>
        /// API Time of last update(from driver)
        /// </summary>
        public DateTime APIUpdateTime
        {
            get { return m_APIUpdateTime; }
            set{m_APIUpdateTime = value;}
        }
        public string OfferPrice
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.OFFER1_PRICE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.OFFER1_PRICE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER1_PRICE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER1_PRICE].Changed = true;
            }
        }
        public string OfferSize
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.OFFER1_SIZE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.OFFER1_SIZE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER1_SIZE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER1_SIZE].Changed = true;
            }
        }

        public void SetOfferDepth(int myPos, string myPx, string mySz)
        {
            if (myPos < KaiTrade.Interfaces.MDConstants.DepthLevels)
            {
                SetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos), myPx);
                SetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos) + 1, mySz);
            }
        }

        public void SetBidDepth(int myPos, string myPx, string mySz)
        {
            if (myPos < KaiTrade.Interfaces.MDConstants.DepthLevels)
            {
                SetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos) + 2, myPx);
                SetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos) + 3, mySz);
            }
        }

        public void GetBidDepth(int myPos, out string myPx, out string mySz)
        {
            myPx = "";
            mySz = "";
            if (myPos < KaiTrade.Interfaces.MDConstants.DepthLevels)
            {
                myPx = GetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos) + 2);
                mySz = GetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos) + 3);
            }
        }

        public void GetOfferDepth(int myPos, out string myPx, out string mySz)
        {
            myPx = "";
            mySz = "";
            if (myPos < KaiTrade.Interfaces.MDConstants.DepthLevels)
            {
                myPx = GetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos));
                mySz = GetField(KaiTrade.Interfaces.MDConstants.BidOfferStart + (KaiTrade.Interfaces.MDConstants.DepthElements * myPos) + 1);
            }
        }

        public string BidSize
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.BID1_SIZE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.BID1_SIZE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.BID1_SIZE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.BID1_SIZE].Changed = true;
            }
        }
        public string BidPrice
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.BID1_PRICE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.BID1_PRICE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.BID1_PRICE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.BID1_PRICE].Changed = true;
            }
        }
        public string BidExchange
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.BID_EXCHANGE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.BID_EXCHANGE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.BID_EXCHANGE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.BID_EXCHANGE].Changed = true;
            }
        }

        public string OfferExchange
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE].Changed = true;
            }
        }

        /// <summary>
        /// QuoteID of a update by a quote - this is only valid in response to
        /// a quote request
        /// </summary>
        public string QuoteID
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.QUOTEID].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.QUOTEID, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE].Changed = true;
            }
        }

        long IL1PX.ValidityPeriod
        {
            get
            {
                return long.Parse(this.ValidityPeriod);
            }
        }
        /// <summary>
        /// Get/set the length of time in milli seconds that this quote (or update) is valid for
        /// </summary>
        public string ValidityPeriod
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.VALIDITYPERIOD].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.VALIDITYPERIOD, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE].Changed = true;
            }
        }

        public string ExchangeTime
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.EXCH_TIME].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.EXCH_TIME, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.EXCH_TIME].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.EXCH_TIME].Changed = true;
            }
        }
        public string Time
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TIME].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TIME, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TIME].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TIME].Changed = true;
            }
        }
        public string MMID
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.MMID].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.MMID, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.MMID].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.MMID].Changed = true;
            }
        }

        public string DayHigh
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.DAY_HIGH].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.DAY_HIGH, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.DAY_HIGH].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.DAY_HIGH].Changed = true;
            }
        }

        public string DayLow
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.DAY_LOW].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.DAY_LOW, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.DAY_LOW].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.DAY_LOW].Changed = true;
            }
        }

        public string TradeFlags
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_FLAGS].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_FLAGS, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_FLAGS].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_FLAGS].Changed = true;
            }
        }
        public string TradeProdExch
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_PRODEXCH].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_PRODEXCH, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_PRODEXCH].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_PRODEXCH].Changed = true;
            }
        }
        public string TradeSymbol
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SYMBOL].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_SYMBOL, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SYMBOL].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SYMBOL].Changed = true;
            }
        }
        public string TradeExchangeTime
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_EXCHTIME].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_EXCHTIME, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_EXCHTIME].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_EXCHTIME].Changed = true;
            }
        }

        public string TradeSize
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SIZE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_SIZE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SIZE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SIZE].Changed = true;
            }
        }
        public string TradePrice
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_PRICE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_PRICE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_PRICE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_PRICE].Changed = true;
            }
        }
        public string TradeVolume
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_VOLUME].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_VOLUME, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_VOLUME].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_VOLUME].Changed = true;
            }
        }
        public string TradeIndicator
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_INDICATOR].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_INDICATOR, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_INDICATOR].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_INDICATOR].Changed = true;
            }
        }
        public string TradeTickIndicator
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_TICKIND].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_TICKIND, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_TICKIND].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_TICKIND].Changed = true;
            }
        }
        public string TradeSerial
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SERIAL].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_SERIAL, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SERIAL].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_SERIAL].Changed = true;
            }
        }

        public string TradeTime
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_TIME].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_TIME, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_TIME].Value = value;
            }
        }
        public string TradeExch
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_EXCH].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.TRADE_EXCH, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_EXCH].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.TRADE_EXCH].Changed = true;
            }
        }
        public string Open
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.OPEN].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.OPEN, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.OPEN].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.OPEN].Changed = true;
            }
        }
        public string PrevClose
        {
            get
            {
                return m_Fields[KaiTrade.Interfaces.MDConstants.PREVCLOSE].Value;
            }
            set
            {
                SetField(KaiTrade.Interfaces.MDConstants.PREVCLOSE, value);
                //m_Fields[KaiTrade.Interfaces.MDConstants.PREVCLOSE].Value = value;
                //m_Fields[KaiTrade.Interfaces.MDConstants.PREVCLOSE].Changed = true;
            }
        }

        public PXPublisher()
        {
            m_Log = log4net.LogManager.GetLogger("KaiTrade");

            m_Fields = new List<KaiTrade.Interfaces.IField>();
            initFields();

            m_UpdateFields = new System.Collections.Generic.List<KaiTrade.Interfaces.IField>();
            m_Subscribers = new System.Collections.Generic.List<KaiTrade.Interfaces.ISubscriber>();

            m_StatusInfo = new System.Collections.Generic.List<KaiTrade.Interfaces.IField>();
            m_Image = new System.Collections.Hashtable();
            m_Name = "";
            //NOT USED? m_isChanged = false;
            m_PubStatus = KaiTrade.Interfaces.Status.opening;
        }

        public string this[int index]
        {
            set { (m_Fields[index]).Value = value; }
            get { return (m_Fields[index]).Value; }
        }

        public void SetField(int myIdent, string myValue)
        {
            m_Fields[myIdent].Value = myValue;
            m_Fields[myIdent].Changed = true;
           //NOT USED? m_isChanged = false;
        }
        /// <summary>
        /// Set the field but only if the new value is different
        /// </summary>
        /// <param name="myIdent"></param>
        /// <param name="myValue"></param>
        /// <param name="ifDifferent"></param>
        public void SetFieldIfDifferent(int myIdent, string myValue)
        {
            if (m_Fields[myIdent].Value != myValue)
            {
                m_Fields[myIdent].Value = myValue;
                m_Fields[myIdent].Changed = true;
            }

        }

        public string GetField(int myIdent)
        {
            return m_Fields[myIdent].Value;
        }

        public int DepthLevelCount
        {
            get { return 5; }
            set { ;}
        }

        private void addField2List(ref System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFields, string myID, int myIdent)
        {
            K2DataObjects.Field myField;
            myField = new K2DataObjects.Field(myID);
            myFields.Add(myField);
        }
        private void initFields()
        {
            addField2List(ref m_Fields, "Status", KaiTrade.Interfaces.MDConstants.FieldList);
            addField2List(ref m_Fields, "FieldList", KaiTrade.Interfaces.MDConstants.FieldList);

            addField2List(ref m_Fields, "OFFER_EXCHANGE", KaiTrade.Interfaces.MDConstants.OFFER_EXCHANGE);
            addField2List(ref m_Fields, "BID_EXCHANGE", KaiTrade.Interfaces.MDConstants.BID_EXCHANGE);
            addField2List(ref m_Fields, "EXCH_TIME", KaiTrade.Interfaces.MDConstants.EXCH_TIME);
            addField2List(ref m_Fields, "TIME", KaiTrade.Interfaces.MDConstants.TIME);
            addField2List(ref m_Fields, "MMID", KaiTrade.Interfaces.MDConstants.MMID);
            addField2List(ref m_Fields, "DAY_HIGH", KaiTrade.Interfaces.MDConstants.DAY_HIGH);
            addField2List(ref m_Fields, "DAY_LOW", KaiTrade.Interfaces.MDConstants.DAY_LOW);
            addField2List(ref m_Fields, "TRADE_FLAGS", KaiTrade.Interfaces.MDConstants.TRADE_FLAGS);
            addField2List(ref m_Fields, "TRADE_PRODEXCH", KaiTrade.Interfaces.MDConstants.TRADE_PRODEXCH);
            addField2List(ref m_Fields, "TRADE_SYMBOL", KaiTrade.Interfaces.MDConstants.TRADE_SYMBOL);
            addField2List(ref m_Fields, "TRADE_EXCHTIME", KaiTrade.Interfaces.MDConstants.TRADE_EXCHTIME);
            addField2List(ref m_Fields, "TRADE_SIZE", KaiTrade.Interfaces.MDConstants.TRADE_SIZE);
            addField2List(ref m_Fields, "TRADE_PRICE", KaiTrade.Interfaces.MDConstants.TRADE_PRICE);
            addField2List(ref m_Fields, "TRADE_VOLUME", KaiTrade.Interfaces.MDConstants.TRADE_VOLUME);
            addField2List(ref m_Fields, "TRADE_INDICATOR", KaiTrade.Interfaces.MDConstants.TRADE_INDICATOR);
            addField2List(ref m_Fields, "TRADE_SERIAL", KaiTrade.Interfaces.MDConstants.TRADE_SERIAL);
            addField2List(ref m_Fields, "TRADE_TICKIND", KaiTrade.Interfaces.MDConstants.TRADE_TICKIND);
            addField2List(ref m_Fields, "TRADE_TIME", KaiTrade.Interfaces.MDConstants.TRADE_TIME);
            addField2List(ref m_Fields, "TRADE_EXCH", KaiTrade.Interfaces.MDConstants.TRADE_EXCH);
            addField2List(ref m_Fields, "OPEN", KaiTrade.Interfaces.MDConstants.OPEN);
            addField2List(ref m_Fields, "PREVCLOSE", KaiTrade.Interfaces.MDConstants.PREVCLOSE);

            addField2List(ref m_Fields, "SETTLEMENTPRICE", KaiTrade.Interfaces.MDConstants.SETTLEMENTPRICE);
            addField2List(ref m_Fields, "VWAPPRICE", KaiTrade.Interfaces.MDConstants.VWAPPRICE);
            addField2List(ref m_Fields, "OPENINTEREST", KaiTrade.Interfaces.MDConstants.OPENINTEREST);
            addField2List(ref m_Fields, "IMBALANCE", KaiTrade.Interfaces.MDConstants.IMBALANCE);
            addField2List(ref m_Fields, "INDEXPRICE", KaiTrade.Interfaces.MDConstants.INDEXPRICE);
            addField2List(ref m_Fields, "EXCH_DATE", KaiTrade.Interfaces.MDConstants.EXCH_DATE);

            addField2List(ref m_Fields, "NETCHGPREVDAY", KaiTrade.Interfaces.MDConstants.NETCHGPREVDAY);

            addField2List(ref m_Fields, "OFFER1_PRICE", KaiTrade.Interfaces.MDConstants.OFFER1_PRICE);
            addField2List(ref m_Fields, "OFFER1_SIZE", KaiTrade.Interfaces.MDConstants.OFFER1_SIZE);
            addField2List(ref m_Fields, "BID1_PRICE", KaiTrade.Interfaces.MDConstants.BID1_PRICE);
            addField2List(ref m_Fields, "BID1_SIZE", KaiTrade.Interfaces.MDConstants.BID1_SIZE);

            addField2List(ref m_Fields, "OFFER2_PRICE", KaiTrade.Interfaces.MDConstants.OFFER2_PRICE);
            addField2List(ref m_Fields, "OFFER2_SIZE", KaiTrade.Interfaces.MDConstants.OFFER2_SIZE);
            addField2List(ref m_Fields, "BID2_PRICE", KaiTrade.Interfaces.MDConstants.BID2_PRICE);
            addField2List(ref m_Fields, "BID2_SIZE", KaiTrade.Interfaces.MDConstants.BID2_SIZE);

            addField2List(ref m_Fields, "OFFER3_PRICE", KaiTrade.Interfaces.MDConstants.OFFER3_PRICE);
            addField2List(ref m_Fields, "OFFER3_SIZE", KaiTrade.Interfaces.MDConstants.OFFER3_SIZE);
            addField2List(ref m_Fields, "BID3_PRICE", KaiTrade.Interfaces.MDConstants.BID3_PRICE);
            addField2List(ref m_Fields, "BID3_SIZE", KaiTrade.Interfaces.MDConstants.BID3_SIZE);

            addField2List(ref m_Fields, "OFFER4_PRICE", KaiTrade.Interfaces.MDConstants.OFFER4_PRICE);
            addField2List(ref m_Fields, "OFFER4_SIZE", KaiTrade.Interfaces.MDConstants.OFFER4_SIZE);
            addField2List(ref m_Fields, "BID4_PRICE", KaiTrade.Interfaces.MDConstants.BID4_PRICE);
            addField2List(ref m_Fields, "BID4_SIZE", KaiTrade.Interfaces.MDConstants.BID4_SIZE);

            addField2List(ref m_Fields, "OFFER5_PRICE", KaiTrade.Interfaces.MDConstants.OFFER5_PRICE);
            addField2List(ref m_Fields, "OFFER5_SIZE", KaiTrade.Interfaces.MDConstants.OFFER5_SIZE);
            addField2List(ref m_Fields, "BID5_PRICE", KaiTrade.Interfaces.MDConstants.BID5_PRICE);
            addField2List(ref m_Fields, "BID5_SIZE", KaiTrade.Interfaces.MDConstants.BID5_SIZE);
            addField2List(ref m_Fields, "TICKDIR", KaiTrade.Interfaces.MDConstants.TICKDIR);
            addField2List(ref m_Fields, "QUOTEID", KaiTrade.Interfaces.MDConstants.QUOTEID);
            addField2List(ref m_Fields, "VALIDITYPERIOD", KaiTrade.Interfaces.MDConstants.VALIDITYPERIOD);
        }

        /// <summary>
        /// Add all the fields and current value that we can publish to a list passed in
        /// </summary>
        /// <param name="myFieldList"></param>
        private void addIntrinsicFields(ref System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList)
        {
            foreach (KaiTrade.Interfaces.IField myField in m_Fields)
            {
                if (myField.IsValid)
                {
                    myFieldList.Add(myField);
                }
            }
        }

        private void resetIntrinsicFieldChanged()
        {
           //NOT USED? m_isChanged = false;
            foreach (KaiTrade.Interfaces.IField myField in m_Fields)
            {
                myField.Changed = false;
            }
           //NOT USED? m_isChanged = false;
        }

        /// <summary>
        /// Add any fields that have changed to a field list
        /// </summary>
        /// <param name="myFieldList">fieldlist that will be added to</param>
        private void addChangedIntrinsicFields(ref System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList)
        {
            foreach (KaiTrade.Interfaces.IField myField in m_Fields)
            {
                if (myField.IsValid)
                {
                    if (myField.Changed)
                    {
                        myFieldList.Add(myField);
                        myField.Changed = false;
                    }
                }
               //NOT USED? m_isChanged = false;
            }
        }

        /// <summary>
        /// Set the image values for a field list passed in
        /// </summary>
        /// <param name="myFieldList"></param>
        private void applyFieldList(System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList)
        {
            foreach (KaiTrade.Interfaces.IField myField in myFieldList)
            {
                switch (myField.ID)
                {
                    case "OFFER1_PRICE":
                        this.OfferPrice = myField.Value;
                        break;
                    case "OFFER1_SIZE":
                        this.OfferSize = myField.Value;
                        break;
                    case "OFFER_EXCHANGE":
                        this.OfferExchange = myField.Value;
                        break;
                    case "BID1_PRICE":
                        this.BidPrice = myField.Value;
                        break;
                    case "BID1_SIZE":
                        this.BidSize = myField.Value;
                        break;
                    case "BID_EXCHANGE":
                        this.BidExchange = myField.Value;
                        break;
                    case "EXCH_TIME":
                        this.ExchangeTime = myField.Value;
                        break;
                    case "TIME":
                        this.Time = myField.Value;
                        break;
                    case "MMID":
                        this.MMID = myField.Value;
                        break;
                    case "DAY_HIGH":
                        this.DayHigh = myField.Value;
                        break;
                    case "DAY_LOW":
                        this.DayLow = myField.Value;
                        break;
                    case "TRADE_FLAGS":
                        this.TradeFlags = myField.Value;
                        break;
                    case "TRADE_PRODEXCH":
                        this.TradeProdExch = myField.Value;
                        break;
                    case "TRADE_SYMBOL":
                        this.TradeSymbol = myField.Value;
                        break;
                    case "TRADE_EXCHTIME":
                        this.TradeExchangeTime = myField.Value;
                        break;
                    case "TRADE_SIZE":
                        this.TradeSize = myField.Value;
                        break;
                    case "TRADE_PRICE":
                        this.TradePrice = myField.Value;
                        break;
                    case "TRADE_VOLUME":
                        this.TradeVolume = myField.Value;
                        break;
                    case "TRADE_INDICATOR":
                        this.TradeIndicator = myField.Value;
                        break;
                    case "TRADE_TICKINDICATOR":
                        this.TradeTickIndicator = myField.Value;
                        break;
                    case "TRADE_SERIAL":
                        this.TradeSerial = myField.Value;
                        break;
                    case "TRADE_TICKIND":
                        //this. = myField.Value;
                        break;
                    case "TRADE_TIME":
                        this.TradeTime = myField.Value;
                        break;
                    case "TRADE_EXCH":
                        this.TradeExch = myField.Value;
                        break;
                    case "OPEN":
                        this.Open = myField.Value;
                        break;
                    case "PREVCLOSE":
                        this.PrevClose = myField.Value;
                        break;
                    case "TICKDIR":
                        //this.ti = myField.Value;
                        break;
                    case "QUOTEID":
                        this.QuoteID = myField.Value;
                        break;
                    case "VALIDITYPERIOD":
                        this.ValidityPeriod = myField.Value;
                        break;
                }
            }
        }

        /// <summary>
        /// Send a list of all the fields to our observers
        /// </summary>
        private void doImage()
        {
            try
            {
                // if we have some observers
                if (m_Subscribers.Count > 0)
                {
                    // create a list and add all the fields that we can publish with their current value
                    System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList = new List<KaiTrade.Interfaces.IField>();
                    addIntrinsicFields(ref myFieldList);

                    // add the current time as the update time to the list
                    K2DataObjects.Field myUpdTimeField;
                    myUpdTimeField = new K2DataObjects.Field("UPDTIME", System.Environment.TickCount.ToString());

                    myFieldList.Add(myUpdTimeField);

                    // update all of our observers
                    foreach (KaiTrade.Interfaces.ISubscriber mySubscriber in m_Subscribers)
                    {
                        try
                        {
                            mySubscriber.OnImage(this, myFieldList);
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("doImage - invoke", myE);
                        }
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("doImage", myE);
            }
        }

        /// <summary>
        /// send the updated fields to our client observers
        /// </summary>
        private void doUpdate()
        {
            try
            {
                // if we have some observers
                if (m_Subscribers.Count > 0)
                {
                    // create a list of changed fields with their current values
                    System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList = new List<KaiTrade.Interfaces.IField>();
                    addChangedIntrinsicFields(ref myFieldList);

                    // add the current time as the update time to the list
                    K2DataObjects.Field myUpdTimeField;
                    myUpdTimeField = new K2DataObjects.Field("UPDTIME", System.Environment.TickCount.ToString());

                    myFieldList.Add(myUpdTimeField);

                    // send the update to all our observers
                    foreach (KaiTrade.Interfaces.ISubscriber mySubscriber in m_Subscribers)
                    {
                        try
                        {
                            if (mySubscriber != null)
                            {
                                mySubscriber.OnUpdate(this, myFieldList);
                            }
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("doUpdate - invoke", myE);
                        }
                    }

                    // mark all fields as unchanged
                    resetIntrinsicFieldChanged();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("doUpdate", myE);
            }
        }
        private void applyStatus(System.Collections.Generic.List<KaiTrade.Interfaces.IField> myStatusInfo)
        {
            m_StatusInfo = myStatusInfo;
            foreach (KaiTrade.Interfaces.ISubscriber mySubscriber in m_Subscribers)
            {
                try
                {
                    mySubscriber.OnStatusChange(this, m_StatusInfo);
                }
                catch (Exception myE)
                {
                    m_Log.Error("updateStatus - invoke", myE);
                }
            }
        }

       /// <summary>
       /// Apply any change in status as a status message and
       /// update to the internal status header
       /// </summary>
       /// <param name="myStatus"></param>
        private void updateStatus(KaiTrade.Interfaces.Status myStatus)
        {
            try
            {
                // if the status has changes then do an
                // status update
                if (m_PubStatus != myStatus)
                {
                    m_PubStatus = myStatus;
                    m_Status = new K2DataObjects.Field("STATUS", myStatus.ToString());
                    m_StatusInfo = new System.Collections.Generic.List<KaiTrade.Interfaces.IField>();
                    m_StatusInfo.Add(m_Status);
                    applyStatus(m_StatusInfo);
                }

                // then update the synthetic status field

                doUpdate();
            }
            catch (Exception myE)
            {
                m_Log.Error("updateStatus", myE);
            }
        }

        #region Publisher Members

        public void Close()
        {
            m_Log.Info("Subject:" + m_Name + "is closing");

            updateStatus(KaiTrade.Interfaces.Status.closed);
            m_Subscribers.Clear();
            m_Image.Clear();
            m_UpdateFields.Clear();
            m_StatusInfo.Clear();
            m_Fields.Clear();

            m_Name = "";
        }

        public void OnImage(List<KaiTrade.Interfaces.IField> itemList)
        {
            // note we expect to be updated by the properties - not this way
            try
            {
                if (itemList != null)
                {
                    applyFieldList(itemList);
                }
                doImage();
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.OnImage", myE);
            }
        }

        /// <summary>
        /// Get/Set the fields list for the publisher - setting this will replace all
        /// existing fields in the publisher and issue an image
        /// </summary>
        public System.Collections.Generic.List<KaiTrade.Interfaces.IField> FieldList
        {
            get { return m_Fields; }
            set
            {
                m_Fields = value;
                doImage();
            }
        }

        /// <summary>
        /// Get/Set the publisher type - this is user defined
        /// </summary>
        public string PublisherType
        { get { return m_PublisherType; } set { m_PublisherType = value; } }

        /// <summary>
        /// get/set the publisher base status - will event all subscribers
        /// </summary>
        public KaiTrade.Interfaces.Status Status
        {
            get { return m_PubStatus; }
            set {updateStatus(value);}
        }

        public void OnStatusChange(List<KaiTrade.Interfaces.IField> itemList)
        {
            try
            {
                if (itemList != null)
                {
                    applyStatus(itemList);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.OnStatusChange", myE);
            }
        }

        public void OnUpdate(List<KaiTrade.Interfaces.IField> itemList)
        {
            lock (m_Token1)
            {
                // note we expect to be updated by the properties - not this way
                try
                {
                    if (itemList != null)
                    {
                        applyFieldList(itemList);
                    }
                    doUpdate();
                }
                catch (Exception myE)
                {
                    m_Log.Error("Publisher.OnUpdate", myE);
                }
            }
        }

        /// <summary>
        /// update some arbitary field in the publisher - note not all publishers
        /// may support this
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        public void OnUpdate(string myID, string myValue)
        {
            try
            {
                //this.updateField(myID, myValue);
                //doUpdate();
            }
            catch (Exception myE)
            {
                m_Log.Error("OnUpdate:field:", myE);
            }
        }

        public void ApplyUpdate(KaiTrade.Interfaces.IPXUpdate update)
        {
            try
            {
                if (update.BidSize.HasValue)
                    SetFieldIfDifferent(KaiTrade.Interfaces.MDConstants.BID1_SIZE, update.BidSize.ToString());
                if (update.BidPrice.HasValue)
                    SetFieldIfDifferent(KaiTrade.Interfaces.MDConstants.BID1_PRICE, update.BidPrice.ToString());
                if (update.OfferPrice.HasValue)
                    SetFieldIfDifferent(KaiTrade.Interfaces.MDConstants.OFFER1_PRICE, update.OfferPrice.ToString());
                if (update.OfferSize.HasValue)
                    SetFieldIfDifferent(KaiTrade.Interfaces.MDConstants.OFFER1_SIZE, update.OfferSize.ToString());
                if (update.TradePrice.HasValue)
                    SetField(KaiTrade.Interfaces.MDConstants.TRADE_PRICE, update.TradePrice.ToString());
                if (update.TradeVolume.HasValue)
                    SetField(KaiTrade.Interfaces.MDConstants.TRADE_SIZE, update.TradeVolume.ToString());
                if (update.ServerTicks.HasValue)
                {
                    SetField(KaiTrade.Interfaces.MDConstants.TRADE_TIME, update.ServerTicks.ToString());
                    APIUpdateTime = new DateTime(update.ServerTicks.Value);
                }
                else
                {
                    APIUpdateTime = DateTime.Now;
                }
                if (update.DayHigh.HasValue)
                   SetField(KaiTrade.Interfaces.MDConstants.DAY_HIGH, update.DayHigh.ToString());
                if (update.DayLow.HasValue)
                    SetField(KaiTrade.Interfaces.MDConstants.DAY_LOW, update.DayLow.ToString());
            }
            catch (Exception myE)
            {
                m_Log.Error("ApplyUpdate", myE);
            }
        }

        private decimal? getDecimal(string decimalValue)
        {
            decimal? returnValue=null;
             try
            {
                if(decimalValue.Length>0)
                {
                    decimal d;
                    if(decimal.TryParse(decimalValue, out d))
                    {
                    returnValue =d;
                    }
                }
            }
            catch
            {
                 // dont log
            }

            return returnValue;
        }
        /// <summary>
        /// Get the price values as an updates
        /// </summary>
        /// <returns></returns>
        public KaiTrade.Interfaces.IPXUpdate AsUpdate()
        {
            KaiTrade.Interfaces.IPXUpdate update = null;
            try
            {
                update = new PXUpdateBase(this.Product);
                update.BidSize = getDecimal(this.BidSize);
                update.BidPrice = getDecimal(this.BidPrice);
                update.OfferPrice = getDecimal(this.OfferPrice);
                update.OfferSize = getDecimal(this.OfferSize);

                update.TradePrice = getDecimal(this.TradePrice);
                update.TradeVolume = getDecimal(this.TradeSize);
                try
                {
                update.ServerTicks = long.Parse(TradeTime);
                }
                catch
                {
                }

                try
                {
                    update.Ticks = this.APIUpdateTime.Ticks;
                }
                catch
                {
                }

                update.DayHigh = getDecimal(this.DayHigh);
                update.DayLow = getDecimal(this.DayLow);
            }
            catch (Exception myE)
            {
                m_Log.Error("AsUpdate", myE);
            }

            return update;
        }

        /// <summary>
        /// Update the publisher with some price update - not all publishers
        /// will action this.
        /// </summary>
        /// <param name="pxUpdate"></param>
        public void OnUpdate(string mnemonic, KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            ApplyUpdate(pxUpdate);
        }

        string KaiTrade.Interfaces.IPublisher.Open(string myData)
        {
            // we will let the GPS use the string passed in as a default name and Key
            m_Name = myData;

            try
            {
                m_Key = m_Name;

                m_Log.Info("Subject:" + m_Name + "is opening");
                //m_Status = KaiTrade.Interfaces.Status.open;
                updateStatus(KaiTrade.Interfaces.Status.open);
                doImage();
            }
            catch (Exception myE)
            {
                m_Log.Error("Open", myE);
            }
            return myData;
        }

        private void addSubscriber(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            try
            {
                if (mySubscriber == null)
                {
                    return;
                }
                foreach (KaiTrade.Interfaces.ISubscriber mySub in m_Subscribers)
                {
                    if (mySub == mySubscriber)
                    {
                        // Do not add duplicate susbcribers
                        return;
                    }
                }
                m_Subscribers.Add(mySubscriber);
            }
            catch (Exception myE)
            {
                m_Log.Error("addSubscriber", myE);
            }
        }

        public void Subscribe(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            try
            {
                //m_Subscribers.Add(mySubscriber);
                addSubscriber(mySubscriber);

                //myObserver.OnStatusChange(m_StatusInfo); //PTR

                // send an initial image
                System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList = new List<KaiTrade.Interfaces.IField>();
                addIntrinsicFields(ref myFieldList);
                mySubscriber.OnImage(this, myFieldList);
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.Subscribe", myE);
            }
        }

        public string Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }

        public string TopicID()
        {
            return m_Name;
        }

        public string TopicID(string myData)
        {
            return m_Name;
        }

        public void UnSubscribe(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            try
            {
                m_Subscribers.Remove(mySubscriber);
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.UnSubscribe:", myE);
            }
        }

        #endregion

        #region L1PX Members

        decimal? IL1PX.BidPrice
        {
            get
            {
                return decimal.Parse(this.BidPrice);
            }
        }

        decimal? IL1PX.BidSize
        {
            get
            {
                return decimal.Parse(this.BidSize);
            }
        }

        decimal? IL1PX.OfferPrice
        {
            get
            {
                return decimal.Parse(this.OfferPrice);
            }
        }

        decimal? IL1PX.OfferSize
        {
            get
            {
                return decimal.Parse(this.OfferSize);
            }
        }

        decimal? IL1PX.TradePrice
        {
            get
            {
                return decimal.Parse(this.TradePrice);
            }
        }

        decimal? IL1PX.TradeVolume
        {
            get
            {
                return decimal.Parse(this.TradeSize);
            }
        }

        decimal? IL1PX.BidPriceDelta
        {
            get
            {
                return 0;
            }
        }

        decimal? IL1PX.BidSizeDelta
        {
            get
            {
                return 0;
            }
        }

        decimal? IL1PX.OfferPriceDelta
        {
            get
            {
                return 0;
            }
        }

        decimal? IL1PX.OfferSizeDelta
        {
            get
            {
                return 0;
            }
        }

        decimal? IL1PX.TradePriceDelta
        {
            get
            {
                return 0;
            }
        }

        decimal? IL1PX.TradeVolumeDelta
        {
            get
            {
                return 0;
            }
        }

        decimal? IL1PX.DayHigh
        {
            get
            {
                return decimal.Parse(this.DayHigh);
            }
        }
        decimal? IL1PX.DayLow
        {
            get
            {
                return decimal.Parse(this.DayLow);
            }
        }
        decimal? IL1PX.Open
        {
            get
            {
                return decimal.Parse(this.Open);
            }
        }

        int IL1PX.TickDirection
        {
            get { return 0; }
            //set;
        }

        /// <summary>
        /// Return a delimited string of current values
        /// time stamp, mnemonic bidsz, bidpx,offerpx, offersz, tradesz, trade px
        /// </summary>
        /// <param name="myDelimiter">delimiter used between pxvalues</param>
        /// <returns></returns>
        public string AsCSVString(string myDelimiter)
        {
            string myDelimitedString = "";

            myDelimitedString += this.APIUpdateTime.Ticks.ToString() + myDelimiter;
            myDelimitedString += this.m_Product.Mnemonic + myDelimiter;
            myDelimitedString += this.BidSize.ToString() + myDelimiter;
            myDelimitedString += this.BidPrice.ToString() + myDelimiter;
            myDelimitedString += this.OfferPrice.ToString() + myDelimiter;
            myDelimitedString += this.OfferSize.ToString() + myDelimiter;
            myDelimitedString += this.TradePrice.ToString() + myDelimiter;
            myDelimitedString += this.TradeSize.ToString() ;

            return myDelimitedString;
        }

        #endregion
    }
}
