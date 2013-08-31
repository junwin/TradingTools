﻿/***************************************************************************
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

        public DOMUpdateProcessor(K2DOM dom, Queue<List<KaiTrade.Interfaces.IDOMSlot>> q, SyncEvents e)
        {
            _queue = q;
            _syncEvents = e;
            _dom = dom;
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
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
            Console.WriteLine("Consumer Thread: consumed {0} items", count);
        }

        private void applySlotUpdate(KaiTrade.Interfaces.IDOMSlot slot)
        {
            if (slot.DOMSlotUpdate != null)
            {
                slot.DOMSlotUpdate(slot, slot.Price, slot.BidSize, slot.AskSize);
            }
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

