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
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using CQG;

namespace KTACQG
{
    public class KTACQG : DriverBase.DriverBase
    {
        /// <summary>
        /// used for lock()
        /// </summary>
        private Object m_Token1 = new Object();
        private Object m_Token2 = new Object();
        private Object m_Token3 = new Object();
        private Object m_Token4 = new Object();
        private Object m_GetTSReqToken = new object();
        private Object m_BarToken1 = new Object();
        private Object m_ExpToken1 = new Object();
        private Object m_CondToken1 = new Object();
        private Object m_InstrToken1 = new Object();
        private Object m_CustomStudyToken1 = new Object();
        private Object m_TradingSystemToken1 = new Object();
        private Object m_InstrSubscribed = new Object();


        /// <summary>
        /// The config for the CQG adapter read from XML file
        /// </summary>
        private KAI.kaitns.CQGConfig m_Config;


        /// <summary>
        /// Used to keep the state of the adaptor
        /// </summary>
        private KaiTrade.Interfaces.Status m_State;


        /// <summary>
        /// CQG API instance (CQGCEL).
        /// </summary>
        private CQGCEL CQGApp;



        /// <summary>
        /// All products that have been subscribed to, accessed by the
        /// CQG instrumnet full name (and Tag on a product def)
        /// </summary>
        private System.Collections.Hashtable m_SubscribedProducts;

        /// <summary>
        /// Used to keep track of our list of subjects accessed by their key/mnemonic
        /// </summary>
        // private System.Collections.Hashtable m_SubjectRegister;

        /// <summary>
        /// Hash table keeps mapping of account descriptive name to account GWID.
        /// </summary>
        private System.Collections.Hashtable m_AccountsMap = new System.Collections.Hashtable(20);

        /// <summary>
        /// Maps ClOrderIDs to order context.
        /// </summary>
        private System.Collections.Hashtable m_XClIDOrder;

        /// <summary>
        /// Maps guid to order context
        /// </summary>
        private System.Collections.Hashtable m_GUID2CQGOrder;

        /// <summary>
        /// Time series sets used in the driver indexes by the CQG id
        /// used when handling updates and when the bars are resolved
        /// </summary>
        private Dictionary<string, KaiTrade.Interfaces.ITSSet> m_TSSets;

        /// <summary>
        /// CQG Gateway status
        /// </summary>
        private eConnectionStatus m_GWStatus;

        /// <summary>
        /// CQG Data Status
        /// </summary>
        private eConnectionStatus m_DataStatus;


        private CQGFrm m_CQGHostForm;

        /// <summary>
        /// used to regulate how often we do updates on the cqg timer
        /// </summary>
        private long m_tickCount = 0;

        /// <summary>
        /// used to track how many time diff warning have been issued
        /// </summary>
        private long m_TimeWarningCount = 0;

        Stack<KaiTrade.Interfaces.IProduct> m_DelayedProductRequests;

        /// <summary>
        /// Time in ticks of the last order routing request
        /// used in DEMO to throttle requests
        /// </summary>
        private long m_LastORRequestTicks = 0;

        /// <summary>
        /// time between each OR Request
        /// </summary>
        private long m_RequiredORIntervalTicks = 1000 * 10000;  // 1 sec

       

        private bool m_UseOEThrottle = true;

        

        public const string ENTRY_TRADE = "TSEntryTrade";


        private KaiTrade.Interfaces.IDriverStatus m_RunningState;

        /// <summary>
        /// Maps an expression to the ID of its corresponding cqg strategy definition
        /// </summary>
        private Dictionary<string, string> m_ExpressionStrategyID;

        /// <summary>
        /// Maps a name to some patternString
        /// </summary>
        private Dictionary<string, string> m_ExecutionPatterns;



        public KTACQG()
        {
            // initialize member data

            // set our ID
            m_ID = "KTACQG";

            // this is the name used in the publisher
            Name = "KTACQG";
            driverLog.Info("KTACQG Created");

            m_Config = new KAI.kaitns.CQGConfig();


            m_SubscribedProducts = new System.Collections.Hashtable();
            //m_SubjectRegister = new System.Collections.Hashtable();

            //m_ClIDOrder = new System.Collections.Hashtable();
            m_GUID2CQGOrder = new System.Collections.Hashtable();
            m_TSSets = new Dictionary<string, KaiTrade.Interfaces.ITSSet>();
            m_DelayedProductRequests = new Stack<KaiTrade.Interfaces.IProduct>();
            m_ExpressionStrategyID = new Dictionary<string, string>();
            m_ExecutionPatterns = new Dictionary<string, string>();
            // set status to down
            setGWStatus(eConnectionStatus.csConnectionDown);


            m_DataStatus = eConnectionStatus.csConnectionDown;

            m_RunningState = new KaiTrade.TradeObjects.DriverStatus();

           
        }

        public bool UseOEThrottle
        {
            get { return m_UseOEThrottle; }
            set { m_UseOEThrottle = value; }
        }

        public long RequiredORIntervalTicks
        {
            get { return m_RequiredORIntervalTicks; }
            set { m_RequiredORIntervalTicks = value; }
        }

        /// <summary>
        /// Set any default image fields called by the base class
        /// </summary>
        protected override void resetDefaultFields()
        {
            try
            {
            }
            catch (Exception myE)
            {
                log.Error("resetDefaultFields", myE);
            }
        }

