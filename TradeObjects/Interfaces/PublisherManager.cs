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

namespace KaiTrade.Interfaces
{
    public enum PublisherType { General,  Price};

    /// <summary>
    /// Defines the interface of an object that can manage a set of publishers
    /// </summary>
    public interface PublisherManager
    {
        /// <summary>
        /// Add/replace a publisher to the manager
        /// </summary>
        /// <param name="myPub"></param>
        void Add(KaiTrade.Interfaces.Publisher myPub);

        /// <summary>
        /// remove a publisher
        /// </summary>
        /// <param name="myPub"></param>
        void Remove(KaiTrade.Interfaces.Publisher myPub);

        /// <summary>
        /// Get the publisher for the TopicID, create one if needs be
        /// </summary>
        /// <param name="myType">Defines the type of subject to Get</param>
        /// <param name="myTopicID">Provides the topic id for the publisher</param>
        /// <param name="register">if true then the publisher is registered with the manager, if not it is simply created</param>
        /// <returns></returns>
        KaiTrade.Interfaces.Publisher GetPublisher(string myType, string myTopicID, bool register);

        /// <summary>
        /// Get a publisher for the topic specified - return null if not found
        /// </summary>
        /// <param name="myTopicID"></param>
        /// <returns>null => not found</returns>
        KaiTrade.Interfaces.Publisher GetPublisher(string myTopicID);

        /// <summary>
        /// Add an instance factory to the publisher manager - this lets 3rd parties
        /// add new publishers
        /// </summary>
        /// <param name="publisherType">price or general</param>
        /// <param name="publisherFactory">factory that can create publishers</param>
        void AddInstanceFactory(PublisherType publisherType, K2InstanceFactory publisherFactory);

        /// <summary>
        /// Get a list of all the available publisher types
        /// </summary>
        /// <returns></returns>
        List<string> GetPublisherTypes();

        /// <summary>
        /// Get a list of all the available publisher topics
        /// </summary>
        /// <returns>list of topics</returns>
        List<string> GetPublisherSubjects();
    }
}
