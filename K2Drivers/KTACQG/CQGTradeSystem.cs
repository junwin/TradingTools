
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
        void CQGApp_TradingSystemDefinitionsResolved(CQGTradingSystemDefinitions trdSysDefs, CQGError cqg_error)
        {
            try
            {
                log.Info("CQGApp_TradingSystemDefinitionsResolved:entered");
                foreach (CQGTradingSystemDefinition cqgTrdSystem in trdSysDefs)
                {
                    KaiTrade.TradeObjects.TradingSystem tradeSystem = new KaiTrade.TradeObjects.TradingSystem();
                    tradeSystem.Name = cqgTrdSystem.Name;
                    tradeSystem.TradeSystemBasis = KaiTrade.Interfaces.eTradeSystemBasis.tradeSystem;

                    copyTSDefParameters(tradeSystem, cqgTrdSystem.ParameterDefinitions);

                    foreach (CQGTradingSystemTradeDefinition cqgTradeDef in cqgTrdSystem.TradeDefinitions)
                    {
                        KaiTrade.TradeObjects.SystemTrade systemTrade = new KaiTrade.TradeObjects.SystemTrade();
                        systemTrade.TradeName = cqgTradeDef.Name;
                        systemTrade.Side = getKTAOrderSide(cqgTradeDef.Side);
                        systemTrade.OrdType = getKTAOrderType(cqgTradeDef.Entry.OrderType);

                        foreach (CQGTradeExitDefinition cqgExitTrade in cqgTradeDef.Exits)
                        {
                            KaiTrade.TradeObjects.ExitTrade exitTrade = new KaiTrade.TradeObjects.ExitTrade();
                            exitTrade.ExitTradeName = cqgExitTrade.Name;
                            exitTrade.OrdType = getKTAOrderType(cqgExitTrade.OrderType);
                            exitTrade.Side = "";
                            systemTrade.ExitTrades.Add(exitTrade);
                        }
                        tradeSystem.AddSystemTrade(systemTrade);
                    }

                    this.Facade.AddReplaceTradeSystem(tradeSystem);
                }
            }
            catch (Exception myE)
            {
                log.Error("CQGApp_TradingSystemDefinitionsResolved", myE);
            }
        }

        private void copyTSDefParameters(KaiTrade.TradeObjects.TradingSystem tradeSystem, CQGParameterDefinitions parameterDefintions)
        {
            try
            {
                log.Info("copyTSDefParameters");
                foreach (CQGParameterDefinition cqgParm in parameterDefintions)
                {
                    KaiTrade.Interfaces.IParameter parm = new K2DataObjects.K2Parameter(cqgParm.Name, (cqgParm.DefaultValue as object).ToString());
                    switch (cqgParm.Type)
                    {
                        case eUserFormulaParameterType.ufptDouble:
                            parm.ParameterType = KaiTrade.Interfaces.ATDLType.Float;
                            break;
                        case eUserFormulaParameterType.ufptInt:
                            parm.ParameterType = KaiTrade.Interfaces.ATDLType.Int;
                            break;

                        default:
                            parm.ParameterType = KaiTrade.Interfaces.ATDLType.String;
                            break;
                    }
                    tradeSystem.Parameters.Add(parm);

                }

            }
            catch (Exception myE)
            {
                log.Error("copyTSDefParameters", myE);
            }
        }

        void CQGApp_TradingSystemInsertNotification(CQGTradingSystem cqg_trading_system, CQGTradingSystemInsertInfo cqg_trading_system_insert_info)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {
                        //return;
                        // looks like you have to read the sodding trading system - this just tells you it happened
                        driverLog.Info("CQGApp_TradingSystemInsertNotification:" + cqg_trading_system.Id);
                        //logTradingSystem(cqg_trading_system);
                        //TradingSystemInsert(cqg_trading_system, cqg_trading_system_insert_info);
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemUpdateNotification", myE);
                    }
                }
            }
            catch
            {
            }
        }

        void CQGApp_TradingSystemUpdateNotification(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqg_trading_system_update_info)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {
                        if (wireLog.IsInfoEnabled)
                        {
                            wireLog.Info("CQGApp_TradingSystemUpdateNotification:" + cqg_trading_system.Id);
                            //logTradingSystem(cqg_trading_system);
                            LogCQGTSUdateInfo(cqg_trading_system, cqg_trading_system_update_info);
                        }
                        TradingSystemImage(cqg_trading_system);

                        //TradingSystemUpdate(cqg_trading_system, cqg_trading_system_update_info);
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemUpdateNotification", myE);
                    }
                }
            }
            catch
            {
            }
        }

        void CQGApp_TradingSystemAddNotification(CQGTradingSystem cqg_trading_system, CQGTradingSystemAddInfo cqg_trading_system_add_info)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {

                        driverLog.Info("CQGApp_TradingSystemAddNotification:" + cqg_trading_system.Id);
                        if (wireLog.IsInfoEnabled)
                        {
                            //logTradingSystem(cqg_trading_system);
                        }


                        TradingSystemImage(cqg_trading_system);
                        //TradingSystemAdd(cqg_trading_system, cqg_trading_system_add_info);
                        //TradingSystemUpdate(cqg_trading_system, cqg_trading_system_add_info);
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemAddNotification", myE);
                    }
                }
            }
            catch
            {
            }
        }



        private void logTradingSystem(CQGTradingSystem cqgts)
        {
            try
            {
                string myTemp = string.Format("Log CQGTradingSystem>>> ID={0}  Status={1} numRows={2}", cqgts.Id, cqgts.Status.ToString(), cqgts.TradesRows.Count);
                wireLog.Info(myTemp);


                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqgts.Id))
                {
                    //mySet = m_TSSets[cqgts.Id];
                    if (cqgts.Status == eRequestStatus.rsSuccess)
                    {
                        mySet = m_TSSets[cqgts.Id];
                        myTemp = string.Format("CQGTradingSystem>>> Set found Alias = {0} numItems = {1}", mySet.Alias, mySet.Items.Count);
                        wireLog.Info(myTemp);


                        CQGTradingSystemTradesRows myTradeRows = cqgts.TradesRows;

                        if (myTradeRows.Count == 0)
                        {
                            myTemp = string.Format("CQGTradingSystem>>> No Trade Rows in ts ID={0}", cqgts.Id);
                            wireLog.Info(myTemp);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            logCQGTradeRow(i, myTradeRow);
                        }
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemImage:TradingSystemImage:TSData not found");
                }
                wireLog.Info(">>>>>>>>>End of trade system log<<<<<<<<<<<<<<<");
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemImage", myE);
            }
        }

        private bool m_inTradingSystemImage = false;

        private long getTradeSystemMarkerTag(DateTime timeStamp, int offset)
        {
            //yyyy mm dd hh mm
            long x = offset;
            x += timeStamp.Minute * 100;
            x += timeStamp.Hour * 60 * 100;
            x += timeStamp.Day * 24 * 60 * 100;
            x += timeStamp.Month * 32 * 24 * 60 * 100;
            x += (timeStamp.Year / 100) * 12 * 32 * 24 * 60 * 100;
            return x;
        }



        private void TradingSystemImage2(CQGTradingSystem cqg_trading_system)
        {
            try
            {
                if (m_inTradingSystemImage)
                {
                    // server error
                    throw new Exception("re-entered TradingSystemImage2");
                }
                else
                {
                    m_inTradingSystemImage = true;
                }

                wireLog.Info("TradingSystemImage2: " + cqg_trading_system.Id + " count:" + cqg_trading_system.TradesRows.Count.ToString());

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    mySet.Status = KaiTrade.Interfaces.Status.open;

                    if (cqg_trading_system.Status == eRequestStatus.rsSuccess)
                    {
                        List<KaiTrade.Interfaces.TradeSignal> signals = new List<KaiTrade.Interfaces.TradeSignal>();
                        // Clears all records
                        // We use the tag to track the last time+cqg offset we processed
                        if (mySet.Items.Count > 0)
                        {
                            mySet.Tag = mySet.CurrentItem.Tag;
                        }
                        else
                        {
                            mySet.Tag = getTradeSystemMarkerTag(DateTime.Now, 0);
                            //mySet.Tag = (long)0;
                        }
                        mySet.Items.Clear();
                        mySet.Status = KaiTrade.Interfaces.Status.open;

                        CQGTradingSystemTradesRows myTradeRows = cqg_trading_system.TradesRows;
                        //cqg_trading_system.TradesRows[0][0].Definition.Exits[0]
                        if (myTradeRows.Count == 0)
                        {
                            wireLog.Info("TradingSystemImage2: No Rows - just return " + cqg_trading_system.Id);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            appendCQGTradeRow2(i, ref signals, myTradeRow, mySet);
                        }

                        wireLog.Info("TradingSystemImage2: Set Added = true " + cqg_trading_system.Id);


                        foreach (KaiTrade.Interfaces.TradeSignal signal in signals)
                        {
                            Facade.ProcessTradeSignal(mySet, signal);
                            wireLog.Info("TradingSystemImage2:" + signal.ToString());
                        }

                    }
                    else
                    {
                        mySet.Text = cqg_trading_system.LastError.Description;
                        mySet.Status = KaiTrade.Interfaces.Status.error;
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "TradingSystemImage:TradingSystemImage" + mySet.Text);
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemImage2:TradingSystemImage:TSData not found");
                }


                wireLog.Info("TradingSystemImage2:Exited " + cqg_trading_system.Id);
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemImage2", myE);
            }
            m_inTradingSystemImage = false;
        }

        private void appendCQGTradeRow2(int rowIndex, ref List<KaiTrade.Interfaces.TradeSignal> signals, CQGTradingSystemTradesRow tradeRow, KaiTrade.Interfaces.ITSSet tsSet)
        {
            try
            {


                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];

                    if (systemTrade.IsActive)
                    {
                        appendTradeEntry2(rowIndex, j, ref signals, tsSet, systemTrade, tradeRow.Timestamp, tradeRow.TimestampOffset);

                    }
                    else
                    {
                        appendTradeEntry2(rowIndex, j, ref signals, tsSet, systemTrade, tradeRow.Timestamp, tradeRow.TimestampOffset);

                    }
                }

                //wireLog.Info("appendCQGTradeRow2 - Exit");
            }

            catch (Exception myE)
            {
                log.Error("appendCQGTradeRow2", myE);
            }
        }
        private void appendTradeEntry2(int rowIndex, int tradeIndex, ref List<KaiTrade.Interfaces.TradeSignal> signals, KaiTrade.Interfaces.ITSSet tsSet, CQGTradingSystemTrade systemTrade, DateTime timeStamp, int timeStampOffset)
        {
            try
            {
                KaiTrade.Interfaces.TradeSignal signal;
                if (systemTrade.TradeEntry.Signal)
                {
                    signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(systemTrade.Definition.Name);

                    setSignalData(signal, systemTrade.TradeEntry, systemTrade.Definition.Name, getKTAOrderSide(systemTrade.Definition.Side), timeStamp, timeStampOffset);
                    signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;

                    signals.Add(signal);
                }
                //wireLog.Info("appendTradeEntry2:entry trade>>> " + signal.ToString());

                // process the exits for the trade
                for (int k = 0; k < systemTrade.Definition.Exits.Count; k++)
                {
                    CQGTradeExit myExit = systemTrade.TradeExits.get_ItemByName(systemTrade.Definition.Exits[k].Name);
                    if (myExit.Signal)
                    {
                        signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(myExit.Definition.Name);

                        setSignalData(signal, myExit, timeStamp, timeStampOffset);
                        signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;
                        signal.Mnemonic = tsSet.Mnemonic;
                        signals.Add(signal);
                        //wireLog.Info("appendTradeEntry2:exitloop>>> " + signal.ToString());
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry2", myE);
            }
        }




        private void LogTradingSystemImage2(CQGTradingSystem cqg_trading_system)
        {
            try
            {

                wireLog.Info("LogTradingSystemImage2: " + cqg_trading_system.Id + " count:" + cqg_trading_system.TradesRows.Count.ToString());

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    mySet.Status = KaiTrade.Interfaces.Status.open;

                    if (cqg_trading_system.Status == eRequestStatus.rsSuccess)
                    {
                        List<KaiTrade.Interfaces.TradeSignal> signals = new List<KaiTrade.Interfaces.TradeSignal>();


                        CQGTradingSystemTradesRows myTradeRows = cqg_trading_system.TradesRows;
                        //cqg_trading_system.TradesRows[0][0].Definition.Exits[0]
                        if (myTradeRows.Count == 0)
                        {
                            wireLog.Info("LogTradingSystemImage2: No Rows - just return " + cqg_trading_system.Id);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            logCQGTradeRow2(i, ref signals, myTradeRow, mySet);
                        }

                    }
                    else
                    {

                    }
                }
                else
                {
                    wireLog.Info("logTradingSystemImage2:TradingSystemImage:TSData not found");
                }


                wireLog.Info("logTradingSystemImage2:Exited " + cqg_trading_system.Id);
            }
            catch (Exception myE)
            {
                log.Error("logTradingSystemImage2", myE);
            }

        }


        private void logCQGTradeRow2(int rowIndex, ref List<KaiTrade.Interfaces.TradeSignal> signals, CQGTradingSystemTradesRow tradeRow, KaiTrade.Interfaces.ITSSet tsSet)
        {
            try
            {
                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];
                    try
                    {
                        string origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;
                        string systemTradeInfo = string.Format("row={0} tradeIndex={1}, TimeStamp={2}, TimeStampOffset={3}, Origin={4}, IsActive={5},  IsSet={6}", rowIndex, j, tradeRow.Timestamp, tradeRow.TimestampOffset, origin, systemTrade.IsActive, systemTrade.TradeEntry.Signal);
                        tSLog.Info("logTradeEntry2:" + systemTradeInfo);

                        // process the exits for the trade
                        for (int k = 0; k < systemTrade.Definition.Exits.Count; k++)
                        {
                            CQGTradeExit myExit = systemTrade.TradeExits.get_ItemByName(systemTrade.Definition.Exits[k].Name);
                            string exitTradeInfo = string.Format("row={0} tradeIndex={1}, TimeStamp={2}, TimeStampOffset={3}, Origin={4}, Name={5}, IsActive={6},  IsSet={7}", rowIndex, j, tradeRow.Timestamp, tradeRow.TimestampOffset, origin, myExit.Definition.Name, systemTrade.IsActive, myExit.Signal);
                            tSLog.Info("logTradeEntry2:" + exitTradeInfo);
                        }
                    }
                    catch (Exception myE)
                    {
                        log.Error("logTradeEntry2", myE);
                    }
                }

                //wireLog.Info("appendCQGTradeRow2 - Exit");
            }

            catch (Exception myE)
            {
                log.Error("logCQGTradeRow2", myE);
            }
        }

        private void TradingSystemImage(CQGTradingSystem cqg_trading_system)
        {
            try
            {
                TradingSystemImage2(cqg_trading_system);
            }
            catch (Exception myE)
            {

            }
            return;


            try
            {
                if (m_inTradingSystemImage)
                {
                    // server error
                    throw new Exception("re-entered TradingSystemImage");
                }
                else
                {
                    m_inTradingSystemImage = true;
                }

                wireLog.Info("TradingSystemImage: " + cqg_trading_system.Id);

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    mySet.Status = KaiTrade.Interfaces.Status.open;

                    if (cqg_trading_system.Status == eRequestStatus.rsSuccess)
                    {
                        // Clears all records
                        // We use the tag to track the last time+cqg offset we processed
                        if (mySet.Items.Count > 0)
                        {
                            mySet.Tag = mySet.CurrentItem.Tag;
                        }
                        else
                        {
                            mySet.Tag = getTradeSystemMarkerTag(DateTime.Now, 0);
                            //mySet.Tag = (long)0;
                        }
                        mySet.Items.Clear();
                        mySet.Status = KaiTrade.Interfaces.Status.open;

                        CQGTradingSystemTradesRows myTradeRows = cqg_trading_system.TradesRows;
                        //cqg_trading_system.TradesRows[0][0].Definition.Exits[0]
                        if (myTradeRows.Count == 0)
                        {
                            wireLog.Info("TradingSystemImage: No Rows - just return " + cqg_trading_system.Id);
                            return;
                        }

                        // the rows are the time slices and each row will contain named Trades, there is 1 TSItem
                        // for each row
                        for (int i = 0; i < myTradeRows.Count; i++)
                        {
                            CQGTradingSystemTradesRow myTradeRow = myTradeRows[i];
                            appendCQGTradeRow(i, mySet, myTradeRow);
                        }

                        wireLog.Info("TradingSystemImage: Set Added = true " + cqg_trading_system.Id);
                        mySet.Added = true;
                    }
                    else
                    {
                        mySet.Text = cqg_trading_system.LastError.Description;
                        mySet.Status = KaiTrade.Interfaces.Status.error;
                        this.SendStatusMessage(KaiTrade.Interfaces.Status.open, "TradingSystemImage:TradingSystemImage" + mySet.Text);
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemImage:TradingSystemImage:TSData not found");
                }
                wireLog.Info("TradingSystemImage:Exited " + cqg_trading_system.Id);
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemImage", myE);
            }
            m_inTradingSystemImage = false;
        }

        private void logCQGTradeRow(int rowIndex, CQGTradingSystemTradesRow tradeRow)
        {
            try
            {
                string myTemp = string.Format("Log TradeRow>>> count ={0} TimeStamp={1} ", tradeRow.Count, tradeRow.Timestamp);
                wireLog.Info(myTemp);

                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];

                    logTradeEntry(rowIndex, j, systemTrade);
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry", myE);
            }
        }

        private void logTradeEntry(int rowIndex, int tradeIndex, CQGTradingSystemTrade systemTrade)
        {
            try
            {
                string myTemp = string.Format("Log TradeRow:systemTrade:>>> rowIndex{0} tradeIndex:{1} tradeTime ={2} IsActive={3} DefName={4}  numExits={5}, Offset={6}", rowIndex, tradeIndex, systemTrade.Timestamp, systemTrade.IsActive, systemTrade.Definition.Name, systemTrade.TradeExits.Count, systemTrade.TimestampOffset);
                wireLog.Info(myTemp);


                KaiTrade.Interfaces.TradeSignal signal;
                signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(systemTrade.Definition.Name);

                setSignalData(signal, systemTrade.TradeEntry, systemTrade.Definition.Name, getKTAOrderSide(systemTrade.Definition.Side), systemTrade.Timestamp, systemTrade.TimestampOffset);

                myTemp = string.Format("Log TradeRow:systemTrade:Entry>>> Origin = {0} signal={1} ", systemTrade.Definition.Name, signal.ToString());
                wireLog.Info(myTemp);

                // process the exits for the trade
                for (int k = 0; k < systemTrade.TradeExits.Count; k++)
                {
                    CQGTradeExit myExit = systemTrade.TradeExits[k];

                    myTemp = string.Format("Log TradeRow:systemTrade:Exit>>> Name ={0} signal={1} ", myExit.Definition.Name, myExit.Signal);
                    wireLog.Info(myTemp);

                    signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(myExit.Definition.Name);

                    setSignalData(signal, myExit, systemTrade.Timestamp, systemTrade.TimestampOffset);

                    myTemp = string.Format("Log TradeRow:systemTrade:Exit:>>> Origin ={0} signal={1} ", systemTrade.Definition.Name, signal.ToString());
                    wireLog.Info(myTemp);
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry", myE);
            }
        }





        private void appendCQGTradeRow(int rowIndex, KaiTrade.Interfaces.ITSSet tsSet, CQGTradingSystemTradesRow tradeRow)
        {
            try
            {
                KaiTrade.Interfaces.TSItem tsItem = tsSet.GetNewItem();

                tsItem.Index = tsSet.Items.Count;

                tsItem.TimeStamp = tradeRow.Timestamp;

                tsItem.ConditionName = tsSet.ConditionName;
                tsItem.Mnemonic = tsSet.Mnemonic;

                string myTemp = string.Format("appendCQGTradeRow>>> rowIndex{0} cond={1} Time={2} Index={3} RowCount={4} trTime{5}", rowIndex, tsItem.ConditionName, tsItem.TimeStamp.ToString(), tsItem.Index.ToString(), tradeRow.Count, tradeRow.Timestamp);
                wireLog.Info(myTemp);

                // Each row can have N trades - these have 1 enter and N exits
                for (int j = 0; j < tradeRow.Count; j++)
                {
                    // process the Entry for the Trade
                    CQGTradingSystemTrade systemTrade = tradeRow[j];
                    tsItem.Tag = getTradeSystemMarkerTag(systemTrade.Timestamp, systemTrade.TimestampOffset);
                    if (systemTrade.IsActive)
                    {
                        appendTradeEntry(rowIndex, j, tsSet, tsItem, systemTrade);
                        tsSet.AddItem(tsItem);
                    }
                    else
                    {
                        appendTradeEntry(rowIndex, j, tsSet, tsItem, systemTrade);
                        tsSet.AddItem(tsItem);
                    }
                }
                myTemp = "appendCQGTradeRow - Exit";
                wireLog.Info(myTemp);
            }

            catch (Exception myE)
            {
                log.Error("appendCQGTradeRow", myE);
            }
        }





        private void appendTradeEntry(int rowIndex, int tradeIndex, KaiTrade.Interfaces.ITSSet tsSet, KaiTrade.Interfaces.TSItem tsItem, CQGTradingSystemTrade systemTrade)
        {
            try
            {
                KaiTrade.Interfaces.TradeSignal signal;
                signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(systemTrade.Definition.Name);

                setSignalData(signal, systemTrade.TradeEntry, systemTrade.Definition.Name, getKTAOrderSide(systemTrade.Definition.Side), systemTrade.Timestamp, systemTrade.TimestampOffset);
                signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;

                tsItem.Signals.Add(signal.Origin + ":" + ENTRY_TRADE, signal);

                wireLog.Info("appendTradeEntry:entry trade>>> " + signal.ToString());

                // process the exits for the trade
                for (int k = 0; k < systemTrade.TradeExits.Count; k++)
                {
                    CQGTradeExit myExit = systemTrade.TradeExits[k];

                    signal = m_Facade.Factory.GetTradeSignalManager().CreateSignal(myExit.Definition.Name);

                    setSignalData(signal, myExit, systemTrade.Timestamp, systemTrade.TimestampOffset);
                    signal.Origin = tsSet.Alias + "." + tsSet.ConditionName + "." + systemTrade.Definition.Name;
                    signal.Mnemonic = tsSet.Mnemonic;
                    tsItem.Signals.Add(signal.Origin + ":" + myExit.Definition.Name, signal);
                    wireLog.Info("appendTradeEntry:exitloop>>> " + signal.ToString());
                }
            }
            catch (Exception myE)
            {
                log.Error("appendTradeEntry", myE);
            }
        }



        private void setSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGTradeEntry tradeEntry, string name, string side, DateTime timeStamp, int timeStampOffset)
        {
            try
            {
                // set basic info
                signal.OrdQty = tradeEntry.Quantity;
                signal.Price = tradeEntry.Price;
                signal.StopPrice = tradeEntry.StopLimitPrice;
                signal.Set = tradeEntry.Signal;
                signal.OrdType = getKTAOrderType(tradeEntry.Definition.OrderType);
                //signal.Side = getKTAOrderSide(systemTrade.Definition.Side);
                signal.Side = side;
                signal.Name = name;
                signal.SignalType = KaiTrade.Interfaces.TradeSignalType.enter;
                signal.TimeCreated = timeStamp;
                signal.Text = timeStampOffset.ToString();
                //signal.Mnemonic = tradeEntry.Trade.Definition.Entry.
            }
            catch (Exception myE)
            {
                log.Error("setEntrySignal:entry", myE);
            }
        }

        private void setSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGTradeExit exitTrade, DateTime timeStamp, int timeStampOffset)
        {
            try
            {
                // set basic info
                signal.OrdQty = exitTrade.Quantity;
                signal.Price = exitTrade.Price;
                signal.StopPrice = exitTrade.StopLimitPrice;
                signal.Set = exitTrade.Signal;
                signal.OrdType = getKTAOrderType(exitTrade.Definition.OrderType);
                signal.Side = getKTAOrderSide(exitTrade.Trade.Definition.Side);
                signal.SignalType = KaiTrade.Interfaces.TradeSignalType.exit;
                signal.Name = exitTrade.Definition.Name;
                signal.TimeCreated = timeStamp;
                signal.Text = timeStampOffset.ToString();
            }
            catch (Exception myE)
            {
                log.Error("setEntrySignal:entry", myE);
            }
        }


        private bool updateSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGChangedTradeEntry changedTradeEntry, string name, string side)
        {
            bool changed = false;

            try
            {
                // set basic info
                if (signal.OrdQty != changedTradeEntry.TradeEntry.Quantity)
                {
                    changed = true;
                    signal.OrdQty = changedTradeEntry.TradeEntry.Quantity;
                }
                if (signal.Price != changedTradeEntry.TradeEntry.Price)
                {
                    changed = true;
                    signal.Price = changedTradeEntry.TradeEntry.Price;
                }
                if (signal.StopPrice != changedTradeEntry.TradeEntry.StopLimitPrice)
                {
                    changed = true;
                    signal.StopPrice = changedTradeEntry.TradeEntry.StopLimitPrice;
                }
                if (signal.Set != changedTradeEntry.TradeEntry.Signal)
                {
                    changed = true;
                    signal.Set = changedTradeEntry.TradeEntry.Signal;
                }
                if (signal.OrdType != getKTAOrderType(changedTradeEntry.TradeEntry.Definition.OrderType))
                {
                    changed = true;
                    signal.OrdType = getKTAOrderType(changedTradeEntry.TradeEntry.Definition.OrderType);
                }


                //signal.Side = getKTAOrderSide(systemTrade.Definition.Side);
                signal.Side = side;
                signal.Name = name;
            }
            catch (Exception myE)
            {
                log.Error("updateEntrySignal:entry", myE);
            }

            return changed;
        }


        private bool updateSignalData(KaiTrade.Interfaces.TradeSignal signal, CQGChangedTradeExit changedTradeExit, string name, string side)
        {
            bool changed = false;

            try
            {
                // set basic info
                if (signal.OrdQty != changedTradeExit.TradeExit.Quantity)
                {
                    changed = true;
                    signal.OrdQty = changedTradeExit.TradeExit.Quantity;
                }
                if (signal.Price != changedTradeExit.TradeExit.Price)
                {
                    changed = true;
                    signal.Price = changedTradeExit.TradeExit.Price;
                }
                if (signal.StopPrice != changedTradeExit.TradeExit.StopLimitPrice)
                {
                    changed = true;
                    signal.StopPrice = changedTradeExit.TradeExit.StopLimitPrice;
                }
                if (signal.Set != changedTradeExit.TradeExit.Signal)
                {
                    changed = true;
                    signal.Set = changedTradeExit.TradeExit.Signal;
                }
                if (signal.OrdType != getKTAOrderType(changedTradeExit.TradeExit.Definition.OrderType))
                {
                    changed = true;
                    signal.OrdType = getKTAOrderType(changedTradeExit.TradeExit.Definition.OrderType);
                }


                //signal.Side = getKTAOrderSide(systemTrade.Definition.Side);
                signal.Side = side;
                signal.Name = name;
            }
            catch (Exception myE)
            {
                log.Error("updateEntrySignal:exit", myE);
            }

            return changed;
        }



        private void TradingSystemAdd(CQGTradingSystem cqg_trading_system, CQGTradingSystemAddInfo addInfo)
        {
            try
            {
                wireLog.Info("TradingSystemAdd: " + cqg_trading_system.Id);

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;

                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];

                    // we expect that cqg added rows
                    if (mySet.Items.Count >= cqg_trading_system.TradesRows.Count)
                    {
                        // this is some problem because we see no new rows
                        //throw new Exception("new rows were expected but not found");
                    }

                    for (int i = 0; i < cqg_trading_system.TradesRows.Count; i++)
                    {
                        CQGTradingSystemTradesRow myTradeRow = cqg_trading_system.TradesRows[i];
                        appendCQGTradeRow(i, mySet, myTradeRow);
                    }
                }
                else
                {
                    wireLog.Info("TradingSystemAdd:TSData not found");
                }
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemAdd", myE);
            }
        }




        private void TradingSystemInsert(CQGTradingSystem cqg_trading_system, CQGTradingSystemInsertInfo cqg_trading_system_insert_info)
        {
            try
            {
                wireLog.Info("TradingSystemInsert: " + cqg_trading_system.Id);
                if (cqg_trading_system_insert_info.Index == 0)
                {
                    TradingSystemImage(cqg_trading_system);
                }
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemInsert:TradingSystemImage", myE);
            }
        }


        private bool LogCQGTSUdateInfo(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqgUpdate)
        {
            bool changed = false;
            try
            {
                string myTemp = string.Format("LogCQGTSUdateInfo>>> index={0} id={1} ", cqgUpdate.Index, cqg_trading_system.Id);
                wireLog.Info(myTemp);


                foreach (CQGChangedTradeEntry tradeEntry in cqgUpdate.ChangedEntries)
                {
                    myTemp = string.Format("LogCQGTSUdateInfo:EntryChange>>> ChangeCat={0} id={1} ", tradeEntry.ChangeCategory, cqg_trading_system.Id);
                    wireLog.Info(myTemp);

                    myTemp = string.Format("LogCQGTSUdateInfo:EntryChange:Values>>> Signal={0} Px={1}, Qty={2}, StopPx={3}", tradeEntry.TradeEntry.Signal, tradeEntry.TradeEntry.Price, tradeEntry.TradeEntry.Quantity, tradeEntry.TradeEntry.StopLimitPrice);
                    wireLog.Info(myTemp);
                }



                foreach (CQGChangedTradeExit tradeExit in cqgUpdate.ChangedExits)
                {
                    myTemp = string.Format("LogCQGTSUdateInfo:ExitChange>>> Name={0} ChangeCat={1} id={2} ", tradeExit.TradeExit.Definition.Name, tradeExit.ChangeCategory, cqg_trading_system.Id);
                    wireLog.Info(myTemp);

                    myTemp = string.Format("LogCQGTSUdateInfo:ExitChange:Values>>> Signal={0} Px={1}, Qty={2}, StopPx={3}", tradeExit.TradeExit.Signal, tradeExit.TradeExit.Price, tradeExit.TradeExit.Quantity, tradeExit.TradeExit.StopLimitPrice);
                    wireLog.Info(myTemp);
                }

                myTemp = "<<< LogCQGTSUdateInfo: Ended>>>";
                wireLog.Info(myTemp);
            }
            catch (Exception myE)
            {
                log.Error("LogCQGTSUdateInfo", myE);
            }
            return changed;
        }


        private bool updateTSRow(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqgUpdate, KaiTrade.Interfaces.ITSSet tsSet)
        {
            bool changed = false;
            try
            {
                // There is only 1 entry signal
                wireLog.Info("updateTSRow: " + cqg_trading_system.Id);
                KaiTrade.Interfaces.TSItem tsItem = tsSet.Items[cqgUpdate.Index];
                foreach (CQGChangedTradeEntry tradeEntry in cqgUpdate.ChangedEntries)
                {
                    KaiTrade.Interfaces.TradeSignal signal = tsItem.Signals[ENTRY_TRADE];
                    // these signals all have the name entry
                    switch (tradeEntry.ChangeCategory)
                    {
                        case eTradeChangeCategory.tccPrice:
                            signal.Price = tradeEntry.TradeEntry.Price;
                            break;
                        case eTradeChangeCategory.tccQuantity:
                            signal.OrdQty = tradeEntry.TradeEntry.Quantity;
                            break;
                        case eTradeChangeCategory.tccSignalReset:
                            signal.Set = false;
                            break;
                        case eTradeChangeCategory.tccSignalSet:
                            signal.Set = true;
                            break;
                        case eTradeChangeCategory.tccStopLimitPrice:
                            signal.StopPrice = tradeEntry.TradeEntry.StopLimitPrice;
                            break;

                        default:
                            if (updateSignalData(tsItem.Signals[ENTRY_TRADE], tradeEntry, tsItem.Signals[ENTRY_TRADE].Name, tsItem.Signals[ENTRY_TRADE].Side))
                            {
                                changed = true;
                            }
                            break;
                    }
                }

                foreach (CQGChangedTradeExit tradeExit in cqgUpdate.ChangedExits)
                {
                    // Use the Name on the tradeExit to locate the signal
                    switch (tradeExit.ChangeCategory)
                    {
                        case eTradeChangeCategory.tccPrice:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].Price = tradeExit.TradeExit.Price;
                            break;
                        case eTradeChangeCategory.tccQuantity:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].OrdQty = tradeExit.TradeExit.Quantity;
                            break;
                        case eTradeChangeCategory.tccSignalReset:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].Set = true;
                            break;
                        case eTradeChangeCategory.tccSignalSet:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].Set = false;
                            break;
                        case eTradeChangeCategory.tccStopLimitPrice:
                            tsItem.Signals[tradeExit.TradeExit.Definition.Name].StopPrice = tradeExit.TradeExit.StopLimitPrice;
                            break;

                        default:
                            if (updateSignalData(tsItem.Signals[tradeExit.TradeExit.Definition.Name], tradeExit, tsItem.Signals[tradeExit.TradeExit.Definition.Name].Name, tsItem.Signals[tradeExit.TradeExit.Definition.Name].Side))
                            {
                                changed = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception myE)
            {
                log.Error("updateTSRow", myE);
            }
            return changed;
        }


        private void TradingSystemUpdate(CQGTradingSystem cqg_trading_system, CQGTradingSystemUpdateInfo cqgUpdate)
        {
            try
            {
                wireLog.Info("TradingSystemUpdate: " + cqg_trading_system.Id);

                return;

                // try get the set
                KaiTrade.Interfaces.ITSSet mySet;
                if (m_TSSets.ContainsKey(cqg_trading_system.Id))
                {
                    mySet = m_TSSets[cqg_trading_system.Id];
                    wireLog.Info("TradingSystemUpdate: no items so doing image" + cqg_trading_system.Id);
                    this.TradingSystemImage(cqg_trading_system);
                    //mySet.Changed = true;
                    mySet.Updated = true;
                    
                }
                else
                {
                    wireLog.Info("TradingSystemUpdate:TSData not found");
                }
            }
            catch (Exception myE)
            {
                log.Error("TradingSystemUpdate", myE);
            }
        }


        private CQGTradingSystem lastTradingSystem = null;
        /// <summary>
        /// Called when the trading system resolves
        /// </summary>
        /// <param name="cqg_trading_system"></param>
        /// <param name="cqg_error"></param>
        void CQGApp_TradingSystemResolved(CQGTradingSystem cqg_trading_system, CQGError cqg_error)
        {
            try
            {
                lock (m_TradingSystemToken1)
                {
                    try
                    {
                        driverLog.Info("CQGApp_TradingSystemResolved:" + cqg_trading_system.Id);
                        if (wireLog.IsInfoEnabled)
                        {
                            logTradingSystem(cqg_trading_system);
                        }

                        TradingSystemImage(cqg_trading_system);
                        logTradeSystemParameters(cqg_trading_system);
                        lastTradingSystem = cqg_trading_system;
                    }
                    catch (Exception myE)
                    {
                        log.Error("CQGApp_TradingSystemResolved", myE);
                    }
                }
            }
            catch
            {
            }
        }

        private void logTradeSystemParameters(CQGTradingSystem ts)
        {
            try
            {

                log.Info("logTradeSystemParameters:default:enter:" + ts.Id);

                foreach (CQGParameterDefinition p in ts.Definition.ParameterDefinitions)
                {
                    driverLog.Info("parameters:name:" + p.Name + "Default value:" + p.DefaultValue.ToString());
                    driverLog.Info("parameters:name:" + p.Name + "Param   value:" + ts.Request.Parameter[p.Name].ToString());

                }
                log.Info("setRequestParameters:default:exit:" + ts.Id);

                driverLog.Info("baseExpression:" + ts.Request.BaseExpression);


            }
            catch (Exception myE)
            {
                log.Error("setRequestParameters", myE);
            }
        }
         */

    }
}