        /// <summary>
        /// Start the CQG Adapter
        /// </summary>
        /// <param name="myState"></param>
        protected override void DoStart(string myState)
        {
            try
            {
                driverLog.Info("KTACQG doStart");

                // open the CQG Host form
                m_CQGHostForm = new CQGFrm();
                m_CQGHostForm.Adapter = this;
                m_CQGHostForm.Show();
                if (base.m_State.IsValidHideDriverUI)
                {
                    if (base.m_State.HideDriverUI)
                    {
                        m_CQGHostForm.Hide();
                    }
                }
                // Read the configuration data
                m_Config.FromXmlFile(m_ConfigPath + "CQGConfig.xml");


                // process config file
                processConfig(m_Config);

                // do any property processing
                try
                {
                    string orTicks = Facade.GetUserProfileProperty("*", m_ID + "::" + "RequiredORIntervalTicks");
                    string useOEThrottle = Facade.GetUserProfileProperty("*", m_ID + "::" + "UseOEThrottle");

                    if (useOEThrottle.ToLower() == "true")
                    {
                        UseOEThrottle = true;
                    }
                    else
                    {
                        UseOEThrottle = false;
                    }

                    if (long.Parse(orTicks) >= 0)
                    {
                        RequiredORIntervalTicks = long.Parse(orTicks);
                    }
                }
                catch (Exception myE)
                {
                    driverLog.Error("property processing", myE);
                }


                m_tickCount = 0;
                m_TimeWarningCount = 0;

                //m_CQGHostForm.Hide();

                // Start connection to CQG
                InitializeCQGCEL();


                // Report that we are open - this will show in the adapter view
                // in the dashboard
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "KTCQG adapter is open");
                driverLog.Info("KTCQG adapter is open");
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "KTCQG is not working");
            }
        }

        /// <summary>
        /// Display or Hide any UI the driver has
        /// </summary>
        /// <param name="uiVisible">true => show UI, False => hide UI</param>
        public override void ShowUI(bool uiVisible)
        {
            try
            {
                if (uiVisible)
                {
                    m_CQGHostForm.Show();
                }
                else
                {
                    m_CQGHostForm.Hide();
                }
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Disconnect a TSSet  
        /// </summary>
        /// <param name="myTSSet"></param>
        public override void DisconnectTSData(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            try
            {
                driverLog.Info("DisconnectTSData:" + myTSSet.ToString());
                if (myTSSet.ExternalRef != null)
                {
                    switch (myTSSet.TSType)
                    {
                        case KaiTrade.Interfaces.TSType.BarData:
                            m_CQGHostForm.CQGApp.RemoveTimedBars(myTSSet.ExternalRef as CQGTimedBars);
                            break;
                        case KaiTrade.Interfaces.TSType.Condition:
                            m_CQGHostForm.CQGApp.RemoveCondition(myTSSet.ExternalRef as CQGCondition);
                            break;
                        case KaiTrade.Interfaces.TSType.StudyData:
                            m_CQGHostForm.CQGApp.RemoveCustomStudy(myTSSet.ExternalRef as CQGCustomStudy);
                            break;
                        case KaiTrade.Interfaces.TSType.Expression:
                            m_CQGHostForm.CQGApp.RemoveExpression(myTSSet.ExternalRef as CQGExpression);
                            break;
                        case KaiTrade.Interfaces.TSType.TradeSystem:
                            m_CQGHostForm.CQGApp.RemoveTradingSystem(myTSSet.ExternalRef as CQGTradingSystem);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    driverLog.Warn("Cannot disconnect set since no external ref provided");
                }
                driverLog.Info("DisconnectTSData:Exit");
            }
            catch (Exception myE)
            {
                log.Error("DisconnectTSData", myE);
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "DoGetTSData" + myE.Message);
            }
        }


        /// <summary>
        /// Will request any trade systems that the driver supports - note that this
        /// is asyncronous the driver will add any trading systems using the Facade - see AddReplaceTradeSystem(
        /// see callback CQGApp_TradingSystemDefinitionsResolved
        /// </summary>
        public override void RequestTradeSystems()
        {
            try
            {
                m_CQGHostForm.CQGApp.RequestTradingSystemDefinitions();
                
            }
            catch (Exception myE)
            {
                log.Error("RequestTradeSystems", myE);
            }
        }

        /// <summary>
        /// Request any conditions that the driver supports- note that this
        /// is asyncronous the driver will add any conditions using the Facade
        /// </summary>
        public override void RequestConditions()
        {
            try
            {
                m_CQGHostForm.CQGApp.RequestConditionDefinitions();
            }
            catch (Exception myE)
            {
                log.Error("RequestConditions", myE);
            }
        }


        public override void RequestTSData(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            lock (m_GetTSReqToken)
            {
                CQGTimedBars myTimedBars;
                CQGTimedBarsRequest myReq;
                try
                {
                    switch (myTSSet.TSType)
                    {
                        case KaiTrade.Interfaces.TSType.BarData:
                            getTSBarData(ref myTSSet);
                            break;
                        case KaiTrade.Interfaces.TSType.ConstantBars:
                            getTSConstantBarData(ref myTSSet);
                            break;
                        case KaiTrade.Interfaces.TSType.Condition:
                            getTSConditionData(ref myTSSet);
                            break;
                        case KaiTrade.Interfaces.TSType.StudyData:
                            getTSStudyData(ref myTSSet);
                            break;
                        case KaiTrade.Interfaces.TSType.Expression:
                            getTSExpressionData(ref myTSSet);
                            break;
                        case KaiTrade.Interfaces.TSType.TradeSystem:
                            this.getTradingSystem(ref myTSSet);
                            break;

                        default:
                            driverLog.Error("Unknown TS Request type:" + myTSSet.TSType.ToString());
                            break;
                    }
                }
                catch (Exception myE)
                {
                    log.Error("RequestTSData", myE);
                    this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "DoGetTSData" + myE.Message);
                }
            }
        }


        /// <summary>
        /// Get time series data from CQG
        /// </summary>
        /// <param name="myState"></param>
        private void getTSConstantBarData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGConstantVolumeBars myBars;
            CQGConstantVolumeBarsRequest myReq;

            try
            {
                driverLog.Info("getTSConstantBarData:" + myTSSet.ToString());
                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateConstantVolumeBarsRequest();


                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }
                myReq.Symbol = instrument.FullName;

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;



                //constant volume stuff
                myReq.VolumeLevel = (int)myTSSet.VolumeLevel;
                myReq.IncludeFlatTicks = myTSSet.IncludeFlatTicks;
                if (myTSSet.VolumeType == KaiTrade.Interfaces.TSVolumeType.Ticks)
                {
                    myReq.VolumeType = eCvbVolumeType.cvbvtTicks;
                }
                else if (myTSSet.VolumeType == KaiTrade.Interfaces.TSVolumeType.Actual)
                {
                    myReq.VolumeType = eCvbVolumeType.cvbvtActual;
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;

                myReq.SessionsFilter = 0;
                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.Undefined:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }

                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                // depending of the type of request


                myBars = m_CQGHostForm.CQGApp.RequestConstantVolumeBars(myReq);

                myTSSet.ExternalID = myBars.Id;
                myTSSet.ExternalRef = myBars;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSConstantBarData completed cqgid:" + myBars.Id.ToString() + ":" + myBars.Status.ToString());
            }
            catch (Exception myE)
            {
                log.Error("getTSConstantBarData", myE);
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSConstantBarData" + myE.Message);

                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }

        private bool isIntraDay(out int interval, KaiTrade.Interfaces.TSPeriod period)
        {
            bool isIntraDay = false;
            interval = 0;
            try
            {
            
            switch (period)
            {
                case KaiTrade.Interfaces.TSPeriod.minute:
                    interval = 1;
                    isIntraDay = true;
                    break;
                case KaiTrade.Interfaces.TSPeriod.five_minute:
                    interval = 5;
                    isIntraDay = true;
                    break;
                case KaiTrade.Interfaces.TSPeriod.two_minute:
                    interval = 2;
                    isIntraDay = true;
                    break;
                case KaiTrade.Interfaces.TSPeriod.three_minute:
                    interval = 3;
                    isIntraDay = true;
                    break;

                case KaiTrade.Interfaces.TSPeriod.fifteen_minute:
                    interval = 15;
                    isIntraDay = true;
                    break;
                case KaiTrade.Interfaces.TSPeriod.thirty_minute:
                    interval = 30;
                    isIntraDay = true;
                    break;
                default:
                    break;
            }
            }
            catch (Exception myE)
            {
            }
            return isIntraDay;
        }

        private void getTSBarData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGTimedBars myTimedBars;
            CQGTimedBarsRequest myReq;
            try
            {
                driverLog.Info("getTSBarData:" + myTSSet.ToString());
                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateTimedBarsRequest();

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }
                myReq.Symbol = instrument.FullName;

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;
                int interval;
                if (isIntraDay(out  interval, myTSSet.Period))
                {
                    myTSSet.IntraDayInterval = interval;

                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            break;
                    }
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;

                myReq.SessionsFilter = 0;
                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.Undefined:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }

                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                // depending of the type of request


                myTimedBars = m_CQGHostForm.CQGApp.RequestTimedBars(myReq);

                myTSSet.ExternalID = myTimedBars.Id;
                myTSSet.ExternalRef = myTimedBars;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSBarData completed cqgid:" + myTimedBars.Id.ToString() + ":" + myTimedBars.Status.ToString());
            }
            catch (Exception myE)
            {
                log.Error("getTSBarData", myE);
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSBarData" + myE.Message);

                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }



        /// <summary>
        /// Get time series data from CQG custom study
        /// </summary>
        /// <param name="myState"></param>
        private void getTSStudyData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGCustomStudy myCustomStudy;
            CQGCustomStudyRequest myReq;
            try
            {
                driverLog.Info("getTSStudyData:" + myTSSet.ToString());

                myTSSet.Status = KaiTrade.Interfaces.Status.opening;

                myReq = m_CQGHostForm.CQGApp.CreateCustomStudyRequest(myTSSet.ConditionName);

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }

                myReq.BaseExpression = instrument.FullName;

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;


                if (myTSSet.Period == KaiTrade.Interfaces.TSPeriod.IntraDay)
                {
                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            break;
                    }
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;


                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }

                // depending of the type of request
                myCustomStudy = m_CQGHostForm.CQGApp.RequestCustomStudy(myReq);


                myTSSet.ExternalID = myCustomStudy.Id;
                myTSSet.ExternalRef = myCustomStudy;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSStudyData completed cqgid" + myCustomStudy.Id.ToString() + ":" + myCustomStudy.Status.ToString());
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSStudyData" + myE.Message);
                log.Error("getTSStudyData", myE);
                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }


        private void getTSConditionData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGCondition myCondition;
            CQGConditionRequest myReq;

            try
            {
                driverLog.Info("getTSConditionData:" + myTSSet.ToString());

                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateConditionRequest(myTSSet.ConditionName);

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);

                // IF they have given an expression then use that instead of
                // the raw menmonic
                if (myTSSet.Expressions.Count > 0)
                {
                    KaiTrade.Interfaces.ITSExpression myExpression = myTSSet.Expressions[0];
                    string myTemp = "";
                    if (instrument != null)
                    {
                        myTemp = myExpression.Expression.Replace("DJI", instrument.FullName);
                    }
                    else
                    {
                        myTemp = myExpression.Expression;
                    }
                    myReq.BaseExpression = myTemp;
                    driverLog.Info("getTSConditionData:using expression:" + myTemp);
                }
                else
                {
                    if (instrument == null)
                    {
                        instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                        if (instrument == null)
                        {
                            Exception myE = new Exception("Invalid instrument");
                            throw myE;
                        }
                    }
                    myReq.BaseExpression = instrument.FullName;
                }

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;


                if (myTSSet.Period == KaiTrade.Interfaces.TSPeriod.IntraDay)
                {
                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            break;
                    }
                }


                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;


                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;
                myReq.SessionFlags = eSessionFlag.sfUndefined;

                // depending of the type of request


                myCondition = m_CQGHostForm.CQGApp.RequestCondition(myReq);

                myTSSet.ExternalID = myCondition.Id;
                myTSSet.ExternalRef = myCondition;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);


                driverLog.Info("getTSConditionData completed cqgid:" + myCondition.Id.ToString() + ":" + myCondition.Status.ToString());
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSConditionData" + myE.Message);
                log.Error("getTSConditionData", myE);
                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }

        private void getTradingSystem(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGTradingSystem myTradingSystem;
            CQGTradingSystemRequest myReq;

            //myTSSet.UpdatesEnabled
            try
            {
                tSLog.Info("getTradingSystem:" + myTSSet.ToString());

                try
                {
                    if (tSLog.IsInfoEnabled)
                    {
                        tSLog.Info(myTSSet.ToDataBinding().ToXml());
                        tSLog.Info("CalculationMode=" + myTSSet.CalculationMode.ToString() + "CalculationPeriod=" + myTSSet.CalculationPeriod.ToString());
                    }
                }
                catch
                {
                    // dont expect to have an exception
                }

                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateTradingSystemRequest(myTSSet.ConditionName);


                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);

                // IF they have given an expression then use that instead of
                // the raw menmonic
                if (myTSSet.Expressions.Count > 0)
                {
                    KaiTrade.Interfaces.ITSExpression myExpression = myTSSet.Expressions[0];
                    string myTemp = "";
                    if (instrument != null)
                    {
                        myTemp = myExpression.Expression.Replace("DJI", instrument.FullName);
                    }
                    else
                    {
                        myTemp = myExpression.Expression;
                    }
                    myReq.BaseExpression = myTemp;
                    tSLog.Info("getTSTradeSystemData:using expression:" + myTemp);
                }
                else
                {
                    if (instrument == null)
                    {
                        instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                        if (instrument == null)
                        {
                            Exception myE = new Exception("Invalid instrument");
                            throw myE;
                        }
                    }
                    myReq.BaseExpression = instrument.FullName;
                }


                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;


                if (myTSSet.Period == KaiTrade.Interfaces.TSPeriod.IntraDay)
                {
                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            break;
                    }
                }


                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                // set calc mode for Trading system
                switch (myTSSet.CalculationMode)
                {
                    case KaiTrade.Interfaces.TSBarCalculationMode.endBarPeriodic:
                        myReq.SubscriptionLevel = eTimeSeriesSubscriptionLevel.tslEndOfBarAndPeriod;
                        myReq.RecalcPeriod = myTSSet.CalculationPeriod;
                        break;
                    case KaiTrade.Interfaces.TSBarCalculationMode.tick:
                        myReq.SubscriptionLevel = eTimeSeriesSubscriptionLevel.tslEachTick;
                        break;
                    default:
                        myReq.SubscriptionLevel = eTimeSeriesSubscriptionLevel.tslEachBar;
                        break;
                }


                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;
                myReq.SessionFlags = eSessionFlag.sfUndefined;

                // Set Any Parameters
                if (myTSSet.Parameters != null)
                {
                    setRequestParameters(myReq, myTSSet.Parameters);
                }

 
                myTradingSystem = m_CQGHostForm.CQGApp.RequestTradingSystem(myReq);
                logTSReqParameters(myReq);


                myTSSet.ExternalID = myTradingSystem.Id;
                myTSSet.ExternalRef = myTradingSystem;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);


                tSLog.Info("getTradingSystem completed cqgid:" + myTradingSystem.Id.ToString() + ":" + myTradingSystem.Status.ToString());
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTradingSystem" + myE.Message);
                tSLog.Error("getTradingSystem", myE);
                log.Error("getTradingSystem", myE);
                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }

        private void logTSReqParameters(CQGTradingSystemRequest myReq)
        {
            try
            {

                driverLog.Info("logTSParameters:enter:" + myReq.BaseExpression);
                foreach (CQGParameterDefinition p in myReq.Definition.ParameterDefinitions)
                {
                    driverLog.Info("parameters:name:" + p.Name + "def value:" + p.DefaultValue.ToString());
                    driverLog.Info("parameters:name:" + p.Name + " value:" + myReq.Parameter[p.Name].ToString());

                }
                log.Info("logTSParameters:exit:" );
                


            }
            catch (Exception myE)
            {
                log.Error("setRequestParameters", myE);
            }
        }

        private void logValidReqParmNames(CQGTradingSystemRequest myReq)
        {
            try
            {

                driverLog.Info("logValidReqParmNames");
                foreach (CQGParameterDefinition pd in myReq.Definition.ParameterDefinitions)
                {
                    string plog = string.Format("Name:{0}:Type:{1}:DefaultValue:{2}", pd.Name, pd.Type.ToString(), pd.DefaultValue);
                    driverLog.Info(plog);
                }
                
            }
            catch (Exception myE)
            {
                log.Error("logValidReqParmNames", myE);
            }
        }
        private void setRequestParameters(CQGTradingSystemRequest myReq, List<KaiTrade.Interfaces.IParameter> parms)
        {
            try
            {
                driverLog.Info("setRequestParameters:enter:"+myReq.BaseExpression);
                logValidReqParmNames(myReq);

                foreach (KaiTrade.Interfaces.IParameter p in parms)
                {
                    try
                    {
                        switch (p.ParameterType)
                        {
                            case KaiTrade.Interfaces.ATDLType.Float:
                                myReq.set_Parameter(p.ParameterName, double.Parse(p.ParameterValue));
                                break;
                            default:
                                myReq.set_Parameter(p.ParameterName, int.Parse(p.ParameterValue));
                                break;
                        }
                        driverLog.Info("setParameters:name:" + p.ParameterName+" value:"+p.ParameterValue);
                    }
                    catch (Exception myE)
                    {
                        log.Error("setRequestParameters:InvalidParameter" + p.ParameterName + ":" + p.ParameterValue, myE);
                    }
                    

                }
                driverLog.Info("setRequestParameters:exit:" + myReq.BaseExpression);
            }
            catch (Exception myE)
            {
                log.Error("setRequestParameters", myE);
            }
        }

        /// <summary>
        /// Get time series data from CQG custom study
        /// </summary>
        /// <param name="myState"></param>
        private void getTSExpressionData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGExpression myExpression;
            CQGExpressionRequest myReq;
            try
            {
                driverLog.Info("getTSExpressionData:" + myTSSet.ToString());

                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateExpressionRequest();

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }
                //myReq.AddSubExpression(myTSSet.ConditionName, myTSSet.Alias);
                foreach (KaiTrade.Interfaces.ITSExpression myTSExpression in myTSSet.Expressions)
                {
                    string myTemp = myTSExpression.Expression.Replace("DJI", instrument.FullName);
                    myReq.AddSubExpression(myTemp, myTSExpression.Alias);
                }

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;


                if (myTSSet.Period == KaiTrade.Interfaces.TSPeriod.IntraDay)
                {
                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            break;
                    }
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;

                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }


                // depending of the type of request
                myExpression = m_CQGHostForm.CQGApp.RequestExpression(myReq);


                myTSSet.ExternalID = myExpression.Id;
                myTSSet.ExternalRef = myExpression;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSExpressionData completed cqgid:" + myExpression.Id.ToString() + ":" + myExpression.Status.ToString());
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSExpressionData" + myE.Message);
                log.Error("getTSExpressionData", myE);
                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }


        /// <summary>
        /// Configures and starts CQGCEL.
        /// </summary>
        private void InitializeCQGCEL()
        {
            try
            {
                driverLog.Info("InitializeCQGCEL:enter");
                // Configure CQG API. Based on this configuration CQG API works differently.
                m_CQGHostForm.CQGApp.APIConfiguration.CollectionsThrowException = false;
                m_CQGHostForm.CQGApp.APIConfiguration.DefaultInstrumentSubscriptionLevel = eDataSubscriptionLevel.dsQuotesAndBBA;
                m_CQGHostForm.CQGApp.APIConfiguration.ReadyStatusCheck = eReadyStatusCheck.rscOff;
                m_CQGHostForm.CQGApp.APIConfiguration.TimeZoneCode = eTimeZone.tzCentral;
                //m_CQGHostForm.CQGApp.APIConfiguration.NewInstrumentChangeMode = false;

                string positionDetailOn = Facade.GetUserProfileProperty("*", "PositionDetailOn");
                if (positionDetailOn.ToLower() == "true")
                {
                    m_CQGHostForm.CQGApp.APIConfiguration.DefPositionSubscriptionLevel = ePositionSubscriptionLevel.pslSnapshotAndUpdates;
                    m_CQGHostForm.CQGApp.APIConfiguration.PositionDetailing = ePositionDetailing.pdAllTrades;
                }
                else
                {
                    m_CQGHostForm.CQGApp.APIConfiguration.DefPositionSubscriptionLevel = ePositionSubscriptionLevel.pslNoPositions;
                    m_CQGHostForm.CQGApp.APIConfiguration.PositionDetailing = ePositionDetailing.pdNoTrades;
                }
                //Specifies the default position subscription level for accounts.
                //m_CQGHostForm.CQGApp.APIConfiguration.DefPositionSubscriptionLevel = ePositionSubscriptionLevel.pslSnapshotAndUpdates;
                //m_CQGHostForm.CQGApp.APIConfiguration.DefPositionSubscriptionLevel = ePositionSubscriptionLevel.pslNoPositions;
                //m_CEL.APIConfiguration.FireEventOnChangedPrices = true;


                //m_CEL.APIConfiguration.UseOrderSide = true;

                //This property specifies the position detailing level of accounts, i.e. the amount of information on trades that user wants to receive.
                //m_CQGHostForm.CQGApp.APIConfiguration.PositionDetailing = ePositionDetailing.pdAllTrades;
                //m_CQGHostForm.CQGApp.APIConfiguration.PositionDetailing = ePositionDetailing.pdNoTrades;


                // Handle following events

                // Status
                m_CQGHostForm.CQGApp.DataConnectionStatusChanged += new _ICQGCELEvents_DataConnectionStatusChangedEventHandler(cel_DataConnectionStatusChanged);
                m_CQGHostForm.CQGApp.GWConnectionStatusChanged += new _ICQGCELEvents_GWConnectionStatusChangedEventHandler(cel_GWConnectionStatusChanged);
                m_CQGHostForm.CQGApp.LineTimeChanged += new _ICQGCELEvents_LineTimeChangedEventHandler(cel_LineTimeChanged);
                m_CQGHostForm.CQGApp.DataError += new _ICQGCELEvents_DataErrorEventHandler(cel_DataError);
                
                // Price and Product
                m_CQGHostForm.CQGApp.InstrumentSubscribed += new _ICQGCELEvents_InstrumentSubscribedEventHandler(cel_InstrumentSubscribed);
                m_CQGHostForm.CQGApp.IncorrectSymbol += new _ICQGCELEvents_IncorrectSymbolEventHandler(cel_IncorrectSymbol);
                m_CQGHostForm.CQGApp.InstrumentChanged += new _ICQGCELEvents_InstrumentChangedEventHandler(cel_InstrumentChanged);
                m_CQGHostForm.CQGApp.CommodityInstrumentsResolved += new CQG._ICQGCELEvents_CommodityInstrumentsResolvedEventHandler(CEL_CommodityInstrumentsResolved);
                m_CQGHostForm.CQGApp.TradableCommoditiesResolved += new CQG._ICQGCELEvents_TradableCommoditiesResolvedEventHandler(CEL_TradableCommoditiesResolved);
                m_CQGHostForm.CQGApp.CommodityInstrumentsResolved += new _ICQGCELEvents_CommodityInstrumentsResolvedEventHandler(CQGApp_CommodityInstrumentsResolved);
                m_CQGHostForm.CQGApp.InstrumentDOMChanged += new _ICQGCELEvents_InstrumentDOMChangedEventHandler(CQGApp_InstrumentDOMChanged);
                m_CQGHostForm.CQGApp.TradableCommoditiesResolved += new _ICQGCELEvents_TradableCommoditiesResolvedEventHandler(CQGApp_TradableCommoditiesResolved);

                //Account, postition and order
                m_CQGHostForm.CQGApp.AccountChanged += new _ICQGCELEvents_AccountChangedEventHandler(cel_AccountChanged);
                m_CQGHostForm.CQGApp.OnQueryProgress += new _ICQGCELEvents_OnQueryProgressEventHandler(CQGApp_OnQueryProgress);                
                m_CQGHostForm.CQGApp.OrderChanged += new _ICQGCELEvents_OrderChangedEventHandler(cel_OrderChanged);
                
                // TS Data
                m_CQGHostForm.CQGApp.TimedBarsResolved += new CQG._ICQGCELEvents_TimedBarsResolvedEventHandler(CEL_TimedBarsResolved);
                m_CQGHostForm.CQGApp.TimedBarsAdded += new CQG._ICQGCELEvents_TimedBarsAddedEventHandler(CEL_TimedBarsAdded);
                m_CQGHostForm.CQGApp.TimedBarsUpdated += new CQG._ICQGCELEvents_TimedBarsUpdatedEventHandler(CEL_TimedBarsUpdated);

                // Constant Bar
                m_CQGHostForm.CQGApp.ConstantVolumeBarsResolved += new CQG._ICQGCELEvents_ConstantVolumeBarsResolvedEventHandler(CEL_ConstantVolumeBarsResolved);
                m_CQGHostForm.CQGApp.ConstantVolumeBarsAdded += new CQG._ICQGCELEvents_ConstantVolumeBarsAddedEventHandler(CEL_ConstantVolumeBarsAdded);
                m_CQGHostForm.CQGApp.ConstantVolumeBarsUpdated += new CQG._ICQGCELEvents_ConstantVolumeBarsUpdatedEventHandler(CEL_ConstantVolumeBarsUpdated);


                // Conditions
                //m_CQGHostForm.CQGApp.ConditionAdded += new _ICQGCELEvents_ConditionAddedEventHandler(CQGApp_ConditionAdded);
                m_CQGHostForm.CQGApp.ConditionDefinitionsResolved += new _ICQGCELEvents_ConditionDefinitionsResolvedEventHandler(CEL_ConditionDefinitionsResolved);
                m_CQGHostForm.CQGApp.ConditionResolved += new _ICQGCELEvents_ConditionResolvedEventHandler(CEL_ConditionResolved);
                m_CQGHostForm.CQGApp.ConditionUpdated += new _ICQGCELEvents_ConditionUpdatedEventHandler(CEL_ConditionUpdated);
                m_CQGHostForm.CQGApp.ConditionAdded += new _ICQGCELEvents_ConditionAddedEventHandler(CEL_ConditionAdded);

                // Custom study
                m_CQGHostForm.CQGApp.CustomStudyDefinitionsResolved += new CQG._ICQGCELEvents_CustomStudyDefinitionsResolvedEventHandler(CEL_CustomStudyDefinitionsResolved);
                m_CQGHostForm.CQGApp.CustomStudyResolved += new CQG._ICQGCELEvents_CustomStudyResolvedEventHandler(CEL_CustomStudyResolved);
                m_CQGHostForm.CQGApp.CustomStudyAdded += new CQG._ICQGCELEvents_CustomStudyAddedEventHandler(CEL_CustomStudyAdded);
                m_CQGHostForm.CQGApp.CustomStudyUpdated += new CQG._ICQGCELEvents_CustomStudyUpdatedEventHandler(CEL_CustomStudyUpdated);

                // Expression Handling
                m_CQGHostForm.CQGApp.ExpressionResolved += new CQG._ICQGCELEvents_ExpressionResolvedEventHandler(CEL_ExpressionResolved);
                m_CQGHostForm.CQGApp.ExpressionAdded += new CQG._ICQGCELEvents_ExpressionAddedEventHandler(CEL_ExpressionAdded);
                m_CQGHostForm.CQGApp.ExpressionUpdated += new CQG._ICQGCELEvents_ExpressionUpdatedEventHandler(CEL_ExpressionUpdated);

                // Trade system
                m_CQGHostForm.CQGApp.TradingSystemResolved += new _ICQGCELEvents_TradingSystemResolvedEventHandler(CQGApp_TradingSystemResolved);
                m_CQGHostForm.CQGApp.TradingSystemAddNotification += new _ICQGCELEvents_TradingSystemAddNotificationEventHandler(CQGApp_TradingSystemAddNotification);
                m_CQGHostForm.CQGApp.TradingSystemUpdateNotification += new _ICQGCELEvents_TradingSystemUpdateNotificationEventHandler(CQGApp_TradingSystemUpdateNotification);
                m_CQGHostForm.CQGApp.TradingSystemDefinitionsResolved += new _ICQGCELEvents_TradingSystemDefinitionsResolvedEventHandler(CQGApp_TradingSystemDefinitionsResolved);
                m_CQGHostForm.CQGApp.TradingSystemInsertNotification += new _ICQGCELEvents_TradingSystemInsertNotificationEventHandler(CQGApp_TradingSystemInsertNotification);
                // Start CQGCEL
                m_CQGHostForm.CQGApp.Startup();
                driverLog.Info("InitializeCQGCEL:exit");
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "KTCQG init is not working:" + myE.Message);
                log.Error("InitializeCQGCEL", myE);
                throw myE;
            }
        }

        

        void CQGApp_InstrumentDOMChanged(CQGInstrument cqg_instrument, CQGDOMQuotes prev_asks, CQGDOMQuotes prev_bids)
        {
            try
            {
                int prevAskCount = prev_asks.Count;
                int prevBidCount = prev_bids.Count;
                int x = cqg_instrument.DOMAsks.Count;
                int y = cqg_instrument.DOMBids.Count;

                if (m_PublisherRegister.ContainsKey(cqg_instrument.FullName))
                {
                    //List<KaiTrade.Interfaces.IDOMSlot> slots = ProcessDOM(cqg_instrument,prev_asks, prev_bids);
                    List<KaiTrade.Interfaces.IDOMSlot> slots = ProcessDOMasImage(cqg_instrument, prev_asks, prev_bids);
                    base.ApplyDOMUpdate(cqg_instrument.FullName, slots.ToArray());
                    return;
                }

            }
            catch (Exception myE)
            {
                log.Error("CQGApp_InstrumentDOMChanged", myE);
            }
        }

        private void dumpDOMUpdate(CQGInstrument instrument, CQGDOMQuotes prev_asks, CQGDOMQuotes prev_bids)
        {
            try
            {
                for (int i = 0; i < prev_bids.Count; i++)
                {
                    string temp = string.Format("BID px {0} sz {1} prev: px {2} sz {3}", instrument.DOMBids[i].Price, instrument.DOMBids[i].Volume, prev_bids[i].Price, prev_bids[i].Volume);
                    log.Error(temp);
                }
                for (int i = 0; i < prev_asks.Count; i++)
                {
                    string temp = string.Format("ASK px {0} sz {1} prev: px {2} sz {3}", instrument.DOMAsks[i].Price, instrument.DOMAsks[i].Volume, prev_asks[i].Price, prev_asks[i].Volume);
                    log.Error(temp);
                }
            }
            catch (Exception myE)
            {
            }
        }
        private List<KaiTrade.Interfaces.IDOMSlot> ProcessDOM(CQGInstrument instrument, CQGDOMQuotes prev_asks, CQGDOMQuotes prev_bids)
        {
            List<KaiTrade.Interfaces.IDOMSlot> domSlots = new List<KaiTrade.Interfaces.IDOMSlot>();
            try
            {
                decimal? price;
                decimal? vol;
                CQGQuote bidQt;
                CQGQuote askQt;
                CQGQuote prevBidQt;
                CQGQuote prevAskQt;

                int prevBidQtIndex = 0;
                int prevAskQtIndex = 0;
                int qtBidIndex = 0;
                int qtAskIndex = 0;

                //dumpDOMUpdate(instrument, prev_asks, prev_bids);

                while ((qtBidIndex < instrument.DOMBids.Count) || (qtAskIndex < instrument.DOMAsks.Count))
                {
                    bidQt = instrument.DOMBids[9 - qtBidIndex];
                    askQt = instrument.DOMAsks[qtAskIndex];
                    prevBidQt = prev_bids[9 - prevBidQtIndex];
                    prevAskQt = prev_asks[prevBidQtIndex];
                    if (qtBidIndex < instrument.DOMAsks.Count)
                    {
                        // start at bottom of current bids
                        if (bidQt.Price < prevBidQt.Price)
                        {
                            // prices have dropped
                            //  ADD SLOT with BidZ = sz AskSz =null
                            domSlots.Add(new K2DataObjects.DOMSlot((decimal)bidQt.Price, (decimal)bidQt.Volume, null));
                            qtBidIndex++;
                        }
                        if (bidQt.Price == prevBidQt.Price)
                        {
                            // same price so just replace
                            if (bidQt.Volume != prevBidQt.Volume)
                            {
                                domSlots.Add(new K2DataObjects.DOMSlot((decimal)bidQt.Price, (decimal)bidQt.Volume, null));

                            }
                            prevBidQtIndex++;
                            qtBidIndex++;
                        }
                        if (bidQt.Price > prevBidQt.Price)
                        {
                            // prices moved up
                            if (prevBidQtIndex < prev_bids.Count)
                            {
                                domSlots.Add(new K2DataObjects.DOMSlot((decimal)prevBidQt.Price, null, null));
                                prevBidQtIndex++;
                            }
                            else
                            {
                                domSlots.Add(new K2DataObjects.DOMSlot((decimal)bidQt.Price, (decimal)bidQt.Volume, null));
                                qtBidIndex++;
                            }
                        }
                    }
                    else
                    {
                        // do the asks
                        if (askQt.Price < prevAskQt.Price)
                        {
                            // prices have dropped
                            domSlots.Add(new K2DataObjects.DOMSlot((decimal)askQt.Price, null, (decimal)askQt.Volume));
                            qtAskIndex++;
                        }
                        if (askQt.Price == prevAskQt.Price)
                        {
                            // same price so just replace
                            if (askQt.Volume != prevAskQt.Volume)
                            {
                                domSlots.Add(new K2DataObjects.DOMSlot((decimal)askQt.Price, null, (decimal)askQt.Volume));

                            }
                            prevAskQtIndex++;
                            qtAskIndex++;
                        }
                        if (askQt.Price > prevAskQt.Price)
                        {
                            // prices moved up
                            if (prevAskQtIndex < prev_asks.Count)
                            {
                                // this slot would have been done in the bid processing
                                //domSlots[slotIndex++] = new K2DataObjects.DOMSlot((decimal)prevAskQt.Price, null, null);
                                prevAskQtIndex++;
                            }
                            else
                            {
                                domSlots.Add(new K2DataObjects.DOMSlot((decimal)askQt.Price, null, (decimal)askQt.Volume));
                                qtAskIndex++;
                            }
                        }
                    }
                }
            }
            catch
            {
            }


            return domSlots;
        }

        private List<KaiTrade.Interfaces.IDOMSlot> ProcessDOMasImage(CQGInstrument instrument, CQGDOMQuotes prev_asks, CQGDOMQuotes prev_bids)
        {
            List<KaiTrade.Interfaces.IDOMSlot> domSlots = new List<KaiTrade.Interfaces.IDOMSlot>();
            try
            {

                CQGQuote bidQt;
                CQGQuote askQt;
                int qtBidIndex = instrument.DOMBids.Count - 1;
                int qtAskIndex = 0;
                while (qtBidIndex >= 0)
                {
                    bidQt = instrument.DOMBids[qtBidIndex--];
                    domSlots.Add(new K2DataObjects.DOMSlot((decimal)bidQt.Price, (decimal)bidQt.Volume, null));
                }
                while (qtAskIndex < instrument.DOMAsks.Count)
                {
                    askQt = instrument.DOMAsks[qtAskIndex++];
                    domSlots.Add(new K2DataObjects.DOMSlot((decimal)askQt.Price, null, (decimal)askQt.Volume));
                }


            }
            catch
            {
            }


            return domSlots;
        }

        void CQGApp_CommodityInstrumentsResolved(string commodity_name, eInstrumentType instrument_types, CQGCommodityInstruments cqg_commodity_intruments)
        {
            try
            {
                // Iterate through the resolved instrument names collection
                foreach (string instrumentName in cqg_commodity_intruments)
                {
                    // Subscribe to each instrument in the collection
                    m_CQGHostForm.CQGApp.NewInstrument(instrumentName);
                }
            }
            catch (Exception myE)
            {
                log.Error("CQGApp_CommodityInstrumentsResolved", myE);
            }
        }


        void CQGApp_TradableCommoditiesResolved(int gw_account_id, CQGCommodities cqg_commodities, CQGError cqg_error)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Stop the CQG adapter
        /// </summary>
        protected override void DoStop()
        {
            try
            {
                driverLog.Info("DoStop:enter");
                setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                if (m_GWStatus == eConnectionStatus.csConnectionUp)
                {
                    m_CQGHostForm.CQGApp.Shutdown();
                }
                m_CQGHostForm.Close();
                m_CQGHostForm = null;



                // Report that we are stopped - this will show in the adapter view
                // in the dashboard
                this.SendStatusMessage(KaiTrade.Interfaces.Status.closed, "KTCQG adapter is closed");

                // clear maps
                m_SubscribedProducts.Clear();
                m_Clients.Clear();


                m_SubscribedProducts.Clear();

                m_TSSets.Clear();
                m_ClOrdIDOrderMap.Clear();
                m_GUID2CQGOrder.Clear();

                driverLog.Info("DoStop:exit");
            }
            catch (Exception myE)
            {
                log.Error("DoStop", myE);
            }
        }

        /// <summary>
        /// Set the status of all subscriptions
        /// </summary>
        /// <param name="myStatus"></param>
        protected override void setSubscriptionsStatus(KaiTrade.Interfaces.Status myStatus)
        {
            try
            {
                driverLog.Info("setSubscriptionsStatus:" + myStatus.ToString());

                foreach (KaiTrade.Interfaces.ITSSet mySet in m_TSSets.Values)
                {
                    mySet.Status = myStatus;
                }

                base.setSubscriptionsStatus(myStatus);
            }
            catch (Exception myE)
            {
                log.Error("setSubscriptionsStatus", myE);
            }
        }

        private void processConfig(KAI.kaitns.CQGConfig myConfig)
        {
            try
            {
                foreach (KAI.kaitns.InstrDef myProduct in myConfig.InterestList.InstrDef)
                {
                    //m_ProductDefs.Add(myProduct.Mnemonic, myProduct);
                }
            }
            catch (Exception myE)
            {
                log.Error("processConfig", myE);
            }
        }

        private void subscribeInterestList()
        {
            try
            {
            }
            catch (Exception myE)
            {
                log.Error("subscribeInterestList", myE);
            }
        }

        /// <summary>
        /// Subscribe to a CQG product
        /// </summary>
        /// <param name="myDef"></param>
        private void subscribeProduct(KAI.kaitns.InstrDef myDef)
        {
            try
            {
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("subscribeProduct:mne=" + myDef.Mnemonic + " src=:" + myDef.Src);
                }
                // Record the Mnemnonic that wants a suscription - used later
                // to get a mnemonic from the long name returned by CQG
                if (m_SubscribedProducts.ContainsKey(myDef.Src))
                {
                    m_SubscribedProducts[myDef.Src] = myDef.Mnemonic;
                }
                else
                {
                    m_SubscribedProducts.Add(myDef.Src, myDef.Mnemonic);
                }
                // Send subscription request
                m_CQGHostForm.CQGApp.NewInstrument(myDef.Src);
                CQGInstrument myInstr = GetInstrument(myDef.Src);
                myInstr.DataSubscriptionLevel = eDataSubscriptionLevel.dsQuotesAndBBA;
            }
            catch (Exception myE)
            {
                log.Warn("subscribeProduct", myE);
            }
        }

        public override void RequestProductDetails(KaiTrade.Interfaces.IProduct myProduct)
        {
            try
            {
                // No Action
            }
            catch (Exception myE)
            {
                log.Error("RequestProductDetails", myE);
            }
        }

        /// <summary>
        /// Subscribe to a CQG product
        /// </summary>
        /// <param name="myFullName">CQG full name of the instrument</param>
        private void subscribeProduct(string myFullName)
        {
            try
            {
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("subscribeProduct:fullname=" + myFullName);
                }
                // Send subscription request
                m_CQGHostForm.CQGApp.NewInstrument(myFullName);
                CQGInstrument myInstr = GetInstrument(myFullName);
                myInstr.DataSubscriptionLevel = eDataSubscriptionLevel.dsQuotesAndBBA;
            }
            catch (Exception myE)
            {
                log.Warn("subscribeProduct:CQG FullName", myE);
            }
        }

        /// <summary>
        /// Subscribe to DOM for a CQG product
        /// </summary>
        /// <param name="myFullName">CQG full name of the instrument</param>
        private void subscribeProductDOM(string myFullName)
        {
            try
            {
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("subscribeProductDOM:fullname=" + myFullName);
                }
                // Send subscription request
                m_CQGHostForm.CQGApp.NewInstrument(myFullName);
                CQGInstrument myInstr = GetInstrument(myFullName);
                myInstr.DataSubscriptionLevel = eDataSubscriptionLevel.dsQuotesAndDOM;

                //m_CQGHostForm.CQGApp.
            }
            catch (Exception myE)
            {
                log.Warn("subscribeProductDOM:CQG FullName", myE);
            }
        }

        private void unSubscribeProduct(KAI.kaitns.InstrDef myDef)
        {
            try
            {
                // Record the Mnemnonic that wants a suscription - used later
                // to get a mnemonic from the long name returned by CQG
                if (m_SubscribedProducts.ContainsKey(myDef.Src))
                {
                    m_SubscribedProducts.Remove(myDef.Src);
                    CQGInstrument myInstr = GetInstrument(myDef.Src);
                    myInstr.DataSubscriptionLevel = eDataSubscriptionLevel.dsNone;
                }
            }
            catch (Exception myE)
            {
                log.Error("subscribeProduct:", myE);
            }
        }

        /// <summary>
        /// Gets a CQG intrument using a KAI mnemonic
        /// </summary>
        /// <returns>Reference to active instrument instance.</returns>
        private CQGInstrument GetInstrumentWithMnemonic(string myMnemonic)
        {
            CQGInstrument myInstr = null;
            try
            {
                KAI.kaitns.InstrDef myDef = null;
                KaiTrade.Interfaces.IProduct product = null;
                product = m_Facade.Factory.GetProductManager().GetProductMnemonic(myMnemonic);
                if (product != null)
                {
                    myInstr = GetInstrument(product.SecurityID);
                }
                else
                {
                    // it could a structured old style mnemonic
                    driverLog.Info("GetInstrumentWithMnemonic:attempt KaiUtil.GetInstrDefOnSrc:" + myMnemonic);
                    KaiUtil.KaiUtil.GetInstrDefOnSrc(out myDef, myMnemonic);
                    myInstr = GetInstrument(myDef.Src);
                }
            }
            catch (Exception myE)
            {
                log.Error("GetInstrumentWithMnemonic", myE);
            }
            return myInstr;
        }

        /// <summary>
        /// Gets a CQG intrument using a product DEF
        /// </summary>
        /// <returns>Reference to active instrument instance.</returns>
        private CQGInstrument GetInstrument(KAI.kaitns.InstrDef myDef)
        {
            try
            {
                return GetInstrument(myDef.Tag);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a CQG intrument using its full name
        /// </summary>
        /// <returns>Reference to active instrument instance.</returns>
        private CQGInstrument GetInstrument(string myFullName)
        {
            try
            {
                return m_CQGHostForm.CQGApp.Instruments[myFullName];
            }
            catch
            {
                driverLog.Info("GetInstrument:not found:" + myFullName);
                return null;
            }
        }

        /// <summary>
        /// Gets a CQG account.
        /// </summary>
        /// <returns>Reference to selected account instance.</returns>
        private CQGAccount GetAccount(string myAccountID)
        {
            try
            {
                int gwAccountID = (int)m_AccountsMap[myAccountID];
                return m_CQGHostForm.CQGApp.Accounts[gwAccountID];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Fills accounts map with reloaded accounts.
        /// </summary>
        private void FillAccountsMap()
        {
            try
            {
                // Fill accounts to combo and hashtable
                string descriptiveName;
                foreach (CQGAccount acc in m_CQGHostForm.CQGApp.Accounts)
                {
                    descriptiveName = acc.GWAccountName + " (" + acc.GWAccountID + ")";
                    this.SendAdvisoryMessage(descriptiveName);
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("AccountMap:Name=" + descriptiveName + ":" + acc.GWAccountID.ToString());
                    }
                    if (m_AccountsMap.ContainsKey(descriptiveName))
                    {
                        m_AccountsMap[descriptiveName] = acc.GWAccountID;
                        if (wireLog.IsInfoEnabled)
                        {
                            wireLog.Info("AccountMap Value replaced");
                        }
                    }
                    else
                    {
                        m_AccountsMap.Add(descriptiveName, acc.GWAccountID);
                        wireLog.Info("AccountMap Value Added");
                    }
                    // creat a KAI account databinding
                    KAI.kaitns.Account myAccountDB = new KAI.kaitns.Account();
                    myAccountDB.LongName = acc.GWAccountName;
                    // use the descriptive name for the ID since we use that as the lookup on orders
                    myAccountDB.Code = descriptiveName;
                    myAccountDB.VenueCode = m_ID;
                    myAccountDB.FirmCode = acc.FcmAccountID;


                    // send an account update message to all clients;
                    this.sendResponse("AccountUpdate", myAccountDB.ToXml());

                    // request the comodities for this account
                    //m_CQGHostForm.CQGApp.RequestTradableCommodities(acc.GWAccountID);
                }
            }
            catch (Exception myE)
            {
                log.Error("FillAccountsMap", myE);
            }
        }

        /// <summary>
        /// Get the KTA order type from a QuickFix order type
        /// </summary>
        /// <param name="ordType"></param>
        /// <returns></returns>
        private string getKTAOrderType(eOrderType ordType)
        {
            string myRet = "";
            switch (ordType)
            {
                case eOrderType.otLimit:
                    myRet = KaiTrade.Interfaces.OrderType.LIMIT;
                    break;
                case eOrderType.otMarket:
                    myRet = KaiTrade.Interfaces.OrderType.MARKET;
                    break;
                case eOrderType.otStop:
                    myRet = KaiTrade.Interfaces.OrderType.STOP;
                    break;
                case eOrderType.otStopLimit:
                    myRet = KaiTrade.Interfaces.OrderType.STOPLIMIT;
                    break;
                case eOrderType.otUndefined:
                    myRet = "";
                    break;

                default:
                    Exception myE = new Exception("CQG:Invalid order type:");
                    throw myE;
                    break;
            }
            return myRet;
        }



        /// <summary>
        /// Get the CQG order type from a QuickFix order type
        /// </summary>
        /// <param name="ordType"></param>
        /// <returns></returns>
        private eOrderType getOrderType(QuickFix.OrdType ordType)
        {
            eOrderType myRet = eOrderType.otUndefined;
            switch (ordType.getValue())
            {
                case QuickFix.OrdType.LIMIT:
                    myRet = eOrderType.otLimit;
                    break;
                case QuickFix.OrdType.MARKET:
                    myRet = eOrderType.otMarket;
                    break;
                case QuickFix.OrdType.STOP:
                    myRet = eOrderType.otStop;
                    break;
                case QuickFix.OrdType.STOP_LIMIT:
                    myRet = eOrderType.otStopLimit;
                    break;
                default:
                    Exception myE = new Exception("CQG:Invalid order type:" + ordType.ToString());
                    throw myE;
                    break;
            }
            return myRet;
        }

        /// <summary>
        /// Get the KTA order side from a CQG side
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        private string getKTAOrderSide(eTradeSide side)
        {
            string mySide = "";
            switch (side)
            {
                case eTradeSide.tsBuy:
                    mySide = KaiTrade.Interfaces.Side.BUY;
                    break;
                case eTradeSide.tsSell:
                    mySide = KaiTrade.Interfaces.Side.SELL;
                    break;
                case eTradeSide.tsOff:
                    mySide = "Off";
                    break;

                default:
                    Exception myE = new Exception("CQG:Invalid order side:");
                    throw myE;

                    break;
            }
            return mySide;
        }

        /// <summary>
        /// Get the CQG order side from a QuickFix side
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        private eOrderSide getOrderSide(QuickFix.Side side)
        {
            eOrderSide myRet = eOrderSide.osdUndefined;
            switch (side.getValue())
            {
                case QuickFix.Side.BUY:
                    myRet = eOrderSide.osdBuy;
                    break;
                case QuickFix.Side.SELL:
                    myRet = eOrderSide.osdSell;
                    break;
                default:
                    Exception myE = new Exception("CQG:Invalid order side:" + side.ToString());
                    throw myE;

                    break;
            }
            return myRet;
        }

        private string getK2Side(eOrderSide side)
        {
            string strSide = "";
            switch (side)
            {
                case  eOrderSide.osdBuy:
                    strSide = KaiTrade.Interfaces.Side.BUY;
                    break;
                case eOrderSide.osdSell:
                    strSide = KaiTrade.Interfaces.Side.SELL;
                    break;               
                default:
                     
                    //log.
                    //Exception myE = new Exception("CQG:Invalid order side:" + side.ToString());
                    //throw myE;
                    break;

            }
            return strSide;
        }



        /// <summary>
        /// Get the CQG order duration from a quickfix TimeInForce
        /// </summary>
        /// <param name="tif"></param>
        /// <returns></returns>
        private eOrderDuration getOrderDuration(QuickFix.TimeInForce tif)
        {
            eOrderDuration myRet = eOrderDuration.odUndefined;
            switch (tif.getValue())
            {
                case QuickFix.TimeInForce.DAY:
                    myRet = eOrderDuration.odDay;
                    break;
                case QuickFix.TimeInForce.GOOD_TILL_DATE:
                    myRet = eOrderDuration.odGoodTillDate;
                    break;
                case QuickFix.TimeInForce.GOOD_TILL_CANCEL:
                    myRet = eOrderDuration.odGoodTillCanceled;
                    break;
                case QuickFix.TimeInForce.FILL_OR_KILL:
                    myRet = eOrderDuration.odFOK;
                    break;

                default:
                    Exception myE = new Exception("CQG:Invalid order duration:" + tif.ToString());
                    throw myE;

                    break;
            }
            return myRet;
        }

        private void demoThrottle()
        {
            try
            {
                if (m_UseOEThrottle)
                {
                    long ticksNow = DateTime.Now.Ticks;
                    long ticksDelta = ticksNow - m_LastORRequestTicks;
                    if (ticksDelta < m_RequiredORIntervalTicks)
                    {
                        if (log.IsInfoEnabled)
                        {
                            log.Info("DEMO THROTTLE APPLIED: Ticks Delta:" + ticksDelta.ToString());
                        }
                        int waitDuration = (int)((m_RequiredORIntervalTicks - ticksDelta) / 10000);
                        System.Threading.Thread.Sleep(waitDuration);
                    }

                    m_LastORRequestTicks = DateTime.Now.Ticks;

                }
            }
            catch (Exception myE)
            {
            }
        }

        private void submitOrder(KaiTrade.Interfaces.Message myMsg)
        {
           
            QuickFix.Message myQFOrder = null;

            try
            {
                // Extract the raw FIX Message from the inbound message
                string strOrder = myMsg.Data;
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("submitOrder:" + strOrder);
                }
                if (driverLog.IsInfoEnabled)
                {
                    driverLog.Info("submitOrder:" + strOrder);
                }

                // Use QuickFix to handle the message
                myQFOrder = new QuickFix.Message(strOrder);

                // We should now use the src
                QuickFix.SecurityID mySecID = new QuickFix.SecurityID();
                QuickFix.SecurityIDSource mySecIDSrc = new QuickFix.SecurityIDSource();
                if (myQFOrder.isSetField(mySecID))
                {
                    myQFOrder.getField(mySecID);

                    if (mySecID.getValue().StartsWith("SPREAD"))
                    {
                        string parametersAlias = "";
                        QuickFix.TargetStrategyParameters targetStrategyParameters = new QuickFix.TargetStrategyParameters("");
                        if (myQFOrder.isSetField(targetStrategyParameters))
                        {
                            myQFOrder.getField(targetStrategyParameters);
                            parametersAlias = targetStrategyParameters.getValue();
                        }
    
                        submitSpreadOrder(myMsg, parametersAlias);
                        return;
                    }
                }

                // Use product manager to validate the product specified on
                // the order exists for this adapter

                // Get the product associated with the FIX message


                QuickFix.Symbol symbol = new QuickFix.Symbol();
                QuickFix.Side side = new QuickFix.Side();
                QuickFix.OrdType ordType = new QuickFix.OrdType();
                QuickFix.OrderQty orderQty = new QuickFix.OrderQty();
                QuickFix.Price price = new QuickFix.Price();
                QuickFix.StopPx stopPx = new QuickFix.StopPx();
                QuickFix.Account account = new QuickFix.Account();
                QuickFix.StrikePrice strikePrice = new QuickFix.StrikePrice();
                QuickFix.Currency currency = new QuickFix.Currency();
                QuickFix.CFICode cfiCode = new QuickFix.CFICode();
                QuickFix.SecurityExchange exchange = new QuickFix.SecurityExchange();
                QuickFix.ClOrdID clOrdID = new QuickFix.ClOrdID();
                QuickFix.TimeInForce tif = new QuickFix.TimeInForce();
                QuickFix.ExpireDate expireDate = new QuickFix.ExpireDate();
                QuickFix.MaturityMonthYear MMY = new QuickFix.MaturityMonthYear();

                // Get the CQG account
                CQGAccount myAccount = null;
                if (myQFOrder.isSetField(account))
                {
                    myQFOrder.getField(account);
                    myAccount = GetAccount(account.getValue());
                    if (myAccount != null)
                    {
                        driverLog.Info("CQGAcct:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
                    }
                }
                if (myAccount == null)
                {
                    this.SendAdvisoryMessage("CQG: you need to provide a valid account");
                    throw new NullReferenceException("No account is selected.");
                }


                ///Apply a throttle 
                demoThrottle();


                CQGInstrument instrument = null;
                // We should now use the src
                //QuickFix.SecurityID mySecID = new QuickFix.SecurityID();
                //QuickFix.SecurityIDSource mySecIDSrc = new QuickFix.SecurityIDSource();
                if (myQFOrder.isSetField(mySecID))
                {
                    myQFOrder.getField(mySecID);
                    instrument = GetInstrument(mySecID.getValue());
                }
                if (instrument == null)
                {
                    // Get the CQG product/intrument we want to order
                    string myMnemonic = KaiUtil.QFUtils.GetProductMnemonic(m_ID, "", myQFOrder);

                    instrument = GetInstrumentWithMnemonic(myMnemonic);
                }
                if (instrument == null)
                {
                    this.SendAdvisoryMessage("CQG: invalid product/instrument requested - check your product sheet");
                    throw new NullReferenceException("No instrument is selected.");
                }

                // Get the Order type
                myQFOrder.getField(ordType);
                eOrderType orderType = getOrderType(ordType);


                // Get the QTY
                myQFOrder.getField(orderQty);
                int quantity = (int)orderQty.getValue();

                // Order side can be specified in two ways:
                //        * if UseOrderSide is set in APIConfig then we need to specify side
                //        explicitly, and order quantity must be greater than 0.
                //      * if setting below is not set the side is detected by order quantity sign.
                //        Negative quantity specifies sell side.
                // Here we use the second one
                myQFOrder.getField(side);
                eOrderSide orderSide = getOrderSide(side);

                if (orderSide == eOrderSide.osdSell)
                {
                    quantity *= -1;
                }

                // Default values of prices
                double limitPrice = 0.0;
                if (myQFOrder.isSetField(price))
                {
                    myQFOrder.getField(price);
                    limitPrice = price.getValue();
                }

                double stopPrice = 0.0;
                if ((myQFOrder.isSetField(stopPx)) && ((orderType == eOrderType.otStop) || (orderType == eOrderType.otStopLimit)))
                {
                    myQFOrder.getField(stopPx);
                    stopPrice = stopPx.getValue();
                    limitPrice = 0;
                }


                // Create order, since we have already all the needed parameters
                CQGOrder order = m_CQGHostForm.CQGApp.CreateOrder(orderType, instrument, myAccount, quantity, eOrderSide.osdUndefined, limitPrice, stopPrice, "");

                // Set order parked status

                order.Properties[eOrderProperty.opParked].Value = false;


                // Set order duration
                eOrderDuration durationType = eOrderDuration.odDay;
                if (myQFOrder.isSetField(tif))
                {
                    myQFOrder.getField(tif);
                    durationType = getOrderDuration(tif);
                    order.Properties[eOrderProperty.opDurationType].Value = durationType;
                    if (durationType == eOrderDuration.odGoodTillDate)
                    {
                        if (myQFOrder.isSetField(expireDate))
                        {
                            myQFOrder.getField(expireDate);

                            DateTime myDate;
                            KaiUtil.KaiUtil.FromLocalMktDate(out  myDate, expireDate.getValue());

                            order.Properties[eOrderProperty.opGTDTime].Value = myDate;
                        }
                        else
                        {
                            this.SendAdvisoryMessage("CQG: no expire date given on a Good till date order");
                            throw new NullReferenceException("CQG: no expire date given on a Good till date order");
                        }
                    }
                }

                // Record the CQG order
                myQFOrder.getField(clOrdID);
                DriverBase.OrderContext myContext = new DriverBase.OrderContext();
                myContext.ExternalOrder = order;
                myContext.QFOrder = myQFOrder;
                myContext.ClOrdID = clOrdID.getValue();


                m_GUID2CQGOrder.Add(order.GUID, myContext);
                m_ClOrdIDOrderMap.Add(clOrdID.getValue(), myContext);

                // send the order
                driverLog.Info("CQGAcctA:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
                order.Place();
                myContext.CurrentCommand = DriverBase.ORCommand.Submit;
                driverLog.Info("CQGAcctB:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
            }
            catch (Exception myE)
            {
                log.Error("submitOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("CQG:submitOrder: problem submitting order:" + myE.ToString());

                QuickFix.OrdStatus myOrdStatus;
                QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.REJECTED);

                myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                QuickFix.OrderQty orderQty = new QuickFix.OrderQty();
                if (myQFOrder != null)
                {
                    myQFOrder.getField(orderQty);
                    QuickFix.OrdRejReason myRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.OTHER);
                    sendExecReport(myQFOrder, new QuickFix.OrderID("UNKNOWN"), myOrdStatus, myExecType, 0.0, (int)orderQty.getValue(), 0, 0, 0, myE.Message, myRejReason);
                }
            }
        }

        private void pullOrder(KaiTrade.Interfaces.Message myMsg)
        {
            QuickFix.Message myQFPullOrder = null;
            DriverBase.OrderContext myContext = null;
            try
            {
                if (m_QueueReplaceRequests)
                {
                    base.ApplyCancelRequest(myMsg);
                    return;
                }
                // Extract the raw FIX Message from the inbound message
                string strOrder = myMsg.Data;

                // Use QuickFix to handle the message
                myQFPullOrder = new QuickFix.Message(strOrder);

                // Get the FIX id's these are mandatory
                QuickFix.ClOrdID clOrdID = new QuickFix.ClOrdID();
                QuickFix.OrigClOrdID origClOrdID = new QuickFix.OrigClOrdID();

                if (myQFPullOrder.isSetField(clOrdID))
                {
                    myQFPullOrder.getField(clOrdID);
                }
                else
                {
                    sendCancelRej(myQFPullOrder, QuickFix.CxlRejReason.UNKNOWN_ORDER, "a clordid must be specified on a cancel order");
                    Exception myE = new Exception("a clordid must be specified on a cancel order");
                    throw myE;
                }

                if (myQFPullOrder.isSetField(origClOrdID))
                {
                    myQFPullOrder.getField(origClOrdID);
                }
                else
                {
                    sendCancelRej(myQFPullOrder, QuickFix.CxlRejReason.UNKNOWN_ORDER, "a original clordid must be specified on a cancel order");
                    Exception myE = new Exception("an original clordid must be specified on a cancel order");
                    throw myE;
                }

                // Get the context - we must have this to access the CQG order
                myContext = null;
                if (m_ClOrdIDOrderMap.ContainsKey(origClOrdID.getValue()))
                {
                    myContext = m_ClOrdIDOrderMap[origClOrdID.getValue()] as DriverBase.OrderContext;
                }
                if (myContext == null)
                {
                    sendCancelRej(myQFPullOrder, QuickFix.CxlRejReason.UNKNOWN_ORDER, "an order does not exist for the cancel requested");
                    Exception myE = new Exception("an order does not exist for the cancel requested");
                    throw myE;
                }
                else
                {
                    // record the context against the new clordid
                    m_ClOrdIDOrderMap.Add(clOrdID.getValue(), myContext);
                }



                QuickFix.ClOrdID newClordID = new QuickFix.ClOrdID(clOrdID.getValue());

                QuickFix.OrigClOrdID newOrigClOrdID = new QuickFix.OrigClOrdID(origClOrdID.getValue());
                myContext.QFOrder.setField(newClordID);
                myContext.QFOrder.setField(newOrigClOrdID);

                demoThrottle();
                //qorList[0].Cancel();

                //int x = m_CQGHostForm.CQGApp.
                
                // Cancel the order
                //m_CQGHostForm.CQGApp.
                (myContext.ExternalOrder as CQGOrder).Cancel();
                

                myContext.CurrentCommand = DriverBase.ORCommand.Pull;

                // record the context against the new clordid
                //m_ClIDOrder.Add(clOrdID.getValue(), myContext);


            }
            catch (Exception myE)
            {
                log.Error("pullOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("CQG:pullOrder: problem pulling order:" + myE.ToString());
                if (myContext != null)
                {
                    myContext.CurrentCommand = DriverBase.ORCommand.Undefined;
                }
                sendCancelRej(myQFPullOrder, QuickFix.CxlRejReason.TOO_LATE_TO_CANCEL, "the order is not in a state that can be cancelled");
            }
        }


        private void modifyOrder(KaiTrade.Interfaces.Message myMsg)
        {
            try
            {
                if (m_QueueReplaceRequests)
                {
                    base.ApplyReplaceRequest(myMsg);
                    return;
                }
                QuickFix.Message myQFModOrder = null;

                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("modifyOrder:" + myMsg.Data);
                }
                // Extract the raw FIX Message from the inbound message
                string strOrder = myMsg.Data;

                // Use QuickFix to handle the message
                myQFModOrder = new QuickFix.Message(strOrder);

                // Get the FIX id's these are mandatory
                QuickFix.ClOrdID clOrdID = new QuickFix.ClOrdID();
                QuickFix.OrigClOrdID origClOrdID = new QuickFix.OrigClOrdID();

                if (myQFModOrder.isSetField(clOrdID))
                {
                    myQFModOrder.getField(clOrdID);
                }
                else
                {
                    sendCancelReplaceRej(myQFModOrder, QuickFix.CxlRejReason.UNKNOWN_ORDER, "a clordid must be specified on a modifyOrder ");
                    Exception myE = new Exception("a clordid must be specified on a modifyOrder");
                    throw myE;
                }

                if (myQFModOrder.isSetField(origClOrdID))
                {
                    myQFModOrder.getField(origClOrdID);
                }
                else
                {
                    sendCancelReplaceRej(myQFModOrder, QuickFix.CxlRejReason.UNKNOWN_ORDER, "a original clordid must be specified on a modifyOrder");
                    Exception myE = new Exception("an original clordid must be specified on a modifyOrder");
                    throw myE;
                }

                // Get the context - we must have this to access the CQG order
                DriverBase.OrderContext myContext = null;
                if (m_ClOrdIDOrderMap.ContainsKey(origClOrdID.getValue()))
                {
                    myContext = m_ClOrdIDOrderMap[origClOrdID.getValue()] as DriverBase.OrderContext;
                }
                if (myContext == null)
                {
                    sendCancelReplaceRej(myQFModOrder, QuickFix.CxlRejReason.UNKNOWN_ORDER, "an order does not exist for the modifyOrder");
                    Exception myE = new Exception("an order does not exist for the modifyOrder");
                    throw myE;
                }
                else
                {
                    // record the context against the new clordid
                    m_ClOrdIDOrderMap.Add(clOrdID.getValue(), myContext);
                }
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("modifyOrder:context" + myContext.ToString());
                    m_ORLog.Info("modifyOrder:order" + myContext.ExternalOrder.ToString());
                }

                // Check that we are not pending
                //if(myContext.CQGOrder.

                double myLastFillPrice = 0.0;
                double myAveFillPrice = 0.0;
                double myLastFillQty = 0.0;
                //getFillValues(myContext.CQGOrder, out myLastFillPrice, out myAveFillPrice, out myLastFillQty);
                //QuickFix.OrdStatus myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_CANCEL);
                //QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.ORDER_STATUS);

                //sendExecReport(myContext.QFGOrder, new QuickFix.OrderID(myContext.CQGOrder.GWOrderID.ToString()), myOrdStatus, myExecType,
                //                           myLastFillQty, myContext.CQGOrder.RemainingQuantity, myContext.CQGOrder.FilledQuantity,
                //                               myLastFillPrice, myAveFillPrice);


                // swap the clordid to the new ones  on our stored order
                QuickFix.ClOrdID newClordID = new QuickFix.ClOrdID(clOrdID.getValue());
                QuickFix.OrigClOrdID newOrigClOrdID = new QuickFix.OrigClOrdID(origClOrdID.getValue());
                myContext.QFOrder.setField(newClordID);
                myContext.QFOrder.setField(newOrigClOrdID);




                // get the modified values - CQGOrderModify object used to modify order's parameters.
                CQGOrderModify myOrderModify = (myContext.ExternalOrder as CQGOrder).PrepareModify();
                bool isModified = false;

                // is the order in a state where it can be modified
                if (!(myContext.ExternalOrder as CQGOrder).CanBeModified)
                {
                    sendCancelReplaceRej(myQFModOrder, QuickFix.CxlRejReason.ORDER_ALREADY_IN_PENDING_CANCEL_OR_PENDING_REPLACE_STATUS, "the order cannot be modified");
                    Exception myE = new Exception("the order cannot be modified");
                    throw myE;
                }

                // modify the qty 
                QuickFix.OrderQty newOrderQty = new QuickFix.OrderQty();
                if (myQFModOrder.isSetField(newOrderQty))
                {
                    try
                    {
                        myQFModOrder.getField(newOrderQty);
                        CQGOrderProperty myProperty = myOrderModify.Properties[eOrderProperty.opQuantity];
                        int quantity = (int)newOrderQty.getValue();
                        // Checking order side by the order's quanitity sign
                        quantity *= Math.Sign((myContext.ExternalOrder as CQGOrder).Quantity);
                        if (quantity != (myContext.ExternalOrder as CQGOrder).Quantity)
                        {
                            myProperty.Value = quantity;
                            isModified = true;
                        }
                    }
                    catch (Exception myE)
                    {
                    }
                }

                // modify the limit price
                QuickFix.Price newPrice = new QuickFix.Price();
                if (myQFModOrder.isSetField(newPrice))
                {
                    try
                    {
                        myQFModOrder.getField(newPrice);
                        CQGOrderProperty myProperty = myOrderModify.Properties[eOrderProperty.opLimitPrice];

                        //double limitPrice = m_Order.Instrument.FromDisplayPrice(newPrice.ToString());
                        double limitPrice = newPrice.getValue();
                        if (limitPrice != (myContext.ExternalOrder as CQGOrder).LimitPrice)
                        {
                            myProperty.Value = limitPrice;
                            isModified = true;
                        }
                    }
                    catch (Exception myE)
                    {
                    }
                }

                // modify the stop price
                //if(myQFModOrder
                QuickFix.OrdType myOldOrdType = new QuickFix.OrdType();
                myContext.QFOrder.getField(myOldOrdType);
                if ((myOldOrdType.getValue() == QuickFix.OrdType.STOP) || (myOldOrdType.getValue() == QuickFix.OrdType.STOP_LIMIT))
                {
                    QuickFix.StopPx newStopPx = new QuickFix.StopPx();
                    if (myQFModOrder.isSetField(newStopPx))
                    {
                        myQFModOrder.getField(newStopPx);
                        CQGOrderProperty myProperty = myOrderModify.Properties[eOrderProperty.opStopPrice];

                        //double limitPrice = m_Order.Instrument.FromDisplayPrice(newStopPx.ToString());
                        double stopPrice = newStopPx.getValue();
                        if (stopPrice != (myContext.ExternalOrder as CQGOrder).StopPrice)
                        {
                            myProperty.Value = stopPrice;
                            isModified = true;
                        }
                    }
                }

                // Modify order if needed
                if (isModified)
                {
                    (myContext.ExternalOrder as CQGOrder).Modify(myOrderModify);
                    myContext.CurrentCommand = DriverBase.ORCommand.Modify;
                }


                // record the context against the new clordid
                //m_ClIDOrder.Add(clOrdID.getValue(), myContext);


            }
            catch (Exception myE)
            {
                log.Error("modifyOrder", myE);
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("modifyOrder:context:Exception", myE);
                }
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("CQG:modifyOrder: problem modifying order:" + myE.ToString());
            }
        }



        public override KaiTrade.Interfaces.orderReplaceResult modifyOrder(DriverBase.CancelReplaceData replaceData)
        {
            try
            {
                if (m_ORLog.IsInfoEnabled)
                {
                    //m_ORLog.Info("modifyOrder:" + myMsg.Data);
                }



                if (replaceData.OrderContext == null)
                {
                    sendCancelReplaceRej(replaceData.LastQFMod, QuickFix.CxlRejReason.UNKNOWN_ORDER, "an order does not exist for the modifyOrder");
                    Exception myE = new Exception("an order does not exist for the modifyOrder");
                    throw myE;
                }


                // get the modified values - CQGOrderModify object used to modify order's parameters.
                CQGOrderModify myOrderModify = (replaceData.OrderContext.ExternalOrder as CQGOrder).PrepareModify();
                bool isModified = false;

                // is the order in a state where it can be modified
                if (!(replaceData.OrderContext.ExternalOrder as CQGOrder).CanBeModified)
                {
                    return KaiTrade.Interfaces.orderReplaceResult.replacePending;
                }

                // modify the qty 
                if (replaceData.qty.HasValue)
                {
                    try
                    {

                        CQGOrderProperty myProperty = myOrderModify.Properties[eOrderProperty.opQuantity];
                        int quantity = (int)replaceData.qty.Value;
                        // Checking order side by the order's quanitity sign
                        quantity *= Math.Sign((replaceData.OrderContext.ExternalOrder as CQGOrder).Quantity);
                        if (quantity != (replaceData.OrderContext.ExternalOrder as CQGOrder).Quantity)
                        {
                            myProperty.Value = quantity;
                            isModified = true;
                        }
                    }
                    catch (Exception myE)
                    {
                    }
                }

                // modify the limit price
                if (replaceData.Price.HasValue)
                {
                    try
                    {
                        CQGOrderProperty myProperty = myOrderModify.Properties[eOrderProperty.opLimitPrice];
                        if (replaceData.Price.Value != (replaceData.OrderContext.ExternalOrder as CQGOrder).LimitPrice)
                        {
                            myProperty.Value = replaceData.Price.Value;
                            isModified = true;
                        }
                    }
                    catch (Exception myE)
                    {
                    }
                }

                // modify the stop price
                //if(myQFModOrder
                QuickFix.OrdType myOldOrdType = new QuickFix.OrdType();
                replaceData.OrderContext.QFOrder.getField(myOldOrdType);
                if ((myOldOrdType.getValue() == QuickFix.OrdType.STOP) || (myOldOrdType.getValue() == QuickFix.OrdType.STOP_LIMIT))
                {

                    if (replaceData.StopPrice.HasValue)
                    {
                        CQGOrderProperty myProperty = myOrderModify.Properties[eOrderProperty.opStopPrice];

                        if (replaceData.StopPrice.Value != (replaceData.OrderContext.ExternalOrder as CQGOrder).StopPrice)
                        {
                            myProperty.Value = replaceData.StopPrice.Value;
                            isModified = true;
                        }
                    }
                }

                // Modify order if needed
                if (isModified)
                {
                    (replaceData.OrderContext.ExternalOrder as CQGOrder).Modify(myOrderModify);
                    replaceData.OrderContext.CurrentCommand = DriverBase.ORCommand.Modify;
                }
                else
                {
                    log.Warn("Order was not replaced as all fields were the same");
                }

                return KaiTrade.Interfaces.orderReplaceResult.success;

            }
            catch (Exception myE)
            {
                log.Error("modifyOrderRD", myE);
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("modifyOrderRD:context:Exception", myE);
                }
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("CQG:modifyOrderRD: problem modifying order:" + myE.ToString());

                return KaiTrade.Interfaces.orderReplaceResult.error;
            }
        }

        /// <summary>
        /// Used from the queue repace and cancel processor
        /// </summary>
        /// <param name="replaceData"></param>
        /// <returns></returns>
        public override KaiTrade.Interfaces.orderReplaceResult cancelOrder(DriverBase.CancelReplaceData replaceData)
        {
            try
            {
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("cancelOrderR:" + replaceData.LastQFMod);
                }

                if (replaceData.OrderContext == null)
                {
                    sendCancelRej(replaceData.LastQFMod, QuickFix.CxlRejReason.UNKNOWN_ORDER, "an order does not exist for the cancel requested");
                    Exception myE = new Exception("an order does not exist for the cancel requested");
                    throw myE;
                }

                // is the order in a state where it can be cancelled
                if (!(replaceData.OrderContext.ExternalOrder as CQGOrder).CanBeCanceled)
                {
                    return KaiTrade.Interfaces.orderReplaceResult.cancelPending;
                }

                // Cancel the order
                (replaceData.OrderContext.ExternalOrder as CQGOrder).Cancel();

                replaceData.OrderContext.CurrentCommand = DriverBase.ORCommand.Pull;

                // record the context against the new clordid
                //m_ClIDOrder.Add(clOrdID.getValue(), myContext);

                return KaiTrade.Interfaces.orderReplaceResult.success;
            }
            catch (Exception myE)
            {
                log.Error("cancelOrderRD", myE);
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("cancelOrderRD:Exception", myE);
                }
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("CQG:pullOrderRepData: problem pulling order:" + myE.ToString());

                return KaiTrade.Interfaces.orderReplaceResult.error;
            }
        }

        private void getFillValues(CQG.ICQGOrder order, out double myLastFillPrice, out double myAveFillPrice, out double myLastFillQty)
        {

            myLastFillPrice = 0.0;
            myAveFillPrice = 0.0;
            myLastFillQty = 0.0;
            try
            {
                if (order.Fills.Count > 0)
                {
                    double myPxTotal = 0.0;
                    foreach (CQG.ICQGFill myFill in order.Fills)
                    {
                        myLastFillPrice = myFill.get_Price(0);
                        myPxTotal += myLastFillPrice;

                        myLastFillQty = myFill.get_Quantity(0);
                    }

                    myAveFillPrice = myPxTotal / order.Fills.Count;
                }
            }
            catch (Exception myE)
            {
            }
        }

        private KaiTrade.Interfaces.OrderData getOrderData(CQGOrder order)
        {

            KaiTrade.Interfaces.OrderData orderData = new K2DataObjects.OrderData();
            try
            {
                orderData.Identity = order.GUID;

                orderData.StrategyName = "CQGNotKnown";
                orderData.Mnemonic = order.InstrumentName;
                orderData.Account = order.Account.GWAccountName;
                
                orderData.OrdType = getKTAOrderType(order.Type);
                switch (orderData.OrdType)
                {
                    case KaiTrade.Interfaces.OrderType.MARKET:
                        break;
                    case KaiTrade.Interfaces.OrderType.LIMIT:
                        orderData.Price = order.LimitPrice;
                        break;
                    case KaiTrade.Interfaces.OrderType.STOP:
                        
                        orderData.StopPx = order.StopPrice;
                        break;
                    case KaiTrade.Interfaces.OrderType.STOPLIMIT:
                        orderData.Price = order.LimitPrice;
                        orderData.StopPx = order.StopPrice;
                        break;
                    default:
                        orderData.StopPx = order.StopPrice;
                        orderData.Price = order.LimitPrice;
                        break;
                }
                 
                orderData.OrderID = order.OriginalOrderID;
                orderData.OrdType = getKTAOrderType(order.Type);
                orderData.Side = getK2Side(order.Side);
                if (order.Side == eOrderSide.osdUndefined)
                {
                    if (order.Quantity < 0)
                    {
                        orderData.OrderQty = order.Quantity * -1;
                        orderData.Side = KaiTrade.Interfaces.Side.SELL;
                    }
                    else
                    {
                        orderData.OrderQty = order.Quantity ;
                        orderData.Side = KaiTrade.Interfaces.Side.BUY;
                    }
                }
                else
                {
                    orderData.OrderQty = order.Quantity;
                }
                if (order.FilledQuantity < 0)
                {
                    orderData.CumQty = order.FilledQuantity*-1;
                }
                else
                {
                    orderData.CumQty = order.FilledQuantity;
                }
                if (order.RemainingQuantity<0)
                {
                orderData.LeavesQty = order.RemainingQuantity*-1;
                }
                else
                {
                    orderData.LeavesQty = order.RemainingQuantity;
                }
                orderData.TradeVenue = "KTACQG";
                 
                if (order.Fills.Count > 0)
                {
                    double myLastFillPrice;
                    double myAveFillPrice;
                    double myLastFillQty;
                    getFillValues(order, out myLastFillPrice, out myAveFillPrice, out myLastFillQty);
                    orderData.LastPx = myLastFillPrice;
                    orderData.AvgPx = myAveFillPrice;
                    orderData.LastQty = myLastFillQty;
                }
                

                QuickFix.OrdStatus ordStatus;
                QuickFix.OrdRejReason ordRejReason;
                string myText;
                deCodeGWStatus(order, out ordStatus, out ordRejReason, out myText);
                orderData.OrdStatus = KaiUtil.QFUtils.DecodeOrderStatus(ordStatus);
                orderData.Text = myText;
                orderData.TimeInForce = getK2TimeType(order.DurationType);
                
                string tag = "CQG:"+order.OriginalOrderID+":";
                tag += order.GUID + ":";
                tag += order.UEName+ ":";
                orderData.Tag = tag;



                orderData.TransactTime = order.PlaceTime.ToUniversalTime().ToString();


            }
            catch (Exception myE)
            {
                log.Error("getOrderData", myE);
            }

            return orderData;
        }

        private string getK2TimeType(eOrderDuration cqgDuration)
        {
            string timeType = "";
            switch (cqgDuration)
            {
                case eOrderDuration.odDay:
                    timeType = KaiTrade.Interfaces.TimeType.DAY;
                    break;
                case eOrderDuration.odATC:
                    timeType = KaiTrade.Interfaces.TimeType.ATC;
                    break;
                case eOrderDuration.odATO:
                    //timeType = KaiTrade.Interfaces.TimeType.a;
                    break;
                case eOrderDuration.odFOK:
                    timeType = KaiTrade.Interfaces.TimeType.FOK;
                    break;
                case eOrderDuration.odGoodTillCanceled:
                    timeType = KaiTrade.Interfaces.TimeType.GTC;
                    break;
                case eOrderDuration.odGoodTillDate:
                    timeType = KaiTrade.Interfaces.TimeType.GTD;
                    break;
                case eOrderDuration.odGoodTillTime:
                    timeType = KaiTrade.Interfaces.TimeType.GTD;
                    break;
                default:
                    break;

            }
            return timeType;
        }
        private void deCodeGWStatus(CQGOrder order, out QuickFix.OrdStatus myOrdStatus, out QuickFix.OrdRejReason myOrdRejReason, out string myText)
        {
            myOrdStatus = new QuickFix.OrdStatus();
            myText = "";
            myOrdRejReason = null;
            try
            {
                switch (order.GWStatus)
                {
                    case eOrderStatus.osInOrderBook:

                        myText = "eOrderStatus.osInOrderBook:";
                        if (order.FilledQuantity == 0)
                        {
                            myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.NEW);
                        }
                        else
                        {
                            myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PARTIALLY_FILLED);

                        }
                        break;
                    case eOrderStatus.osRejectFCM:
                        myText = "eOrderStatus.osRejectFCM:" + order.LastError.Description;
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                        myOrdRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.OTHER);
                        break;
                    case eOrderStatus.osRejectGW:
                        myText = "eOrderStatus.osRejectGW:" + order.LastError.Description;
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                        myOrdRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.OTHER);
                        break;
                    case eOrderStatus.osInCancel:
                        myText = "eOrderStatus.osInCancel";
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_CANCEL);
                        break;
                    case eOrderStatus.osCanceled:
                        myText = "eOrderStatus.osCanceled";
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.CANCELED);
                        break;
                    case eOrderStatus.osInModify:
                        myText = "eOrderStatus.osInModify";
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_REPLACE);
                        break;
                    case eOrderStatus.osBusted:
                        myText = "eOrderStatus.osBusted";
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.SUSPENDED);
                        break;
                    case eOrderStatus.osExpired:
                        myText = "eOrderStatus.osExpired";
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.EXPIRED);
                        break;
                    case eOrderStatus.osFilled:
                        myText = "eOrderStatus.osFilled";
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.FILLED);
                        break;
                    case eOrderStatus.osInTransit:
                        myText = "eOrderStatus.osInTransit";
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_NEW);
                        break;
                    case eOrderStatus.osInTransitTimeout:
                        myText = "eOrderStatus.osInTransitTimeout" + order.LastError.Description;
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                        myOrdRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.TOO_LATE_TO_ENTER);
                        break;
                    case eOrderStatus.osNotSent:
                        myText = "eOrderStatus.osNotSent" + order.LastError.Description;
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                        myOrdRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.OTHER);
                        break;
                    default:
                        myText = "eOrderStatus - not known:" + order.GWStatus.ToString();
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                        myOrdRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.OTHER);
                        break;
                }
            }
            catch (Exception myE)
            {
            }
        }

        private void updateOrder(eChangeType change, CQGOrder order)
        {
            try
            {
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("updateOrder:" + change.ToString());
                }
                QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.ORDER_STATUS);

                // get the order context using the GUID
                DriverBase.OrderContext myContext = null;

                if (m_GUID2CQGOrder.ContainsKey(order.GUID))
                {
                    myContext = m_GUID2CQGOrder[order.GUID] as DriverBase.OrderContext;
                };

                if (myContext == null)
                {
                    string orderDetails = string.Format("Guid:{0} origId:{1} symbol:{2} :{3} side:{4} qty:{5} leaves:{6} type:{7}", order.GUID, order.OriginalOrderID, order.GWOrderID, order.InstrumentName, order.Side.ToString(), order.Quantity, order.RemainingQuantity, order.Type.ToString());
                    Exception myE = new Exception("Order is not known:" +orderDetails);
                    throw myE;
                }



                double myLastFillPrice = 0.0;
                double myAveFillPrice = 0.0;
                double myLastFillQty = 0.0;
                getFillValues(order, out myLastFillPrice, out myAveFillPrice, out myLastFillQty);
                if (myLastFillQty < 0)
                {
                    myLastFillQty *= -1;
                }

                // get the order status from the gateway
                string myText;
                QuickFix.OrdStatus myOrdStatus;
                QuickFix.OrdRejReason myOrdRejReason;
                deCodeGWStatus(order, out myOrdStatus, out myOrdRejReason, out myText);
                switch (myOrdStatus.getValue())
                {
                    case QuickFix.OrdStatus.FILLED:
                        myExecType = new QuickFix.ExecType(QuickFix.ExecType.FILL);
                        break;
                    case QuickFix.OrdStatus.PARTIALLY_FILLED:
                        myExecType = new QuickFix.ExecType(QuickFix.ExecType.PARTIAL_FILL);
                        break;
                    default:
                        break;
                }



                //eOrderLocalState.
                // Send an Exec Report depending on the type of change
                switch (change)
                {
                    case eChangeType.ctAdded:
                        // status is set by GW above
                        // should be NEW
                        if (myContext.CurrentCommand != DriverBase.ORCommand.Submit)
                        {
                            myContext.CurrentCommand = DriverBase.ORCommand.Undefined;
                            // expected submit
                            driverLog.Warn("ctAdded - but not on submit");
                        }
                        break;

                    case eChangeType.ctChanged:
                        // status is set by GW
                        // if pending some replace then set as replaced
                        if (myContext.CurrentCommand == DriverBase.ORCommand.Modify)
                        {
                            if (order.GWStatus == eOrderStatus.osInModify)
                            {
                                myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_REPLACE);
                            }
                            else
                            {
                                myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REPLACED);
                            }
                        }
                        break;
                    case eChangeType.ctRemoved:
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.CANCELED);
                        myContext.CurrentCommand = DriverBase.ORCommand.Undefined;

                        break;
                    default:
                        //myContext.CurrentCommand = DriverBase.ORCommand.Undefined;
                        Exception myE = new Exception("unknown order change type:" + change.ToString());
                        throw myE;
                        break;
                }



                // get the CQG qtys - check for -ve  CQG can treat a Sell Qty as -ve value, the toolkit always wants +ve and a SIDE
                int myRemQty = (myContext.ExternalOrder as CQGOrder).RemainingQuantity;
                if (myRemQty < 0)
                {
                    myRemQty *= -1;
                }
                int myFillQty = (myContext.ExternalOrder as CQGOrder).FilledQuantity;
                if (myFillQty < 0)
                {
                    myFillQty *= -1;
                }

                sendExecReport(myContext.QFOrder, new QuickFix.OrderID((myContext.ExternalOrder as CQGOrder).GWOrderID.ToString()), myOrdStatus, myExecType,
                                            myLastFillQty, myRemQty, myFillQty, myLastFillPrice, myAveFillPrice, myText, myOrdRejReason);
            }
            catch (Exception myE)
            {
                this.SendAdvisoryMessage("updateOrder:" + myE.ToString());
                log.Error("updateOrder:", myE);
            }
        }


        private void sendExecReport(QuickFix.Message myOrder, QuickFix.OrderID myOrderID, QuickFix.OrdStatus myStatus, QuickFix.ExecType myExecType, double myLastQty, int myLeavesQty, int myCumQty, double myLastPx, double myAvePx)
        {
            try
            {
                sendExecReport(myOrder, myOrderID, myStatus, myExecType, myLastQty, myLeavesQty, myCumQty, myLastPx, myAvePx, "", null);
            } 
            catch (Exception myE)
            {
                
            }
        }

        /// <summary>
        /// Create and send an Execution report based on the order passed in and the
        /// exec type (the type of request that is causing the report)
        /// </summary>
        /// <param name="myOrder"></param>
        /// <param name="myExecType"></param>
        private void sendExecReport(QuickFix.Message myOrder, QuickFix.OrderID myOrderID, QuickFix.OrdStatus myStatus, QuickFix.ExecType myExecType, double myLastQty, int myLeavesQty, int myCumQty, double myLastPx, double myAvePx, string myText, QuickFix.OrdRejReason myOrdRejReason)
        {
            try
            {
                string myFixMsg = KaiUtil.QFUtils.GetExecReport(myOrder, myOrderID, myStatus, myExecType, myLastQty, myLeavesQty, myCumQty, myLastPx, myAvePx, myText, myOrdRejReason);


                // send our response message back to the clients of the adapter
                sendResponse("8", myFixMsg);
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("ktacqg:sendExecReport:" + myFixMsg);
                }
            }
            catch (Exception myE)
            {
                log.Error("sendExecReport", myE);
            }
        }

        private void sendCancelRej(QuickFix.Message myReq, int myReason, string myReasonText)
        {
            try
            {
                string myFixMsg = KaiUtil.QFUtils.DoRejectCxlReq(myReq, myReason, myReasonText);
                // send our response message back to the clients of the adapter
                sendResponse("9", myFixMsg);
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("ktacqg:sendCancelRej:" + myFixMsg);
                }
            }
            catch (Exception myE)
            {
                log.Error("sendCancelRej", myE);
            }
        }

        private void sendCancelReplaceRej(QuickFix.Message myReq, int myReason, string myReasonText)
        {
            try
            {
                string myFixMsg = KaiUtil.QFUtils.DoRejectModReq(myReq, myReason, myReasonText);
                // send our response message back to the clients of the adapter
                sendResponse("9", myFixMsg);
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("sendCancelReplaceRej:" + myFixMsg);
                }
            }
            catch (Exception myE)
            {
                log.Error("sendCancelRej", myE);
            }
        }


        

        protected override void DoSend(KaiTrade.Interfaces.Message myMsg)
        {
            lock (m_Token3)
            {
                try
                {
                    // Here are the typical FIX Messages you will receive to
                    // Support Order Routing
                    switch (myMsg.Label)
                    {
                        case "D":
                            submitOrder(myMsg);
                            break;
                        case "F":
                            pullOrder(myMsg);
                            break;
                        case "G":
                            modifyOrder(myMsg);
                            break;
                        case "c":
                            //securityDefRequest(myMsg);
                            break;
                        case "x":
                            //securityListRequest(myMsg);
                            break;
                        case "V":
                            //marketDataRequest(myMsg);
                            break;
                        case "S":
                            //quote(myMsg);
                            break;
                        case "Z":
                            //quoteCancel(myMsg);
                            break;

                        case "AddExecPattern":
                            if (m_ExecutionPatterns.ContainsKey(myMsg.Tag))
                            {
                                m_ExecutionPatterns[myMsg.Tag] = myMsg.Data;
                            }
                            else
                            {
                                m_ExecutionPatterns.Add(myMsg.Tag, myMsg.Data);
                            }
                            break;



                        default:
                            // No Action - Discard Message
                            break;
                    }
                }
                catch (Exception e)
                {
                    log.Error("doSend", e);
                }
            }
        }

        protected override void SubscribeMD(KaiTrade.Interfaces.Publisher pub, int depthLevels, string requestID)
        {
            try
            {
                // this will result in DoReg() being called on CQG thread
                m_CQGHostForm.Register(pub, depthLevels, requestID);
            }
            catch (Exception myE)
            {
                log.Error("SubscribeMD", myE);
            }
        }

        protected override void UnSubscribeMD(KaiTrade.Interfaces.Publisher pub)
        {
            try
            {
                // check if the subject is already registered and add or update
                // the map of subjects we are keeping - this can be used to
                // resubscribe

                string myKey = pub.TopicID();


                // Try and get the product def fromn the interest list
                //if (m_ProductDefs.ContainsKey(myKey))
                //{
                //   KAI.kaitns.InstrDef myDef = m_ProductDefs[myKey] as KAI.kaitns.InstrDef;
                //   unSubscribeProduct(myDef);
                //}
            }
            catch (Exception myE)
            {
                log.Error("UnSubscribeMD", myE);
            }
        }


        public void DoReg(KaiTrade.Interfaces.Publisher myPub, int depthLevels, string requestID)
        {
            try
            {
                // check if the subject is already registered and add or update
                // the map of subjects we are keeping - this can be used to
                // resubscribe

                // Decode the key(mnemonic) into an instrDef, in CQG's
                // case this will be ID and IDSRC

                string myKey = myPub.TopicID();


                // get the current full name for a generic
                if (m_ProductRegister.ContainsKey(myKey))
                {
                    myKey = m_ProductRegister[myKey].SecurityID;

                    // processing for generics - in this case they mnemonic is not the
                    // name of the product CQG uses so we alos need to add the publisher by its
                    // product full name so its correctly resolved on updates
                    if (myKey != myPub.TopicID())
                    {
                        if (!m_PublisherRegister.ContainsKey(myKey))
                        {
                            m_PublisherRegister.Add(myKey, myPub);
                        }
                    }
                }

                if (depthLevels > 0)
                {
                    this.subscribeProductDOM(myKey);
                }
                else
                {
                    // use the src (CQG FullName) to subscribe
                    this.subscribeProduct(myKey);
                }
            }
            catch (Exception myE)
            {
                log.Error("doRegisterSubject", myE);
            }
        }


        protected override void DoUnRegister(KaiTrade.Interfaces.Publisher myPub)
        {
            try
            {
                // check if the subject is already registered and add or update
                // the map of subjects we are keeping - this can be used to
                // resubscribe

                string myKey = myPub.TopicID();


                // Try and get the product def fromn the interest list
                /*
                if (m_ProductDefs.ContainsKey(myKey))
                {
                    KAI.kaitns.InstrDef myDef = m_ProductDefs[myKey] as KAI.kaitns.InstrDef;
                    unSubscribeProduct(myDef);
                }
                 * */
            }
            catch (Exception myE)
            {
                log.Error("doUnRegisterSubject", myE);
            }
        }



        /// <summary>
        /// Send a FIX style response to our clients
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="myResponseMsg"></param>
        private void sendResponse(string msgType, string myResponseMsg)
        {
            try
            {
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("sendResponse:" + msgType + ":" + myResponseMsg);
                }
                // Create message envelope
                KaiTCPComm.KaiMessageWrap myMsg = new KaiTCPComm.KaiMessageWrap();

                // Set the raw FIX Message
                myMsg.Data = myResponseMsg;

                myMsg.Tag = m_ID;
                myMsg.AppSpecific = 0;
                myMsg.Label = msgType;
                myMsg.AppType = "FIX.4.3";
                myMsg.TargetID = "MYTARG";
                myMsg.ClientID = "MYCLIENT";
                myMsg.CreationTime = System.DateTime.Now.ToString();

                // Send the exec report to all clients
                this.SendMessage(myMsg);
            }
            catch (Exception myE)
            {

            }
        }

        /// <summary>
        /// Publish a quote from CQG
        /// </summary>
        /// <param name="instrument"></param>
        private void publishQuote(CQGInstrument instrument)
        {
            try
            {
                publishQuote(instrument.FullName, instrument);
            }
            catch (Exception myE)
            {
                
            }
        }

        #region CQGCEL event handlers

        /// <summary>
        /// This event is fired, when some changes occur in the connection
        /// with the CQG data server.
        /// </summary>
        /// <param name="newStatus"> The current status of the
        /// connection with the Data server.</param>
        private void cel_DataConnectionStatusChanged(eConnectionStatus newStatus)
        {
            try
            {
                wireLog.Info("cel_DataConnectionStatusChanged old:" + m_DataStatus.ToString() + " new=" + newStatus.ToString());
                m_DataStatus = newStatus;

                switch (newStatus)
                {
                    case eConnectionStatus.csConnectionUp:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CQG Data Connection is UP");
                        this.SendAdvisoryMessage("CQG Data Connection is UP");
                        UpdateStatus("Data", "", "", "", KaiTrade.Interfaces.Status.open, "CQG Data Connection is UP");
                        // Subscribe to the products in our interest list
                        subscribeInterestList();
                        m_RunningState.Prices = KaiTrade.Interfaces.StatusConditon.good;
                        m_RunningState.HistoricData = KaiTrade.Interfaces.StatusConditon.good;
                        break;

                    case eConnectionStatus.csConnectionDelayed:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "CQG Data Connection is Delayed");
                        this.SendAdvisoryMessage("CQG Data Connection is Delayed");
                        UpdateStatus("Data", "", "", "", KaiTrade.Interfaces.Status.error, "CQG Data Connection is Delayed");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        m_RunningState.Prices = KaiTrade.Interfaces.StatusConditon.alert;
                        m_RunningState.HistoricData = KaiTrade.Interfaces.StatusConditon.alert;
                        Facade.RaiseAlert("KTACQG", "CQG Data Connection is Delayed", 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;

                    case eConnectionStatus.csConnectionDown:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.closed, "CQG Data Connection is Down");
                        this.SendAdvisoryMessage("CQG Data Connection is Down");
                        UpdateStatus("Data", "", "", "", KaiTrade.Interfaces.Status.closed, "CQG Data Connection is Down");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        m_RunningState.Prices = KaiTrade.Interfaces.StatusConditon.error;
                        m_RunningState.HistoricData = KaiTrade.Interfaces.StatusConditon.error;
                        Facade.RaiseAlert("KTACQG", "CQG Data Connection is Down", 0, KaiTrade.Interfaces.ErrorLevel.fatal, KaiTrade.Interfaces.FlashMessageType.error);
                        break;

                    default:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "CQG Data Connection unknown connection status: " + newStatus.ToString());
                        UpdateStatus("Data", "", "", "", KaiTrade.Interfaces.Status.error, "CQG Data Connection unknown connection status: " + newStatus.ToString());
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        //SendAdvisoryMessage();
                        m_RunningState.Prices = KaiTrade.Interfaces.StatusConditon.alert;
                        m_RunningState.HistoricData = KaiTrade.Interfaces.StatusConditon.alert;
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("cel_DataConnectionStatusChanged", ex);
            }
        }

        /// <summary>
        /// Get the running status of some driver
        /// compliments the StatusRequest();
        /// </summary>
        public override KaiTrade.Interfaces.IDriverStatus GetRunningStatus()
        {
            // returns none on all states - unless overridden
            return m_RunningState;
        }

        private void setGWStatus(eConnectionStatus newStatus)
        {
            try
            {
                if ((m_GWStatus != eConnectionStatus.csConnectionUp) && (newStatus == eConnectionStatus.csConnectionUp))
                {
                    m_GWStatus = newStatus;
                    // Do any deleyed requests
                    while (m_DelayedProductRequests.Count > 0)
                    {
                        KaiTrade.Interfaces.IProduct product = m_DelayedProductRequests.Pop();
                        m_CQGHostForm.CQGApp.NewInstrument(product.GenericName);
                    }
                }
                else
                {
                    m_GWStatus = newStatus;
                }
            }
            catch (Exception ex)
            {
                log.Error("setGWStatus", ex);
            }
        }

        /// <summary>
        /// This event is fired, when some changes occur in the connection
        /// with the CQG Gateway server.
        /// </summary>
        /// <param name="newStatus"> The current status of the
        /// connection with the Gateway server.</param>
        private void cel_GWConnectionStatusChanged(eConnectionStatus newStatus)
        {
            try
            {
                wireLog.Info("cel_GWConnectionStatusChanged old:" + m_GWStatus.ToString() + " new=" + newStatus.ToString());

                setGWStatus(newStatus);

                switch (newStatus)
                {
                    case eConnectionStatus.csConnectionUp:
                        m_CQGHostForm.CQGApp.AccountSubscriptionLevel = eAccountSubscriptionLevel.aslAccountUpdatesAndOrders;
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CQG GW Connection is UP");
                        this.SendAdvisoryMessage("CQG GW Connection is UP");
                        UpdateStatus("GW", "", "", "", KaiTrade.Interfaces.Status.open, "CQG GW Connection is UP");
                        m_RunningState.OrderRouting = KaiTrade.Interfaces.StatusConditon.good;

                        break;

                    case eConnectionStatus.csConnectionDelayed:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "CQG GW Connection is Delayed");
                        this.SendAdvisoryMessage("CQG GW Connection is Delayed");
                        UpdateStatus("GW", "", "", "", KaiTrade.Interfaces.Status.error, "CQG GW Connection is Delayed");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.error);
                        m_RunningState.OrderRouting = KaiTrade.Interfaces.StatusConditon.alert;
                        Facade.RaiseAlert("KTACQG", "CQG GW Connection is Delayed", 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;

                    case eConnectionStatus.csConnectionDown:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.closed, "CQG GW Connection is Down");
                        this.SendAdvisoryMessage("CQG GW Connection is Down");
                        UpdateStatus("GW", "", "", "", KaiTrade.Interfaces.Status.closed, "CQG GW Connection is Down");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        m_RunningState.OrderRouting = KaiTrade.Interfaces.StatusConditon.error;
                        Facade.RaiseAlert("KTACQG", "CQG GW Connection is Down", 0, KaiTrade.Interfaces.ErrorLevel.fatal, KaiTrade.Interfaces.FlashMessageType.error);
                        break;

                    default:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "CQG GW Connection unknown connection status: " + newStatus.ToString());
                        UpdateStatus("GW", "", "", "", KaiTrade.Interfaces.Status.error, "CQG GW Connection unknown connection status: " + newStatus.ToString());
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("cel_GWConnectionStatusChanged", ex);
            }
        }

        /// <summary>
        /// This event is fired, when CQGCEL detects some abnormal discrepancy between data expected and data received.
        /// </summary>
        /// <param name="obj">Object, in which the error occurred.</param>
        /// <param name="errorDescription">String, describing the error.</param>
        private void cel_DataError(object obj, string errorDescription)
        {
            try
            {
                CQGError cqgErr = obj as CQGError;
                if (cqgErr != null)
                {
                    if (cqgErr.Code == 102)
                    {
                        errorDescription += " Restart the application.";
                    }
                    else if (cqgErr.Code == 125)
                    {
                        errorDescription += " Turn on CQG Client and restart the application.";
                    }
                    m_RunningState.OrderRouting = KaiTrade.Interfaces.StatusConditon.error;
                    m_RunningState.Prices = KaiTrade.Interfaces.StatusConditon.error;
                    m_RunningState.HistoricData = KaiTrade.Interfaces.StatusConditon.error;
                    Facade.RaiseAlert("KTACQG", "CQG DataError" + errorDescription, cqgErr.Code, KaiTrade.Interfaces.ErrorLevel.fatal, KaiTrade.Interfaces.FlashMessageType.error);
                }


                this.SendStatusMessage(KaiTrade.Interfaces.Status.error, errorDescription);
                this.SendAdvisoryMessage(errorDescription);

                //wireLog.Info("cel_DataError:" + errorDescription);
                log.Error("cel_DataError:" + errorDescription);
                m_CQGHostForm.Message = errorDescription;
                m_CQGHostForm.BringToFront();
            }
            catch (Exception myE)
            {
                log.Error("cel_DataError", myE);
            }
        }

        /// <summary>
        /// This event is fired when the account or position information is changed.
        /// </summary>
        /// <param name="change">Account change type which allows to differentiate the occurred changes.</param>
        /// <param name="account">A CQGAccount object representing the account
        /// to which the current change refers.</param>
        /// <param name="position">A CQGPosition object representing the position
        /// to which the current change refers.</param>
        private void cel_AccountChanged(eAccountChangeType change, CQGAccount account, CQGPosition position)
        {
            try
            {
                switch (change)
                {
                    case eAccountChangeType.actAccountChanged:
                        // no action
                        break;
                    case eAccountChangeType.actAccountsReloaded:
                        FillAccountsMap();
                        try
                        {
                            driverLog.Info("cel_AccountChanged:Will requiery orders");
                            foreach (CQGAccount acc in m_CQGHostForm.CQGApp.Accounts)
                            {
                                CQGOrdersQuery oq = m_CQGHostForm.CQGApp.QueryOrders(acc);
                            }
                        }
                        catch
                        {
                        }
                        
                        break;
                    case eAccountChangeType.actPositionChanged:
                    case eAccountChangeType.actPositionAdded:
                        if (position != null)
                        {
                            handlePosition(position);
                        }
                        break;
                    case eAccountChangeType.actPositionsReloaded:
                        if (account != null)
                        {
                            foreach (CQGPosition posn in account.Positions)
                            {
                                handlePosition(posn);
                            }
                        }
                        break;
                    default:
                        driverLog.Error("cel_AccountChanged: Change type not processed:" + change.ToString());
                        break;
                }
                
                
            }
            catch (Exception ex)
            {
                log.Error("cel_AccountChanged", ex);
            }
        }

        List<CQGOrder> qorList = new List<CQGOrder>();

        void CQGApp_OnQueryProgress(CQGOrdersQuery cqg_orders_query, CQGError cqg_error)
        {
            try
            {
                if (cqg_orders_query.Status == eRequestStatus.rsSuccess)
                {
                    foreach (CQGOrder order in cqg_orders_query.Orders)
                    {
                        cqg_orders_query.Orders.AddToLiveOrders();
                        List<KaiTrade.Interfaces.Fill> fills = new List<KaiTrade.Interfaces.Fill>();
                        KaiTrade.Interfaces.OrderData ktOrder = getOrderData(order);
                        if (ktOrder != null)
                        {
                            // try find the order using the GUID
                            if (m_GUID2CQGOrder.ContainsKey(order.GUID))
                            {
                                DriverBase.OrderContext myContext = m_GUID2CQGOrder[order.GUID] as DriverBase.OrderContext;
                                myContext.ExternalOrder = order;
                                qorList.Add(order);
                                //m_CQGHostForm.CQGApp.Orders.AddToLiveOrders(order);

                                int x = m_CQGHostForm.CQGApp.Orders.Count;

                                // try get the order
                                KaiTrade.Interfaces.Order o = Facade.Factory.GetOrderManager().GetOrderWithClOrdIDID(myContext.ClOrdID);
                                if (o != null)
                                {
                                    ktOrder.Identity = o.Identity;
                                    ktOrder.StrategyName = o.StrategyName;
                                    ktOrder.Mnemonic = o.Mnemonic; 
                                    // update any key order details
                                    Facade.UpdateOrderInformation(ktOrder, fills);
                                }
                                bool bc = order.CanBeCanceled;
                                //order.Cancel();
                                // we know this order - so we can do an regular update
                                updateOrder(eChangeType.ctChanged, order);
                            }
                            else
                            {
                                // we dont know this order - we can record the details
                                Facade.UpdateOrderInformation(ktOrder, fills);
                            }
                        }
                         
                    }
                }

            }
            catch (Exception myE)
            {
                log.Error("CQGApp_OnQueryProgress", myE);
            }
        }




        private void handlePosition(CQGPosition position)
        {
            try
            {
                
                KaiTrade.Interfaces.IPosition posn = new K2DataObjects.Position();
                posn.AccountCode = position.Account.GWAccountName;
                posn.Mnemonic = position.InstrumentName;
                posn.AvgPrice = (decimal)position.AveragePrice;
                posn.MVO = (decimal)position.MVO;
                posn.OTE = (decimal)position.OTE;
                posn.PnL = (decimal)position.ProfitLoss;
                posn.Quantity = position.Quantity;
                if (position.Quantity < 0)
                {
                    posn.Side = KaiTrade.Interfaces.Side.SELL;
                }
                else
                {
                    posn.Side = KaiTrade.Interfaces.Side.BUY;
                }
                
                posn.UpdateTime = DateTime.Now;

                Facade.ProcessPositionUpdate(posn);

                //string info = posn.ToString();
                //driverLog.Info("handlePosition:" + info);
            }
            catch  (Exception ex)
            {
                log.Error("handlePosition", ex);
            }
             
        }

        /// <summary>
        /// publish a quote for the given mnemonic
        /// </summary>
        /// <param name="myMnemonic">mnemonic/subject key</param>
        /// <param name="instrument">CQG instrument that has changed</param>
        private void publishTradeUpdate(string myMnemonic, CQGInstrument instrument, CQGQuote quote)
        {
            try
            {
                KaiTrade.TradeObjects.PXUpdateBase pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                pxupdate.Mnemonic = myMnemonic;
                pxupdate.DriverTag = "T";
                pxupdate.UpdateType = KaiTrade.Interfaces.PXUpdateType.trade;
                if (instrument.Bid.IsValid)
                {
                    pxupdate.BidSize = instrument.Bid.Volume;
                    pxupdate.BidPrice = (decimal)instrument.Bid.Price;
                    pxupdate.Ticks = instrument.Bid.ServerTimestamp.Ticks;
                }
                if (instrument.Ask.IsValid)
                {
                    pxupdate.OfferSize = instrument.Ask.Volume;
                    pxupdate.OfferPrice = (decimal)instrument.Ask.Price;
                    pxupdate.Ticks = instrument.Ask.ServerTimestamp.Ticks;
                }

                pxupdate.TradePrice = (decimal)quote.Price;
                pxupdate.ServerTicks = quote.ServerTimestamp.Ticks;

                if (quote.HasVolume)
                {
                    pxupdate.TradeVolume = quote.Volume;
                }

                ApplyPriceUpdate(pxupdate);
            }
            catch (Exception myE)
            {
                log.Error("publishTradeUpdate", myE);
            }
        }

        /// <summary>
        /// publish a quote for the given mnemonic
        /// </summary>
        /// <param name="myMnemonic">mnemonic/subject key</param>
        /// <param name="instrument">CQG instrument that has changed</param>
        private void publishQuote(string myMnemonic, CQGInstrument instrument)
        {
            try
            {
                KaiTrade.TradeObjects.PXUpdateBase pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                pxupdate.Mnemonic = myMnemonic;

                if (instrument.Bid.IsValid)
                {
                    pxupdate.BidSize = instrument.Bid.Volume;
                    pxupdate.BidPrice = (decimal)instrument.Bid.Price;
                    pxupdate.Ticks = instrument.Bid.ServerTimestamp.Ticks;
                }
                if (instrument.Ask.IsValid)
                {
                    pxupdate.OfferSize = instrument.Ask.Volume;
                    pxupdate.OfferPrice = (decimal)instrument.Ask.Price;
                    pxupdate.Ticks = instrument.Ask.ServerTimestamp.Ticks;
                }
                if (instrument.Trade.IsValid)
                {
                    switch (instrument.Trade.Type)
                    {
                        /*
                    case eQuoteType.qtTrade:
                        pxupdate.TradePrice = (decimal)instrument.Trade.Price;
                        pxupdate.TradeVolume = instrument.Trade.Volume;
                        pxupdate.LastTradeTicks = instrument.Trade.ServerTimestamp.Ticks;

                        break;
                         */
                        case eQuoteType.qtDayHigh:
                            pxupdate.DayHigh = (decimal)instrument.Trade.Price;
                            pxupdate.TradePrice = 0;
                            pxupdate.TradeVolume = 0;
                            break;
                        case eQuoteType.qtDayLow:
                            pxupdate.DayLow = (decimal)instrument.Trade.Price;
                            pxupdate.TradePrice = 0;
                            pxupdate.TradeVolume = 0;
                            break;
                        case eQuoteType.qtDayOpen:
                            //myPub.Open = instrument.Trade.Price.ToString();
                            break;
                        default:
                            pxupdate.TradePrice = null;
                            pxupdate.TradeVolume = null;
                            break;
                    }
                    pxupdate.Ticks = instrument.Timestamp.Ticks;
                }
                else
                {
                    pxupdate.TradePrice = 0;
                    pxupdate.TradeVolume = 0;
                }

                ApplyPriceUpdate(pxupdate);
            }
            catch (Exception myE)
            {
                log.Error("publishQuote", myE);
            }
        }

        /// <summary>
        /// This event is fired as a response to a Tradable Commodities request either when the
        /// collection is resolved or when an error has occurred during the request processing.
        /// </summary>
        /// <param name="GWAccountID">
        /// Gateway account ID, for which tradable commodity names were requested, represented as Long.
        /// </param>
        /// <param name="commodityNames">
        /// Collection of the tradable commodities.
        /// </param>
        /// <param name="cqgErr">
        /// CQGError object representing the error occurred while processing the time series request.
        /// </param>
        private void CEL_TradableCommoditiesResolved(int GWAccountID, CQG.CQGCommodities commodityNames, CQG.CQGError cqgErr)
        {
            try
            {
                // Raise error, when an error occurs during the request.
                if (cqgErr != null)
                {
                    string myError = "Error " + cqgErr.Code + ": " + cqgErr.Description;
                    throw new Exception(myError);
                }

                // just do futures for now
                eInstrumentType myType = eInstrumentType.itFuture;
                for (int l = 0; l <= commodityNames.Count - 1; l++)
                {
                    m_CQGHostForm.CQGApp.RequestCommodityInstruments(commodityNames[l], myType, true);
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_TradableCommoditiesResolved", myE);
            }
        }

        /// <summary>
        /// This event is fired when a previously requested commodity symbol
        /// is resolved into the instruments list.
        /// this is where we will send a security definition back to our clients
        /// </summary>
        /// <param name="commodityName">
        /// The commodity symbol that was requested by the user in the
        /// corresponding RequestCommodityInstruments method call.
        /// </param>
        /// <param name="instrumentTypes">
        /// Instrument type bitmask that was set by the user in the
        /// corresponding RequestCommodityInstruments method call.
        /// </param>
        /// <param name="instrumentNames">
        /// Collection, which contains names of instruments corresponding to the requested
        /// commodity and filtered by the bitmask set in RequestCommodityInstruments Method.
        /// </param>
        private void CEL_CommodityInstrumentsResolved(string commodityName, eInstrumentType instrumentTypes, CQGCommodityInstruments instrumentNames)
        {
            /*
                     * Dont use this as we handle the list
            try
            {
                // set up a security def message
                QuickFix.SecurityReqID mySecReqID;
                QuickFix.SecurityResponseID mySecurityResponseID;
                QuickFix.SecurityResponseType mySecurityResponseType;

                QuickFix43.SecurityDefinition mySecDef;

                QuickFix.Message myMsg;

                // get teh CFI code
                string myCFICode = getCFI(instrumentTypes);

                for (int l = 0; l <= instrumentNames.Count - 1; l++)
                {
                    // set up a security def message
                    mySecReqID = new QuickFix.SecurityReqID(m_ID);
                    mySecurityResponseID = new QuickFix.SecurityResponseID(KaiUtil.Identities.Instance.genReqID());
                    mySecurityResponseType = new QuickFix.SecurityResponseType(QuickFix.SecurityResponseType.LIST_OF_SECURITIES_RETURNED_PER_REQUEST);

                    mySecDef = new QuickFix43.SecurityDefinition(mySecReqID, mySecurityResponseID, mySecurityResponseType);

                    myMsg = mySecDef as QuickFix.Message;


                    KaiUtil.QFUtils.Instance().SetupInstrument(ref myMsg, "FIX.4.3", commodityName, "",
                        instrumentNames[l], "CQG", "XXX", myCFICode, "", 0.0);



                    // send our response message back to the clients of the adapter
                    this.sendResponse("d", mySecDef.ToString());

                    m_CQGHostForm.Message = "Product resolved:" + instrumentNames[l];
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_TradableCommoditiesResolved", myE);
            }
             *  */
        }

        /// <summary>
        /// Fired when the collection of timed bars (CQGTimedBars) is resolved or
        /// when some error occured during timed bars request processing.
        /// </summary>
        /// <param name="cqg_timed_bars">
        /// Reference to resolved CQGTimedBars
        /// </param>
        /// <param name="cqg_error">
        /// The CQGError object that describes the last error occurred
        /// while processing the TimedBars request or
        /// Nothing/Invalid_Error in case of no error.
        /// </param>
        private void CEL_TimedBarsResolved(CQG.CQGTimedBars cqg_timed_bars, CQG.CQGError cqg_error)
        {
            try
            {
                lock (m_BarToken1)
                {
                    try
                    {
                        driverLog.Info("CEL_TimedBarsResolved:" + cqg_timed_bars.Id.ToString());
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_timed_bars.Id))
                        {
                            mySet = m_TSSets[cqg_timed_bars.Id];

                            if (wireLog.IsInfoEnabled)
                            {
                                wireLog.Info("CEL_TimedBarsResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                            }

                            if (cqg_timed_bars.Status == eRequestStatus.rsSuccess)
                            {
                                mySet.Status = KaiTrade.Interfaces.Status.open;
                                DumpAllData(cqg_timed_bars, mySet);
                            }
                            else
                            {
                                mySet.Status = KaiTrade.Interfaces.Status.error;
                                mySet.Text = cqg_error.Description;
                                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CEL_TimedBarsResolved" + mySet.Text);
                            }
                        }
                        else
                        {
                            driverLog.Info("CEL_TimedBarsResolved:Dataset not found");
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_TimedBarsResolved", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Fired when CQGTimedBar item is added to the end of CQGTimedBars.
        /// </summary>
        /// <param name="cqg_timed_bars">
        /// Reference to changed CQGTimedBars.
        /// </param>
        private void CEL_TimedBarsAdded(CQG.CQGTimedBars cqg_timed_bars)
        {
            try
            {
                lock (m_BarToken1)
                {
                    try
                    {
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_timed_bars.Id))
                        {
                            mySet = m_TSSets[cqg_timed_bars.Id];
                            //mySet.Items.Clear();  Done in the dump all
                            if (cqg_timed_bars.Status == eRequestStatus.rsSuccess)
                            {
                                DumpAllData(cqg_timed_bars, mySet);
                            }
                            else
                            {
                                mySet.Text = cqg_timed_bars.LastError.Description;
                            }
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_TimedBarsAdded", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Fired when CQGTimedBar item is updated.
        /// </summary>
        /// <param name="cqg_timed_bars">
        /// Reference to changed CQGTimedBars
        /// </param>
        /// <param name="index_">
        /// Specifies the updated CQGTimedBar index.
        /// </param>
        private void CEL_TimedBarsUpdated(CQG.CQGTimedBars cqg_timed_bars, int index_)
        {
            try
            {
                lock (m_BarToken1)
                {
                    try
                    {
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        //return;
                        if (m_TSSets.ContainsKey(cqg_timed_bars.Id))
                        {
                            mySet = m_TSSets[cqg_timed_bars.Id];
                            if (!mySet.ReportAll)
                            {
                                // they only want added bars - so exit
                                return;
                            }
                            KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();

                            DumpRecord(myItem, mySet, cqg_timed_bars[index_], index_);
                            myItem.SourceActionType = KaiTrade.Interfaces.TSItemSourceActionType.barUpdated;
                            myItem.DriverChangedData = true;
                            mySet.ReplaceItem(myItem, index_);
                            mySet.Changed = true;
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_TimedBarsUpdated", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Dumps all request data (outputs\parameters\records)
        /// </summary>
        private void DumpAllData(CQG.CQGTimedBars myBars, KaiTrade.Interfaces.ITSSet mySet)
        {
            try
            {
                int exitingItemCount = mySet.Items.Count;
                // Clears all records
                mySet.Items.Clear();

                if (myBars.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < myBars.Count; i++)
                {
                    // get a new TS item
                    KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                    DumpRecord(myItem, mySet, myBars[i], i);
                    myItem.SourceActionType = KaiTrade.Interfaces.TSItemSourceActionType.barAdded;
                    myItem.ItemType = KaiTrade.Interfaces.TSItemType.time;
                    if (i >= exitingItemCount)
                    {
                        myItem.DriverChangedData = true;
                    }
                    else if (i >= myBars.Count - 2)
                    {
                        myItem.DriverChangedData = true;
                    }
                    else
                    {
                        myItem.DriverChangedData = false;
                    }


                    mySet.AddItem(myItem);
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("TBADD:" + myItem.ToTabSeparated());
                    }
                }
                

                // mark the set as changed - will cause the publisher
                // to do an update or image
                mySet.Added = true;
            }
            catch (Exception myE)
            {
                log.Error("DumpAllData", myE);
            }
        }



        private void DumpRecord(KaiTrade.Interfaces.TSItem myItem, KaiTrade.Interfaces.ITSSet mySet, CQGTimedBar myBar, long myIndex)
        {
            try
            {
                // get a new TS item
                myItem.ItemType = KaiTrade.Interfaces.TSItemType.time;
                myItem.Index = myIndex;
                myItem.TimeStamp = myBar.Timestamp;
                myItem.Open = myBar.Open;
                myItem.High = myBar.High;
                myItem.Low = myBar.Low;
                myItem.Close = myBar.Close;
                myItem.Mid = myBar.Mid;
                myItem.HLC3 = myBar.HLC3;
                myItem.Avg = myBar.Avg;
                myItem.TrueHigh = myBar.TrueHigh;
                myItem.TrueLow = myBar.TrueLow;
                myItem.Range = myBar.Range;
                myItem.TrueRange = myBar.TrueRange;
                myItem.AskVolume = myBar.AskVolume;
                myItem.BidVolume = myBar.BidVolume;
                myItem.Volume = myBar.ActualVolume;
                
            }
            catch (Exception myE)
            {
                log.Error("DumpRecord", myE);
            }
        }

        /// <summary>
        /// Dumps all request data (outputs\parameters\records)
        /// </summary>
        private void DumpAllData(CQG.CQGConstantVolumeBars myBars, KaiTrade.Interfaces.ITSSet mySet)
        {
            try
            {
                // Clears all records
                mySet.Items.Clear();

                if (myBars.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < myBars.Count; i++)
                {
                    // get a new TS item
                    KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                    DumpRecord(myItem, mySet, myBars[i], i);
                    myItem.SourceActionType = KaiTrade.Interfaces.TSItemSourceActionType.barAdded;
                    myItem.ItemType = KaiTrade.Interfaces.TSItemType.constantVolume;
                    mySet.AddItem(myItem);
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("TBADD:" + myItem.ToTabSeparated());
                    }
                }

                // mark the set as changed - will cause the publisher
                // to do an update or image
                mySet.Added = true;
            }
            catch (Exception myE)
            {
                log.Error("DumpAllData", myE);
            }
        }

        private void DumpRecord(KaiTrade.Interfaces.TSItem myItem, KaiTrade.Interfaces.ITSSet mySet, CQGConstantVolumeBar myBar, long myIndex)
        {
            try
            {
                // get a new TS item
                myItem.Index = myIndex;
                myItem.TimeStamp = myBar.Timestamp;
                myItem.Open = myBar.Open;
                myItem.High = myBar.High;
                myItem.Low = myBar.Low;
                myItem.Close = myBar.Close;
                myItem.Mid = myBar.Mid;
                myItem.HLC3 = myBar.HLC3;
                myItem.Avg = myBar.Avg;
                myItem.TrueHigh = myBar.TrueHigh;
                myItem.TrueLow = myBar.TrueLow;
                myItem.Range = myBar.Range;
                myItem.TrueRange = myBar.TrueRange;
                myItem.AskVolume = myBar.AskVolume;
                myItem.BidVolume = myBar.BidVolume;
            }
            catch (Exception myE)
            {
                log.Error("DumpRecord:CV", myE);
            }
        }

        /// <summary>
        /// Fired when the collection of constant volume bars (CQGConstantVolumeBars) is
        /// resolved or when some error has occurred during the constant volume bars request processing.
        /// </summary>
        /// <param name="cqg_constant_volume_bars">
        /// Reference to resolved CQGConstantVolumeBars
        /// </param>
        /// <param name="cqg_error">
        /// The CQGError object that describes the last error occurred
        /// while processing the ConstantVolumeBars request or
        /// Nothing/Invalid_Error in case of no error.
        /// </param>
        private void CEL_ConstantVolumeBarsResolved(CQG.CQGConstantVolumeBars cqg_constant_volume_bars, CQG.CQGError cqg_error)
        {
            try
            {
                lock (m_Token4)
                {
                    try
                    {
                        driverLog.Info("CEL_ConstantVolumeBarsResolved:" + cqg_constant_volume_bars.Id.ToString());
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_constant_volume_bars.Id))
                        {
                            mySet = m_TSSets[cqg_constant_volume_bars.Id];

                            if (wireLog.IsInfoEnabled)
                            {
                                wireLog.Info("CEL_ConstantVolumeBarsResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                            }

                            if (cqg_constant_volume_bars.Status == eRequestStatus.rsSuccess)
                            {
                                mySet.Status = KaiTrade.Interfaces.Status.open;
                                DumpAllData(cqg_constant_volume_bars, mySet);
                            }
                            else
                            {
                                mySet.Status = KaiTrade.Interfaces.Status.error;
                                mySet.Text = cqg_error.Description;
                                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CEL_ConstantVolumeBarsResolved" + mySet.Text);
                            }
                        }
                        else
                        {
                            driverLog.Info("CEL_ConstantVolumeBarsResolved:Dataset not found");
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_ConstantVolumeBarsResolved", myE);
                    }
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// Fired when CQGConstantVolumeBar item is added to the end of CQGConstantVolumeBars.
        /// </summary>
        /// <param name="cqg_constant_volume_bars">
        /// Reference to changed CQGConstantVolumeBars.
        /// </param>
        private void CEL_ConstantVolumeBarsAdded(CQG.CQGConstantVolumeBars cqg_constant_volume_bars)
        {
            try
            {
                lock (m_Token4)
                {
                    try
                    {
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_constant_volume_bars.Id))
                        {
                            mySet = m_TSSets[cqg_constant_volume_bars.Id];
                            mySet.Items.Clear();
                            if (cqg_constant_volume_bars.Status == eRequestStatus.rsSuccess)
                            {
                                DumpAllData(cqg_constant_volume_bars, mySet);
                            }
                            else
                            {
                                mySet.Text = cqg_constant_volume_bars.LastError.Description;
                            }
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_ConstantVolumeBarsAdded", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Fired when CQGConstantVolumeBar item is updated.
        /// </summary>
        /// <param name="cqg_constant_volume_bars">
        /// Reference to changed CQGConstantVolumeBars
        /// </param>
        /// <param name="index_">
        /// Specifies the updated CQGConstantVolumeBar index.
        /// </param>
        private void CEL_ConstantVolumeBarsUpdated(CQG.CQGConstantVolumeBars cqg_constant_volume_bars, int index_)
        {
            try
            {
                lock (m_Token4)
                {
                    try
                    {
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        //return;
                        if (m_TSSets.ContainsKey(cqg_constant_volume_bars.Id))
                        {
                            mySet = m_TSSets[cqg_constant_volume_bars.Id];
                            if (!mySet.ReportAll)
                            {
                                // they only want added bars - so exit
                                return;
                            }
                            KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();

                            DumpRecord(myItem, mySet, cqg_constant_volume_bars[index_], index_);

                            mySet.ReplaceItem(myItem, index_);
                            mySet.Changed = true;
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_ConstantVolumeBarsUpdated", myE);
                    }
                }
            }
            catch
            {
            }
        }





        /// <summary>
        /// Fired when the condition (CQGCondition) is resolved or when some
        /// error has occurred during the condition request processing.
        /// </summary>
        /// <param name="cqg_condition">
        /// Reference to resolved CQGCondition
        /// </param>
        /// <param name="cqg_error">
        /// The CQGError object that describes the last error occurred
        /// while processing the Condition request or
        /// Nothing/Invalid_Error in case of no error.
        /// </param>
        private void CEL_ConditionResolved(CQG.CQGCondition myCondition, CQG.CQGError cqg_error)
        {
            try
            {
                driverLog.Info("CEL_ConditionResolved:" + myCondition.Id.ToString());
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(myCondition.Id))
                {
                    mySet = m_TSSets[myCondition.Id];

                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("CEL_ConditionResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (driverLog.IsInfoEnabled)
                    {
                        driverLog.Info("CEL_ConditionResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                     
                    if (myCondition.Status == eRequestStatus.rsSuccess)
                    {
                        // the last bar is the current bar, the index N has occured before N+1
                        int currentBar = myCondition.Count - 1;
                        Facade.ProcessCondition(mySet, mySet.ConditionName, myCondition[currentBar].Timestamp, myCondition[currentBar].Value);

                        /*
                        // Clears all records
                        mySet.Items.Clear();
                        mySet.Status = KaiTrade.Interfaces.Status.open;

                        if (myCondition.Count == 0)
                        {
                            return;
                        }


                        for (int i = 0; i < myCondition.Count; i++)
                        {
                            // get a new TS item
                            KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                            myItem.Index = i;
                            myItem.TimeStamp = myCondition[i].Timestamp;
                            myItem.ConditionName = mySet.ConditionName;
                            myItem.ConditionValue = myCondition[i].Value;
                            mySet.AddItem(myItem);
                        }
                        mySet.Added = true;
                         */
                    }
                    else
                    {
                        mySet.Status = KaiTrade.Interfaces.Status.error;
                        mySet.Text = cqg_error.Description;
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CEL_ConditionResolved" + mySet.Text);
                    }
                }
                else
                {
                    driverLog.Info("CEL_ConditionResolved:TSDAta set not found");
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionResolved", myE);
            }
        }

        /// <summary>
        /// fired when a set of conditions are received
        /// </summary>
        /// <param name="myConditions"></param>
        /// <param name="cqg_error"></param>
        private void CEL_ConditionDefinitionsResolved(CQG.CQGConditionDefinitions myConditions, CQG.CQGError cqg_error)
        {
            try
            {
                log.Info("CEL_ConditionDefinitionsResolved:entered");
                foreach (CQG.CQGConditionDefinition myCond in myConditions)
                {
                    //myCond.
                    KaiTrade.TradeObjects.TradingSystem tradeSystem = new KaiTrade.TradeObjects.TradingSystem();

                    // this system will be just based on a condition
                    tradeSystem.TradeSystemBasis = KaiTrade.Interfaces.eTradeSystemBasis.simpleCondition;

                    tradeSystem.Name = myCond.Name;

                    copyTSDefParameters(tradeSystem, myCond.ParameterDefinitions);

                    this.Facade.AddReplaceTradeSystem(tradeSystem);
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionDefinitionsResolved", myE);
            }
            
        }

        /// <summary>
        /// Fired when some condition changes
        /// </summary>
        /// <param name="cqg_condition"></param>
        /// <param name="index_"></param>
        private void CEL_ConditionUpdated(CQGCondition myCondition, int index_)
        {
            try
            {
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(myCondition.Id))
                {
                    mySet = m_TSSets[myCondition.Id];
                    if (!mySet.ReportAll)
                    {
                        // they only want added bars - so exit
                        return;
                    }
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("CEL_ConditionUpdated:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (driverLog.IsInfoEnabled)
                    {
                        driverLog.Info("CEL_ConditionUpdated:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (myCondition.Status == eRequestStatus.rsSuccess)
                    {
                        if (myCondition.Count == 0)
                        {
                            return;
                        }

                        // the last bar is the current bar, the index N has occured before N+1
                        int currentBar = myCondition.Count - 1;
                        Facade.ProcessCondition(mySet, mySet.ConditionName, myCondition[index_].Timestamp, myCondition[index_].Value);
                        /*
                        // get a new TS item
                        KaiTrade.Interfaces.TSItem myItem = null;
                        if (mySet.Items.Count > 0)
                        {
                            myItem = mySet.GetItem(index_);
                        }
                        else
                        {
                            myItem = mySet.GetNewItem();
                        }
                        myItem.Index = index_;
                        myItem.TimeStamp = myCondition[index_].Timestamp;
                        myItem.ConditionName = mySet.ConditionName;
                        string myDump = "CEL_CONDUPD";



                        //wireLog.Info(myDump);
                        if (myItem.ConditionValue != myCondition[index_].Value)
                        {
                            myItem.ConditionValue = myCondition[index_].Value;
                            mySet.ReplaceItem(myItem, index_);
                            myDump += "|" + myItem.ConditionValue;
                            mySet.Changed = true;
                        }
                        driverLog.Info(myDump);
                         */
                    }
                    else
                    {
                        mySet.Text = myCondition.LastError.Description;
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionUpdated", myE);
            }
        }

        /// <summary>
        /// Fired when some condition changes
        /// </summary>
        /// <param name="cqg_condition"></param>
        /// <param name="index_"></param>
        private void CEL_ConditionAdded(CQGCondition myCondition)
        {
            try
            {
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(myCondition.Id))
                {
                    mySet = m_TSSets[myCondition.Id];
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("CEL_ConditionAdded:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (driverLog.IsInfoEnabled)
                    {
                        driverLog.Info("CEL_ConditionAdded:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (myCondition.Status == eRequestStatus.rsSuccess)
                    {
                        // the last bar is the current bar, the index N has occured before N+1
                        int currentBar = myCondition.Count - 1;
                        Facade.ProcessCondition(mySet, mySet.ConditionName, myCondition[currentBar].Timestamp, myCondition[currentBar].Value);
                        /*
                        // Clears all records
                        mySet.Items.Clear();

                        if (myCondition.Count == 0)
                        {
                            return;
                        }
                        string myDump = "CEL_CONDADD";
                        // add any new conditons
                        for (int i = mySet.Items.Count; i < myCondition.Count; i++)
                        {
                            // get a new TS item
                            KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                            myItem.Index = i;
                            myItem.TimeStamp = myCondition[i].Timestamp;
                            myItem.ConditionName = mySet.ConditionName;
                            myItem.ConditionValue = myCondition[i].Value;
                            mySet.AddItem(myItem);
                            myDump += "|" + myCondition[i].Value + "|" + myCondition.Definition.Name;
                        }
                        mySet.Added = true;
                        if (wireLog.IsInfoEnabled)
                        {
                            wireLog.Info(myDump);
                        }
                        if (driverLog.IsInfoEnabled)
                        {
                            driverLog.Info(myDump);
                        }
                        */
                    }
                    else
                    {
                        mySet.Text = myCondition.LastError.Description;
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionAdded", myE);
            }
        }


        void CQGApp_TradingSystemDefinitionsResolved(CQGTradingSystemDefinitions trdSysDefs, CQGError cqg_error)
        {
            try
            {
                log.Info("CQGApp_TradingSystemDefinitionsResolved:entered");
                foreach (CQGTradingSystemDefinition cqgTrdSystem in trdSysDefs)
                {
                    KaiTrade.TradeObjects.TradingSystem tradeSystem = new KaiTrade.TradeObjects.TradingSystem();
                    tradeSystem.Name = cqgTrdSystem.Name;
                    tradeSystem.TradeSystemBasis = KaiTrade.Interfaces.eTradeSystemBasis.tradeSystem;

                    copyTSDefParameters(tradeSystem, cqgTrdSystem.ParameterDefinitions);

                    foreach (CQGTradingSystemTradeDefinition cqgTradeDef in cqgTrdSystem.TradeDefinitions)
                    {
                        KaiTrade.TradeObjects.SystemTrade systemTrade = new KaiTrade.TradeObjects.SystemTrade();
                        systemTrade.TradeName = cqgTradeDef.Name;
                        systemTrade.Side = getKTAOrderSide(cqgTradeDef.Side);
                        systemTrade.OrdType = getKTAOrderType(cqgTradeDef.Entry.OrderType);

                        foreach (CQGTradeExitDefinition cqgExitTrade in cqgTradeDef.Exits)
                        {
                            KaiTrade.TradeObjects.ExitTrade exitTrade = new KaiTrade.TradeObjects.ExitTrade();
                            exitTrade.ExitTradeName = cqgExitTrade.Name;
                            exitTrade.OrdType = getKTAOrderType(cqgExitTrade.OrderType);
                            exitTrade.Side = "";
                            systemTrade.ExitTrades.Add(exitTrade);
                        }
                        tradeSystem.AddSystemTrade(systemTrade);
                    }

                    this.Facade.AddReplaceTradeSystem(tradeSystem);
                }
            }
            catch (Exception myE)
            {
                log.Error("CQGApp_TradingSystemDefinitionsResolved", myE);
            }
        }

        private void copyTSDefParameters(KaiTrade.TradeObjects.TradingSystem tradeSystem, CQGParameterDefinitions  parameterDefintions)
        {
            try
            {
                log.Info("copyTSDefParameters");
                foreach (CQGParameterDefinition cqgParm in parameterDefintions)
                {
                    KaiTrade.Interfaces.IParameter parm = new K2DataObjects.K2Parameter(cqgParm.Name, (cqgParm.DefaultValue as object).ToString());
                    switch (cqgParm.Type)
                    {
                        case eUserFormulaParameterType.ufptDouble:
                            parm.ParameterType = KaiTrade.Interfaces.ATDLType.Float;
                            break;
                        case eUserFormulaParameterType.ufptInt:
                            parm.ParameterType = KaiTrade.Interfaces.ATDLType.Int;
                            break;
               
                        default:
                            parm.ParameterType = KaiTrade.Interfaces.ATDLType.String;
                            break;
                    }
                    tradeSystem.Parameters.Add(parm);

                }

            }
            catch (Exception myE)
            {
                log.Error("copyTSDefParameters", myE);
            }
        }

        void CQGApp_TradingSystemInsertNotification(CQGTradingSystem cqg_trading_system, CQGTradingSystemInsertInfo cqg_trading_system_insert_info)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {
                        //return;
                        // looks like you have to read the sodding trading system - this just tells you it happened
                        driverLog.Info("CQGApp_TradingSystemInsertNotification:" + cqg_trading_system.Id);
                        //logTradingSystem(cqg_trading_system);
                        //TradingSystemInsert(cqg_trading_system, cqg_trading_system_insert_info);
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemUpdateNotification", myE);
                    }
                }
            }
            catch
            {
            }
        }

        void CQGApp_TradingSystemUpdateNotification(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqg_trading_system_update_info)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {
                        if (wireLog.IsInfoEnabled)
                        {
                            wireLog.Info("CQGApp_TradingSystemUpdateNotification:" + cqg_trading_system.Id);
                            //logTradingSystem(cqg_trading_system);
                            LogCQGTSUdateInfo(cqg_trading_system, cqg_trading_system_update_info);
                        }
                        TradingSystemImage(cqg_trading_system);

                        //TradingSystemUpdate(cqg_trading_system, cqg_trading_system_update_info);
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemUpdateNotification", myE);
                    }
                }
            }
            catch
            {
            }
        }

        void CQGApp_TradingSystemAddNotification(CQGTradingSystem cqg_trading_system, CQGTradingSystemAddInfo cqg_trading_system_add_info)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {
                        
                        driverLog.Info("CQGApp_TradingSystemAddNotification:" + cqg_trading_system.Id);
                        if (wireLog.IsInfoEnabled)
                        {
                            //logTradingSystem(cqg_trading_system);
                        }


                        TradingSystemImage(cqg_trading_system);
                        //TradingSystemAdd(cqg_trading_system, cqg_trading_system_add_info);
                        //TradingSystemUpdate(cqg_trading_system, cqg_trading_system_add_info);
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemAddNotification", myE);
                    }
                }
            }
            catch
            {
            }
        }



        private void logTradingSystem(CQGTradingSystem cqgts)
        {
            try
            {
                string myTemp = string.Format("Log CQGTradingSystem>>> ID={0}  Status={1} numRows={2}", cqgts.Id, cqgts.Status.ToString(), cqgts.TradesRows.Count);
                wireLog.Info(myTemp);


                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqgts.Id))
                {
                    //mySet = m_TSSets[cqgts.Id];
                    if (cqgts.Status == eRequestStatus.rsSuccess)
                    {
                        mySet = m_TSSets[cqgts.Id];
                        myTemp = string.Format("CQGTradingSystem>>> Set found Alias = {0} numItems = {1}", mySet.Alias, mySet.Items.Count);
                        wireLog.Info(myTemp);

                        
                        CQGTradingSystemTradesRows myTradeRows = cqgts.TradesRows;

                        if (myTradeRows.Count == 0)
                        {
                            myTemp = string.Format("CQGTradingSystem>>> No Trade Rows in ts ID={0}", cqgts.Id);
                            wireLog.Info(myTemp);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            logCQGTradeRow(i, myTradeRow);
                        }
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemImage:TradingSystemImage:TSData not found");
                }
                wireLog.Info(">>>>>>>>>End of trade system log<<<<<<<<<<<<<<<");
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemImage", myE);
            }
        }

        private bool m_inTradingSystemImage = false;

        private long getTradeSystemMarkerTag(DateTime timeStamp, int offset)
        {
            //yyyy mm dd hh mm
            long x = offset;
            x+= timeStamp.Minute*100;
            x += timeStamp.Hour * 60 * 100;
            x += timeStamp.Day * 24 * 60 * 100;
            x += timeStamp.Month * 32 * 24 * 60 * 100;
            x += (timeStamp.Year/100) * 12 * 32 * 24 * 60 * 100;
            return x;
        }

       

        private void TradingSystemImage2(CQGTradingSystem cqg_trading_system)
        {
            try
            {
                if (m_inTradingSystemImage)
                {
                    // server error
                    throw new Exception("re-entered TradingSystemImage2");
                }
                else
                {
                    m_inTradingSystemImage = true;
                }

                wireLog.Info("TradingSystemImage2: " + cqg_trading_system.Id + " count:" + cqg_trading_system.TradesRows.Count.ToString());

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    mySet.Status = KaiTrade.Interfaces.Status.open;

                    if (cqg_trading_system.Status == eRequestStatus.rsSuccess)
                    {
                        List<KaiTrade.Interfaces.TradeSignal> signals = new List<KaiTrade.Interfaces.TradeSignal>();
                        // Clears all records
                        // We use the tag to track the last time+cqg offset we processed
                        if (mySet.Items.Count > 0)
                        {
                            mySet.Tag = mySet.CurrentItem.Tag;
                        }
                        else
                        {
                            mySet.Tag = getTradeSystemMarkerTag(DateTime.Now, 0);
                            //mySet.Tag = (long)0;
                        }
                        mySet.Items.Clear();
                        mySet.Status = KaiTrade.Interfaces.Status.open;

                        CQGTradingSystemTradesRows myTradeRows = cqg_trading_system.TradesRows;
                        //cqg_trading_system.TradesRows[0][0].Definition.Exits[0]
                        if (myTradeRows.Count == 0)
                        {
                            wireLog.Info("TradingSystemImage2: No Rows - just return " + cqg_trading_system.Id);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            appendCQGTradeRow2(i, ref signals, myTradeRow, mySet);
                        }

                        wireLog.Info("TradingSystemImage2: Set Added = true " + cqg_trading_system.Id);


                        foreach (KaiTrade.Interfaces.TradeSignal signal in signals)
                        {
                            Facade.ProcessTradeSignal(mySet, signal);
                            wireLog.Info("TradingSystemImage2:" + signal.ToString());
                        }
                         
                    }
                    else
                    {
                        mySet.Text = cqg_trading_system.LastError.Description;
                        mySet.Status = KaiTrade.Interfaces.Status.error;
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "TradingSystemImage:TradingSystemImage" + mySet.Text);
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemImage2:TradingSystemImage:TSData not found");
                }

                
                wireLog.Info("TradingSystemImage2:Exited " + cqg_trading_system.Id);
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemImage2", myE);
            }
            m_inTradingSystemImage = false;
        }

        private void appendCQGTradeRow2(int rowIndex, ref List<KaiTrade.Interfaces.TradeSignal> signals, CQGTradingSystemTradesRow tradeRow, KaiTrade.Interfaces.ITSSet tsSet)
        {
            try
            {
                

                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];
                    
                    if (systemTrade.IsActive)
                    {
                        appendTradeEntry2(rowIndex, j,ref signals, tsSet, systemTrade, tradeRow.Timestamp, tradeRow.TimestampOffset);
                        
                    }
                    else
                    {
                        appendTradeEntry2(rowIndex, j, ref signals, tsSet, systemTrade, tradeRow.Timestamp, tradeRow.TimestampOffset);
                        
                    }
                }

                //wireLog.Info("appendCQGTradeRow2 - Exit");
            }

            catch (Exception myE)
            {
                log.Error("appendCQGTradeRow2", myE);
            }
        }
        private void appendTradeEntry2(int rowIndex, int tradeIndex, ref List<KaiTrade.Interfaces.TradeSignal> signals, KaiTrade.Interfaces.ITSSet tsSet,CQGTradingSystemTrade systemTrade,DateTime timeStamp, int timeStampOffset)
        {
            try
            {
                KaiTrade.Interfaces.TradeSignal signal;
                if (systemTrade.TradeEntry.Signal)
                {
                    signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(systemTrade.Definition.Name);

                    setSignalData(signal, systemTrade.TradeEntry, systemTrade.Definition.Name, getKTAOrderSide(systemTrade.Definition.Side), timeStamp, timeStampOffset);
                    signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;

                    signals.Add(signal);
                }
                //wireLog.Info("appendTradeEntry2:entry trade>>> " + signal.ToString());

                // process the exits for the trade
                for (int k = 0; k < systemTrade.Definition.Exits.Count; k++)
                {
                    CQGTradeExit myExit = systemTrade.TradeExits.get_ItemByName(systemTrade.Definition.Exits[k].Name);
                    if (myExit.Signal)
                    {
                        signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(myExit.Definition.Name);

                        setSignalData(signal, myExit, timeStamp, timeStampOffset);
                        signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;
                        signal.Mnemonic = tsSet.Mnemonic;
                        signals.Add(signal);
                        //wireLog.Info("appendTradeEntry2:exitloop>>> " + signal.ToString());
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry2", myE);
            }
        }




        private void LogTradingSystemImage2(CQGTradingSystem cqg_trading_system)
        {
            try
            {

                wireLog.Info("LogTradingSystemImage2: " + cqg_trading_system.Id + " count:" + cqg_trading_system.TradesRows.Count.ToString());

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    mySet.Status = KaiTrade.Interfaces.Status.open;

                    if (cqg_trading_system.Status == eRequestStatus.rsSuccess)
                    {
                        List<KaiTrade.Interfaces.TradeSignal> signals = new List<KaiTrade.Interfaces.TradeSignal>();
                        

                        CQGTradingSystemTradesRows myTradeRows = cqg_trading_system.TradesRows;
                        //cqg_trading_system.TradesRows[0][0].Definition.Exits[0]
                        if (myTradeRows.Count == 0)
                        {
                            wireLog.Info("LogTradingSystemImage2: No Rows - just return " + cqg_trading_system.Id);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            logCQGTradeRow2(i, ref signals, myTradeRow, mySet);
                        }

                    }
                    else
                    {

                    }
                }
                else
                {
                    wireLog.Info("logTradingSystemImage2:TradingSystemImage:TSData not found");
                }


                wireLog.Info("logTradingSystemImage2:Exited " + cqg_trading_system.Id);
            }
            catch (Exception myE)
            {
                log.Error("logTradingSystemImage2", myE);
            }

        }


        private void logCQGTradeRow2(int rowIndex, ref List<KaiTrade.Interfaces.TradeSignal> signals, CQGTradingSystemTradesRow tradeRow, KaiTrade.Interfaces.ITSSet tsSet)
        {
            try
            {
                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];
                    try
                    {
                        string origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;
                        string systemTradeInfo = string.Format("row={0} tradeIndex={1}, TimeStamp={2}, TimeStampOffset={3}, Origin={4}, IsActive={5},  IsSet={6}", rowIndex, j, tradeRow.Timestamp, tradeRow.TimestampOffset, origin, systemTrade.IsActive, systemTrade.TradeEntry.Signal);
                        tSLog.Info("logTradeEntry2:" + systemTradeInfo);

                        // process the exits for the trade
                        for (int k = 0; k < systemTrade.Definition.Exits.Count; k++)
                        {
                            CQGTradeExit myExit = systemTrade.TradeExits.get_ItemByName(systemTrade.Definition.Exits[k].Name);
                            string exitTradeInfo = string.Format("row={0} tradeIndex={1}, TimeStamp={2}, TimeStampOffset={3}, Origin={4}, Name={5}, IsActive={6},  IsSet={7}", rowIndex, j, tradeRow.Timestamp, tradeRow.TimestampOffset, origin, myExit.Definition.Name, systemTrade.IsActive, myExit.Signal);
                            tSLog.Info("logTradeEntry2:" + exitTradeInfo);
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("logTradeEntry2", myE);
                    }
                }

                //wireLog.Info("appendCQGTradeRow2 - Exit");
            }

            catch (Exception myE)
            {
                log.Error("logCQGTradeRow2", myE);
            }
        }
        
        private void TradingSystemImage(CQGTradingSystem cqg_trading_system)
        {
            try
            {
            TradingSystemImage2(cqg_trading_system);
            }
            catch (Exception myE)
            {

            }
            return;


            try
            {
                if (m_inTradingSystemImage)
                {
                    // server error
                    throw new Exception("re-entered TradingSystemImage");
                }
                else
                {
                    m_inTradingSystemImage = true;
                }

                wireLog.Info("TradingSystemImage: " + cqg_trading_system.Id);

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    mySet.Status = KaiTrade.Interfaces.Status.open;

                    if (cqg_trading_system.Status == eRequestStatus.rsSuccess)
                    {
                        // Clears all records
                        // We use the tag to track the last time+cqg offset we processed
                        if (mySet.Items.Count > 0)
                        {
                            mySet.Tag = mySet.CurrentItem.Tag;
                        }
                        else
                        {
                            mySet.Tag = getTradeSystemMarkerTag(DateTime.Now, 0);
                            //mySet.Tag = (long)0;
                        }
                        mySet.Items.Clear();
                        mySet.Status = KaiTrade.Interfaces.Status.open;

                        CQGTradingSystemTradesRows myTradeRows = cqg_trading_system.TradesRows;
                        //cqg_trading_system.TradesRows[0][0].Definition.Exits[0]
                        if (myTradeRows.Count == 0)
                        {
                            wireLog.Info("TradingSystemImage: No Rows - just return " + cqg_trading_system.Id);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            appendCQGTradeRow(i, mySet, myTradeRow);
                        }

                        wireLog.Info("TradingSystemImage: Set Added = true " + cqg_trading_system.Id);
                        mySet.Added = true;
                    }
                    else
                    {
                        mySet.Text = cqg_trading_system.LastError.Description;
                        mySet.Status = KaiTrade.Interfaces.Status.error;
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "TradingSystemImage:TradingSystemImage" + mySet.Text);
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemImage:TradingSystemImage:TSData not found");
                }
                wireLog.Info("TradingSystemImage:Exited " + cqg_trading_system.Id);
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemImage", myE);
            }
            m_inTradingSystemImage = false;
        }

        private void logCQGTradeRow(int rowIndex, CQGTradingSystemTradesRow tradeRow)
        {
            try
            {
                string myTemp = string.Format("Log TradeRow>>> count ={0} TimeStamp={1} ", tradeRow.Count, tradeRow.Timestamp);
                wireLog.Info(myTemp);

                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];

                    logTradeEntry(rowIndex, j, systemTrade);
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry", myE);
            }
        }

        private void logTradeEntry(int rowIndex, int tradeIndex, CQGTradingSystemTrade systemTrade)
        {
            try
            {
                string myTemp = string.Format("Log TradeRow:systemTrade:>>> rowIndex{0} tradeIndex:{1} tradeTime ={2} IsActive={3} DefName={4}  numExits={5}, Offset={6}", rowIndex, tradeIndex, systemTrade.Timestamp, systemTrade.IsActive, systemTrade.Definition.Name, systemTrade.TradeExits.Count, systemTrade.TimestampOffset);
                wireLog.Info(myTemp);


                KaiTrade.Interfaces.TradeSignal signal;
                signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(systemTrade.Definition.Name);

                setSignalData(signal, systemTrade.TradeEntry, systemTrade.Definition.Name, getKTAOrderSide(systemTrade.Definition.Side), systemTrade.Timestamp, systemTrade.TimestampOffset);

                myTemp = string.Format("Log TradeRow:systemTrade:Entry>>> Origin = {0} signal={1} ", systemTrade.Definition.Name, signal.ToString());
                wireLog.Info(myTemp);

                // process the exits for the trade
                for (int k = 0; k < systemTrade.TradeExits.Count; k++)
                {
                    CQGTradeExit myExit = systemTrade.TradeExits[k];

                    myTemp = string.Format("Log TradeRow:systemTrade:Exit>>> Name ={0} signal={1} ", myExit.Definition.Name, myExit.Signal);
                    wireLog.Info(myTemp);

                    signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(myExit.Definition.Name);

                    setSignalData(signal, myExit, systemTrade.Timestamp, systemTrade.TimestampOffset);

                    myTemp = string.Format("Log TradeRow:systemTrade:Exit:>>> Origin ={0} signal={1} ", systemTrade.Definition.Name, signal.ToString());
                    wireLog.Info(myTemp);
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry", myE);
            }
        }





        private void appendCQGTradeRow(int rowIndex, KaiTrade.Interfaces.ITSSet tsSet, CQGTradingSystemTradesRow tradeRow)
        {
            try
            {
                KaiTrade.Interfaces.TSItem tsItem = tsSet.GetNewItem();

                tsItem.Index = tsSet.Items.Count;

                tsItem.TimeStamp = tradeRow.Timestamp;

                tsItem.ConditionName = tsSet.ConditionName;
                tsItem.Mnemonic = tsSet.Mnemonic;

                string myTemp = string.Format("appendCQGTradeRow>>> rowIndex{0} cond={1} Time={2} Index={3} RowCount={4} trTime{5}", rowIndex, tsItem.ConditionName, tsItem.TimeStamp.ToString(), tsItem.Index.ToString(), tradeRow.Count, tradeRow.Timestamp);
                wireLog.Info(myTemp);

                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];
                    tsItem.Tag = getTradeSystemMarkerTag(systemTrade.Timestamp, systemTrade.TimestampOffset);
                    if (systemTrade.IsActive)
                    {
                        appendTradeEntry(rowIndex, j, tsSet, tsItem, systemTrade);                     
                        tsSet.AddItem(tsItem);
                    }
                    else
                    {
                        appendTradeEntry(rowIndex, j, tsSet, tsItem, systemTrade);
                        tsSet.AddItem(tsItem);
                    }
                }
                myTemp = "appendCQGTradeRow - Exit";
                wireLog.Info(myTemp);
            }

            catch (Exception myE)
            {
                log.Error("appendCQGTradeRow", myE);
            }
        }





        private void appendTradeEntry(int rowIndex, int tradeIndex, KaiTrade.Interfaces.ITSSet tsSet, KaiTrade.Interfaces.TSItem tsItem, CQGTradingSystemTrade systemTrade)
        {
            try
            {
                KaiTrade.Interfaces.TradeSignal signal;
                signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(systemTrade.Definition.Name);

                setSignalData(signal, systemTrade.TradeEntry, systemTrade.Definition.Name, getKTAOrderSide(systemTrade.Definition.Side), systemTrade.Timestamp, systemTrade.TimestampOffset);
                signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;

                tsItem.Signals.Add(signal.Origin + ":" + ENTRY_TRADE, signal);

                wireLog.Info("appendTradeEntry:entry trade>>> " + signal.ToString());

                // process the exits for the trade
                for (int k = 0; k < systemTrade.TradeExits.Count; k++)
                {
                    CQGTradeExit myExit = systemTrade.TradeExits[k];

                    signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(myExit.Definition.Name);

                    setSignalData(signal, myExit, systemTrade.Timestamp, systemTrade.TimestampOffset);
                    signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;
                    signal.Mnemonic = tsSet.Mnemonic;
                    tsItem.Signals.Add(signal.Origin + ":" + myExit.Definition.Name, signal);
                    wireLog.Info("appendTradeEntry:exitloop>>> " + signal.ToString());
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry", myE);
            }
        }



        private void setSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGTradeEntry tradeEntry, string name, string side, DateTime timeStamp, int timeStampOffset)
        {
            try
            {
                // set basic info
                signal.OrdQty = tradeEntry.Quantity;
                signal.Price = tradeEntry.Price;
                signal.StopPrice = tradeEntry.StopLimitPrice;
                signal.Set = tradeEntry.Signal;
                signal.OrdType = getKTAOrderType(tradeEntry.Definition.OrderType);
                //signal.Side = getKTAOrderSide(systemTrade.Definition.Side);
                signal.Side = side;
                signal.Name = name;
                signal.SignalType = KaiTrade.Interfaces.TradeSignalType.enter;
                signal.TimeCreated = timeStamp;
                signal.Text = timeStampOffset.ToString();
                //signal.Mnemonic = tradeEntry.Trade.Definition.Entry.
            }
            catch (Exception myE)
            {
                log.Error("setEntrySignal:entry", myE);
            }
        }

        private void setSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGTradeExit exitTrade, DateTime timeStamp, int timeStampOffset)
        {
            try
            {
                // set basic info
                signal.OrdQty = exitTrade.Quantity;
                signal.Price = exitTrade.Price;
                signal.StopPrice = exitTrade.StopLimitPrice;
                signal.Set = exitTrade.Signal;
                signal.OrdType = getKTAOrderType(exitTrade.Definition.OrderType);
                signal.Side = getKTAOrderSide(exitTrade.Trade.Definition.Side);
                signal.SignalType = KaiTrade.Interfaces.TradeSignalType.exit;
                signal.Name = exitTrade.Definition.Name;
                signal.TimeCreated = timeStamp;
                signal.Text = timeStampOffset.ToString();
            }
            catch (Exception myE)
            {
                log.Error("setEntrySignal:entry", myE);
            }
        }


        private bool updateSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGChangedTradeEntry changedTradeEntry, string name, string side)
        {
            bool changed = false;

            try
            {
                // set basic info
                if (signal.OrdQty != changedTradeEntry.TradeEntry.Quantity)
                {
                    changed = true;
                    signal.OrdQty = changedTradeEntry.TradeEntry.Quantity;
                }
                if (signal.Price != changedTradeEntry.TradeEntry.Price)
                {
                    changed = true;
                    signal.Price = changedTradeEntry.TradeEntry.Price;
                }
                if (signal.StopPrice != changedTradeEntry.TradeEntry.StopLimitPrice)
                {
                    changed = true;
                    signal.StopPrice = changedTradeEntry.TradeEntry.StopLimitPrice;
                }
                if (signal.Set != changedTradeEntry.TradeEntry.Signal)
                {
                    changed = true;
                    signal.Set = changedTradeEntry.TradeEntry.Signal;
                }
                if (signal.OrdType != getKTAOrderType(changedTradeEntry.TradeEntry.Definition.OrderType))
                {
                    changed = true;
                    signal.OrdType = getKTAOrderType(changedTradeEntry.TradeEntry.Definition.OrderType);
                }


                //signal.Side = getKTAOrderSide(systemTrade.Definition.Side);
                signal.Side = side;
                signal.Name = name;
            }
            catch (Exception myE)
            {
                log.Error("updateEntrySignal:entry", myE);
            }

            return changed;
        }


        private bool updateSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGChangedTradeExit changedTradeExit, string name, string side)
        {
            bool changed = false;

            try
            {
                // set basic info
                if (signal.OrdQty != changedTradeExit.TradeExit.Quantity)
                {
                    changed = true;
                    signal.OrdQty = changedTradeExit.TradeExit.Quantity;
                }
                if (signal.Price != changedTradeExit.TradeExit.Price)
                {
                    changed = true;
                    signal.Price = changedTradeExit.TradeExit.Price;
                }
                if (signal.StopPrice != changedTradeExit.TradeExit.StopLimitPrice)
                {
                    changed = true;
                    signal.StopPrice = changedTradeExit.TradeExit.StopLimitPrice;
                }
                if (signal.Set != changedTradeExit.TradeExit.Signal)
                {
                    changed = true;
                    signal.Set = changedTradeExit.TradeExit.Signal;
                }
                if (signal.OrdType != getKTAOrderType(changedTradeExit.TradeExit.Definition.OrderType))
                {
                    changed = true;
                    signal.OrdType = getKTAOrderType(changedTradeExit.TradeExit.Definition.OrderType);
                }


                //signal.Side = getKTAOrderSide(systemTrade.Definition.Side);
                signal.Side = side;
                signal.Name = name;
            }
            catch (Exception myE)
            {
                log.Error("updateEntrySignal:exit", myE);
            }

            return changed;
        }



        private void TradingSystemAdd(CQGTradingSystem cqg_trading_system, CQGTradingSystemAddInfo addInfo)
        {
            try
            {
                wireLog.Info("TradingSystemAdd: " + cqg_trading_system.Id);

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;

                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];

                    // we expect that cqg added rows
                    if (mySet.Items.Count >= cqg_trading_system.TradesRows.Count)
                    {
                        // this is some problem because we see no new rows
                        //throw new Exception("new rows were expected but not found");
                    }

                    for (int i = 0; i < cqg_trading_system.TradesRows.Count; i++)
                    {
                        CQGTradingSystemTradesRow myTradeRow = cqg_trading_system.TradesRows[i];
                        appendCQGTradeRow(i, mySet, myTradeRow);
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemAdd:TSData not found");
                }
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemAdd", myE);
            }
        }




        private void TradingSystemInsert(CQGTradingSystem cqg_trading_system, CQGTradingSystemInsertInfo cqg_trading_system_insert_info)
        {
            try
            {
                wireLog.Info("TradingSystemInsert: " + cqg_trading_system.Id);
                if (cqg_trading_system_insert_info.Index == 0)
                {
                    TradingSystemImage(cqg_trading_system);
                }
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemInsert:TradingSystemImage", myE);
            }
        }


        private bool LogCQGTSUdateInfo(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqgUpdate)
        {
            bool changed = false;
            try
            {
                string myTemp = string.Format("LogCQGTSUdateInfo>>> index={0} id={1} ", cqgUpdate.Index, cqg_trading_system.Id);
                wireLog.Info(myTemp);


                foreach (CQGChangedTradeEntry tradeEntry in cqgUpdate.ChangedEntries)
                {
                    myTemp = string.Format("LogCQGTSUdateInfo:EntryChange>>> ChangeCat={0} id={1} ", tradeEntry.ChangeCategory, cqg_trading_system.Id);
                    wireLog.Info(myTemp);

                    myTemp = string.Format("LogCQGTSUdateInfo:EntryChange:Values>>> Signal={0} Px={1}, Qty={2}, StopPx={3}", tradeEntry.TradeEntry.Signal, tradeEntry.TradeEntry.Price, tradeEntry.TradeEntry.Quantity, tradeEntry.TradeEntry.StopLimitPrice);
                    wireLog.Info(myTemp);
                }



                foreach (CQGChangedTradeExit tradeExit in cqgUpdate.ChangedExits)
                {
                    myTemp = string.Format("LogCQGTSUdateInfo:ExitChange>>> Name={0} ChangeCat={1} id={2} ", tradeExit.TradeExit.Definition.Name, tradeExit.ChangeCategory, cqg_trading_system.Id);
                    wireLog.Info(myTemp);

                    myTemp = string.Format("LogCQGTSUdateInfo:ExitChange:Values>>> Signal={0} Px={1}, Qty={2}, StopPx={3}", tradeExit.TradeExit.Signal, tradeExit.TradeExit.Price, tradeExit.TradeExit.Quantity, tradeExit.TradeExit.StopLimitPrice);
                    wireLog.Info(myTemp);
                }

                myTemp = "<<< LogCQGTSUdateInfo: Ended>>>";
                wireLog.Info(myTemp);
            }
            catch (Exception myE)
            {
                log.Error("LogCQGTSUdateInfo", myE);
            }
            return changed;
        }


        private bool updateTSRow(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqgUpdate, KaiTrade.Interfaces.ITSSet tsSet)
        {
            bool changed = false;
            try
            {
                // There is only 1 entry signal
                wireLog.Info("updateTSRow: " + cqg_trading_system.Id);
                KaiTrade.Interfaces.TSItem tsItem = tsSet.Items[cqgUpdate.Index];
                foreach (CQGChangedTradeEntry tradeEntry in cqgUpdate.ChangedEntries)
                {
                    KaiTrade.Interfaces.TradeSignal signal = tsItem.Signals[ENTRY_TRADE];
                    // these signals all have the name entry
                    switch (tradeEntry.ChangeCategory)
                    {
                        case eTradeChangeCategory.tccPrice:
                            signal.Price = tradeEntry.TradeEntry.Price;
                            break;
                        case eTradeChangeCategory.tccQuantity:
                            signal.OrdQty = tradeEntry.TradeEntry.Quantity;
                            break;
                        case eTradeChangeCategory.tccSignalReset:
                            signal.Set = false;
                            break;
                        case eTradeChangeCategory.tccSignalSet:
                            signal.Set = true;
                            break;
                        case eTradeChangeCategory.tccStopLimitPrice:
                            signal.StopPrice = tradeEntry.TradeEntry.StopLimitPrice;
                            break;

                        default:
                            if (updateSignalData(tsItem.Signals[ENTRY_TRADE], tradeEntry, tsItem.Signals[ENTRY_TRADE].Name, tsItem.Signals[ENTRY_TRADE].Side))
                            {
                                changed = true;
                            }
                            break;
                    }
                }

                foreach (CQGChangedTradeExit tradeExit in cqgUpdate.ChangedExits)
                {
                    // Use the Name on the tradeExit to locate the signal
                    switch (tradeExit.ChangeCategory)
                    {
                        case eTradeChangeCategory.tccPrice:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].Price = tradeExit.TradeExit.Price;
                            break;
                        case eTradeChangeCategory.tccQuantity:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].OrdQty = tradeExit.TradeExit.Quantity;
                            break;
                        case eTradeChangeCategory.tccSignalReset:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].Set = true;
                            break;
                        case eTradeChangeCategory.tccSignalSet:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].Set = false;
                            break;
                        case eTradeChangeCategory.tccStopLimitPrice:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].StopPrice = tradeExit.TradeExit.StopLimitPrice;
                            break;

                        default:
                            if (updateSignalData(tsItem.Signals[tradeExit.TradeExit.Definition.Name], tradeExit, tsItem.Signals[tradeExit.TradeExit.Definition.Name].Name, tsItem.Signals[tradeExit.TradeExit.Definition.Name].Side))
                            {
                                changed = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("updateTSRow", myE);
            }
            return changed;
        }


        private void TradingSystemUpdate(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqgUpdate)
        {
            try
            {
                wireLog.Info("TradingSystemUpdate: " + cqg_trading_system.Id);

                return;

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    wireLog.Info("TradingSystemUpdate: no items so doing image" + cqg_trading_system.Id);
                    this.TradingSystemImage(cqg_trading_system);
                    //mySet.Changed = true;
                    mySet.Updated = true;
                    /*
                    if (mySet.Items.Count == 0)
                    {
                        driverLog.Info("TradingSystemUpdate: no items so doing image" + cqg_trading_system.Id);
                        this.TradingSystemImage(cqg_trading_system);
                        mySet.Changed = true;
                        mySet.Updated = true;
                    }
                    else
                    {
                        // check we have the index they are updating
                        if (mySet.Items.Count > cqgUpdate.Index)
                        {
                            // we have the row - it can be updated
                            if (updateTSRow(cqg_trading_system, cqgUpdate, mySet))
                            {
                                mySet.Changed = true;
                                mySet.Updated = true;
                            }
                        }
                    }
                     */
                }
                else
                {
                    wireLog.Info("TradingSystemUpdate:TSData not found");
                }
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemUpdate", myE);
            }
        }


        private CQGTradingSystem lastTradingSystem = null;
        /// <summary>
        /// Called when the trading system resolves
        /// </summary>
        /// <param name="cqg_trading_system"></param>
        /// <param name="cqg_error"></param>
        void CQGApp_TradingSystemResolved(CQGTradingSystem cqg_trading_system, CQGError cqg_error)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {
                        driverLog.Info("CQGApp_TradingSystemResolved:" + cqg_trading_system.Id);
                        if (wireLog.IsInfoEnabled)
                        {
                            logTradingSystem(cqg_trading_system);
                        }

                        TradingSystemImage(cqg_trading_system);
                        logTradeSystemParameters(cqg_trading_system);
                        lastTradingSystem  = cqg_trading_system;
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemResolved", myE);
                    }
                }
            }
            catch
            {
            }
        }

        private void logTradeSystemParameters(CQGTradingSystem ts)
        {
            try
            {
                
                log.Info("logTradeSystemParameters:default:enter:" + ts.Id);

                foreach (CQGParameterDefinition p in ts.Definition.ParameterDefinitions)
                {
                    driverLog.Info("parameters:name:" + p.Name + "Default value:" + p.DefaultValue.ToString());
                    driverLog.Info("parameters:name:" + p.Name + "Param   value:" + ts.Request.Parameter[ p.Name].ToString());
                    
                }
                log.Info("setRequestParameters:default:exit:" + ts.Id);
                
                driverLog.Info("baseExpression:" + ts.Request.BaseExpression);
               

            }
            catch (Exception myE)
            {
                log.Error("setRequestParameters", myE);
            }
        }

        /// <summary>
        /// Displays resolved CustomStudy definitions.
        /// </summary>
        /// <param name="cqg_custom_study_definitions">
        /// The resolved CQGCustomStudyDefinitions object.
        /// </param>
        /// <param name="cqg_error">
        /// CQGError object describing last error occurred
        /// during processing request or Nothing in case of no error.
        /// </param>
        private void CEL_CustomStudyDefinitionsResolved(CQGCustomStudyDefinitions cqg_custom_study_definitions, CQGError cqg_error)
        {
            try
            {
                // Notifying when any error occurred during request
                if (cqg_error != null)
                {
                    throw new Exception(cqg_error.Description);
                }

                // Fill up new data
                foreach (CQGCustomStudyDefinition definition in cqg_custom_study_definitions)
                {
                    //cmbDefinition.Items.Add(definition.Name);
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_CustomStudyDefinitionsResolved", myE);
            }
        }

        /// <summary>
        /// This event is fired when the custom study (CQGCustomStudy) is resolved
        /// or when some error has occurred during the custom studies request processing.
        /// </summary>
        /// <param name="cqg_custom_study">
        /// Reference to resolved CQGCustomStudy.
        /// </param>
        /// <param name="cqg_error">
        /// The CQGError object that describes the last error occurred
        /// while processing the CustomStudy request or
        /// Nothing/Invalid_Error in case of no error.
        /// </param>
        private void CEL_CustomStudyResolved(CQGCustomStudy cqg_custom_study, CQGError cqg_error)
        {
            try
            {
                lock (m_CustomStudyToken1)
                {
                    try
                    {
                        driverLog.Info("CEL_CustomStudyResolved:" + cqg_custom_study.Id);

                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_custom_study.Id))
                        {
                            mySet = m_TSSets[cqg_custom_study.Id];
                            if (cqg_custom_study.Status == eRequestStatus.rsSuccess)
                            {
                                // Clears all records
                                mySet.Items.Clear();
                                mySet.Status = KaiTrade.Interfaces.Status.open;
                                if (cqg_custom_study.Count == 0)
                                {
                                    return;
                                }


                                for (int i = 0; i < cqg_custom_study.Count; i++)
                                {
                                    // get a new TS item
                                    KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                                    myItem.Index = i;
                                    myItem.TimeStamp = cqg_custom_study[i].Timestamp;
                                    myItem.ConditionName = mySet.ConditionName;

                                    CQGCurves myCurves = cqg_custom_study[i];

                                    for (int j = 0; j <= myCurves.Count - 1; j++)
                                    {
                                        myItem.SetCurveValue(cqg_custom_study.CurveHeaders[j], double.Parse(myCurves[j].ToString()));
                                    }

                                    mySet.AddItem(myItem);
                                }
                                mySet.Added = true;
                            }
                            else
                            {
                                mySet.Text = cqg_error.Description;
                                mySet.Status = KaiTrade.Interfaces.Status.error;
                                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CEL_CustomStudyResolved" + mySet.Text);
                            }
                        }
                        else
                        {
                            driverLog.Info("CEL_CustomStudyResolved:TSData not found");
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_CustomStudyResolved", myE);
                    }
                }
            }
            catch
            {
            }
        }



        /// <summary>
        /// Fired when a CQGCurves item is added to the end of CQGCustomStudy.
        /// </summary>
        /// <param name="cqg_custom_study">
        /// Reference to changed CQGCustomStudy.
        /// </param>
        private void CEL_CustomStudyAdded(CQGCustomStudy cqg_custom_study)
        {
            try
            {
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_custom_study.Id))
                {
                    mySet = m_TSSets[cqg_custom_study.Id];
                    mySet.Items.Clear();
                    if (cqg_custom_study.Status == eRequestStatus.rsSuccess)
                    {
                        // Clears all records
                        mySet.Items.Clear();

                        if (cqg_custom_study.Count == 0)
                        {
                            return;
                        }


                        for (int i = 0; i < cqg_custom_study.Count; i++)
                        {
                            // get a new TS item
                            KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                            myItem.Index = i;
                            myItem.TimeStamp = cqg_custom_study[i].Timestamp;
                            myItem.ConditionName = mySet.ConditionName;

                            CQGCurves myCurves = cqg_custom_study[i];

                            for (int j = 0; j <= myCurves.Count - 1; j++)
                            {
                                myItem.SetCurveValue(cqg_custom_study.CurveHeaders[j], double.Parse(myCurves[j].ToString()));
                            }

                            mySet.AddItem(myItem);
                        }
                        mySet.Added = true;
                    }
                    else
                    {
                        mySet.Text = cqg_custom_study.LastError.Description;
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_CustomStudyAdded", myE);
            }
        }

        /// <summary>
        /// Fired when the CQGCurves item is updated.
        /// </summary>
        /// <param name="cqg_custom_study">
        /// Reference to changed CQGCustomStudy
        /// </param>
        /// <param name="index_">
        /// Specifies the updated CQGCurves index.
        /// </param>
        private void CEL_CustomStudyUpdated(CQGCustomStudy cqg_custom_study, int index_)
        {
            try
            {
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;

                if (m_TSSets.ContainsKey(cqg_custom_study.Id))
                {
                    mySet = m_TSSets[cqg_custom_study.Id];

                    if (!mySet.ReportAll)
                    {
                        // they only want added bars - so exit
                        return;
                    }

                    CQGCurves myCurves = cqg_custom_study[index_];
                    mySet.TimeStamp = myCurves.Timestamp;
                    for (int j = 0; j <= myCurves.Count - 1; j++)
                    {
                        //myItem.SetCurveValue(cqg_expression.OutputHeaders[j], double.Parse(expressionOutputs[j].ToString()));
                        mySet.SetUDCurveValue(j, double.Parse(myCurves[j].ToString()));
                        mySet.SetUDCurveName(j, cqg_custom_study.CurveHeaders[j].ToString());
                    }



                    mySet.Updated = true;
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_CustomStudyUpdated", myE);
            }
        }


        /// <summary>
        /// This event is fired when the expression (CQGExpression) is resolved or
        /// when some error has occurred during the expression request processing.
        /// </summary>
        /// <param name="cqg_expression">
        /// Reference to resolved CQGExpression
        /// </param>
        /// <param name="cqg_error">
        /// CQGError object describing the last error occurred during the expression
        /// request processing or Nothing/Invalid_Error in case of no error.
        /// CQGCEL.IsValid(Invalid_Error) returns False.
        /// </param>
        private void CEL_ExpressionResolved(CQG.CQGExpression cqg_expression, CQG.CQGError cqg_error)
        {
            try
            {
                lock (m_ExpToken1)
                {
                    try
                    {
                        driverLog.Info("CEL_ExpressionResolved:" + cqg_expression.Id.ToString());
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_expression.Id))
                        {
                            mySet = m_TSSets[cqg_expression.Id];

                            if (wireLog.IsInfoEnabled)
                            {
                                wireLog.Info("CEL_ExpressionResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                            }

                            if (cqg_expression.Status == eRequestStatus.rsSuccess)
                            {
                                // Clears all records
                                mySet.Items.Clear();
                                mySet.Status = KaiTrade.Interfaces.Status.open;

                                if (cqg_expression.Count == 0)
                                {
                                    return;
                                }


                                for (int i = 0; i < cqg_expression.Count; i++)
                                {
                                    // get a new TS item
                                    KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                                    myItem.Index = i;
                                    myItem.TimeStamp = cqg_expression[i].Timestamp;
                                    myItem.ConditionName = mySet.ConditionName;
                                    CQGExpressionOutputs expressionOutputs = cqg_expression[i];

                                    for (int j = 0; j <= expressionOutputs.Count - 1; j++)
                                    {
                                        myItem.SetCurveValue(cqg_expression.OutputHeaders[j], double.Parse(expressionOutputs[j].ToString()));
                                    }
                                    myItem.DriverChangedData = true;
                                    mySet.AddItem(myItem);
                                }
                                mySet.Added = true;
                            }
                            else
                            {
                                mySet.Text = cqg_error.Description;
                                mySet.Status = KaiTrade.Interfaces.Status.error;
                                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CEL_ExpressionResolved" + mySet.Text);
                            }
                        }
                        else
                        {
                            driverLog.Info("CEL_ExpressionResolved:TSData not found");
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_ExpressionResolved", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Fired when a CQGExpressionOutputs item is added to the end of CQGExpression.
        /// </summary>
        /// <param name="cqg_expression">
        /// Reference to changed CQGExpression.
        /// </param>
        private void CEL_ExpressionAdded(CQG.CQGExpression cqg_expression)
        {
            try
            {
                lock (m_ExpToken1)
                {
                    try
                    {
                        if (wireLog.IsInfoEnabled)
                        {
                            wireLog.Info("CEL_ExpressionAdded:enter");
                        }

                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_expression.Id))
                        {
                            mySet = m_TSSets[cqg_expression.Id];

                            if (wireLog.IsInfoEnabled)
                            {
                                wireLog.Info("CEL_ExpressionAdded:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                            }

                            if (cqg_expression.Status == eRequestStatus.rsSuccess)
                            {
                                // Clears all records
                                mySet.Items.Clear();

                                if (cqg_expression.Count == 0)
                                {
                                    return;
                                }


                                for (int i = 0; i < cqg_expression.Count; i++)
                                {
                                    // get a new TS item
                                    KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                                    myItem.Index = i;
                                    myItem.TimeStamp = cqg_expression[i].Timestamp;
                                    myItem.ConditionName = mySet.ConditionName;
                                    CQGExpressionOutputs expressionOutputs = cqg_expression[i];
                                    string myDump = "CEL_EADD";
                                    for (int j = 0; j <= expressionOutputs.Count - 1; j++)
                                    {
                                        string myTemp = expressionOutputs[j].ToString();
                                        myItem.SetCurveValue(cqg_expression.OutputHeaders[j], double.Parse(expressionOutputs[j].ToString()));
                                        myDump += "|" + cqg_expression.OutputHeaders[j].ToString() + "|" + expressionOutputs[j].ToString() + "|" + cqg_expression[i].Timestamp.ToString();
                                    }
                                    myItem.DriverChangedData = true;
                                    mySet.AddItem(myItem);
                                    wireLog.Info(myDump);
                                }
                                mySet.Added = true;
                            }
                            else
                            {
                                mySet.Text = cqg_expression.LastError.Description;
                            }
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_ExpressionAdded", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Fired when a CQGExpressionOutputs item is updated.
        /// </summary>
        /// <param name="cqg_expression">
        /// Reference to changed CQGExpression
        /// </param>
        /// <param name="index_">
        /// Specifies the updated CQGExpressionOutputs index.
        /// </param>
        private void CEL_ExpressionUpdated(CQG.CQGExpression cqg_expression, int index_)
        {
            try
            {
                lock (m_ExpToken1)
                {
                    try
                    {
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        lock (m_Token1)
                        {
                            if (m_TSSets.ContainsKey(cqg_expression.Id))
                            {
                                mySet = m_TSSets[cqg_expression.Id];
                                if (!mySet.ReportAll)
                                {
                                    // they only want added bars - so exit
                                    return;
                                }
                                if (wireLog.IsInfoEnabled)
                                {
                                    wireLog.Info("CEL_ExpressionUpdated:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                                }


                                if ((mySet.Items.Count - 1) == index_)
                                {
                                    // get a new TS item
                                    //KaiTrade.Interfaces.TSItem myItem = mySet.GetNewItem();
                                    //myItem.Index = i;
                                    //myItem.TimeStamp = cqg_expression[index_].Timestamp;

                                    //myItem.ConditionName = mySet.ConditionName;
                                    CQGExpressionOutputs expressionOutputs = cqg_expression[index_];
                                    string myDump = "CEL_EUPD" + "|" + expressionOutputs.Timestamp.ToString();
                                    mySet.TimeStamp = expressionOutputs.Timestamp;
                                    
                                    for (int j = 0; j <= expressionOutputs.Count - 1; j++)
                                    {
                                        //myItem.SetCurveValue(cqg_expression.OutputHeaders[j], double.Parse(expressionOutputs[j].ToString()));
                                        mySet.SetUDCurveValue(j, double.Parse(expressionOutputs[j].ToString()));
                                        mySet.SetUDCurveName(j, cqg_expression.OutputHeaders[j].ToString());
                                        myDump += "|" + cqg_expression.OutputHeaders[j].ToString() + "|" + expressionOutputs[j].ToString();
                                    }
                                    wireLog.Info(myDump);
                                }
                                else
                                {
                                }



                                mySet.Updated = true;
                            }
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_ExpressionUpdated", myE);
                    }
                }
            }
            catch
            {
            }

        }


        /// <summary>
        /// Get a CFI from a CQG instrument
        /// </summary>
        /// <param name="instrumentType"></param>
        /// <returns></returns>
        private string getCFI(eInstrumentType instrumentType)
        {
            string myCFI = "";
            switch (instrumentType)
            {
                case eInstrumentType.itFuture:
                    myCFI = "FXXXXX";
                    break;
                case eInstrumentType.itOptionCall:
                    myCFI = "OCXXXX";
                    break;
                case eInstrumentType.itOptionPut:
                    myCFI = "OPXXXX";
                    break;
                case eInstrumentType.itStock:
                    myCFI = "ESXXXX";
                    break;
                default:
                    myCFI = "ESXXXX";
                    break;
            }
            return myCFI;
        }

        public override void GetProduct(KaiTrade.Interfaces.IProduct myProduct, string myGenericName)
        {
            try
            {
                if (myGenericName.Trim().Length > 0)
                {
                    driverLog.Info("GetProduct:" + myGenericName);
                    m_ProductRegister.Add(myGenericName, myProduct);
                    if (m_GWStatus == eConnectionStatus.csConnectionUp)
                    {
                        m_CQGHostForm.CQGApp.NewInstrument(myGenericName.Trim());
                    }
                    else
                    {
                        driverLog.Info("GetProduct:GW not up save for later subscription" + myGenericName);
                        m_DelayedProductRequests.Push(myProduct);
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("GetProduct", myE);
            }
        }
        /// <summary>
        /// This event is fired when a new instrument is resolved and subscribed.
        /// </summary>
        /// <param name="symbol">The commodity symbol that was requested by user in NewInstrument method.</param>
        /// <param name="instrument">Subscribed CQGInstrument object.</param>
        private void cel_InstrumentSubscribed(string symbol, CQGInstrument instrument)
        {
            try
            {
                driverLog.Info("cel_InstrumentSubscribed:Pre" + symbol);
                lock (m_InstrSubscribed)
                {
                    try
                    {
                        driverLog.Info("cel_InstrumentSubscribed" + symbol);
                        KaiTrade.Interfaces.IProduct myProduct = null;
                        if (m_ProductRegister.ContainsKey(symbol))
                        {
                            myProduct = m_ProductRegister[symbol];
                            if (symbol != instrument.FullName)
                            {
                                if (m_ProductGenericNameRegister.ContainsKey(instrument.FullName))
                                {
                                    m_ProductGenericNameRegister[instrument.FullName] = symbol;
                                }
                                else
                                {
                                    m_ProductGenericNameRegister.Add(instrument.FullName, symbol);
                                }
                            }
                            if (instrument.FullName.StartsWith("SPREAD"))
                            {
                                DefineCQGStrategy(instrument.FullName);
                            }
                        }
                        else
                        {
                            // Use the product manager directly to add a product
                            myProduct = m_Facade.Factory.GetProductManager().CreateProductWithSecID(instrument.FullName, m_Name, instrument.ExchangeAbbreviation, instrument.FullName, m_Name);
                            m_ProductRegister.Add(symbol, myProduct);
                            
                        }
                        setProductValues(myProduct, symbol, instrument);
                        // This will case a register update to be sent to subscribers of the product manager - e.g. like the rabbit publisher
                        m_Facade.Factory.GetProductManager().RegisterProduct(myProduct);
                    }
                    catch (Exception myE)
                    {
                        log.Error("cel_InstrumentSubscribed", myE);
                    }
                }
            }
            catch
            {
            }
        }

        private void setProductValues(KaiTrade.Interfaces.IProduct myProduct, string symbol, CQGInstrument instrument)
        {
            try
            {
                
                myProduct.TradeVenue = m_Name;
                myProduct.Commodity = instrument.Commodity;
                myProduct.Exchange = instrument.ExchangeAbbreviation;
                myProduct.SecurityID = instrument.FullName;
                myProduct.IDSource = m_Name;
                myProduct.GenericName = symbol;
                myProduct.Symbol = instrument.Commodity;
                myProduct.TickSize = (decimal)instrument.TickSize;
                myProduct.TickValue = (decimal)instrument.TickValue;
                myProduct.LongName = instrument.FullName;
                myProduct.CFICode = "FXXXXX";

            }
            catch (Exception myE)
            {
                log.Error("setProductValues", myE);
            }
        }

        /// <summary>
        /// This event is fired when the requested instrument is invalid.
        /// </summary>
        /// <param name="symbl">The commodity symbol that was requested by user in NewInstrument method.</param>
        private void cel_IncorrectSymbol(string symbol)
        {
            try
            {
                string myText = "CQG:The symbol '" + symbol + "' is incorrect.";
                this.SendAdvisoryMessage(myText);
                log.Error("cel_IncorrectSymbol:" + myText);
                m_CQGHostForm.Message = myText;
            }
            catch (Exception myE)
            {
                log.Error("cel_IncorrectSymbol", myE);
            }
        }


        /// <summary>
        /// This event is fired when any of the instrument quotes or dynamic instrument properties are changed.
        /// </summary>
        /// <param name="instrument">Changed instrument.</param>
        /// <param name="quotes">Collection of changed quotes.</param>
        /// <param name="props">Collection of changed dynamic properties. </param>
        private void cel_InstrumentChanged(CQGInstrument instrument, CQGQuotes quotes, CQGInstrumentProperties props)
        {
            try
            {
                lock (m_InstrToken1)
                {
                    try
                    {
                        if (!m_PublisherRegister.ContainsKey(instrument.FullName))
                        {
                            return;
                        }
                        if (!m_PXContexts.ContainsKey(instrument.FullName))
                        {
                            m_PXContexts.Add(instrument.FullName, new DriverBase.PXUpdateContext(instrument.FullName));
                        }
                        KaiTrade.TradeObjects.PXUpdateBase pxupdate = null;
                        if (quotes.Count > 0)
                        {
                            foreach (CQGQuote quote in quotes)
                            {
                                switch (quote.Type)
                                {
                                    case eQuoteType.qtTrade:
                                        pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                                        pxupdate.Mnemonic = instrument.FullName;
                                        pxupdate.UpdateType = KaiTrade.Interfaces.PXUpdateType.trade;
                                        pxupdate.DriverTag = "Q";
                                        if (quote.HasVolume)
                                        {
                                            pxupdate.TradeVolume = quote.Volume;
                                        }
                                        else
                                        {
                                            pxupdate.TradeVolume = 0;
                                        }
                                        pxupdate.TradePrice = (decimal)quote.Price;
                                        pxupdate.ServerTicks = quote.ServerTimestamp.Ticks;
                                        if (m_PXContexts[instrument.FullName].IsUpdatedTrade(pxupdate))
                                        {
                                            ApplyPriceUpdate(pxupdate);
                                        }
                                        break;
                                    case eQuoteType.qtAsk:
                                        pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                                        pxupdate.Mnemonic = instrument.FullName;
                                        pxupdate.UpdateType = KaiTrade.Interfaces.PXUpdateType.ask;
                                        pxupdate.DriverTag = "Q";
                                        if (quote.HasVolume)
                                        {
                                            pxupdate.OfferSize = quote.Volume;
                                        }
                                        else
                                        {
                                            pxupdate.OfferSize = 0;
                                        }
                                        pxupdate.OfferPrice = (decimal)quote.Price;
                                        pxupdate.ServerTicks = quote.ServerTimestamp.Ticks;
                                        if (m_PXContexts[instrument.FullName].IsUpdatedOffer(pxupdate))
                                        {
                                            ApplyPriceUpdate(pxupdate);
                                        }

                                        break;
                                    case eQuoteType.qtBid:
                                        pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                                        pxupdate.Mnemonic = instrument.FullName;
                                        pxupdate.UpdateType = KaiTrade.Interfaces.PXUpdateType.bid;
                                        pxupdate.DriverTag = "Q";
                                        if (quote.HasVolume)
                                        {
                                            pxupdate.BidSize = quote.Volume;
                                        }
                                        else
                                        {
                                            pxupdate.BidSize = 0;
                                        }

                                        pxupdate.BidPrice = (decimal)quote.Price;
                                        pxupdate.ServerTicks = quote.ServerTimestamp.Ticks;
                                        if (m_PXContexts[instrument.FullName].IsUpdatedBid(pxupdate))
                                        {
                                            ApplyPriceUpdate(pxupdate);
                                        }
                                        break;
                                    default:
                                        pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                                        pxupdate.Mnemonic = instrument.FullName;
                                        pxupdate.UpdateType = KaiTrade.Interfaces.PXUpdateType.none;
                                        pxupdate.DriverTag = "Q";

                                        if (instrument.Bid.IsValid)
                                        {
                                            pxupdate.BidSize = instrument.Bid.Volume;
                                            pxupdate.BidPrice = (decimal)instrument.Bid.Price;
                                            pxupdate.Ticks = instrument.Bid.ServerTimestamp.Ticks;
                                        }
                                        if (instrument.Ask.IsValid)
                                        {
                                            pxupdate.OfferSize = instrument.Ask.Volume;
                                            pxupdate.OfferPrice = (decimal)instrument.Ask.Price;
                                            pxupdate.Ticks = instrument.Ask.ServerTimestamp.Ticks;
                                        }

                                        pxupdate.TradePrice = (decimal)instrument.Trade.Price;
                                        pxupdate.ServerTicks = instrument.ServerTimestamp.Ticks;

                                        if (instrument.Trade.HasVolume)
                                        {
                                            pxupdate.TradeVolume = instrument.Trade.Volume;
                                        }

                                        ApplyPriceUpdate(pxupdate);

                                        //publishQuote(instrument);
                                        break;
                                }
                            }
                        }
                        else
                        {
                        }
                        // there were no quotes


                        pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                        pxupdate.Mnemonic = instrument.FullName;
                        pxupdate.UpdateType = KaiTrade.Interfaces.PXUpdateType.none;
                        pxupdate.DriverTag = "I";

                        if (instrument.Bid.IsValid)
                        {
                            pxupdate.BidSize = instrument.Bid.Volume;
                            pxupdate.BidPrice = (decimal)instrument.Bid.Price;
                            pxupdate.Ticks = instrument.Bid.ServerTimestamp.Ticks;
                        }
                        if (instrument.Ask.IsValid)
                        {
                            pxupdate.OfferSize = instrument.Ask.Volume;
                            pxupdate.OfferPrice = (decimal)instrument.Ask.Price;
                            pxupdate.Ticks = instrument.Ask.ServerTimestamp.Ticks;
                        }
                        if (instrument.Trade.IsValid)
                        {
                            pxupdate.TradePrice = (decimal)instrument.Trade.Price;
                            pxupdate.ServerTicks = instrument.Ask.ServerTimestamp.Ticks;

                            if (instrument.Trade.HasVolume)
                            {
                                pxupdate.TradeVolume = instrument.Trade.Volume;
                            }
                        }


                        ApplyPriceUpdate(pxupdate);

                        //publishQuote(instrument);
                    }
                    catch (Exception ex)
                    {
                        log.Error("cel_InstrumentChanged", ex);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// This event is fired when a new order is added to the collection,
        /// order status is changed or the order is removed from the collection.
        /// </summary>
        /// <param name="change">The type of the occurred change.</param>
        /// <param name="order">CQGOrder object representing the order to which the change refers.</param>
        /// <param name="oldProperties">CQGOrderProperties collection representing the old
        /// values of the changed order properties.</param>
        /// <param name="fill">CQGFill object representing the last fill of the order.</param>
        /// <param name="objError">CQGError object representing either an error sent by CQG Gateway
        /// or an intermediate error.</param>
        private void cel_OrderChanged(eChangeType change, CQGOrder order, CQGOrderProperties oldProperties, CQGFill fill,
                                      CQGError objError)
        {
            try
            {
                updateOrder(change, order);
                string myText = "";
                switch (order.GWStatus)
                {
                    case eOrderStatus.osInOrderBook:                    
                    case eOrderStatus.osInCancel:
                    case eOrderStatus.osCanceled:
                    case eOrderStatus.osInModify:                    
                    case eOrderStatus.osExpired:
                    case eOrderStatus.osFilled:
                    case eOrderStatus.osInTransit:
                        break;

                    case eOrderStatus.osNotSent:
                        myText = "eOrderStatus.osNotSent" + order.LastError.Description;
                        Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osBusted:
                        myText = "eOrderStatus.osBusted";
                        Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osInTransitTimeout:
                        myText = "eOrderStatus.osInTransitTimeout" + order.LastError.Description;
                        Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osRejectFCM:
                        myText = "eOrderStatus.osRejectFCM:" + order.LastError.Description;
                        Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osRejectGW:
                        myText = "eOrderStatus.osRejectGW:" + order.LastError.Description;
                        Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    default:
                        myText = "eOrderStatus - not known:" + order.GWStatus.ToString();
                        Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("cel_OrderChanged:", ex);
            }
        }

        /// <summary>
        /// This event is fired once every second to synchronize with the exchange.
        /// </summary>
        /// <param name="dateTime">Date containing the line time (time on the current exchange)
        /// converted to the timezone.</param>
        private void cel_LineTimeChanged(DateTime dateTime)
        {
            try
            {
                lock (m_Token2)
                {
                    try
                    {
                        if (m_tickCount++ > 0)
                        {
                            TimeSpan myDiff = new TimeSpan(Math.Abs(dateTime.Ticks - DateTime.Now.Ticks));


                            if (myDiff.Seconds > 60)
                            {
                                if (m_TimeWarningCount < 2)
                                {
                                    // Data is slow
                                    m_TimeWarningCount++;
                                    string myMsg = "cel_LineTimeChanged: Difference in time too great CQG=:" + dateTime.ToString() + " Sys=" + DateTime.Now.ToString();
                                    this.SendAdvisoryMessage(myMsg);
                                    wireLog.Info(myMsg);
                                }
                            }
                            else
                            {
                                m_TimeWarningCount = 0;
                            }

                            this.OnUpdate("CQGTIME", dateTime.Ticks.ToString());

                            m_tickCount = 0;
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("cel_LineTimeChanged", myE);
                    }
                }
            }
            catch
            {
            }
        }


        #endregion

        public void test(string testField1, string testField2)
        {
            try
            {
                LogTradingSystemImage2(lastTradingSystem);
                //TradingSystemImage2(lastTradingSystem);
                
                //long pp = Facade.Factory.GetPositionPublisher().GetProductPosition(testField1);
                //long ap = Facade.Factory.GetPositionPublisher().GetAccountProductPosition(testField2, testField1);

                /*
                if (QNumber.Length > 0)
                {

                    m_CQGHostForm.CQGApp.NewInstrument(QNumber);
                }
                else
                {
                    m_CQGHostForm.CQGApp.DefineStrategy("SPREAD(F.US.EPM12-0.1*F.US.YMM12, L2, 0.25, 1:10)");
                    CQGInstrument ins = GetInstrument("SPREAD(F.US.EPM12-0.1*F.US.YMM12, L2, 0.25, 1:10)");

                    foreach (CQGStrategyDefinition s in m_CQGHostForm.CQGApp.AllStrategyDefinitions)
                    {
                         
                        string lclientId = s.ClientId;
                        string lrequestString = s.RequestString;
                        eStrategyRequestStatus lstatus = s.Status;
                        string lsymbol = s.Symbol;
                        string lid = s.Id;
                    }
                    m_CQGHostForm.CQGApp.DefineStrategy("K2SPREAD");
                }
                 */
                 
                
            }
            catch
            {
            }


        }







        private void DefineCQGStrategy(string instrumentFullName)
        {
            try
            {
                CQGStrategyDefinition sd = null;
                sd = m_CQGHostForm.CQGApp.DefineStrategy(instrumentFullName);
                if (m_ExpressionStrategyID.ContainsKey(instrumentFullName))
                {
                    m_ExpressionStrategyID[instrumentFullName] = sd.Id;
                }
                else
                {
                    m_ExpressionStrategyID.Add(instrumentFullName, sd.Id);
                }
                string lclientId = sd.ClientId;
                string lrequestString = sd.RequestString;
                eStrategyRequestStatus lstatus = sd.Status;
                string lsymbol = sd.Symbol;
                string lid = sd.Id;
            }
            catch (Exception myE)
            {
                log.Error("DefineCQGStrategy", myE);
            }
        }

        private CQGStrategyDefinition GetCQGStrategyDefinition(string instrumentFullName)
        {
            CQGStrategyDefinition sd = null;
            try
            {
                sd = m_CQGHostForm.CQGApp.DefineStrategy(instrumentFullName);
                if (m_ExpressionStrategyID.ContainsKey(instrumentFullName))
                {
                    string id = m_ExpressionStrategyID[instrumentFullName];
                    sd = m_CQGHostForm.CQGApp.AllStrategyDefinitions[id];

                    string lclientId = sd.ClientId;
                    string lrequestString = sd.RequestString;
                    eStrategyRequestStatus lstatus = sd.Status;
                    string lsymbol = sd.Symbol;
                    string lid = sd.Id;
                }
                
                
            }
            catch (Exception myE)
            {
                log.Error("GetCQGStrategyDefinition", myE);
            }
            return sd;
        }

        

        private void submitSpreadOrder(KaiTrade.Interfaces.Message myMsg, string execPatternName)
        {
            QuickFix.Message myQFOrder = null;
            try
            {
               
                // Extract the raw FIX Message from the inbound message
                string strOrder = myMsg.Data;
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("submitOrder:" + strOrder);
                }
                if (driverLog.IsInfoEnabled)
                {
                    driverLog.Info("submitOrder:" + strOrder);
                }

                // Use QuickFix to handle the message
                myQFOrder = new QuickFix.Message(strOrder);

                // Use product manager to validate the product specified on
                // the order exists for this adapter

                // Get the product associated with the FIX message


                QuickFix.Symbol symbol = new QuickFix.Symbol();
                QuickFix.Side side = new QuickFix.Side();
                QuickFix.OrdType ordType = new QuickFix.OrdType();
                QuickFix.OrderQty orderQty = new QuickFix.OrderQty();
                QuickFix.Price price = new QuickFix.Price();
                QuickFix.StopPx stopPx = new QuickFix.StopPx();
                QuickFix.Account account = new QuickFix.Account();
                QuickFix.StrikePrice strikePrice = new QuickFix.StrikePrice();
                QuickFix.Currency currency = new QuickFix.Currency();
                QuickFix.CFICode cfiCode = new QuickFix.CFICode();
                QuickFix.SecurityExchange exchange = new QuickFix.SecurityExchange();
                QuickFix.ClOrdID clOrdID = new QuickFix.ClOrdID();
                QuickFix.TimeInForce tif = new QuickFix.TimeInForce();
                QuickFix.ExpireDate expireDate = new QuickFix.ExpireDate();
                QuickFix.MaturityMonthYear MMY = new QuickFix.MaturityMonthYear();
                QuickFix.TargetStrategy targetStrategy = new QuickFix.TargetStrategy();

                // Get the CQG account
                CQGAccount myAccount = null;
                if (myQFOrder.isSetField(account))
                {
                    myQFOrder.getField(account);
                    myAccount = GetAccount(account.getValue());
                    if (myAccount != null)
                    {
                        driverLog.Info("CQGAcct:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
                    }
                }
                if (myAccount == null)
                {
                    this.SendAdvisoryMessage("CQG: you need to provide a valid account");
                    throw new NullReferenceException("No account is selected.");
                }
                CQGInstrument instrument = null;
                // We should now use the src
                QuickFix.SecurityID mySecID = new QuickFix.SecurityID();
                QuickFix.SecurityIDSource mySecIDSrc = new QuickFix.SecurityIDSource();
                if (myQFOrder.isSetField(mySecID))
                {
                    myQFOrder.getField(mySecID);
                    instrument = GetInstrument(mySecID.getValue());
                }
                if (instrument == null)
                {
                    // Get the CQG product/intrument we want to order
                    string myMnemonic = KaiUtil.QFUtils.GetProductMnemonic(m_ID, "", myQFOrder);

                    instrument = GetInstrumentWithMnemonic(myMnemonic);
                }
                if (instrument == null)
                {
                    this.SendAdvisoryMessage("CQG: invalid product/instrument requested - check your product sheet");
                    throw new NullReferenceException("No instrument is selected.");
                }


                CQGStrategyDefinition sd = GetCQGStrategyDefinition(mySecID.getValue());
               
                if (sd == null)
                {
                    throw new Exception("Can not access a strategy Definition");
                }
            
                // Get the Order type
                myQFOrder.getField(ordType);
                eOrderType orderType = getOrderType(ordType);


                // Get the QTY
                myQFOrder.getField(orderQty);
                int quantity = (int)orderQty.getValue();

                // Order side can be specified in two ways:
                //        * if UseOrderSide is set in APIConfig then we need to specify side
                //        explicitly, and order quantity must be greater than 0.
                //      * if setting below is not set the side is detected by order quantity sign.
                //        Negative quantity specifies sell side.
                // Here we use the second one
                myQFOrder.getField(side);
                eOrderSide orderSide = getOrderSide(side);

                if (orderSide == eOrderSide.osdSell)
                {
                    quantity *= -1;
                }

                // Default values of prices
                double limitPrice = 0.0;
                if (myQFOrder.isSetField(price))
                {
                    myQFOrder.getField(price);
                    limitPrice = price.getValue();
                }

                double stopPrice = 0.0;
                if ((myQFOrder.isSetField(stopPx)) && ((orderType == eOrderType.otStop) || (orderType == eOrderType.otStopLimit)))
                {
                    myQFOrder.getField(stopPx);
                    stopPrice = stopPx.getValue();
                    limitPrice = 0;
                }


                // Create order, since we have already all the needed parameters
                
                CQGOrder order = m_CQGHostForm.CQGApp.CreateStrategyOrder(orderType, sd, myAccount, null, quantity, eOrderSide.osdUndefined, limitPrice, stopPrice, "");
                // Set order parked status

                order.Properties[eOrderProperty.opParked].Value = false;


                // Set order duration
                eOrderDuration durationType = eOrderDuration.odDay;
                if (myQFOrder.isSetField(tif))
                {
                    myQFOrder.getField(tif);
                    durationType = getOrderDuration(tif);
                    order.Properties[eOrderProperty.opDurationType].Value = durationType;
                    if (durationType == eOrderDuration.odGoodTillDate)
                    {
                        if (myQFOrder.isSetField(expireDate))
                        {
                            myQFOrder.getField(expireDate);

                            DateTime myDate;
                            KaiUtil.KaiUtil.FromLocalMktDate(out  myDate, expireDate.getValue());

                            order.Properties[eOrderProperty.opGTDTime].Value = myDate;
                        }
                        else
                        {
                            this.SendAdvisoryMessage("CQG: no expire date given on a Good till date order");
                            throw new NullReferenceException("CQG: no expire date given on a Good till date order");
                        }
                    }
                }

                if (myQFOrder.isSetField(targetStrategy))
                {
                    myQFOrder.getField(targetStrategy); 
                }
                string execPatternKey = getExecPatternKey(execPatternName, KaiUtil.QFUtils.DecodeOrderType(ordType));
                string execPattern = "";
                if (m_ExecutionPatterns.ContainsKey(execPatternKey))
                {
                    execPattern = m_ExecutionPatterns[execPatternKey];
                    
                }
                else if (m_ExecutionPatterns.ContainsKey(execPatternName))
                {
                    // try the bare name i.e. with out the .LIMIT or .MARKET post fix
                    execPattern = m_ExecutionPatterns[execPatternName];
                    
                }

                driverLog.Info("ExecPatternKey:" + execPatternKey + " stringis:" + execPattern);

                CQGExecutionPattern debugCqgEP = m_CQGHostForm.CQGApp.CreateExecutionPattern(sd, orderType);
                string debugExecPattern = debugCqgEP.PatternString;
                driverLog.Info("CQG DefaultExecPatternKey:" + debugExecPattern );

                if (execPattern.Length == 0)
                {
                    CQGExecutionPattern cqgEP = m_CQGHostForm.CQGApp.CreateExecutionPattern(sd, orderType);
                    execPattern = cqgEP.PatternString;
                }
                order.Properties[eOrderProperty.opExecutionPattern].Value = execPattern;

                // Record the CQG order
                myQFOrder.getField(clOrdID);
                DriverBase.OrderContext myContext = new DriverBase.OrderContext();
                myContext.ExternalOrder = order;
                myContext.QFOrder = myQFOrder;
                myContext.ClOrdID = clOrdID.getValue();


                m_GUID2CQGOrder.Add(order.GUID, myContext);
                m_ClOrdIDOrderMap.Add(clOrdID.getValue(), myContext);

                // send the order
                driverLog.Info("CQGAcctA:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
                order.Place();
                myContext.CurrentCommand = DriverBase.ORCommand.Submit;
                driverLog.Info("CQGAcctB:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
            }
            catch (Exception myE)
            {
                log.Error("submitOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("CQG:submitOrder: problem submitting order:" + myE.ToString());

                QuickFix.OrdStatus myOrdStatus;
                QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.REJECTED);

                myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                QuickFix.OrderQty orderQty = new QuickFix.OrderQty();
                if (myQFOrder != null)
                {
                    myQFOrder.getField(orderQty);
                    QuickFix.OrdRejReason myRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.OTHER);
                    sendExecReport(myQFOrder, new QuickFix.OrderID("UNKNOWN"), myOrdStatus, myExecType, 0.0, (int)orderQty.getValue(), 0, 0, 0, myE.Message, myRejReason);
                }
            }
        }

        private string getExecPatternKey(string name, string ordType)
        {
            return name + "." + ordType;
        }
        private string getExecStrategyPattern(CQGStrategyDefinition sd, eOrderType orderType, string name)
        {
            string execStrategyPattern = "";

            string key = "";
            switch (orderType)
            {
                case eOrderType.otLimit:
                    key = KaiTrade.Interfaces.OrderType.LIMIT + "." + name;
                    break;
                case eOrderType.otMarket:
                    key = KaiTrade.Interfaces.OrderType.MARKET + "." + name;
                    break;
                case eOrderType.otStop:
                    key = KaiTrade.Interfaces.OrderType.STOP + "." + name;
                    break;
                case eOrderType.otStopLimit:
                    key = KaiTrade.Interfaces.OrderType.STOP + "." + name;
                    break;

            }
            if (m_ExecutionPatterns.ContainsKey(key))
            {
                execStrategyPattern = m_ExecutionPatterns[key];
            }
            else
            {
                CQGExecutionPattern cqgEP = m_CQGHostForm.CQGApp.CreateExecutionPattern(sd, orderType);
                execStrategyPattern = cqgEP.PatternString;
                m_ExecutionPatterns.Add(key, execStrategyPattern);
            }
            return execStrategyPattern;

        }

        public void AddExecStrategyPattern(string orderType, string name, string execStrategyPattern)
        {
            string key = orderType + "." + name;
            if (m_ExecutionPatterns.ContainsKey(key))
            {
                m_ExecutionPatterns[key] = execStrategyPattern;
            }
            else
            {
                m_ExecutionPatterns.Add(key, execStrategyPattern);
            }
        }

        
        public void FromFileJSONExecPatternString(string myDataPath)
        {
            FileStream file;
            StreamReader reader;

            try
            {
                file = new FileStream(myDataPath, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(file);
               

                while (!reader.EndOfStream)
                {
                    string epsData = reader.ReadLine();
                    ExecPatternString ps = Newtonsoft.Json.JsonConvert.DeserializeObject<ExecPatternString>(epsData);
                    AddExecStrategyPattern(ps.OrderType, ps.Name, ps.PatternString);
                }


                reader.Close();
                file.Close();

            }
            catch (Exception myE)
            {
                log.Error("FromFileJSONExecPatternString:" + myDataPath, myE);
            }
            finally
            {
                reader = null;
                file = null;
            }

        }
       

    }

   

}
