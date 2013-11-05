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
    /// Defines types of strategy
    /// </summary>
    public enum StrategyType
    {
        user, other
    }
    /// <summary>
    /// Basic state of a strategy
    /// </summary>
    public enum StrategyState
    {
        enter, exit, complete, init, error, cancelled
    }

   /// <summary>
    /// Delegate to indicate that the strategy has changed in some way
   /// </summary>
   /// <param name="strategy"></param>
    public delegate void StrategyChanged(KaiTrade.Interfaces.IStrategy strategy);

   
    /// <summary>
    /// Represents the base data in a strategy
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// Get the Identity of the strategy
        /// </summary>
        string Identity
        {
            get;
            set;
        }

        /// <summary>
        /// Get set AutoCreate, if true then the system will create the strategy if needed without
        /// the user needed to do an explicit add
        /// </summary>
        bool AutoCreate
        { get; set; }

        /// <summary>
        /// Get/Set user's session ID that the order belongs to - note that a user can have N sessions over some time period
        /// </summary>
        string SessionID
        { get; set; }


        /// <summary>
        /// Get/Set user identity (a guid) that the strategy belongs to
        /// </summary>
        string User
        { get; set; }



        /// <summary>
        /// The identifier for a particuar run/entry  of the strategy
        /// can be used to publish status and information from a particular run
        /// for example fills for a particular run.
        /// </summary>
        string RunIdentifier
        { get; set; }


        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID
        /// </summary>
        string CorrelationID
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the type of strategy
        /// </summary>
        StrategyType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Get the state of the strategy
        /// </summary>
        StrategyState State
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the OR Alg used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        string ORAlgorithmName
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the PXAlgorithm used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        string PXAlgorithmName
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set paramters as a string delimited bag of values - used to persist K2Parameters
        /// </summary>
        string ParameterBag
        {
            get;
            set;
        }

        /// <summary>
        /// Get set the list of parameters associated with the strategy
        /// </summary>
        List<IParameter> K2Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the users namer for the strategy - more friendly than the GUID ID
        /// </summary>
        string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// get the identity of the last order submitted for this strategy
        /// </summary>
        string LastOrdIdentity
        {
            get;
            set;
        }
        /// <summary>
        /// Get the product ID associated with the strategy
        /// </summary>
        string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the product mnemonic
        /// </summary>
        string Mnemonic
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the data mnemonic, this is used when accessing data if specified
        /// if this is not use then the base Mnemonic is used for both. It helps when
        /// you want to get data from one venue and trade on another
        /// </summary>
        string DataMnemonic
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the account for the strategy
        /// </summary>
        string Account
        {
            get;
            set;
        }

        /// <summary>
        /// Get the default side of this strategy
        /// </summary>
        string Side
        {
            get;
            set;
        }

        /// <summary>
        /// If the side is Short Sell this needs to be used
        /// to specify where the products are held
        /// </summary>
        string ShortSaleLocate
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default order type
        /// </summary>
        string OrdType
        {
            get;
            set;
        }
        /// <summary>
        /// get/set the default TimeInForce
        /// </summary>
        string TimeInForce
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default date time for the time in force
        /// </summary>
        string TIFDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Set the default qty  for the strategy
        /// </summary>
        double Qty
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the max floor
        /// </summary>
        double? MaxFloor
        {
            get;
            set;
        }

        /// <summary>
        /// Set the default price  for the strategy
        /// </summary>
        double Price
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the stop PX for the strategy - note the behaviour
        /// depends on the type of strategy, in general this works only for
        /// single legs
        /// </summary>
        double StopPx
        {
            get;
            set;
        }

        /// <summary>
        /// Set the qty limit for the strategy
        /// </summary>
        double QtyLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the maximum number of iterations that are allowed in any
        /// run of the strategy - for example how many scalp orders sets
        /// can be placed
        /// </summary>
        int MaxIterations
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the maximum number of times the strategy may be entered - this is reset
        /// when a strategy is loaded or created, unlike max iterations which is the max number of
        /// runs/orders perminted in each entry, this is the max number of times you
        /// can enter the strategy. It defaults to -1 i.e. not limited
        /// </summary>
        int MaxEntries
        {
            get;
            set;
        }

        /// <summary>
        /// Max price for the strategy: max price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        double  MaxPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Min price for the strategy: min price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        double  MinPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if we flatten position on Exit
        /// </summary>
        bool FlattenOnExit
        { get; set; }

        /// <summary>
        /// Determines if we cancels working orders  on Exit
        /// </summary>
        bool CancelOnExit
        { get; set; }

        /// <summary>
        /// determine if we use the strategies start and end times
        /// </summary>
        bool UseStrategyTimes
        { get; set; }

        /// <summary>
        /// Time of day the strategy can run from(start time) - if specified not time limits
        /// </summary>
        DateTime StartTime
        { get; set; }

        /// <summary>
        /// Time of day the strategy can run to(end time) - if specified not time limits
        /// if the strategy is an enter state when the end time passes the strategy will
        /// Exit and obey any exit rules in force
        /// </summary>
        DateTime EndTime
        { get; set; }

        /// <summary>
        /// A signal name to the list of names we support - this
        /// can be used to control what signals are processed by the strategy
        /// </summary>
        /// <returns></returns>
        List<string> SignalNames
        {
            get;
            set;
        }

        string EntryTriggerName
        {
            get;
            set;
        }
        string ExitTriggerName
        {
            get;
            set;
        }

        /// <summary>
        /// get/set whether the strategy will attempt to auto connect the enter and exit
        /// trigger names to constions in the trade venue
        /// </summary>
        bool AutoConnectTrg
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the interval used on conditions
        /// </summary>
        int ConditionInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the enabled flag for the strategy
        /// </summary>
        bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Free format info about the strategy - for example output from an Algo
        /// </summary>
        string Info
        {
            get;
            set;
        }

        /// <summary>
        /// Has the strategy been initialized?
        /// </summary>
        bool Initialized
        {
            get;
            set;
        }
    }
}
