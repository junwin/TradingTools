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
    public class K2SignalDS
    {
        private log4net.ILog m_Log;

        public K2SignalDS()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");
        }

        public void Insert(K2DataObjects.TradeSignal signal, bool allowUpdate)
        {
            try
            {
                

                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    var dbSignals =
                      (from s in db.TradeSignals
                       where s.Identity == signal.Identity
                       select s).SingleOrDefault();

                    if (dbSignals != null)
                    {
                        if (allowUpdate)
                        {
                            db.TradeSignals.DeleteOnSubmit(dbSignals);
                            db.SubmitChanges();
                        }
                        else
                        {
                            throw new Exception("trade signal exists");
                        }
                    }
                    db.TradeSignals.InsertOnSubmit(signal);

                    db.SubmitChanges();

                }

            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }

    }
}
