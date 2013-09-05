using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace K2DataObjects
{

    /// <summary>
    /// Represents a simple data field used
    /// </summary>
    [DataContract]
    public class Field : KaiTrade.Interfaces.IField
    {
        private string m_Value;
        private string m_ID;
        private bool m_Changed = false;
        private bool m_IsValid = false;

        public Field()
        {
            m_Value = "";
            m_ID = "";
            m_Changed = false;
            m_IsValid = false;
        }

        public Field(string myID, string myValue)
        {
            m_Value = myValue;
            m_ID = myID;
            m_Changed = false;
            m_IsValid = false;
        }

        public Field(string myID)
        {
            m_Value = "";
            m_ID = myID;
            m_Changed = false;
            m_IsValid = false;
        }

        #region Field Members
        [DataMember]
        public string ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
            }
        }
        [DataMember]
        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
                m_Changed = true;
                m_IsValid = true;
            }
        }
        [DataMember]
        public bool Changed
        {
            get
            {
                return m_Changed;
            }
            set
            {
                m_Changed = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return m_IsValid;
            }
        }

        #endregion
    }

}
