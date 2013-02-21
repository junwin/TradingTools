using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarDataTools
{
    public class BarDataUtils
    {
        static public  K2DataObjects.PriceBar GetPriceBar(KaiTrade.Interfaces.ITSItem tsItem)
        {
            K2DataObjects.PriceBar bar = new K2DataObjects.PriceBar();
            bar.High = (decimal) tsItem.High;
            bar.AskVolume = (decimal)tsItem.AskVolume;
            bar.Avg = (decimal)tsItem.Avg;
            bar.BidVolume = (decimal)tsItem.BidVolume;
            bar.Close = (decimal)tsItem.Close;
            bar.Low = (decimal)tsItem.Low;
            bar.Mnemonic = tsItem.Mnemonic;
            bar.Open = (decimal)tsItem.Open;
            bar.Volume = (decimal)tsItem.Volume;
            bar.TimeStamp = tsItem.TimeStamp.Ticks;
            bar.ItemType = (int)tsItem.ItemType;
            bar.ItemSize = 1;
            bar.RequestID = "1";
            return bar;    

        }

        static public void TimeBaseCollapse(Dictionary<long, Dictionary<string, K2DataObjects.CurveValue>> curveValues, K2DataObjects.CurveValue[] inValues, long timeGranularity)
        {  
            try
            {
                foreach (var cv in inValues)
                {
                    if (cv.Value >= 0m)
                    {
                        long roundedTIme = RoundTimeStamp(cv.TimeStamp, timeGranularity);
                        if (!curveValues.ContainsKey(roundedTIme))
                        {
                            curveValues.Add(roundedTIme, new Dictionary<string, K2DataObjects.CurveValue>());
                        }
                        if (curveValues[roundedTIme].ContainsKey(cv.HeaderID))
                        {
                            curveValues[roundedTIme][cv.HeaderID] = cv;
                        }
                        else
                        {
                            curveValues[roundedTIme].Add(cv.HeaderID, cv);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        static public K2DataObjects.CurveValue[] GetCurveValue(KaiTrade.Interfaces.ITSItem tsItem)
        {
            if (tsItem.CurveValues.Length > 0)
            {
                K2DataObjects.CurveValue[] curveVals = new K2DataObjects.CurveValue[tsItem.CurveValues.Length];
                int i = 0;
                foreach (var parm in tsItem.CurveValues)
                {
                    try
                    {
                        K2DataObjects.CurveValue cv = new K2DataObjects.CurveValue();
                        cv.Mnemonic = tsItem.Mnemonic;
                        cv.HeaderID = parm.ParameterName;
                        cv.TimeStamp = tsItem.TimeStamp.Ticks;
                        cv.ItemType = (int)tsItem.ItemType;
                        cv.ItemSize = 1;
                        cv.RequestID = "1";
                        cv.Value = decimal.Parse(parm.ParameterValue);
                        curveVals[i++] = cv;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                return curveVals;
            }
            return null;

        }

        public static long RoundTimeStamp(long ticks)
        {
            return RoundTimeStamp( ticks, 1000);
        }

        public static long RoundTimeStamp(long ticks, long timeGranularity)
        {
            long roundingFactor = timeGranularity * 10000;
            return (ticks / roundingFactor) * roundingFactor;
        }
    }

    
  
}
