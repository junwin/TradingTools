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
        /// Request any conditions that the driver supports- note that this
        /// is asyncronous the driver will add any conditions using the Facade
        /// </summary>
        public override void RequestConditions()
        {
            try
            {
                m_CQGHostForm.CQGApp.RequestConditionDefinitions();
            }
            catch (Exception myE)
            {
                log.Error("RequestConditions", myE);
            }
        }

          
         private void getTSConditionData(ref KaiTrade.Interfaces.ITSSet myTSSet)
        {
            CQGCondition myCondition;
            CQGConditionRequest myReq;

            try
            {
                driverLog.Info("getTSConditionData:" + myTSSet.ToString());

                myTSSet.Status = KaiTrade.Interfaces.Status.opening;
                myReq = m_CQGHostForm.CQGApp.CreateConditionRequest(myTSSet.ConditionName);

                // Get the CQG Instrument for the request
                CQGInstrument instrument = GetInstrument(myTSSet.Mnemonic);

                // IF they have given an expression then use that instead of
                // the raw menmonic
                if (myTSSet.Expressions.Count > 0)
                {
                    KaiTrade.Interfaces.ITSExpression myExpression = myTSSet.Expressions[0];
                    string myTemp = "";
                    if (instrument != null)
                    {
                        myTemp = myExpression.Expression.Replace("DJI", instrument.FullName);
                    }
                    else
                    {
                        myTemp = myExpression.Expression;
                    }
                    myReq.BaseExpression = myTemp;
                    driverLog.Info("getTSConditionData:using expression:" + myTemp);
                }
                else
                {
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
                myReq.SessionFlags = eSessionFlag.sfUndefined;

                // depending of the type of request


                myCondition = m_CQGHostForm.CQGApp.RequestCondition(myReq);

                myTSSet.ExternalID = myCondition.Id;
                myTSSet.ExternalRef = myCondition;

                m_TSSets.Add(myTSSet.ExternalID, myTSSet);


                driverLog.Info("getTSConditionData completed cqgid:" + myCondition.Id.ToString() + ":" + myCondition.Status.ToString());
            }
            catch (Exception myE)
            {
                this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "getTSConditionData" + myE.Message);
                log.Error("getTSConditionData", myE);
                myTSSet.Status = KaiTrade.Interfaces.Status.error;
                myTSSet.Text = myE.Message;
            }
        }
          
         

        /// <summary>
        /// Fired when the condition (CQGCondition) is resolved or when some
        /// error has occurred during the condition request processing.
        /// </summary>
        /// <param name="cqg_condition">
        /// Reference to resolved CQGCondition
        /// </param>
        /// <param name="cqg_error">
        /// The CQGError object that describes the last error occurred
        /// while processing the Condition request or
        /// Nothing/Invalid_Error in case of no error.
        /// </param>
        private void CEL_ConditionResolved(CQG.CQGCondition myCondition, CQG.CQGError cqg_error)
        {
            try
            {
                driverLog.Info("CEL_ConditionResolved:" + myCondition.Id.ToString());
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(myCondition.Id))
                {
                    mySet = m_TSSets[myCondition.Id];

                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("CEL_ConditionResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (driverLog.IsInfoEnabled)
                    {
                        driverLog.Info("CEL_ConditionResolved:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                     
                    if (myCondition.Status == eRequestStatus.rsSuccess)
                    {
                        // the last bar is the current bar, the index N has occured before N+1
                        int currentBar = myCondition.Count - 1;
                        Facade.ProcessCondition(mySet, mySet.ConditionName, myCondition[currentBar].Timestamp, myCondition[currentBar].Value);

                        
                    }
                    else
                    {
                        mySet.Status = KaiTrade.Interfaces.Status.error;
                        mySet.Text = cqg_error.Description;
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "CEL_ConditionResolved" + mySet.Text);
                    }
                }
                else
                {
                    driverLog.Info("CEL_ConditionResolved:TSDAta set not found");
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionResolved", myE);
            }
        }

        /// <summary>
        /// fired when a set of conditions are received
        /// </summary>
        /// <param name="myConditions"></param>
        /// <param name="cqg_error"></param>
        private void CEL_ConditionDefinitionsResolved(CQG.CQGConditionDefinitions myConditions, CQG.CQGError cqg_error)
        {
            try
            {
                log.Info("CEL_ConditionDefinitionsResolved:entered");
                foreach (CQG.CQGConditionDefinition myCond in myConditions)
                {
                    //myCond.
                    KaiTrade.TradeObjects.TradingSystem tradeSystem = new KaiTrade.TradeObjects.TradingSystem();

                    // this system will be just based on a condition
                    tradeSystem.TradeSystemBasis = KaiTrade.Interfaces.eTradeSystemBasis.simpleCondition;

                    tradeSystem.Name = myCond.Name;

                    copyTSDefParameters(tradeSystem, myCond.ParameterDefinitions);

                    this.Facade.AddReplaceTradeSystem(tradeSystem);
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionDefinitionsResolved", myE);
            }
            
        }

        /// <summary>
        /// Fired when some condition changes
        /// </summary>
        /// <param name="cqg_condition"></param>
        /// <param name="index_"></param>
        private void CEL_ConditionUpdated(CQGCondition myCondition, int index_)
        {
            try
            {
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(myCondition.Id))
                {
                    mySet = m_TSSets[myCondition.Id];
                    if (!mySet.ReportAll)
                    {
                        // they only want added bars - so exit
                        return;
                    }
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("CEL_ConditionUpdated:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (driverLog.IsInfoEnabled)
                    {
                        driverLog.Info("CEL_ConditionUpdated:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (myCondition.Status == eRequestStatus.rsSuccess)
                    {
                        if (myCondition.Count == 0)
                        {
                            return;
                        }

                        // the last bar is the current bar, the index N has occured before N+1
                        int currentBar = myCondition.Count - 1;
                        Facade.ProcessCondition(mySet, mySet.ConditionName, myCondition[index_].Timestamp, myCondition[index_].Value);
                        
                    }
                    else
                    {
                        mySet.Text = myCondition.LastError.Description;
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionUpdated", myE);
            }
        }

        /// <summary>
        /// Fired when some condition changes
        /// </summary>
        /// <param name="cqg_condition"></param>
        /// <param name="index_"></param>
        private void CEL_ConditionAdded(CQGCondition myCondition)
        {
            try
            {
                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(myCondition.Id))
                {
                    mySet = m_TSSets[myCondition.Id];
                    if (wireLog.IsInfoEnabled)
                    {
                        wireLog.Info("CEL_ConditionAdded:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (driverLog.IsInfoEnabled)
                    {
                        driverLog.Info("CEL_ConditionAdded:TSSetFound:" + mySet.Name + ":" + mySet.Alias + ":" + mySet.Mnemonic);
                    }
                    if (myCondition.Status == eRequestStatus.rsSuccess)
                    {
                        // the last bar is the current bar, the index N has occured before N+1
                        int currentBar = myCondition.Count - 1;
                        Facade.ProcessCondition(mySet, mySet.ConditionName, myCondition[currentBar].Timestamp, myCondition[currentBar].Value);
                        
                    }
                    else
                    {
                        mySet.Text = myCondition.LastError.Description;
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("CEL_ConditionAdded", myE);
            }
        }


          
         */
    }
}
