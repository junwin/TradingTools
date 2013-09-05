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
using System.Text;
using System.Reflection;
using K2ServiceInterface;
using Newtonsoft.Json;

namespace K2Managers
{
    /// <summary>
    /// Used to manage a set of drivers used to communicate with brokers and exchanges
    /// </summary>
    public class DriverManager : PublisherManager, IDriverManager
    {
         /// <summary>
        /// Singleton instance of the DriverManager class
        /// </summary>
        private static volatile DriverManager s_instance;

        /// <summary>
        /// Object we use to lock during singleton instantiation.
        /// </summary>
        private static object s_Token = new object();

        private System.Collections.Hashtable m_LoadedDrivers;

        /// <summary>
        /// Map of driver definitions
        /// </summary>
        private Dictionary<string, KaiTrade.Interfaces.IDriverDef> m_DriverDefinition;

        /// <summary>
        /// Subscribers for updates from the driver manager
        /// </summary>
        //NOT USED? private List<KaiTrade.Interfaces.Subscriber> m_Subscribers;

        /// <summary>
        /// This publisher name - should be unique
        /// </summary>
        //NOT USED? private string m_Name = "KTADriverManager";

        /// <summary>
        /// provides access to the facade
        /// </summary>
        private IFacade m_Facade;

        private log4net.ILog m_WireLog;

        protected DriverManager()
		{
            m_LoadedDrivers = new System.Collections.Hashtable();
            m_DriverDefinition = new Dictionary<string, KaiTrade.Interfaces.IDriverDef>();
            m_WireLog = log4net.LogManager.GetLogger("KaiTradeWireLog");
            m_WireLog.Info("DriverManager Created");
		}

