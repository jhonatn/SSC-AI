using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Solver {
    using Parser;
    using State;
    public static class SolverHelper {
        public static List<State> GeneratePlay (State cur) {
            List<State> result = new List<State>();
            while (cur != null) {
                result.Add(cur);
                cur = cur.parent;
            }
            result.Reverse();
            return result;
        }
        public static State GenerateInitialNode (bool is_double, ICostFactory cost_factory) {
            State initial = new State(null, 0.0f, -1, false);
            initial.limbs[0] = new Analyzer.State.Limb();
            initial.limbs[1] = new Analyzer.State.Limb();
            if (is_double) {
                initial.limbs[0].main = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[4],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[0].sub = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[3],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[0].extra = new Analyzer.State.Part(
                    Analyzer.State.Movement.Unknown,
                    null,
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[1].main = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[5],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[1].sub = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[6],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[1].extra = new Analyzer.State.Part(
                    Analyzer.State.Movement.Unknown,
                    null,
                    0.0f, 0.0f, 0.0f
                );
            } else {
                initial.limbs[0].main = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[0],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[0].sub = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[1],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[0].extra = new Analyzer.State.Part(
                    Analyzer.State.Movement.Unknown,
                    null,
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[1].main = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[4],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[1].sub = new Analyzer.State.Part(
                    Analyzer.State.Movement.PassiveDown,
                    Panel.Panels_1D_Playable[3],
                    0.0f, 0.0f, 0.0f
                );
                initial.limbs[1].extra = new Analyzer.State.Part(
                    Analyzer.State.Movement.Unknown,
                    null,
                    0.0f, 0.0f, 0.0f
                );
            }
            initial.facing = new Vector(0.0f, 1.0f);
            initial.facing_desired = new Vector(0.0f, 1.0f);
            initial.sanityCheck();
            initial.cost = cost_factory.Calculate(initial);
            return initial;
        }
    }
}
