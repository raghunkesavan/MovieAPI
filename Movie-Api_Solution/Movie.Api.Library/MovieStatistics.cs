using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieApiLibrary
{
    public class MovieStatistics: MovieBase
    {
        
        public long averageWatchDurationS
        {
            get;
            set;
        }
         
        public long watches
        {
            get;
            set;
        }
    }
}
