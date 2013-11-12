//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="KaiTrade LLC">
// Copyright (c) 2013, KaiTrade LLC.
//// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>// <author>John Unwin</author>
// <website>https://github.com/junwin/K2RTD.git</website>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;



namespace KaiTrade.Interfaces
{
    /// <summary>
    /// RTD connection state
    /// </summary>
    public enum RTDConnectionState { connecting, connected, stale, disconnected, none };

    class Constants
    {
        //public const string BUY = "BUY";
        //public const string SELL = "SELL";
    }

    /// <summary>
    /// Type of queue on Rabbit MQ or other queue
    /// </summary>
    public struct MQType
    {
        public const string SUBSCRIBE = "subscribe";
        public const string UPDATE = "update";
        public const string TRADESIGNAL = "TradeSignal";
        public const string PRICES = "Prices";
        public const string TRADE = "Trade";
        public const string DEPTHSLICE = "DepthSlice";
        public const string TSBAR = "BarData";
        public const string ORDER = "Order";
        public const string EXECREPORT = "Order";
    }


    public struct MQExchanges
    {
        public const string DEFAULT = "KTDefault";
        public const string CHARTINFO = "ChartInfo";
        public const string TRADESIGNAL = "TradeSignal";
        public const string PRICES = "TradeSignal";
        public const string ORDERROUTING = "TradeSignal";
    }

    public struct Side
    {
        public const string BUY = "BUY";
        public const string SELL = "SELL";
        public const string SHORT = "SHORT";
    }
    public struct ExecType
    {
        public const string ORDER_STATUS = "ORDER_STATUS";
        public const string REJECTED = "REJECTED ";
        public const string PARTIAL_FILL = "Partial Fill";
        public const string FILL = "Fill";
    }
    public struct OrderType
    {
        public const string LIMIT = "LIMIT";
        public const string MARKET = "MARKET";
        public const string STOP = "STOP";
        public const string STOPLIMIT = "STOPLIMIT";
        public const string KSTOP = "KSTOP";
        public const string KTRAILINGSTOP = "KTRAILINGSTOP";
        public const string KPROFIT = "KPROFIT";
        public const string KRETRACEMENT = "KRETRACEMENT";
        public const string KTARGET = "KTARGET";
        public const string KMULTITARGET = "KMULTITARGET";
        public const string KHELD = "KHELD";
        public const string MOC = "MOC";
        public const string LOC = "LOC";
        public const string KOCO = "KOCO";
        public const string TRIGGER = "TRIGGER";
        public const string KICEBERG = "KICEBERG";
        public const string TARGET = "TARGET";
        public const string TARGETLIMIT = "TARGETLIMIT";
    }

    public struct FIXOrderType
    {
        public const char LIMIT = '2';
        public const char MARKET = '1';
        public const char STOP = '3';
        public const char STOPLIMIT = '4';
        public const char KSTOP = 'z';
        public const char KTRAILINGSTOP = 's';
        public const char KTARGET = 'y';
        public const char KMULTITARGET = 't';
        public const char KRETRACEMENT = 's';
        public const char KHELD = 'x';
        public const char MOC = '5';
        public const char LOC = 'B';
        public const char KOCO = 'w';
        public const char TRIGGER ='v';
        public const char KICEBERG = 'u';
        public const char TARGET = 'q';
        public const char TARGETLIMIT = 'r';
        public const char KPROFIT = 'p';
    }

   

     public struct OrderStatus
    {
        public const string NEW = "New Order";
        public const string PARTIALLY_FILLED = "Partial Fill";
        public const string FILLED = "Filled";
        public const string DONE_FOR_DAY = "Done for Day";
        public const string CANCELED = "Cancelled";
        public const string PENDING_CANCEL = "Pending Cancel";
        public const string STOPPED = "Stopped";
        public const string REJECTED = "Rejected";
        public const string SUSPENDED = "Suspended";
        public const string PENDING_NEW = "Pending New";
        public const string CALCULATED = "Calculated";
        public const string EXPIRED = "Expired";
        public const string PENDING_REPLACE = "Pending Replace";
        public const string REPLACED = "Replaced";
    }


