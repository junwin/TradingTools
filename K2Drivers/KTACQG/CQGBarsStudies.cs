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
