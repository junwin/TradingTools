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
    public class MessageProcessorThread
    {
        private BlockingCollection<KaiTrade.Interfaces.IMessage> messages;
        List<KaiTrade.Interfaces.IClient> _Clients;


        public MessageProcessorThread(ref List<KaiTrade.Interfaces.IClient> clients, BlockingCollection<KaiTrade.Interfaces.IMessage> m)
        {
            messages = m;
            _Clients = clients;
        }
        public MessageProcessorThread(KaiTrade.Interfaces.IClient client, BlockingCollection<KaiTrade.Interfaces.IMessage> m)
        {
            messages = m;
            _Clients = new List<KaiTrade.Interfaces.IClient>();
            _Clients.Add(client);
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
            int count = 0;
            foreach (var msg in messages.GetConsumingEnumerable())
            {

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
                count++;
            }

            
        }
    }
}
