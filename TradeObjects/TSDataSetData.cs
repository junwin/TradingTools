/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
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
using System.Runtime.Serialization;
using System.ServiceModel;

namespace K2DataObjects
{
    /// <summary>
    /// Represents a set of Time series data - can publish results from TSBar, Custom studies and Expressions
    /// conditions area also published as triggers
    /// </summary>
    [DataContract]
    public class TSDataSetData :  KaiTrade.Interfaces.ITSSet
    {
        private string m_ID;

        /// <summary>
        /// client request id
        /// </summary>
        private string m_RequestID="";

        private string m_ExternalIdentity="";
        private object m_ExternalRef = null;
        private object m_TSDATag = null;
        private string m_Mnemonic = "";
        private string m_Alias = "";
        private string m_StrategyName = "";
        private bool m_AutoCreateStrategy = false;
        /// <summary>
        /// if this is true all updates are pblished not just bar additions
        /// </summary>
        private bool m_PublishUpdates = false;
        private bool m_Updated = false;
        /// <summary>
        /// Data items/bars
        /// </summary>
        private List<KaiTrade.Interfaces.ITSItem> m_TSItems;

        /// <summary>
        /// Copy of all the items used in test mode
        /// </summary>
        private List<KaiTrade.Interfaces.ITSItem> m_TSTestModeItems;

        private KaiTrade.Interfaces.TSUpdate m_TSUpdate;

        private KaiTrade.Interfaces.TSAdded m_TSAdded;

        private KaiTrade.Interfaces.TSStatus m_TSStatus;

        private KaiTrade.Interfaces.TSPeriod m_Period = KaiTrade.Interfaces.TSPeriod.IntraDay;
        private KaiTrade.Interfaces.TSRangeType m_RangeType = KaiTrade.Interfaces.TSRangeType.IntInt;
        private DateTime m_DateTimeStart = DateTime.Now;
        private DateTime m_DateTimeEnd = DateTime.Now;
        private int m_IntStart= 0;
        private int m_IntEnd = 0;
        private bool m_IncludeEnd = true;
        private int m_IntraDayInterval = 0;
        private bool m_UpdatesEnabled = false;
        private string m_Text = "";
        private string m_ConditionName="";
        private string m_StudyName="";

        //NOT USED? private bool m_IsInUpdate = false;
        private bool m_Changed = false;
        private bool m_Added = false;
        private bool m_ReportAll = true;
        private DateTime m_TimeStamp;
        private bool m_IncludeFlatTicks = false;
        private KaiTrade.Interfaces.TSVolumeType m_VolumeType = KaiTrade.Interfaces.TSVolumeType.Ticks;
        private long m_VolumeLevel = 10;

        /// <summary>
        /// Price agregator used to handle raw data
        /// </summary>
        //private KaiTrade.Interfaces.PriceAgregator m_PriceAgregator = null;

        //private List<KaiTrade.Interfaces.PatternMatcher> m_PatternMatchers = null;

        /// <summary>
        /// Determines if the current bar or the last completed period is used
        /// </summary>
        private bool m_UseCurrentBar = true;

        /// <summary>
        /// used to hold a set of expressions to be evaluated in the driver or data server
        /// </summary>
        //private List<KaiTrade.Interfaces.TSExpression> m_Expressions;

        //private string m_Name = "";

        /// <summary>
        /// type of time series to request
        /// </summary>
        private KaiTrade.Interfaces.TSType m_TSType;

        /// <summary>
        /// Session flags used when requesting TS Data
        /// </summary>
        private KaiTrade.Interfaces.TSSessionFlags m_TSSessionFlags;

        /// <summary>
        /// USed by CQG
        /// </summary>
        private long m_TSSessionFilter = 0;

        /// <summary>
        /// Trigger publisher - used for conditions used as triggers
        /// </summary>
        private KaiTrade.Interfaces.IPublisher m_TriggerPublisher;

