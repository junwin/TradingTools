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
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Timers;
using System.Threading;

using System.Reflection;
using Newtonsoft.Json;


namespace DriverBase
{
    

    /// <summary>
    /// Base class for drivers, provides status message handling and 
    /// session management
    /// </summary>
    public class DriverBase  : KaiTrade.Interfaces.IClient
    {

        /// <summary>
        /// Module information based on the drivers assembly
        /// </summary>
        IModuleDef _module;

        /// <summary>
        /// Holds that last status message sent to clients - used for status requests
        /// </summary>
        KaiTrade.Interfaces.IMessage m_LastStatus;

        /// <summary>
        /// You must define these in the main drievr class NOW IN BASE CLASS!!
        /// </summary>
        //private string Name = "";

        protected string m_ID = "";
        protected string m_Tag = "";
        protected string Name { get; set; }

        /// <summary>
        /// unique idenetity for this instance of a driver
        /// </summary>
        private string m_Identity;

        /// <summary>
        /// Clients of this driver
        /// </summary>
        protected List<KaiTrade.Interfaces.IClient> m_Clients;

        /// <summary>
        /// Parent Driver manager
        /// </summary>
        protected IDriverManager m_Parent;


        /// <summary>
        /// Create a logger for use in this Driver 
        /// </summary>
        public log4net.ILog m_Log;

        public log4net.ILog m_ORLog = log4net.LogManager.GetLogger("ORTalkTalk");
        public log4net.ILog m_TSLog = log4net.LogManager.GetLogger("TradeSystem");

        /// <summary>
        /// Create a logger to record low level details - a wire log
        /// </summary>
        public log4net.ILog m_WireLog;

        /// <summary>
        /// Create a logger to record driver specific info 
        /// </summary>
        public log4net.ILog m_DriverLog;

        /// <summary>
        /// Maps the CLOrdID of incomming requests to an
        /// order context
        /// </summary>
        protected Dictionary<string, OrderContext> m_ClOrdIDOrderMap;

        /// <summary>
        /// Maps some external ID to an Order context
        /// </summary>
        protected Dictionary<string, OrderContext> m_ApiIDOrderMap;

        /// <summary>
        /// Maps all active contexts using their ID
        /// </summary>
        protected Dictionary<string, OrderContext> m_ActiveContextMap;

        /// <summary>
        /// Used to keep track of our list of subjects accessed by their key
        /// </summary>
        protected Dictionary<string, KaiTrade.Interfaces.IPublisher> m_PublisherRegister;

        /// <summary>
        /// Register of products used by the driver
        /// </summary>
        protected Dictionary<string, KaiTrade.Interfaces.IProduct> m_ProductRegister;

        /// <summary>
        /// This maps a long product name to a Generic name, for example F.US.EPU2 --> EP
        /// it is used to locate a generic product whose menmonic is some symbol of short name
        /// at this point in tim this is only done in DOM processing
        /// </summary>
        protected Dictionary<string, string> m_ProductGenericNameRegister;

        /// <summary>
        /// Product defined in the driver, i.e. the drivers product object
        /// </summary>
        protected Dictionary<string, object> m_ExternalProduct;

        /// <summary>
        /// Keeps track of driver sessions using the a string key
        /// </summary>
        protected Dictionary<string, IDriverSession> m_Sessions;

        protected KaiTrade.Interfaces.Status m_Status = KaiTrade.Interfaces.Status.closed;

        /// <summary>
        /// Access to the kaitrade facade
        /// </summary>
        protected IFacade m_Facade = null;

        /// <summary>
        /// Path to config files
        /// </summary>
        protected string m_ConfigPath;

        /// <summary>
        /// The state of the driver as loaded from the app config
        /// </summary>
        protected IDriverState _state;

        /// <summary>
        /// state passed in with the start request - this should be XML used to construct m_State
        /// </summary>
        private string m_StartState = "";

        /// <summary>
        /// Not to run on live market
        /// </summary>
        protected bool m_LiveMarket = false;

        /// <summary>
        /// Timer for those algos that require some time based processing
        /// </summary>
        private System.Timers.Timer m_Timer;

        /// <summary>
        /// Timer interval used for the timer
        /// </summary>
        private long m_TimerInterval = 1000;

        private bool m_UseWatchDogStart = false;

        /// <summary>
        /// Watch dog thread - looks after starting BB API for lockups and 
        /// failed api starts
        /// </summary>
        private Thread m_WDThread;
        private bool m_RunWD = true;


        protected DriverStatus m_RunningState;

        /// <summary>
        /// Worker thread to handle Px messages
        /// </summary>
        private PxUpdateProcessor m_PxUpdateProcessor;
        private Thread m_PXUpdateThread;
        private Queue<KaiTrade.Interfaces.IPXUpdate> m_PxUpdateQueue;
        private SyncEvents m_SyncEvents;
        protected bool m_UseAsyncPriceUpdates = false;

        /// <summary>
        /// Worker thread to handle Inbound messages
        /// </summary>
        private MessageProcessorThread  m_InboundProcessor;
        private Thread m_InboundProcessorThread;
        private Queue<KaiTrade.Interfaces.IMessage> m_InboundProcessorQueue;
        private SyncEvents m_InboundProcessorSyncEvents;

        /// <summary>
        /// Worker thread to handle Outbound messages
        /// </summary>
        private MessageProcessorThread m_OutboundProcessor;
        private Thread m_OutboundProcessorThread;
        private Queue<KaiTrade.Interfaces.IMessage> m_OutboundProcessorQueue;
        private SyncEvents m_OutboundProcessorSyncEvents;
        

        /// <summary>
        /// Worker thread to handle replace messages
        /// </summary>
        private OrderReplaceProcessor  m_ReplaceProcessor;

        private Thread m_ReplaceUpdateThread;

        /// <summary>
        /// Stores cancel and modify request data
        /// </summary>
        private Queue<RequestData> m_ReplaceQueue;

        private SyncEvents m_ReplaceSyncEvents;

        /// <summary>
        /// are we supposed to queue replace requests?
        /// </summary>
        protected bool m_QueueReplaceRequests = false;
        

        protected string[] m_Currencies = { "AUD", "USD", "GBP", "GBp", "EUR", "JPY", "CHF", "CAD", "NZD" , "CZK", "DKK", "HUF", "PLN", "SEK", "NOK", "TRY", "ZAR"};

        /// <summary>
        /// Dictionary of update contexts - used by some driver(CQG) to detect duplicate updates
        /// </summary>
        protected Dictionary<string, PXUpdateContext> m_PXContexts;



        protected Dictionary<string, List<IPriceAgregator>> m_PriceAgregators;


