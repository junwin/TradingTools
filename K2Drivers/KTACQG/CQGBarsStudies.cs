using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using CQG;
using K2ServiceInterface;

namespace KTACQG
{
    public partial class KTACQG : DriverBase.DriverBase
    {

        private void getTSBarData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGTimedBars myTimedBars;
            CQGTimedBarsRequest myReq;
            try
            {               
                driverLog.Info("getTSBarData:" + myTSSet.ToString());
                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateTimedBarsRequest();

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }
                myReq.Symbol = instrument.FullName;

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;
                int interval;
                if (isIntraDay(out  interval, myTSSet.Period))
                {
                    myTSSet.IntraDayInterval = interval;

                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpUndefined;
                            break;
                    }
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;

                myReq.SessionsFilter = 0;
                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.Undefined:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }

                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                // depending of the type of request


                myTimedBars = m_CQGHostForm.CQGApp.RequestTimedBars(myReq);

                myTSSet.ExternalID = myTimedBars.Id;
                myTSSet.ExternalRef = myTimedBars;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSBarData completed cqgid:" + myTimedBars.Id.ToString() + ":" + myTimedBars.Status.ToString());
            }
            catch (Exception myE)
            {
                log.Error("getTSBarData", myE);
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSBarData" + myE.Message);

                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }

        private bool isIntraDay(out int interval, KaiTrade.Interfaces.TSPeriod period)
        {
            bool isIntraDay = false;
            interval = 0;
            try
            {

                switch (period)
                {
                    case KaiTrade.Interfaces.TSPeriod.minute:
                        interval = 1;
                        isIntraDay = true;
                        break;
                    case KaiTrade.Interfaces.TSPeriod.five_minute:
                        interval = 5;
                        isIntraDay = true;
                        break;
                    case KaiTrade.Interfaces.TSPeriod.two_minute:
                        interval = 2;
                        isIntraDay = true;
                        break;
                    case KaiTrade.Interfaces.TSPeriod.three_minute:
                        interval = 3;
                        isIntraDay = true;
                        break;

                    case KaiTrade.Interfaces.TSPeriod.fifteen_minute:
                        interval = 15;
                        isIntraDay = true;
                        break;
                    case KaiTrade.Interfaces.TSPeriod.thirty_minute:
                        interval = 30;
                        isIntraDay = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception myE)
            {
            }
            return isIntraDay;
        }


        /// <summary>
        /// Fired when the collection of timed bars (CQGTimedBars) is resolved or
        /// when some error occured during timed bars request processing.
        /// </summary>
        /// <param name="cqg_timed_bars">
        /// Reference to resolved CQGTimedBars
        /// </param>
        /// <param name="cqg_error">
        /// The CQGError object that describes the last error occurred
        /// while processing the TimedBars request or
        /// Nothing/Invalid_Error in case of no error.
        /// </param>
        private void CEL_TimedBarsResolved(CQG.CQGTimedBars cqg_timed_bars, CQG.CQGError cqg_error)
        {
            try
            {
                lock (m_BarToken1)
                {
                    try
                    {
                        driverLog.Info("CEL_TimedBarsResolved:" + cqg_timed_bars.Id.ToString());
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_timed_bars.Id))
                        {
                            mySet = m_TSSets[cqg_timed_bars.Id];

                            if (wireLog.IsInfoEnabled)
                            {
                                wireLog.Info("CEL_TimedBarsResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                            }

                            if (cqg_timed_bars.Status == eRequestStatus.rsSuccess)
                            {
                                mySet.Status = KaiTrade.Interfaces.Status.open;
                                DumpAllData(cqg_timed_bars, mySet);
                            }
                            else
                            {
                                mySet.Status = KaiTrade.Interfaces.Status.error;
                                mySet.Text = cqg_error.Description;
                                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CEL_TimedBarsResolved" + mySet.Text);
                            }
                        }
                        else
                        {
                            driverLog.Info("CEL_TimedBarsResolved:Dataset not found");
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_TimedBarsResolved", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Fired when CQGTimedBar item is added to the end of CQGTimedBars.
        /// </summary>
        /// <param name="cqg_timed_bars">
        /// Reference to changed CQGTimedBars.
        /// </param>
        private void CEL_TimedBarsAdded(CQG.CQGTimedBars cqg_timed_bars)
        {
            try
            {
                lock (m_BarToken1)
                {
                    try
                    {
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        if (m_TSSets.ContainsKey(cqg_timed_bars.Id))
                        {
                            mySet = m_TSSets[cqg_timed_bars.Id];
                            //mySet.Items.Clear();  Done in the dump all
                            if (cqg_timed_bars.Status == eRequestStatus.rsSuccess)
                            {
                                DumpAllData(cqg_timed_bars, mySet);
                            }
                            else
                            {
                                mySet.Text = cqg_timed_bars.LastError.Description;
                            }
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_TimedBarsAdded", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Fired when CQGTimedBar item is updated.
        /// </summary>
        /// <param name="cqg_timed_bars">
        /// Reference to changed CQGTimedBars
        /// </param>
        /// <param name="index_">
        /// Specifies the updated CQGTimedBar index.
        /// </param>
        private void CEL_TimedBarsUpdated(CQG.CQGTimedBars cqg_timed_bars, int index_)
        {
            try
            {
                lock (m_BarToken1)
                {
                    try
                    {
                        // try get the set
                        KaiTrade.Interfaces.ITSSet mySet;
                        //return;
                        if (m_TSSets.ContainsKey(cqg_timed_bars.Id))
                        {
                            mySet = m_TSSets[cqg_timed_bars.Id];
                            if (!mySet.ReportAll)
                            {
                                // they only want added bars - so exit
                                return;
                            }
                            KaiTrade.Interfaces.ITSItem[] bars = new KaiTrade.Interfaces.ITSItem[1];
                            bars[0] = mySet.GetNewItem();

                            CopyBar(bars[0], mySet, cqg_timed_bars[index_], index_);
                            bars[0].SourceActionType = KaiTrade.Interfaces.TSItemSourceActionType.barUpdated;
                            bars[0].DriverChangedData = true;
                            mySet.ReplaceItem(bars[0], index_);

                            BarUpdateClients(mySet.RequestID, bars);
                            //mySet.Changed = true;
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("CEL_TimedBarsUpdated", myE);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Dumps all request data (outputs\parameters\records)
        /// </summary>
        private void DumpAllData(CQG.CQGTimedBars myBars, KaiTrade.Interfaces.ITSSet mySet)
        {
            try
            {
                int exitingItemCount = mySet.Items.Count;
                // Clears all records
                mySet.Items.Clear();

                if (myBars.Count == 0)
                {
                    return;
                }

                KaiTrade.Interfaces.ITSItem[] bars = new KaiTrade.Interfaces.ITSItem[myBars.Count];
                for (int i = 0; i < myBars.Count; i++)
                {
                    // get a new TS item
                    bars[i] = mySet.GetNewItem();
                    CopyBar(bars[i], mySet, myBars[i], i);
                    bars[i].SourceActionType = KaiTrade.Interfaces.TSItemSourceActionType.barAdded;
                    bars[i].ItemType = KaiTrade.Interfaces.TSItemType.time;
                    if (i >= exitingItemCount)
                    {
                        bars[i].DriverChangedData = true;
                    }
                    else if (i >= myBars.Count - 2)
                    {
                        bars[i].DriverChangedData = true;
                    }
                    else
                    {
                        bars[i].DriverChangedData = false;
                    }

                    //this.BarUpdateClients(my
                    mySet.AddItem(bars[i]);
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("TBADD:" + bars[i].ToTabSeparated());
                    }
                }

                // Will enable all bar clients of the driver to handle the bars
                this.BarUpdateClients(mySet.RequestID, bars);

            }
            catch (Exception myE)
            {
                log.Error("DumpAllData", myE);
            }
        }


        /// <summary>
        /// Copies the data from a CQG Timed Bar into a KaiTrade standatd bar
        /// </summary>
        /// <param name="myItem"></param>
        /// <param name="mySet"></param>
        /// <param name="myBar"></param>
        /// <param name="myIndex"></param>
        private void CopyBar(KaiTrade.Interfaces.ITSItem myItem, KaiTrade.Interfaces.ITSSet mySet, CQGTimedBar myBar, long myIndex)
        {
            try
            {
                // get a new TS item
                myItem.ItemType = KaiTrade.Interfaces.TSItemType.time;
                myItem.Index = myIndex;
                myItem.TimeStamp = myBar.Timestamp;
                myItem.Open = myBar.Open;
                myItem.High = myBar.High;
                myItem.Low = myBar.Low;
                myItem.Close = myBar.Close;
                myItem.Mid = myBar.Mid;
                myItem.HLC3 = myBar.HLC3;
                myItem.Avg = myBar.Avg;
                myItem.TrueHigh = myBar.TrueHigh;
                myItem.TrueLow = myBar.TrueLow;
                myItem.Range = myBar.Range;
                myItem.TrueRange = myBar.TrueRange;
                myItem.AskVolume = myBar.AskVolume;
                myItem.BidVolume = myBar.BidVolume;
                myItem.Volume = myBar.ActualVolume;

            }
            catch (Exception myE)
            {
                log.Error("DumpRecord", myE);
            }
        }

        /// <summary>
        /// Dumps all request data (outputs\parameters\records)
        /// </summary>
        private void DumpAllData(CQG.CQGConstantVolumeBars myBars, KaiTrade.Interfaces.ITSSet mySet)
        {
            try
            {
                // Clears all records
                mySet.Items.Clear();

                if (myBars.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < myBars.Count; i++)
                {
                    // get a new TS item
                    KaiTrade.Interfaces.ITSItem myItem = mySet.GetNewItem();
                    DumpRecord(myItem, mySet, myBars[i], i);
                    myItem.SourceActionType = KaiTrade.Interfaces.TSItemSourceActionType.barAdded;
                    myItem.ItemType = KaiTrade.Interfaces.TSItemType.constantVolume;
                    mySet.AddItem(myItem);
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("TBADD:" + myItem.ToTabSeparated());
                    }
                }

                // mark the set as changed - will cause the publisher
                // to do an update or image
                mySet.Added = true;
            }
            catch (Exception myE)
            {
                log.Error("DumpAllData", myE);
            }
        }

        private void DumpRecord(KaiTrade.Interfaces.ITSItem myItem, KaiTrade.Interfaces.ITSSet mySet, CQGConstantVolumeBar myBar, long myIndex)
        {
            try
            {
                // get a new TS item
                myItem.Index = myIndex;
                myItem.TimeStamp = myBar.Timestamp;
                myItem.Open = myBar.Open;
                myItem.High = myBar.High;
                myItem.Low = myBar.Low;
                myItem.Close = myBar.Close;
                myItem.Mid = myBar.Mid;
                myItem.HLC3 = myBar.HLC3;
                myItem.Avg = myBar.Avg;
                myItem.TrueHigh = myBar.TrueHigh;
                myItem.TrueLow = myBar.TrueLow;
                myItem.Range = myBar.Range;
                myItem.TrueRange = myBar.TrueRange;
                myItem.AskVolume = myBar.AskVolume;
                myItem.BidVolume = myBar.BidVolume;
            }
            catch (Exception myE)
            {
                log.Error("DumpRecord:CV", myE);
            }
        }


        /*
         * 
           /// <summary>
        /// Get time series data from CQG custom study
        /// </summary>
        /// <param name="myState"></param>
        private void getTSExpressionData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGExpression myExpression;
            CQGExpressionRequest myReq;
            try
            {
                driverLog.Info("getTSExpressionData:" + myTSSet.ToString());

                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateExpressionRequest();

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }
                //myReq.AddSubExpression(myTSSet.ConditionName, myTSSet.Alias);
                foreach (KaiTrade.Interfaces.ITSExpression myTSExpression in myTSSet.Expressions)
                {
                    string myTemp = myTSExpression.Expression.Replace("DJI", instrument.FullName);
                    myReq.AddSubExpression(myTemp, myTSExpression.Alias);
                }

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;


                if (myTSSet.Period == KaiTrade.Interfaces.TSPeriod.IntraDay)
                {
                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            break;
                    }
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;

                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }


                // depending of the type of request
                myExpression = m_CQGHostForm.CQGApp.RequestExpression(myReq);


                myTSSet.ExternalID = myExpression.Id;
                myTSSet.ExternalRef = myExpression;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSExpressionData completed cqgid:" + myExpression.Id.ToString() + ":" + myExpression.Status.ToString());
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSExpressionData" + myE.Message);
                log.Error("getTSExpressionData", myE);
                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }

          /// <summary>
        /// Get time series data from CQG
        /// </summary>
        /// <param name="myState"></param>
        private void getTSConstantBarData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGConstantVolumeBars myBars;
            CQGConstantVolumeBarsRequest myReq;

            try
            {
                driverLog.Info("getTSConstantBarData:" + myTSSet.ToString());
                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateConstantVolumeBarsRequest();


                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }
                myReq.Symbol = instrument.FullName;

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;



                //constant volume stuff
                myReq.VolumeLevel = (int)myTSSet.VolumeLevel;
                myReq.IncludeFlatTicks = myTSSet.IncludeFlatTicks;
                if (myTSSet.VolumeType == KaiTrade.Interfaces.TSVolumeType.Ticks)
                {
                    myReq.VolumeType = eCvbVolumeType.cvbvtTicks;
                }
                else if (myTSSet.VolumeType == KaiTrade.Interfaces.TSVolumeType.Actual)
                {
                    myReq.VolumeType = eCvbVolumeType.cvbvtActual;
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;

                myReq.SessionsFilter = 0;
                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.Undefined:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }

                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                // depending of the type of request


                myBars = m_CQGHostForm.CQGApp.RequestConstantVolumeBars(myReq);

                myTSSet.ExternalID = myBars.Id;
                myTSSet.ExternalRef = myBars;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSConstantBarData completed cqgid:" + myBars.Id.ToString() + ":" + myBars.Status.ToString());
            }
            catch (Exception myE)
            {
                log.Error("getTSConstantBarData", myE);
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSConstantBarData" + myE.Message);

                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }

       

        /// <summary>
        /// Get time series data from CQG custom study
        /// </summary>
        /// <param name="myState"></param>
        private void getTSStudyData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGCustomStudy myCustomStudy;
            CQGCustomStudyRequest myReq;
            try
            {
                driverLog.Info("getTSStudyData:" + myTSSet.ToString());

                myTSSet.Status = KaiTrade.Interfaces.Status.opening;

                myReq = m_CQGHostForm.CQGApp.CreateCustomStudyRequest(myTSSet.ConditionName);

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);
                if (instrument == null)
                {
                    instrument = GetInstrumentWithMnemonic(myTSSet.Mnemonic);
                    if (instrument == null)
                    {
                        Exception myE = new Exception("Invalid instrument");
                        throw myE;
                    }
                }

                myReq.BaseExpression = instrument.FullName;

                switch (myTSSet.RangeType)
                {
                    case KaiTrade.Interfaces.TSRangeType.IntInt:
                        myReq.RangeStart = myTSSet.IntStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateInt:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.IntEnd;
                        break;
                    case KaiTrade.Interfaces.TSRangeType.DateDate:
                        myReq.RangeStart = myTSSet.DateTimeStart;
                        myReq.RangeEnd = myTSSet.DateTimeEnd;
                        break;
                    default:
                        break;
                }


                myReq.IncludeEnd = myTSSet.IncludeEnd;


                if (myTSSet.Period == KaiTrade.Interfaces.TSPeriod.IntraDay)
                {
                    myReq.IntradayPeriod = myTSSet.IntraDayInterval;
                }
                else
                {
                    switch (myTSSet.Period)
                    {
                        case KaiTrade.Interfaces.TSPeriod.Day:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpDaily;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Week:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpWeekly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Month:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpMonthly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Quarter:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpQuarterly;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.SemiYear:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpSemiannual;
                            break;
                        case KaiTrade.Interfaces.TSPeriod.Year:
                            myReq.HistoricalPeriod = eHistoricalPeriod.hpYearly;
                            break;
                        default:
                            break;
                    }
                }

                // Not used
                myReq.Continuation = eTimeSeriesContinuationType.tsctNoContinuation;
                myReq.EqualizeCloses = true;
                myReq.DaysBeforeExpiration = 0;

                myReq.UpdatesEnabled = myTSSet.UpdatesEnabled;


                myReq.SessionsFilter = (int)myTSSet.TSSessionFilter;

                switch (myTSSet.TSSessionFlags)
                {
                    case KaiTrade.Interfaces.TSSessionFlags.DailySession:
                        myReq.SessionFlags = eSessionFlag.sfDailyFromIntraday;
                        break;
                    default:
                        myReq.SessionFlags = eSessionFlag.sfUndefined;
                        break;
                }

                // depending of the type of request
                myCustomStudy = m_CQGHostForm.CQGApp.RequestCustomStudy(myReq);


                myTSSet.ExternalID = myCustomStudy.Id;
                myTSSet.ExternalRef = myCustomStudy;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);

                driverLog.Info("getTSStudyData completed cqgid" + myCustomStudy.Id.ToString() + ":" + myCustomStudy.Status.ToString());
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSStudyData" + myE.Message);
                log.Error("getTSStudyData", myE);
                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }

         
         * 
         * 
         * 
         * 
         * 
         */
    }
}
