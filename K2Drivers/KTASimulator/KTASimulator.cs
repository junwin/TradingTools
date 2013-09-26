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
using System.Threading;
using System.Timers;
using System.IO;
using Newtonsoft.Json;
using K2ServiceInterface;

namespace KTASimulator
{
    

    public class KTASimulator : DriverBase.DriverBase
    {

        private delegate void SendResponseDelegate(string myType, string myMsg);
        SendResponseDelegate m_SRDelegate;

        private Object m_PriceUpdateLock = new Object();
        private Object m_GetTSReqToken = new object();
       
        /// <summary>
        /// Map to the products that we can simulate indexes by Mnemonic
        /// </summary>
        private Dictionary<string, SimulatorProduct> _products;

        /// <summary>
        /// Map of order contectxs to Order ID
        /// </summary>
        private Dictionary<string, DriverBase.OrderContext> m_OrderContextOrdID;

        /// <summary>
        /// Map of order contectxs to Order ClOrdID
        /// </summary>
        //private Dictionary<string, DriverBase.OrderContext> m_OrderContextClOrdID;

        
        /// <summary>
        /// Simulator config
        /// </summary>
        private SimulatorConfig _config;

        /// <summary>
        /// thread to generate some simple prices
        /// </summary>
        private Thread m_PriceThread;

        /// <summary>
        /// do the price gen loop true=> run price updates
        /// </summary>
        private bool m_GenPrices = false;

        private Dictionary<string, FilePriceSource> _filePrices;

        private Random m_RNGen;

        /// <summary>
        /// Timer for those algos that require some time based processing
        /// </summary>
        private System.Timers.Timer m_Timer;

        /// <summary>
        /// Timer interval used for the timer
        /// </summary>
        private long m_TimerInterval = 5000;

        /// <summary>
        /// List of orders to fill over time
        /// </summary>
        List<DriverBase.OrderContext> m_FillList;

        private Dictionary<string, Market> m_Markets;

        /// <summary>
        /// Is the zSELL order triggered?
        /// </summary>
        private bool m_ZSELLTrigger = false;
        /// <summary>
        /// Is the zSELL order triggered?
        /// </summary>
        private bool m_ZBUYTrigger = false;

        private SimulatorMainUI m_MainForm = null;

     

