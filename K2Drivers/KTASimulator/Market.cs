
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Newtonsoft.Json;
using K2ServiceInterface;

namespace KTASimulator
{
    public class Market
    {
        /// <summary>
        /// Parent simulation
        /// </summary>
        private KTASimulator m_Parent;

        /// <summary>
        /// Product that this market is based on
        /// </summary>
        private string m_Mnemonic;

        /// <summary>
        /// Product for the market
        /// </summary>
        private KaiTrade.Interfaces.IProduct m_Product = null;

       

        /// <summary>
        /// Timer for those algos that require some time based processing
        /// </summary>
        private System.Timers.Timer m_Timer;

        /// <summary>
        /// Timer interval used for the timer
        /// </summary>
        private long m_TimerInterval = 5000;

        /// <summary>
        /// Map of order contectxs to Order ID
        /// </summary>
        private Dictionary<string, DriverBase.OrderContext> m_OrderContextOrdID;

        /// <summary>
        /// Map of order contectxs to Order ClOrdID
        /// </summary>
        private Dictionary<string, DriverBase.OrderContext> m_OrderContextClOrdID;

        /// <summary>
        /// Set to true on a price update
        /// </summary>
        private bool m_IsChanged = false;


        public Market(string mnemonic, KTASimulator parent)
        {
            m_Parent = parent;
            m_Mnemonic = mnemonic;
            m_Product = parent.Facade.GetProductManager().GetProductMnemonic(mnemonic);

            m_OrderContextOrdID = new Dictionary<string, DriverBase.OrderContext>();
            m_OrderContextClOrdID = new Dictionary<string, DriverBase.OrderContext>();
            StartTimer();
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
                ///hardcoded on
                m_IsChanged = true;
                if (m_IsChanged)
                {
                    foreach (DriverBase.OrderContext cntx in m_OrderContextClOrdID.Values)
                    {
                        if (cntx.isActive())
                        {
                            fillOrder(cntx);

                        }
                    }
                    m_IsChanged = false;
                }
                
            }
            catch (Exception myE)
            {
                m_Parent.Log.Error("OnTimer", myE);
            }
        }

        private void fillOrder(DriverBase.OrderContext cntx)
        {
            try
            {
                if (m_Product == null)
                {
                    m_Product = m_Parent.Facade.GetProductManager().GetProductMnemonic(m_Mnemonic);
                    if (m_Product == null)
                    {
                        return;
                    }
                }
                
                IL1PX L1PX = m_Parent.Facade.PriceHandler.GetPXPublisher(m_Product) as IL1PX;

                if (cntx.OrderType == KaiTrade.Interfaces.OrderType.MARKET)
                {
                    if (cntx.Side == KaiTrade.Interfaces.Side.BUY)
                    {

                        fillOrder(cntx, L1PX.OfferSize.Value, L1PX.OfferPrice);

                    }
                    else if (cntx.Side == KaiTrade.Interfaces.Side.SELL)
                    {

                        fillOrder(cntx, L1PX.BidSize.Value, L1PX.BidPrice.Value);

                    }
                }
                else
                {
                    if (cntx.Side == KaiTrade.Interfaces.Side.BUY)
                    {
                        if (cntx.Price >= L1PX.OfferPrice.Value)
                        {
                            fillOrder(cntx, L1PX.OfferSize.Value, L1PX.OfferPrice);
                        }
                    }
                    else if (cntx.Side == KaiTrade.Interfaces.Side.SELL)
                    {
                        if (cntx.Price <= L1PX.BidPrice.Value)
                        {
                            fillOrder(cntx, L1PX.BidSize.Value, L1PX.BidPrice);
                        }
                    }
                    else
                    {
                    }
                }
                  
            }
            catch (Exception myE)
            {
                m_Parent.Log.Error("fillOrder", myE);
            }
        }

        private void fillOrder(DriverBase.OrderContext cntx, decimal availableQty, decimal? fillPx)
        {
            try
            {
                m_Parent.fixedAmountFill(cntx, cntx.TargetFillAmount, fillPx);

            }
            catch (Exception myE)
            {
                m_Parent.Log.Error("fillOrder", myE);
            }
        }

        public void ApplyPxUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            try
            {
                m_IsChanged = true;

            }
            catch (Exception myE)
            {
                m_Parent.Log.Error("ApplyPxUpdate", myE);
            }
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
                m_Parent.Log.Error("SUBTEST:" + myMsg.Data);
                nos = JsonConvert.DeserializeObject<K2DataObjects.SubmitRequest>(myMsg.Data);

                int quantity = (int)nos.OrderQty;

                decimal myOrdPrice = 99;
                if (nos.Price.HasValue)
                {

                    myOrdPrice = nos.Price.Value;
                }


                KaiTrade.Interfaces.IFill fill = new K2DataObjects.Fill();
                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.NEW;
                fill.ExecType = KaiTrade.Interfaces.ExecType.ORDER_STATUS;
                fill.OrderID = DriverBase.Identities.Instance.getNextOrderID();


