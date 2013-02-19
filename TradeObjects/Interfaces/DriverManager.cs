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

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// The driver manager loads and controls a set of drivers that can be used in
    /// KaiTrade
    /// </summary>
    public interface DriverManager
    {
        /// <summary>
        /// Return a driver given its ID
        /// </summary>
        /// <param name="myName">ID of the desired adapter</param>
        /// <returns></returns>
        Driver GetDriver(string myID);

        /// <summary>
        /// return an array of loaded drivers
        /// </summary>
        /// <returns></returns>
        System.Collections.Generic.List<Driver> GetDrivers();

        /// <summary>
        /// Issue a status request to all drivers - this causes them all to
        /// respond to thier clients
        /// </summary>
        void RequestStautus();

        /// <summary>
        /// Issue a start request to all loaded drivers
        /// </summary>
        void Start();

        /// <summary>
        /// Start an individual driver based on its ID
        /// </summary>
        /// <param name="myID"></param>
        void Start(string myID);

        /// <summary>
        /// Stop all adapters
        /// </summary>
        void Stop();

        /// <summary>
        /// Stop an individual driver based on its ID
        /// </summary>
        /// <param name="myID"></param>
        void Stop(string myID);

        /// <summary>
        /// Restart an individual driver
        /// </summary>
        /// <param name="myID"></param>
        void Restart(string myID);

        /// <summary>
        /// Send a message to all adapters
        /// </summary>
        /// <param name="myMsg"></param>
        void Send(Message myMsg);

        /// <summary>
        /// Dynamically Load a driver from the path specified
        /// </summary>
        /// <param name="myCode">code that will be assigned to the driver - overrides the driver code</param>
        /// <param name="myPath">path to the assembly</param>
        /// <returns></returns>
        Driver DynamicLoad(string myCode, string myPath);

        /// <summary>
        /// Get a subject used for publish subscribe
        /// </summary>
        /// <param name="myType">Type of Subject</param>
        /// <param name="myTopicID">publishers topic - what the publisher is about</param>
        /// <returns> a subject</returns>
        KaiTrade.Interfaces.Publisher GetPublisher(string myType, string myTopicID);

        /// <summary>
        /// get the message helper that must be used with this driver manager
        /// the helper will format the various status messages from a driver
        /// into the form the driver manager wants
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.MessageHelper GetMessageHelper();

        /// <summary>
        /// Apply a driver status message and update any subscribed views
        /// </summary>
        /// <param name="myDSM"></param>
        void ApplyStatus(string myDSMXML);

        /// <summary>
        /// Provide access to the facade
        /// </summary>
        KaiTrade.Interfaces.Facade Facade
        {
            get;
            set;
        }

        /// <summary>
        /// Add a driver definition - this records the existance of a
        /// driver but does not load it.
        /// </summary>
        /// <param name="myDriver"></param>
        void AddDriverDefinition(KAI.kaitns.Driver myDriver);

        /// <summary>
        /// Add a driver to the manager
        /// </summary>
        /// <param name="myDriver"></param>
        void AddDriver(KaiTrade.Interfaces.Driver myDriver);

        /// <summary>
        /// Get the list of driver definitions
        /// </summary>
        /// <param name="myCode"></param>
        /// <returns></returns>
        List<KAI.kaitns.Driver> GetDriverDefinition();

        /// <summary>
        /// Will request any trade systems that the driver supports - note that this
        /// is asyncronous the driver will add any trading systems using the Facade
        /// </summary>
        void RequestTradeSystems();

        /// <summary>
        /// Request any conditions that the driver supports- note that this
        /// is asyncronous the driver will add any conditions using the Facade
        /// </summary>
        void RequestConditions();

        /// <summary>
        /// Display or Hide any UI the driver has
        /// </summary>
        /// <param name="uiVisible">true => show UI, False => hide UI</param>
        void ShowUI(bool uiVisible);
    }
}
