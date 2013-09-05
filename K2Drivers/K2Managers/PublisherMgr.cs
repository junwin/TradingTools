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

namespace KTManagers
{
    /// <summary>
    /// Class for managing a set of publishers used by the system and
    /// external clients - this is the class that contains a set of publishers
    /// </summary>
    public class PublisherMgr : KaiTrade.Interfaces.PublisherManager
    {
        private static volatile PublisherMgr s_instance;

        private static object syncRoot = new object();

        private Dictionary<string, KaiTrade.Interfaces.Publisher> m_PublisherMap;

        private List<KaiTrade.Interfaces.K2InstanceFactory> m_PublisherFactories;

        /// <summary>
        /// Logger
        /// </summary>
        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        protected PublisherMgr()
		{
            m_PublisherMap = new Dictionary<string, KaiTrade.Interfaces.Publisher>();
            m_PublisherFactories = new List<KaiTrade.Interfaces.K2InstanceFactory>();
		}

        public static PublisherMgr Instance()
		{
			// Singleton method
            if (s_instance == null)
            {
                lock (syncRoot)
                {
                    if (s_instance == null)
                    {
                        s_instance = new PublisherMgr();
                    }
                }
            }
			return s_instance;
		}

        /// <summary>
        /// Add some external instance factory that will be used to create algos of a given type - this is used to allow
        /// plugins and other external assemblies register their oen algos
        /// </summary>
        /// <param name="algoType"></param>
        /// <param name="factory"></param>
        public void AddInstanceFactory(KaiTrade.Interfaces.PublisherType publisherType, KaiTrade.Interfaces.K2InstanceFactory factory)
        {
            switch (publisherType)
            {
                case KaiTrade.Interfaces.PublisherType.General:
                    m_PublisherFactories.Add(factory);
                    break;
                case KaiTrade.Interfaces.PublisherType.Price:
                    m_PublisherFactories.Add(factory);
                    break;

                default:
                    m_PublisherFactories.Add(factory);
                    break;
            }
        }

        /// <summary>
        /// Get a list of all the available publisher types
        /// </summary>
        /// <returns></returns>
        public List<string> GetPublisherTypes()
        {
            List<string> typeNames = new List<string>();
            try
            {
                typeNames.Add("PXPublisher");
                typeNames.Add("Publisher");
                foreach (KaiTrade.Interfaces.K2InstanceFactory factory in m_PublisherFactories)
                {
                    try
                    {
                        foreach (string pubName in factory.Names)
                        {
                            typeNames.Add(pubName);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch(Exception myE)
            {
                m_Log.Error("GetPublisherTypes", myE);
            }
            return typeNames;
        }

        /// <summary>
        /// Add/replace a publisher to the manager
        /// </summary>
        /// <param name="myPub"></param>
        public void Add(KaiTrade.Interfaces.Publisher myPub)
        {
            if(m_PublisherMap.ContainsKey(myPub.TopicID()))
            {
                m_PublisherMap[myPub.TopicID()] = myPub;
            }
            else
            {
                m_PublisherMap.Add(myPub.TopicID(), myPub);
            }
        }

        /// <summary>
        /// remove a publisher
        /// </summary>
        /// <param name="myPub"></param>
        public void Remove(KaiTrade.Interfaces.Publisher myPub)
        {
            m_PublisherMap.Remove(myPub.TopicID());
        }

        /// <summary>
        /// Create an instance of a particular publisher
        /// </summary>
        /// <param name="myType">type of publisher wanted</param>
        /// <returns></returns>
        private KaiTrade.Interfaces.Publisher createPub(string myType)
        {
            KaiTrade.Interfaces.Publisher newPub = null;
            try
            {
                switch (myType)
                {
                    case "PXPublisher":
                        newPub = new KaiTrade.TradeObjects.PXPublisher();
                        break;
                    case "Publisher":
                        newPub = new KaiTrade.TradeObjects.Publisher();
                        break;

                    default:
                        newPub = null;
                        break;
                }
                if (newPub == null)
                {
                    foreach (KaiTrade.Interfaces.K2InstanceFactory factory in m_PublisherFactories)
                    {
                        try
                        {
                            newPub = factory.CreateInstance(myType) as KaiTrade.Interfaces.Publisher;
                        }
                        catch (Exception)
                        {
                        }
                        if (newPub != null)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("createPub", myE);
            }
            return newPub;
        }

        #region PublisherManager Members

        public KaiTrade.Interfaces.Publisher GetPublisher(string myType, string myTopicID, bool register)
        {
            KaiTrade.Interfaces.Publisher myRet = null;

            try
            {
                KaiTrade.Interfaces.Publisher myPub = null;
                if (register)
                {
                    // Does the subject exist in the subject map
                    if (m_PublisherMap.ContainsKey(myTopicID))
                    {
                        myPub = m_PublisherMap[myTopicID];
                    }
                    else
                    {
                        myPub = createPub(myType);

                        myPub.TopicID(myTopicID);
                        m_PublisherMap.Add(myTopicID, myPub);
                    }
                }
                else
                {
                    myPub = createPub(myType);
                    myPub.TopicID(myTopicID);
                }

                return myPub;
            }
            catch (Exception myE)
            {
                m_Log.Error("GetPublisher", myE);
            }

            return myRet;
        }

        /// <summary>
        /// Get a publisher for the topic specified - return null if not found
        /// </summary>
        /// <param name="myTopicID"></param>
        /// <returns>null => not found</returns>
        public KaiTrade.Interfaces.Publisher GetPublisher(string myTopicID)
        {
            KaiTrade.Interfaces.Publisher myRet = null;
            if (m_PublisherMap.ContainsKey(myTopicID))
            {
                myRet = m_PublisherMap[myTopicID];
            }
            return myRet;
        }

        /// <summary>
        /// Get a list of all the available publisher topics
        /// </summary>
        /// <returns>list of topics</returns>
        public List<string> GetPublisherSubjects()
        {
            List<string> subjectList = new List<string>();
            try
            {
                foreach (string subject in m_PublisherMap.Keys)
                {
                    subjectList.Add(subject);
                }
            }
            catch
            {
            }
            return subjectList;
        }

        #endregion
    }
}
