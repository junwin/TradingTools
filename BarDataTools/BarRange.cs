using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarDataTools
{
	/// <summary>
	/// Used to establish a time range between bars, this can
	/// be used to see if bars are missing 
	/// </summary>
    public class BarRange
    {
        string _mnemonic;
        long _startTicks=0;
        long _endTicks=0;
        long _intervalTicks=60*10000000;

        public BarRange()
        {
        }

        public BarRange(string mnemonic, long intervalTicks, long startTicks, long endTicks)
        {
            _mnemonic = mnemonic;
            _intervalTicks = intervalTicks;
            _startTicks = startTicks;
            _endTicks = endTicks;
        }
        public string Mnemonic
        {
            get { return _mnemonic; }
            set { _mnemonic = value; }
        }
        public long StartTicks
        {
            get { return _startTicks; }
            set { _startTicks = value; }
        }
        public long EndTicks
        {
            get { return _endTicks; }
            set { _endTicks = value; }
        }
        public long IntervalTicks
        {
            get { return _intervalTicks; }
            set { _intervalTicks = value; }
        }

        public long MissingCount
        {
            get { return (EndTicks-StartTicks)/IntervalTicks; } 
        }

        public DateTime Start
        {
            get { return new DateTime(StartTicks); }
        }
        public DateTime End
        {
            get { return new DateTime(EndTicks); }
        }
    }
}
