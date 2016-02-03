using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Solver {
    using State;
    public class StateComparer : IComparer<State> {
        private float distance_neg_cost_multiplier;
        public StateComparer (float distance_neg_cost_multiplier) {
            this.distance_neg_cost_multiplier = distance_neg_cost_multiplier;
        }
        public int Compare (State a, State b) {
            if (a == null) {
                return (b == null) ? 0 : 1;
            } else if (b == null) {
                return -1;
            }

            float a_order = a.cost.GetTotalCost() - a.distance_from_start * distance_neg_cost_multiplier;
            float b_order = b.cost.GetTotalCost() - b.distance_from_start * distance_neg_cost_multiplier;
            if (a_order != b_order) {
                return (a_order < b_order) ? -1 : 1;
            }

            if (a.id != b.id) {
                return a.id < b.id ? -1 : 1;
            }
            return 0;
        }
    }
}
