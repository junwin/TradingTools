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
using Newtonsoft.Json;
using K2ServiceInterface;

namespace KTAIB
{
    
	
    public enum Operation_Type
    {
        Insert, Update, Delete
    }
    public enum PxSide
    {
        Ask, Bid
    }
    /// <summary>
    /// Class to provide connectivity to Interavtive brokers using TWS app
    /// </summary>
    public class KTAIB : DriverBase.DriverBase
    {
        /// <summary>
        /// Form uses to manage the TWS connection
        /// </summary>
        private Host m_Host = null;

        /// <summary>
        /// Next  ID for IB TWS requests - supplied by them - set to a value at start time
        /// </summary>
        private int m_NextID = 0;

        /// <summary>
        /// IB TWS driver config
        /// </summary>
        private KAI.kaitns.KATIBConfig m_Config = null;

        /// <summary>
        /// provide access to the order context using ClOrdID
        /// </summary>
        //private Dictionary<string, OrderContext> m_ClOrdIDCntxMap;

        /// <summary>
        /// provide access to the order context using IB's order ID
        /// </summary>
        //private Dictionary<int, OrderContext> m_IBOrderCntxMap;

        /// <summary>
        /// Maps IB request ID to a publisher
        /// </summary>
        private Dictionary<int, KaiTrade.Interfaces.TradableProduct> m_IBReqIDProductMap;

        /// <summary>
        /// map  an IB request to a TSSet
        /// </summary>
        private Dictionary<int, KaiTrade.Interfaces.ITSSet> m_IBReqIDTSSet;

        /// <summary>
        /// Fields we want on price subscriptions
        /// </summary>
        private string m_QuoteFields = "";

        private Object m_doSendLock = new Object();
        private Object m_DoStartToken = new object();

        private List<KaiTrade.Interfaces.TradableProduct> m_ContractDetailsRequest;