        /// <summary>
        /// thread used in the test mode process
        /// </summary>
        private System.Threading.Thread m_TestModeThread;

        /// <summary>
        /// wait time in ms between each update in test mode
        /// </summary>
        private int m_TestModeWaitTime;

        /// <summary>
        /// number of bars in the test mode set of items
        /// </summary>
        private int m_TestModeBars;

        private bool m_InTestMode = false;

        /// <summary>
        /// user defined values -
        /// </summary>
        private double[] m_UDValues;

        private string[] m_UDNames;

        KaiTrade.Interfaces.IPXUpdate m_LastUpdate = null;

        private decimal? m_SetOpen;
        private decimal? m_SetClose;
        private decimal? m_SetHigh;
        private decimal? m_SetLow;

        private string m_Name = "";

        private List<KaiTrade.Interfaces.IParameter> m_Parameters;

        private KaiTrade.Interfaces.TSBarCalculationMode m_CalculationMode = KaiTrade.Interfaces.TSBarCalculationMode.endBarPeriodic;

        private int m_CalculationPeriod = 5;

        // Create a logger for use in this class
        public log4net.ILog m_Log;

        /// <summary>
        /// Status of the TSSet
        /// </summary>
        private KaiTrade.Interfaces.Status m_Status = KaiTrade.Interfaces.Status.loaded;

        public TSDataSetData()
        {
            m_ID = System.Guid.NewGuid().ToString();
            m_Log = log4net.LogManager.GetLogger("Kaitrade");
            m_Name = "TSDATASET";
            m_TimeStamp = DateTime.Now;
            //m_Expressions = new List<KaiTrade.Interfaces.TSExpression>();
            m_TSItems = new List<KaiTrade.Interfaces.ITSItem>();
            m_TSSessionFlags = KaiTrade.Interfaces.TSSessionFlags.Undefined;

            m_UDValues = new double[10];
            m_UDNames = new string[10];

            m_TSTestModeItems = new List<KaiTrade.Interfaces.ITSItem>();
            //m_PatternMatchers = new List<KaiTrade.Interfaces.PatternMatcher>();

            m_SetLow = decimal.MinValue;
            m_SetHigh = decimal.MaxValue;
            m_Parameters = new List<KaiTrade.Interfaces.IParameter>();
        }

        /// <summary>
        /// You need to set the name id you want to publish
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        [DataMember]
        public string RequestID
        {
            get
            {
                return m_RequestID;
            }
            set
            {
                m_RequestID = value;
            }
        }

        /// <summary>
        /// Called on Set updates
        /// </summary>
        public KaiTrade.Interfaces.TSUpdate TSUpdate
        {
            get { return m_TSUpdate; }
            set { m_TSUpdate = value; }
        }

        /// <summary>
        /// Called when an item(bar) is added
        /// </summary>
        public KaiTrade.Interfaces.TSAdded TSAdded
        {
            get { return m_TSAdded; }
            set { m_TSAdded = value; }
        }

        /// <summary>
        /// Called when Status changes
        /// </summary>
        public KaiTrade.Interfaces.TSStatus TSStatus
        {
            get { return m_TSStatus; }
            set { m_TSStatus = value; }
        }

        /// <summary>
        /// ID of the strategy that this set is related to (if any)
        /// </summary>
        [DataMember]
        public string StrategyName
        {
            get { return m_StrategyName; }
            set { m_StrategyName = value; }
        }

        public bool AutoCreateStrategy
        {
            get { return m_AutoCreateStrategy; }
            set { m_AutoCreateStrategy = value; }
        }

       

        /// <summary>
        /// Alias name will be used to publish - in particular for CQG Expressions
        /// </summary>
        [DataMember]
        public string Alias
        {
            get
            {
                return m_Alias;
            }
            set
            {
                m_Alias = value;
            }
        }