                DriverBase.OrderContext myContext = new DriverBase.OrderContext();
                //myContext.QFOrder = myOrder;
                myContext.ClOrdID = nos.ClOrdID;
                myContext.OrderID = fill.OrderID;
                myContext.OrderQty = quantity;
                //myContext.LeavesQty = quantity;
                myContext.CumQty = 0;
                myContext.Price = myOrdPrice;
                myContext.Side = nos.Side;
                myContext.OrderType = nos.OrdType;
                // record the order in the context maps
                m_OrderContextClOrdID.Add(myContext.ClOrdID, myContext);
                m_OrderContextOrdID.Add(myContext.OrderID, myContext);
                myContext.OrdStatus = fill.OrderStatus;

                m_Parent.sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, (int)nos.OrderQty, 0, 0.0, 0.0, "", "");

                

            }
            catch (Exception myE)
            {
                m_Parent.log.Error("submitOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                m_Parent.SendAdvisoryMessage("KTA Simulator:submitOrder: problem submitting order:" + myE.ToString());

                if (nos != null)
                {
                    m_Parent.sendExecReport(nos, KaiTrade.Interfaces.OrderStatus.REJECTED, KaiTrade.Interfaces.ExecType.REJECTED, myE.ToString(), "OTHER");

                }
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


                if (mod.ClOrdID.Length==0)
                {
              
                    m_Parent.sendCancelReplaceRej(mod, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a clordid must be specified on a modify order");
                    Exception myE = new Exception("a clordid must be specified on a modify order");
                    throw myE;
                }

                if (mod.OrigClOrdID.Length ==0)
                {
                    m_Parent.sendCancelReplaceRej(mod, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a original clordid must be specified on a modify order");
                    Exception myE = new Exception("an original clordid must be specified on a modify order");
                    throw myE;
                }

                // Get the context - we must have this to access the CQG order
                DriverBase.OrderContext myContext = null;
                if (m_OrderContextClOrdID.ContainsKey(mod.OrigClOrdID))
                {
                    myContext = m_OrderContextClOrdID[mod.OrigClOrdID];
                }
                if (myContext == null)
                {
                    m_Parent.sendCancelReplaceRej(mod, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "an order does not exist for the modify requested");
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
                    myContext.OrderQty = (int) mod.Qty.Value;                

                }

                // update the ClOrdID's on our order in the context
                myContext.ClOrdID = mod.ClOrdID;
                myContext.OrigClOrdID = mod.OrigClOrdID;

                // record the context against the new clordid
                m_OrderContextClOrdID.Add(mod.ClOrdID, myContext);

                // send order in book exec report
                m_Parent.sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, (int)myContext.LeavesQty, myContext.CumQty, 0.0, 0.0, "", "");

            }
            catch (Exception myE)
            {

                m_Parent.Log.Error("modifyOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                m_Parent.SendAdvisoryMessage("TDA:modifyOrder: problem modify order:" + myE.ToString());

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
                fill.OrderStatus = KaiTrade.Interfaces.OrderStatus.REPLACED;
                fill.ExecType = KaiTrade.Interfaces.ExecType.ORDER_STATUS;
                fill.OrderID = DriverBase.Identities.Instance.getNextOrderID();


                if (cancel.ClOrdID.Length == 0)
                {
                    m_Parent.sendCancelRej(cancel, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a clordid must be specified on a modify order");
                    Exception myE = new Exception("a clordid must be specified on a modify order");
                    throw myE;
                }

                if (cancel.OrigClOrdID.Length == 0)
                {
                    m_Parent.sendCancelRej(cancel, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "a original clordid must be specified on a modify order");
                    Exception myE = new Exception("an original clordid must be specified on a modify order");
                    throw myE;
                }

                // Extract the raw FIX Message from the inbound message
                string strOrder = msg.Data;

              

                // Get the context - we must have this to access the CQG order
                DriverBase.OrderContext myContext = null;
                if (m_OrderContextClOrdID.ContainsKey(cancel.OrigClOrdID))
                {
                    myContext = m_OrderContextClOrdID[cancel.OrigClOrdID];
                }
                if (myContext == null)
                {
                    m_Parent.sendCancelRej(cancel, KaiTrade.Interfaces.CxlRejReason.UnknownOrder, "an order does not exist for the cancel requested");
                    Exception myE = new Exception("an order does not exist for the cancel requested");
                    throw myE;
                }

                double myAveFillPrice = 0.0;

                myContext.ClOrdID = cancel.ClOrdID;
                myContext.OrigClOrdID = cancel.OrigClOrdID;

                // send order in book exec report
                m_Parent.sendExecReport(myContext, fill.OrderID, fill.OrderStatus, fill.ExecType, 0.0, (int)myContext.LeavesQty, myContext.CumQty, 0.0, 0.0, "", "");

                myContext.OrdStatus = fill.OrderStatus;

            }
            catch (Exception myE)
            {

                m_Parent.Log.Error("pullOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                m_Parent.SendAdvisoryMessage("pullOrder: problem pulling order:" + myE.ToString());

            }
        }



    }
}
