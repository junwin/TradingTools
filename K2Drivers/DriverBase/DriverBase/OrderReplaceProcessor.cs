/***************************************************************************
 *    
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
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
    /// <summary>
    /// Provide asyn processing of replace requests - note that this
    /// will collapse requests until they can be actioned by the broker API
    /// </summary>
    public class OrderReplaceProcessor
    {
        private Queue<RequestData> _queue;

        private SyncEvents _syncEvents;
        DriverBase _Handler;

        private Dictionary<string, RequestData> _replaceRequest;

        /// <summary>
        /// Create a logger to record driver specific info 
        /// </summary>
        public log4net.ILog m_DriverLog;

        public OrderReplaceProcessor(DriverBase myHandler, Queue<RequestData> q, SyncEvents e)
        {
            _queue = q;
            _syncEvents = e;
            _Handler = myHandler;
            _replaceRequest = new Dictionary<string, RequestData>();
            m_DriverLog = log4net.LogManager.GetLogger("KaiDriverLog");
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
            int count = 0;
            Queue<RequestData> myWorkQueue = new Queue<RequestData>();
            while (WaitHandle.WaitAny(_syncEvents.EventArray) != 1)
            {
                try
                {
                    // get any new replace requests
                    lock (((ICollection)_queue).SyncRoot)
                    {
                        while (_queue.Count > 0)
                        {
                            myWorkQueue.Enqueue(_queue.Dequeue());
                        }
                    }

                    // action most recent requests
                    while (myWorkQueue.Count > 0)
                    {
                        RequestData replace = myWorkQueue.Dequeue();
                        if (_replaceRequest.ContainsKey(replace.Mnemonic))
                        {
                            _replaceRequest[replace.Mnemonic]=(replace);
                        }
                        else
                        {
                            _replaceRequest.Add(replace.Mnemonic, replace);
                        }
                        OrderReplaceResult result = OrderReplaceResult.error;
                        if (_replaceRequest[replace.Mnemonic].CRRType == crrType.replace)
                        {
                            result = _Handler.modifyOrder(_replaceRequest[replace.Mnemonic]as ModifyRequestData);
                        }
                        else if (_replaceRequest[replace.Mnemonic].CRRType == crrType.cancel)
                        {
                            result = _Handler.cancelOrder(_replaceRequest[replace.Mnemonic]as CancelRequestData);
                        }
                        else
                        {
                            // error condition
                            m_DriverLog.Error("Invild CancelReplace type");

                        }

                        switch (result)
                        {
                            case OrderReplaceResult.success:
                                _replaceRequest.Remove(replace.Mnemonic);
                                break;
                            case OrderReplaceResult.error:
                                _replaceRequest.Remove(replace.Mnemonic);
                                break;
                            case OrderReplaceResult.replacePending:
                                _replaceRequest[replace.Mnemonic].RetryCount += 1;
                                break;
                            case OrderReplaceResult.cancelPending:
                                _replaceRequest[replace.Mnemonic].RetryCount += 1;
                                break;
                            default:
                                _replaceRequest[replace.Mnemonic].RetryCount += 1;
                                break;
                        }

                    }

                    // action any remaining requests
                    List<string> mnemonics = new List<string>();
                    foreach (string mnemonic in _replaceRequest.Keys)
                    {
                        mnemonics.Add(mnemonic);
                    }
                    foreach (string mnemonic in mnemonics)
                    {

                        OrderReplaceResult result = _Handler.modifyOrder(_replaceRequest[mnemonic] as ModifyRequestData);
                        switch (result)
                        {
                            case OrderReplaceResult.success:
                                _replaceRequest.Remove(mnemonic);
                                break;
                            case OrderReplaceResult.error:
                                _replaceRequest.Remove(mnemonic);
                                break;
                            case OrderReplaceResult.replacePending:
                                break;
                            default:
                                break;
                        }

                    }

                    count++;
                }
                catch (Exception myE)
                {
                    m_DriverLog.Error("Main Loop:", myE);
                }
            }

        }


    } 
}
