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
    public class K2BarData
    {
        private log4net.ILog m_Log;
        string _connectString = "";

        public K2BarData()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");
            _connectString = global::K2DS.Properties.Settings.Default.K2DSConnectionString;
        }

        public void Insert(K2DataObjects.PriceBar inBar, bool allowUpdate)
        {
            try
            {
                
                using (K2DataObjects.PriceBarDataContext db = new K2DataObjects.PriceBarDataContext(_connectString))
                {
                    var dbPriceBars =
                      (from bar in db.PriceBars
                       where bar.Mnemonic == inBar.Mnemonic &&
                        bar.ItemType == inBar.ItemType &&
                        bar.ItemSize == inBar.ItemSize && 
                        bar.TimeStamp == inBar.TimeStamp &&
                        bar.RequestID == inBar.RequestID
                       select bar).SingleOrDefault();

                    if (dbPriceBars != null)
                    {
                        if (allowUpdate)
                        {
                            db.PriceBars.DeleteOnSubmit(dbPriceBars);
                            db.SubmitChanges();
                        }
                        else
                        {
                            throw new Exception("bar exists");
                        }
                    }
                    db.PriceBars.InsertOnSubmit(inBar);

                    db.SubmitChanges();

                }

            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }
        public K2DataObjects.PriceBar[] GetPriceBars(string mnemonic, long startTick, int count)
        {
            using (K2DataObjects.PriceBarDataContext db = new K2DataObjects.PriceBarDataContext(_connectString))
            {
                var barSet =
                   (from bar in db.PriceBars
                    where (bar.Mnemonic == mnemonic) && (bar.TimeStamp >= startTick)
                    select bar).Take(count);

                return (barSet.ToArray());

            }
        }

        public K2DataObjects.PriceBar GetLastPriceBars(string mnemonic, long startTick)
        {
            using (K2DataObjects.PriceBarDataContext db = new K2DataObjects.PriceBarDataContext(_connectString))
            {
                var barSet =
                   (from bar in db.PriceBars
                    where (bar.Mnemonic == mnemonic) && (bar.TimeStamp >= startTick)
                    select bar).LastOrDefault();

                return (barSet);

            }
        }
    }
}
