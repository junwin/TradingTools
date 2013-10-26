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
using K2ServiceInterface;

namespace KTACQG
{
    public partial class KTACQG : DriverBase.DriverBase
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
        private CQGConfig _config;


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


        private K2ServiceInterface.IDriverStatus  m_RunningState;

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

            _config = new CQGConfig();


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

            m_RunningState = new DriverBase.DriverStatus();

           
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
                if (base._state.HideDriverUI)
                {                   
                        //CQGHostForm.Hide();
                }
                // Read the configuration data
                try
                {
                    using (StreamReader sr = new StreamReader(_configPath + "CQGConfig.xml"))
                    {
                        _config = JsonConvert.DeserializeObject<CQGConfig>(sr.ReadToEnd());
                    }

                    // process config file
                    processConfig(_config);
                }
                catch(Exception e)
                {
                    // not consider an issue if not found
                    driverLog.Error("error processing config", e);
                }

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
                        /*
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
                        */

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
                /*
                m_CQGHostForm.CQGApp.OnQueryProgress += new _ICQGCELEvents_OnQueryProgressEventHandler(CQGApp_OnQueryProgress);                
                m_CQGHostForm.CQGApp.OrderChanged += new _ICQGCELEvents_OrderChangedEventHandler(cel_OrderChanged);
                 */
                
                // TS Data
                m_CQGHostForm.CQGApp.TimedBarsResolved += new CQG._ICQGCELEvents_TimedBarsResolvedEventHandler(CEL_TimedBarsResolved);
                m_CQGHostForm.CQGApp.TimedBarsAdded += new CQG._ICQGCELEvents_TimedBarsAddedEventHandler(CEL_TimedBarsAdded);
                m_CQGHostForm.CQGApp.TimedBarsUpdated += new CQG._ICQGCELEvents_TimedBarsUpdatedEventHandler(CEL_TimedBarsUpdated);

                // Constant Bar
                m_CQGHostForm.CQGApp.ConstantVolumeBarsResolved += new CQG._ICQGCELEvents_ConstantVolumeBarsResolvedEventHandler(CEL_ConstantVolumeBarsResolved);
                m_CQGHostForm.CQGApp.ConstantVolumeBarsAdded += new CQG._ICQGCELEvents_ConstantVolumeBarsAddedEventHandler(CEL_ConstantVolumeBarsAdded);
                m_CQGHostForm.CQGApp.ConstantVolumeBarsUpdated += new CQG._ICQGCELEvents_ConstantVolumeBarsUpdatedEventHandler(CEL_ConstantVolumeBarsUpdated);


                // Conditions
                /*
                //m_CQGHostForm.CQGApp.ConditionAdded += new _ICQGCELEvents_ConditionAddedEventHandler(CQGApp_ConditionAdded);
                m_CQGHostForm.CQGApp.ConditionDefinitionsResolved += new _ICQGCELEvents_ConditionDefinitionsResolvedEventHandler(CEL_ConditionDefinitionsResolved);
                m_CQGHostForm.CQGApp.ConditionResolved += new _ICQGCELEvents_ConditionResolvedEventHandler(CEL_ConditionResolved);
                m_CQGHostForm.CQGApp.ConditionUpdated += new _ICQGCELEvents_ConditionUpdatedEventHandler(CEL_ConditionUpdated);
                m_CQGHostForm.CQGApp.ConditionAdded += new _ICQGCELEvents_ConditionAddedEventHandler(CEL_ConditionAdded);
                */
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
                /*
                m_CQGHostForm.CQGApp.TradingSystemResolved += new _ICQGCELEvents_TradingSystemResolvedEventHandler(CQGApp_TradingSystemResolved);
                m_CQGHostForm.CQGApp.TradingSystemAddNotification += new _ICQGCELEvents_TradingSystemAddNotificationEventHandler(CQGApp_TradingSystemAddNotification);
                m_CQGHostForm.CQGApp.TradingSystemUpdateNotification += new _ICQGCELEvents_TradingSystemUpdateNotificationEventHandler(CQGApp_TradingSystemUpdateNotification);
                m_CQGHostForm.CQGApp.TradingSystemDefinitionsResolved += new _ICQGCELEvents_TradingSystemDefinitionsResolvedEventHandler(CQGApp_TradingSystemDefinitionsResolved);
                m_CQGHostForm.CQGApp.TradingSystemInsertNotification += new _ICQGCELEvents_TradingSystemInsertNotificationEventHandler(CQGApp_TradingSystemInsertNotification);
                 */
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

