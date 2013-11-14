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
using System.Linq;
using System.Text;

namespace K2DS
{
    public class K2CurveValues
    {
        private log4net.ILog m_Log;
        string _connectString = "";

        public K2CurveValues()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");
            _connectString = global::K2DS.Properties.Settings.Default.K2DSConnectionString;
        }

        public void Insert(K2DataObjects.CurveValue inCurveValue, bool allowUpdate)
        {
            try
            {

                using (K2DataObjects.PriceBarDataContext db = new K2DataObjects.PriceBarDataContext(_connectString))
                {
                    var dbCurveVals =
                      (from cv in db.CurveValues
                       where cv.Mnemonic == inCurveValue.Mnemonic &&
                        cv.ItemType == inCurveValue.ItemType &&
                        cv.ItemSize == inCurveValue.ItemSize &&
                        cv.TimeStamp == inCurveValue.TimeStamp &&
                        cv.RequestID == inCurveValue.RequestID &&
                        cv.HeaderID == inCurveValue.HeaderID
                       select cv).SingleOrDefault();

                    if (dbCurveVals != null)
                    {
                        if (allowUpdate)
                        {
                            db.CurveValues.DeleteOnSubmit(dbCurveVals);
                            db.SubmitChanges();
                        }
                        else
                        {
                            throw new Exception("bar exists");
                        }
                    }
                    db.CurveValues.InsertOnSubmit(inCurveValue);

                    db.SubmitChanges();

                }

            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }

        public static long RoundTimeStamp(long ticks, long timeGranularity)
        {
            long roundingFactor = timeGranularity * 10000;
            return (ticks / roundingFactor) * roundingFactor;
        }
        public decimal[][] GetCurveValues(string mnemonic, long startTick, int count, List<string> headerIDs)
        {
            try
            {
                Dictionary<string, int> headerIndex = new Dictionary<string, int>();
                int i = 0;
                foreach (string header in headerIDs)
                {
                    if (!headerIndex.ContainsKey(header))
                    {
                        headerIndex.Add(header, i++);
                    }
                }
                List<decimal[]> cvResult = new List<decimal[]>();
                using (K2DataObjects.PriceBarDataContext db = new K2DataObjects.PriceBarDataContext(_connectString))
                {
                    var curveValueSet =
                       (from cv in db.CurveValues
                        where (cv.Mnemonic == mnemonic) && (cv.TimeStamp >= startTick)
                        orderby cv.TimeStamp
                        select cv);

                    
                    long prevTimeStamp = 0;
                    decimal[] headerValues=null;
                    // NEED TO AGGRGATE TO N second chuncks so match bars
                    // ELSE NEED SOME FAST LOOK UP 
                    long roundedTime = 0;
                    foreach (K2DataObjects.CurveValue v in curveValueSet)
                    {
                        roundedTime = RoundTimeStamp(v.TimeStamp, 60000);
  
                        if (prevTimeStamp != roundedTime)
                        {
                            // DONT ADD IF NOT USED i.e. 0 values
                            headerValues = new decimal[headerIDs.Count];
                            cvResult.Add(headerValues);
                            prevTimeStamp = roundedTime;
                            if (cvResult.Count >= count)
                            {
                                break;
                            }
                            
                        }
                        if (headerIndex.ContainsKey(v.HeaderID))
                        {
                            // MARK ROW AS USED
                            if (v.Value > 0)
                            {
                                headerValues[headerIndex[v.HeaderID]] = v.Value;
                            }
                        }
                        if (headerIndex.ContainsKey("TimeStamp"))
                        {
                            if (v.Value > 0)
                            {
                                headerValues[headerIndex["TimeStamp"]] = v.TimeStamp;
                            }
                        }
                    }

                }
                return cvResult.ToArray();
                
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public long GetLastCurveValueTimeTick(string mnemonic, long startTick)
        {
            try
            {

                using (K2DataObjects.PriceBarDataContext db = new K2DataObjects.PriceBarDataContext(_connectString))
                {
                    var curveValueSet =
                       (from cv in db.CurveValues
                        where (cv.Mnemonic == mnemonic) && (cv.TimeStamp >= startTick)
                        orderby cv.TimeStamp
                        select cv).LastOrDefault();



                    return curveValueSet.TimeStamp;
                }

            }
            catch (Exception ex)
            {
            }
            return 0;
        }
       
       
    }
}
