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
using System.Runtime.Serialization;
//using System.ServiceModel;
using System.Linq;

namespace OrderServices
{
    /// <summary>
    /// Represents an order that can be sent to a trade venue for different asset classes
    /// can have associated legs when a Basket or Strategy order.
    /// Orders impliment the publisher interface to be able to publish thier data
    /// </summary>
    public class Order : Publisher
    {
        
        private KaiTrade.Interfaces.LastOrderCommand m_LastCommand = KaiTrade.Interfaces.LastOrderCommand.none;
        //NOT USED? private bool m_PendingReplace = false;

        
        private List<KaiTrade.Interfaces.IFill> m_FillsList = new List<KaiTrade.Interfaces.IFill>();

        private KaiTrade.Interfaces.IOrder _currentOrder = null;

        private bool m_IsInUpdate = false;

        
        
        public Order()
        {

            m_PublisherType = "Order";
            
        }

        
        /// <summary>
        /// Set any default image fields called by the base class
        /// </summary>
        protected override void resetDefaultFields()
        {
            try
            {
                this.setImageField("Identity", _currentOrder.Identity);

                if (_currentOrder.StrategyName.Length > 0)
                {
                    this.setImageField("StrategyName", _currentOrder.StrategyName);
                }
                if (_currentOrder.CumQty != null)
                {
                    this.setImageField("CumQty", _currentOrder.CumQty.ToString());
                }

                if (_currentOrder.LeavesQty != null)
                {
                    this.setImageField("LeavesQty", _currentOrder.LeavesQty.ToString());
                }
                if (_currentOrder.OrdStatus != null)
                {
                    this.setImageField("OrdStatus", _currentOrder.OrdStatus);
                }
                if (_currentOrder.AvgPx != null)
                {
                    this.setImageField("AvgPx", _currentOrder.AvgPx.ToString());
                }
                if (_currentOrder.Price != null)
                {
                    this.setImageField("Price", _currentOrder.Price.ToString());
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("resetDefaultFields", myE);
            }
        }

        /// <summary>
        /// Record and updated field and update subscribers if not in some transaction
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        protected void applyUpdate(string myID, string myValue)
        {
            try
            {
                this.updateField(myID, myValue);
                // if we are not in some update transaction then publish the new value
                if (!m_IsInUpdate)
                {
                    this.doUpdate();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("doUpdate", myE);
            }
        }

        #region Order Members

        
        /// <summary>
        /// Consideration AvgPx*amount filled
        /// </summary>
        public double Consideration
        {
            get
            {
                double dRet = 0;
                if (_currentOrder.AvgPx != null)
                {
                    if (_currentOrder.CumQty != null)
                    {
                        dRet = _currentOrder.AvgPx * _currentOrder.CumQty;
                    }
                }
                return dRet;
            }
        }

        

        public List<KaiTrade.Interfaces.IFill> FillsList
        {
            get { return m_FillsList; }
        }

        

        private string getNewValue(string existingValue, string newValue)
        {
            if (newValue == null)
            {
                return existingValue;
            }
            if (newValue.Length > 0)
            {
                return newValue;
            }
            else
            {
                return existingValue;
            }
        }

       

        public double HighLimit
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        
        public double LowLimit
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        

        public KaiTrade.Interfaces.LastOrderCommand LastCommand
        {
            get { return m_LastCommand; }
            set { m_LastCommand = value; }
        }
        /// <summary>
        /// Is the order pending a replace request
        /// </summary>
        bool PendingReplace
        { get; set; }

        /// <summary>
        /// Is the order pending a cancel request
        /// </summary>
        bool PendingCancel
        { get; set; }

        

        /// <summary>
        /// Get/Set the group(if any) that the order belongs to, this is used when orders are
        /// related for example in pairs trading - note that a Strategy can contain
        /// one of more OrderGroups
        /// </summary>
        public KaiTrade.Interfaces.OrderGroup OrderGroup
        {
            get { return m_OrderGroup; }
            set { m_OrderGroup = value;}
        }

        public string OCAOrderLinkName
        {
            get { return m_OCOOrderLinkName; }
            set { m_OCOOrderLinkName = value; }
        }
        /// <summary>
        /// Get/Set the name of the group of OCO orders that this order
        /// belongs to.
        /// </summary>
        public string OCAGroupName
        {
            get { return m_OCAGroupName; }
            set
            {
                m_OCAGroupName = value;
                if (value.Length > 0)
                {
                    Factory.Instance().AppFacade.Factory.GetOCOManager().AddOrderOCO(this, value);
                }
            }
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
        

        
        #endregion

        #region Order Members

        

        /// <summary>
        /// Returns if the order is working i.e. it can be canceled or modified
        /// </summary>
        /// <returns></returns>
        public bool IsWorking(KaiTrade.Interfaces.IOrder order)
        {
            bool bRet = true;
            // if there is no status its not working
            if (order.OrdStatus == null)
            {
                bRet = false;
            }
            else
            {
                // check the status
                switch (order.OrdStatus)
                {
                    case KaiTrade.Interfaces.OrderStatus.FILLED :
                        bRet = false;
                        break;
                    case KaiTrade.Interfaces.OrderStatus.CANCELED:
                        bRet = false;
                        break;
                    case KaiTrade.Interfaces.OrderStatus.REJECTED:
                        // Is there still qty in the market? for example a replace request
                        // was rejected but the order is working
                        if (order.LeavesQty != null)
                        {
                            if (order.LeavesQty == 0)
                            {
                                bRet = false;
                            }
                            
                        }
                        else
                        {
                            bRet = false;
                        }
                        break;

                }
            }
            return bRet;
        }

        #endregion

        /// <summary>
        /// Get the P&L for the order provided at this time(current market prices) if the orders
        /// product is subscribed - will throw an exception if the product is not valid.
        /// Note this is not realtime, its calculated when called
        /// </summary>
        public double GetCurrentPNL(KaiTrade.Interfaces.IOrder order)
        {
            if (order.AvgPx != null)
            {
                return GetCurrentPNL(order.AvgPx);
            }
            return 0.0;
        }

        /// <summary>
        /// Get the P&L for the order provided at this time(current market prices) using the
        /// fill prices passed in.- will throw an exception if the product is not valid.
        /// This is used to calc the PNL implications of trading working qty at market price
        /// Note this is not realtime, its calculated when called
        /// </summary>
        /// <param name="AvgPx">Assumed fill prices</param>
        /// <returns></returns>
        public double GetCurrentPNL(double AvgPx, KaiTrade.Interfaces.IOrder order)
        {
            double myPNL = 0;
            if (order.Product == null)
            {
                throw new Exception("The product is not valid cannot calclate PNL");
            }
            double myCumQty = this.CumQty.getValue();
            if (myCumQty > 0)
            {
                // they have a filled qty
                double myPriceChange;
                if (this.Side.getValue() == QuickFix.Side.BUY)
                {
                    myPriceChange = (double)Product.L1PX.OfferPrice.Value - AvgPx;
                }
                else
                {
                    myPriceChange = AvgPx - (double)Product.L1PX.BidPrice.Value;
                }

                myPNL = myCumQty * (double)this.Product.ContractSize * myPriceChange;
            }

            return myPNL;
        }

        #region Transaction Members

        /// <summary>
        /// an update has been completed so do an update
        /// </summary>
        public void EndUpdate()
        {
            m_IsInUpdate = false;
            this.doUpdate();
        }

        /// <summary>
        /// start an update we do not publish anything until the EndUpdate
        /// </summary>
        public void StartUpdate()
        {
            m_IsInUpdate = true;
        }

        #endregion

        public override string ToString()
        {
            string myRet = "";
            try
            {
                myRet += string.Format("Strategy {0}, ID {1}, Parent {2}, VenueCode {3}", this.StrategyName, this.Identity, this.ParentIdentity, m_VenueCode);
                myRet += string.Format("Mnemonic {0}, Tag{1}, Account {2}, Side {3}", this.Mnemonic, this.Tag, this.Account, KaiUtil.QFUtils.DecodeSide(this.Side));
                if (this.ClOrdID != null)
                {
                    myRet += string.Format("ClOrdID {0}", ClOrdID.getValue());
                }
                double myStopPx = 0.0;
                if (this.StopPx != null)
                {
                    myStopPx = this.StopPx;
                }
                double myPx = 0.0;
                if (this.Price != null)
                {
                    myPx = this.Price;
                }
                double myLeavesQty = 0.0;
                if (this.LeavesQty != null)
                {
                    myLeavesQty = this.LeavesQty;
                }
                double myCumQty = 0.0;
                if (this.CumQty != null)
                {
                    myCumQty = this.CumQty.getValue();
                }
                double myLastPx = 0.0;
                if (this.LastPx != null)
                {
                    myLastPx = this.LastPx.getValue();
                }
                double myLastQty = 0.0;
                if (this.LastQty != null)
                {
                    myLastQty = this.LastQty.getValue();
                }
                double myAvgPx = 0.0;
                if (this.AvgPx != null)
                {
                    myAvgPx = this.AvgPx.getValue();
                }
                myRet += string.Format("OrdType {0}, Qty{1}, Price {2}, StopPx {3}", KaiUtil.QFUtils.DecodeOrderType(this.OrdType), this.OrderQty.getValue(), myPx, myStopPx);
                myRet += string.Format("OrdStatus {0}, LeavesQty{1}, CumQty {2}, OrderID {3}", KaiUtil.QFUtils.DecodeOrderStatus(this.OrdStatus), myLeavesQty, myCumQty, this.OrderID.getValue());
                myRet += string.Format("TIF {0}, LastPx {1}, LastQty {2}, AvePx {3}", KaiUtil.QFUtils.DecodeTimeInForce(this.TimeInForce), myLastPx, myLastQty, myAvgPx);
                myRet += string.Format("Text {0}", this.Text);
                if (m_UserID != null)
                {
                    myRet += string.Format("UserID {0}", this.User);
                }
                if (m_SessionID != null)
                {
                    myRet += string.Format("SessionID {0}", this.SessionID);
                }
                myRet += string.Format("CorrelationID {0}", this.CorrelationID);
                if (m_ParentIdentity!= null)
                {
                    myRet += string.Format("ParentID {0}", this.ParentIdentity);
                }

                //myOrd.ExpireDate = m_ExpireDate.getValue();
            }
            catch
            {
            }

            return myRet;
        }

        /// <summary>
        /// Return an order as tab separated
        /// </summary>
        /// <returns></returns>
        public string ToTabSeparated(KaiTrade.Interfaces.IOrder order)
        {
            string myRet = "";
            try
            {
                myRet += string.Format("{0}\t{1}\t{2}\t{3}\t", order.StrategyName, order.Identity, order.ParentIdentity, order.TradeVenue);
                myRet += string.Format("{0}\t{1}\t{2}\t{3}\t", order.Mnemonic, order.Tag, order.Account, order.Side);
                double myStopPx = 0.0;
                if (order.StopPx != null)
                {
                    myStopPx = order.StopPx;
                }
                double myPx = 0.0;
                if (order.Price != null)
                {
                    myPx = order.Price;
                }
                double myLeavesQty = 0.0;
                if (order.LeavesQty != null)
                {
                    myLeavesQty = order.LeavesQty;
                }
                double myCumQty = 0.0;
                if (order.CumQty != null)
                {
                    myCumQty = order.CumQty;
                }
                double myLastPx = 0.0;
                if (order.LastPx != null)
                {
                    myLastPx = order.LastPx;
                }
                double myLastQty = 0.0;
                if (order.LastQty != null)
                {
                    myLastQty = order.LastQty;
                }
                double myAvgPx = 0.0;
                if (order.AvgPx != null)
                {
                    myAvgPx = order.AvgPx;
                }

                myRet += string.Format("{0}\t{1}\t{2}\t{3}\t", order.OrdType, order.OrderQty, myPx, myStopPx);
                myRet += string.Format("{0}\t{1}\t{2}\t{3}\t", order.OrdStatus, myLeavesQty, myCumQty, order.OrderID.getValue());
                myRet += string.Format("{0}\t{1}\t{2}\t{3}\t", order.TimeInForce, myLastPx, myLastQty, myAvgPx);
                myRet += string.Format("{0}", order.Text);
                myRet += "\n";

                //myOrd.ExpireDate = m_ExpireDate.getValue();
            }
            catch
            {
            }

            return myRet;
        }
    }

}
