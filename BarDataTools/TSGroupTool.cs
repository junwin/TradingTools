using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace BarDataTools
{
    public class TSGroupTool
    {
        private TSQueryGroup _tsQueryGroup;

        public TSQueryGroup TsQueryGroup
        {
            get { return _tsQueryGroup; }
            set { _tsQueryGroup = value; }
        }
        public void FromXML(string xml)
        {
            _tsQueryGroup = new TSQueryGroup();
            XmlSerializer serilaizer = new XmlSerializer(typeof(TSQueryGroup));
            byte[] byteArray = Encoding.ASCII.GetBytes(xml);
            MemoryStream stream = new MemoryStream(byteArray);
            _tsQueryGroup = (TSQueryGroup)serilaizer.Deserialize(stream);
        }

        public string GetXML()
        {
            string retXML = "";
            try
            {
                XmlSerializer serilaizer = new XmlSerializer(typeof(TSQueryGroup));
                MemoryStream m = new MemoryStream();
                StreamWriter writer = new StreamWriter(m);
                serilaizer.Serialize(writer, _tsQueryGroup);
                writer.Flush();
                byte[] xmlData = m.ToArray();
                writer.Close();
                m = new MemoryStream(xmlData);
                StreamReader reader = new StreamReader(m);
                retXML = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
            }

            return retXML;
        }

        public void SetStartDate(DateTime startDate)
        {
            foreach (TSQueryGroupTSDataSet dataSet in TsQueryGroup.TSDataSet)
            {
                dataSet.StartTime = startDate;
            }
        }

        public void SetEndDate(DateTime endDate)
        {
            foreach (TSQueryGroupTSDataSet dataSet in TsQueryGroup.TSDataSet)
            {
                dataSet.EndTime = endDate;
            }
        }
        public void SetUpdatesEnabled(bool updatesEnabled)
        {
            foreach (TSQueryGroupTSDataSet dataSet in TsQueryGroup.TSDataSet)
            {
                dataSet.UpdatesEnabled = updatesEnabled;
            }
        }


        public void SetRangeType(KaiTrade.Interfaces.TSRangeType rangeType)
        {
            foreach (TSQueryGroupTSDataSet dataSet in TsQueryGroup.TSDataSet)
            {
                dataSet.RangeType = (byte) rangeType;
            }
        }


    }

    
}
