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
using System.Threading;
using System.Collections;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DriverBase
{
    /// <summary>
    /// Provides a worker thread to process price updates so releave the broker API
    /// </summary>
    public class PxUpdateProcessor
    {
        private BlockingCollection<KaiTrade.Interfaces.IPXUpdate> pxUpdates;
        DriverBase handler;


        public PxUpdateProcessor(DriverBase myHanlder, BlockingCollection<KaiTrade.Interfaces.IPXUpdate> newUpdates)
        {
            pxUpdates = newUpdates;
            handler = myHanlder;
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
            int count = 0;
            foreach (var update in pxUpdates.GetConsumingEnumerable())
            {
                handler.PriceUpdateClients(update);
                count++;
            }
        }
        
    }

    
}
