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

namespace KaiTrade.Interfaces
{
    public interface Subscriber
    {
        /// <summary>
        /// Called by a publisher with a complete set of fields - always the
        /// first message after a a subscriber  subcribes to some topic
        /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
        /// <param name="itemList">list of fields and values</param>
        void OnImage(Publisher mySender, System.Collections.Generic.List<Field> itemList);

       /// <summary>
        /// Called by a publisher when one or more fields value changes
       /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
       /// <param name="itemList">list of changed feilds</param>
        void OnUpdate(Publisher mySender, System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// Called when the subject status changes
        /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
        /// <param name="itemList">list of status fields</param>
        void OnStatusChange(Publisher mySender, System.Collections.Generic.List<Field> itemList);
    }
}
