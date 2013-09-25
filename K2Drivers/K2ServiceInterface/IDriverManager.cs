using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2ServiceInterface
{

    public enum OrderReplaceResult {success, error, replacePending, cancelPending, none };
    

    public interface IDriverManager
    {
    
           /// <summary>
        /// Return a driver given its ID
        /// </summary>
        /// <param name="myName">ID of the desired adapter</param>
        /// <returns></returns>
        IDriver GetDriver(string myID);

        /// <summary>
        /// return an array of loaded drivers
        /// </summary>
        /// <returns></returns>
        System.Collections.Generic.List<IDriver> GetDrivers();

        /// <summary>
        /// Issue a status request to all drivers - this causes them all to
        /// respond to thier clients
        /// </summary>
        void RequestStautus();

        /// <summary>
        /// Issue a start request to all loaded drivers
        /// </summary>
        void Start();

        /// <summary>
        /// Start an individual driver based on its ID
        /// </summary>
        /// <param name="myID"></param>
        void Start(string myID);

        /// <summary>
        /// Stop all adapters
        /// </summary>
        void Stop();

        /// <summary>
        /// Stop an individual driver based on its ID
        /// </summary>
        /// <param name="myID"></param>
        void Stop(string myID);

        /// <summary>
        /// Restart an individual driver
        /// </summary>
        /// <param name="myID"></param>
        void Restart(string myID);

        /// <summary>
        /// Send a message to all adapters
        /// </summary>
        /// <param name="myMsg"></param>
        void Send(KaiTrade.Interfaces.IMessage myMsg);

        /// <summary>
        /// Dynamically Load a driver from the path specified
        /// </summary>
        /// <param name="myCode">code that will be assigned to the driver - overrides the driver code</param>
        /// <param name="myPath">absolute path to the assembly, can be http:\\..</param>
        /// <returns></returns>
        IDriver DynamicLoad(string myPath);


        /// <summary>
        /// Load drivers based on a list of driver defs
        /// </summary>
        /// <param name="driverDefs"></param>
        /// <returns></returns>
        List<IDriver> Load(List<KaiTrade.Interfaces.IDriverDef> driverDefs);

       

        /// <summary>
        /// Provide access to the facade
        /// </summary>
       IFacade Facade
        {
            get;
            set;
        }

        /// <summary>
        /// Add a driver definition - this records the existance of a
        /// driver but does not load it.
        /// </summary>
        /// <param name="myDriver"></param>
        void AddDriverDefinition(KaiTrade.Interfaces.IDriverDef driverDef);

        /// <summary>
        /// Add a driver to the manager
        /// </summary>
        /// <param name="myDriver"></param>
        void AddDriver(IDriver driver);

        /// <summary>
        /// Get the list of driver definitions
        /// </summary>
        /// <param name="myCode"></param>
        /// <returns></returns>
        List<KaiTrade.Interfaces.IDriverDef> GetDriverDefinition();

       
    
    }
}
