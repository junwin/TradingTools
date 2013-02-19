//-----------------------------------------------------------------------
// <copyright file="IAccount.cs" company="KaiTrade LLC">
// Copyright (c) 2013, KaiTrade LLC.
//// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>// <author>John Unwin</author>
// <website>https://github.com/junwin/K2RTD.git</website>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines the account type
    /// </summary>
    public enum AccountType { Agency, Proprietary, MarketMaker, none };

    /// <summary>
    ///  define the interface for an account object
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// Unique ID for the Account
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// user identity - not their sign in name
        /// </summary>
        string User
        {
            get;
            set;
        }

        /// <summary>
        /// Venue code the account applies to
        /// </summary>
        string VenueCode
        {
            get;
            set;
        }

        /// <summary>
        /// Get set the long name for this account
        /// </summary>
        string LongName
        {
            get;
            set;
        }

        /// <summary>
        /// Account code used on orders
        /// </summary>
        string AccountCode
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the firm that the account belongs to
        /// </summary>
        string FirmCode
        {
            get;
            set;
        }

        /// <summary>
        /// Set up account from an XML data binding
        /// </summary>
        /// <param name="myOrder"></param>
        void FromXml(string xml);
        //void FromXMLDB(KAI.kaitns.Account myAccount);

        /// <summary>
        /// write account onto an XML data bining
        /// </summary>
        /// <returns></returns>
        string AsXml();
        //KAI.kaitns.Account ToXMLDB();

        /// <summary>
        /// Type of account
        /// </summary>
        AccountType AccountType
        { get; set; }

        /// <summary>
        /// initial margine required for this account - depends on the
        /// number and type of products
        /// </summary>
        decimal InitialMargin
        { get; set; }

        /// <summary>
        /// maintanace margine required for this account - depends on the
        /// number and type of products
        /// </summary>
        decimal MaintMargin
        { get; set; }

        /// <summary>
        /// Net liquidity of the account
        /// </summary>
        decimal NetLiquidity
        { get; set; }

        /// <summary>
        /// NetLiq - initial margin
        /// </summary>
        decimal AvailableFunds
        { get; set; }

        /// <summary>
        /// NetLiq - manint margin
        /// </summary>
        decimal ExcessFunds
        { get; set; }
    }
}
