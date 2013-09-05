/***************************************************************************
 *
 *      Copyright (c) 2009,2010 KaiTrade LLC (registered in Delaware)
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

namespace K2Managers
{
    /// <summary>
    /// Provide a base class for all the general purpose managers we have - note
    /// *does not* not manage the publishers themselves
    /// </summary>
    public class PublisherManager : KaiTrade.Interfaces.IPublisher
    {
        /// <summary>
        /// Name of this publisher
        /// </summary>
        protected string m_Name = "";

        /// <summary>
        /// list of our subscribers - we image and update these as changes occur
        /// </summary>
        private System.Collections.Generic.List<KaiTrade.Interfaces.ISubscriber> m_Subscribers;

        private string m_PublisherType = "PublisherManager";

        /// <summary>
        /// Logger
        /// </summary>
        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        /// <summary>
        /// Tag used on the publisher
        /// </summary>
        private string m_Tag = "";

        public PublisherManager()
        {
            m_Subscribers = new List<KaiTrade.Interfaces.ISubscriber>();
        }

        /// <summary>
        /// Update a single field to all the subscribers
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        protected void DoUpdate(string myID, string myValue)
        {
            try
            {
                // if we have some observers
                if (m_Subscribers.Count > 0)
                {
                    // create a list of changed fields with their current values
                    System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList = new List<KaiTrade.Interfaces.IField>();
                    K2DataObjects.Field myField = new K2DataObjects.Field(myID, myValue);

                    // add the current time as the update time to the list
                    K2DataObjects.Field myUpdTimeField;
                    myUpdTimeField = new K2DataObjects.Field("UPDTIME", System.Environment.TickCount.ToString());

                    myFieldList.Add(myUpdTimeField);
                    myFieldList.Add(myField);

                    // send the update to all our observers
                    foreach (KaiTrade.Interfaces.ISubscriber mySubscriber in m_Subscribers)
                    {
                        try
                        {
                            mySubscriber.OnUpdate(this, myFieldList);
                        }
                        catch (Exception myE)
                        {
                            m_Log.Error("doUpdate - invoke", myE);
                        }
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("doUpdate", myE);
            }
        }

        #region Publisher Members

        void KaiTrade.Interfaces.IPublisher.Close()
        {
            m_Log.Info(m_Name + "is closing");

            m_Subscribers.Clear();
        }

        void KaiTrade.Interfaces.IPublisher.OnImage(List<KaiTrade.Interfaces.IField> itemList)
        {
            //
            // No action for an Order
        }

        void KaiTrade.Interfaces.IPublisher.OnStatusChange(List<KaiTrade.Interfaces.IField> itemList)
        {
            //
            // No action for an Order
        }
        /// <summary>
        /// Get/Set the fields list for the publisher - setting this will replace all
        /// existing fields in the publisher and issue an image
        /// </summary>
        public System.Collections.Generic.List<KaiTrade.Interfaces.IField> FieldList
        {
            get
            {
                System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList = new List<KaiTrade.Interfaces.IField>();

                return myFieldList;
            }
            set
            {
                (this as KaiTrade.Interfaces.IPublisher).OnImage(value);
            }
        }

        /// <summary>
        /// get/set the publisher base status - will event all subscribers
        /// </summary>
        public KaiTrade.Interfaces.Status Status
        {
            get { return KaiTrade.Interfaces.Status.undefined; }
            set { ;}
        }
        /// <summary>
        /// Get/Set the publisher type - this is user defined
        /// </summary>
        public string PublisherType
        {
            get { return m_PublisherType; }
            set { m_PublisherType = value; }
        }

        void KaiTrade.Interfaces.IPublisher.OnUpdate(List<KaiTrade.Interfaces.IField> itemList)
        {
            //
            // No action for an Order
        }

        /// <summary>
        /// Update the publisher with some price update - not all publishers
        /// will action this.
        /// </summary>
        /// <param name="pxUpdate"></param>
        public void OnUpdate(string mnemonic, KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            // No action
        }

        /// <summary>
        /// update some arbitary field in the publisher - note not all publishers
        /// may support this
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        public void OnUpdate(string myID, string myValue)
        {
            try
            {
            }
            catch (Exception myE)
            {
                m_Log.Error("OnUpdate:field:", myE);
            }
        }

        string KaiTrade.Interfaces.IPublisher.Open(string myData)
        {
            //
            // No action for an Order
            return "";
        }

        void KaiTrade.Interfaces.IPublisher.Subscribe(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            try
            {
                m_Subscribers.Add(mySubscriber);
                //myObserver.OnStatusChange(m_StatusInfo); //PTR

                // send an initial image
                System.Collections.Generic.List<KaiTrade.Interfaces.IField> myFieldList = new List<KaiTrade.Interfaces.IField>();

                K2DataObjects.Field myField = new K2DataObjects.Field("IMAGE", "IMAGE");
                myFieldList.Add(myField);
                mySubscriber.OnImage(this, myFieldList);
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.Subscribe", myE);
            }
        }

        string KaiTrade.Interfaces.IPublisher.Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }

        string KaiTrade.Interfaces.IPublisher.TopicID()
        {
            return m_Name;
        }

        string KaiTrade.Interfaces.IPublisher.TopicID(string myData)
        {
            return m_Name;
        }

        void KaiTrade.Interfaces.IPublisher.UnSubscribe(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            try
            {
                m_Subscribers.Remove(mySubscriber);
            }
            catch (Exception myE)
            {
                m_Log.Error("Publisher.UnSubscribe:", myE);
            }
        }
        #endregion
    }
}