        public DriverBase()
        {
            // Set up logging - will participate in the standard toolkit log
            m_Log = log4net.LogManager.GetLogger("KaiTrade");

            m_WireLog = log4net.LogManager.GetLogger("KaiTradeWireLog");
            m_DriverLog = log4net.LogManager.GetLogger("KaiDriverLog");

            _state = new DriverState();

            m_ClOrdIDOrderMap = new Dictionary<string, OrderContext>();
            m_ApiIDOrderMap = new Dictionary<string, OrderContext>();
            m_PXContexts = new Dictionary<string, PXUpdateContext>();

            m_Clients = new List<KaiTrade.Interfaces.IClient>();
            m_PublisherRegister = new Dictionary<string, KaiTrade.Interfaces.IPublisher>();
            m_ProductGenericNameRegister = new Dictionary<string, string>();
            m_ProductRegister = new Dictionary<string, KaiTrade.Interfaces.IProduct>();
            m_ExternalProduct = new Dictionary<string, object>();
            m_Sessions = new Dictionary<string, IDriverSession>();
            m_PriceAgregators = new Dictionary<string, List<IPriceAgregator>>();
            m_Identity = System.Guid.NewGuid().ToString();

            // setup the components info - used on status messages
            SetComponentInfo(out _module, this, this.m_ID, "DriverManager", 0, "");

            // creat a last status object - used to report status when requested
            m_LastStatus = new K2DataObjects.Message();

            m_LastStatus.Label = "Status";
            m_LastStatus.Data = "Loaded";
            m_LastStatus.AppState = (int)KaiTrade.Interfaces.Status.loaded;
            m_ActiveContextMap = new Dictionary<string, OrderContext>();
            m_RunningState = new DriverStatus();


            m_Log.Info("MainMessageHandler Created");


            m_PxUpdateQueue = new Queue<KaiTrade.Interfaces.IPXUpdate>();
            m_SyncEvents = new SyncEvents();
            m_PxUpdateProcessor = new PxUpdateProcessor(this, m_PxUpdateQueue, m_SyncEvents);
            m_PXUpdateThread = new Thread(m_PxUpdateProcessor.ThreadRun);
            m_PXUpdateThread.Start();


            m_ReplaceQueue = new Queue<RequestData>();
            m_ReplaceSyncEvents = new SyncEvents();
            m_ReplaceProcessor = new OrderReplaceProcessor(this, m_ReplaceQueue, m_ReplaceSyncEvents);
            m_ReplaceUpdateThread = new Thread(m_ReplaceProcessor.ThreadRun);
            m_ReplaceUpdateThread.Start();



            m_InboundProcessorQueue = new Queue<KaiTrade.Interfaces.IMessage>();
            m_InboundProcessorSyncEvents = new SyncEvents();           
            m_InboundProcessor = new MessageProcessorThread(this, m_InboundProcessorQueue, m_InboundProcessorSyncEvents);
            m_InboundProcessorThread = new Thread(m_InboundProcessor.ThreadRun);
            m_InboundProcessorThread.Start();

            m_OutboundProcessorQueue = new Queue<KaiTrade.Interfaces.IMessage>();
            m_OutboundProcessorSyncEvents = new SyncEvents();
            m_OutboundProcessor = new MessageProcessorThread(ref m_Clients, m_OutboundProcessorQueue, m_OutboundProcessorSyncEvents);
            m_OutboundProcessorThread = new Thread(m_OutboundProcessor.ThreadRun);
            m_OutboundProcessorThread.Start();

        }

        public string[] Currencies
        {
            get { return m_Currencies; }
            set { m_Currencies = value; }
        }

        public IFacade Facade
        {get{ return m_Facade;}}

        protected void StartTimer()
        {
            try
            {
                if (m_TimerInterval > 0)
                {
                    m_Timer = new System.Timers.Timer(m_TimerInterval);
                    m_Timer.Elapsed += new ElapsedEventHandler(OnTimer);
                    m_Timer.Interval = (double)m_TimerInterval;
                    m_Timer.Enabled = true;
                }
            }
            catch (Exception myE)
            {
            }
        }

        protected void StopTimer()
        {
            try
            {
            if (m_Timer != null)
            {
                m_Timer.Enabled = false;
            }
            m_Timer = null;
             }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// called on each timer tick - implimented by the base class
        /// </summary>
        protected virtual void TimerTick()
        {
        }
        private void OnTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                
                TimerTick();
            }
            catch (Exception myE)
            {
                m_Log.Error("OnTimer", myE);
            }
        }
        /// <summary>
        /// get/set timer interval - note this doen not change a running timer
        /// </summary>
        public long TimerInterval
        {
            get { return m_TimerInterval; }
            set { m_TimerInterval = value; }
        }

        /// <summary>
        /// Display or Hide any UI the driver has
        /// </summary>
        /// <param name="uiVisible">true => show UI, False => hide UI</param>
        public virtual void ShowUI(bool uiVisible)
        {
            // needs to be done by the subclass
        }

        /// <summary>
        /// Add a session with the name specified
        /// </summary>
        /// <param name="myName"></param>
        protected IDriverSession AddSession(string myName)
        {
            try
            {
                return AddSession(myName, "", "", "");
            }
            catch (Exception myE)
            {
                m_Log.Error("AddSession", myE);
                return null;
            }
        }

        /// <summary>
        /// Add a session with the details specified
        /// </summary>
        /// <param name="myName"></param>
        /// <param name="myVersion"></param>
        /// <param name="myTID"></param>
        /// <param name="mySID"></param>
        protected IDriverSession AddSession(string myName, string myVersion, string myTID, string mySID)
        {
            try
            {
                IDriverSession mySession = new DriverSession(this, myName, myVersion, myTID, mySID);
                string myKey = mySession.Key;
                if (m_Sessions.ContainsKey(myKey))
                {
                    m_Sessions[myKey] = mySession;
                }
                else
                {
                    m_Sessions.Add(myKey, mySession);
                }
                return mySession;
            }
            catch (Exception myE)
            {
                m_Log.Error("AddSession2", myE);
                return null;
            }
        }


        /// <summary>
        /// Update a session status adding a session if needed
        /// </summary>
        /// <param name="myName"></param>
        /// <param name="myVersion"></param>
        /// <param name="myTID"></param>
        /// <param name="mySID"></param>
        /// <param name="myState"></param>
        /// <param name="myStatusText"></param>
        protected void UpdateStatus(string myName, string myVersion, string myTID, string mySID, KaiTrade.Interfaces.Status myState, string myStatusText)
        {
            try
            {
                string mySessionKey = this.m_ID + ":" + myName;
                IDriverSession mySession = null;
                if (m_Sessions.ContainsKey(mySessionKey))
                {
                    mySession = m_Sessions[mySessionKey];
                }
                else
                {
                    mySession = this.AddSession(myName, myVersion, myTID, mySID);
                }
                mySession.State = myState;
                mySession.StatusText = myStatusText;
            }
            catch (Exception myE)
            {
                m_Log.Error("UpdateStatus", myE);
            }
        }

        /// <summary>
        /// Send a simple advisory message to all clients of the 
        /// adapter
        /// </summary>
        /// <param name="myMessageText"></param>
        public void SendAdvisoryMessage(string myMessageText)
        {
            try
            {
                m_DriverLog.Info(Name + " advisory:"  + myMessageText);
                IDriverStatusMessage myDSM;
                setupAdvisory(out myDSM, myMessageText);
                KaiTrade.Interfaces.IMessage myMessage = new K2DataObjects.Message();
                
                myMessage.Label = "DriverAdvisory";
                myMessage.Data = JsonConvert.SerializeObject(myDSM);
                SendStatusMessage(myMessage);
            }
            catch (Exception myE)
            {
                m_Log.Error("SendAdvisoryMessage", myE);
            }
        }

