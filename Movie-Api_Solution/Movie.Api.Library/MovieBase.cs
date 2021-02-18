using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieApiLibrary
{
    public class MovieBase
    {
        public int MovieId
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public int ReleaseYear
        {
            get;
            set;
        }
    }
}
