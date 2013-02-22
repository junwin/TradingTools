using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarDataTools
{
    /// <summary>
    /// Provide functions to clean data
    /// </summary>
    public class CleanerTools
    {
        /// <summary>
        /// Finds the missing bars in some set of price bars given the interval of 
		/// each bar
        /// </summary>
        /// <returns>
        /// The gaps in the data as BarRange objects
        /// </returns>
        /// <param name='priceBars'>
        /// Price bars.
        /// </param>
        /// <param name='startTicks'>
        /// Start ticks.
        /// </param>
        /// <param name='intervalTicks'>
        /// Interval ticks.
        /// </param>
        public static List<BarRange> FindMissingBars(K2DataObjects.PriceBar[] priceBars, long startTicks, long intervalTicks)
        {
            List<BarRange> missingRanges = new List<BarRange>();
            try
            {
                for(long index =0; index < priceBars.Length; index++)
                {
                    if(priceBars[index].TimeStamp < startTicks)
                        continue;

                    if (index > 0)
                    {
                  
                        long diffTicks = priceBars[index].TimeStamp - priceBars[index - 1].TimeStamp;
                        if(diffTicks > intervalTicks)
                        {
                            DateTime start = new DateTime(priceBars[index - 1].TimeStamp);
                            DateTime end = new DateTime(priceBars[index].TimeStamp);
                            if ((start.Hour == 15) && (start.Minute == 14))
                                continue;
                            if ((start.Hour == 16) && (start.Minute == 14))
                                continue;
                            if ((start.Day <10) || (start.Month!=1))
                                continue;
                            BarRange barRange = new BarRange(priceBars[index - 1].Mnemonic, intervalTicks, priceBars[index - 1].TimeStamp, priceBars[index].TimeStamp);
                            missingRanges.Add(barRange);
                        }
                    }

                    
                }
            }
            catch (Exception e)
            {
            }
            return missingRanges;
        }
    }
}
