using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyWebAPI.Helpers
{
    public class CatchError
    {
        public HttpStatusCode status { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string message { get; set; }

        internal Exception Exception()
        {
            throw new NotImplementedException();
        }
    }
}