                if (_publisherRegister.ContainsKey(cqg_instrument.FullName))
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
                //m_Clients.Clear();


                m_SubscribedProducts.Clear();

                m_TSSets.Clear();
                _clOrdIDOrderMap.Clear();
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

        private void processConfig(CQGConfig  config)
        {
            try
            {
                
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
        private void subscribeProduct(KaiTrade.Interfaces.IProduct myDef)
        {
            try
            {
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("subscribeProduct:mne=" + myDef.Mnemonic + " src=:" + myDef.SecurityID);
                }
                // Record the Mnemnonic that wants a suscription - used later
                // to get a mnemonic from the long name returned by CQG
                if (m_SubscribedProducts.ContainsKey(myDef.SecurityID))
                {
                    m_SubscribedProducts[myDef.SecurityID] = myDef.Mnemonic;
                }
                else
                {
                    m_SubscribedProducts.Add(myDef.SecurityID, myDef.Mnemonic);
                }
                // Send subscription request
                m_CQGHostForm.CQGApp.NewInstrument(myDef.SecurityID);
                CQGInstrument myInstr = GetInstrument(myDef.SecurityID);
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

        private void unSubscribeProduct(KaiTrade.Interfaces.IProduct myDef)
        {
            try
            {
                // Record the Mnemnonic that wants a suscription - used later
                // to get a mnemonic from the long name returned by CQG
                if (m_SubscribedProducts.ContainsKey(myDef.SecurityID))
                {
                    m_SubscribedProducts.Remove(myDef.SecurityID);
                    CQGInstrument myInstr = GetInstrument(myDef.SecurityID);
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
                KaiTrade.Interfaces.IProduct myDef = null;
                KaiTrade.Interfaces.IProduct product = null;
                product = Facade.GetProductManager().GetProductMnemonic(myMnemonic);
                if (product != null)
                {
                    myInstr = GetInstrument(product.SecurityID);
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
        private CQGInstrument GetInstrument(KaiTrade.Interfaces.IProduct myDef)
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
                    
                    // use the descriptive name for the ID since we use that as the lookup on orders

                    // Register the account with consumers
                    AddAccount(descriptiveName, acc.GWAccountName, acc.FcmAccountID);


                }
            }
            catch (Exception myE)
            {
                log.Error("FillAccountsMap", myE);
            }
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

       

        protected override void DoSend(KaiTrade.Interfaces.IMessage myMsg)
        {
            lock (m_Token3)
            {
                try
                {
                    // Here are the typical FIX Messages you will receive to
                    // Support Order Routing
                    switch (myMsg.Label)
                    {
                            /*
                        case "D":
                            submitOrder(myMsg);
                            break;
                        case "F":
                            pullOrder(myMsg);
                            break;
                        case "G":
                            modifyOrder(myMsg);
                            break;
                             */
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

        protected override void SubscribeMD(KaiTrade.Interfaces.IProduct product, int depthLevels, string requestID)
        {
            try
            {
                // this will result in DoReg() being called on CQG thread
                m_CQGHostForm.Register(product, depthLevels, requestID);
            }
            catch (Exception myE)
            {
                log.Error("SubscribeMD", myE);
            }
        }

        protected override void UnSubscribeMD(KaiTrade.Interfaces.IPublisher pub)
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
                //   KaiTrade.Interfaces.IProduct myDef = m_ProductDefs[myKey] as KaiTrade.Interfaces.IProduct;
                //   unSubscribeProduct(myDef);
                //}
            }
            catch (Exception myE)
            {
                log.Error("UnSubscribeMD", myE);
            }
        }


        public void DoReg(KaiTrade.Interfaces.IProduct product, int depthLevels, string requestID)
        {
            try
            {
                // check if the subject is already registered and add or update
                // the map of subjects we are keeping - this can be used to
                // resubscribe

                // Decode the key(mnemonic) into an instrDef, in CQG's
                // case this will be ID and IDSRC

                string myKey = product.Mnemonic;


                // get the current full name for a generic
                if (_productRegister.ContainsKey(myKey))
                {
                    myKey = _productRegister[myKey].SecurityID;

                    // processing for generics - in this case they mnemonic is not the
                    // name of the product CQG uses so we alos need to add the publisher by its
                    // product full name so its correctly resolved on updates
                    /*
                    if (myKey != myPub.TopicID())
                    {
                        if (!_publisherRegister.ContainsKey(myKey))
                        {
                            _publisherRegister.Add(myKey, myPub);
                        }
                    }
                     */
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


        protected override void DoUnRegister(KaiTrade.Interfaces.IPublisher myPub)
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
                    KaiTrade.Interfaces.IProduct myDef = m_ProductDefs[myKey] as KaiTrade.Interfaces.IProduct;
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
                        m_RunningState.Prices = StatusConditon.good;
                        m_RunningState.HistoricData = StatusConditon.good;
                        break;

                    case eConnectionStatus.csConnectionDelayed:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "CQG Data Connection is Delayed");
                        this.SendAdvisoryMessage("CQG Data Connection is Delayed");
                        UpdateStatus("Data", "", "", "", KaiTrade.Interfaces.Status.error, "CQG Data Connection is Delayed");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        m_RunningState.Prices = StatusConditon.alert;
                        m_RunningState.HistoricData = StatusConditon.alert;
                        //Facade.RaiseAlert("KTACQG", "CQG Data Connection is Delayed", 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;

                    case eConnectionStatus.csConnectionDown:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.closed, "CQG Data Connection is Down");
                        this.SendAdvisoryMessage("CQG Data Connection is Down");
                        UpdateStatus("Data", "", "", "", KaiTrade.Interfaces.Status.closed, "CQG Data Connection is Down");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        m_RunningState.Prices = StatusConditon.error;
                        m_RunningState.HistoricData = StatusConditon.error;
                        //Facade.RaiseAlert("KTACQG", "CQG Data Connection is Down", 0, KaiTrade.Interfaces.ErrorLevel.fatal, KaiTrade.Interfaces.FlashMessageType.error);
                        break;

                    default:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "CQG Data Connection unknown connection status: " + newStatus.ToString());
                        UpdateStatus("Data", "", "", "", KaiTrade.Interfaces.Status.error, "CQG Data Connection unknown connection status: " + newStatus.ToString());
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        //SendAdvisoryMessage();
                        m_RunningState.Prices = StatusConditon.alert;
                        m_RunningState.HistoricData = StatusConditon.alert;
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
        public override IDriverStatus GetRunningStatus()
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
                        m_RunningState.OrderRouting = StatusConditon.good;

                        break;

                    case eConnectionStatus.csConnectionDelayed:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "CQG GW Connection is Delayed");
                        this.SendAdvisoryMessage("CQG GW Connection is Delayed");
                        UpdateStatus("GW", "", "", "", KaiTrade.Interfaces.Status.error, "CQG GW Connection is Delayed");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.error);
                        m_RunningState.OrderRouting = StatusConditon.alert;
                        //Facade.RaiseAlert("KTACQG", "CQG GW Connection is Delayed", 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;

                    case eConnectionStatus.csConnectionDown:
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.closed, "CQG GW Connection is Down");
                        this.SendAdvisoryMessage("CQG GW Connection is Down");
                        UpdateStatus("GW", "", "", "", KaiTrade.Interfaces.Status.closed, "CQG GW Connection is Down");
                        // set the status of any subscribed items
                        setSubscriptionsStatus(KaiTrade.Interfaces.Status.closed);
                        m_RunningState.OrderRouting = StatusConditon.error;
                        //Facade.RaiseAlert("KTACQG", "CQG GW Connection is Down", 0, KaiTrade.Interfaces.ErrorLevel.fatal, KaiTrade.Interfaces.FlashMessageType.error);
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
                    m_RunningState.OrderRouting = StatusConditon.error;
                    m_RunningState.Prices = StatusConditon.error;
                    m_RunningState.HistoricData = StatusConditon.error;
                    //Facade.RaiseAlert("KTACQG", "CQG DataError" + errorDescription, cqgErr.Code, KaiTrade.Interfaces.ErrorLevel.fatal, KaiTrade.Interfaces.FlashMessageType.error);
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
                L1PriceSupport.PXUpdateBase pxupdate = new L1PriceSupport.PXUpdateBase(m_ID);
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
                L1PriceSupport.PXUpdateBase pxupdate = new L1PriceSupport.PXUpdateBase(m_ID);
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
                            KaiTrade.Interfaces.ITSItem myItem = mySet.GetNewItem();

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
                                    KaiTrade.Interfaces.ITSItem myItem = mySet.GetNewItem();
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
                            KaiTrade.Interfaces.ITSItem myItem = mySet.GetNewItem();
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
                                    KaiTrade.Interfaces.ITSItem myItem = mySet.GetNewItem();
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
                                    KaiTrade.Interfaces.ITSItem myItem = mySet.GetNewItem();
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
                                    //KaiTrade.Interfaces.ITSItem myItem = mySet.GetNewItem();
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

        public void GetProduct(KaiTrade.Interfaces.IProduct myProduct, string myGenericName)
        {
            try
            {
                if (myGenericName.Trim().Length > 0)
                {
                    driverLog.Info("GetProduct:" + myGenericName);
                    _productRegister.Add(myGenericName, myProduct);
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
                        if (_productRegister.ContainsKey(symbol))
                        {
                            myProduct = _productRegister[symbol];
                            if (symbol != instrument.FullName)
                            {
                                if (_productGenericNameRegister.ContainsKey(instrument.FullName))
                                {
                                    _productGenericNameRegister[instrument.FullName] = symbol;
                                }
                                else
                                {
                                    _productGenericNameRegister.Add(instrument.FullName, symbol);
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
                            myProduct = Facade.GetProductManager().CreateProductWithSecID(instrument.FullName, Name, instrument.ExchangeAbbreviation, instrument.FullName, Name);
                            _productRegister.Add(symbol, myProduct);
                            
                        }
                        setProductValues(myProduct, symbol, instrument);
                        // This will case a register update to be sent to subscribers of the product manager - e.g. like the rabbit publisher
                        Facade.GetProductManager().RegisterProduct(myProduct);
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
                
                myProduct.TradeVenue = Name;
                myProduct.Commodity = instrument.Commodity;
                myProduct.Exchange = instrument.ExchangeAbbreviation;
                myProduct.SecurityID = instrument.FullName;
                myProduct.IDSource = Name;
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
                        if (!_publisherRegister.ContainsKey(instrument.FullName))
                        {
                            return;
                        }
                        if (!_pXContexts.ContainsKey(instrument.FullName))
                        {
                            _pXContexts.Add(instrument.FullName, new DriverBase.PXUpdateContext(instrument.FullName));
                        }
                        L1PriceSupport.PXUpdateBase pxupdate = null;
                        if (quotes.Count > 0)
                        {
                            foreach (CQGQuote quote in quotes)
                            {
                                switch (quote.Type)
                                {
                                    case eQuoteType.qtTrade:
                                        pxupdate = new L1PriceSupport.PXUpdateBase(m_ID);
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
                                        if (_pXContexts[instrument.FullName].IsUpdatedTrade(pxupdate))
                                        {
                                            ApplyPriceUpdate(pxupdate);
                                        }
                                        break;
                                    case eQuoteType.qtAsk:
                                        pxupdate = new L1PriceSupport.PXUpdateBase(m_ID);
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
                                        if (_pXContexts[instrument.FullName].IsUpdatedOffer(pxupdate))
                                        {
                                            ApplyPriceUpdate(pxupdate);
                                        }

                                        break;
                                    case eQuoteType.qtBid:
                                        pxupdate = new L1PriceSupport.PXUpdateBase(m_ID);
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
                                        if (_pXContexts[instrument.FullName].IsUpdatedBid(pxupdate))
                                        {
                                            ApplyPriceUpdate(pxupdate);
                                        }
                                        break;
                                    default:
                                        pxupdate = new L1PriceSupport.PXUpdateBase(m_ID);
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


                        pxupdate = new L1PriceSupport.PXUpdateBase(m_ID);
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

                            //this.OnUpdate("CQGTIME", dateTime.Ticks.ToString());

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
                //LogTradingSystemImage2(lastTradingSystem);
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

       

    }

   

}
