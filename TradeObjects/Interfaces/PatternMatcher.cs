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
    /// Delegate to indicate matching has occured
    /// </summary>
    /// <param name="myMatcher"></param>
    public delegate void MatchCompleted(KaiTrade.Interfaces.PatternMatcher myMatcher, bool isPositive);

    /// <summary>
    /// Defines the interface that pattern matching objects used with a TS Set
    /// will use
    /// </summary>
    public interface PatternMatcher
    {
        /// <summary>
        /// Alias for the patternmatcher - used to publish results
        /// </summary>
        string Alias
        { get; set;}

        /// <summary>
        /// Publish name for conditions and events ids
        /// </summary>
        string PublishName
        { get; set;}

        /// <summary>
        /// Add a strategy name to the list the matcher supports publishes to
        /// </summary>
        /// <param name="myName"></param>
        void AddStrategyName(string myName);

        /// <summary>
        /// Called on Set updates
        /// </summary>
        MatchCompleted MatchCompleted
        {
            get;
            set;
        }
        /// <summary>
        /// Get/Set the TSSet the matcher will work on
        /// </summary>
        KaiTrade.Interfaces.TSSet TSSet
        {
            get;
            set;
        }

        /// <summary>
        /// Detect matches in the set associated with the matcher - this will
        /// typically set/publish conditions
        /// </summary>
        void DetectMatches(bool isAdded);

        /// <summary>
        /// matcher name
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// WILL BE DEPRICATED delimited set/express for parameters
        /// </summary>
        string ParameterString
        {
            get;
            set;
        }

        /// <summary>
        /// Paramters requred for the expression
        /// </summary>
        List<KaiTrade.Interfaces.K2Parameter> Parameters
        { get; set; }

        /// <summary>
        /// Get the Atdl that corresponds to this algo - older algos may not support this.
        /// </summary>
        /// <returns></returns>
        string GetAtdl();

        /// <summary>
        /// Child pattern matchers - these can be called as a
        /// hieracrchy
        /// </summary>
        List<KaiTrade.Interfaces.PatternMatcher> Children
        { get;}

        /// <summary>
        /// Array of outputs - specific to the matcher
        /// </summary>
        double[] Output
        { get;}

        /// <summary>
        /// Array of outputs names- specific to the matcher
        /// </summary>
        string[] OutputNames
        { get;}

        /// <summary>
        /// Get a list of the trigger names the pattern matcher publishes - if any
        /// </summary>
        List<string> TriggerNames
        { get;}

        /// <summary>
        /// Set the patern matcher back to its initial state
        /// </summary>
        void Reset();

        /// <summary>
        /// Apply a simple price update - note bar data sets are handled as TS Sets
        /// </summary>
        /// <param name="pxUpdate"></param>
        void ApplyPriceUpdate(KaiTrade.Interfaces.PXUpdate pxUpdate);

        /// <summary>
        /// Get a setting value by name - used to get matcher parameters
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        string GetParameterValue(string myName);

        /// <summary>
        /// Set a alg setting by name, used to set parameters used by the matcher
        /// </summary>
        /// <param name="myValue"></param>
        void SetParameterValue(string myName, string myValue);

        /// <summary>
        /// Get a list of avalaible parameters for the matcher
        /// </summary>
        /// <returns></returns>
        List<string> GetParameterNames();

        /// <summary>
        /// Return the XML rendering of the Matcher
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.PatternMatcher ToXMLDB();

        /// <summary>
        /// Render the matcher in XML
        /// </summary>
        /// <param name="myMatcher"></param>
        void FromXMLDB(KAI.kaitns.PatternMatcher myMatcher);
    }
}
