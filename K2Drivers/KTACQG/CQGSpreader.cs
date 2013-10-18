
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

        private void submitSpreadOrder(KaiTrade.Interfaces.IMessage myMsg, string execPatternName)
        {
            QuickFix.Message myQFOrder = null;
            try
            {

                // Extract the raw FIX Message from the inbound message
                string strOrder = myMsg.Data;
                if (wireLog.IsInfoEnabled)
                {
                    wireLog.Info("submitOrder:" + strOrder);
                }
                if (driverLog.IsInfoEnabled)
                {
                    driverLog.Info("submitOrder:" + strOrder);
                }

                // Use QuickFix to handle the message
                myQFOrder = new QuickFix.Message(strOrder);

                // Use product manager to validate the product specified on
                // the order exists for this adapter

                // Get the product associated with the FIX message


                QuickFix.Symbol symbol = new QuickFix.Symbol();
                QuickFix.Side side = new QuickFix.Side();
                QuickFix.OrdType ordType = new QuickFix.OrdType();
                QuickFix.OrderQty orderQty = new QuickFix.OrderQty();
                QuickFix.Price price = new QuickFix.Price();
                QuickFix.StopPx stopPx = new QuickFix.StopPx();
                QuickFix.Account account = new QuickFix.Account();
                QuickFix.StrikePrice strikePrice = new QuickFix.StrikePrice();
                QuickFix.Currency currency = new QuickFix.Currency();
                QuickFix.CFICode cfiCode = new QuickFix.CFICode();
                QuickFix.SecurityExchange exchange = new QuickFix.SecurityExchange();
                QuickFix.ClOrdID clOrdID = new QuickFix.ClOrdID();
                QuickFix.TimeInForce tif = new QuickFix.TimeInForce();
                QuickFix.ExpireDate expireDate = new QuickFix.ExpireDate();
                QuickFix.MaturityMonthYear MMY = new QuickFix.MaturityMonthYear();
                QuickFix.TargetStrategy targetStrategy = new QuickFix.TargetStrategy();

                // Get the CQG account
                CQGAccount myAccount = null;
                if (myQFOrder.isSetField(account))
                {
                    myQFOrder.getField(account);
                    myAccount = GetAccount(account.getValue());
                    if (myAccount != null)
                    {
                        driverLog.Info("CQGAcct:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
                    }
                }
                if (myAccount == null)
                {
                    this.SendAdvisoryMessage("CQG: you need to provide a valid account");
                    throw new NullReferenceException("No account is selected.");
                }
                CQGInstrument instrument = null;
                // We should now use the src
                QuickFix.SecurityID mySecID = new QuickFix.SecurityID();
                QuickFix.SecurityIDSource mySecIDSrc = new QuickFix.SecurityIDSource();
                if (myQFOrder.isSetField(mySecID))
                {
                    myQFOrder.getField(mySecID);
                    instrument = GetInstrument(mySecID.getValue());
                }
                if (instrument == null)
                {
                    // Get the CQG product/intrument we want to order
                    string myMnemonic = KaiUtil.QFUtils.GetProductMnemonic(m_ID, "", myQFOrder);

                    instrument = GetInstrumentWithMnemonic(myMnemonic);
                }
                if (instrument == null)
                {
                    this.SendAdvisoryMessage("CQG: invalid product/instrument requested - check your product sheet");
                    throw new NullReferenceException("No instrument is selected.");
                }


                CQGStrategyDefinition sd = GetCQGStrategyDefinition(mySecID.getValue());

                if (sd == null)
                {
                    throw new Exception("Can not access a strategy Definition");
                }

                // Get the Order type
                myQFOrder.getField(ordType);
                eOrderType orderType = getOrderType(ordType);


                // Get the QTY
                myQFOrder.getField(orderQty);
                int quantity = (int)orderQty.getValue();

                // Order side can be specified in two ways:
                //        * if UseOrderSide is set in APIConfig then we need to specify side
                //        explicitly, and order quantity must be greater than 0.
                //      * if setting below is not set the side is detected by order quantity sign.
                //        Negative quantity specifies sell side.
                // Here we use the second one
                myQFOrder.getField(side);
                eOrderSide orderSide = getOrderSide(side);

                if (orderSide == eOrderSide.osdSell)
                {
                    quantity *= -1;
                }

                // Default values of prices
                double limitPrice = 0.0;
                if (myQFOrder.isSetField(price))
                {
                    myQFOrder.getField(price);
                    limitPrice = price.getValue();
                }

                double stopPrice = 0.0;
                if ((myQFOrder.isSetField(stopPx)) && ((orderType == eOrderType.otStop) || (orderType == eOrderType.otStopLimit)))
                {
                    myQFOrder.getField(stopPx);
                    stopPrice = stopPx.getValue();
                    limitPrice = 0;
                }


                // Create order, since we have already all the needed parameters

                CQGOrder order = m_CQGHostForm.CQGApp.CreateStrategyOrder(orderType, sd, myAccount, null, quantity, eOrderSide.osdUndefined, limitPrice, stopPrice, "");
                // Set order parked status

                order.Properties[eOrderProperty.opParked].Value = false;


                // Set order duration
                eOrderDuration durationType = eOrderDuration.odDay;
                if (myQFOrder.isSetField(tif))
                {
                    myQFOrder.getField(tif);
                    durationType = getOrderDuration(tif);
                    order.Properties[eOrderProperty.opDurationType].Value = durationType;
                    if (durationType == eOrderDuration.odGoodTillDate)
                    {
                        if (myQFOrder.isSetField(expireDate))
                        {
                            myQFOrder.getField(expireDate);

                            DateTime myDate;
                            KaiUtil.KaiUtil.FromLocalMktDate(out  myDate, expireDate.getValue());

                            order.Properties[eOrderProperty.opGTDTime].Value = myDate;
                        }
                        else
                        {
                            this.SendAdvisoryMessage("CQG: no expire date given on a Good till date order");
                            throw new NullReferenceException("CQG: no expire date given on a Good till date order");
                        }
                    }
                }

                if (myQFOrder.isSetField(targetStrategy))
                {
                    myQFOrder.getField(targetStrategy);
                }
                string execPatternKey = getExecPatternKey(execPatternName, KaiUtil.QFUtils.DecodeOrderType(ordType));
                string execPattern = "";
                if (m_ExecutionPatterns.ContainsKey(execPatternKey))
                {
                    execPattern = m_ExecutionPatterns[execPatternKey];

                }
                else if (m_ExecutionPatterns.ContainsKey(execPatternName))
                {
                    // try the bare name i.e. with out the .LIMIT or .MARKET post fix
                    execPattern = m_ExecutionPatterns[execPatternName];

                }

                driverLog.Info("ExecPatternKey:" + execPatternKey + " stringis:" + execPattern);

                CQGExecutionPattern debugCqgEP = m_CQGHostForm.CQGApp.CreateExecutionPattern(sd, orderType);
                string debugExecPattern = debugCqgEP.PatternString;
                driverLog.Info("CQG DefaultExecPatternKey:" + debugExecPattern);

                if (execPattern.Length == 0)
                {
                    CQGExecutionPattern cqgEP = m_CQGHostForm.CQGApp.CreateExecutionPattern(sd, orderType);
                    execPattern = cqgEP.PatternString;
                }
                order.Properties[eOrderProperty.opExecutionPattern].Value = execPattern;

                // Record the CQG order
                myQFOrder.getField(clOrdID);
                DriverBase.OrderContext myContext = new DriverBase.OrderContext();
                myContext.ExternalOrder = order;
                myContext.QFOrder = myQFOrder;
                myContext.ClOrdID = clOrdID.getValue();


                m_GUID2CQGOrder.Add(order.GUID, myContext);
                m_ClOrdIDOrderMap.Add(clOrdID.getValue(), myContext);

                // send the order
                driverLog.Info("CQGAcctA:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
                order.Place();
                myContext.CurrentCommand = DriverBase.ORCommand.Submit;
                driverLog.Info("CQGAcctB:" + myAccount.GWAccountID.ToString() + ":" + myAccount.GWAccountName.ToString() + ":" + myAccount.FcmID.ToString());
            }
            catch (Exception myE)
            {
                log.Error("submitOrder", myE);
                // To provide the end user with more information
                // send an advisory message, again this is optional
                // and depends on the adpater
                this.SendAdvisoryMessage("CQG:submitOrder: problem submitting order:" + myE.ToString());

                QuickFix.OrdStatus myOrdStatus;
                QuickFix.ExecType myExecType = new QuickFix.ExecType(QuickFix.ExecType.REJECTED);

                myOrdStatus = new QuickFix.OrdStatus(QuickFix.OrdStatus.REJECTED);
                QuickFix.OrderQty orderQty = new QuickFix.OrderQty();
                if (myQFOrder != null)
                {
                    myQFOrder.getField(orderQty);
                    QuickFix.OrdRejReason myRejReason = new QuickFix.OrdRejReason(QuickFix.OrdRejReason.OTHER);
                    sendExecReport(myQFOrder, new QuickFix.OrderID("UNKNOWN"), myOrdStatus, myExecType, 0.0, (int)orderQty.getValue(), 0, 0, 0, myE.Message, myRejReason);
                }
            }
        }

        private string getExecPatternKey(string name, string ordType)
        {
            return name + "." + ordType;
        }
        private string getExecStrategyPattern(CQGStrategyDefinition sd, eOrderType orderType, string name)
        {
            string execStrategyPattern = "";

            string key = "";
            switch (orderType)
            {
                case eOrderType.otLimit:
                    key = KaiTrade.Interfaces.IOrderType.LIMIT + "." + name;
                    break;
                case eOrderType.otMarket:
                    key = KaiTrade.Interfaces.IOrderType.MARKET + "." + name;
                    break;
                case eOrderType.otStop:
                    key = KaiTrade.Interfaces.IOrderType.STOP + "." + name;
                    break;
                case eOrderType.otStopLimit:
                    key = KaiTrade.Interfaces.IOrderType.STOP + "." + name;
                    break;

            }
            if (m_ExecutionPatterns.ContainsKey(key))
            {
                execStrategyPattern = m_ExecutionPatterns[key];
            }
            else
            {
                CQGExecutionPattern cqgEP = m_CQGHostForm.CQGApp.CreateExecutionPattern(sd, orderType);
                execStrategyPattern = cqgEP.PatternString;
                m_ExecutionPatterns.Add(key, execStrategyPattern);
            }
            return execStrategyPattern;

        }

        public void AddExecStrategyPattern(string orderType, string name, string execStrategyPattern)
        {
            string key = orderType + "." + name;
            if (m_ExecutionPatterns.ContainsKey(key))
            {
                m_ExecutionPatterns[key] = execStrategyPattern;
            }
            else
            {
                m_ExecutionPatterns.Add(key, execStrategyPattern);
            }
        }


        public void FromFileJSONExecPatternString(string myDataPath)
        {
            FileStream file;
            StreamReader reader;

            try
            {
                file = new FileStream(myDataPath, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(file);


                while (!reader.EndOfStream)
                {
                    string epsData = reader.ReadLine();
                    ExecPatternString ps = Newtonsoft.Json.JsonConvert.DeserializeObject<ExecPatternString>(epsData);
                    AddExecStrategyPattern(ps.OrderType, ps.Name, ps.PatternString);
                }


                reader.Close();
                file.Close();

            }
            catch (Exception myE)
            {
                log.Error("FromFileJSONExecPatternString:" + myDataPath, myE);
            }
            finally
            {
                reader = null;
                file = null;
            }

        }
         */
       
    }
}
