using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarDataTools
{
    public class PotentialTradeTools
    {
        public enum ProfitSide { Buy, Sell, Ignore, none };
        /// <summary>
        /// Walks through a set of bars, looks back at a previous bar and
        /// and determines if it was an opportunity to buy or sell based on the
        /// current evidance
        /// </summary>
        /// <param name="priceBars">Set of price bars to analyse</param>
        /// <param name="scanSize">how many bars are scanned at a time</param>
        /// <param name="stops">max stops out of profit before potential buy/sell is rejected</param>
        /// <returns></returns>
        public static long[] GetWinners(K2DataObjects.PriceBar[] priceBars, int offset, int count, int scanSize, decimal stops, decimal profitTarget)
        {
            ProfitSide ps = ProfitSide.none;
            long[] winners = new long[count];
            //int outRow = 0;
            for (int i = offset; i < priceBars.Length; i++)
            {
                // need at least more bars than the scan size
                if (i >= scanSize)
                {
                    // check preceeding bars
                    ps = ProfitSide.none;

                    // right now its neither a buy or sell so look if 
                    // there is some direction
                    if (priceBars[i - scanSize].High < priceBars[i].Low)
                    {
                        // rising so its a buy opportunity
                        ps = ProfitSide.Buy;
                    }
                    else if (priceBars[i - scanSize].Low > priceBars[i].High)
                    {
                        // fallins so a sell opportunity
                        ps = ProfitSide.Sell;
                    }

                    for (int j = i - scanSize; j < i; j++)
                    {
                        // if profit side has been set to ignore dont analyse any more
                        if (ps != ProfitSide.Ignore)
                        {
                            // check still moving in the right direction
                            decimal diff = 0.0m;
                            if (ps == ProfitSide.Buy)
                            {
                                diff = priceBars[i - scanSize].High - priceBars[j].Low;
                                if (diff > stops)
                                {
                                    // Stopped out so ignore this bar as a buy opportunity
                                    ps = ProfitSide.Ignore;
                                    continue;
                                }
                            }
                            else if (ps == ProfitSide.Sell)
                            {
                                diff = priceBars[j].High - priceBars[i - scanSize].Low;
                                if (diff > stops)
                                {
                                    // Stopped out so ignore this bar as a sell opportunity
                                    ps = ProfitSide.Ignore;
                                    continue;
                                }
                            }                           
                        }
                    }
                    // Finished scanning the bars can now see if there was 
                    // a buy or sell opportunity
                    decimal profit = 0.0m;
                    int row = i - scanSize;
                    if (row >= offset)
                    {
                        row -= offset;
                        if (row < count)
                        {
                            winners[row] = 1;
                            if (ps == ProfitSide.Buy)
                            {
                                profit = priceBars[i].Low - priceBars[i - scanSize].High;
                                if (profit >= profitTarget)
                                {
                                    winners[row] = 3;
                                }                              
                            }
                            else if (ps == ProfitSide.Sell)
                            {
                                profit = priceBars[i - scanSize].Low - priceBars[i].High;
                                if (profit >= profitTarget)
                                {
                                    winners[row] = 2;
                                }                               
                            }                           
                        }
                        else
                        {
                            break;
                        }
                    }
                }


            }
            for (int i = 0; i < winners.Length; i++)
            {
                if (winners[i] == 0)
                {
                    winners[i] = 1;
                }
            }
            return winners.ToArray();
        }
    }
}
