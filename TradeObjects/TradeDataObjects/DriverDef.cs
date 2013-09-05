using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2DataObjects
{
    public class DriverDef : KaiTrade.Interfaces.IDriverDef
    {
        private string _name = "";

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _code = "";

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        private string _loadPath = "";

        public string LoadPath
        {
            get { return _loadPath; }
            set { _loadPath = value; }
        }
        private bool _enabled = false;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
    }
}
