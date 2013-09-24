using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Provides the data to define a driver/connection we may load
    /// </summary>
    public interface IDriverDef
    {
        string Name { get; set; }
        string Code { get; set; }
        string LoadPath { get; set; }
        bool Enabled { get; set; }


        string RouteCode { get; set; }
        string ConfigPath { get; set; }
        bool ManualStart { get; set; }
        bool LiveMarket { get; set; }
        bool HideDriverUI { get; set; }
        bool AsyncPrices { get; set; }
        bool QueueReplaceRequests { get; set; }

        /*
         *<xs:element ref="IPEndpoint" minOccurs="0" maxOccurs="1" />
        <xs:element ref="UserCredential" minOccurs="0" maxOccurs="1" />
        <xs:element ref="MQExchange" minOccurs="0" maxOccurs="unbounded" />
        <xs:element ref="MQRoutingKey" minOccurs="0" maxOccurs="unbounded" />
        <xs:element ref="UserProperty" minOccurs="0" maxOccurs="unbounded" />
         * 
         * 
         * 
         * <xs:element name="IPEndpoint">
    <xs:complexType>
      <xs:sequence />
      <xs:attribute name="Server" type="xs:string" use="optional" />
      <xs:attribute name="Port" type="xs:unsignedInt" use="required" />
      <xs:attribute name="Name" type="xs:string" use="required" />
      <xs:attribute name="Path" type="xs:string" />
    </xs:complexType>
  </xs:element>

<xs:element name="UserCredential">
    <xs:complexType>
      <xs:sequence />
      <xs:attribute name="userId" type="xs:string" />
      <xs:attribute name="pwd" type="xs:string" />
    </xs:complexType>
  </xs:element>


<xs:element name="MQExchange">
    <xs:complexType>
      <xs:sequence />
      <xs:attributeGroup ref="MQExchangeAttributes" />
    </xs:complexType>
  </xs:element>

  <xs:element name="MQRoutingKey">
    <xs:complexType>
      <xs:sequence />
      <xs:attributeGroup ref="MQRoutingKeyAttributes" />
    </xs:complexType>
  </xs:element>

  <xs:attributeGroup name="MQRoutingKeyAttributes">
    <xs:attribute name="Type" type="xs:string" use="required" />
    <xs:attribute name="Name" type="xs:string" use="required" />
    <xs:attribute name="Exchange" type="xs:string" use="required" />
    <xs:attribute name="Key" type="xs:string" use="required" />
    <xs:attribute name="Queue" type="xs:string"/>
    <xs:attribute name="Enabled" type="xs:boolean" use="required" />
  </xs:attributeGroup>

  <xs:attributeGroup name="MQExchangeAttributes">
    <xs:attribute name="Name" type="xs:string" use="required" />
    <xs:attribute name="Exchange" type="xs:string" use="required" />
    <xs:attribute name="Type" type="xs:string" use="required" />
    <xs:attribute name="Enabled" type="xs:boolean" use="required" />
  </xs:attributeGroup>

         */
    }
}
