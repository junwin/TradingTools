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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KTAIB
{
    /// <summary>
    /// Provides a form to host the IB TWS control and to switch calls
    /// to the correct (GUI) thread
    /// </summary>
    public partial class Host : Form
    {
        private KTAIB m_Adapter;

        // this section supports invoking commands on the TDA UI control
        public delegate void RegisterSubject(KaiTrade.Interfaces.IPublisher myPub, int depthLevels, string requestID);
        public RegisterSubject m_SubscribeMD;

        public delegate void RequestTS(KaiTrade.Interfaces.ITSSet myTSSet);
        public RequestTS m_RegisterTS;

        public delegate void ProcessMessage(KaiTrade.Interfaces.IMessage myMsg);
        public ProcessMessage m_ProcessMessage;
        public delegate void Connect(string Server, int Port, int ClientID);
        public Connect m_Connect;
        // Create a logger for use in this class
        private log4net.ILog m_Log;


        public Host()
        {
            InitializeComponent();
            m_Log = log4net.LogManager.GetLogger("Kaitrade");
            m_SubscribeMD = new RegisterSubject(this.doSubscribeMD);
            m_RegisterTS = new RequestTS(this.doRequestTS);
            m_ProcessMessage = new ProcessMessage(this.doProcessMessage);
            m_Connect = new Connect(this.doConnect);
            //timer1.Interval = 0;
            //timer1.Start();
        }

        /// <summary>
        /// Get the TWS control
        /// </summary>
        public AxTWSLib.AxTws TWS
        {
            get
            {
                return axTws1;
            }
        }

        public KTAIB Adapter
        {
            get
            {
                return m_Adapter;
            }
            set
            {
                m_Adapter = value;
            }
        }

        public void TWSConnect(string Server, int Port, int ClientID)
        {
            try
            {
                this.Invoke(this.m_Connect, new object[] {Server, Port, ClientID });
            }
            catch (Exception myE)
            {
                m_Log.Error("Send", myE);
            }
        }
        public void doConnect(string Server, int Port, int ClientID)
        {
            try
            {
                TWS.connect(Server, Port, ClientID);
                TWS.reqCurrentTime();
            }
            catch (Exception myE)
            {
                m_Log.Error("doConnect", myE);
            }
        }

        public void Send(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                this.Invoke(this.m_ProcessMessage, new object[] { myMsg });
            }
            catch (Exception myE)
            {
                m_Log.Error("Send", myE);
            }
        }

        private void doProcessMessage(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                m_Adapter.doSend(myMsg);
            }
            catch (Exception myE)
            {
                m_Log.Error("doProcessMessage", myE);
            }
        }

        public void RequestTSData(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            try
            {
                this.Invoke(this.m_RegisterTS, new object[] { myTSSet });

            }
            catch (Exception myE)
            {
                m_Log.Error("RequestTSData", myE);
            }
        }

        public void doRequestTS(KaiTrade.Interfaces.ITSSet myTSSet)
        {
            try
            {
                m_Adapter.doRequestTS(myTSSet);
            }
            catch (Exception myE)
            {
                m_Log.Error("doRequestTS", myE);
            }
        }

        public void SubscribeMD(KaiTrade.Interfaces.IPublisher myPub, int depthLevels, string requestID)
        {
            try
            {
                this.Invoke(this.m_SubscribeMD, new object[] { myPub, depthLevels, requestID });
            }
            catch (Exception myE)
            {
                m_Log.Error("Register", myE);
            }


        }

        private void doSubscribeMD(KaiTrade.Interfaces.IPublisher myPub, int depthLevels, string requestID)
        {
            try
            {
                m_Adapter.doSubscribeMD(myPub, depthLevels, requestID);
            }
            catch (Exception myE)
            {
                m_Log.Error("doRegister", myE);

            }
        }

        public void UnRegister(KaiTrade.TradeObjects.PXPublisher myPXPub)
        {
        }

        

        private void TDAHost_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_ProcessMessage = null;
            m_SubscribeMD = null;
            m_Adapter = null;
            
        }

        private void Host_Load(object sender, EventArgs e)
        {

        }
    }
}