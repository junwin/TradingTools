/***************************************************************************
 *    
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
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

using CQG;

namespace KTACQG
{
    public partial class CQGFrm : Form
    {

        private KTACQG m_Adapter;
        public delegate void RegisterSubject(KaiTrade.Interfaces.Publisher myPub, int depthLevels, string requestID);
        public RegisterSubject m_RegisterSubject;

        public delegate void ProcessMessage(KaiTrade.Interfaces.Message myMsg);
        public ProcessMessage m_ProcessMessage;

        // Create a logger for use in this class
        private log4net.ILog m_Log;

        /// <summary>
        /// CQG API instance (CQGCEL).
        /// </summary>
        private CQGCEL m_CQG;

        public CQGFrm()
        {
            try
            {
                InitializeComponent();
                m_Log = log4net.LogManager.GetLogger("Kaitrade");
                m_RegisterSubject = new RegisterSubject(this.doRegisterSubject);
                m_ProcessMessage = new ProcessMessage(this.doProcessMessage);
                // Start connection to CQG
                m_CQG = null;
                //m_CQG = new CQGCEL();
            }
            catch (Exception myE)
            {
            }
            
        }

        public CQGCEL CQGApp
        {
            get
            {
                if (m_CQG == null)
                {
                    m_CQG = new CQGCEL();                    
                }
                return m_CQG;
            }
        }

        public KTACQG Adapter
        {
            get
            {
                return m_Adapter;
            }
            set
            {
                m_Adapter = value;
                chkUseThrottle.Checked = m_Adapter.UseOEThrottle;
            }
        }

        public string Message
        {
            get { return txtMessage.Text; }
            set
            {
                try
                {
                    txtMessage.Text = value;
                }
                catch (Exception myE)
                {
                }
            }
        }

        public void Send(KaiTrade.Interfaces.Message myMsg)
        {
            try
            {
                this.Invoke(this.m_ProcessMessage, new object[] { myMsg });
            }
            catch (Exception myE)
            {
                //m_Log.Error("Send", myE);
            }
        }

        private void doProcessMessage(KaiTrade.Interfaces.Message myMsg)
        {
            try
            {
                //m_Adapter.DoSend(myMsg);
            }
            catch (Exception myE)
            {
                m_Log.Error("Register", myE);
            }
        }

        public void Register(KaiTrade.Interfaces.Publisher myPub, int depthLevels, string requestID)
        {
            try
            {
                this.Invoke(this.m_RegisterSubject, new object[] { myPub, depthLevels, requestID });
            }
            catch (Exception myE)
            {
                //m_Log.Error("doProcessMessage", myE);
            }


        }

        private void doRegisterSubject(KaiTrade.Interfaces.Publisher myPub, int depthLevels, string requestID)
        {
            try
            {
               m_Adapter.DoReg(myPub, depthLevels,  requestID);
            }
            catch (Exception myE)
            {
                m_Log.Error("doRegisterSubject", myE);
            }
        }

        public void UnRegister(KaiTrade.Interfaces.Publisher myPub)
        {
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //this.CQGApp.NewInstrument("EPZ8");
        }

        private void CQGFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                m_Log.Info("CQGFrm_FormClosed");
                m_Adapter = null;
                m_CQG = null;
                m_ProcessMessage = null;
                m_RegisterSubject = null;

            }
            catch (Exception myE)
            {

            }

        }
        private void subscribeProduct(string mySymbol)
        {
            try
            {

                if (mySymbol.Trim().Length > 0)
                {
                    
                    //m_CQG.NewInstrument(mySymbol.Trim());
                    m_CQG.RequestCommodityInstruments(mySymbol.Trim(), eInstrumentType.itFuture, false);
                    txtMessage.Text ="Subscribing:"+mySymbol;

                }
            }
            catch (Exception myE)
            {
                m_Log.Error("subscribeProduct", myE);
                txtMessage.Text = myE.Message;
            }
        }
        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtProduct.Text.Trim().Length > 0)
                {
                    Cursor myOld = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    subscribeProduct(txtProduct.Text.Trim());
                    Cursor.Current = myOld;

                }
            }
            catch (Exception myE)
            {
                m_Log.Error("btnSubscribe_Click", myE);
                txtMessage.Text = myE.Message;
            }
        }

        private void btnGetConditions_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor myOld = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                m_CQG.RequestConditionDefinitions();
                m_CQG.RequestTradingSystemDefinitions();
                Cursor.Current = myOld;
            }
            catch (Exception myE)
            {
                m_Log.Error("", myE);
                txtMessage.Text = myE.Message;
            }
        }

       
        /*
        private void CQGApp_ConditionDefinitionsResolved(CQG.CQGConditionDefinitions myConditions, CQG.CQGError cqg_error)
        {
            try
            {
                foreach (CQG.CQGConditionDefinition myCond in myConditions)
                {
                    cmbConditons.Items.Add(myCond.Name);
                }
            }
            catch (Exception myE)
            {
            }
           

        }
        */

        private void btnBtest_Click(object sender, EventArgs e)
        {
            
        }

        private void CQGFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Log.Info("CQGFrm_FormClosing");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor myOld = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                //m_CQG.RequestConditionDefinitions();
                m_CQG.RequestTradingSystemDefinitions();
                Cursor.Current = myOld;
            }
            catch (Exception myE)
            {
                m_Log.Error("", myE);
                txtMessage.Text = myE.Message;
            }
        }

        private void btnTestProbe_Click(object sender, EventArgs e)
        {
            try{
                m_Adapter.test(txtTestField1.Text, txtTestField2.Text);

            }
            catch
            {
            }
        }

        private void chkUseThrottle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                m_Adapter.UseOEThrottle = chkUseThrottle.Checked;
                m_Adapter.RequiredORIntervalTicks = long.Parse(txtFunc.Text)*(long)10000;
            }
            catch
            {
            }
        }
    }
}