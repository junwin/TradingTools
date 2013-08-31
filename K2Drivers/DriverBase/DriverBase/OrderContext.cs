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

namespace DriverBase
{
    /// <summary>
    /// Used by the simulator
    /// </summary>
    public enum fillPattern { gradual, fixedAmount };

    public enum ORCommand
    {
        Undefined, Submit, Pull, Modify, OrderRecap, TradeRecap
    }
    /// <summary>
    /// provides a context for a driver to monitor orders sumbitted to market via 
    /// an external API
    /// </summary>
    public class OrderContext
    {
        private string m_ClOrdID;
        private string m_OrigClOrdID;
        private string m_OrderID = "";
        private string m_APIOrderID = "";
        private object m_ExternalOrder;
        private KaiTrade.Interfaces.IOrder _originalOrder;
        

        /// <summary>
        /// Command being processed
        /// </summary>
        private ORCommand m_CurrentCommand = ORCommand.Undefined;

        /// <summary>
        /// Time in ticks when the command was started
        /// </summary>
        private long m_LastCommandTicks;


        private int m_LeavesQty = 0;
        private int m_OrderQty = 0;
        private int m_CumQty = 0;
        private double m_FilledVolume = 0;
        private double m_TotalFillXVol = 0;
        private double m_AveragePrice = 0;
        private string m_Service = "";
        private string m_OrdStatus;
        private string m_ExecType;


        private string m_Identity = "";

        private string m_Side = "";
        private string m_OrderType = "";
        private decimal m_Price = -1;
        private decimal? _stopPrice = null;

        

        private int m_TargetFillAmount = 1;

        /// <summary>
        ///  do the fill update every n times
        /// </summary>
        private long m_UpdatePeriod = 1;

        /// <summary>
        /// how many time we have been called by the timer
        /// </summary>
        private long m_TimerCount = 0;
        private int m_OriginalQty;



        private fillPattern m_FillPattern = fillPattern.gradual;

        public OrderContext()
        {
            m_Identity = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// unique id for the context
        /// </summary>
        public string Identity
        {
            get
            {
                return m_Identity;
            }
            set
            {
                m_Identity = value;
            }
        }

        public bool isActive()
        {
            bool active = true;
            try
            {
                if ((OrdStatus == KaiTrade.Interfaces.OrderStatus.FILLED) || (OrdStatus == KaiTrade.Interfaces.OrderStatus.CANCELED) || (OrdStatus == KaiTrade.Interfaces.OrderStatus.REJECTED))
                {
                    active = false;
                }
            }
            catch (Exception myE)
            {
            }
            return active;
             
        }
        public bool isPending(long delay)
        {
            bool pending = true;
            try
            {
                if (this.CurrentCommand == ORCommand.Undefined)
                {
                    pending = false;
                }
                else
                {
                    if (DateTime.Now.Ticks > (m_LastCommandTicks + delay))
                    {
                        pending = true;
                    }
                    else
                    {
                        pending = false;
                    }
                }
            }
            catch (Exception myE)
            {
            }
            return pending;
        }

        /// <summary>
        /// get set the ClOrdId for the context
        /// </summary>
        public string ClOrdID
        {
            get
            {
                return m_ClOrdID;
            }
            set
            {
                m_ClOrdID = value;
            }
        }


        public string OrigClOrdID
        {
            get
            {
                return m_OrigClOrdID;
            }
            set
            {
                m_OrigClOrdID = value;
            }
        }



        /// <summary>
        /// get/set broker order id
        /// </summary>
        public string OrderID
        {
            get
            {
                return m_OrderID;
            }
            set
            {
                m_OrderID = value;
            }
        }

        /// <summary>
        /// get/set broker order id
        /// </summary>
        public string APIOrderID
        {
            get
            {
                return m_APIOrderID;
            }
            set
            {
                m_APIOrderID = value;
            }
        }

        /// <summary>
        /// Service that the order is placed on - depends on driver
        /// </summary>
        public string Service
        {
            get
            {
                return m_Service;
            }
            set
            {
                m_Service = value;
            }
        }

        public double GetAverageFillPx()
        {
            try
            {
                if (m_FilledVolume > 0)
                {
                    return m_TotalFillXVol / m_FilledVolume;
                }
                else
                {
                    return 0.0;
                }
            }
            catch (Exception myE)
            {
                
            }
            return 0;
        }
        public double LastAveragePrice
        {
            get { return m_AveragePrice; }
            set { m_AveragePrice = value; }
        }

        public void AddFillData(double myVolume, double myPx)
        {
            try
            {
                m_FilledVolume += myVolume;
                m_TotalFillXVol += (myVolume * myPx);
                
            }
            catch (Exception myE)
            {

            }
        }

        public int CumQty
        {
            get { return m_CumQty; }
            set { m_CumQty = value; }
        }

        public int LeavesQty
        {
            get { return m_OrderQty - m_CumQty; }
           
        }
        public int OrderQty
        {
            get { return m_OrderQty; }
            set { m_OrderQty = value; }
        }

        /// <summary>
        /// Get/Set the current Order status- as send on the last exec report
        /// </summary>
        public string OrdStatus 
        {
            get { return m_OrdStatus; }
            set { m_OrdStatus = value; }
        }

        /// <summary>
        /// Get/Set the current ExectType - as send on the last exec report
        /// </summary>
        public string ExecType
        {
            get { return m_ExecType; }
            set { m_ExecType = value; }
        }

        public ORCommand CurrentCommand
        {
            get
            {
                return m_CurrentCommand;
            }
            set
            {
                m_CurrentCommand = value;
                m_LastCommandTicks = DateTime.Now.Ticks;
            }
        }


        /// <summary>
        /// Get set the API order object in the context
        /// </summary>
        public object ExternalOrder
        {
            get
            {
                return m_ExternalOrder;
            }
            set
            {
                m_ExternalOrder = value;
            }
        }
        /// <summary>
        /// Get set the QuickFix order in the context
        /// </summary>
        public KaiTrade.Interfaces.IOrder OriginalOrder
        {
            get
            {
                return _originalOrder;
            }
            set
            {
                _originalOrder = value;
            }
        }
        public override string ToString()
        {
            string myRet = "";
            try
            {
                myRet += string.Format("ClOrdId {0}, OrderID {1}, Command {2}, OrderStatus {3}", this.ClOrdID, this.OrderID, this.CurrentCommand.ToString(), OrdStatus);   
            }
            catch (Exception myE)
            {
            }
            return myRet;
        }


        public string OrderType
        {
            get { return m_OrderType; }
            set { m_OrderType = value; }
        }

        public string Side
        {
            get { return m_Side; }
            set { m_Side = value; }
        }
        public decimal Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }
        public decimal? StopPrice
        {
            get { return _stopPrice; }
            set { _stopPrice = value; }
        }
        public int XOriginalQty
        {
            get
            {
                return m_OriginalQty;
            }
            set
            {
                m_OriginalQty = value;
            }
        }

        public int TargetFillAmount
        {
            get
            {
                return m_TargetFillAmount;
            }
            set
            {
                m_TargetFillAmount = value;
            }
        }
        public fillPattern FillPattern
        {
            get
            {
                return m_FillPattern;
            }
            set
            {
                m_FillPattern = value;
            }
        }

        public long TimerCount
        {
            get
            {
                return m_TimerCount;
            }
            set
            {
                m_TimerCount = value;
            }
        }

        public long UpdatePeriod
        {
            get
            {
                return m_UpdatePeriod;
            }
            set
            {
                m_UpdatePeriod = value;
            }
        }

    }
}
