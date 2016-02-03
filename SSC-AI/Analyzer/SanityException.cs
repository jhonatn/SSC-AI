using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer {
    public class SanityException : Exception {
        public SanityException () { }
        public SanityException (string message) : base(message) { }
        public SanityException (string message, Exception inner) : base(message, inner) { }
    }
}
