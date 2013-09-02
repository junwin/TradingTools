using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KTASimulator
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CannedData
    {
        private bool _playOnSubscribe = true;
        private bool _runRealTime = true;
        private int _runInterval = 100;
        private bool _repeatOnEnd = true;
        private string _cannedDataFile = "filepath";

        public string CannedDataFile
        {
            get { return _cannedDataFile; }
            set { _cannedDataFile = value; }
        }
         

         public bool RepeatOnEnd
         {
             get { return _repeatOnEnd; }
             set { _repeatOnEnd = value; }
         }
         

         public int RunInterval
         {
             get { return _runInterval; }
             set { _runInterval = value; }
         }
         

         public bool RunRealTime
         {
             get { return _runRealTime; }
             set { _runRealTime = value; }
         }
         

         public bool PlayOnSubscribe
         {
             get { return _playOnSubscribe; }
             set { _playOnSubscribe = value; }
         }
    }
}
