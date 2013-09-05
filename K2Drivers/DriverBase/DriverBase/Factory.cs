﻿/***************************************************************************
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
using K2ServiceInterface;

namespace DriverBase
{
    /// <summary>
    /// Provide access to external objects and the kaitrade facade
    /// </summary>
    public class AppFactory : IFactory
    {
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile AppFactory s_instance;

        /// <summary>
        /// used to lock the class during instantiation
        /// </summary>
        private static object s_Token = new object();

        /// <summary>
        /// Logger
        /// </summary>
        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        /// <summary>
        /// Main facade used by the app
        /// </summary>
        private IFacade m_AppFacade = null;

        /// <summary>
        /// K2ServiceClient - if used
        /// </summary>
        private object m_K2ServiceClient;

        Dictionary<string, K2Depth.K2DOM> _mnemonicDOM;

        Dictionary<string, L1PriceSupport.PXPublisher> _L1Publisher;

        

        public static IFactory Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new AppFactory();
                    }
                }
            }
            return s_instance;
        }

        protected AppFactory()
        {
            m_AppFacade = AppFacade.Instance();
            _mnemonicDOM = new Dictionary<string, K2Depth.K2DOM>();
        }

        public IFacade Facade
        {
            get
            {
                return m_AppFacade;
            }
            set
            {
                m_AppFacade = value;
            }
        }

        public object K2ServiceClient
        {
            get
            {
                return m_K2ServiceClient;
            }
            set
            {
                m_K2ServiceClient = value;
            }
        }




        public KaiTrade.Interfaces.IDOM GetProductDOM(string mnemonic, decimal startPx)
        {
            KaiTrade.Interfaces.IProduct product = AppFacade.Instance().GetProductManager().GetProductMnemonic(mnemonic);
            if (!_mnemonicDOM.ContainsKey(mnemonic))
            {
                K2Depth.K2DOM dom = new K2Depth.K2DOM();
                dom.Create(startPx, (decimal)200, product.TickSize.Value);
                _mnemonicDOM.Add(mnemonic, dom);
                
            }
            return _mnemonicDOM[mnemonic];

        }
        public KaiTrade.Interfaces.IDOM GetProductDOM(string mnemonic)
        {
            if (_mnemonicDOM.ContainsKey(mnemonic))
            {
                return _mnemonicDOM[mnemonic];

            }
            return null;

        }


        public KaiTrade.Interfaces.IPublisher  GetPXPublisher(string mnmonic)
        {
            if (!_L1Publisher.ContainsKey(mnmonic))
            {          
                _L1Publisher.Add(mnmonic, new L1PriceSupport.PXPublisher());
                
            }
            return _L1Publisher[mnmonic];
        }
       

        public void ApplyUpdate(KaiTrade.Interfaces.IPXUpdate update)
        {
            throw new Exception("Not implimented");
        }

        public IPriceAgregator GetPriceAgregator(string name)
        {
            return null;
        }

        public KaiTrade.Interfaces.IClient GetMainMessageHandler()
        {
            return null;
        }
    }
}