
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
        private string getK2Side(eOrderSide side)
        {
            string strSide = "";
            switch (side)
            {
                case eOrderSide.osdBuy:
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


        /*
          
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
                        //Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osBusted:
                        myText = "eOrderStatus.osBusted";
                        //Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osInTransitTimeout:
                        myText = "eOrderStatus.osInTransitTimeout" + order.LastError.Description;
                        //Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osRejectFCM:
                        myText = "eOrderStatus.osRejectFCM:" + order.LastError.Description;
                        //Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    case eOrderStatus.osRejectGW:
                        myText = "eOrderStatus.osRejectGW:" + order.LastError.Description;
                        //Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                    default:
                        myText = "eOrderStatus - not known:" + order.GWStatus.ToString();
                        //Facade.RaiseAlert("KTACQG", myText, 0, KaiTrade.Interfaces.ErrorLevel.recoverable, KaiTrade.Interfaces.FlashMessageType.error);
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("cel_OrderChanged:", ex);
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
                        List<KaiTrade.Interfaces.IFill> fills = new List<KaiTrade.Interfaces.IFill>();
                        KaiTrade.Interfaces.IOrder ktOrder = getOrderData(order);
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
                                KaiTrade.Interfaces.IOrder o = Facade.Factory.GetOrderManager().GetOrderWithClOrdIDID(myContext.ClOrdID);
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



        private KaiTrade.Interfaces.IOrder getOrderData(CQGOrder order)
        {

            KaiTrade.Interfaces.IOrder orderData = new K2DataObjects.Order();
            try
            {
                orderData.Identity = order.GUID;

                orderData.StrategyName = "CQGNotKnown";
                orderData.Mnemonic = order.InstrumentName;
                orderData.Account = order.Account.GWAccountName;
                
                orderData.OrdType = getKTAOrderType(order.Type);
                switch (orderData.OrdType)
                {
                    case KaiTrade.Interfaces.IOrderType.MARKET:
                        break;
                    case KaiTrade.Interfaces.IOrderType.LIMIT:
                        orderData.Price = order.LimitPrice;
                        break;
                    case KaiTrade.Interfaces.IOrderType.STOP:
                        
                        orderData.StopPx = order.StopPrice;
                        break;
                    case KaiTrade.Interfaces.IOrderType.STOPLIMIT:
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


         
        private void submitOrder(KaiTrade.Interfaces.IMessage myMsg)
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

        private void pullOrder(KaiTrade.Interfaces.IMessage myMsg)
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

        
        private void modifyOrder(KaiTrade.Interfaces.IMessage myMsg)
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



        public override OrderReplaceResult modifyOrder(DriverBase.ModifyRequestData replaceData)
        {
            try
            {
                if (oRLog.IsInfoEnabled)
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
                    return KaiTrade.Interfaces.IOrderReplaceResult.replacePending;
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

                return KaiTrade.Interfaces.IOrderReplaceResult.success;

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

                return KaiTrade.Interfaces.IOrderReplaceResult.error;
            }
        }

        /// <summary>
        /// Used from the queue repace and cancel processor
        /// </summary>
        /// <param name="replaceData"></param>
        /// <returns></returns>
        public override OrderReplaceResult cancelOrder(DriverBase.CancelRequestData replaceData)
        {
            try
            {
                if (oRLog.IsInfoEnabled)
                {
                    oRLog.Info("cancelOrderR:" + replaceData.LastQFMod);
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
                    return KaiTrade.Interfaces.IOrderReplaceResult.cancelPending;
                }

                // Cancel the order
                (replaceData.OrderContext.ExternalOrder as CQGOrder).Cancel();

                replaceData.OrderContext.CurrentCommand = DriverBase.ORCommand.Pull;

                // record the context against the new clordid
                //m_ClIDOrder.Add(clOrdID.getValue(), myContext);

                return KaiTrade.Interfaces.IOrderReplaceResult.success;
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

                return KaiTrade.Interfaces.IOrderReplaceResult.error;
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
                    myRet = KaiTrade.Interfaces.IOrderType.LIMIT;
                    break;
                case eOrderType.otMarket:
                    myRet = KaiTrade.Interfaces.IOrderType.MARKET;
                    break;
                case eOrderType.otStop:
                    myRet = KaiTrade.Interfaces.IOrderType.STOP;
                    break;
                case eOrderType.otStopLimit:
                    myRet = KaiTrade.Interfaces.IOrderType.STOPLIMIT;
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

         
         */

    }
}
