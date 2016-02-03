using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Solver {
    using State;
    using Parser;
    public delegate void OnProgressDelegate (OnProgressArg arg);
    public interface ISolver {
        List<State> Solve (List<Measure> measures, ICostFactory cost_factory, OnProgressDelegate on_progress);
    }
}
