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
using K2ServiceInterface;
using Newtonsoft.Json;
using K2DataObjects;

namespace OrderServices
{
    /// <summary>
    /// Perform basic order operations submit, execution report, cancel and Replace
    /// </summary>
    public class OrderService
    {
        /// <summary>
        /// Singleton instance of the MainMessageHandler class
        /// </summary>
        private static volatile OrderService s_instance;

        /// <summary>
        /// Object we use to lock during singleton instantiation.
        /// </summary>
        private static object s_Token = new object();

        private delegate void SendDelegate(KaiTrade.Interfaces.IMessage myMsg);
        //NOT USED? SendDelegate m_SendDelegate;

        /// <summary>
        /// Logger
        /// </summary>
        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");
        public log4net.ILog m_ORLog = log4net.LogManager.GetLogger("ORTalkTalk");

        protected OrderService()
        {
            m_Log.Info("OrderService Created");
            m_ORLog.Info("OrderService Created"); 
        }

        public static OrderService Instance()
        {
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new OrderService();
                    }
                }
            }
            return s_instance;
        }

        /// <summary>
        /// Invert the side passed in
        /// </summary>
        /// <param name="mySide"></param>
        /// <returns></returns>
        public string InvertSide(string mySide)
        {
            string myRet = "";
            switch (mySide.ToUpper())
            {
                case KaiTrade.Interfaces.Side.BUY:
                    myRet = KaiTrade.Interfaces.Side.SELL;
                    break;
                case KaiTrade.Interfaces.Side.SELL:
                    myRet = KaiTrade.Interfaces.Side.BUY;
                    break;
                default:
                    myRet = mySide;
                    break;
            }
            return myRet;
        }

        /// <summary>
        /// Handle a reject message from some driver
        /// </summary>
        /// <param name="myMsg"></param>
        public void HandleReject(KaiTrade.Interfaces.IMessage myMsg)
        {
            ITransaction myTrn = null;
            try
            {
                m_Log.Info("HandleReject");
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("HandleReject:"+myMsg.Data);
                }
                K2DataObjects.Fill fill = JsonConvert.DeserializeObject<K2DataObjects.Fill>(myMsg.Data);

              

                // get the order
                KaiTrade.Interfaces.IOrder order = AppFactory.Instance().GetOrderManager().GetOrderWithClOrdIDID(fill.ClOrdID);

                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("HandleReject:" +order.ToString());
                }


                order.OrdStatus = KaiTrade.Interfaces.OrderStatus.REJECTED;
                order.Text = "Reject received:" + fill.Text;
                if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.cancel)
                {
                    order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    m_ORLog.Info("HandleReject: cancel reset to none");
                } 
                else if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.replace)
                {
                    order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    m_ORLog.Info("HandleReject: replace reset to none");
                }

                if (myTrn != null)
                {
                    myTrn.StartUpdate();
                }

                // Check if the order is part of a strategy
                if (myOrder.ParentIdentity != null)
                {
                    // get the strategy concerned
                    KaiTrade.Interfaces.Strategy myStrat = KTAFacade.Instance().Factory.GetStrategyManager().GetStrategy(myOrder.ParentIdentity);
                    if (myStrat != null)
                    {
                        myStrat.HandleReject(myReject, myOrder);
                    }
                    else
                    {
                        m_Log.Error("Strategy not found for:" + myOrder.ParentIdentity);
                    }
                }
            }
            catch (Exception myE)
            {
                if (myTrn != null)
                {
                    myTrn.StartUpdate();
                }
                m_Log.Error("HandleReject", myE);
            }
        }

        /// <summary>
        /// Handle an incomming execution report
        /// </summary>
        /// <param name="myMsg"></param>
        public void HandleExecReport(KaiTrade.Interfaces.Message myMsg)
        {
            try
            {
                if (m_Log.IsInfoEnabled)
                {
                    m_Log.Info("OrderService:HandleExecReport:" + myMsg.Data);
                }
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("OrderService:HandleExecReport:" + myMsg.Data);
                }
                // Get order manager
                KaiTrade.Interfaces.OrderManager myOM = KTAFacade.Instance().Factory.GetOrderManager();

                //Create a quickfix message object
                QuickFix.Message myFixMsg = new QuickFix.Message(myMsg.Data);

                QuickFix.ExecID myExecID = new QuickFix.ExecID();
                myFixMsg.getField(myExecID);

                // check if we have already processed this report
                if (!myOM.RecordExecutionReport(myExecID.getValue(), myFixMsg))
                {
                    // we have processed this already
                    m_Log.Warn("HandleExecReport: Duplicate Exec Report:" + myMsg.Data);
                    return;
                }

                // these fields must be present
                QuickFix.OrderID myOrderID = new QuickFix.OrderID();
                myFixMsg.getField(myOrderID);

                QuickFix.ExecType myExecType = new QuickFix.ExecType();
                myFixMsg.getField(myExecType);

                QuickFix.OrdStatus myOrdStatus = new QuickFix.OrdStatus();
                myFixMsg.getField(myOrdStatus);

                // we need the clordid to update an existing order - if
                // it is not present or unknow we create a synthetic order
                QuickFix.ClOrdID myClOrdID = new QuickFix.ClOrdID();
                KaiTrade.Interfaces.Order myOrder = null;
                if (myFixMsg.isSetField(myClOrdID))
                {
                    myFixMsg.getField(myClOrdID);
                    myOrder = myOM.GetOrderWithClOrdIDID(myClOrdID.getValue());
                }
                if (myOrder == null)
                {
                    myOrder = CreateSyntheticOrder(myFixMsg);
                }

                // add the ExecRep to the Order
                KaiTrade.Interfaces.Fill myFill = new K2DataObjects.FillData();
                myFill.OrderID = myOrder.Identity;
                myFill.SetUpFromFixExecReport(myFixMsg.ToString());
         
                myOrder.FillsList.Add(myFill);

                // update mandatory fields
                myOrder.OrderID = myOrderID;
                myOrder.OrdStatus = myOrdStatus;
                QuickFix.Text myText = new QuickFix.Text();
                if (myFixMsg.isSetField(myText))
                {
                    myFixMsg.getField(myText);
                    myOrder.Text = myText.getValue();
                }

                QuickFix.Account myAccount = new QuickFix.Account();
                if (myFixMsg.isSetField(myAccount))
                {
                    myFixMsg.getField(myAccount);

                    myOrder.Account = myAccount;
                }

                // process the execution depanding on type of ExecReport
                switch (myExecType.getValue())
                {
                    case QuickFix.ExecType.NEW:
                        processGeneralExecRep(ref myOrder, myFixMsg);
                        /*
                        if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.neworder)
                        {
                            order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                        }
                        else
                        {
                            m_Log.Error("HandleExecReport:OrderStatus new not expected");
                        }
                         * */
                        break;
                    case QuickFix.ExecType.FILL:
                        if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.neworder)
                        {
                            order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                        }

                        processFill(ref myOrder, myFixMsg);
                        break;
                    case QuickFix.ExecType.PARTIAL_FILL:
                        if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.neworder)
                        {
                            order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                        }
                        processFill(ref myOrder, myFixMsg);
                        break;
                    case QuickFix.ExecType.ORDER_STATUS:
                        processGeneralExecRep(ref myOrder, myFixMsg);
                        break;
                    default:
                        processGeneralExecRep(ref myOrder, myFixMsg);
                        break;
                }

                myOM.SetChanged(myOrder.Identity);

                // Check if the order is part of a strategy
                if (myOrder.ParentIdentity != null)
                {
                    // get the strategy concerned
                    KaiTrade.Interfaces.Strategy myStrat = KTAFacade.Instance().Factory.GetStrategyManager().GetStrategy(myOrder.ParentIdentity);
                    if (myStrat != null)
                    {
                        myStrat.HandleExecReport(myFixMsg, myOrder);
                    }
                    else
                    {
                        m_Log.Error("Strategy not found for:" + myOrder.ParentIdentity);
                    }
                }

                if (myOrder.OCAGroupName.Length > 0)
                {
                    KTAFacade.Instance().Factory.GetOCOManager().OnOrderTraded(myOrder);
                }
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("OrderService:HandleExecRep:Exit:" + myOrder.ToString());
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("HandleExecReport", myE);
            }
        }

        /// <summary>
        /// Process partial or fully filled
        /// </summary>
        /// <param name="myOrder"></param>
        /// <param name="myExec"></param>
        private void processFill(ref KaiTrade.Interfaces.Order myOrder, QuickFix.Message myExec)
        {
            KaiTrade.Interfaces.Transaction myTrn = null;
            try
            {
                // if the object supports transactions do the update as a trn
                myTrn = myOrder as KaiTrade.Interfaces.Transaction;
                if (myTrn != null)
                {
                    myTrn.StartUpdate();
                }
                // Get the currently executed quantity for chain of orders.
                QuickFix.CumQty myCumQty = new QuickFix.CumQty();
                myExec.getField(myCumQty);
                myOrder.CumQty = myCumQty;

                // Get quantity open for further execution (order qty - cum qty)
                QuickFix.LeavesQty myLeavesQty = new QuickFix.LeavesQty();
                myExec.getField(myLeavesQty);
                myOrder.LeavesQty = myLeavesQty;

                QuickFix.LastPx myLastPx = new QuickFix.LastPx();
                if (myExec.isSetField(myLastPx))
                {
                    myExec.getField(myLastPx);
                    myOrder.LastPx = myLastPx;
                }

                QuickFix.AvgPx myAvgPx = new QuickFix.AvgPx();
                if (myExec.isSetField(myAvgPx))
                {
                    myExec.getField(myAvgPx);
                    myOrder.AvgPx = myAvgPx;
                }

                QuickFix.LastQty myLastQty = new QuickFix.LastQty();
                if (myExec.isSetField(myLastQty))
                {
                    myExec.getField(myLastQty);
                    myOrder.LastQty = myLastQty;
                }

                QuickFix.OrdStatus myOrdStatus = new QuickFix.OrdStatus();
                myExec.getField(myOrdStatus);
                myOrder.OrdStatus = myOrdStatus;
                // always signal the end of the update
                if (myTrn != null)
                {
                    myTrn.EndUpdate();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("processFill", myE);
                // always signal the end of the update
                if (myTrn != null)
                {
                    myTrn.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Process general purpose exec i.e. some type of order status change
        /// </summary>
        /// <param name="myOrder"></param>
        /// <param name="myExec"></param>
        private void processGeneralExecRep(ref KaiTrade.Interfaces.Order myOrder, QuickFix.Message myExec)
        {
            if (m_ORLog.IsInfoEnabled)
            {
                m_ORLog.Info("processGeneralExecRep:");
            }
            QuickFix.OrdStatus myOrdStatus = new QuickFix.OrdStatus();
            QuickFix.LeavesQty myLeavesQty = new QuickFix.LeavesQty();
            QuickFix.CumQty myCumQty = new QuickFix.CumQty();
            myExec.getField(myOrdStatus);
            myOrder.OrdStatus = myOrdStatus;

            switch (myOrdStatus.getValue())
            {
                case QuickFix.OrdStatus.CANCELED:
                    if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.cancel)
                    {
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    }

                    myLeavesQty = new QuickFix.LeavesQty(0.0);
                    myOrder.LeavesQty = myLeavesQty;
                    break;
                case QuickFix.OrdStatus.NEW:

                    if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.neworder)
                    {
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    }
                    else if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.replace)
                    {
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    }
                    else
                    {
                        m_Log.Error("HandleExecReport:OrderStatus new not expected");
                    }
                    myOrder.LeavesQty  = new QuickFix.LeavesQty(myOrder.OrderQty.getValue());
                    myOrder.CumQty = new QuickFix.CumQty(0.0);
                    break;
                case QuickFix.OrdStatus.REPLACED:
                    if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.replace)
                    {
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    }
                    if (myExec.isSetField(myLeavesQty))
                    {
                        myExec.getField(myLeavesQty);
                        myOrder.LeavesQty = myLeavesQty;
                    }
                    if (myExec.isSetField(myCumQty))
                    {
                        myExec.getField(myCumQty);
                        myOrder.CumQty = myCumQty;
                    }

                    break;
                case QuickFix.OrdStatus.PENDING_REPLACE:
                    if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.replace)
                    {
                        m_ORLog.Error("Pending replace received but last command is not replace");
                    }
                    
                    break;
                case QuickFix.OrdStatus.REJECTED:
                    if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.cancel)
                    {
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    }
                    else if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.neworder)
                    {
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    }
                    else if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.replace)
                    {
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Create a synthetic order
        /// </summary>
        /// <param name="myMsg"></param>
        /// <returns>n order</returns>
        public KaiTrade.Interfaces.Order CreateSyntheticOrder(QuickFix.Message myFixMsg)
        {
            KaiTrade.Interfaces.Order myOrder = null;
            try
            {
                if (m_Log.IsInfoEnabled)
                {
                    m_Log.Info("exec report recd:" + myFixMsg.ToString());
                }

                // Get order manager
                KaiTrade.Interfaces.OrderManager myOM = KTAFacade.Instance().Factory.GetOrderManager();

                // these fields must be present
                QuickFix.OrderID myOrderID = new QuickFix.OrderID();
                myFixMsg.getField(myOrderID);

                QuickFix.ExecType myExecType = new QuickFix.ExecType();
                myFixMsg.getField(myExecType);

                QuickFix.ExecID myExecID = new QuickFix.ExecID();
                myFixMsg.getField(myExecID);

                QuickFix.OrdStatus myOrdStatus = new QuickFix.OrdStatus();
                myFixMsg.getField(myOrdStatus);

                // Create the order
                myOrder = myOM.CreateOrder();

                // we need the clordid to update an existing order - if
                // it is not present or unknow we create a synthetic order
                QuickFix.ClOrdID myClOrdID = new QuickFix.ClOrdID();
                if (myFixMsg.isSetField(myClOrdID))
                {
                    myOrder.ClOrdID = myClOrdID;
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("CreateSyntheticOrder", myE);
            }
            return myOrder;
        }

        /// <summary>
        /// submit a complete order for processing, note the order must have a
        /// valid product and trade venue
        /// </summary>
        /// <param name="myOrder"></param>
        public void SubmitOrder(KaiTrade.Interfaces.Order myOrder)
        {
            try
            {
                lock (this)
                {
                    QuickFix.Message myNOS;
                    KaiTrade.Interfaces.Message myMsg;
                    string myDriverCode;
                    RenderOrderAsFix(out  myNOS, out  myMsg, out myDriverCode, myOrder);

                    try
                    {
                        if (myOrder.OCAGroupName.Length > 0)
                        {
                            KTAFacade.Instance().Factory.GetOCOManager().AddOrderOCO(myOrder, myOrder.OCAGroupName);
                        }
                        //Send the message
                        KaiTrade.Interfaces.Driver myDrv = AppFactory.Instance().GetDriverManager().GetDriver(myDriverCode);
                        if (myDrv != null)
                        {
                            // send the message for processing
                            order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.neworder;

                            //SendDelegate mySend = new SendDelegate(myDrv.Send);
                            //IAsyncResult ar = mySend.BeginInvoke(myMsg, SRCallback, "123456789");
                            myDrv.Send(myMsg);
                        }
                        else
                        {
                            string myError = "Driver not found for code:" + myDriverCode;
                            m_Log.Error(myError);
                            Exception myE = new Exception(myError);
                            throw myE;
                        }
                        if (m_ORLog.IsInfoEnabled)
                        {
                            m_ORLog.Info("SubmitOrder:" + myOrder.ToString());
                        }
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("SubmitOrder:" + myOrder.Identity, myE);
                        myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                        throw (myE);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("SubmitOrder:", myE);
                myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                myOrder.Text = myE.Message;
                // inform order manager clients that the order has changed
                AppFactory.Instance().GetOrderManager().SetChanged(myOrder.Identity);
                throw (myE);
            }
        }

        public void RenderOrderAsFix(out QuickFix.Message myNOS, out KaiTrade.Interfaces.Message myMsg, out string myDriverCode, KaiTrade.Interfaces.Order myOrder)
        {
            try
            {
                lock (this)
                {
                    // do not actually submit triggered orders

                    // register the order as a publisher
                    KaiTrade.Interfaces.Publisher myPub = null;
                    myPub = AppFactory.Instance().GetPublisherManager().GetPublisher(myOrder.Identity);
                    if (myPub == null)
                    {
                        myPub = myOrder as KaiTrade.Interfaces.Publisher;
                        AppFactory.Instance().GetPublisherManager().Add(myPub);
                    }

                    // Check the limits for the order
                    string myTextErr;
                    if (LimitChecker.Instance().BreaksLimits(out myTextErr, myOrder))
                    {
                        myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                        myOrder.Text = "order failed order qty limit test";
                        Exception myE = new Exception("process order failed order qty limit test");
                        throw myE;
                    }

                    // Create the FIX order message
                    myNOS = new QuickFix.Message();

                    // pending new at this stage
                    myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.PENDING_NEW);

                    myOrder.TransactTime = new QuickFix.TransactTime(DateTime.Now.ToUniversalTime());
                    myNOS.setField(myOrder.TransactTime);

                    //myOrder.ClOrdID = new QuickFix.ClOrdID(KaiUtil.Identities.Instance.genReqID());

                    // Associate the CLOrdID with this order
                    AppFactory.Instance().GetOrderManager().AssociateClOrdID(myOrder.ClOrdID.getValue(), myOrder);

                    // Set fix version
                    QuickFix.BeginString myBeginString = new QuickFix.BeginString("FIX.4.4");
                    myNOS.getHeader().setField(myBeginString);

                    // Set order message type
                    QuickFix.MsgType msgType = new QuickFix.MsgType();
                    msgType.setValue("D");
                    myNOS.getHeader().setField(msgType);

                    myNOS.setField(myOrder.ClOrdID);

                    // set up the product details in the message
                    myOrder.Product.Set(myNOS);

                    // Qty
                    myNOS.setField(myOrder.OrderQty);

                    // MaxFloor
                    if (myOrder.MaxFloor != null)
                    {
                        myNOS.setField(myOrder.MaxFloor);
                    }

                    // set the side
                    myNOS.setField(myOrder.Side);

                    // set the order type
                    myNOS.setField(myOrder.OrdType);

                    // Time in force
                    if (myOrder.TimeInForce != null)
                    {
                        myNOS.setField(myOrder.TimeInForce);
                    }
                    else
                    {
                        myOrder.TimeInForce = new QuickFix.TimeInForce(QuickFix.TimeInForce.DAY);
                        myNOS.setField(myOrder.TimeInForce);
                    }

                    // Validation - check they entered a date/time if needed
                    switch (myOrder.TimeInForce.getValue())
                    {
                        case QuickFix.TimeInForce.GOOD_TILL_DATE:
                            break;

                        default:
                            break;
                    }

                    // do not add the price to the fix message on Market orders
                    if (myOrder.OrdType.getValue() != QuickFix.OrdType.MARKET)
                    {
                        if (myOrder.Price.getValue() > 0)
                        {
                            myNOS.setField(myOrder.Price);
                        }
                        else
                        {
                            throw new Exception("Must specify price > 0 on Limit and other non Market orders");
                        }
                    }

                    if (myOrder.StopPx != null)
                    {
                        myNOS.setField(myOrder.StopPx);
                    }

                    // default the handlInst to automated
                    if (myOrder.HandlInst != null)
                    {
                        myNOS.setField(myOrder.HandlInst);
                    }
                    else
                    {
                        myOrder.HandlInst = new QuickFix.HandlInst(QuickFix.HandlInst.AUTOEXECPUB);
                        myNOS.setField(myOrder.HandlInst);
                    }

                    // Create the message wrapper used to send the order to the driver
                    myMsg = new KaiTCPComm.KaiMessageWrap();

                    myMsg.Label = "D";

                    myDriverCode = "";
                    // A trade venue must be specified on the order
                    KaiTrade.Interfaces.Venue myVenue = null;
                    if (myOrder.Product.TradeVenue.Length > 0)
                    {
                        // get the driver code and session details from the venue manager
                        myVenue = AppFactory.Instance().GetVenueManager().GetVenue(myOrder.Product.TradeVenue);
                        if (myVenue != null)
                        {
                            myDriverCode = myVenue.DriverCode;
                            if (myVenue.TargetVenue.Length > 0)
                            {
                                myMsg.VenueCode = myVenue.TargetVenue;
                            }
                            if (myVenue.BeginString.Length > 0)
                            {
                                // this is the FIX version the server wants, we convert between
                                // versions in the FIX client
                                myMsg.Format = myVenue.BeginString;
                            }

                            if (myVenue.SID.Length > 0)
                            {
                                QuickFix.SenderCompID mySID = new QuickFix.SenderCompID(myVenue.SID);
                                myNOS.getHeader().setField(mySID);
                                myMsg.ClientID = myVenue.SID;
                            }
                            if (myVenue.TID.Length > 0)
                            {
                                QuickFix.TargetCompID myTID = new QuickFix.TargetCompID(myVenue.TID);
                                myNOS.getHeader().setField(myTID);
                                myMsg.TargetID = myVenue.TID;
                            }
                        }
                        else
                        {
                            string myErr = "Invalid Venue Code:" + myOrder.Product.TradeVenue;
                            m_Log.Error(myErr);
                            Exception myE = new Exception(myErr);
                            throw myE;
                        }
                    }
                    else
                    {
                        string myErr = "No Venue Code specified on product:";
                        m_Log.Error(myErr);
                        Exception myE = new Exception(myErr);
                        throw myE;
                    }

                    // process any venue field bags - do this prior to the order bags
                    // to allow the order setting to override the venues
                    List<KaiTrade.Interfaces.Field> myFieldBag;
                    KTAFacade.Instance().SetBag(out myFieldBag, myVenue.NOSBag, ",");
                    foreach (KaiTrade.Interfaces.Field myField in myFieldBag)
                    {
                        try
                        {
                            myNOS.setString(int.Parse(myField.ID), myField.Value);
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("NOS Extra tags from venue error:", myE);
                        }
                    }

                    // process any additional tags for the order
                    foreach (KaiTrade.Interfaces.Field myField in myOrder.NOSBag)
                    {
                        try
                        {
                            myNOS.setString(int.Parse(myField.ID), myField.Value);
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("NOS Extra tags error:", myE);
                        }
                    }

                    // additional setting placed here will override the
                    // fields in the bags

                    if (myOrder.Account != null)
                    {
                        if (myOrder.Account.getValue().Length > 0)
                        {
                            myNOS.setField(myOrder.Account);
                        }
                    }

                    QuickFix.TargetStrategyParameters targetStrategyParameters = new QuickFix.TargetStrategyParameters(myOrder.StrategyName);
                    myNOS.setField(targetStrategyParameters); 

                    // Process any strategy parameters
                    if (myOrder.K2Parameters.Count > 0)
                    {
                        QuickFix.NoStrategyParameters noStratParms = new QuickFix.NoStrategyParameters(myOrder.K2Parameters.Count);
                        myNOS.setField(noStratParms);
                        QuickFix50Sp2.NewOrderSingle.NoStrategyParameters group = new QuickFix50Sp2.NewOrderSingle.NoStrategyParameters();
                        QuickFix.StrategyParameterName strategyParameterName;
                        QuickFix.StrategyParameterType strategyParameterType;
                        QuickFix.StrategyParameterValue strategyParameterValue;

                        foreach (KaiTrade.Interfaces.K2Parameter param in myOrder.K2Parameters)
                        {
                            strategyParameterName = new QuickFix.StrategyParameterName(param.ParameterName);
                            group.setField(strategyParameterName);
                            strategyParameterType = new QuickFix.StrategyParameterType((int)param.ParameterType);
                            group.setField(strategyParameterType);
                            strategyParameterValue = new QuickFix.StrategyParameterValue(param.ParameterValue);
                            group.setField(strategyParameterValue);
                            myNOS.addGroup(group);
                        }
                    }

                    // Set the FIX string as message data
                    myMsg.Data = myNOS.ToString();

                    // inform order manager clients that the order has changed
                    AppFactory.Instance().GetOrderManager().SetChanged(myOrder.Identity);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("RenderOrderAsFix:", myE);
                myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                myOrder.Text = myE.Message;
                // inform order manager clients that the order has changed
                AppFactory.Instance().GetOrderManager().SetChanged(myOrder.Identity);
                throw (myE);
            }
        }

        private void SRCallback(IAsyncResult ar)
        {
        }

        /// <summary>
        /// Submit an order based on some orderdata object
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string SubmitOrder(KaiTrade.Interfaces.OrderData order)
        {
            lock (this)
            {
                KaiTrade.Interfaces.Strategy myStrategy = AppFactory.Instance().GetStrategyManager().GetStrategy(order.ParentIdentity);
                if (!myStrategy.Enabled)
                {
                    m_Log.Error("Strategy is not enabled for trading");
                    throw new Exception("Strategy is not enabled for trading");
                }

                KaiTrade.Interfaces.Order myOrder = AppFactory.Instance().GetOrderManager().CreateOrder();
                if (order.ClientAssignedID.Length > 0)
                {
                    myOrder.ClientAssignedID = order.ClientAssignedID;
                }
                else
                {
                    myOrder.ClientAssignedID = order.Identity;
                }
                
                myOrder.ParentIdentity = order.ParentIdentity;
                if (myStrategy.SessionID != null)
                {
                    myOrder.SessionID = myStrategy.SessionID;
                }

                if (myStrategy.User != null)
                {
                    myOrder.User = myStrategy.User;
                }

                if (myStrategy.CorrelationID != null)
                {
                    myOrder.CorrelationID = myStrategy.CorrelationID;
                }

                KTAFacade.Instance().Factory.GetPublisherManager().Add(myOrder as KaiTrade.Interfaces.Publisher);

                KaiTrade.Interfaces.TradableProduct myProduct = AppFactory.Instance().GetProductManager().GetProductMnemonic(order.Mnemonic);

                myOrder.Product = myProduct;
                myOrder.ClOrdID = new QuickFix.ClOrdID(KaiUtil.Identities.Instance.genOrderID().ToString());
                myOrder.OrderQty = new QuickFix.OrderQty(order.OrderQty);
                if (order.MaxFloor > 0)
                {
                    myOrder.MaxFloor = new QuickFix.MaxFloor(order.MaxFloor);
                }
                myOrder.Side = KaiUtil.QFUtils.EncodeSide(order.Side);
                myOrder.OrdType = KaiUtil.QFUtils.EncodeOrderType(order.OrdType);
                myOrder.TimeInForce = KaiUtil.QFUtils.EncodeTimeType(order.TimeInForce);

                //set the account if passed
                if (order.Account.Length > 0)
                {
                    myOrder.Account = new QuickFix.Account(order.Account);
                }

                myOrder.ExtendedOrdType = order.ExtendedOrdType;

                // If its not a market order - then use the price
                if (!(myOrder.OrdType.getValue() == QuickFix.OrdType.MARKET))
                {
                    myOrder.Price = new QuickFix.Price(order.Price);
                }

                // do order type specific processing
                if (myOrder.OrdType.getValue() == QuickFix.OrdType.STOP)
                {
                    myOrder.StopPx = new QuickFix.StopPx(order.StopPx);
                }
                else if (myOrder.OrdType.getValue() == QuickFix.OrdType.STOP_LIMIT)
                {
                    myOrder.StopPx = new QuickFix.StopPx(order.StopPx);
                }
                else if (myOrder.OrdType.getValue() == KaiTrade.Interfaces.FIXOrderType.TARGETLIMIT)
                {
                    myOrder.StopPx = new QuickFix.StopPx(order.StopPx);
                }
                else if (myOrder.OrdType.getValue() == KaiTrade.Interfaces.FIXOrderType.TARGET)
                {
                    myOrder.StopPx = new QuickFix.StopPx(order.StopPx);
                }
                else if (order.ExtendedOrdType.Length > 0)
                {
                    //this is some sort of extended order type
                    myOrder.StopPx = new QuickFix.StopPx(order.StopPx);
                    myOrder.Price = new QuickFix.Price(order.StopPx);
                    KTAFacade.Instance().AddTriggerOrder(myOrder, order.ExtendedOrdType, order.OrdType, order.K2Parameters);
                    return myOrder.Identity;
                }
                else
                {
                    // no action
                    m_Log.Error("unknown order type");
                }

                this.SubmitOrder(myOrder);
                return myOrder.Identity;
            }
        }

        /// <summary>
        /// Submit a simple order
        /// </summary>
        /// <param name="myStrategyID">Identity of strategy</param>
        /// <param name="myMnemonic"></param>
        /// <param name="myQty"></param>
        /// <param name="myPrice"></param>
        /// <param name="mySide"></param>
        /// <param name="myOrderType"></param>
        /// <returns></returns>
        public string SubmitOrder(string myStrategyID, string myMnemonic, double myQty, double myPrice, string mySide, string myOrderType, double myStopPx, string myTimeType, string myDateTime, string myAccount,string myShortSellLocate, double? maxFloor, string extendedOrderType, List<KaiTrade.Interfaces.K2Parameter> parms)
        {
            lock (this)
            {
                KaiTrade.Interfaces.Strategy myStrategy =  AppFactory.Instance().GetStrategyManager().GetStrategy(myStrategyID);
                if (!myStrategy.Enabled)
                {
                    m_Log.Error("Strategy is not enabled for trading");
                    throw new Exception("Strategy is not enabled for trading");
                }

                KaiTrade.Interfaces.Order myOrder = AppFactory.Instance().GetOrderManager().CreateOrder();

                myOrder.ParentIdentity = myStrategyID;
                if (myStrategy.SessionID != null)
                {
                    myOrder.SessionID = myStrategy.SessionID;
                }

                if (myStrategy.User != null)
                {
                    myOrder.User = myStrategy.User;
                }

                if (myStrategy.CorrelationID != null)
                {
                    myOrder.CorrelationID = myStrategy.CorrelationID;
                }

                KTAFacade.Instance().Factory.GetPublisherManager().Add(myOrder as KaiTrade.Interfaces.Publisher);

                KaiTrade.Interfaces.TradableProduct myProduct = AppFactory.Instance().GetProductManager().GetProductMnemonic(myMnemonic);

                myOrder.Product = myProduct;
                myOrder.ClOrdID = new QuickFix.ClOrdID(KaiUtil.Identities.Instance.genOrderID().ToString());
                myOrder.OrderQty = new QuickFix.OrderQty(myQty);
                if (maxFloor.HasValue)
                {
                    myOrder.MaxFloor = new QuickFix.MaxFloor(maxFloor.Value);
                }
                myOrder.Side = KaiUtil.QFUtils.EncodeSide(mySide);
                myOrder.OrdType = KaiUtil.QFUtils.EncodeOrderType(myOrderType);
                myOrder.TimeInForce = KaiUtil.QFUtils.EncodeTimeType(myTimeType);

                //set the account if passed
                if (myAccount.Length > 0)
                {
                    myOrder.Account = new QuickFix.Account(myAccount);
                }

                myOrder.ExtendedOrdType = extendedOrderType;

                // If its not a market order - then use the price
                if (!(myOrder.OrdType.getValue() == QuickFix.OrdType.MARKET))
                {
                    myOrder.Price = new QuickFix.Price(myPrice);
                }

                // do order type specific processing
                if (myOrder.OrdType.getValue() == QuickFix.OrdType.LIMIT)
                {
                    // no action 
                }
                else if (myOrder.OrdType.getValue() == QuickFix.OrdType.MARKET)
                {
                    // no action
                }
                else if (myOrder.OrdType.getValue() == QuickFix.OrdType.STOP)
                {
                    myOrder.StopPx = new QuickFix.StopPx(myStopPx);
                }
                else if (myOrder.OrdType.getValue() == QuickFix.OrdType.STOP_LIMIT)
                {
                    myOrder.StopPx = new QuickFix.StopPx(myStopPx);
                }
                else if (myOrder.OrdType.getValue() == KaiTrade.Interfaces.FIXOrderType.TARGETLIMIT)
                {
                    myOrder.StopPx = new QuickFix.StopPx(myStopPx);
                }
                else if (myOrder.OrdType.getValue() == KaiTrade.Interfaces.FIXOrderType.TARGET)
                {
                    myOrder.StopPx = new QuickFix.StopPx(myStopPx);
                }
                else if (extendedOrderType.Length > 0)
                {
                    //this is some sort of extended order type
                    myOrder.StopPx = new QuickFix.StopPx(myStopPx);
                    myOrder.Price = new QuickFix.Price(myPrice);
                    KTAFacade.Instance().AddTriggerOrder(myOrder, extendedOrderType, myOrderType, parms);
                    return myOrder.Identity;
                }
                else
                {
                    // no action
                    m_Log.Error("unknown order type");
                }

                this.SubmitOrder(myOrder);
                return myOrder.Identity;
            }
        }

        /// <summary>
        /// Cancel a group of orders
        /// </summary>
        /// <param name="myGrp"></param>
        public void CancelOrder(KaiTrade.Interfaces.OrderGroup myGrp)
        {
            try
            {
                foreach (KaiTrade.Interfaces.Order myOrder in myGrp.Orders)
                {
                    this.CancelOrder(myOrder.Identity);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("CancelOrder:OrderGroup", myE);
            }
        }

        /// <summary>
        /// Cancel the order specified
        /// </summary>
        /// <param name="myID">ID of order to cancel</param>
        public void CancelOrder(string myID)
        {
            try
            {
                // Get Order
                KaiTrade.Interfaces.OrderManager myOM = AppFactory.Instance().GetOrderManager();
                KaiTrade.Interfaces.Order myOrder = myOM.GetOrder(myID);

                if (myOrder == null)
                {
                    string myError = "Order not found cannot cancel:" + myID;
                    Exception myE = new Exception(myError);
                }

                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("CancelOrder:" + myOrder.ToString());
                }

                if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.cancel)
                {
                    string myError = "Order has already had a cancel processed - cannot recancel:" + myID;
                    Exception myE = new Exception(myError);
                }

                // Is this a triggered order
                if (myOrder.TriggerOrderID.Length > 0)
                {
                    KaiTrade.Interfaces.TriggeredOrder triggeredOrder = KTAFacade.Instance().Factory.GetTriggeredOrderManager().Get(myOrder.TriggerOrderID);
                    triggeredOrder.Cancel();
                    return;
                }
                // store the clordID's
                //NOT USED? QuickFix.OrigClOrdID myOrigClOrdID;
                QuickFix.ClOrdID myClOrdID;

                if ((myOrder.OrdStatus.getValue() == QuickFix.OrdStatus.FILLED) || (myOrder.OrdStatus.getValue() == QuickFix.OrdStatus.CANCELED))
                {
                    // cannot cancel the order - so return
                    return;
                }

                // Create a FIX cancel messsage

                QuickFix.Message myFixMsg = new QuickFix.Message();
                QuickFix.MsgType msgType = new QuickFix.MsgType("F");
                myFixMsg.getHeader().setField(msgType);
                QuickFix.BeginString myBeginString = new QuickFix.BeginString("FIX.4.4");
                myFixMsg.getHeader().setField(myBeginString);

                // set the original to the existing clordid
                myOrder.OrigClOrdID = new QuickFix.OrigClOrdID(myOrder.ClOrdID.getValue());
                myFixMsg.setField(myOrder.OrigClOrdID);

                // save the existing clordid and set the new clordid
                myClOrdID = myOrder.ClOrdID;
                myOrder.ClOrdID = new QuickFix.ClOrdID(KaiUtil.Identities.Instance.genReqID());
                myFixMsg.setField(myOrder.ClOrdID);
                // Associate the CLOrdID with this order
                AppFactory.Instance().GetOrderManager().AssociateClOrdID(myOrder.ClOrdID.getValue(), myOrder);

                // OrderID
                if (myOrder.OrderID != null)
                {
                    myFixMsg.setField(myOrder.OrderID);
                }

                ////Side
                myFixMsg.setField(myOrder.Side);

                // set up the product details in the message
                myOrder.Product.Set(myFixMsg);

                //// Transact Time
                myOrder.TransactTime = new QuickFix.TransactTime(DateTime.Now.ToUniversalTime());
                myFixMsg.setField(myOrder.TransactTime);

                // Create the message wrapper used to send the order cancel to the driver
                KaiTrade.Interfaces.Message myMsg = new KaiTCPComm.KaiMessageWrap();

                myMsg.Label = "F";

                string myDriverCode = "";
                // A trade venue must be specified on the order
                KaiTrade.Interfaces.Venue myVenue = null;
                if (myOrder.Product.TradeVenue.Length > 0)
                {
                    // get the driver code and session details from the venue manager
                    myVenue = AppFactory.Instance().GetVenueManager().GetVenue(myOrder.Product.TradeVenue);
                    if (myVenue != null)
                    {
                        myDriverCode = myVenue.DriverCode;
                        if (myVenue.TargetVenue.Length > 0)
                        {
                            myMsg.VenueCode = myVenue.TargetVenue;
                        }
                        if (myVenue.BeginString.Length > 0)
                        {
                            // this is the FIX version the server wants, we convert between
                            // versions in the FIX client
                            myMsg.Format = myVenue.BeginString;
                        }

                        if (myVenue.SID.Length > 0)
                        {
                            QuickFix.SenderCompID mySID = new QuickFix.SenderCompID(myVenue.SID);
                            myFixMsg.getHeader().setField(mySID);
                            myMsg.ClientID = myVenue.SID;
                        }
                        if (myVenue.TID.Length > 0)
                        {
                            QuickFix.TargetCompID myTID = new QuickFix.TargetCompID(myVenue.TID);
                            myFixMsg.getHeader().setField(myTID);
                            myMsg.TargetID = myVenue.TID;
                        }
                    }
                    else
                    {
                        string myErr = "Invalid Venue Code:" + myOrder.Product.TradeVenue;
                        m_Log.Error(myErr);
                        Exception myE = new Exception(myErr);
                        throw myE;
                    }
                }
                else
                {
                    string myErr = "No Venue Code specified on product:";
                    m_Log.Error(myErr);
                    Exception myE = new Exception(myErr);
                    throw myE;
                }

                // process any venue field bags - do this prior to the order bags
                // to allow the order setting to override the venues
                List<KaiTrade.Interfaces.Field> myFieldBag;
                KTAFacade.Instance().SetBag(out myFieldBag, myVenue.CancelBag, ",");

                foreach (KaiTrade.Interfaces.Field myField in myFieldBag)
                {
                    try
                    {
                        myFixMsg.setString(int.Parse(myField.ID), myField.Value);
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("cancel Extra tags from venue error:", myE);
                    }
                }

                // process any additional tags for the order
                foreach (KaiTrade.Interfaces.Field myField in myOrder.CancelBag)
                {
                    try
                    {
                        myFixMsg.setString(int.Parse(myField.ID), myField.Value);
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("cancel Extra tags error:", myE);
                    }
                }
                if (myOrder.Account != null)
                {
                    if (myOrder.Account.getValue().Length > 0)
                    {
                        myFixMsg.setField(myOrder.Account);
                    }
                }
                // Set the FIX string as message data
                myMsg.Data = myFixMsg.ToString();

                // inform order manager clients that the order has changed
                myOM.SetChanged(myOrder.Identity);

                try
                {
                    //Send the message
                    KaiTrade.Interfaces.Driver myDrv = AppFactory.Instance().GetDriverManager().GetDriver(myDriverCode);
                    if (myDrv != null)
                    {
                        // send the message for processing
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.cancel;

                        //SendDelegate mySend = new SendDelegate(myDrv.Send);
                        //IAsyncResult ar = mySend.BeginInvoke(myMsg, SRCallback, "123456789");
                        // send the message for processing
                        myDrv.Send(myMsg);
                    }
                    else
                    {
                        string myError = "Driver not found for code:" + myDriverCode;
                        m_Log.Error(myError);
                        Exception myE = new Exception(myError);
                        throw myE;
                    }
                }
                catch (Exception myE)
                {
                    m_Log.Error("CanceltOrder:" + myOrder.Identity, myE);
                    myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                    order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;
                    throw (myE);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("CancelOrder:", myE);
                throw (myE);
            }
        }

        private bool newValueDifferentFromOld(KaiTrade.Interfaces.Order o, double? newQty, double? newPrice, double? newStopPx)
        {
            bool different = false;
            if (newQty.HasValue)
            {
                if (newQty.Value != o.OrderQty.getValue())
                {
                    different = true;
                    return different;
                }
            }
            if (newPrice.HasValue)
            {
                if (newPrice.Value != o.Price.getValue())
                {
                    different = true;
                    return different;
                }
            }
            if ((newStopPx.HasValue) && (o.StopPx!=null))
            {
                if (newStopPx.Value != o.StopPx.getValue())
                {
                    different = true;
                    return different;
                }
            }
            return different;
        }
        /// <summary>
        /// Edit the order for the ID specified - throws an exception if the order isnt working
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="newQty">new qty if specified</param>
        /// <param name="newPrice">new price if specified</param>
        public void ReplaceOrder(string myID, double? newQty, double? newPrice, double? newStopPx)
        {
            KaiTrade.Interfaces.Order myOrder = null; 
            try
            {
                // Get Order
                KaiTrade.Interfaces.OrderManager myOM = AppFactory.Instance().GetOrderManager();
                myOrder = myOM.GetOrder(myID);
                if (myOrder == null)
                {
                    string myError = "Order not found cannot edit:" + myID;
                    Exception myE = new Exception(myError);
                    throw myE;
                }
                if (m_ORLog.IsInfoEnabled)
                {
                    m_ORLog.Info("ReplaceOrder:"+ myOrder.ToString());
                }
                // are these changes?
                if (!newValueDifferentFromOld(myOrder, newQty, newPrice, newStopPx))
                {
                    string myError = "The new fields in the Replace request are the same as the existing fields" + myID;
                    Exception myE = new Exception(myError);
                    throw myE;
                }
                // Is this a triggered order
                if (myOrder.TriggerOrderID.Length > 0)
                {
                    if (newQty.HasValue)
                    {
                        myOrder.OrderQty = new QuickFix.OrderQty((double)newQty);
                    }

                    if (newPrice.HasValue)
                    {
                        myOrder.Price = new QuickFix.Price((double)newPrice);
                    }
                    if (newStopPx.HasValue)
                    {
                        myOrder.StopPx = new QuickFix.StopPx((double)newStopPx);
                    }
                    return;
                }
                // store the clordID's
                //NOT USED? QuickFix.OrigClOrdID myOrigClOrdID;
                QuickFix.ClOrdID myClOrdID;

                // check if we are pending cancel or pending mod
                if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.replace)
                {
                    // we are currently doing a replace - cannot do another
                    string myError = "Order is already processing a replace cannot modify at this time:" + myID;
                    Exception myE = new Exception(myError);
                    throw myE;
                }

                if (order.LastOrderCommand == KaiTrade.Interfaces.LastOrderCommand.cancel)
                {
                    // we are currently doing a cancel - cannot do a mod
                    string myError = "Order is already processing a cancel cannot modify" + myID;
                    Exception myE = new Exception(myError);
                    throw myE;
                }

                // check that the order is working
                if (!myOrder.IsWorking())
                {
                    if ((myOrder.OrdType.getValue() == QuickFix.OrdType.STOP) || (myOrder.OrdType.getValue() == QuickFix.OrdType.STOP_LIMIT))
                    {
                        // no action
                    }
                    else
                    {
                        string myError = "Order is not in a working state cannot replace:" + myID;
                        Exception myE = new Exception(myError);
                        throw myE;
                    }
                }
                // Check the limits for the order
                string myTextErr;
                double qty;
                if (newQty.HasValue)
                {
                    qty = newQty.Value;
                }
                else
                {
                    qty = myOrder.OrderQty.getValue();
                }
                double px;
                if (newPrice.HasValue)
                {
                    px = newPrice.Value;
                }
                else
                {
                    px = myOrder.Price.getValue();
                }
                if (LimitChecker.Instance().BreaksLimits(out myTextErr, myOrder, qty, px))
                {
                    myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                    myOrder.Text = "You have exceeded the order qty limit - please re-enter";
                    Exception myE = new Exception("You have exceeded the order qty limit - please re-enter");
                    throw myE;
                }

                // Create a FIX cancel replace messsage

                QuickFix.Message myFixMsg = new QuickFix.Message();
                QuickFix.MsgType msgType = new QuickFix.MsgType("G");
                myFixMsg.getHeader().setField(msgType);
                QuickFix.BeginString myBeginString = new QuickFix.BeginString("FIX.4.4");
                myFixMsg.getHeader().setField(myBeginString);

                // OrderID
                if (myOrder.OrderID == null)
                {
                    myOrder.Text = "No order ID found cannot modify order";
                    Exception myE = new Exception("No order ID found cannot modify order");
                    throw myE;
                }
                
                // set the original to the existing clordid
                myOrder.OrigClOrdID = new QuickFix.OrigClOrdID(myOrder.ClOrdID.getValue());
                myFixMsg.setField(myOrder.OrigClOrdID);

                myFixMsg.setField(myOrder.OrderID);

                ////Side
                myFixMsg.setField(myOrder.Side);

                // set up the product details in the message
                myOrder.Product.Set(myFixMsg);

                //// Transact Time
                myOrder.TransactTime = new QuickFix.TransactTime(DateTime.Now.ToUniversalTime());
                myFixMsg.setField(myOrder.TransactTime);

                myFixMsg.setField(myOrder.OrdType);

               

                

                // Create the message wrapper used to send the order cancel to the driver
                KaiTrade.Interfaces.Message myMsg = new KaiTCPComm.KaiMessageWrap();

                myMsg.Label = "G";

                string myDriverCode = "";
                // A trade venue must be specified on the order
                KaiTrade.Interfaces.Venue myVenue = null;
                if (myOrder.Product.TradeVenue.Length > 0)
                {
                    // get the driver code and session details from the venue manager
                    myVenue = AppFactory.Instance().GetVenueManager().GetVenue(myOrder.Product.TradeVenue);
                    if (myVenue != null)
                    {
                        myDriverCode = myVenue.DriverCode;
                        if (myVenue.TargetVenue.Length > 0)
                        {
                            myMsg.VenueCode = myVenue.TargetVenue;
                        }
                        if (myVenue.BeginString.Length > 0)
                        {
                            // this is the FIX version the server wants, we convert between
                            // versions in the FIX client
                            myMsg.Format = myVenue.BeginString;
                        }

                        if (myVenue.SID.Length > 0)
                        {
                            QuickFix.SenderCompID mySID = new QuickFix.SenderCompID(myVenue.SID);
                            myFixMsg.getHeader().setField(mySID);
                            myMsg.ClientID = myVenue.SID;
                        }
                        if (myVenue.TID.Length > 0)
                        {
                            QuickFix.TargetCompID myTID = new QuickFix.TargetCompID(myVenue.TID);
                            myFixMsg.getHeader().setField(myTID);
                            myMsg.TargetID = myVenue.TID;
                        }
                    }
                    else
                    {
                        string myErr = "Invalid Venue Code:" + myOrder.Product.TradeVenue;
                        m_Log.Error(myErr);
                        Exception myE = new Exception(myErr);
                        throw myE;
                    }
                }
                else
                {
                    string myErr = "No Venue Code specified on product:";
                    m_Log.Error(myErr);
                    Exception myE = new Exception(myErr);
                    throw myE;
                }

                // process any venue field bags - do this prior to the order bags
                // to allow the order setting to override the venues
                List<KaiTrade.Interfaces.Field> myFieldBag;
                KTAFacade.Instance().SetBag(out myFieldBag, myVenue.ReplaceBag, ",");

                foreach (KaiTrade.Interfaces.Field myField in myFieldBag)
                {
                    try
                    {
                        myFixMsg.setString(int.Parse(myField.ID), myField.Value);
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("replace Extra tags from venue error:", myE);
                    }
                }

                // process any additional tags for the order
                foreach (KaiTrade.Interfaces.Field myField in myOrder.ReplaceBag)
                {
                    try
                    {
                        myFixMsg.setString(int.Parse(myField.ID), myField.Value);
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("replace Extra tags error:", myE);
                    }
                }
                if (myOrder.Account != null)
                {
                    if (myOrder.Account.getValue().Length > 0)
                    {
                        myFixMsg.setField(myOrder.Account);
                    }
                }
               

                try
                {
                    //Send the message
                    KaiTrade.Interfaces.Driver myDrv = AppFactory.Instance().GetDriverManager().GetDriver(myDriverCode);
                    if (myDrv != null)
                    {
                        // save the existing clordid and set the new clordid
                        myClOrdID = myOrder.ClOrdID;
                        myOrder.ClOrdID = new QuickFix.ClOrdID(KaiUtil.Identities.Instance.genReqID());
                        myFixMsg.setField(myOrder.ClOrdID);
                        // Associate the CLOrdID with this order
                        AppFactory.Instance().GetOrderManager().AssociateClOrdID(myOrder.ClOrdID.getValue(), myOrder);



                        if (newQty.HasValue)
                        {
                            myOrder.OrderQty = new QuickFix.OrderQty((double)newQty);
                            myFixMsg.setField(myOrder.OrderQty);
                        }

                        if (newPrice.HasValue)
                        {
                            myOrder.Price = new QuickFix.Price((double)newPrice);
                            myFixMsg.setField(myOrder.Price);
                        }
                        if (newStopPx.HasValue)
                        {
                            myOrder.StopPx = new QuickFix.StopPx((double)newStopPx);
                            myFixMsg.setField(myOrder.StopPx);
                        }

                        // Set the FIX string as message data
                        myMsg.Data = myFixMsg.ToString();

                        // inform order manager clients that the order has changed
                        myOM.SetChanged(myOrder.Identity);
                        // send the message for processing
                        //myDrv.Send(myMsg);
                        // send the message for processing
                        order.LastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.replace;
                        //SendDelegate mySend = new SendDelegate(myDrv.Send);
                        //IAsyncResult ar = mySend.BeginInvoke(myMsg, SRCallback, "123456789");
                        myDrv.Send(myMsg);
                    }
                    else
                    {
                        string myError = "Driver not found for code:" + myDriverCode;
                        m_Log.Error(myError);
                        Exception myE = new Exception(myError);
                        throw myE;
                    }
                }
                catch (Exception myE)
                {
                    m_Log.Error("EditOrder:Sendnessage part" + myOrder.Identity, myE);
                    myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                    throw (myE);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("ReplaceOrder:", myE);
                if (myOrder != null)
                {
                    myOrder.OrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                    myOrder.Text = myE.Message;
                }
                throw (myE);
            }
        }
    }
}
