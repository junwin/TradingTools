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

namespace K2DomainSvc
{
    public class Trade
    {

        /// <summary>
        /// Update a trade using an order
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="o"></param>
        public  void UpdateTrade(KaiTrade.Interfaces.ITrade trade, KaiTrade.Interfaces.IOrder o)
        {
            try
            {
                if (o.OrdStatus == KaiTrade.Interfaces.OrderStatus.FILLED.ToString())
                {
                    if (trade.ClOrdID != null)
                    {
                        if (trade.ClOrdID != o.ClOrdID)
                        {
                            trade.ClOrdID2 = o.ClOrdID;
                        }
                    }
                    else
                    {
                        trade.ClOrdID = o.ClOrdID;
                    }
                    trade.Mnemonic = o.Mnemonic;

                    trade.LastPx = (decimal)o.LastPx;
                    trade.LastQty = (decimal)o.LastQty;
                    //trade.TransactTime = o.TransactTime;
                    //trade.TradeDate = o.TransactTime;
                    trade.Side = o.Side;


                    /*
                   
                    if (tcr.IsValidExecID)
                    {
                        trade.ExecutionID = tcr.ExecID;
                    } 
                     */
                }
            }
            catch (Exception myE)
            {
            }
        }

        public void InsertFill(KaiTrade.Interfaces.IFill f)
        {
            try
            {
                K2DS.K2TradeDS tradeDS = new K2DS.K2TradeDS();
                tradeDS.InsertFill(f as K2DataObjects.Fill,true);
            }
            catch (Exception myE)
            {
            }
        }
    }
}