        public KTASimulator()
        {
            
            Name = "KTSIM";
            m_ID = "KTSIM";
            m_Tag = "";

            _log.Info(Name + " Created");

            m_SRDelegate = new SendResponseDelegate(this.handleResponse);

            m_OrderContextOrdID = new Dictionary<string, DriverBase.OrderContext>();
            //m_OrderContextClOrdID = new Dictionary<string, DriverBase.OrderContext>();
            m_Markets = new Dictionary<string, Market>();
            m_FillList = new List<DriverBase.OrderContext>();
            genSampleConfig();

            _products = new Dictionary<string, SimulatorProduct>();

            m_RNGen = new Random();

            _filePrices = new Dictionary<string, FilePriceSource>();

            m_MainForm = new SimulatorMainUI();

            m_MainForm.Simulator = this;

            


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
                    m_MainForm.Show();
                }
                else
                {
                    m_MainForm.Hide();
                }
            }
            catch (Exception myE)
            {
            }
        }

        public log4net.ILog Log
        {
            get { return _log; }
        }

        public void StartTimer()
        {
            if (m_TimerInterval > 0)
            {
                m_Timer = new System.Timers.Timer(m_TimerInterval);
                m_Timer.Elapsed += new ElapsedEventHandler(OnTimer);
                m_Timer.Interval = (double)m_TimerInterval;
                m_Timer.Enabled = true;
            }
        }

        public void StopTimer()
        {
            if (m_Timer != null)
            {
                m_Timer.Enabled = false;
            }
            m_Timer = null;
        }

        private void OnTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                doFillList();  
            }
            catch (Exception myE)
            {
                _log.Error("OnTimer", myE);
            }
        }


        private void genSampleConfig()
        {
            SimulatorConfig config = new SimulatorConfig();
            SimulatorProduct mySimProduct = new SimulatorProduct();
            CannedData cannedData = new CannedData();
            K2DataObjects.Product myProduct = new K2DataObjects.Product();
            

            myProduct.Symbol = "EPZ8";
            myProduct.CFICode = "FXXXXX";
            myProduct.MMY = "20081219";
            myProduct.Exchange = "CME";
            myProduct.TradeVenue = "CQG";
            myProduct.Mnemonic = "EMINI";
            myProduct.LongName = "CME eMini SP500";
            myProduct.SecurityID = "ZPZ8";

            //mySimProduct.InstrDef = myProduct;
            mySimProduct.HighPrice = 99.99M;
            mySimProduct.LowPrice = 98.99M;
            mySimProduct.IsAutoFill = true;
            mySimProduct.IsCannedData = true;
            mySimProduct.RunAsMarket = true;
            mySimProduct.Mnemonic = "EPZ8";

            cannedData.CannedDataFile = "filepath";
            cannedData.RepeatOnEnd = true;
            cannedData.RunInterval = 100;
            cannedData.RunRealTime = true;
            cannedData.PlayOnSubscribe = true;
            
            mySimProduct.CannedData = cannedData;
             
            config.SimulatorProduct.Add(mySimProduct);

            string configData = JsonConvert.SerializeObject(config);
            

            //m_Config.
           // _config.ToXmlFile("KTSimConfigTEMP.xml");
        }

        /// <summary>
        /// Override the base class DoStart
        /// </summary>
        protected override void DoStart(string myState)
        {
            try
            {
                _log = log4net.LogManager.GetLogger("Kaitrade");

                _log.Info("KTA Simulator Driver Started");

                // try load our config file
                processConfigFile(_configPath + "SimulatorConfig.txt");
 
                m_GenPrices = true;
                m_PriceThread = new Thread(new ThreadStart(this.generatePrices));
                m_PriceThread.SetApartmentState(ApartmentState.MTA);
                m_PriceThread.Start();
                 
                // do the file thing in SubscribeMD
                //this.AddPriceFile("KTSIM:ZZ:DELL:ESXXXX", "testdata.csv");

                // Report that we are open - this will show in the Driver view
                // in the dashboard
                SendStatusMessage(KaiTrade.Interfaces.Status.open, "KT Simulator Driver is open");

                AddAccount("JSMITH", "Simulator JSMITH", "KAI");
                AddAccount("MGiles", "Simulator MGiles", "KAI");
                AddAccount("MWTrader", "Simulator MWTrader", "KAIB");
                AddAccount("AKlien", "Simulator AKlien", "KAIB");
                AddAccount("ACMEFund", "Simulator ACMEFund", "KAIB");
                AddAccount("NewTrade", "Simulator NewTrade", "KAIB");
                AddAccount("TestBadAccount", "Account will be rejected", "KAIB");

                /// Get any price files in PriceData and use them for markets and price generation
                try
                {
                    GetPriceFiles();
                }
                catch (Exception myE)
                {
                }
                
                setStatus(KaiTrade.Interfaces.Status.open);
                StartTimer();
                _runningState = new DriverBase.DriverStatus(StatusConditon.good, StatusConditon.good);
               
                    if (!_state.HideDriverUI)
                    {
                        m_MainForm.Show();
                    }
                
            }
            catch (Exception myE)
            {
                _log.Error("doStart", myE);
            }


        }

        /// <summary>
        /// Get the running status of some driver
        /// compliments the StatusRequest();
        /// </summary>
        public override IDriverStatus GetRunningStatus()
        {
            // returns none on all states - unless overridden
            return _runningState;
            
        }

        /// <summary>
        /// Process the config file for the simulator
        /// </summary>
        /// <param name="myPath"></param>
        private void processConfigFile(string myPath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(myPath))
                {
                    _config = JsonConvert.DeserializeObject<SimulatorConfig>(sr.ReadToEnd());
                }

                if (_config.ProductFilePath.Length > 0)
                {
                    AddProductDirect(_config.ProductFilePath);
                }

                // process the config file products these are the things we will simulate
                foreach (SimulatorProduct myProd in _config.SimulatorProduct)
                {
                    try
                    {

                        _products.Add(myProd.Mnemonic, myProd);
                        if (myProd.RunAsMarket)
                        {
                            CreateMarket(myProd.Mnemonic);
                        }

                        if (myProd.CannedData != null && myProd.IsCannedData)
                        {
                            this.AddPriceFile(myProd.Mnemonic, myProd.CannedData.CannedDataFile, myProd.CannedData.RunInterval, myProd.CannedData.RunRealTime, myProd.CannedData.RepeatOnEnd, myProd.CannedData.PlayOnSubscribe);
                        }
                        else
                        {
                            _products.Add(myProd.Mnemonic, myProd);
                        }

                    }
                    catch (Exception myE)
                    {
                        _log.Error("processConfigFile:loop", myE);
                    }
                }

            }
            catch (Exception myE)
            {
                _log.Error("processConfigFile", myE);
            }
        }

        private SimulatorProduct getProduct(string myMnemonic)
        {

            SimulatorProduct myProd = null;
            if (_products.ContainsKey(myMnemonic))
            {
                myProd = _products[myMnemonic];
            }
            return myProd;
            
        }

       

        protected override void SubscribeMD(KaiTrade.Interfaces.IProduct product, int depthLevels, string requestID)
        {
            try
            {
                string topic = product.Mnemonic;
                topic = topic.Substring(2, topic.Length - 2);
               
                if(_filePrices.ContainsKey(product.Mnemonic))
                {
                    if (_filePrices[product.Mnemonic].PlayOnSubscribe)
                    {
                        // note file source may not support depth
                        _filePrices[product.Mnemonic].DepthLevels = depthLevels;

                        _filePrices[product.Mnemonic].Start(_filePrices[product.Mnemonic].FilePath);
                    }
                }
                else if (_filePrices.ContainsKey(topic))
                {
                    // note file source may not support depth
                    _filePrices[topic].DepthLevels = depthLevels;

                    if (_filePrices[topic].PlayOnSubscribe)
                    {
                        _filePrices[topic].Start(_filePrices[topic].FilePath);
                    }
                }
                


            }
            catch (Exception myE)
            {
                _log.Error("SubscribeMD", myE);
            }
        }

        protected override void UnSubscribeMD(KaiTrade.Interfaces.IPublisher pub)
        {
            // no action
        }

        
        

        private void loadTSSet(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            try
            {
                if (myTSSet.UpdatesEnabled)
                {
                    StartBarsGenerate(myTSSet);
                }
                else
                {
                    _log.Error("Bar data not supported");
                    /*
                    string fileName = myTSSet.Mnemonic + "_tsset.xml";
                    KAI.kaitns.TSDataSet myDB = new KAI.kaitns.TSDataSet();
                    //fileName.Replace(':','');
                    myDB.FromXmlFile(fileName);
                    myTSSet.From(myDB);
                    myTSSet.Added = true;
                     */
                }
            }
            catch(Exception myE)
            {
            }
        }
        public override void RequestTSData(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            lock (m_GetTSReqToken)
            {
               
                try
                {
                    switch (myTSSet.TSType)
                    {
                        case KaiTrade.Interfaces.TSType.BarData:
                            loadTSSet(myTSSet); 
                            break;
                        case KaiTrade.Interfaces.TSType.ConstantBars:
                             
                            break;
                        case KaiTrade.Interfaces.TSType.Condition:
                             
                            break;
                        case KaiTrade.Interfaces.TSType.StudyData:
                             
                            break;
                        case KaiTrade.Interfaces.TSType.Expression:
                             
                            break;
                        case KaiTrade.Interfaces.TSType.TradeSystem:
                             
                            break;

                        default:
                            _driverLog.Error("Unknown TS Request type:" + myTSSet.TSType.ToString());
                            break;
                    }


                }
                catch (Exception myE)
                {
                    _log.Error("RequestTSData", myE);
                    this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "DoGetTSData" + myE.Message);

                }
            }
        }

       
        public void CreateMarket(string mnemonic)
        {
            try
            {
                Market market = new Market(mnemonic, this);
                m_Markets.Add(mnemonic, market);
            }
            catch (Exception myE)
            {
                _log.Error("CreateMarket", myE);
            }
        }

        public void GetPriceFiles()
        {
            try
            {
                string basePath = Facade.AppPath + @"\PriceData";
                string[] files = Directory.GetFiles(basePath, "*.csv");
                foreach (string file in files)
                {
                    int endMnemonicPos = file.IndexOf("_data.csv");
                    if (endMnemonicPos > 0)
                    {
                        int startMnemonic = file.IndexOf("PriceData");
                        string mnemonic = file.Substring(startMnemonic+10);
                        endMnemonicPos = mnemonic.IndexOf("_data.csv");
                        mnemonic = mnemonic.Substring(0, endMnemonicPos);

                        AddPriceFile(mnemonic, file, 1, true, true, true);
                        CreateMarket(mnemonic);
                    }
                }
            }
            catch (Exception myE)
            {
                _log.Error("GetPriceFiles", myE);
            }
        }

        public void AddPriceFile(string mnemonic, string filePath, int interval, bool runRealTime, bool repeatOnEnd, bool playOnSubscribe)
        {
            try
            {
                FilePriceSource pxSrc = new FilePriceSource(this);
                _filePrices.Add(mnemonic, pxSrc);
                pxSrc.PriceUpdate += this.PriceUpdate;
                pxSrc.PriceUpdateStatus += this.PriceUpdateStatus;
                pxSrc.Interval = interval;
                pxSrc.RunRealTime = runRealTime;
                pxSrc.RepeatOnEnd = repeatOnEnd;
                pxSrc.FilePath = filePath;
                if (!playOnSubscribe)
                {
                    pxSrc.Start(filePath);
                }
                else
                {
                    pxSrc.SetUpProductNoRun(filePath);
                }
            }
            catch (Exception myE)
            {
                _log.Error("AddPriceFile", myE);
            }
        }

        public void RunPxRealTime()
        {
            try
            {
                foreach (FilePriceSource pxSrc in _filePrices.Values)
                {

                    pxSrc.RunRealTime = true;
                }
            }
            catch (Exception myE)
            {
                _log.Error("RunPxRealTime", myE);
            }
        }

        public void RunPxInterval(int interval)
        {
            try
            {
                foreach (FilePriceSource pxSrc in _filePrices.Values)
                {
                    pxSrc.Interval = interval;
                    pxSrc.RunRealTime = false;
                }
            }
            catch (Exception myE)
            {
                _log.Error("RunPxRealTime", myE);
            }
        }



        public void PriceUpdateStatus(FilePriceSource source, SrcStatus status)
        {
            try
            {
                if (status == SrcStatus.ended)
                {
                    if (source.RepeatOnEnd)
                    {
                        // just reopen
                        source.Stop();
                        source.Start(source.FilePath);
                    }
                    else
                    {
                        // we are done
                    }
                }
            }
            catch (Exception myE)
            {
                _log.Error("PriceUpdateStatus", myE);
            }
        }

        public void PriceUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            lock (m_PriceUpdateLock)
            {
                try
                {
                    if (m_Markets.ContainsKey(pxUpdate.Mnemonic))
                    {
                        m_Markets[pxUpdate.Mnemonic].ApplyPxUpdate(pxUpdate);
                    }
                    ApplyPriceUpdate(pxUpdate);
                    if (pxUpdate.Mnemonic == "KTSIM:ZZ:DELL:ESXXXX")
                    {
                        //ApplyDOMUpdate(pxUpdate.Mnemonic, pxUpdate);
                    }
                }
                catch (Exception myE)
                {
                    _log.Error("PriceUpdate", myE);
                }
            }
        }

        private void generatePrices()
        {
            while (m_GenPrices)
            {
                SimulatorProduct myProd;
                foreach (string myMnemonic in _products.Keys)
                {
                    myProd = _products[myMnemonic];
                    generateProductPrices( myProd);
                    Thread.Sleep(50);
                }
                Thread.Sleep(100);

            }
        }

        private void generateProductPrices(SimulatorProduct product)
        {
            try
            {
                K2DataObjects.PXUpdateBase pxupdate = new K2DataObjects.PXUpdateBase(m_ID);
                pxupdate.Mnemonic = product.Mnemonic;

                pxupdate.BidSize = m_RNGen.Next(100);
                pxupdate.OfferSize = m_RNGen.Next(100);
                pxupdate.BidPrice = product.LowPrice;
                pxupdate.OfferPrice = pxupdate.BidPrice + 1;
                pxupdate.TradePrice = ((pxupdate.BidPrice + pxupdate.OfferPrice) / 2);
                pxupdate.TradeVolume = (pxupdate.BidSize + pxupdate.OfferSize) / 2;
                pxupdate.DayHigh = product.HighPrice;
                pxupdate.DayLow = product.LowPrice;
                if (product.LowPrice != product.HighPrice)
                {
                    pxupdate.BidPrice = (int)product.LowPrice + m_RNGen.Next(10);
                    pxupdate.OfferPrice = pxupdate.BidPrice + 1;
                    pxupdate.TradePrice = ((pxupdate.BidPrice + pxupdate.OfferPrice) / 2);
                }

                pxupdate.Ticks = DateTime.Now.Ticks;

                PriceUpdate(pxupdate);


            }
            catch (Exception myE)
            {

            }
        }

        private double getTradePx(string myMnemonic)
        {
            double myPx = 0.0;
            try
            {
                // try to get a PXPublisher

                L1PriceSupport.PXPublisher myPXPublisher = _publisherRegister[myMnemonic] as L1PriceSupport.PXPublisher;
                //KaiTrade.Interfaces.IPublisher myPublisher = _publisherRegister[myMnemonic] as KaiTrade.Interfaces.IPublisher;

                if (myPXPublisher != null)
                {
                    myPx = double.Parse( myPXPublisher.GetField(KaiTrade.Interfaces.MDConstants.TRADE_PRICE));
                    

                }
            }
            catch (Exception myE)
            {
            }
            return myPx;
        }


        /// <summary>
        /// FIX loop back order 
        /// </summary>
        /// <param name="msg"></param>
        public void submitOrder(KaiTrade.Interfaces.IMessage myMsg)
        {
            KaiTrade.Interfaces.ISubmitRequest nos = null;
            try
            {
                nos = JsonConvert.DeserializeObject<K2DataObjects.SubmitRequest>(myMsg.Data);
                
                _log.Error("SUBTEST:" + myMsg.Data);
                
                long quantity = nos.OrderQty;
                decimal myOrdPrice = nos.Price.Value;
                


                // Get the we want to order
                string myMnemonic = nos.Mnemonic;

                if (nos.SecurityID != null)
                {
                    if (nos.SecurityID.Length > 0)
                    {
                        // is this new market processing?
                        if (m_Markets.ContainsKey(nos.SecurityID))
                        {
                            m_Markets[nos.SecurityID].submitOrder(myMsg);
                            return;
                        }
                    }
                }

                SimulatorProduct myProd = getProduct( myMnemonic);

                if (myProd == null)
                {
                    // we dont simulate this so reject it
                    string myError = "Product not available";
                    Exception myE = new Exception(myError);
                    throw myE;
                }

                // simulate rejecting certail accounts
                if (nos.Account.Length>0)
                {
                    if (nos.Account == "TestBadAccount")
                    {
                        // we dont simulate this so reject it
                        string myError = "Account not valid:" + nos.Account;
                        Exception myE = new Exception(myError);
                        throw myE;
                    }
                }

                // put this into the internal book
                // send order in book exec report
                KaiTrade.Interfaces.IFill fill = new K2DataObjects.Fill();
                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.NEW;
                fill.ExecType = KaiTrade.Interfaces.ExecType.ORDER_STATUS;
                fill.OrderID = DriverBase.Identities.Instance.getNextOrderID();
                //sendExecReport(string orderID, string status, string execType, double lastQty, int leavesQty,int cumQty, double lastPx, double avePx, string text, string ordRejReason)


                if (myProd.IsAutoFill)
                {
                    DriverBase.OrderContext myContext = new DriverBase.OrderContext();
                    //myContext.QFOrder = myOrder;
                    myContext.ClOrdID = nos.ClOrdID;
                    myContext.OrderID = fill.OrderID;
                    myContext.OrderQty =(int) quantity;
                    //myContext.LeavesQty = quantity;
                    myContext.CumQty = 0;
                    // record the order in the context maps
                    RecordOrderContext(myContext.ClOrdID, myContext);
                    //m_OrderContextClOrdID.Add(myContext.ClOrdID, myContext);
                    m_OrderContextOrdID.Add(myContext.OrderID, myContext);

                    sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, (int)quantity, 0, (double)myOrdPrice, (double)myOrdPrice, "", "");
                    if ((nos.OrdType == KaiTrade.Interfaces.OrderType.STOP) || (nos.OrdType == KaiTrade.Interfaces.OrderType.STOPLIMIT))
                    {
                        // Just do the default
                    }
                    else
                    {

                        // is this for auto fill over time?
                        if (nos.Mnemonic == "HPQ")
                        {
                            myContext.FillPattern = DriverBase.fillPattern.gradual;
                            myContext.UpdatePeriod = 1;
                            m_FillList.Add(myContext);
                        }
                        // is this for auto fill over time?
                        if (nos.Mnemonic == "VOD")
                        {
                            myContext.FillPattern = DriverBase.fillPattern.fixedAmount;
                            int fillAmount =(int) ( (double)quantity * 0.5);
                            if (fillAmount > 0)
                            {
                                myContext.TargetFillAmount = fillAmount;
                            }
                            else
                            {
                                myContext.TargetFillAmount = 1;
                            }
                            
                            myContext.UpdatePeriod = 12;
                            myContext.TimerCount = 1;
                            m_FillList.Add(myContext);
                        }
                    }
                }
                else
                {
                    // put this into the internal book
                    // send order in book exec report
                    if (myOrdPrice == (decimal)0.0)
                    {
                        myOrdPrice = (decimal) this.getTradePx(myMnemonic);
                    }
                    

                    // do nothing for now just let is stay working
                    DriverBase.OrderContext myContext = new DriverBase.OrderContext();
                    //myContext.QFOrder = myOrder;
                    myContext.ClOrdID = nos.ClOrdID;
                    myContext.OrderID = fill.OrderID;
                    myContext.OrderQty = (int)nos.OrderQty;
                    //myContext.LeavesQty = quantity;
                    myContext.CumQty = 0;

                    // record the order in the context maps
                    RecordOrderContext(myContext.ClOrdID, myContext);
                    //m_OrderContextClOrdID.Add(myContext.ClOrdID, myContext);
                    m_OrderContextOrdID.Add(myContext.OrderID, myContext);

                    sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, (int)nos.OrderQty, 0, 0.0, 0.0,"","");

                   
                    // do some simple STOP processing
                    if ((nos.OrdType == KaiTrade.Interfaces.OrderType.STOP) || (nos.OrdType == KaiTrade.Interfaces.OrderType.STOPLIMIT))
                    {
                        
                    }
                    else
                    {

                        // is this for auto fill over time?
                        if (nos.Mnemonic == "MRK")
                        {
                            // partial fill
                            int lastQty = 1;

                            int leavesQty = 0;
                            if (quantity > 1)
                            {
                                lastQty = (int)(quantity / 2);

                                leavesQty = (int)quantity - lastQty;
                                quantity = (int)(quantity / 2);
                            }

                            fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.PARTIALLY_FILLED;
                            fill.ExecType = KaiTrade.Interfaces.ExecType.PARTIAL_FILL;
                            sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, (double)lastQty, leavesQty, lastQty, (double)myOrdPrice, (double)myOrdPrice,"", "");


                            myContext.OrderQty = (int)nos.OrderQty;
                           // myContext.LeavesQty = quantity;
                            myContext.CumQty = 0;

                            // record the order in the context maps
                            RecordOrderContext(myContext.ClOrdID, myContext);
                            //m_OrderContextClOrdID.Add(myContext.ClOrdID, myContext);
                            m_OrderContextOrdID.Add(myContext.OrderID, myContext);

                        }
                        else if (nos.Mnemonic == "ZSELL")
                        {
                            int lastQty = 1;

                            int leavesQty = 0;
                            if (!m_ZSELLTrigger)
                            {
                                m_ZSELLTrigger = true;
                                // partial fill

                                if (quantity > 1)
                                {
                                    lastQty = (int)(quantity / 2);

                                    leavesQty = (int)quantity - lastQty;
                                    quantity = (int)(quantity / 2);
                                }

                                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.PARTIALLY_FILLED;
                                fill.ExecType = KaiTrade.Interfaces.ExecType.PARTIAL_FILL;
                                
                            }
                            else
                            {
                                m_ZSELLTrigger = false;
                                lastQty = (int)(quantity);

                                leavesQty = 0;
                                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.PARTIALLY_FILLED;
                                fill.ExecType = KaiTrade.Interfaces.ExecType.PARTIAL_FILL;
                                
                            }
                            sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, (double)lastQty, leavesQty, lastQty, (double)myOrdPrice, (double)myOrdPrice, "", "");

                            
                            myContext.OrderQty = (int)quantity;
                            //myContext.LeavesQty = quantity;
                            myContext.CumQty = 0;

                            // record the order in the context maps
                            RecordOrderContext(myContext.ClOrdID, myContext);
                            //m_OrderContextClOrdID.Add(myContext.ClOrdID, myContext);
                            m_OrderContextOrdID.Add(myContext.OrderID, myContext);
                        }
                        else if ((nos.Mnemonic == "ZBUY") && (nos.Side == KaiTrade.Interfaces.Side.SELL))
                        {
                            int lastQty = 1;

                            int leavesQty = 0;
                            if (!m_ZBUYTrigger)
                            {
                                m_ZBUYTrigger = true;
                                // partial fill

                                if (quantity > 1)
                                {
                                    lastQty = (int)(quantity / 2);

                                    leavesQty = (int)quantity - lastQty;
                                    quantity = (int)(quantity / 2);
                                }

                                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.PARTIALLY_FILLED;
                                fill.ExecType = KaiTrade.Interfaces.ExecType.PARTIAL_FILL;
                            }
                            else
                            {
                                m_ZBUYTrigger = false;
                                lastQty = (int)(quantity);

                                leavesQty = 0;
                                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.FILLED;
                                fill.ExecType = KaiTrade.Interfaces.ExecType.FILL;

                            }

                            sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, (double)lastQty, leavesQty, lastQty, (double)myOrdPrice, (double)myOrdPrice, "", "");

                            
                            myContext.OrderQty = (int) quantity;
                            //myContext.LeavesQty = quantity;
                            myContext.CumQty = 0;

                            // record the order in the context maps
                            RecordOrderContext(myContext.ClOrdID, myContext);
                            //m_OrderContextClOrdID.Add(myContext.ClOrdID, myContext);
                            m_OrderContextOrdID.Add(myContext.OrderID, myContext);
                        }
                        else
                        {
                            // fully fill
                             fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.FILLED;
                             fill.ExecType = KaiTrade.Interfaces.ExecType.FILL;
                            sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, (double)quantity, 0, (int)quantity, (double)myOrdPrice, (double)myOrdPrice, "", "");

                        }
                    }
                    
                }
    
                

            }
            catch (Exception myE)
            {
                _log.Error("submitOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                SendAdvisoryMessage("KTA Simulator:submitOrder: problem submitting order:" + myE.ToString());

                if (nos != null)
                {
                    sendExecReport(nos, KaiTrade.Interfaces.OrderStatus.REJECTED, KaiTrade.Interfaces.ExecType.REJECTED, myE.ToString(), "OTHER");
                    
                }

            }
        }

        public void gradualFill(DriverBase.OrderContext myContext)
        {
            try
            {
                int qtyToFill = 0;
                if (myContext.LeavesQty > 2)
                {
                    qtyToFill = myContext.LeavesQty / 2;
                }
                else
                {
                    qtyToFill = myContext.LeavesQty;
                }
                fixedAmountFill(myContext, qtyToFill,null);
            }
            catch (Exception myE)
            {
                _log.Error("gradualFill", myE);
            }
        }

        public void fixedAmountFill(DriverBase.OrderContext myContext, int qtyToFill, decimal? fillPx)
        {
            try
            {
                // send order in book exec report
                // fully fill
                KaiTrade.Interfaces.IFill fill = new K2DataObjects.Fill();
                fill.ClOrdID = myContext.ClOrdID;

                fill.OrderID = myContext.OrderID;
                if (fillPx.HasValue)
                {
                    fill.LastPx = (double)fillPx.Value;
                }
                else
                {
                    fill.LastPx = (double)myContext.Price;
                }
                fill.AvgPx = 0;

                if (myContext.LeavesQty > 0)
                {
                   

                    if (myContext.LeavesQty > qtyToFill)
                    {
                        fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.PARTIALLY_FILLED;
                        fill.ExecType = KaiTrade.Interfaces.ExecType.PARTIAL_FILL;
                        fill.FillQty = qtyToFill;
                        myContext.CumQty += qtyToFill;
                        fill.CumQty = myContext.CumQty;
                        
                    }
                    else
                    {
                        fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.FILLED;
                        fill.ExecType = KaiTrade.Interfaces.ExecType.FILL;

                        double myQty = myContext.LeavesQty;
                        myContext.CumQty += (int)myQty;
                        fill.FillQty = myContext.LeavesQty;
                        fill.CumQty = myContext.CumQty;
                        
                    }
                    fill.LeavesQty = myContext.LeavesQty;
                    sendExecReport(fill);
                }
            }
            catch (Exception myE)
            {
                _log.Error("gradualFill", myE);
            }
        }
        private void doFillList()
        {
            try
            {
                foreach (DriverBase.OrderContext myContext in m_FillList)
                {
                    if ((myContext.TimerCount % myContext.UpdatePeriod) == 0)
                    {
                        switch(myContext.FillPattern)
                        {
                            case DriverBase.fillPattern.gradual:
                                gradualFill(myContext);
                                break;
                            case DriverBase.fillPattern.fixedAmount:
                                fixedAmountFill(myContext, myContext.TargetFillAmount,null);
                                break;
                            default:
                                break;
                        }
                       
                    }
                    myContext.TimerCount++;
                }
            }
            catch (Exception myE)
            {
                _log.Warn("doFillList", myE);
            }
        }


        /// <summary>
        /// Example code of pulling an order - used for testing 
        /// </summary>
        /// <param name="msg"></param>
        public void pullOrder(KaiTrade.Interfaces.IMessage msg)
        {
            KaiTrade.Interfaces.ICancelOrderRequest cancel = null;
            try
            {
                cancel = JsonConvert.DeserializeObject<K2DataObjects.CancelOrderRequest>(msg.Data);

                KaiTrade.Interfaces.IFill fill = new K2DataObjects.Fill();
                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.CANCELED;
                fill.ExecType = KaiTrade.Interfaces.ExecType.ORDER_STATUS;
                fill.OrderID = DriverBase.Identities.Instance.getNextOrderID();


                if (cancel.ClOrdID.Length == 0)
                {
                    sendCancelRej(cancel, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a clordid must be specified on a modify order");
                    Exception myE = new Exception("a clordid must be specified on a modify order");
                    throw myE;
                }

                if (cancel.OrigClOrdID.Length == 0)
                {
                    sendCancelRej(cancel, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a original clordid must be specified on a modify order");
                    Exception myE = new Exception("an original clordid must be specified on a modify order");
                    throw myE;
                }

                
                string strOrder = msg.Data;

                if (m_Markets.ContainsKey(cancel.Mnemonic))
                {
                    m_Markets[cancel.Mnemonic].pullOrder(msg);
                    return;
                }
 

                // Get the context - we must have this to access the CQG order
                DriverBase.OrderContext myContext = null;
                if (_clOrdIDOrderMap.ContainsKey(cancel.OrigClOrdID))
                {
                    myContext = _clOrdIDOrderMap[cancel.OrigClOrdID];
                }
                if (myContext == null)
                {
                    sendCancelRej(cancel, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "an order does not exist for the cancel requested");
                    Exception myE = new Exception("an order does not exist for the cancel requested");
                    throw myE;
                }

                double myLastFillPrice = 0.0;
                double myAveFillPrice = 0.0;
                double myLastFillQty = 0.0;

                myContext.ClOrdID = cancel.ClOrdID;
                myContext.OrigClOrdID = cancel.OrigClOrdID;

                // send order cancelled exec report
                sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, (int)myContext.LeavesQty, myContext.CumQty, 0.0, 0.0, "", "");

                myContext.OrdStatus = fill.OrderStatus;

                throw new Exception("Delete the order from the simulator");

            }
            catch (Exception myE)
            {

                Log.Error("pullOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                SendAdvisoryMessage("pullOrder: problem pulling order:" + myE.ToString());

            }
        }

        /// <summary>
        /// Example code of pulling an order - used for testing 
        /// </summary>
        /// <param name="msg"></param>
        public void pullOrder(DriverBase.CancelRequestData cancelData)
        {
            /*
            QuickFix.Message myQFPullOrder = null;
            try
            {
                           
                 // is this new market processing?
                QuickFix.SecurityID securityID = new QuickFix.SecurityID();
                if (myQFPullOrder.isSetField(securityID))
                {
                    myQFPullOrder.getField(securityID);
                    // is this new market processing?
                    if (m_Markets.ContainsKey(securityID.ToString()))
                    {
                        m_Markets[securityID.ToString()].pullOrder(msg);
                        return;
                    }
                }
               

                // send order canceled exec report
                QuickFix.OrdStatus myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.CANCELED);
                QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.ORDER_STATUS);


                sendExecReport(cancelData.OrderContext.QFOrder, new QuickFix.OrderID(cancelData.OrderContext.OrderID), myOrdStatus, myExecType,
                                 0, cancelData.OrderContext.LeavesQty, cancelData.OrderContext.CumQty, 0, 0);




            }
            catch (Exception myE)
            {

                _log.Error("pullOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                SendAdvisoryMessage("pullOrder: problem pulling order:" + myE.ToString());

            }
             */
        }

        /// <summary>
        /// Modify an order 
        /// </summary>
        /// <param name="msg"></param>
        public override OrderReplaceResult modifyOrder(DriverBase.ModifyRequestData replaceData)
        {
            try
            {
               
                // Get the context - we must have this to access the  order            
                if (replaceData.Price.HasValue)
                {
                    replaceData.OrderContext.Price = (decimal)replaceData.Price.Value;
                }

                if (replaceData.StopPrice.HasValue)
                {
                    replaceData.OrderContext.StopPrice = (decimal)replaceData.StopPrice.Value;
                }



                // modify the qty 
                if (replaceData.qty.HasValue)
                {
                    replaceData.OrderContext.OrderQty = (int)replaceData.qty.Value;
                    // force the leaves qty for the purpose of a simple simulation
                    //myContext.LeavesQty = (int)newOrderQty.getValue();
                }



              

                // record the context against the new clordid
                //RecordOrderContext(replaceData.ClOrdID, replaceData.OrderContext);
                //m_OrderContextClOrdID.Add(replaceData.ClOrdID, replaceData.OrderContext);

                // send order in book exec report
                // fully fill
                KaiTrade.Interfaces.IFill fill = new K2DataObjects.Fill();
                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.REPLACED;
                fill.ExecType = KaiTrade.Interfaces.ExecType.ORDER_STATUS;
                fill.OrderID = replaceData.OrderContext.OrderID;
                fill.LastPx = 0;
                fill.AvgPx = 0;

                sendExecReport(replaceData.OrderContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, replaceData.OrderContext.LeavesQty, replaceData.OrderContext.CumQty,fill.LastPx,fill.AvgPx ,"", "");


                if (replaceData.Price.HasValue)
                {
                    if (replaceData.Price.Value == 100)
                    {
                        m_FillList.Add(replaceData.OrderContext);
                    }
                }

                return OrderReplaceResult.success;
            }
            catch (Exception myE)
            {

                _log.Error("modifyOrderRD", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                SendAdvisoryMessage("SIM:modifyOrderRD: problem modify order:" + myE.ToString());
                return OrderReplaceResult.error;
            }
        }

        /// <summary>
        /// Modify an order 
        /// </summary>
        /// <param name="msg"></param>
        public void modifyOrder(KaiTrade.Interfaces.IMessage msg)
        {


            KaiTrade.Interfaces.IModifyOrderRequst mod = null;
            try
            {
                mod = JsonConvert.DeserializeObject<K2DataObjects.ModifyOrderRequest>(msg.Data);
                // Extract the raw FIX Message from the inbound message
                string strOrder = msg.Data;

                KaiTrade.Interfaces.IFill fill = new K2DataObjects.Fill();
                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.REPLACED;
                fill.ExecType = KaiTrade.Interfaces.ExecType.ORDER_STATUS;
                fill.OrderID = DriverBase.Identities.Instance.getNextOrderID();


                if (mod.ClOrdID.Length == 0)
                {

                    sendCancelReplaceRej(mod, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a clordid must be specified on a modify order");
                    Exception myE = new Exception("a clordid must be specified on a modify order");
                    throw myE;
                }

                if (mod.OrigClOrdID.Length == 0)
                {
                    sendCancelReplaceRej(mod, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a original clordid must be specified on a modify order");
                    Exception myE = new Exception("an original clordid must be specified on a modify order");
                    throw myE;
                }

                // is this new market processing?

                    if (m_Markets.ContainsKey(mod.Mnemonic))
                    {
                        m_Markets[mod.Mnemonic].modifyOrder(msg);
                        return;
                    }
 

                // Get the context - we must have this to access the CQG order
                DriverBase.OrderContext myContext = null;
                if (_clOrdIDOrderMap.ContainsKey(mod.OrigClOrdID))
                {
                    myContext = _clOrdIDOrderMap[mod.OrigClOrdID];
                }
                if (myContext == null)
                {
                    sendCancelReplaceRej(mod, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "an order does not exist for the modify requested");
                    Exception myE = new Exception("an order does not exist for the modify requested");
                    throw myE;
                }


                // modify the limit price
                if (mod.Price.HasValue)
                {
                    myContext.Price = (decimal)mod.Price.Value;
                }

                // modify the stop price
                if (mod.StopPrice.HasValue)
                {
                    myContext.StopPrice = (decimal)mod.StopPrice.Value;
                }


                // modify the qtyqty
                if (mod.Qty.HasValue)
                {
                    myContext.OrderQty = (int)mod.Qty.Value;

                }

                // update the ClOrdID's on our order in the context
                myContext.ClOrdID = mod.ClOrdID;
                myContext.OrigClOrdID = mod.OrigClOrdID;

                // record the context against the new clordid
                _clOrdIDOrderMap.Add(mod.ClOrdID, myContext);

                // send order in book exec report
                sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, (int)myContext.LeavesQty, myContext.CumQty, 0.0, 0.0, "", "");

            }
            catch (Exception myE)
            {

                Log.Error("modifyOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                SendAdvisoryMessage("modifyOrder: problem modify order:" + myE.ToString());

            }
     
        }

        
        

        private void SRCallback(IAsyncResult ar)
        {
        }
        /// <summary>
        /// Send the resposnse async
        /// </summary>
        /// <param name="myMsgType"></param>
        /// <param name="myMsg"></param>
        private void handleResponse(string myMsgType, string myMsg)
        {
            _log.Error("TEST:" + myMsg);
            sendResponse(myMsgType, myMsg);
        }

        /// <summary>
        /// Override the base driver DoSend - this is where we process 
        /// incomming requests from the system as a whole
        /// </summary>
        /// <param name="myMsg"></param>
        protected override void DoSend(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                lock (this)
                {
                    // Here are the typical FIX Messages you will receive to
                    // Support Order Routing
                    switch (myMsg.Label)
                    {
                        case "D":
                            submitOrder(myMsg);
                            break;
                        case "F":
                            // Pull or Cancel and Order
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



                        default:
                            // No Action - Discard Message
                            break;
                    }
                }


            }
            catch (Exception myE)
            {
                _log.Error("Driver.Send", myE);
            }
        }



        /// <summary>
        /// Override the base driver DoStop method
        /// </summary>
        protected override void DoStop()
        {
            try
            {
                this.StopTimer();
                m_GenPrices = false;
                m_PriceThread.Abort();
                
                
                // Report that we are stopped - this will show in the Driver view
                // in the dashboard 
                SendStatusMessage(KaiTrade.Interfaces.Status.closed, "TDA Driver is closed");

                _clOrdIDOrderMap.Clear();
                m_OrderContextOrdID.Clear();
                
                _products.Clear();

                foreach (L1PriceSupport.PXPublisher myPub in _publisherRegister.Values)
                {
                    (myPub as KaiTrade.Interfaces.IPublisher).Status = KaiTrade.Interfaces.Status.closed;
                }

                _publisherRegister.Clear();
                setStatus(KaiTrade.Interfaces.Status.closed);
                _runningState = new DriverBase.DriverStatus(StatusConditon.none, StatusConditon.none);
                
            }
            catch (Exception myE)
            {
                _log.Error("Kaitrade.Interfaces.Driver.Stop:", myE);
            }
        }


        /// <summary>
        /// Override the base driver's un register publisher method
        /// </summary>
        /// <param name="myPublisher"></param>
        protected override void DoUnRegister(KaiTrade.Interfaces.IPublisher myPublisher)
        {
            try
            {
                if (_publisherRegister.ContainsKey(myPublisher.TopicID()))
                {
                    _publisherRegister.Remove(myPublisher.TopicID());
                }

                //UnSubscribeMD(myPublisher);
            }
            catch (Exception myE)
            {
                _log.Error("Driver.UnRegister:publisher", myE);
            }
        }

         

    }
}
