using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace DriverBase
{
    public class DriverUtils
    {
        /// <summary>
        /// Convert a string of format CCYYMMDD to a UTC DateTime
        /// </summary>
        /// <param name="myDate"> out parm of DateTime</param>
        /// <param name="strDate"> string format CCYYMMDD</param>
        public static void FromLocalMktDate(out DateTime myDate, string strDate)
        {
            if (strDate.Length >= 8)
            {
                short year = short.Parse(strDate.Substring(0, 4));
                byte month = byte.Parse(strDate.Substring(4, 2));
                byte day = byte.Parse(strDate.Substring(6, 2));
                myDate = new DateTime(year, month, day);
            }
            else
            {
                myDate = new DateTime();
            }
        }
        /// <summary>
        /// Convert a string of format CCYYMMDD to a UTC DateTime
        /// </summary>
        /// <param name="myDate"> out parm of DateTime</param>
        /// <param name="strDate"> string format CCYYMMDD</param>
        /// <param name="strDate"> string format hh:mm:ss</param>
        public static void FromLocalMktDate(out DateTime myDate, string strDate, string strTime)
        {
            if (strDate.Length >= 8)
            {
                short year = short.Parse(strDate.Substring(0, 4));
                byte month = byte.Parse(strDate.Substring(4, 2));
                byte day = byte.Parse(strDate.Substring(6, 2));
                int hh = int.Parse(strTime.Substring(0, 2));
                int mm = int.Parse(strTime.Substring(3, 2));
                int ss = int.Parse(strTime.Substring(6, 2));
                myDate = new DateTime(year, month, day, hh, mm, ss);
            }
            else
            {
                myDate = new DateTime();
            }
        }
        /// <summary>
        /// Get a datetime based on today and some time
        /// </summary>
        /// <param name="myHH">hour</param>
        /// <param name="myMM">minute</param>
        /// <param name="mySS">sec</param>
        /// <returns></returns>
        public static DateTime setTime(int myHH, int myMM, int mySS)
        {
            DateTime myToday = DateTime.Today;
            DateTime myTime = new DateTime(myToday.Year, myToday.Month, myToday.Day, myHH, myMM, mySS);
            return myTime;
        }

        /// <summary>
        /// decode a string containing publisher status back to the enum value
        /// </summary>
        /// <param name="myStatus"></param>
        /// <returns></returns>
        public static KaiTrade.Interfaces.Status DecodeStatusString(string myStatus)
        {
            KaiTrade.Interfaces.Status myRet = KaiTrade.Interfaces.Status.undefined;

            switch (myStatus.ToLower())
            {
                case "loaded":
                    myRet = KaiTrade.Interfaces.Status.loaded;
                    break;
                case "opening":
                    myRet = KaiTrade.Interfaces.Status.opening;
                    break;
                case "open":
                    myRet = KaiTrade.Interfaces.Status.open;
                    break;
                case "closed":
                    myRet = KaiTrade.Interfaces.Status.closed;
                    break;
                case "closing":
                    myRet = KaiTrade.Interfaces.Status.closing;
                    break;
                case "error":
                    myRet = KaiTrade.Interfaces.Status.error;
                    break;
                default:
                    myRet = KaiTrade.Interfaces.Status.undefined;
                    break;
            }
            return myRet;
        }

        public static string GetWebFileAsString(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            // you may need these next two lines to prevent a .net bug
            // System.IO.IOException : Unable to read data from the transport connection: The connection was closed.
            // see http://support.microsoft.com/default.aspx?scid=kb;EN-US;915599
            httpWebRequest.KeepAlive = false;
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.ASCII);
            return streamReader.ReadToEnd();


        }
    }
}