    public struct TimeType
    {
        public const string DAY = "DAY";
        public const string GTD = "GTD";
        public const string GTC = "GTC";
        public const string IOC = "IOC";
        public const string FOK = "FOK";
        public const string GTX = "GTX";
        public const string ATC = "ATC";
        public const string OPG = "OPG";
    }
    public struct CFICode
    {
        public const string FUTURE = "FXXXXX";
        public const string INDEXFUTURE = "FFIXXX";
        public const string PUTOPTION = "OPXXXX";
        public const string CALLOPTION = "OCXXXX";
        public const string STOCK = "ESXXXX";
        public const string FX = "MRCXXX";
        public const string CALLOPTIONFUTURE = "OCXFXS";
        public const string PUTOPTIONFUTURE = "OPXFXS";
    }

    /*
    public struct GatewayStatus
    {
        public const string CLOSED = "CLOSED";
        public const string CLOSING = "CLOSING";
        public const string OPEN = "OPEN";
        public const string OPENING = "OPENING";
        public const string NONE = "NOTKNOWN";
    }
     */

    public struct TSBarField
    {
        public const string HLC3 = "HLC3";
        public const string CLOSE = "CLOSE";
        public const string OPEN = "OPEN";
        public const string HIGH = "HIGH";
        public const string LOW = "LOW";
        public const string MID = "MID";
    }

    public struct MDConstants
    {
        public const int NumNDFields = 50;
        public const int BidOfferStart = 30;
        public const int DepthElements = 4;
        public const int DepthLevels = 5;

        // Field IDS
        public const int Status = 0;
        public const int FieldList = 1;
        public const int OFFER_EXCHANGE = 2;
        public const int BID_EXCHANGE = 3;
        public const int EXCH_TIME = 4;
        public const int TIME = 5;
        public const int MMID = 6;
        public const int DAY_HIGH = 7;
        public const int DAY_LOW = 8;
        public const int TRADE_FLAGS = 9;
        public const int TRADE_PRODEXCH = 10;
        public const int TRADE_SYMBOL = 11;
        public const int TRADE_EXCHTIME = 12;
        public const int TRADE_SIZE = 13;
        public const int TRADE_PRICE = 14;
        public const int TRADE_VOLUME = 15;
        public const int TRADE_INDICATOR = 16;
        public const int TRADE_SERIAL = 17;
        public const int TRADE_TICKIND = 18;
        public const int TRADE_TIME = 19;
        public const int TRADE_EXCH = 20;
        public const int OPEN = 21;
        public const int PREVCLOSE = 22;
        public const int SETTLEMENTPRICE = 23;
        public const int VWAPPRICE = 24;
        public const int OPENINTEREST = 25;
        public const int IMBALANCE = 26;
        public const int INDEXPRICE = 27;
        public const int EXCH_DATE = 28;
        public const int NETCHGPREVDAY = 29;

        // Bid & Offer depth

        public const int OFFER1_PRICE = 30;
        public const int OFFER1_SIZE = 31;
        public const int BID1_PRICE = 32;
        public const int BID1_SIZE = 33;
        public const int OFFER2_PRICE = 34;
        public const int OFFER2_SIZE = 35;
        public const int BID2_PRICE = 36;
        public const int BID2_SIZE = 37;
        public const int OFFER3_PRICE = 38;
        public const int OFFER3_SIZE = 39;
        public const int BID3_PRICE = 40;
        public const int BID3_SIZE = 41;
        public const int OFFER4_PRICE = 42;
        public const int OFFER4_SIZE = 43;
        public const int BID4_PRICE = 44;
        public const int BID4_SIZE = 45;
        public const int OFFER5_PRICE = 46;
        public const int OFFER5_SIZE = 47;
        public const int BID5_PRICE = 48;
        public const int BID5_SIZE = 49;

        // others
        public const int TICKDIR = 50;
        public const int QUOTEID = 51;
        public const int VALIDITYPERIOD = 52;


       
    }
    public struct OrderFields
    {
        public const string CMMQTY = "CumQty";
        public const string AVGPX = "AvgPx";
        public const string LEAVESQTY = "LeavesQty";
        public const string ORDSTATUS = "OrdStatus";
        public const string ORDERQTY = "OrderQty";

    }

    /// <summary>
    /// Defines the different types of post trade message we can handle
    /// </summary>
    public struct PostTradeMsgType
    {
        public const string TCR = "TrdCaptRpt";
        public const string TCRACK = "TrdCaptRptAck";
        public const string AR = "AllocRpt";
        public const string ARACK = "AllocRptAck";
        public const string AI = "AllocInstrctn";
        public const string AIACK = "AllocInstrctnAck";
        public const string AIALERT = "AllocInstrctnAlert";

    }

 

}
