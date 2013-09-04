using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace KTASimulator
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SimulatorConfig
    {
        private List<SimulatorProduct> _SimulatorProduct;
        private string _ProductFilePath = "";

       

        public SimulatorConfig()
        {
            _SimulatorProduct = new List<SimulatorProduct>();
        }

        [JsonProperty]
        public string ProductFilePath
        {
            get { return _ProductFilePath; }
            set { _ProductFilePath = value; }
        }

        [JsonProperty]
        public List<SimulatorProduct> SimulatorProduct
        {
            get { return _SimulatorProduct; }
            set { _SimulatorProduct = value; }
        }
    }
}
