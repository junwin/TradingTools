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
using System.Linq;
using System.Text;

namespace K2DS
{
    public class Factory
    {
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile Factory s_instance;

        /// <summary>
        /// used to lock the class during instantiation
        /// </summary>
        private static object s_Token = new object();

        //private string m_ConnectString=@"Data Source=JUWIN7\SQLEXPRESS;Initial Catalog=K2DS;Integrated Security=True;Pooling=False";
        //private string m_ConnectString=@"Data Source=10.1.11.15;Initial Catalog=K2DS;User ID=sa;Password=quink1nk";
        private string m_ConnectString = "BANG";
        public static Factory Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new Factory();
                    }
                }
            }
            return s_instance;
        }

        protected Factory()
        {
             
            m_ConnectString = global::K2DS.Properties.Settings.Default.K2DSConnectionString;
        }

        public K2DataObjects.DataContext GetDSContext()
        {
            return new K2DataObjects.DataContext(m_ConnectString);
        }


    }
}