        /// <summary>
        /// Get/Set the status of a TSSet stricktly speaking this
        /// is only set by the provider of information e.g. CQG
        /// </summary>
        [DataMember]
        public KaiTrade.Interfaces.Status Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Publisher used to publising results
        /// </summary>
        public KaiTrade.Interfaces.IPublisher TriggerPublisher
        {
            get
            {
                return m_TriggerPublisher;
            }
            set
            {
                m_TriggerPublisher = value;
            }
        }

        public List<KaiTrade.Interfaces.IParameter> Parameters
        {
            get { return m_Parameters; }
            set { m_Parameters = value; }
        }

        /// <summary>
        /// get/set publish updates - if set to true will publish all updates as well as added bars
        /// </summary>
        [DataMember]
        public  bool PublishUpdates
        {
            get { return m_PublishUpdates; }
            set { m_PublishUpdates = value; }
        }

        public double GetUDCurveValue(int myIndex)
        {
            return m_UDValues[myIndex];
        }
        public void SetUDCurveValue(int myIndex, double myValue)
        {
            m_UDValues[myIndex] = myValue;
        }

        public string GetUDCurveName(int myIndex)
        {
            return m_UDNames[myIndex];
        }
        public void SetUDCurveName(int myIndex, string myValue)
        {
            m_UDNames[myIndex] = myValue;
        }

        /// <summary>
        /// Reset the User  defined values to 0
        /// </summary>
        public void ResetUDValues()
        {
            for (int i = 0; i < m_UDValues.Length; i++)
            {
                m_UDValues[i] = 0.0;
            }
        }

        /// <summary>
        /// Reset all data - will delete all the sets data
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Get the calc/algs needed to process the TSSet expressions - use for
        /// KaiTrade internal expression processing
        /// </summary>
        public void ResolveExpressions()
        {
        }

        /// <summary>
        /// Cause all expressions to be evaluated on the current set of data
        /// used by the proccess updating the raw data in the set
        /// </summary>
        public void EvaluateExpressions()
        {
        }

        #region TSSet Members
        [DataMember]
        public string Identity
        {
            get { return m_ID; }
        }

        /// <summary>
        /// Get/Set the type of data requested
        /// </summary>
        [DataMember]
        public KaiTrade.Interfaces.TSType TSType
        {
            get
            {
                return m_TSType;
            }
            set
            {
                m_TSType = value;
            }
        }

        /// <summary>
        /// get/set the session flags
        /// </summary>
        [DataMember]
        public KaiTrade.Interfaces.TSSessionFlags TSSessionFlags
        {
            get { return m_TSSessionFlags; }
            set { m_TSSessionFlags = value; }
        }

        /// <summary>
        /// CQG Specific - see CQG for doc
        /// </summary>
        [DataMember]
        public long TSSessionFilter
        {
            get { return m_TSSessionFilter; }
            set { m_TSSessionFilter = value; }
        }

        /// <summary>
        /// Identifier provided by external system if any
        /// </summary>
        [DataMember]
        public string ExternalID
        {
            get { return m_ExternalIdentity; }
            set { m_ExternalIdentity = value; }
        }

        /// <summary>
        /// Reference of an external object e.g. from the providers API - should be only used by the system
        /// </summary>
        public object ExternalRef
        {
            get { return m_ExternalRef; }
            set { m_ExternalRef = value; }
        }

        /// <summary>
        /// user defined tag
        /// </summary>
        [DataMember]
        public object Tag
        {
            get { return m_TSDATag; }
            set { m_TSDATag = value; }
        }

        /// <summary>
        /// Set the high and low based on the item passed in
        /// </summary>
        /// <param name="myTSItem"></param>
        private void setHiLoClose(KaiTrade.Interfaces.ITSItem myTSItem)
        {
            try
            {
                if ((decimal)myTSItem.High > this.SetHigh)
                {
                    this.SetHigh = (decimal) myTSItem.High;
                }

                if ((decimal)myTSItem.Low < this.SetLow)
                {
                    this.SetLow = (decimal)myTSItem.Low;
                }

                this.SetClose = (decimal) myTSItem.Close;
            }
            catch (Exception)
            {
            }
        }

