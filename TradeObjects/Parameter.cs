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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class Parameter :KaiTrade.Interfaces.IParameter
    {
        private string m_ParameterName;
        private string m_ParameterGroup = "";
        private string m_ParameterValue;
        private KaiTrade.Interfaces.ATDLType m_ParameterType = KaiTrade.Interfaces.ATDLType.String;

        private bool m_Optional = true;
        private int m_FIXTag = 0;

        public Parameter()
        {
        }

        public Parameter(string name, string value)
        {
            ParameterName = name;
            ParameterType = KaiTrade.Interfaces.ATDLType.String;
            ParameterValue = value;
        }

        public Parameter(string name, KaiTrade.Interfaces.ATDLType type, string value)
        {
            ParameterName = name;
            ParameterType = type;
            ParameterValue = value;
        }

        [DataMember]
        [JsonProperty]
        public string ParameterName
        {
            get
            {
                return m_ParameterName;
            }
            set
            {
                m_ParameterName = value;
            }
        }

        /// <summary>
        /// Parameter group that the parameter belongs to
        /// </summary>
        [DataMember]
        [JsonProperty]
        public string ParameterGroup
        {
            get
            {
                return m_ParameterGroup;
            }
            set
            {
                m_ParameterGroup = value;
            }
        }


        [DataMember]
        [JsonProperty]
        public string ParameterValue
        {
            get
            {
                return m_ParameterValue;
            }
            set
            {
                m_ParameterValue = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public KaiTrade.Interfaces.ATDLType ParameterType
        {
            get
            {
                return m_ParameterType;
            }
            set
            {
                m_ParameterType = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public bool Optional
        {
            get
            {
                return m_Optional;
            }
            set
            {
                m_Optional = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public int FIXTag
        {
            get
            {
                return m_FIXTag;
            }
            set
            {
                m_FIXTag = value;
            }
        }
    }
}