        public KTAIB()
        {
            // initialize member data
            m_Name = "KTA IB TWS  Driver";
            m_ID = "KTIBTWS";
            m_Tag = "";

            _log.Info(m_Name + " Created");
            m_ContractDetailsRequest = new List<KaiTrade.Interfaces.TradableProduct>();
            //m_ClOrdIDCntxMap = new Dictionary<string, OrderContext>();
            //m_IBOrderCntxMap = new Dictionary<int, OrderContext>();
            m_IBReqIDProductMap = new Dictionary<int, KaiTrade.Interfaces.TradableProduct>();
            m_IBReqIDTSSet = new Dictionary<int, KaiTrade.Interfaces.ITSSet>();

            m_QuoteFields = "100,101,104,106,165,221,225,236";
            // create show the IB TWS host form
            m_Host = new Host();
            m_Host.Show();
            m_Host.Adapter = this;
            //m_Host.TWS.c
            // hook up IB events with our handlers
            this.wireIBEvents();

            // use thread to call start/ retry
            UseWatchDogStart = true;


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
                    m_Host.Show();
                }
                else
                {
                    m_Host.Hide();
                }
            }
            catch  
            {
            }
        }

        protected override void DoStart(string myPath)
        {
            lock (m_DoStartToken)
            {
                try
                {

                    _log.Info("KT IB(TWS) Driver Started");

                    // are we already open?
                    if ((Status == KaiTrade.Interfaces.Status.open) || (Status == KaiTrade.Interfaces.Status.opening))
                    {
                        _log.Info("KT IB(TWS) Already Open");
                        return;
                    }

                    m_ContractDetailsRequest = new List<KaiTrade.Interfaces.TradableProduct>();

                    
                    this.setStatus(KaiTrade.Interfaces.Status.opening);


                    // try load our config file
                    processConfigFile(m_ConfigPath + "KTAIBConfig.xml");

                    // default to local machine unless they specify elsewise
                    if (!m_Config.IPEndpoint.IsValidServer)
                    {
                        m_Config.IPEndpoint.Server = System.Environment.MachineName;
                        m_Config.IPEndpoint.Server = "";
                    }
                    if (m_Host == null)
                    {
                        m_Host = new Host();
                        m_Host.Show();
                        m_Host.Adapter = this;
                    }
                    m_Host.TWSConnect(m_Config.IPEndpoint.Server, (int)m_Config.IPEndpoint.Port, m_Config.ClientID);


                    if (base.m_State.IsValidHideDriverUI)
                    {
                        if (base.m_State.HideDriverUI)
                        {
                            m_Host.Hide();
                        }
                    }
                    // connect to IB TWS - you need to enable this on the TWS workstation
                    //m_Host.TWS.connect(m_Config.IPEndpoint.Server, (int)m_Config.IPEndpoint.Port, m_Config.ClientID);

                    

                    // if the server responds to this we consider ourselves open
                    //m_Host.TWS.reqCurrentTime();

                    // Report that we are open - this will show in the Driver view
                    // in the dashboard
                    //this.setStatus(KaiTrade.Interfaces.Status.open);
                }
                catch (Exception myE)
                {
                    _log.Error("doStart", myE);
                }
            }

        }

        protected override void DoStop()
        {
            try
            {
                m_Host.TWS.disconnect();
                this.unWireIBEvents();

                m_Host.Close();
                m_Host = null;

                
                // Report that we are open - this will show in the Driver view
                // in the dashboard
                this.setStatus(KaiTrade.Interfaces.Status.closed);

            }
            catch (Exception myE)
            {
                _log.Error("doStop", myE);
            }
        }

        private void processConfigFile(string myPath)
        {
            try
            {
                m_Config = new KAI.kaitns.KATIBConfig();
                m_Config.FromXmlFile(myPath);

                
            }
            catch (Exception myE)
            {
                _log.Error("processConfigFile", myE);
            }
        }

        /// <summary>
        /// generate a blank config file
        /// </summary>
        private void genDummyConfig()
        {
            try
            {
                m_Config = new KAI.kaitns.KATIBConfig();
                KAI.kaitns.IPEndpoint myEP = new KAI.kaitns.IPEndpoint();
                myEP.Port = 7496;
                m_Config.IPEndpoint = myEP;
                m_Config.ClientID = 0;
                m_Config.ToXmlFile("KTAIBConfigTEMP.xml");
            }
            catch (Exception myE)
            {
                _log.Error("genDummyConfig", myE);
            }

        }

        /// <summary>
        /// wire IB events
        /// </summary>
        private void wireIBEvents()
        {
            try
            {
                m_Host.TWS.tickPrice += new AxTWSLib._DTwsEvents_tickPriceEventHandler(TWS_tickPrice);
                m_Host.TWS.tickSize += new AxTWSLib._DTwsEvents_tickSizeEventHandler(TWS_tickSize);
                m_Host.TWS.errMsg += new AxTWSLib._DTwsEvents_errMsgEventHandler(TWS_errMsg);
                m_Host.TWS.nextValidId += new AxTWSLib._DTwsEvents_nextValidIdEventHandler(TWS_nextValidId);
                m_Host.TWS.orderStatus += new AxTWSLib._DTwsEvents_orderStatusEventHandler(TWS_orderStatus);
                m_Host.TWS.openOrderEx += new AxTWSLib._DTwsEvents_openOrderExEventHandler(TWS_openOrderEx);
                m_Host.TWS.connectionClosed += new EventHandler(TWS_connectionClosed);
                
                m_Host.TWS.updateMktDepth += new AxTWSLib._DTwsEvents_updateMktDepthEventHandler(TWS_updateMktDepth);
                m_Host.TWS.updateMktDepthL2 += new AxTWSLib._DTwsEvents_updateMktDepthL2EventHandler(TWS_updateMktDepthL2);
                m_Host.TWS.contractDetailsEx += new AxTWSLib._DTwsEvents_contractDetailsExEventHandler(TWS_contractDetailsEx);
                m_Host.TWS.currentTime += new AxTWSLib._DTwsEvents_currentTimeEventHandler(TWS_currentTime);
                m_Host.TWS.historicalData += new AxTWSLib._DTwsEvents_historicalDataEventHandler(TWS_historicalData);
                m_Host.TWS.realtimeBar += new AxTWSLib._DTwsEvents_realtimeBarEventHandler(TWS_realtimeBar);
                 
                 
            }
            catch (Exception myE)
            {
                _log.Error("wireIBEvents", myE);
            }

        }

        void TWS_realtimeBar(object sender, AxTWSLib._DTwsEvents_realtimeBarEvent e)
        {
            try
            {
                if (m_IBReqIDTSSet.ContainsKey(e.tickerId))
                {
                    KaiTrade.Interfaces.ITSSet tsSet = m_IBReqIDTSSet[e.tickerId];


                    // get a new TS item
                    KaiTrade.Interfaces.ITSItem tsItem = tsSet.GetNewItem();

                    DumpRecord(tsItem, tsSet, e);
                    tsSet.AddItem(tsItem);

                    tsSet.Added = true;

                }
                else
                {
                    _log.Error("TWS_realtimeBar:unknown request id");
                }
            }
            catch (Exception myE)
            {
                _log.Error("TWS_realtimeBar", myE);
            }
        }

        /// <summary>
        /// Process historical data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TWS_historicalData(object sender, AxTWSLib._DTwsEvents_historicalDataEvent e)
        {
            try
            {
                if (m_IBReqIDTSSet.ContainsKey(e.reqId))
                {
                    KaiTrade.Interfaces.ITSSet tsSet = m_IBReqIDTSSet[e.reqId];
                   

                    // get a new TS item
                    KaiTrade.Interfaces.ITSItem tsItem = tsSet.GetNewItem();
                    if (tsSet.Items.Count > 200)
                    {
                        tsSet.Added = true;
                        tsSet.Items.Clear();
                    }
                    DumpRecord(tsItem, tsSet,e);
                    tsSet.AddItem(tsItem);
                    if (tsItem.Tag.ToString().IndexOf("fin",0) >= 0)
                    {
                        tsSet.Added = true;
                    }
                    if (tsSet.Items.Count > 200)
                    {
                    }

                    _log.Info("BAR: " + tsItem.ToString());
                    
                }
                else
                {
                    _log.Error("TWS_historicalData:unknown request id");
                }
            }
            catch (Exception myE)
            {
                _log.Error("TWS_historicalData", myE);
            }
        }

        private void DumpRecord(KaiTrade.Interfaces.ITSItem tsItem, KaiTrade.Interfaces.ITSSet tsSet, AxTWSLib._DTwsEvents_realtimeBarEvent e)
        {
            try
            {
                // get a new TS item
                tsItem.Index = e.count;

                tsItem.Tag = e.time;
                //tsItem.TimeStamp = e.date
                //tsItem.TimeStamp = e.date;
                tsItem.Open = e.open;
                tsItem.High = e.high;
                tsItem.Low = e.low;
                tsItem.Close = e.close;
                tsItem.Volume = e.volume;
                DateTime result=new DateTime(1970,1,1);
                TimeSpan span = new TimeSpan(0, 0, e.time);
                result.Add(span);
                tsItem.TimeStamp = result;

                

            }
            catch (Exception myE)
            {
                _log.Error("DumpRecord", myE);
            }
        }

        private void DumpRecord(KaiTrade.Interfaces.ITSItem tsItem, KaiTrade.Interfaces.ITSSet tsSet, AxTWSLib._DTwsEvents_historicalDataEvent e)
        {
            try
            {
                // get a new TS item
                tsItem.Index = e.barCount;
                
                tsItem.Tag = e.date;
                //tsItem.TimeStamp = e.date
                //tsItem.TimeStamp = e.date;
                tsItem.Open = e.open;
                tsItem.High = e.high;
                tsItem.Low = e.low;
                tsItem.Close =e.close;
                tsItem.Volume = e.volume;
                DateTime result;
                if (e.date.Length > 10)
                {
                    DriverBase.DriverUtils.FromLocalMktDate(out result, e.date.Substring(0, 8), e.date.Substring(10, 8));
                }
                else
                {
                    DriverBase.DriverUtils.FromLocalMktDate(out result, e.date);
                }
                
                tsItem.TimeStamp = result;

                //tsItem.AskVolume = e.;
                //tsItem.BidVolume = myBar.BidVolume;
                // e.volume
                //tsItem.Mid = e.m;

                //tsItem.HLC3 = myBar.HLC3;
                //tsItem.Avg = myBar.Avg;
                //tsItem.TrueHigh = myBar.TrueHigh;
                //tsItem.TrueLow = myBar.TrueLow;
                //tsItem.Range = myBar.Range;
                //tsItem.TrueRange = myBar.TrueRange;
                //tsItem.Index = int.Parse(e.date);

            }
            catch (Exception myE)
            {
                _log.Error("DumpRecord", myE);
            }
        }

        void TWS_currentTime(object sender, AxTWSLib._DTwsEvents_currentTimeEvent e)
        {
            try
            {
                // We use this to determine if the connection is open
                if (this.Status != KaiTrade.Interfaces.Status.open)
                {
                    this.setStatus(KaiTrade.Interfaces.Status.open);
                }
            }
            catch (Exception myE)
            {
                _log.Error("TWS_currentTime", myE);
            }
        }

        /// <summary>
        /// Request the product details, get the driver to access the product and fill in 
        /// product details in the kaitrade product object.
        /// Note that not all frivers support this and that the call may take some
        /// time to set the values.
        /// </summary>
        /// <param name="myProduct"></param>
        public override void RequestProductDetails(KaiTrade.Interfaces.TradableProduct myProduct)
        {
            try
            {
                TWSLib.IContract myIBContract = getIBContract(myProduct);
                m_ContractDetailsRequest.Add(myProduct);
                m_Host.TWS.reqContractDetailsEx(m_ContractDetailsRequest.Count -1, myIBContract);
            }
            catch (Exception myE)
            {
                _log.Error("RequestProductDetails", myE);
            }
        }

        void TWS_contractDetailsEx(object sender, AxTWSLib._DTwsEvents_contractDetailsExEvent e)
        {
            try
            {
                KaiTrade.Interfaces.TradableProduct myProduct = m_ContractDetailsRequest[e.reqId];

                if (myProduct.TradeVenueSequence == 0)
                {
                    myProduct.TradeVenueSequence += 1;
                }
                else
                {
                    // this is not the first definition
                    long venueSeq = myProduct.TradeVenueSequence;
                    myProduct.TradeVenueSequence += 1;
                    myProduct = Facade.Factory.GetProductManager().CloneProduct(myProduct.Identity);
                    myProduct.Mnemonic = myProduct.Mnemonic + "." + venueSeq.ToString();
                    Facade.Factory.GetProductManager().RegisterProduct(myProduct);

                }
                

                //string zztemp = e.contractDetails.summary.ToString();
                 
                myProduct.TickSize = (decimal)e.contractDetails.minTick;
                myProduct.LongName = e.contractDetails.longName;
                
                string[] temp = myProduct.TickSize.ToString().Split('.');
                if (temp.Length > 1)
                {
                    myProduct.NumberDecimalPlaces = temp[1].Length;
                }
                else
                {
                    myProduct.NumberDecimalPlaces = 0;
                }
                if ( (e.contractDetails.contractMonth.Length > 0))
                {
                    myProduct.MMY = e.contractDetails.contractMonth;
                        
                }

                if (myProduct.CFICode != null)
                {
                    if (myProduct.CFICode.ToString()[0] == 'E')
                    {
                        myProduct.PriceFeedQuantityMultiplier = 100;
                    }
                    else
                    {
                        myProduct.PriceFeedQuantityMultiplier = 1;
                    }
                }
            }
            catch (Exception myE)
            {
                _log.Error("TWS_contractDetailsEx", myE);
            }
        }

        void TWS_updateMktDepthL2(object sender, AxTWSLib._DTwsEvents_updateMktDepthL2Event e)
        {
            try
            {
                updateMktDepth(e.id, e.position, e.marketMaker, e.operation, e.side, e.price, e.size);
            }
            catch (Exception myE)
            {
                _log.Error("TWS_updateMktDepthL2", myE);
            }
        }

        void TWS_updateMktDepth(object sender, AxTWSLib._DTwsEvents_updateMktDepthEvent e)
        {
            try
            {
                updateMktDepth(e.id, e.position, "", e.operation, e.side, e.price, e.size);
            }
            catch (Exception myE)
            {
                _log.Error("TWS_updateMktDepth", myE);
            }  
        }

        private KaiTrade.Interfaces.PXDepthOperation GetDepthOperation(int ibOpType)
        {
            KaiTrade.Interfaces.PXDepthOperation ret = KaiTrade.Interfaces.PXDepthOperation.none;
            try
            {
                switch (ibOpType)
                {
                    case 0:
                        ret = KaiTrade.Interfaces.PXDepthOperation.insert;
                        break;
                    case 1:
                        ret = KaiTrade.Interfaces.PXDepthOperation.update;
                        break;
                    case 2:
                        ret = KaiTrade.Interfaces.PXDepthOperation.delete;
                        break;
                    default:
                        ret = KaiTrade.Interfaces.PXDepthOperation.none;
                        break;
                 
                }
                }
            catch(Exception myE)
            {
                _log.Error("GetDepthOperation", myE);
            }
            return ret;
        }
        private void updateMktDepth(int myId, int myRow, string marketMaker, int operation, int mySide, double price, int size)
        {
             
            try
            {
                if (m_IBReqIDProductMap.ContainsKey(myId))
                {
                    KaiTrade.TradeObjects.PXUpdateBase pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                    pxupdate.Mnemonic = m_IBReqIDProductMap[myId].Mnemonic;
                    pxupdate.DepthPosition = myRow;
                    pxupdate.DepthOperation = GetDepthOperation(operation);

                    pxupdate.DepthMarket = marketMaker;
                    pxupdate.Ticks = DateTime.Now.Ticks;
                    switch (mySide)
                    {
                        case 0:
                            //ASK
                            pxupdate.OfferPrice = (decimal)price;
                            pxupdate.OfferSize = (decimal)size;
                            
                            break;
                        case 1:
                            //BID
                            pxupdate.BidPrice = (decimal)price;
                            pxupdate.BidSize = (decimal)size;
                             
                            break;
                        default:
                            break;
                    }
                    //myPub.APIUpdateTime = DateTime.Now;
                    ApplyPriceUpdate(pxupdate);
                    
                }


            }
            catch (Exception myE)
            {
                _log.Error("updateMktDepth", myE);
            } 
              
        }

        /// <summary>
        /// Unrwires IB
        /// </summary>
        private void unWireIBEvents()
        {
            try
            {
                m_Host.TWS.tickPrice -= new AxTWSLib._DTwsEvents_tickPriceEventHandler(TWS_tickPrice);
                m_Host.TWS.tickSize -= new AxTWSLib._DTwsEvents_tickSizeEventHandler(TWS_tickSize);
                m_Host.TWS.errMsg -= new AxTWSLib._DTwsEvents_errMsgEventHandler(TWS_errMsg);
                m_Host.TWS.nextValidId -= new AxTWSLib._DTwsEvents_nextValidIdEventHandler(TWS_nextValidId);
                m_Host.TWS.orderStatus -= new AxTWSLib._DTwsEvents_orderStatusEventHandler(TWS_orderStatus);
                m_Host.TWS.openOrderEx -= new AxTWSLib._DTwsEvents_openOrderExEventHandler(TWS_openOrderEx);
                m_Host.TWS.connectionClosed -= new EventHandler(TWS_connectionClosed);
                m_Host.TWS.contractDetailsEx -=  new AxTWSLib._DTwsEvents_contractDetailsExEventHandler(TWS_contractDetailsEx);
                m_Host.TWS.currentTime -= new AxTWSLib._DTwsEvents_currentTimeEventHandler(TWS_currentTime);

                
            }
            catch (Exception myE)
            {
                _log.Error("unWireIBEvents", myE);
            }
        }

        public void doSend(KaiTrade.Interfaces.IMessage myMsg)
        {
            lock (m_doSendLock)
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
                catch (Exception myE)
                {
                    _log.Error("doSend", myE);
                }
            }
        }

        public void doSubscribeMD(KaiTrade.Interfaces.Publisher myPub, int depthLevels, string requestID)
        {
            try
            {
                
                // Subscribe to the market data from TWS
               

                // get the product
                KaiTrade.Interfaces.TradableProduct product = m_Facade.Factory.GetProductManager().GetProductMnemonic(myPub.TopicID());

                // IB's newest API uses its IContract  
                // Note the PXPublisher must have a valid product
                if (product == null)
                {
                    throw new Exception("The PXPublisher does not have a product");
                }
                // register the pub against the IB ReqID - updates pass that back

                m_IBReqIDProductMap.Add(++m_NextID, product);

                TWSLib.IContract myContract = getIBContract(product);
                
                myContract.multiplier = "0";
               // m_Host.TWS.reqMktDataEx(m_NextID, myContract, m_QuoteFields, 0);

                 
                if (depthLevels > 0)
                {
                    m_Host.TWS.reqMktDepthEx(m_NextID, myContract, depthLevels);
                    m_Host.TWS.reqMktDataEx(m_NextID, myContract, m_QuoteFields, 0);
                }
                else
                {
                    m_Host.TWS.reqMktDataEx(m_NextID, myContract, m_QuoteFields, 0);
                }
                 
                



            }
            catch (Exception myE)
            {
                _log.Error("doSubscribeMD", myE);
            }

        }

       
        public void doUnRegister(KaiTrade.Interfaces.Publisher myPublisher)
        {
        }

        /// <summary>
        /// submit an order to IB using a incomming FIX new order single
        /// </summary>
        /// <param name="myMsg"></param>
        private void submitOrder(KaiTrade.Interfaces.IMessage myMsg)
        {
            QuickFix.Message myQFOrder = null;
            try
            {
                // Extract the raw FIX Message from the inbound message
                string strOrder = myMsg.Data;

                // Use QuickFix to handle the message
                myQFOrder = new QuickFix.Message(strOrder);

                // Use product manager to validate the product specified on 
                // the order exists for this adapter

                // Get the product associated with the FIX message


                //QuickFix.Symbol symbol = new QuickFix.Symbol();
                //myQFOrder.getField(symbol);

                QuickFix.Side mySide = new QuickFix.Side();
                QuickFix.OrdType myOrdType = new QuickFix.OrdType();
                QuickFix.OrderQty myOrderQty = new QuickFix.OrderQty();
                QuickFix.Price myPrice = new QuickFix.Price();
                QuickFix.StopPx myStopPx = new QuickFix.StopPx();
                QuickFix.Account myAccount = new QuickFix.Account();
                
                QuickFix.ClOrdID myClOrdID = new QuickFix.ClOrdID();
                QuickFix.TimeInForce myTimeInForce = new QuickFix.TimeInForce();


                // the account code is mandatory
                if (myQFOrder.isSetField(myAccount))
                {
                    myQFOrder.getField(myAccount);
                }
                else
                {
                    this.SendAdvisoryMessage("IB TWS: you need to provide a valid account");
                    throw new Exception("IB TWS: you need to provide a valid account");
                }

                myQFOrder.getField(myClOrdID);

                // Get the Order type
                myQFOrder.getField(myOrdType);


                // Get the QTY
                myQFOrder.getField(myOrderQty);
                

                // get the Side of the order
                myQFOrder.getField(mySide);
                

                // Set order duration 
                myQFOrder.getField(myTimeInForce);
                

                // Prices
                if (myQFOrder.isSetField(myPrice))
                {
                    myQFOrder.getField(myPrice);
                }

                if (myQFOrder.isSetField(myStopPx))
                {
                    myQFOrder.getField(myStopPx);
                }

                // get the contract
                TWSLib.IContract myContract = getIBContract(myQFOrder);
                if (myContract == null)
                {
                    this.SendAdvisoryMessage("IB TWS: cannot find a valid contract");
                    throw new Exception("IB TWS: cannot find a valid contract");
                }

                // create an IB Order
                TWSLib.IOrder myIBOrder = m_Host.TWS.createOrder();

                myIBOrder.whatIf = 0;
                myIBOrder.account = myAccount.getValue();

                if(myOrdType.getValue() == QuickFix.OrdType.LIMIT)
                {
                    myIBOrder.orderType = "LMT";
                    myIBOrder.lmtPrice = myPrice.getValue();
                }
                else if(myOrdType.getValue() == QuickFix.OrdType.MARKET)
                {
                    myIBOrder.orderType = "MKT";
                }
                else if (myOrdType.getValue() == QuickFix.OrdType.STOP)
                {
                    myIBOrder.orderType = "STP";
                    myIBOrder.auxPrice = myStopPx.getValue();
                    if (myPrice.getValue() == -1)
                    {
                        myIBOrder.lmtPrice = myStopPx.getValue();
                    }
                    else
                    {
                        myIBOrder.lmtPrice = myPrice.getValue();
                    }
                }
                else if (myOrdType.getValue() == QuickFix.OrdType.STOP_LIMIT)
                {
                    myIBOrder.orderType = "STP";
                    myIBOrder.auxPrice = myStopPx.getValue();
                    if (myPrice.getValue() == -1)
                    {
                        myIBOrder.lmtPrice = myStopPx.getValue();
                    }
                    else
                    {
                        myIBOrder.lmtPrice = myPrice.getValue();
                    }
                }
                else
                {
                    this.SendAdvisoryMessage("IB TWS: order type not supported");
                    throw new Exception("IB TWS: order type not supported");
                }

                myIBOrder.totalQuantity = (int) myOrderQty.getValue();

                // Side
                string myAction = KaiUtil.QFUtils.DecodeSide(mySide).ToUpper();
                myIBOrder.action = myAction;

                // Time in force
                string myTIF = KaiUtil.QFUtils.DecodeTimeInForce(myTimeInForce).ToUpper();
                myIBOrder.timeInForce = myTIF;

                if (myOrdType.getValue() == QuickFix.OrdType.LIMIT)
                {
                    myIBOrder.lmtPrice = myPrice.getValue();
                }

                DriverBase.OrderContext myCntx = RecordOrderContext(myClOrdID.getValue(), myQFOrder, null, m_NextID.ToString());
                SetContextCommand(myCntx, DriverBase.ORCommand.Undefined, DriverBase.ORCommand.Submit);

                myCntx.OrderQty = myIBOrder.totalQuantity;
                myCntx.CumQty = 0;
                
                m_Host.TWS.placeOrderEx(m_NextID++, myContract, myIBOrder);

                
            }
            catch (Exception myE)
            {

                _log.Error("submitOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("IB TWS:submitOrder: problem submitting order:" + myE.ToString());

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

       

        #region productHandling

        private string CFI2IBSecType(string myCFICode)
        {
            string myRet = "";
            switch (myCFICode)
            {
                case KaiTrade.Interfaces.CFICode.STOCK:
                    myRet = "STK";
                    break;
                case KaiTrade.Interfaces.CFICode.FUTURE:
                    myRet = "FUT";
                    break;
                case KaiTrade.Interfaces.CFICode.CALLOPTION:
                case KaiTrade.Interfaces.CFICode.PUTOPTION:
                    myRet = "OPT";
                    break;
                case KaiTrade.Interfaces.CFICode.PUTOPTIONFUTURE:
                    myRet = "FOP";
                    break;
                case KaiTrade.Interfaces.CFICode.CALLOPTIONFUTURE:
                    myRet = "FOP";
                    break;
                case KaiTrade.Interfaces.CFICode.FX:
                    myRet = "CASH";
                    break;
                default:
                    break;
            }
            return myRet;
        }

        public override List<KaiTrade.Interfaces.IVenueTradeDestination> GetTradeDestinations(string cfiCode)
        {
            
            List<KaiTrade.Interfaces.IVenueTradeDestination> destinations = new List<KaiTrade.Interfaces.IVenueTradeDestination>();
            try
            {
                K2DataObjects.TradeVenueDestination dest = new K2DataObjects.TradeVenueDestination();
                if (cfiCode == KaiTrade.Interfaces.CFICode.FX)
                {
                    dest.ExchangeCode = "";
                    dest.ExDestination = "IDEALPRO";
                    dest.PrimaryCFICode = cfiCode;
                    dest.VenueCode =  "KTIBTWS";
                }
                else
                {
                    dest.ExchangeCode = "";
                    dest.ExDestination = "SMART";
                    dest.PrimaryCFICode = cfiCode;
                    dest.VenueCode = "KTIBTWS";
                }
            }
            catch (Exception myE)
            {
                _log.Error("GetTradeDestinations", myE); 
            }
            return destinations;
        }

       
           

        private TWSLib.IContract getIBContract(KaiTrade.Interfaces.TradableProduct myProduct)
        {
            TWSLib.IContract myContract = null;
            try
            {
                if (m_ExternalProduct.ContainsKey(myProduct.Identity))
                {
                    myContract = m_ExternalProduct[myProduct.Identity] as TWSLib.IContract;
                    if (myContract != null)
                    {
                        return myContract;
                    }
                }
                myContract = m_Host.TWS.createContract();
                // Sumbol takes priority over the security ID
                if (myProduct.Symbol.Length> 0 )
                {
                    myContract.symbol = myProduct.Symbol;
                }
                else
                {                
                    if (myProduct.SecurityID != null)
                    {
                        myContract.symbol = myProduct.SecurityID;
                    }
                }

                
                myContract.secType = CFI2IBSecType(myProduct.CFICode);
                

                
                if (myProduct.Exchange != null)
                {

                    myContract.primaryExchange = myProduct.Exchange;
                }

                if (myProduct.ExDestination != null)
                {

                    myContract.exchange = myProduct.ExDestination;
                }
                
                
                if (myProduct.Currency != null)
                {
                    
                    //myContract.currency = myProduct.Currency;
                }
                else
                {
                    //myContract.currency = "GBP"; ;
                }

               


                switch (myProduct.CFICode)
                {
                    case KaiTrade.Interfaces.CFICode.STOCK:
                        // no action
                        break;
                    case KaiTrade.Interfaces.CFICode.FX:
                        // no action
                        break;
                    case KaiTrade.Interfaces.CFICode.FUTURE:
                        // get the mmy
                        if (myProduct.MMY != null)
                        {
                            myContract.expiry = myProduct.MMY.ToString();
                        }
                        break;
                    case KaiTrade.Interfaces.CFICode.PUTOPTION:
                    case KaiTrade.Interfaces.CFICode.PUTOPTIONFUTURE:
                        // get MMY, and strike
                        if (myProduct.MMY != null)
                        {
                            myContract.expiry = myProduct.MMY.ToString();
                        }
                        if (myProduct.StrikePrice.HasValue)
                        {
                            myContract.strike = (double)myProduct.StrikePrice;
                        }
                        myContract.right = "P"; 
                        break;
                    case KaiTrade.Interfaces.CFICode.CALLOPTION:
                    case KaiTrade.Interfaces.CFICode.CALLOPTIONFUTURE:
                        // get MMY, and strike
                        if (myProduct.MMY != null)
                        {
                            myContract.expiry = myProduct.MMY.ToString();
                        }
                        if (myProduct.StrikePrice.HasValue)
                        {
                            myContract.strike = (double) myProduct.StrikePrice;
                        }
                        myContract.right = "C";
                        break;
                    default:
                        break;
                }
                
                
            }
            catch (Exception myE)
            {
                _log.Error("getIBContract:product", myE);
            }
            return myContract;
        }

       
        #endregion

        /// <summary>
        /// Pull an order using the IB TWS api
        /// </summary>
        /// <param name="myMsg"></param>
        private void pullOrder(KaiTrade.Interfaces.IMessage myMsg)
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

                

               try
               {

                   DriverBase.OrderContext myCntx;

                   // if the origClORdID is set then use that else use the
                   // ClOrdID

                       myCntx = GetOrderContextClID(cancel.OrigClOrdID);
                       if (myCntx == null)
                       {
                           throw new Exception("Cannot locate order context on cancel request");
                       }
                       // record the context against the incomming request
                       RecordOrderContext(myClOrdID.getValue(), myCntx);

                  


                    int myIBOrderID = 0;
                    myIBOrderID = int.Parse(myCntx.APIOrderID);


                    // Cancel the order
                    m_Host.TWS.cancelOrder(myIBOrderID);

                }
                catch (Exception myE)
                {
                    // cancel failed so reject the cancel request
                    _log.Error("pullOrder", myE);
                    if (myPull != null)
                    {
                        sendCancelRej(myPull, QuickFix.CxlRejReason.OTHER, myE.Message);
                    }
                }



            }
            catch (Exception myE)
            {
                _log.Error("pullOrder", myE);
            }
        }


        private void modifyOrder(KaiTrade.Interfaces.IMessage msg)
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

               

                int myIBOrderID = 0;

                    DriverBase.OrderContext myCntx = GetOrderContextClID(mod.OrigClOrdID);
                    if (myCntx == null)
                    {
                        throw new Exception("Cannot locate order context on modify request");
                    }
                    // record the context against the incomming request
                    RecordOrderContext(mod.ClOrdID, myCntx);

                

                // get the contract
                TWSLib.IContract myContract = getIBContract(myCntx.QFOrder);
                if (myContract == null)
                {
                    this.SendAdvisoryMessage("IB TWS: cannot find a valid contract");
                    throw new Exception("IB TWS: cannot find a valid contract");
                }

                // create an IB Order
                TWSLib.IOrder myIBOrder = m_Host.TWS.createOrder();

                myIBOrder.whatIf = 0;
                myIBOrder.account = myCntx.OriginalOrder.Account;

                if (myCntx.OriginalOrder.OrdType == KaiTrade.Interfaces.OrderType.LIMIT)
                {
                    myIBOrder.orderType = "LMT";
                }
                else if (myCntx.OriginalOrder.OrdType == KaiTrade.Interfaces.OrderType.MARKET)
                {
                    myIBOrder.orderType = "MKT";
                }
                else
                {
                    this.SendAdvisoryMessage("IB TWS: order type not supported");
                    throw new Exception("IB TWS: order type not supported");
                }

                myIBOrder.totalQuantity = (int)myCntx.OriginalOrder.OrderQty;

                // Side
                string myAction = myCntx.OriginalOrder.Side;
                myIBOrder.action = myAction;

                // Time in force
                string myTIF = myCntx.OriginalOrder.TimeInForce;
                myIBOrder.timeInForce = myTIF;

                if (myCntx.OriginalOrder.OrdType == KaiTrade.Interfaces.OrderType.LIMIT)
                {
                    myIBOrder.lmtPrice = myCntx.Price;
                }

                myCntx.ClOrdID = mod.ClOrdID;
                //int myIBOrderID = 0;
                myIBOrderID = int.Parse(myCntx.APIOrderID);


                m_Host.TWS.placeOrderEx(myIBOrderID, myContract, myIBOrder);
            }
            catch (Exception myE)
            {
                sendCancelReplaceRej(mod, KaiTrade.Interfaces.CxlRejReason.Other, myE.Message);
                
                _log.Error("modifyOrder", myE);
                _log.Error("modifyOrder:req:"+msg.Data);
            }
        }

        private void XsendExecReport(QuickFix.Message myOrder, QuickFix.OrderID myOrderID, QuickFix.OrdStatus myStatus, QuickFix.ExecType myExecType, double myLastQty, int myLeavesQty, int myCumQty, double myLastPx, double myAvePx)
        {
            sendExecReport(myOrder, myOrderID, myStatus, myExecType, myLastQty, myLeavesQty, myCumQty, myLastPx, myAvePx, "", null);
        }
        /// <summary>
        /// Create and send an Execution report based on the order passed in and the
        /// exec type (the type of request that is causing the report)
        /// </summary>
        /// <param name="myOrder"></param>
        /// <param name="myExecType"></param>
        private void XsendExecReport(QuickFix.Message myOrder, QuickFix.OrderID myOrderID, QuickFix.OrdStatus myStatus, QuickFix.ExecType myExecType, double myLastQty, int myLeavesQty, int myCumQty, double myLastPx, double myAvePx, string myText, QuickFix.OrdRejReason myOrdRejReason)
        {
            try
            {
                string myFixMsg = KaiUtil.QFUtils.GetExecReport(myOrder, myOrderID, myStatus, myExecType, myLastQty, myLeavesQty, myCumQty, myLastPx, myAvePx, myText, myOrdRejReason);


                // send our response message back to the clients of the adapter
                this.sendResponse("8", myFixMsg);
            }
            catch (Exception myE)
            {
                _log.Error("sendExecReport", myE);
            }
        }

        
        /// <summary>
        /// Handles the nextValidId event of the AxTWSLib.AxTws object in the TwsHostForm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The _DTwsEvents_nextValidIdEvent.</param>
        void TWS_nextValidId(object sender, AxTWSLib._DTwsEvents_nextValidIdEvent e)
        {
            m_NextID = e.id;
        }

        /// <summary>
        /// Handles the errMsg event of the AxTWSLib.AxTws object in the TwsHostForm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void TWS_errMsg(object sender, AxTWSLib._DTwsEvents_errMsgEvent e)
        {
            try
            {
                string myError = "TWS_errMsg: " + e.errorMsg + " " + e.errorCode + " " + e.id;
                _log.Error(myError);
                this.SendAdvisoryMessage(myError);
                if (e.errorCode == 504)
                {
                    setStatus(KaiTrade.Interfaces.Status.closed);
                }
                //will reject the order - if any
                if (e.id >= 0)
                {
                    rejectOrder(e.id.ToString(), e.errorMsg);
                }
            }
            catch (Exception myE)
            {
                _log.Error("TWS_errMsg", myE);

            }
        }

        /// <summary>
        /// apply a size update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TWS_tickSize(object sender, AxTWSLib._DTwsEvents_tickSizeEvent e)
        {
            try
            {
                if (m_IBReqIDProductMap.ContainsKey(e.id))
                {
                    this.UpdateSize(m_IBReqIDProductMap[e.id], e);
 
                }
                
            }
            catch (Exception myE)
            {
                _log.Error("TWS_tickSize", myE);

            }

        }

        /// <summary>
        /// Apply a price update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TWS_tickPrice(object sender, AxTWSLib._DTwsEvents_tickPriceEvent e)
        {
            try
            {
                if (m_IBReqIDProductMap.ContainsKey(e.id))
                {
                    this.UpdatePrice(m_IBReqIDProductMap[e.id], e);
                    /*
                    (m_IBReqIDPubMap[e.id] as KaiTrade.Interfaces.Publisher).OnUpdate(null);
                    if ((m_IBReqIDPubMap[e.id] as KaiTrade.Interfaces.Publisher).Status != KaiTrade.Interfaces.Status.open)
                    {
                        (m_IBReqIDPubMap[e.id] as KaiTrade.Interfaces.Publisher).Status = KaiTrade.Interfaces.Status.open;
                    }
                     */
                }

            }
            catch (Exception myE)
            {
                _log.Error("TWS_tickPrice", myE);

            }
           
        }

        


        /// <summary>
        /// Updates the publisher price
        /// </summary>
        /// <param name="myPub"></param>
        /// <param name="e"></param>
        private void UpdatePrice(KaiTrade.Interfaces.TradableProduct product, AxTWSLib._DTwsEvents_tickPriceEvent e)
        {
            try
            {

                KaiTrade.TradeObjects.PXUpdateBase pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
                pxupdate.Mnemonic = product.Mnemonic;
                pxupdate.DepthOperation = KaiTrade.Interfaces.PXDepthOperation.none;

                switch (e.tickType)
                {
                    case 1:
                        pxupdate.BidPrice = (decimal)e.price;
                        break;
                    case 2:
                        pxupdate.OfferPrice = (decimal)e.price;
                        break;
                    case 4:
                        pxupdate.TradePrice = (decimal)e.price;
                        break;
                    case 6:
                        pxupdate.DayHigh = (decimal)e.price;
                        break;
                    case 7:
                        pxupdate.DayLow = (decimal)e.price;
                        break;
                }
                //myPub.APIUpdateTime = DateTime.Now;
                ApplyPriceUpdate(pxupdate);
            }
            catch (Exception myE)
            {
                _log.Error("UpdateMDSubjectPrice", myE);

            }
        }

        /// <summary>
        /// Update the publisher size
        /// </summary>
        /// <param name="mySubject"></param>
        /// <param name="e"></param>
        private void UpdateSize(KaiTrade.Interfaces.TradableProduct product, AxTWSLib._DTwsEvents_tickSizeEvent e)
        {
            KaiTrade.TradeObjects.PXUpdateBase pxupdate = new KaiTrade.TradeObjects.PXUpdateBase(m_ID);
            pxupdate.Mnemonic = product.Mnemonic;

            switch (e.tickType)
            {
                case 0:
                    pxupdate.BidSize = e.size * product.PriceFeedQuantityMultiplier;
                    break;
                case 3:
                    pxupdate.OfferSize = e.size * product.PriceFeedQuantityMultiplier;
                    
                    break;
                case 5:
                    pxupdate.TradeVolume = e.size * product.PriceFeedQuantityMultiplier;

                    break;               
            }
            ApplyPriceUpdate(pxupdate);
            
        }

        /// <summary>
        /// apply an order status change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TWS_orderStatus(object sender, AxTWSLib._DTwsEvents_orderStatusEvent e)
        {
            try
            {
                if (m_DriverLog.IsDebugEnabled)
                {
                    m_DriverLog.Debug("TWS_openOrderEx:" + e.clientId.ToString() + ": " + e.ToString() + ": " + e.status.ToString());
                }
                 
                
                QuickFix.OrdStatus myOrdStatus;
                QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.ORDER_STATUS);

                double myLastFill = 0.0;
                // get the order contect using the IB ID
                DriverBase.OrderContext myCntx = null;

                if (this.m_ApiIDOrderMap.ContainsKey(e.id.ToString()))
                {
                    myCntx = m_ApiIDOrderMap[e.id.ToString()];
                }
                if (myCntx == null)
                {
                    string myErr = "IB TWS: no context found for IB ID:" + e.id.ToString();
                    Exception myE = new Exception(myErr);
                    throw myE;
                }

                myLastFill = myCntx.LeavesQty - e.remaining;
                myCntx.CumQty = e.filled;
                // get the original order from the context
                QuickFix.Message myOrder = myCntx.QFOrder;

                switch (e.status)
                {
                    case "PendingSubmit":
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_NEW);
                        sendExecReport(myOrder, new QuickFix.OrderID(e.id.ToString()), myOrdStatus, myExecType, myLastFill, e.remaining, e.filled, e.lastFillPrice, e.avgFillPrice);
                        break;
                    case "PendingCancel":
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_CANCEL);
                        sendExecReport(myOrder, new QuickFix.OrderID(e.id.ToString()), myOrdStatus, myExecType,   myLastFill, e.remaining, e.filled, e.lastFillPrice, e.avgFillPrice);
                        break;
                    case "PreSubmitted":
                        break;

                    case "Inactive":
                        break;
                    case "Submitted":

                        if (e.filled > 0)
                        {
                            myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PARTIALLY_FILLED);
                            myExecType = new QuickFix.ExecType(QuickFix.ExecType.PARTIAL_FILL);
                        }
                        else
                        {
                            myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.NEW);
                            
                        }
                        // mark the context as done for the sub
                        SetContextCommand(myCntx, DriverBase.ORCommand.Submit, DriverBase.ORCommand.Undefined);

                        sendExecReport(myOrder, new QuickFix.OrderID(e.id.ToString()), myOrdStatus, myExecType,  myLastFill, e.remaining, e.filled, e.lastFillPrice, e.avgFillPrice);
                        break;

                    case "Cancelled":
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.CANCELED);
                        sendExecReport(myOrder, new QuickFix.OrderID(e.id.ToString()), myOrdStatus, myExecType,  myLastFill, e.remaining, e.filled, e.lastFillPrice, e.avgFillPrice);
                        // mark the context as done for the sub
                        SetContextCommand(myCntx, DriverBase.ORCommand.Pull, DriverBase.ORCommand.Undefined);

                        break;

                    case "Filled":
                        myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.FILLED);
                        myExecType = new QuickFix.ExecType(QuickFix.ExecType.FILL);
                        sendExecReport(myOrder, new QuickFix.OrderID(e.id.ToString()), myOrdStatus, myExecType,  myLastFill, e.remaining, e.filled, e.lastFillPrice, e.avgFillPrice);
                        // mark the context as done for the sub
                        SetContextCommand(myCntx, DriverBase.ORCommand.Undefined, DriverBase.ORCommand.Undefined);
                        break;

                    default:
                        _log.Error("TWS_orderStatus:unknown status" + e.status);
                        break;
                }
            }
            catch (Exception myE)
            {
                _log.Error("TWS_orderStatus", myE);
            }
        }

        /// <summary>
        /// Handles the openOrderEx event of the AxTWSLib.AxTws object in the TwsHostForm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void TWS_openOrderEx(object sender, AxTWSLib._DTwsEvents_openOrderExEvent e)
        {
            try
            {
                if (_log.IsDebugEnabled)
                {
                    _log.Debug("TWS_openOrderEx:" + e.orderId.ToString() + ": " + e.contract.ToString() + ": " + e.orderState.ToString());
                }
                switch (e.orderState.status)
                {
                    case "PendingSubmit":
                        break;
                    case "PendingCancel":
                        break;
                    case "PreSubmitted":
                        break;
                    case "Cancelled":
                        break;
                    case "Inactive":
                        break;
                    case "Submitted":
                       
                        break;

                    case "Filled":
                        
                        break;

                    default:
                        _log.Error("TWS_openOrderEx:unknown order state" + e.orderState.status);
                        break;
                }
            }
            catch (Exception myE)
            {
                _log.Error("TWS_openOrderEx", myE);
            }
        }

       

        /// <summary>
        /// Handles the connectionClosed event of the TWS control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void TWS_connectionClosed(object sender, EventArgs e)
        {
            unWireIBEvents();
            this.setStatus(KaiTrade.Interfaces.Status.closed);

        }


        void rejectOrder(string id, string msg)
        {
            try
            {



                QuickFix.OrdStatus myOrdStatus;
                QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.ORDER_STATUS);

                double myLastFill = 0.0;
                // get the order contect using the IB ID
                DriverBase.OrderContext myCntx = null;

                if (this.m_ApiIDOrderMap.ContainsKey(id))
                {
                    myCntx = m_ApiIDOrderMap[id];
                }
                if (myCntx == null)
                {
                    string myErr = "IB TWS: no context found for IB ID:" + id;
                    Exception myE = new Exception(myErr);
                    throw myE;
                }


                // get the original order from the context
                QuickFix.Message myOrder = myCntx.QFOrder;


                myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                QuickFix.OrdRejReason reason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.UNKNOWN);
                sendExecReport(myOrder, new QuickFix.OrderID(id), myOrdStatus, myExecType, myLastFill, 0, 0, 0, 0,msg, reason );
                // mark the context as done for the sub
                SetContextCommand(myCntx, DriverBase.ORCommand.Pull, DriverBase.ORCommand.Undefined);


            }
            catch (Exception myE)
            {
                _log.Error("rejectOrder", myE);
            }
        }

        protected override void SubscribeMD(KaiTrade.Interfaces.Publisher myPub, int depthLevels, string requestID)
        {
            try
            {
                m_Host.SubscribeMD(myPub, depthLevels, requestID);

            }
            catch (Exception myE)
            {
                _log.Error("SubscribeMD", myE);
            }
        }

        /// <summary>
        /// this is where we do the subscribe bar data
        /// </summary>
        /// <param name="myTSSet"></param>
        public void doRequestTS(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            try
            {
                switch (myTSSet.TSType)
                {
                    case KaiTrade.Interfaces.TSType.BarData:
                        getBarData( myTSSet);
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
                        m_DriverLog.Error("Unknown TS Request type:" + myTSSet.TSType.ToString());
                        break;
                }

            }
            catch (Exception myE)
            {
                _log.Error("doRequestTS", myE);
            }
        }
        public override void RequestTSData(KaiTrade.Interfaces.ITSSet myTSSet)
        {
           
                try
                {
                    m_Host.RequestTSData(myTSSet);

                }
                catch (Exception myE)
                {
                    _log.Error("RequestTSData", myE);
                    

                }
             
        }

        private string getIBBReqDuration(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            string duration = "";
            string mult = Math.Abs(myTSSet.IntEnd).ToString();
            duration += mult;
            switch (myTSSet.Period)
            {
                case KaiTrade.Interfaces.TSPeriod.Day:
                    duration += " D";
                    break;
                case KaiTrade.Interfaces.TSPeriod.Week:
                    duration += " W";
                    break;
                case KaiTrade.Interfaces.TSPeriod.Year:
                    duration += " Y";
                    break;
                case KaiTrade.Interfaces.TSPeriod.IntraDay:
                    mult = Math.Abs(myTSSet.IntEnd*60).ToString();
                    duration = mult.ToString()+" S";
                    //duration ="3600 S";
                    break;
                case KaiTrade.Interfaces.TSPeriod.Month:
                    duration += " M";
                    break;
                default:
                    duration += " Y";
                    break;
            }
            return duration;

        }

        private string getIBDuration(KaiTrade.Interfaces.TSPeriod period, int count, TimeSpan? reqSpan)
        {

            string duration="";
            duration = count.ToString();

            switch (period)
            {
                case KaiTrade.Interfaces.TSPeriod.Day:
                    if (reqSpan.HasValue)
                    {
                        duration = ((int)reqSpan.Value.TotalDays).ToString();
                    }
                    duration += " D";
                    break;
                case KaiTrade.Interfaces.TSPeriod.Week:
                    if (reqSpan.HasValue)
                    {
                        duration = ((int)reqSpan.Value.TotalDays/7).ToString();
                    }
                    duration += " W";
                    break;
                case KaiTrade.Interfaces.TSPeriod.Year:
                    duration = count.ToString();
                    duration += " Y";
                    break;
                case KaiTrade.Interfaces.TSPeriod.Month:
                    duration = count.ToString();
                    duration += " M";
                    break;
                default:
                    if (reqSpan.HasValue)
                    {
                        duration = ((int)reqSpan.Value.TotalSeconds).ToString();
                    }
                    duration += " S";
                    break;
            }
            return duration;

        }

        private void getIBBReqBarSize(KaiTrade.Interfaces.ITSSet myTSSet, out string barSize, out string duration)
        {
            barSize = "";

            duration = "";

             
            TimeSpan? reqDuration = null;
            switch (myTSSet.Period)
            {
                case KaiTrade.Interfaces.TSPeriod.Day:
                    barSize = "1 day";
                    reqDuration = new TimeSpan(Math.Abs(myTSSet.IntEnd), 0, 0, 0);
                    break;
                case KaiTrade.Interfaces.TSPeriod.Week:
                    barSize = "1 week";
                    reqDuration = new TimeSpan(Math.Abs(myTSSet.IntEnd)*7, 0, 0, 0);
                    break;
                case KaiTrade.Interfaces.TSPeriod.Year:
                    barSize = "1 year";
                    break;
                case KaiTrade.Interfaces.TSPeriod.hour:
                    barSize = "1 hour";
                    reqDuration = new TimeSpan(Math.Abs(myTSSet.IntEnd), 0, 0);
                    break;
                case KaiTrade.Interfaces.TSPeriod.minute:
                    barSize = "1 min";

                    reqDuration = new TimeSpan(0,Math.Abs(myTSSet.IntEnd), 0);
                    break;
                case KaiTrade.Interfaces.TSPeriod.two_minute:
                    barSize = "2 mins";
                    reqDuration = new TimeSpan(0, Math.Abs(myTSSet.IntEnd)*2, 0);

                    break;
                case KaiTrade.Interfaces.TSPeriod.three_minute:
                    barSize = "3 mins";
                    reqDuration = new TimeSpan(0, Math.Abs(myTSSet.IntEnd) * 3, 0);

                    break;
                case KaiTrade.Interfaces.TSPeriod.five_minute:
                    barSize = "5 mins";
                    reqDuration = new TimeSpan(0, Math.Abs(myTSSet.IntEnd) * 5, 0);

                    break;
                case KaiTrade.Interfaces.TSPeriod.fifteen_minute:
                    barSize = "15 mins";
                    reqDuration = new TimeSpan(0, Math.Abs(myTSSet.IntEnd) * 15, 0);
 
                    break;
                case KaiTrade.Interfaces.TSPeriod.thirty_minute:
                    barSize = "30 mins";
                    reqDuration = new TimeSpan(0, Math.Abs(myTSSet.IntEnd) * 30, 0);
 
                    break;
                case KaiTrade.Interfaces.TSPeriod.IntraDay:
                    reqDuration = new TimeSpan(0, Math.Abs(myTSSet.IntEnd) , 0);
                    barSize = "1 min";
                    break;
                case KaiTrade.Interfaces.TSPeriod.Month:
                    barSize = "1 month";
                    break;
                default:
                    barSize = "";
                    break;
            }
            
            // Now work out duration
            // if myTSSet.IntEnd
            
            duration="";
            if (myTSSet.IntEnd == 0)
            {
                // do all from start to now
                DateTime reqEnd = DateTime.Now;
                reqDuration = new TimeSpan(reqEnd.Ticks - myTSSet.DateTimeStart.Ticks);
                duration=getIBDuration(myTSSet.Period, myTSSet.IntEnd, reqDuration);
                
            }
            else if (myTSSet.IntEnd < 0)
            {
                // get the last N bars
                duration=getIBDuration(myTSSet.Period, myTSSet.IntEnd, reqDuration);
            }
            else
            {
                // get the number they ask for
                duration = getIBDuration(myTSSet.Period, myTSSet.IntEnd, reqDuration);
            }
        }


        private void getBarData(KaiTrade.Interfaces.ITSSet myTSSet)
        {

            try
            {
 
                // get the product
                KaiTrade.Interfaces.TradableProduct product = m_Facade.Factory.GetProductManager().GetProductMnemonic(myTSSet.Mnemonic);

                // IB's newest API uses its IContract  
                // Note the PXPublisher must have a valid product
                if (product == null)
                {
                    throw new Exception("TSData set does not have a valid product");
                }

              
               
                // register the pub against the IB ReqID - updates pass that back

                m_IBReqIDTSSet.Add(++m_NextID, myTSSet);
                

                TWSLib.IContract myContract = getIBContract(product);

                myContract.multiplier = "0";

                // m_Host.TWS.reqMktDataEx(m_NextID, myContract, m_QuoteFields, 0);
                string endTime =myTSSet.DateTimeEnd.ToString("yyyyMMdd HH:mm:ss");
                string barSize ="";
                string duration ="";
                getIBBReqBarSize(myTSSet, out barSize, out duration);
                //string duration = getIBBReqDuration(myTSSet);
                //string barSize = getIBBReqBarSize(myTSSet);
                string headers = "TRADES";
                if (product.CFICode == KaiTrade.Interfaces.CFICode.FX)
                {

                    headers = "MIDPOINT";
                }
                
                //"1 M","1 day"
                //m_Host.TWS.reqHistoricalDataEx(m_NextID, myContract, endTime, "1 M", "1 day", headers, 0, 2);
                m_Host.TWS.reqHistoricalDataEx(m_NextID, myContract, endTime, duration, barSize, headers, 0, 1);
                if (myTSSet.UpdatesEnabled)
                {
                    m_IBReqIDTSSet.Add(++m_NextID, myTSSet);
                    m_Host.TWS.reqRealTimeBarsEx(m_NextID, myContract, 5, headers, 0);
                }
            }
            catch (Exception myE)
            {
                _log.Error("getBarData", myE);


            }

        }


        protected override void DoSend(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                // use the form to invoke the send back onto doSend() above
                m_Host.Send(myMsg);

            }
            catch (Exception myE)
            {
                _log.Error("DoSend", myE);
            }
        }

       

    }
}
