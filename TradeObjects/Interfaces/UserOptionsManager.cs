/***************************************************************************
 *
 *      Copyright (c) 2009 KaiTrade LLC (registered in Delaware)
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
    /// Define the interface for a user options manager - keeps various user settings under control
    /// </summary>
    public interface UserOptionsManager
    {
        /// <summary>
        /// Load the data in the user settings manager from a file
        /// </summary>
        /// <param name="myFilePath"></param>
        void Load(string myFilePath);

        /// <summary>
        /// Save User Settings into a file
        /// </summary>
        /// <param name="myFilePath"></param>
        void Save(string myFilePath);

        /// <summary>
        /// Save on the current file path (or default file path)
        /// </summary>
        void Save();

        /// <summary>
        /// Get the Trade limits
        /// </summary>
        KaiTrade.Interfaces.TradeLimits TradeLimits
        { get; }

        /// <summary>
        /// Get/Set user options databinding
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.UserOptions UserOptions
        {
            get;
            set;
        }
    }
}
