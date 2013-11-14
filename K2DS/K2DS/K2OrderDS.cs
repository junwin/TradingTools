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
using System.Linq;
using System.Text;

namespace K2DS
{
    /// <summary>
    /// Provides basic DS for orders
    /// </summary>
    public class K2OrderDS
    {
        private log4net.ILog m_Log;

        public K2OrderDS()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");
        }

        /// <summary>
        /// Insert the order passed into the databae
        /// </summary>
        /// <param name="order"></param>
        /// <param name="allowUpdate">if true then updates are allowed, if false an excpetion is thrown if order exists</param>
        public void InsertOrder(K2DataObjects.Order order, bool allowUpdate)
        {
            try
            {
                order.SystemDate = DateTime.Now;

                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    var dbOrders =
                      (from o in db.Orders
                       where o.Identity == order.Identity
                       select o).SingleOrDefault();

                    if (dbOrders != null)
                    {
                         
                        if (allowUpdate)
                        {
                            UpdateOrderFields(dbOrders, order);
                            //db.Orders.DeleteOnSubmit(dbOrders);
                            db.SubmitChanges();
                            //dbOrders = order;
                            //dbOrders.
                        }
                        else
                        {
                            throw new Exception("Order already exists");
                        }
                    }
                    else
                    {
                        db.Orders.InsertOnSubmit(order as K2DataObjects.Order);

                        db.SubmitChanges();
                    }

                }

            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }


        private void UpdateOrderFields(K2DataObjects.Order targetOrder, K2DataObjects.Order sourceOrder)
        {
            try
            {
                targetOrder.ClOrdID = sourceOrder.ClOrdID;
                targetOrder.AvgPx = sourceOrder.AvgPx;
                targetOrder.CumQty = sourceOrder.CumQty;

                targetOrder.LastPx = sourceOrder.LastPx;
                targetOrder.LastQty = sourceOrder.LastQty;
                targetOrder.LeavesQty = sourceOrder.LeavesQty;

                targetOrder.OrderQty = sourceOrder.OrderQty;
                targetOrder.OrdStatus = sourceOrder.OrdStatus;
                targetOrder.OrigClOrdID = sourceOrder.OrigClOrdID;
                targetOrder.OrdType = sourceOrder.OrdType;
                targetOrder.Side = sourceOrder.Side;


                targetOrder.Price = sourceOrder.Price;
                targetOrder.StopPx = sourceOrder.StopPx;
                targetOrder.Text = sourceOrder.Text;

                targetOrder.Tag = sourceOrder.Tag;
                targetOrder.TransactTime = sourceOrder.TransactTime;
                targetOrder.Account = sourceOrder.Account;
            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }

        }

    }
}
