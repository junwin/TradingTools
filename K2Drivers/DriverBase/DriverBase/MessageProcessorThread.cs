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

namespace DriverBase
{
    public class MessageProcessorThread
    {
        private Queue<KaiTrade.Interfaces.IMessage> _queue;
        private SyncEvents _syncEvents;
        List<KaiTrade.Interfaces.IClient> _Clients;


        public MessageProcessorThread(ref List<KaiTrade.Interfaces.IClient> clients, Queue<KaiTrade.Interfaces.IMessage> q, SyncEvents e)
        {
            _queue = q;
            _syncEvents = e;
            _Clients = clients;
        }
        public MessageProcessorThread(KaiTrade.Interfaces.IClient client, Queue<KaiTrade.Interfaces.IMessage> q, SyncEvents e)
        {
            _queue = q;
            _syncEvents = e;
            _Clients = new List<KaiTrade.Interfaces.IClient>();
            _Clients.Add(client);
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
            int count = 0;
            Queue<KaiTrade.Interfaces.IMessage> myWorkQueue = new Queue<KaiTrade.Interfaces.IMessage>();
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
                    KaiTrade.Interfaces.IMessage msg = myWorkQueue.Dequeue();

                    foreach (KaiTrade.Interfaces.IClient myClient in _Clients)
                    {
                        try
                        {
                            if (myClient != null)
                            {
                                myClient.OnMessage(msg);
                            }
                        }
                        catch (Exception myE)
                        {
                            
                        }
                    }
                }
                
                count++;
            }
            //Console.WriteLine("Consumer Thread: consumed {0} items", count);
        }
    }
}