        /// <summary>
        /// Set up an advisory message - this give any clients general purpose advisory
        /// information - note that regular status messages should be used for any type of error
        /// </summary>
        /// <param name="myAdvisory"></param>
        /// <param name="myText"></param>
        private void setupAdvisory(out IDriverStatusMessage myDSM, string myText)
        {
            myDSM = new DriverStatusMessage();
            try
            {
                
                myDSM.Text = myText;
                myDSM.DriverCode = this.m_ID;
                myDSM.Module = _module.ToString();
            }
            catch (Exception myE)
            {
                m_Log.Error("setupAdvisory", myE);
            }

        }


        /// <summary>
        /// Send some message back to our clients
        /// </summary>
        /// <param name="myMessage"></param>
        public void SendMessage(KaiTrade.Interfaces.IMessage myMessage)
        {
            try
            {
                // do the update assync
                lock (((ICollection)m_OutboundProcessorQueue).SyncRoot)
                {
                    m_OutboundProcessorQueue.Enqueue(myMessage);
                    m_OutboundProcessorSyncEvents.NewItemEvent.Set();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("SendMessage", myE);
            }

            

        }
        /// <summary>
        /// Send a FIX style response to our clients
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="myResponseMsg"></param>
        public void sendResponse(string msgType, string myResponseMsg)
        {
            try
            {
                // Create message envelope
                KaiTrade.Interfaces.IMessage myMsg = new K2DataObjects.Message();

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
                SendMessage(myMsg);
            }
            catch (Exception myE)
            {
                m_Log.Error("sendResponse", myE);
            }
        }

        /// <summary>
        /// Resend the last status message
        /// </summary>
        public void SendLastStatusMessage()
        {
            try
            {
                SendStatusMessage(m_LastStatus);
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Send a status message to all of our clients
        /// </summary>
        /// <param name="myMessage"></param>
        public void SendStatusMessage(KaiTrade.Interfaces.IMessage myMessage)
        {
            try
            {
                // only preserve status messages - do not save an advisory
                if (myMessage.Label == "DriverStatus")
                {
                    m_LastStatus = myMessage;
                }
                foreach (KaiTrade.Interfaces.IClient myClient in m_Clients)
                {
                    if (myClient != null)
                    {
                        try
                        {
                            myClient.OnStatusMessage(myMessage);
                        }
                        catch (Exception myE)
                        {
                            //m_Log.Error("SendStatusMessage - invoke", myE);
                        }
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("SendStatusMessage", myE);
            }

        }

        /// <summary>
        /// Setup a general purpose status message
        /// </summary>
        /// <param name="myStatus"></param>
        /// <param name="myState"></param>
        /// <param name="myText"></param>
        private void setupStatus(out IDriverStatusMessage myDSM, KaiTrade.Interfaces.Status myState, string myText)
        {
            myDSM = null;
            try
            {
                setupAdvisory(out myDSM, myText);
                myDSM.State = (int)myState;
            }
            catch (Exception myE)
            {
                m_Log.Error("setupStatus", myE);
            }

        }

        /// <summary>
        /// Send a FIX style status message specifing all parameters
        /// </summary>
        /// <param name="myState"></param>
        /// <param name="myText"></param>
        /// <param name="myBegin"></param>
        /// <param name="mySID"></param>
        /// <param name="myTID"></param>
        /// <param name="myFixName"></param>
        public void SendStatusMessage(KaiTrade.Interfaces.Status myState, string myText, string myBegin, string mySID, string myTID, string myFixName)
        {
            try
            {
                // Update a FIX session status
                UpdateStatus(myFixName, myBegin, myTID, mySID, myState, myText);

                IDriverStatusMessage myDSM;

                setupStatus(out myDSM, myState, myText);

                IDriverSession mySession = new DriverSession();
                mySession.SessionName = myFixName;              
                mySession.BeginString = myBegin;
                mySession.SID = mySID;
                mySession.TID = myTID;
                mySession.UserName = myFixName;
                mySession.State = myState;
                mySession.Text = myText;
                myDSM.Sessions.Add(mySession);
                
                KaiTrade.Interfaces.IMessage statusMsg = new K2DataObjects.Message();
                statusMsg.Format = "XML";
                statusMsg.Label = "DriverStatus";
                statusMsg.Data = JsonConvert.SerializeObject(myDSM);
                SendStatusMessage(statusMsg);
                m_LastStatus = statusMsg;
            }
            catch (Exception myE)
            {
                m_Log.Error("SendStatusMessage", myE);
            }

        }



        /// <summary>
        /// send a stuts message
        /// </summary>
        /// <param name="myState"></param>
        /// <param name="myText"></param>
        public void SendStatusMessage(KaiTrade.Interfaces.Status myState, string myText)
        {
            try
            {
                m_DriverLog.Info(Name + ":" +myState.ToString() +":" + myText);
                // update the base session status
                UpdateStatus("DriverStatus", "", "", "", myState, myText);

                IDriverStatusMessage myDSM;

                setupStatus(out myDSM, myState, myText);

                KaiTrade.Interfaces.IMessage statusMsg = new K2DataObjects.Message();
                statusMsg.Format = "XML";
                statusMsg.Label = "DriverStatus";
                statusMsg.Data = JsonConvert.SerializeObject(myDSM);
                SendStatusMessage(statusMsg);
                m_LastStatus = statusMsg;
            }
            catch(Exception myE)
            {
                m_Log.Error("SendStatusMessage", myE);
            }
        }


        public static void SetComponentInfo(out IModuleDef myModule, Object myObj, string myModuleName, string myPackageName, int nInstance, string myTag)
        {
            try
            {
                myModule = new ModuleDef();
                myModule.Name = myModuleName;
                myModule.PackageFullName = myPackageName;
                myModule.Instance = nInstance;
                myModule.HostName = myPackageName + ":" + nInstance.ToString();
                myModule.Server = System.Environment.MachineName;


                // Other component info

                Assembly myAssy = myObj.GetType().Assembly;

                string myAssyFullName = myAssy.FullName;

                myModule.PackageFullName = myAssyFullName;
                int myVersionStart = myAssyFullName.IndexOf("Version=");
                myVersionStart += 8;
                int myCultureStart = myAssyFullName.IndexOf("Culture=");

                string myAssyVersion = myAssyFullName.Substring(myVersionStart, myCultureStart - myVersionStart);
                myAssyVersion = myAssyVersion.Trim();

                myModule.PackageVersion = myAssyVersion;


                Process myCurrentProcess = Process.GetCurrentProcess();
                myModule.HostModule = myCurrentProcess.MainModule.ModuleName;
                myModule.HostFileName = myCurrentProcess.MainModule.FileName;
                myModule.HostVersionInfo = myCurrentProcess.MainModule.FileVersionInfo.ToString();

                myModule.Tag = myTag;



            }
            catch (Exception myE)
            {
                //m_Log.Error("SetComponentInfo", myE);
                myModule = null;
            }

        }

        /// <summary>
        /// Will apply any replace requests that are pending
        /// </summary>
        public void ApplyAnyPendingReplace()
        {
            try
            {
                m_ReplaceSyncEvents.NewItemEvent.Set();
            }
            catch (Exception myE)
            {
                m_Log.Error("ApplyAnyPendingReplace", myE);
            }
        }

        /// <summary>
        /// Apply new replace requests
        /// </summary>
        /// <param name="myMsg"></param>
        public void ApplyReplaceRequest(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                if (1==1)
                {
                    string mnemonic = "";
                    KaiTrade.Interfaces.IModifyOrderRequst modifyRequest;
                    modifyRequest = JsonConvert.DeserializeObject<K2DataObjects.ModifyOrderRequest>(myMsg.Data);
                    ModifyRequestData r = new ModifyRequestData(crrType.cancel, modifyRequest);

                  

                    // Get the context - we must have this to access the order
                    if (m_ClOrdIDOrderMap.ContainsKey(r.OrigClOrdID))
                    {
                        r.OrderContext = m_ClOrdIDOrderMap[r.OrigClOrdID];
                        m_ClOrdIDOrderMap.Add(r.ClOrdID, r.OrderContext);
                    }
                    else
                    {
                    
                        //sendCancelReplaceRej(replaceData.LastQFMod, QuickFix.CxlRejReason.UNKNOWN_ORDER, "an order does not exist for the modifyOrder");
                        Exception myE = new Exception("an order does not exist for the modifyOrder");
                        throw myE;
                    }

                    try
                    {
                        if (m_ORLog.IsInfoEnabled)
                        {
                            m_ORLog.Info("modifyOrder:context" + r.OrderContext.ToString());
                            m_ORLog.Info("modifyOrder:order" + r.OrderContext.ExternalOrder.ToString());
                        }
                    }
                    catch
                    {
                    }

                    // swap the clordid to the new ones  on our stored order

                    r.OrderContext.ClOrdID =r.ClOrdID;
                    r.OrderContext.OrigClOrdID = r.OrigClOrdID;

                    // do the update assync
                    lock (((ICollection)m_ReplaceQueue).SyncRoot)
                    {
                        m_ReplaceQueue.Enqueue(r);
                        m_ReplaceSyncEvents.NewItemEvent.Set();
                    }
                }
                else
                {
                    //sync update
                    //DoApplyPriceUpdate(update);
                }



            }
            catch (Exception myE)
            {
                m_Log.Error("ApplyReplaceRequest", myE);
            }
        }


        /// <summary>
        /// Apply new cancel requests
        /// </summary>
        /// <param name="myMsg"></param>
        public void ApplyCancelRequest(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                if (1 == 1)
                {
                    string mnemonic = "";
                    KaiTrade.Interfaces.ICancelOrderRequest cancelRequest;
                    cancelRequest = JsonConvert.DeserializeObject<K2DataObjects.CancelOrderRequest>(myMsg.Data);
                    CancelRequestData r = new CancelRequestData(crrType.cancel, cancelRequest);

                    // Get the context - we must have this to access the order
                    if (m_ClOrdIDOrderMap.ContainsKey(cancelRequest.OrigClOrdID))
                    {
                        r.OrderContext = m_ClOrdIDOrderMap[cancelRequest.OrigClOrdID];
                        m_ClOrdIDOrderMap.Add(r.ClOrdID, r.OrderContext);
                    }
                    else
                    {

                        //sendCancelReplaceRej(replaceData.LastQFMod, QuickFix.CxlRejReason.UNKNOWN_ORDER, "an order does not exist for the modifyOrder");
                        Exception myE = new Exception("an order does not exist for the cancelOrder");
                        throw myE;
                    }

                    try
                    {
                        if (m_ORLog.IsInfoEnabled)
                        {
                            m_ORLog.Info("cancelOrder:context" + r.OrderContext.ToString());
                            m_ORLog.Info("cancelOrder:order" + r.OrderContext.ExternalOrder.ToString());
                        }
                    }
                    catch
                    {
                    }

                    
                    r.OrderContext.ClOrdID = r.ClOrdID;
                    r.OrderContext.OrigClOrdID = r.ClOrdID;

                    // do the update assync
                    lock (((ICollection)m_ReplaceQueue).SyncRoot)
                    {
                        m_ReplaceQueue.Enqueue(r);
                        m_ReplaceSyncEvents.NewItemEvent.Set();
                    }
                }
                else
                {
                    //sync update
                    //DoApplyPriceUpdate(update);
                }



            }
            catch (Exception myE)
            {
                m_Log.Error("ApplyReplaceRequest", myE);
            }
        }

        public void ApplyDOMUpdate(string mnemonic, KaiTrade.Interfaces.IDOMSlot[] slots)
        {
            try
            {
                if (slots.Length > 0)
                {
                    K2Depth.K2DOM DOM = Factory.Instance().GetProductDOM(mnemonic,slots[0].Price);
                   
                    DOM.Update(slots);
                    
                }
            }
            catch (Exception myE)
            {
            }
        }

        public void ApplyDOMUpdate(string mnemonic, KaiTrade.Interfaces.IPXUpdate update)
        {
            try
            {

                K2Depth.K2DOM dom = Factory.Instance().GetProductDOM(mnemonic);
                if (dom == null)
                {
                    // must have a least a price
                    decimal? initPx = null;
                    if (update.BidPrice.HasValue)
                    {
                        initPx = update.BidPrice;
                    }
                    else if (update.OfferPrice.HasValue)
                    {
                        initPx = update.OfferPrice;
                    }
                    else if (update.TradePrice.HasValue)
                    {
                        initPx = update.TradePrice;
                    }
                    if (initPx.HasValue)
                    {
                        dom = Factory.Instance().GetProductDOM(mnemonic, initPx.Value);

                    }
                }
                dom.Update(update);

            }
            catch (Exception myE)
            {
            }
        }
        public void ApplyPriceUpdate(KaiTrade.Interfaces.IPXUpdate update)
        {
            try
            {
                if (m_UseAsyncPriceUpdates)
                {
                    // do the update assync
                    lock (((ICollection)m_PxUpdateQueue).SyncRoot)
                    {
                        m_PxUpdateQueue.Enqueue(update);
                        m_SyncEvents.NewItemEvent.Set();
                    }
                }
                else
                {
                    //sync update
                    //DoApplyPriceUpdate(update);
                    // do the update assync
                    lock (((ICollection)m_PxUpdateQueue).SyncRoot)
                    {
                        m_PxUpdateQueue.Enqueue(update);
                        m_SyncEvents.NewItemEvent.Set();
                    }
                }

                

            }
            catch (Exception myE)
            {
                m_Log.Error("ApplyPriceUpdate", myE);
            }
        }

        public  void DoApplyPriceUpdate(KaiTrade.Interfaces.IPXUpdate update)
        {
            try
            {
                // try to get a L1PriceSupport.PXPublisher
                //L1PriceSupport.PXPublisher myL1PriceSupport.PXPublisher = m_PublisherRegister[myMnemonic] as L1PriceSupport.PXPublisher;
                if (!m_PublisherRegister.ContainsKey(update.Mnemonic))
                {
                    return;
                }

                KaiTrade.Interfaces.IProduct product = Factory.Instance().GetProductManager().GetProductMnemonic(update.Mnemonic);
                if (product != null)
                {
                    Factory.Instance().ApplyUpdate(update);
                }



                KaiTrade.Interfaces.IPublisher myPublisher = m_PublisherRegister[update.Mnemonic] as KaiTrade.Interfaces.IPublisher;

                if (myPublisher != null)
                {

                    L1PriceSupport.PXPublisher myPXPublisher = myPublisher as L1PriceSupport.PXPublisher;

                     if (myPXPublisher != null)
                     {
                         myPXPublisher.ApplyUpdate(update);
                         
                         myPublisher.OnUpdate(null);
                         if (myPXPublisher.Status != KaiTrade.Interfaces.Status.open)
                         {
                             myPXPublisher.Status = KaiTrade.Interfaces.Status.open;
                         }

                     }
                     else
                     {

                         // to do prices in the new way we expect that the pub impliments the price update interface
                         KaiTrade.Interfaces.IPXUpdate target = myPublisher as KaiTrade.Interfaces.IPXUpdate;
                         if (target != null)
                         {
                             // this will apply the update to the publisher
                             target.From(update);
                         }
                     }
                }

                if(m_PriceAgregators.ContainsKey(update.Mnemonic))
                {
                    foreach (IPriceAgregator pa in m_PriceAgregators[update.Mnemonic])
                    {
                        pa.ProcessPrice(update);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("DoApplyPriceUpdate", myE);
            }
        }

        protected virtual void SubscribeMD(KaiTrade.Interfaces.IPublisher myPub, int depthLevels, string requestID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void UnSubscribeMD(KaiTrade.Interfaces.IPublisher myPub)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// The base implementation records the publisher and then calls
        /// subscribeMD to the impliemting class so it can then use the
        /// subject for its owb registration
        /// </summary>
        /// <param name="myPublisher"></param>
        protected virtual void DoRegister(KaiTrade.Interfaces.IPublisher myPublisher, int depthLevels, string requestID)
        { 
            try
            {
                // a PXPublisher - we only support these for prices
                L1PriceSupport.PXPublisher myPXPub = myPublisher as L1PriceSupport.PXPublisher;
                if (myPXPub == null)
                {
                    // we allow publishers (new style) that impliment the PXUpdae to work as well
                    KaiTrade.Interfaces.IPXUpdate upd = myPublisher as KaiTrade.Interfaces.IPXUpdate;
                    if (upd == null)
                    {
                        throw new Exception("Not a valid publisher type - only L1PriceSupport.PXPublishers allowed here");
                    }

                }

                // check if the subject is already registered and add or update 
                // the map of subjects we are keeping - this can be used to 
                // resubscribe

                string myKey = myPublisher.TopicID();

                // add the publisher to our register
                if (m_PublisherRegister.ContainsKey(myKey))
                {
                    m_PublisherRegister[myKey] = myPublisher;
                }
                else
                {
                    m_PublisherRegister.Add(myKey, myPublisher);
                }

                myPublisher.Status = KaiTrade.Interfaces.Status.opening;

                SubscribeMD(myPublisher, depthLevels, requestID);
            }
            catch (Exception myE)
            {
            }
        }

        protected virtual void DoSend(KaiTrade.Interfaces.IMessage myMsg)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual OrderReplaceResult modifyOrder(ModifyRequestData replaceData)
        {
            return OrderReplaceResult.error;
        }
        public virtual OrderReplaceResult cancelOrder(CancelRequestData replaceData)
        {
            return OrderReplaceResult.error;
        }

        protected virtual void DoStart(string myState)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void DoStop()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void DoUnRegister(KaiTrade.Interfaces.IPublisher myPublisher)
        {
            try
            {
            // a L1PriceSupport.PXPublisher - we only support these for prices
            L1PriceSupport.PXPublisher myPXPub = myPublisher as L1PriceSupport.PXPublisher;
            if (myPXPub == null)
            {
                throw new Exception("Not a valid publisher type - only L1PriceSupport.PXPublishers allowed here");
            }

            if (m_PublisherRegister.ContainsKey(myPublisher.TopicID()))
            {
                m_PublisherRegister.Remove(myPublisher.TopicID());
            }

            UnSubscribeMD(myPXPub);
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Get a set of TS data - the driver will implement this if supported
        /// </summary>
        /// <param name="myTSSet"></param>
        protected virtual void DoGetTSData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            throw new Exception("TS1 Data requests are not supported.");
        }

        public bool IsCurrencyPair(string myPair)
        {
            bool bRet = false;
            try
            {
                string currA = "";
                string currB = "";
                if (myPair.Length == 6)
                {
                    currA = myPair.Substring(0, 3);
                    currB = myPair.Substring(3, 3);
                    if (IsCurrrenyCode(currA) && IsCurrrenyCode(currB))
                    {
                        return true;
                    }
                }
                else if (myPair.Length == 7)
                {
                    currA = myPair.Substring(0, 3);
                    currB = myPair.Substring(4, 3);
                    if (IsCurrrenyCode(currA) && IsCurrrenyCode(currB))
                    {
                        return true;
                    }
                }
                 
            }
            catch (Exception myE)
            {
                m_Log.Error("IsCurrencyPair", myE);
            }

            return bRet;
        }

        public bool IsCurrrenyCode(string myCurrency)
        {
            bool bRet = false;
            try
            {
            foreach (string myListCurr in m_Currencies)
            {
                if (myListCurr == myCurrency)
                {
                    return true;
                }
            }
            }
            catch (Exception myE)
            {
            }
            return bRet;
        }

        #region Driver Members

        List<KaiTrade.Interfaces.IClient> Clients
        {
            get { return m_Clients; }
        }

        IDriverState GetState()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        string ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
            }
        }

       


        void Register(KaiTrade.Interfaces.IPublisher publisher, int depthLevels, string requestID)
        {
            try
            {
                DoRegister(publisher, depthLevels, requestID);

            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.Register:publisher", myE);
            }
        }

        /// <summary>
        /// Will request any trade systems that the driver supports - note that this
        /// is asyncronous the driver will add any trading systems using the Facade
        /// </summary>
        public virtual void RequestTradeSystems()
        {
        }

        /// <summary>
        /// Request any conditions that the driver supports- note that this
        /// is asyncronous the driver will add any conditions using the Facade
        /// </summary>
        public virtual void RequestConditions()
        {
        }


        /// <summary>
        /// Request that a driver opens/subscribes to a product - this needs to
        /// be done by the base class since that knows the API/Broker to
        /// open or get some product if it exists
        /// </summary>
        /// <param name="myProductXml">string of XML that defines the product</param>
        public virtual void GetProduct(string myProductXml)
        {
            try
            {
                m_Log.Error("GetProduct - Driver Base class cannot process");

            }
            catch (Exception myE)
            {
                m_Log.Error("GetProduct", myE);
            }
        }

        /// <summary>
        /// Request that a driver opens/subscribes to a product 
        /// </summary>
        /// <param name="myProduct">Tradable product - to get or request</param>
        /// <param name="myGenericName">The generic name for the product - i.e. not time sensitive
        ///  for example the S&P emini can be refered to as EP in CQG, it then may resolve to
        ///  EPM9, EPZ9  .... depending on which is the active front contract</param>
        public virtual void GetProduct(KaiTrade.Interfaces.IProduct myProduct, string myGenericName)
        {
            try
            {
                m_Log.Error("GetProduct:product i/f - Driver Base class cannot process");

            }
            catch (Exception myE)
            {
                m_Log.Error("GetProduct", myE);
            }
        }

        /// <summary>
        /// Request the product details, get the driver to access the product and fill in 
        /// product details in the kaitrade product object.
        /// Note that not all drivers support this and that the call may take some
        /// time to set the values.
        /// </summary>
        /// <param name="myProduct"></param>
        public virtual void RequestProductDetails(KaiTrade.Interfaces.IProduct myProduct)
        {
            try
            {
                m_Log.Error("RequestProductDetails:product i/f - Driver Base class cannot process");

            }
            catch (Exception myE)
            {
                m_Log.Error("RequestProductDetails", myE);
            }
        }

        public virtual List<KaiTrade.Interfaces.IVenueTradeDestination> GetTradeDestinations(string cfiCode)
        {
            // returns an empty list  if not overrridden
            return new List<KaiTrade.Interfaces.IVenueTradeDestination>();
        }

        void Register(string myTag, KaiTrade.Interfaces.IClient myClient)
        {
            try
            {
                m_Clients.Add(myClient);
            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.Register:client", myE);
            }
        }

        /// <summary>
        /// Process a set of order - this is driver specific and used to provide
        /// some access to specific driver functions - it should be avoided as it will
        /// be depricated
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public virtual int ProcessOrders(string cmdName, string[] orderIds)
        {
            return 0;
        }

        void Send(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                // do the update assync
                lock (((ICollection)m_InboundProcessorQueue).SyncRoot)
                {
                    m_InboundProcessorQueue.Enqueue(myMsg);
                    m_InboundProcessorSyncEvents.NewItemEvent.Set();
                }
                
                
            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.Send", myE);
            }
        }

        public void OnMessage(KaiTrade.Interfaces.IMessage myMessage)
        {
            try
            {
                DoSend(myMessage);
            }
            catch (Exception myE)
            {
                m_Log.Error("OnMessage", myE);
            }
        }

        public void OnStatusMessage(KaiTrade.Interfaces.IMessage myMessage)
        {
            throw new NotImplementedException();
        }

        void SetParent(IDriverManager myParent)
        {
            m_Parent = myParent;

        }

        void Start(string myState)
        {
            try
            {
                m_StartState = myState;
                // set this as a publisher
                //Factory.Instance().GetPublisherManager().Add(this);
               
                //set the  default location of the config path

                m_ConfigPath = Directory.GetCurrentDirectory() + @"\config\";
                try
                {
                    // load the driver state
                    if (myState.Length > 0)
                    {
                        _state = JsonConvert.DeserializeObject<DriverState>(myState);
                        m_ConfigPath = _state.ConfigPath;
                        m_UseAsyncPriceUpdates = _state.AsyncPrices;
                        m_QueueReplaceRequests = _state.QueueReplaceRequests;
                        
                    }
                }
                catch (Exception myE)
                {
                    m_Log.Error("Driver.Start:cannot read state", myE);
                }
                if (this.UseWatchDogStart)
                {
                    /// the watch dog issues the start req
                    StartWD();
                }
                else
                {
                    DoStart(m_StartState);
                }
                

            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.Start", myE);
            }

        }
        protected bool UseWatchDogStart
        {
            get { return m_UseWatchDogStart; }
            set { m_UseWatchDogStart = value; }
        }
            
        /// <summary>
        /// Start the watchdog thread
        /// </summary>
        /// <param name="myState"></param>
        private void StartWD()
        {
            try
            {
                m_RunWD = true;
                m_WDThread = new Thread(new ThreadStart(this.runWDThread));
                m_WDThread.SetApartmentState(ApartmentState.STA);
                m_WDThread.Start();

            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "Issue starting watchdog thread" + myE.Message);
                m_Log.Error("StartWD", myE);

            }
        }


        private void runWDThread()
        {
            try
            {
                while (m_RunWD)
                {
                    
                    if ((m_Status != KaiTrade.Interfaces.Status.open) && (m_Status != KaiTrade.Interfaces.Status.opening))
                    {
                        DoStart(m_StartState);
                    }
                     
                    Thread.Sleep(5000);
                }

            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.error, "WD Thread terminated :" + myE.Message);
                m_Log.Error("runWDThread", myE);

            }
        }

        /// <summary>
        /// Set the APP Facade in the plugin - this lets the plugin do things with KTA
        /// </summary>
        /// <param name="myFacade"></param>
        public void SetFacade(IFacade myFacade)
        {
            m_Facade = myFacade;
        }

        /// <summary>
        /// Get the running status of some driver
        /// compliments the StatusRequest();
        /// </summary>
        public virtual DriverStatus GetRunningStatus()
        {
            // returns none on all states - unless overridden
            return new DriverStatus();
        }

        void StatusRequest()
        {
            try
            {
                SendLastStatusMessage();
            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.StatusRequest", myE);
            }
        }

        void Stop()
        {
            try
            {
                m_Clients.Clear();
                m_RunWD = false;
                if (this.UseWatchDogStart)
                {
                    m_WDThread.Abort();
                }
                
                try
                {
                    DoStop();
                }
                catch (Exception myE)
                {
                    m_DriverLog.Error("Stop:DoStop in base", myE);
                }

                m_PublisherRegister.Clear();
                m_Sessions.Clear();


            }
            catch (Exception myE)
            {
                m_Log.Error("Stop:", myE);
            }
        }

        /// <summary>
        /// If set to true then run on the live market
        /// </summary>
        public bool LiveMarket
        {
            get
            { return m_LiveMarket; }
            set
            {
                m_LiveMarket = value;
                if (m_LiveMarket)
                {
                    m_Log.Info("LiveMarket:True");
                }
                else
                {
                    m_Log.Info("LiveMarket:False");
                }
            }
        }
         
        string Tag
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

        void UnRegister(KaiTrade.Interfaces.IPublisher myPublisher)
        {
            try
            {
                
                DoUnRegister(myPublisher);
            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.UnRegister:publisher", myE);
            }
        }

        void UnRegister(KaiTrade.Interfaces.IClient myClient)
        {
            try
            {
                m_Clients.Remove(myClient);
            }
            catch (Exception myE)
            {
                m_Log.Error("Driver.UnRegister:client", myE);
            }
        }

        #endregion


        #region Driver Members


        public List<IDriverSession> Sessions
        {
            get 
            {
                List<IDriverSession> mySessions = new List<IDriverSession>();
                foreach (IDriverSession mySession in m_Sessions.Values)
                {
                    mySessions.Add(mySession);
                }
                return mySessions;
            }
        }

        public KaiTrade.Interfaces.Status Status
        {
            get { return m_Status; }
        }

        /// <summary>
        /// set status and report change
        /// </summary>
        /// <param name="myStatus"></param>
        public void setStatus(KaiTrade.Interfaces.Status myStatus)
        {
            try
            {
                m_Status = myStatus;
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, Name + "Status changed to:" + m_Status.ToString());
                this.SendAdvisoryMessage(Name + "Status changed to:" + m_Status.ToString());
                m_Log.Info(Name + "Status changed to:" + m_Status.ToString());
                m_WireLog.Info(Name + "Status changed to:" + m_Status.ToString());
            }
            catch (Exception myE)
            {
                m_Log.Error("setStatus", myE);

            }
        }

        /// <summary>
        /// Set the status of all subscriptions
        /// </summary>
        /// <param name="myStatus"></param>
        protected virtual void setSubscriptionsStatus(KaiTrade.Interfaces.Status myStatus)
        {
            try
            {
                m_DriverLog.Info("setSubscriptionsStatus:" + myStatus.ToString());
                foreach (KaiTrade.Interfaces.IPublisher myPub in m_PublisherRegister.Values)
                {
                    myPub.Status = myStatus;
                }

            }
            catch (Exception myE)
            {
                m_Log.Error("setSubscriptionsStatus", myE);
            }
        }


        /// <summary>
        /// Get a set of time series data - if supported
        /// </summary>
        /// <param name="myTSSet"></param>
        public virtual void RequestTSData(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            //DoGetTSData(myTSSet);
        }

        /// <summary>
        /// Get a set of time series data - if supported
        /// </summary>
        /// <param name="myTSSet"></param>
        public virtual void DisconnectTSData(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            //DoGetTSData(ref  myTSSet);
        }

        protected virtual  void StartBarsGenerate(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            try
            {

                // try get an agregator
                IPriceAgregator priceAgregator = Factory.Instance().GetPriceAgregator("K2PriceAggregatorBase");
                if (priceAgregator != null)
                {
                    priceAgregator.TSSet = myTSSet;
                    AddPriceAgregator(priceAgregator);
                }
            }
            catch (Exception myE)
            {

            }
        }


        protected virtual void AddPriceAgregator(IPriceAgregator priceAgregator)
        {
            try
            {

                if (m_PriceAgregators.ContainsKey(priceAgregator.TSSet.Mnemonic))
                {
                    if (m_PriceAgregators[priceAgregator.TSSet.Mnemonic].Contains(priceAgregator))
                    {
                        // do nothing already in the list
                    }
                    else
                    {
                        m_PriceAgregators[priceAgregator.TSSet.Mnemonic].Add(priceAgregator);
                    }
                }
                else
                {
                    List<IPriceAgregator> agList = new List<IPriceAgregator>();
                    agList.Add(priceAgregator);
                    m_PriceAgregators.Add(priceAgregator.TSSet.Mnemonic, agList);
                }
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Add an exchange supported by the venue concerned
        /// </summary>
        /// <param name="myCode"></param>
        /// <param name="myName"></param>
        protected void AddExchange(string myCode, string myName)
        {
        }

        protected void AddContract(string myExchangeID, string myCode, string myName)
        {
        }

        /// <summary>
        /// Add product to the product manager - no events are raised
        /// </summary>
        /// <param name="productdata"></param>
        /// <returns></returns>
        public string AddProductDirect(K2DataObjects.Product  productdata)
        {
            string productId = "";
            try
            {
                productId =  Factory.Instance().GetProductManager().AddProduct(productdata);
            }
            catch (Exception myE)
            {
            }
            return productId;
            
        }

        public KaiTrade.Interfaces.IProduct AddProductDirect(string myCFICode, string myExchangeID, string ExDestination, string mySymbol, string mySecID, string myMMY, string myStrikePx, bool doEvent)
        {

            KaiTrade.Interfaces.IProduct product = null;
            try
            {
                double? strikePx = new double?();
                if (myStrikePx.Length > 0)
                {
                    strikePx = double.Parse(myStrikePx);
                }
                product = AddProductDirect(mySecID, myCFICode, myExchangeID, ExDestination, mySymbol, mySecID, myMMY, myStrikePx, "", strikePx, doEvent);
            }
            catch (Exception myE)
            {
            }
            return product;
        }

        public KaiTrade.Interfaces.IProduct AddProductDirect(string mnemonic, string myCFICode, string myExchangeID, string ExDestination, string mySymbol, string mySecID, string myMMY, string myStrikePx, string myCurrency, double? strikePx, bool doEvent)
        {
            KaiTrade.Interfaces.IProduct product = null;
            try
            {
                product = Factory.Instance().AppFacade.AddProduct(mnemonic, Name, mySecID, myExchangeID, ExDestination, myCFICode, myMMY, myCurrency,strikePx,doEvent);
  
            }
            catch (Exception myE)
            {
                m_Log.Error("AddProductDirect", myE);
            }
            return product;
        }
       

        /// <summary>
        /// Add an account to the KaiTrade Account manager
        /// </summary>
        /// <param name="myAccountCode">Brokers account code (this will be used on orders</param>
        /// <param name="myLongName">Descriptive name</param>
        /// <param name="myFirmCode">Free format test - Code of the broker firm</param>
        protected void AddAccount(string myAccountCode, string myLongName, string myFirmCode)
        {
            K2DataObjects.Account account = new K2DataObjects.Account();
            account.LongName = myLongName;
            // use the descriptive name for the ID since we use that as the lookup on orders
            account.AccountCode = myAccountCode;
            account.VenueCode = m_ID;
            account.FirmCode = myFirmCode;

            // send an account update message to all clients;
            this.sendResponse("AccountUpdate", JsonConvert.SerializeObject(account));
        }

        protected void SetContextCommand(OrderContext myCntx, ORCommand expectedCmd, ORCommand newCmd)
        {
            try
            {
                if (myCntx.CurrentCommand != expectedCmd)
                {
                    m_DriverLog.Error("SetContextCommand:unexpected existing cmd:clordid:" + myCntx.ClOrdID + " existing:" + myCntx.CurrentCommand.ToString() + " expectd:" + expectedCmd.ToString() + " new:" + newCmd.ToString());
                }
                myCntx.CurrentCommand = newCmd;
            }
            catch (Exception myE)
            {
                m_DriverLog.Error("SetContextCommand", myE);
            }
        }

        protected  long removeInactiveContexts()
        {
            long numberActive = 0;
            try
            {
                List<string> removeIds = new List<string>();
                foreach (OrderContext cntx in m_ActiveContextMap.Values)
                {
                    if (!cntx.isActive())
                    {
                        removeIds.Add(cntx.Identity);
                    }

                    
                }
                foreach (string id in removeIds)
                {
                    m_ActiveContextMap.Remove(id);
                }
                numberActive = m_ActiveContextMap.Count;
            }
            catch (Exception myE)
            {
                m_Log.Error("removeInactiveContexts", myE);
            }
            return numberActive;
        }

        protected void identifyPendingContexts(long delay,log4net.ILog log)
        {
            try
            {
                 
                foreach (OrderContext cntx in m_ActiveContextMap.Values)
                {
                    if (cntx.isPending(delay))
                    {
                        log.Info("identifyPendingContexts:pending:" + cntx.ToString());
                    }
                  
                }
            }
            catch (Exception myE)
            {
                log.Error("identifyPendingContexts", myE);
            }
        }

        /// <summary>
        /// record an order context
        /// </summary>
        /// <param name="myClOrdID"></param>
        /// <param name="myFixMsg"></param>
        /// <param name="myApiOrder"></param>
        protected OrderContext RecordOrderContext(string myClOrdID,  object myApiOrder, string myApiID)
        {
            OrderContext myCntx = null;
            try
            {
                myCntx = new OrderContext();
                myCntx.ClOrdID = myClOrdID;
                myCntx.ExternalOrder = myApiOrder;
                
                myCntx.APIOrderID = myApiID;
                if (m_ClOrdIDOrderMap.ContainsKey(myClOrdID))
                {
                    m_ClOrdIDOrderMap[myClOrdID] = myCntx;
                }
                else
                {
                    m_ClOrdIDOrderMap.Add(myClOrdID, myCntx);
                }
                if (myApiID.Length > 0)
                {
                    if (m_ApiIDOrderMap.ContainsKey(myApiID))
                    {
                        m_ApiIDOrderMap[myApiID] = myCntx;
                    }
                    else
                    {
                        m_ApiIDOrderMap.Add(myApiID, myCntx);
                    }
                }

                if (!m_ActiveContextMap.ContainsKey(myCntx.Identity))
                {
                    m_ActiveContextMap.Add(myCntx.Identity, myCntx);
                }

            }
            catch (Exception myE)
            {
                m_Log.Error("RecordOrderContext", myE);
            }
            return myCntx;
        }

        /// <summary>
        /// Record a context against a particular cl order ID
        /// </summary>
        /// <param name="myClOrdID"></param>
        /// <param name="myCntx"></param>
        protected void RecordOrderContext(string myClOrdID, OrderContext myCntx)
        {
            try
            {
                
                if (m_ClOrdIDOrderMap.ContainsKey(myClOrdID))
                {
                    m_ClOrdIDOrderMap[myClOrdID] = myCntx;
                }
                else
                {
                    m_ClOrdIDOrderMap.Add(myClOrdID, myCntx);
                }
                

            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Record a context against a particular cl order ID
        /// </summary>
        /// <param name="myClOrdID"></param>
        /// <param name="myCntx"></param>
        protected void RecordOrderContextApiID(string myApiID, OrderContext myCntx)
        {
            try
            {
                 
                if (m_ApiIDOrderMap.ContainsKey(myApiID))
                {
                    m_ApiIDOrderMap[myApiID] = myCntx;
                }
                else
                {
                    m_ApiIDOrderMap.Add(myApiID, myCntx);
                }


            }
            catch (Exception myE)
            {
            }
        }

        protected OrderContext GetOrderContextClID(string myClOrdID)
        {
            OrderContext myCntx = null;
            if (m_ClOrdIDOrderMap.ContainsKey(myClOrdID))
            {
                myCntx = m_ClOrdIDOrderMap[myClOrdID];
            }
            return myCntx;
            
        }
        protected OrderContext GetOrderContextApiID(string myApiID)
        {
            OrderContext myCntx = null;
            if (m_ApiIDOrderMap.ContainsKey(myApiID))
            {
                myCntx = m_ApiIDOrderMap[myApiID];
            }
            return myCntx;
             
        }

        public void sendExecReport(OrderContext context, string orderID, string status, string execType, double lastQty, int leavesQty, int cumQty, double lastPx, double avePx, string text, string ordRejReason)
        {
            K2DataObjects.Fill fill = new K2DataObjects.Fill();
            fill.OrderID = orderID;
            fill.ClOrdID = context.ClOrdID;
            fill.OrigClOrdID = context.OrigClOrdID;
            fill.OrderStatus = status;
            fill.ExecType = execType;
            fill.FillQty = lastQty;
            fill.LeavesQty = leavesQty;
            fill.CumQty = cumQty;
            fill.LastPx = lastPx;
            fill.AvgPx = avePx;
            fill.Text = text;
            fill.OrdRejReason = ordRejReason;
            sendExecReport(fill);
        }

        public void sendExecReport(string orderID, string status, string execType, double lastQty, int leavesQty,int cumQty, double lastPx, double avePx, string text, string ordRejReason)
        {
            try
            {
                
                K2DataObjects.Fill fill = new K2DataObjects.Fill();
                fill.OrderID = orderID;
                fill.OrderStatus = status;
                fill.ExecType = execType;
                fill.FillQty = lastQty;
                fill.LeavesQty = leavesQty;
                fill.CumQty = cumQty;
                fill.LastPx = lastPx;
                fill.AvgPx = avePx;
                fill.Text = text;
                fill.OrdRejReason = ordRejReason;
            }
            catch (Exception myE)
            {
                m_Log.Error("sendExecReport", myE);
            }
        }

         
        public void sendExecReport(OrderContext myCntx, string orderID, string status, string execType, double lastQty, int leavesQty, int cumQty, double lastPx, double avePx)
        {
            myCntx.OrdStatus = status;
            myCntx.ExecType = execType;
            myCntx.LastAveragePrice = avePx;
            K2DataObjects.Fill fill = new K2DataObjects.Fill();
            fill.OrderID = orderID;
            fill.OrderStatus = status;
            fill.ExecType = execType;
            fill.FillQty = lastQty;
            fill.LeavesQty = leavesQty;
            fill.CumQty = cumQty;
            fill.LastPx = lastPx;
            fill.AvgPx = avePx;

            sendExecReport(fill);

        }

        public void sendExecReport(KaiTrade.Interfaces.ISubmitRequest nos, string status, string execType, string text, string ordRejReason)
        {

            K2DataObjects.Fill fill = new K2DataObjects.Fill();
            fill.OrderID = "";
            fill.OrderStatus = status;
            fill.ExecType = execType;
            fill.FillQty = 0;
            fill.LeavesQty = 0;
            fill.CumQty = 0;
            fill.LastPx = 0.0;
            fill.AvgPx = 0.0;
            fill.Text = text;
            fill.OrdRejReason = ordRejReason;

            sendExecReport(fill);

        }

        

        public void sendExecReport(KaiTrade.Interfaces.IFill fill)
        {
            try
            {

                string execReportixMsg =  JsonConvert.SerializeObject(fill);

                // send our response message back to the clients of the adapter
                sendResponse("8", execReportixMsg);
            }
            catch (Exception myE)
            {
                m_Log.Error("sendExecReport", myE);
            }
        }



       
       
        public void sendCancelRej(KaiTrade.Interfaces.ICancelOrderRequest myReq, KaiTrade.Interfaces.CxlRejReason myReason, string myReasonText)
        {
            try
            {
                string myFixMsg = RequestBuilder.DoRejectCxlReq(myReq, "UNKNOWN",myReason, myReasonText);
                // send our response message back to the clients of the adapter
                sendResponse("9", myFixMsg);
            }
            catch (Exception myE)
            {
                m_Log.Error("sendCancelRej", myE);
            }
        }

        public void sendCancelReplaceRej(KaiTrade.Interfaces.IModifyOrderRequst myReq,  KaiTrade.Interfaces.CxlRejReason myReason, string myReasonText)
        {
            try
            {
                string myFixMsg = RequestBuilder.DoRejectModReq(myReq, "UNKNOWN", myReason, myReasonText);
                // send our response message back to the clients of the adapter
                sendResponse("G", myFixMsg);

            }
            catch (Exception myE)
            {
                m_Log.Error("sendCancelReplaceRej", myE);
            }
        }


      

        #endregion

       
    }
}
