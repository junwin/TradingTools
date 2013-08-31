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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Timers;

namespace K2Depth
{
    public class K2DOM : KaiTrade.Interfaces.IDOM
    {
        private delegate void SendDOMSlotUpdate(KaiTrade.Interfaces.IDOMSlot slot);
        //SendDOMSlotUpdate m_SndDOMSlotUpdDelegate;

        private KaiTrade.Interfaces.OnDOMImage domImage;


        private KaiTrade.Interfaces.OnDOMUpdate domUpdate;

        /// <summary>
        /// Worker thread to handle inbound dom updatesmessages
        /// </summary>
        private DOMUpdateProcessor m_DOMUpdateProcessor;
        private Queue<List<KaiTrade.Interfaces.IDOMSlot>> m_SlotUpdateQueue;
        //private Queue<KaiTrade.Interfaces.IDOMSlot> m_SlotUpdateQueue;
        private SyncEvents m_SyncEvents;
        private Thread m_DOMUpdateThread;

        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        /// <summary>
        /// Get set the DOM data associated with this object
        /// </summary>
        private KaiTrade.Interfaces.IDOMData domData;

        public KaiTrade.Interfaces.OnDOMImage DOMImage
        { get { return domImage; } set { domImage = value; } }

        public KaiTrade.Interfaces.OnDOMUpdate DOMUpdate
        { get { return domUpdate; } set { domUpdate = value; } }

        public KaiTrade.Interfaces.IDOMData DOMData
        { get { return domData; } set { domData = value; } }

        public K2DOM()
        {
            
            m_SlotUpdateQueue = new Queue<List<KaiTrade.Interfaces.IDOMSlot>>();
            m_SyncEvents = new SyncEvents();
            m_DOMUpdateProcessor  =new DOMUpdateProcessor(this,m_SlotUpdateQueue,m_SyncEvents);
            m_DOMUpdateThread = new Thread(m_DOMUpdateProcessor.ThreadRun);
            m_DOMUpdateThread.Start();


        }

        public int GetSlotIndex(decimal price)
        {
            int slotIndex = -1;
            try
            {
                // note that the BasePrice is low - ideally the lowest price
                // allowing for a max daily movement
                decimal offset = price - domData.BasePrice;
                slotIndex = (int)(offset / domData.MinPxIncrement);
            }
            catch (Exception myE)
            {
            }
            return slotIndex;
        }

        public KaiTrade.Interfaces.IDOMData Create(decimal startPx, decimal maxPxMovement, decimal minPxIncrement)
        {

            domData = null;
            try
            {
                // Calculate the max slots
                int maxSlots = (int)(maxPxMovement / minPxIncrement);
                domData = new K2DataObjects.DOMData();
                domData.K2DOMSlots = new K2DataObjects.DOMSlot[maxSlots];
                domData.MaxSlots = maxSlots;

                // base price is low down the array (should be pos 0)
                domData.BasePrice = startPx - (minPxIncrement * maxSlots / 2);
                domData.MinPxIncrement = minPxIncrement;

            }
            catch (Exception myE)
            {
                m_Log.Error("Create", myE);
            }
            return domData;
        }

        

        public void Update(decimal price, decimal? bidSize, decimal? askSize)
        {
            try
            {
                List<KaiTrade.Interfaces.IDOMSlot> slotUpdates = new List<KaiTrade.Interfaces.IDOMSlot>();
                int slotIndex = GetSlotIndex(price);
                if (domData.K2DOMSlots[slotIndex] == null)
                {
                    domData.K2DOMSlots[slotIndex] = new K2DataObjects.DOMSlot(price, bidSize, askSize);
                }
                else
                {

                    if (askSize.HasValue)
                    {
                        if (domData.K2DOMSlots[slotIndex].AskSize.Value != askSize.Value)
                        {
                            domData.K2DOMSlots[slotIndex].AskSize = askSize;
                        }
                    }
                    else
                    {
                        domData.K2DOMSlots[slotIndex].AskSize = null;
                    }
                    if (bidSize.HasValue)
                    {
                        if (domData.K2DOMSlots[slotIndex].BidSize.Value != bidSize.Value)
                        {
                            domData.K2DOMSlots[slotIndex].BidSize = bidSize;
                        }
                    }
                    else
                    {
                        domData.K2DOMSlots[slotIndex].BidSize = null;
                    }


                }

                domData.K2DOMSlots[slotIndex].LastUpdateTicks = DateTime.Now.Ticks;
                slotUpdates.Add(domData.K2DOMSlots[slotIndex]);                
                ApplyDOMUpdate(slotUpdates);
                 
            }
            catch (Exception myE)
            {
            }
        }

