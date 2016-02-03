using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    public interface ICostFactory {
        ICost Calculate (State state);
    }
}
