/***************************************************************************
 *
 *      Copyright (c) 2009,2010, 2011,2012 KaiTrade LLC (registered in Delaware)
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

namespace OrderServices
{
    /// <summary>
    /// provides an arbitary grouping of orders
    /// </summary>
    public class OrderGroup : Publisher, KaiTrade.Interfaces.OrderGroup
    {
        /// <summary>
        /// Used to lock
        /// </summary>
        private object m_CalcPosn = new Object();

        private string m_ID;
        /// <summary>
        /// list of orders in the group
        /// </summary>
        private List<KaiTrade.Interfaces.Order> m_Orders;

        /// <summary>
        /// User defined tag
        /// </summary>
        //private string m_Tag = "";

        /// <summary>
        /// user defined step number - used for sequencing actions on the group
        /// </summary>
        private int m_StepNumber = 0;

        /// <summary>
        /// Time in ticks that the order group expires - used in some
        /// algos to expire groups that for example have not completed in a given period
        /// of time
        /// </summary>
        public long m_Expiration = 0;

        /// <summary>
        /// Are all the orders in the group completed
        /// </summary>
        private bool m_IsCompleted = false;

        // Totals for the group of orders
        private double m_ShortPendingQty = 0.0;
        private double m_LongPendingQty = 0.0;
        private double m_ShortWorkingQty = 0.0;
        private double m_LongWorkingQty = 0.0;
        private double m_ShortFilledQty = 0.0;
        private double m_LongFilledQty = 0.0;
        private double m_LongQty = 0.0;
        private double m_ShortQty = 0.0;

        /// <summary>
        /// Average price for the order group
        /// </summary>
        private double m_AvgPx = 0.0;

        private double m_TotalLots = 0.0;

        /// <summary>
        /// Consideration for the group
        /// </summary>
        private double m_Consideration = 0.0;

        private double m_PNL = 0;

        private string m_UserName = "";

        public OrderGroup()
        {
            m_ID = System.Guid.NewGuid().ToString();
            m_Name = m_ID;
            m_Orders = new List<KaiTrade.Interfaces.Order>();
        }

        protected override void resetDefaultFields()
        {
            try
            {
                RefreshPosition();
                this.setImageField("UPDTIME", System.Environment.TickCount.ToString());
                this.setImageField("Identity", m_ID);
                if (m_Orders.Count > 0)
                {
                    this.setImageField("LastOrdIdentity", m_Orders[m_Orders.Count-1].Identity);
                }

                if (m_UserName.Length > 0)
                {
                    this.setImageField("UserName", m_UserName);
                }

                this.setImageField("ShortWorkingQty", this.ShortWorkingQty.ToString());
                this.setImageField("ShortFilledQty", this.ShortFilledQty.ToString());
                this.setImageField("LongFilledQty", this.LongFilledQty.ToString());
                this.setImageField("LongWorkingQty", this.LongWorkingQty.ToString());
                this.setImageField("AvgPx", m_AvgPx.ToString());
                this.setImageField("Consideration", m_Consideration.ToString());
                this.setImageField("TotalLots", m_TotalLots.ToString());
                this.setImageField("PNL", this.PNL.ToString());
                this.setImageField("Tag", this.Tag);
            }
            catch (Exception myE)
            {
                m_Log.Error("resetDefaultFields", myE);
            }
        }

        /// <summary>
        /// Refresh the position of the group, will iterate the orders to
        /// total position
        /// </summary>
        public virtual void RefreshPosition()
        {
            lock (m_CalcPosn)
            {
                try
                {
                    m_ShortWorkingQty = 0.0;
                    m_LongWorkingQty = 0.0;
                    m_ShortFilledQty = 0.0;
                    m_LongFilledQty = 0.0;
                    m_LongQty = 0.0;
                    m_ShortQty = 0.0;

                    m_Consideration = 0.0;
                    m_AvgPx = 0.0;
                    m_TotalLots = 0.0;

                    // iterate all orders
                    foreach (KaiTrade.Interfaces.Order myOrd in m_Orders)
                    {
                        try
                        {
                            if (myOrd.Side.getValue() == QuickFix.Side.SELL)
                            {
                                if (myOrd.LeavesQty != null)
                                    m_ShortWorkingQty += myOrd.LeavesQty.getValue();
                                if (myOrd.CumQty != null)
                                    m_ShortFilledQty += myOrd.CumQty.getValue();
                                m_ShortQty += myOrd.OrderQty.getValue();
                            }
                            else
                            {
                                if (myOrd.CumQty != null)
                                    m_LongFilledQty += myOrd.CumQty.getValue();
                                if (myOrd.LeavesQty != null)
                                    m_LongWorkingQty += myOrd.LeavesQty.getValue();
                                m_LongQty += myOrd.OrderQty.getValue();
                            }
                            m_Consideration += myOrd.Consideration;
                            m_TotalLots += myOrd.CumQty.getValue();
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("RefreshPosition:loop", myE);
                        }
                    }
                    if (m_TotalLots > 0)
                    {
                        m_AvgPx = m_Consideration / m_TotalLots;
                    }
                }
                catch (Exception myE)
                {
                    m_Log.Error("RefreshPosition", myE);
                }
            }
        }

        /// <summary>
        /// add an order to the group
        /// </summary>
        /// <param name="myOrder"></param>
        public void AddOrder(KaiTrade.Interfaces.Order myOrder)
        {
            m_Orders.Add(myOrder);
        }

        /// <summary>
        /// get the unique id for the group
        /// </summary>
        public string ID
        {
            get{ return m_ID;}
        }

        /// <summary>
        /// get a list of the groups orders
        /// </summary>
        public List<KaiTrade.Interfaces.Order> Orders
        {
            get
            {
                return m_Orders;
            }
        }

        /// <summary>
        /// Get the short working qty of the strategy
        /// </summary>
        public double ShortWorkingQty
        {
            get { return m_ShortWorkingQty; }
            set { m_ShortWorkingQty = value; }
        }
        /// <summary>
        /// Get the LongWorking qty for the strategy
        /// </summary>
        public double LongWorkingQty
        {
            get { return m_LongWorkingQty; }
            set { m_LongWorkingQty = value; }
        }
        /// <summary>
        /// get the short filled qty
        /// </summary>
        public double ShortFilledQty
        {
            get { return m_ShortFilledQty; }
            set { m_ShortFilledQty = value; }
        }
        /// <summary>
        /// get the long filled qty
        /// </summary>
        public double LongFilledQty
        {
            get { return m_LongFilledQty; }
            set { m_LongFilledQty = value; }
        }
        /// <summary>
        /// get the long filled qty
        /// </summary>
        public double LongQty
        {
            get { return m_LongPendingQty; }
            set { m_LongPendingQty = value; }
        }
        /// <summary>
        /// get the long filled qty
        /// </summary>
        public double ShortQty
        {
            get { return m_ShortPendingQty; }
            set { m_ShortPendingQty = value; }
        }

        /// <summary>
        /// return the potential position (long filled - short filled) +ve => long, this includes working and pending qty
        /// </summary>
        public double PotentialPosition
        {
            get
            {
                return (this.LongQty - this.ShortQty);
            }
        }

        /// <summary>
        /// return the working qty (long working - short working) +ve => long
        /// </summary>
        public double Working
        {
            get
            {
                return this.LongWorkingQty - this.ShortWorkingQty;
            }
        }

        /// <summary>
        /// Get the user defined step numbers - typically used to apply a sequence of actions to the group
        /// </summary>
        public int StepNumber
        {
            get { return m_StepNumber; }
            set { m_StepNumber = value; }
        }

        /// <summary>
        /// Time in ticks that the order group expires - used in some
        /// algos to expire groups that for example have not completed in a given period
        /// of time
        /// </summary>
        public long Expiration
        {
            get { return m_Expiration; }
            set { m_Expiration = value; }
        }

        /// <summary>
        /// get/set a user defined tag to the group
        /// </summary>
        public string Tag
        {
            get { return m_Tag; }
            set { m_Tag = value; }
        }

        /// <summary>
        /// Detect if the ID is a member of the group
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool IsGroupMember(string ID)
        {
            foreach (KaiTrade.Interfaces.Order myOrder in m_Orders)
            {
                if (myOrder.Identity == ID)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Cancel any working orders in the group
        /// </summary>
        public void Cancel()
        {
            try
            {
                foreach (KaiTrade.Interfaces.Order order in m_Orders)
                {
                    if (order.IsWorking())
                    {
                        Factory.Instance().AppFacade.CancelOrder(order.Identity);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Cancel", myE);
            }
        }

        /// <summary>
        /// Edit the order for the ID specified
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="newQty">new qty if specified</param>
        /// <param name="newPrice">new price if specified</param>
        public void ReplaceOrder( double? newQty, double? newPrice, double? newStopPx)
        {
            try
            {
                foreach (KaiTrade.Interfaces.Order order in m_Orders)
                {
                    if (order.IsWorking())
                    {
                        Factory.Instance().AppFacade.ReplaceOrder(order.Identity, newQty, newPrice, newStopPx);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Cancel", myE);
            }
        }

        /// <summary>
        /// Returns whether or not a particular group of
        /// orders has completed i.e. that there are no more
        /// expected transactions for any of the individual orders
        /// </summary>
        /// <returns></returns>
        public bool IsCompleted()
        {
            if (!m_IsCompleted)
            {
                // if the group is flagged as not completed check to see if has
                // find out how many are not compelete
                int myNotCompletedCount = 0;
                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders)
                {
                    if (myOrder.IsWorking())
                    {
                        myNotCompletedCount++;
                    }
                }
                if (myNotCompletedCount > 0)
                {
                    m_IsCompleted = false;
                }
                else
                {
                    m_IsCompleted = true;
                }
            }

            return m_IsCompleted;
        }

        public int PendingOrderCount()
        {
            int pending = 0;
            foreach (KaiTrade.Interfaces.Order o in m_Orders)
            {
                if (o.OrderID != null)
                {
                    if (o.OrderID.getValue().Length == 0)
                    {
                        // no valid order id (i.e. empty string)
                        pending++;
                    }
                }
                else
                {
                    // no order id so must be pending
                    pending++;
                }
                if (o.OrdStatus.getValue() == QuickFix.OrdStatus.PENDING_NEW)
                {
                    pending++;
                }

            }
            return pending;
        }
        

        private double calcTotQty()
        {
            double myRetQty = 0.0;
            try
            {
                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders)
                {
                    if (myOrder.Side.getValue() == QuickFix.Side.BUY)
                    {
                        myRetQty += myOrder.OrderQty.getValue();
                    }
                    else if (myOrder.Side.getValue() == QuickFix.Side.SELL)
                    {
                        myRetQty -= myOrder.OrderQty.getValue();
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("calcTotQty", myE);
            }
            return myRetQty;
        }

        private double calcTotWorking()
        {
            double myRetQty = 0.0;
            m_PNL = 0.0;
            try
            {
                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders)
                {
                    if (myOrder.IsWorking())
                    {
                        if (myOrder.Side.getValue() == QuickFix.Side.BUY)
                        {
                            if (myOrder.LeavesQty != null)
                            {
                                myRetQty += myOrder.LeavesQty.getValue();
                            }
                        }
                        else if (myOrder.Side.getValue() == QuickFix.Side.SELL)
                        {
                            if (myOrder.LeavesQty != null)
                            {
                                myRetQty -= myOrder.LeavesQty.getValue();
                            }
                        }
                    }
                    m_PNL += myOrder.GetCurrentPNL();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("calcTotWorking", myE);
            }
            return myRetQty;
        }

        private double calcTotPosition()
        {
            double myRetQty = 0.0;
            m_PNL = 0.0;
            try
            {
                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders)
                {
                    if (myOrder.Side.getValue() == QuickFix.Side.BUY)
                    {
                        if (myOrder.CumQty != null)
                        {
                            myRetQty += myOrder.CumQty.getValue();
                        }
                    }
                    else if (myOrder.Side.getValue() == QuickFix.Side.SELL)
                    {
                        if (myOrder.CumQty != null)
                        {
                            myRetQty -= myOrder.CumQty.getValue();
                        }
                    }
                    m_PNL += myOrder.GetCurrentPNL();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("calcTotPosition", myE);
            }
            return myRetQty;
        }

        /// <summary>
        /// Get the the total order qty for the group - i.e. the sum of
        /// all the individual order qtys
        /// </summary>
        public double OrderQty
        {
            get { return calcTotQty(); }
        }

        /// <summary>
        /// Get the total working qty for the grouo i.e. the sum of all the working qtys (-ve => short)
        /// </summary>
        public double WorkingQty
        {
            get { return calcTotWorking(); }
        }

        /// <summary>
        /// Get the total position for the grouo i.e. the sum of all the working qtys (-ve => short)
        /// </summary>
        public double Position
        {
            get { return calcTotPosition(); }
        }

        /// <summary>
        /// Get/calculate the current PNL
        /// </summary>
        /// <returns></returns>
        public double CalculateCurrentPNL()
        {
            double m_PNL = 0.0;
            try
            {
                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders)
                {
                    m_PNL += myOrder.GetCurrentPNL();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("CalculateCurrentPNL", myE);
            }
            return m_PNL;
        }

        /// <summary>
        /// return the average px of all the orders - note this
        /// only makes sense for the same product
        /// </summary>
        /// <returns></returns>
        public double CalcAvgPx()
        {
            double AvgPx = 0.0;

            try
            {
                double myToTConsideration = 0.0;
                double myTotCumQty =0.0;
                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders)
                {
                    if(myOrder.CumQty != null)
                    {
                        myToTConsideration += myOrder.Consideration;
                        myTotCumQty += myOrder.CumQty.getValue();
                    }
                }
                if(myTotCumQty != 0)
                {
                    AvgPx =  myToTConsideration/myTotCumQty;
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("calcAvgPx", myE);
            }

            return AvgPx;
        }

        /// <summary>
        /// Get the PNL value based on last calculation
        /// </summary>
        public double PNL
        {
            get{return m_PNL;}
        }

        public string[] GetOrderLogInfo()
        {
            string[] orderInfo = new string[m_Orders.Count];
            for (int x=0; x < m_Orders.Count; x++)
            {
                orderInfo[x] = m_Orders[x].ToString();
            }
            return orderInfo;
        }

       
    }
}