        public static DriverManager Instance()
		{	
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new DriverManager();
                    }
                }
            }
			return s_instance;
		}

        /// <summary>
        /// Add a driver definition - this records the existance of a
        /// driver but does not load it.
        /// </summary>
        /// <param name="myDriver"></param>
        public void AddDriverDefinition(KaiTrade.Interfaces.IDriverDef myDriver)
        {
            try
            {
                if (m_DriverDefinition.ContainsKey(myDriver.Code))
                {
                    m_DriverDefinition[myDriver.Code] = myDriver;
                }
                else
                {
                    m_DriverDefinition.Add(myDriver.Code,myDriver);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("AddDriverDefinition", myE);
            }
        }

        /// <summary>
        /// Get the list of driver definitions
        /// </summary>
        /// <param name="myCode"></param>
        /// <returns></returns>
        public List<KaiTrade.Interfaces.IDriverDef> GetDriverDefinition()
        {
            List<KaiTrade.Interfaces.IDriverDef> myDriverDefs = new List<KaiTrade.Interfaces.IDriverDef>();
            try
            {
                foreach (KaiTrade.Interfaces.IDriverDef myDriverDef in m_DriverDefinition.Values)
                {
                    myDriverDefs.Add(myDriverDef);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetDriverDefinition", myE);
            }
            return myDriverDefs;
        }

        /// <summary>
        /// load a kai driver form the path specified
        /// </summary>
        /// <param name="myPath"></param>
        /// <returns></returns>
        public IDriver DynamicLoad(string myPath)
        {
            IDriver myDriver = null;
            try
            {
                System.Reflection.Assembly myAssy = null;
                if (myPath.ToLower().StartsWith("http:"))
                {
                    myAssy = System.Reflection.Assembly.LoadFrom(myPath);
                }
                else
                {
                    myAssy = System.Reflection.Assembly.LoadFile(myPath);
                }

                System.Type myIF;
                Object oTemp;
                foreach (System.Type myType in myAssy.GetTypes())
                {
                    myIF = myType.GetInterface("IDriver");
                    if (myIF != null)
                    {
                        // Will call the constructor - guess you need to invoke the instance
                        // method
                        oTemp = myAssy.CreateInstance(myType.ToString());
                        myDriver = oTemp as IDriver;
                        if (myDriver != null)
                        {
                            string myName = myDriver.Name;
                            string sID = myDriver.ID;
                            myDriver.SetParent(this);
                            addDriver(sID, myDriver);
                            this.DoUpdate("ADDED", sID);
                        }
                    }
                }
                return myDriver;
            }
            catch (Exception myE)
            {
                m_Log.Error("DynamicLoad", myE);
                return myDriver;
            }
        }

        private void addDriver(string myCode, IDriver myDriver)
        {
            if (m_LoadedDrivers.ContainsKey(myCode))
            {
                m_LoadedDrivers[myCode] = myDriver;
            }
            else
            {
                m_LoadedDrivers.Add(myCode, myDriver);
            }
        }
        private void addDriver( IDriver myDriver)
        {
            myDriver.SetParent(this);
            if (m_LoadedDrivers.ContainsKey(myDriver.ID))
            {
                m_LoadedDrivers[myDriver.ID] = myDriver;
            }
            else
            {
                m_LoadedDrivers.Add(myDriver.ID, myDriver);
            }

            this.DoUpdate("ADDED", myDriver.ID);
        }

        #region DriverManager Members

        IDriver IDriverManager.DynamicLoad(string myCode, string myPath)
        {
            IDriver myDrv = null;
            try
            {
                myDrv = DynamicLoad(myPath);
                //addDriver(myCode, myDrv);

                // update subscribers that we added a driver
                DoUpdate("ADDED", myCode);
            }
            catch (Exception myE)
            {
                m_Log.Error("DriverManager.DynamicLoad", myE);
            }
            return myDrv;
        }

        /// <summary>
        /// Get a loaded driver
        /// </summary>
        /// <param name="myID"></param>
        /// <returns>a driver or null</returns>
        IDriver IDriverManager.GetDriver(string myID)
        {
            IDriver myDriver = null;

            if (m_LoadedDrivers.ContainsKey(myID))
            {
                myDriver = m_LoadedDrivers[myID] as IDriver;
            }

            return myDriver;
        }

        List<IDriver> IDriverManager.GetDrivers()
        {
            System.Collections.Generic.List<IDriver> myDrivers = new System.Collections.Generic.List<IDriver>();
            foreach (IDriver myDrv in m_LoadedDrivers.Values)
            {
                myDrivers.Add(myDrv);
            }
            return myDrivers;
        }

       

        void IDriverManager.RequestStautus()
        {
            try
            {
                foreach (IDriver myDrv in m_LoadedDrivers.Values)
                {
                    try
                    {
                        myDrv.StatusRequest();
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("GetLastStatusAll:driver failed to process" + myDrv.Name, myE);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("RequestStautus", myE);
            }
        }

        void IDriverManager.Send(KaiTrade.Interfaces.IMessage myMsg)
        {
            try
            {
                foreach (IDriver myDrv in m_LoadedDrivers.Values)
                {
                    myDrv.Send(myMsg);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Send", myE);
            }
        }

        /// <summary>
        /// Display or Hide any UI the driver has
        /// </summary>
        /// <param name="uiVisible">true => show UI, False => hide UI</param>
        public void ShowUI(bool uiVisible)
        {
            try
            {
                foreach (IDriver myDrv in m_LoadedDrivers.Values)
                {
                    myDrv.ShowUI(uiVisible);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("ShowUI", myE);
            }
        }

        void IDriverManager.Start()
        {
            try
            {
                foreach (IDriver myDrv in m_LoadedDrivers.Values)
                {
                    m_WireLog.Info("Start driver enter:" + myDrv.ID);
                    Start(myDrv.ID);
                    m_WireLog.Info("Start driver exit:" + myDrv.ID);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Start", myE);
            }
        }

        /// <summary>
        /// Start an individual driver based on its ID
        /// </summary>
        /// <param name="myID"></param>
        public void Start(string myID)
        {
            try
            {
                IDriver myDrv = (this as IDriverManager).GetDriver(myID);

                // try get the driver def
                if (m_DriverDefinition.ContainsKey(myID))
                {
                    // if a vaild driver def exists pass that into the
                    // driver
                    KaiTrade.Interfaces.IDriverDef myDrvDef = m_DriverDefinition[myID];
                    if (myDrvDef.Enabled)
                    {
                        // set the main message handler for the driver
                        myDrv.Register("KTAFacade", this.Facade.Factory.GetMainMessageHandler());
                        myDrv.SetFacade(this.Facade);
                        myDrv.Start(JsonConvert.SerializeObject(myDrvDef));
                    }
                    else
                    {
                        m_Log.Error("Start individual driver:attempt to start non enabled driver:" + myDrvDef.Code);
                    }
                }
                else
                {
                    // there was no driver def use the startstate which is the
                    // path to the config folder
                    myDrv.Start("");
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Start individual driver", myE);
            }
        }

        /// <summary>
        /// Stop an individual driver based on its ID
        /// </summary>
        /// <param name="myID"></param>
        public void Stop(string myID)
        {
            try
            {
                IDriver myDrv = (this as IDriverManager).GetDriver(myID);
                myDrv.Stop();
            }
            catch (Exception myE)
            {
                m_Log.Error("Stop", myE);
            }
        }

        void IDriverManager.Stop()
        {
            try
            {
                foreach (IDriver myDrv in m_LoadedDrivers.Values)
                {
                    myDrv.Stop();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Stop", myE);
            }
        }

        /// <summary>
        /// Restart an individual driver
        /// </summary>
        /// <param name="myID"></param>
        public void Restart(string myID)
        {
            try
            {
                Stop(myID);
                Start(myID);
            }
            catch (Exception myE)
            {
                m_Log.Error("Restart", myE);
            }
        }

        #endregion

        

        #region DriverManager Members

        /// <summary>
        /// Add a driver to the manager
        /// </summary>
        /// <param name="myDriver"></param>
        public void AddDriver(IDriver myDriver)
        {
            try
            {
                addDriver(myDriver);
            }
            catch (Exception myE)
            {
                m_Log.Error("AddDriver", myE);
            }
        }

        public void ApplyStatus(string myDSMXML)
        {
            // apply the update to any views
            this.DoUpdate("DSM", myDSMXML);
        }
        /// <summary>
        /// Provide access to the facade
        /// </summary>
        public IFacade Facade
        {
            get { return m_Facade; }
            set { m_Facade = value; }
        }
        #endregion

    }
}
