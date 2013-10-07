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
        private Queue<List<KaiTrade.Interfaces.IDOMSlot>> _queue;
        private SyncEvents _syncEvents;
        private K2DOM _dom;
        private BlockingCollection<List<KaiTrade.Interfaces.IDOMSlot>> slotUpdates;

        public DOMUpdateProcessor(K2DOM dom, BlockingCollection<List<KaiTrade.Interfaces.IDOMSlot>> updates, SyncEvents e)
        {
            slotUpdates = updates;
            _syncEvents = e;
            _dom = dom;
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
            foreach (var item in slotUpdates.GetConsumingEnumerable())
            {
                applySlotUpdate(item);
            }
            /*
            int count = 0;
            Queue<List<KaiTrade.Interfaces.IDOMSlot>> myWorkQueue = new Queue<List<KaiTrade.Interfaces.IDOMSlot>>();
            while (WaitHandle.WaitAny(_syncEvents.EventArray) != 1)
            {
                lock (((ICollection)_queue).SyncRoot)
                {
                    while (_queue.Count > 0)
                    {
                        myWorkQueue.Enqueue(_queue.Dequeue());
                    }
                }
                while (myWorkQueue.Count > 0)
                {
                    try
                    {
                        List<KaiTrade.Interfaces.IDOMSlot> slotUpdate = myWorkQueue.Dequeue();
                        foreach (KaiTrade.Interfaces.IDOMSlot slot in slotUpdate)
                        {
                            if (slot != null)
                            {
                                applySlotUpdate(slot);
                            }
                        }
                    }
                    catch
                    {
                    }
                    
                }
                
                count++;
            }
             */
            Console.WriteLine("Consumer Thread: consumed {0} items", 0);
        }

        private void applySlotUpdate(List<KaiTrade.Interfaces.IDOMSlot> slot)
        {
            //List<KaiTrade.Interfaces.IDOMSlot> slots =  new List<KaiTrade.Interfaces.IDOMSlot>();
            //slots.Add(slot);
            _dom.DOMUpdate(_dom, slot);
            
        }
        
    }

   

    public class SyncEvents
    {
        public SyncEvents()
        {

            _newItemEvent = new AutoResetEvent(false);
            _exitThreadEvent = new ManualResetEvent(false);
            _eventArray = new WaitHandle[2];
            _eventArray[0] = _newItemEvent;
            _eventArray[1] = _exitThreadEvent;
        }

        public EventWaitHandle ExitThreadEvent
        {
            get { return _exitThreadEvent; }
        }
        public EventWaitHandle NewItemEvent
        {
            get { return _newItemEvent; }
        }
        public WaitHandle[] EventArray
        {
            get { return _eventArray; }
        }

        private EventWaitHandle _newItemEvent;
        private EventWaitHandle _exitThreadEvent;
        private WaitHandle[] _eventArray;
    }
}

