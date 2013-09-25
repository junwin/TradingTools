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

        private Dictionary<string, IDriver> _loadedDrivers;

        /// <summary>
        /// Map of driver definitions
        /// </summary>
        private Dictionary<string, KaiTrade.Interfaces.IDriverDef> _driverDefinition;

        /// <summary>
        /// provides access to the facade
        /// </summary>
        private IFacade _facade;

        private string _binPath;

        private log4net.ILog _wireLog;

        protected DriverManager()
		{
            _loadedDrivers = new Dictionary<string, IDriver>();
            _driverDefinition = new Dictionary<string, KaiTrade.Interfaces.IDriverDef>();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
            _wireLog = log4net.LogManager.GetLogger("KaiTradeWireLog");
            _wireLog.Info("DriverManager Created");        
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
                if (_driverDefinition.ContainsKey(myDriver.Code))
                {
                    _driverDefinition[myDriver.Code] = myDriver;
                }
                else
                {
                    _driverDefinition.Add(myDriver.Code,myDriver);
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
                foreach (KaiTrade.Interfaces.IDriverDef myDriverDef in _driverDefinition.Values)
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

        public List<IDriver> Load(List<KaiTrade.Interfaces.IDriverDef> driverDefs)
        {
            List<IDriver> drivers = new List<IDriver>();
            try
            {
                foreach (KaiTrade.Interfaces.IDriverDef def in driverDefs)
                {
                    this.AddDriverDefinition(def);
                    IDriver driver =  this.DynamicLoad(def.LoadPath);
                    drivers.Add(driver);
                }
            }
            catch (Exception ex)
            {
                m_Log.Error("Load", myE);
            }
            return drivers;
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

        /// <summary>
        /// Called if we need to resolve some dependancy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private System.Reflection.Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            System.Reflection.Assembly assy4 = null;
            try
            {
                // attempt to load from the binpath or the requesting assmebly path
                string[] assyDef = args.Name.Split(',');
                string[] reqAssyDef = args.RequestingAssembly.FullName.Split(',');
                int reqDllNamePos = args.RequestingAssembly.Location.IndexOf(reqAssyDef[0]);
                string reqDllPath = args.RequestingAssembly.Location.Substring(0, reqDllNamePos - 1);
                string DllPath = "";
                if (_binPath.Length > 0)
                {
                    DllPath = _binPath + @"\" + assyDef[0] + ".dll";
                }
                else
                {
                    DllPath = reqDllPath + @"\" + assyDef[0] + ".dll";
                }

                assy4 = System.Reflection.Assembly.LoadFrom(DllPath);
            }
            catch
            {
            }
            return assy4;
        }

        private void addDriver(string myCode, IDriver myDriver)
        {
            if (_loadedDrivers.ContainsKey(myCode))
            {
                _loadedDrivers[myCode] = myDriver;
            }
            else
            {
                _loadedDrivers.Add(myCode, myDriver);
            }
        }
        private void addDriver( IDriver myDriver)
        {
            myDriver.SetParent(this);
            if (_loadedDrivers.ContainsKey(myDriver.ID))
            {
                _loadedDrivers[myDriver.ID] = myDriver;
            }
            else
            {
                _loadedDrivers.Add(myDriver.ID, myDriver);
            }

            this.DoUpdate("ADDED", myDriver.ID);
        }

        #region DriverManager Members

      

        /// <summary>
        /// Get a loaded driver
        /// </summary>
        /// <param name="myID"></param>
        /// <returns>a driver or null</returns>
        IDriver IDriverManager.GetDriver(string myID)
        {
            IDriver myDriver = null;

            if (_loadedDrivers.ContainsKey(myID))
            {
                myDriver = _loadedDrivers[myID] as IDriver;
            }

            return myDriver;
        }

        List<IDriver> IDriverManager.GetDrivers()
        {
            System.Collections.Generic.List<IDriver> myDrivers = new System.Collections.Generic.List<IDriver>();
            foreach (IDriver myDrv in _loadedDrivers.Values)
            {
                myDrivers.Add(myDrv);
            }
            return myDrivers;
        }

       

        void IDriverManager.RequestStautus()
        {
            try
            {
                foreach (IDriver myDrv in _loadedDrivers.Values)
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
                foreach (IDriver myDrv in _loadedDrivers.Values)
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
                foreach (IDriver myDrv in _loadedDrivers.Values)
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
                foreach (IDriver myDrv in _loadedDrivers.Values)
                {
                    _wireLog.Info("Start driver enter:" + myDrv.ID);
                    Start(myDrv.ID);
                    _wireLog.Info("Start driver exit:" + myDrv.ID);
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
                if (_driverDefinition.ContainsKey(myID))
                {
                    // if a vaild driver def exists pass that into the
                    // driver
                    KaiTrade.Interfaces.IDriverDef myDrvDef = _driverDefinition[myID];
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
                foreach (IDriver myDrv in _loadedDrivers.Values)
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
            get { return _facade; }
            set { _facade = value; }
        }
        #endregion

    }
}