        public void Update(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            try
            {
                int bidSlotIndex = -1;
                int askSlotIndex = -1;
                List<KaiTrade.Interfaces.IDOMSlot> slotUpdates = new List<KaiTrade.Interfaces.IDOMSlot>();
                if (pxUpdate.BidPrice.HasValue)
                {
                    bidSlotIndex = GetSlotIndex(pxUpdate.BidPrice.Value);
                    if (domData.K2DOMSlots[bidSlotIndex] == null)
                    {
                        domData.K2DOMSlots[bidSlotIndex] = new K2DataObjects.DOMSlot(pxUpdate.BidPrice.Value, pxUpdate.BidSize, null);
                    }
                    else
                    {
                        if (pxUpdate.BidSize.HasValue)
                        {
                            domData.K2DOMSlots[bidSlotIndex].BidSize = pxUpdate.BidSize;
                        }

                    }


                }
                if (pxUpdate.OfferPrice.HasValue)
                {
                    askSlotIndex = GetSlotIndex(pxUpdate.OfferPrice.Value);
                    if (domData.K2DOMSlots[askSlotIndex] == null)
                    {
                        domData.K2DOMSlots[askSlotIndex] = new K2DataObjects.DOMSlot(pxUpdate.OfferPrice.Value, null, pxUpdate.OfferSize);
                    }
                    else
                    {
                        if (pxUpdate.OfferSize.HasValue)
                        {
                            domData.K2DOMSlots[askSlotIndex].AskSize = pxUpdate.OfferSize;
                        }

                    }


                }

                if ((bidSlotIndex == askSlotIndex) && (bidSlotIndex != -1))
                {
                    domData.K2DOMSlots[bidSlotIndex].LastUpdateTicks = DateTime.Now.Ticks;
                    slotUpdates.Add(domData.K2DOMSlots[bidSlotIndex]);
                }
                else if (bidSlotIndex != -1)
                {
                    domData.K2DOMSlots[bidSlotIndex].LastUpdateTicks = DateTime.Now.Ticks;
                    slotUpdates.Add(domData.K2DOMSlots[bidSlotIndex]);
                }
                else if (askSlotIndex != -1)
                {
                    domData.K2DOMSlots[askSlotIndex].LastUpdateTicks = DateTime.Now.Ticks;
                    slotUpdates.Add(domData.K2DOMSlots[askSlotIndex]);
                }
                ApplyDOMUpdate(slotUpdates);



            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Update the DOM using a set of DOM slots/levels
        /// </summary>
        /// <param name="updateSlices"></param>
        public void Update(KaiTrade.Interfaces.IDOMSlot[] updateSlices)
        {
            try
            {
                List<KaiTrade.Interfaces.IDOMSlot> slotUpdates = new List<KaiTrade.Interfaces.IDOMSlot>();
                foreach (KaiTrade.Interfaces.IDOMSlot slot in updateSlices)
                {
                    if (slot == null)
                    {
                        continue;
                    }

                    // get DOM index for the slot price and make sure there is a slot at that index in the DOM
                    int slotIndex = GetSlotIndex(slot.Price);
                    if (domData.K2DOMSlots[slotIndex] != null)
                    {
                        domData.K2DOMSlots[slotIndex].Price = slot.Price;
                        if (slot.AskSize.HasValue)
                        {
                            // update the size if its different or does not exist
                            if (domData.K2DOMSlots[slotIndex].AskSize.HasValue)
                            {
                                if (domData.K2DOMSlots[slotIndex].AskSize.Value != slot.AskSize.Value)
                                {
                                    domData.K2DOMSlots[slotIndex].AskSize = slot.AskSize;
                                }
                            }
                            else
                            {
                                domData.K2DOMSlots[slotIndex].AskSize = slot.AskSize;
                            }
                        }
                        else
                        {
                            // was no ask size in the inbound slot so set the DOM slot ask size to null
                            domData.K2DOMSlots[slotIndex].AskSize = null;
                        }
                        if (slot.BidSize.HasValue)
                        {
                            if (domData.K2DOMSlots[slotIndex].BidSize.HasValue)
                            {
                                if (domData.K2DOMSlots[slotIndex].BidSize.Value != slot.BidSize.Value)
                                {
                                    domData.K2DOMSlots[slotIndex].BidSize = slot.BidSize;
                                }
                            }
                            else
                            {
                                domData.K2DOMSlots[slotIndex].BidSize = slot.BidSize;
                            }
                        }
                        else
                        {
                            // was no bid size in the inbound slot so set the DOM slot ask size to null
                            domData.K2DOMSlots[slotIndex].BidSize = null;
                        }

                    }
                    else
                    {
                        domData.K2DOMSlots[slotIndex] = slot;
                    }
                    slotUpdates.Add(domData.K2DOMSlots[slotIndex]);
                    //updateClients(domData.K2DOMSlots[slotIndex]);
                }
                ApplyDOMUpdate(slotUpdates);

            }
            catch (Exception myE)
            {
            }
        }

        

        public void ApplyDOMUpdate(List<KaiTrade.Interfaces.IDOMSlot> slotUpdates)
        {
            try
            {
                // do the update assync
                lock (((ICollection)m_SlotUpdateQueue).SyncRoot)
                {
                    m_SlotUpdateQueue.Enqueue(slotUpdates);
                    m_SyncEvents.NewItemEvent.Set();
                }

            }
            catch (Exception myE)
            {
                //m_Log.Error("ApplyPriceUpdate", myE);
            }
        }

    }
}
