using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Solver {
    public class OnProgressArg {
        public int progress_cur = 0; //out of 100

        public int depth_cur;
        public int depth_max;

        public float second_cur = 0.0f;
        public float second_max = 0.0f;

        public int closed = 0;

        public int prv_iter_opened_total = 0;
        public int prv_iter_discarded = 0;

        public int opened_cur = 0;
        public int opened_total = 0;

        public int discarded = 0;

        public bool carry_on = true;
    }
}
