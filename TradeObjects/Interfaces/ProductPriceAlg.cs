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
    public enum PxAlgState
    {
        enter, exit,  init, error
    }
    /// <summary>
    /// This interface defines an algorithm that can be associcated with a object
    /// that reacts to price changes - for example used to set prices on a pairs trader
    /// </summary>
    public interface PriceAlg
    {
        /// <summary>
        /// Get the name of this alg
        /// </summary>
        string Name
        { get;}

        /// <summary>
        /// get the state of the price alg
        /// </summary>
        PxAlgState State
        { get; }

        /// <summary>
        /// Reset the alg to its starting state
        /// </summary>
        void Reset();

        /// <summary>
        /// Enter the strategy - action depends on implimenting class
        /// </summary>
        void Enter();

        /// <summary>
        /// Exit the strategy - action depends on implimenting class
        /// </summary>
        void Exit();

        /// <summary>
        /// Get a setting value by name - used to get alg parameters
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        string GetParameterValue(string myName);

        /// <summary>
        /// Set a alg setting by name, used to set parameters used by the alg
        /// </summary>
        /// <param name="myValue"></param>
        void SetParameterValue(string myName, string myValue);

        /// <summary>
        /// Get a list of avalaible parameters for the alg
        /// </summary>
        /// <returns></returns>
        List<string> GetParameterNames();

        /// <summary>
        /// Clear the algo parameters
        /// </summary>
        void ClearParameters();

        /// <summary>
        /// Set the product that will be tracked
        /// </summary>
        KaiTrade.Interfaces.TradableProduct Product
        { get; set;}

        /// <summary>
        /// Get/Set the strategy used for trades if required
        /// </summary>
        KaiTrade.Interfaces.Strategy Strategy
        { get; set;}

        /// <summary>
        /// Get/Set a single order to be processed - use either strategy or order
        /// </summary>
        KaiTrade.Interfaces.Order Order
        { get; set;}
    }
}
