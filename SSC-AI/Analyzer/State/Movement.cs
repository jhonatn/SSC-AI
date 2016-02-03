using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    public enum Movement {
        Unknown,
        Tap,
        ForceDownStart,
        ForceDown,
        PassiveDown
    }
}