        public void AddItem(KaiTrade.Interfaces.ITSItem myTSItem)
        {
            try
            {
                myTSItem.Mnemonic = this.Mnemonic;
                m_TSItems.Add(myTSItem);
                if (m_TSItems.Count == 1)
                {
                    this.SetOpen = (decimal)myTSItem.Open;
                }

                setHiLoClose( myTSItem);
            }
            catch (Exception)
            {
            }
        }

        public void ReplaceItem(KaiTrade.Interfaces.ITSItem myTSItem, int myIndex)
        {
            try
            {
                if (myIndex < m_TSItems.Count)
                {
                    m_TSItems[myIndex] = myTSItem;
                }
            }
            catch (Exception)
            {
            }
        }

        public KaiTrade.Interfaces.ITSItem GetItem(int myIndex)
        {
            try
            {
                if (myIndex < m_TSItems.Count)
                {
                    return m_TSItems[myIndex];
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        [DataMember]
        public KaiTrade.Interfaces.TSPeriod Period
        {
            get
            {
                return m_Period;
            }
            set
            {
                m_Period = value;
            }
        }

        [DataMember]
        public KaiTrade.Interfaces.TSRangeType RangeType
        {
            get { return m_RangeType; }
            set { m_RangeType = value; }
        }

        [DataMember]
        public DateTime DateTimeEnd
        {
            get
            {
                return m_DateTimeEnd;
            }
            set
            {
                m_DateTimeEnd = value;
            }
        }

        [DataMember]
        public DateTime DateTimeStart
        {
            get
            {
                return m_DateTimeStart;
            }
            set
            {
                m_DateTimeStart = value;
            }
        }

        [DataMember]
        public int IntStart
        {
            get
            {
                return m_IntStart;
            }
            set
            {
                m_IntStart = value; ;
            }
        }

        [DataMember]
        public int IntEnd
        {
            get
            {
                return m_IntEnd;
            }
            set
            {
                m_IntEnd = value; ;
            }
        }

        [DataMember]
        public bool IncludeEnd
        {
            get
            {
                return m_IncludeEnd;
            }
            set
            {
                m_IncludeEnd = value;
            }
        }

        [DataMember]
        public int IntraDayInterval
        {
            get
            {
                return m_IntraDayInterval;
            }
            set
            {
                m_IntraDayInterval = value;
            }
        }

        /// <summary>
        /// Get/Set the volume level for constant volume bars
        /// </summary>
        [DataMember]
        public long VolumeLevel
        {
            get
            {
                return m_VolumeLevel;
            }
            set
            {
                m_VolumeLevel = value;
            }
        }

        /// <summary>
        /// Get/Set include flat ticks
        /// </summary>
        [DataMember]
        public bool IncludeFlatTicks
        {
            get
            {
                return m_IncludeFlatTicks;
            }
            set
            {
                m_IncludeFlatTicks = value;
            }
        }

        /// <summary>
        /// get/set the volume type used on a constant volume query
        /// </summary>
        [DataMember]
        public KaiTrade.Interfaces.TSVolumeType VolumeType
        {
            get
            {
                return m_VolumeType;
            }
            set
            {
                m_VolumeType = value;
            }
        }

        public List<KaiTrade.Interfaces.ITSItem> Items
        {
            get { return m_TSItems; }
        }

        /// <summary>
        /// get the current item being updated - the current bar
        /// </summary>
        public KaiTrade.Interfaces.ITSItem CurrentItem
        {
            get { return m_TSItems[m_TSItems.Count - 1]; }
        }

        /// <summary>
        /// get the last complete bar - the one before the currenty updating top bar
        /// </summary>
        public KaiTrade.Interfaces.ITSItem LastCompleteItem
        {
            get
            {
                if (m_TSItems.Count >= 2)
                {
                    return m_TSItems[m_TSItems.Count - 2];
                }
                else
                {
                    return null;
                }
            }
        }

        [DataMember]
        public string ConditionName
        {
            get { return m_ConditionName; }
            set { m_ConditionName = value; }
        }

        [DataMember]
        public string StudyName
        {
            get { return m_StudyName; }
            set { m_StudyName = value; }
        }

        [DataMember]
        public string Mnemonic
        {
            get
            {
                return m_Mnemonic;
            }
            set
            {
                m_Mnemonic = value;
            }
        }

        /// <summary>
        /// Subscribe to the base mnemonic's price updates - this can be used
        /// in connection with the price agregator to build up a set of bars
        /// </summary>
        public void SubscribeBaseMnemonic()
        {
        }


        [DataMember]
        public KaiTrade.Interfaces.TSBarCalculationMode CalculationMode
        {
            get { return m_CalculationMode; }
            set { m_CalculationMode = value; }
        }


        [DataMember]
        public int CalculationPeriod
        {
            get { return m_CalculationPeriod; }
            set { m_CalculationPeriod = value; }
        }

        [DataMember]
        public bool UpdatesEnabled
        {
            get
            {
                return m_UpdatesEnabled;
            }
            set
            {
                m_UpdatesEnabled = value;
            }
        }

        [DataMember]
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        [DataMember]
        public bool Changed
        {
            get
            {
                return m_Changed;
            }
            set
            {
                m_Changed = value;
            }
        }

        public bool Added
        {
            get
            {
                return m_Added;
            }
            set
            {
                m_Added = value;
            }
        }

        /// <summary>
        /// Time stamp of an update
        /// </summary>
        [DataMember]
        public DateTime TimeStamp
        {
            get { return m_TimeStamp; }
            set { m_TimeStamp = value; }
        }

        /// <summary>
        /// Set to true if a new bar is added
        /// </summary>
        [DataMember]
        public bool ReportAll
        {
            get { return m_ReportAll; }
            set { m_ReportAll = value; }
        }

        /// <summary>
        /// Determines if the current bar will be used for updates - with CQG the current bar can
        /// have undefined data - e.g. until an intrabar update is received. If this is false then
        /// data in images will be from the last complete bar
        /// </summary>
        [DataMember]
        public bool UseCurrentBar
        {
            get { return m_UseCurrentBar; }
            set { m_UseCurrentBar = value; }
        }

        public bool Updated
        {
            get { return m_Updated; }
            set
            {
                m_Updated = value;
            }
        }

        /// <summary>
        /// Get a new empty time series item
        /// </summary>
        /// <returns></returns>
        public KaiTrade.Interfaces.ITSItem GetNewItem()
        {
            return new K2DataObjects.TSItem();
        }

        
        /// <summary>
        /// Return a string of Tab separated data good for Excel
        /// </summary>
        /// <returns></returns>
        public string ToTabSeparated()
        {
            string myRet = "";
            try
            {
                foreach (KaiTrade.Interfaces.ITSItem myItem in m_TSItems)
                {
                    myRet += myItem.ToTabSeparated();
                }
            }
            catch (Exception /*myE*/)
            {
                //m_Log.Error("ToTabSeparated", myE);
            }
            return myRet;
        }

        
        /// <summary>
        /// Switch the set into test mode - the set will replay all the
        /// data as updates 1 per period entered
        /// </summary>
        /// <param name="myPeriod">period between updates in ms</param>
        /// <param name="setSize">size of set exposed to clients - must be smaller than the original number of bars</param>
        public void StartTestMode(int mySetSize, int myWaitTime)
        {
            try
            {
                if (!m_InTestMode)
                {
                    m_InTestMode = true;
                    m_TestModeWaitTime = myWaitTime;
                    m_TestModeBars = mySetSize;

                    // start thread
                    m_TestModeThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.testModeProc));
                    m_TestModeThread.Start();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("StartTestMode", myE);
            }
        }

        private void testModeProc()
        {
            try
            {
                // if the test item store is empty copy items
                if (m_TSTestModeItems.Count == 0)
                {
                    copyTestItems();
                }

                // clear the working set
                m_TSItems.Clear();

                int myOffset = 0;
                // set up the set from the test copy - to the amount of bars they want
                //copyNItems(m_TSTestModeItems, m_TSItems, myOffset, m_TestModeBars);
                //myOffset += m_TestModeBars;

                while (myOffset < (m_TSTestModeItems.Count - m_TestModeBars))
                {
                    // set up the set from the test copy - to the amount of bars they want
                    copyNItems(m_TSTestModeItems, m_TSItems, myOffset, m_TestModeBars);
                    this.Changed = true;
                    myOffset++;
                    System.Threading.Thread.Sleep(m_TestModeWaitTime);
                }
                m_InTestMode = false;
            }
            catch (Exception myE)
            {
                m_Log.Error("testModeProc", myE);
                m_InTestMode = false;
            }
        }

        private int copyNItems(List<KaiTrade.Interfaces.ITSItem> myTestSet, List<KaiTrade.Interfaces.ITSItem> myItemSet, int myOffset, int myCount)
        {
            int myCopied = 0;
            try
            {
                if ((myOffset + myCount) <= myTestSet.Count)
                {
                    myItemSet.Clear();
                    for (int myIndex = 0; myIndex < myCount; myIndex++)
                    {
                        myTestSet[myIndex + myOffset].Index = myIndex;
                        myItemSet.Add(myTestSet[myIndex + myOffset]);
                    }
                    myCopied = myCount;
                }
                else
                {
                    int myAvailable = myTestSet.Count - myOffset;

                    int myItemSetStart = myItemSet.Count - myAvailable;
                    int myTestSetStart = myTestSet.Count - myAvailable;
                    for (int myIndex = 0; myIndex < myAvailable; myIndex++)
                    {
                        myItemSet[myIndex + myItemSetStart] = myTestSet[myIndex + myTestSetStart];
                    }
                    myCopied = myAvailable;
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("copyNfromBack", myE);
            }
            return myCopied;
        }

        /// <summary>
        /// Copy all items in to the test set
        /// </summary>
        private void copyTestItems()
        {
            try
            {
                m_TSTestModeItems.Clear();
                foreach (KaiTrade.Interfaces.ITSItem myItem in m_TSItems)
                {
                    m_TSTestModeItems.Add(myItem);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("copyTestItems", myE);
            }
        }

        #endregion

        public override string ToString()
        {
            string myRet = "";
            myRet +=this.Name + ",";
            myRet += this.Mnemonic + ",";
            myRet += this.Status.ToString();

            return myRet;

            //return string.Format("Customer = {0} ID = {1}", Customer, CustomerID);
        }

        /// <summary>
        /// Apply a simple price update to the TSSet - if a price aggregator is set then
        /// the set may add bars and call other routines
        /// </summary>
        /// <param name="update"></param>
        public void ApplyPriceUpdate(KaiTrade.Interfaces.IPXUpdate update)
        {
            try
            {
               
                m_LastUpdate = update;
            }
            catch
            {
                //m_Log.Error("OnUpdate", myE);
            }
        }

        /// <summary>
        /// Get the last update the set had
        /// </summary>
        public KaiTrade.Interfaces.IPXUpdate LastUpdate
        {
            get { return m_LastUpdate; }
        }

        public decimal? SetClose
        {
            get
            {
                return m_SetClose;
            }
            set
            {
                m_SetClose = value;
            }
        }

        public decimal? SetHigh
        {
            get
            {
                return m_SetHigh;
            }
            set
            {
                m_SetHigh = value;
            }
        }

        public decimal? SetLow
        {
            get
            {
                return m_SetLow;
            }
            set
            {
                m_SetLow = value;
            }
        }

        public decimal? SetOpen
        {
            get
            {
                return m_SetOpen;
            }
            set
            {
                m_SetOpen = value;
            }
        }
    }
}
