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

namespace K2Depth
{
    /// <summary>
    /// Provides a worker thread to process price updates so releave the broker API
    /// </summary>
    public class DOMUpdateProcessor
    {

        private K2DOM domData;
        private BlockingCollection<List<KaiTrade.Interfaces.IDOMSlot>> slotUpdates;

        public DOMUpdateProcessor(K2DOM dom, BlockingCollection<List<KaiTrade.Interfaces.IDOMSlot>> updates)
        {
            slotUpdates = updates;
            domData = dom;
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
            foreach (var item in slotUpdates.GetConsumingEnumerable())
            {
                applySlotUpdate(item);
            }
            
            Console.WriteLine("Consumer Thread: consumed {0} items", 0);
        }

        private void applySlotUpdate(List<KaiTrade.Interfaces.IDOMSlot> slot)
        {
            //List<KaiTrade.Interfaces.IDOMSlot> slots =  new List<KaiTrade.Interfaces.IDOMSlot>();
            //slots.Add(slot);
            domData.DOMUpdate(domData, slot);
            
        }
        
    }

   

   
}

