using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public static class Node2Calculator {
        public static void Calculate11 (List<Node> result, Beat beat, int distance_from_start, Panel a, Panel b) {
            bool is_force_tap = beat.hasTapTypeOrNoneOnly(TapType.Force);

            Iterate.Foot2((foot_a, foot_b) => {
                Iterate.Part1(foot_a, (a_0) => {
                    Iterate.Part1(foot_b, (b_0) => {
                        if (is_force_tap && (a_0 == Limb.INDEX_EXTRA || b_0 == Limb.INDEX_EXTRA)) {
                            return;
                        }
                        Node state = new Node(beat.second, distance_from_start);

                        state.limbs[foot_a] = new Limb();
                        state.limbs[foot_a][a_0] = new Part(beat, a);
                        state.limbs[foot_b] = new Limb();
                        state.limbs[foot_b][b_0] = new Part(beat, b);
                        state.sanityCheck();
                        result.Add(state);
                    });
                });
            });
        }
        public static void Calculate2 (List<Node> result, Beat beat, int distance_from_start, Panel a, Panel b) {
            if (!Panel.IsBracketable(a.index, b.index)) { return; }
            bool is_force_tap = beat.hasTapTypeOrNoneOnly(TapType.Force);

            Iterate.Foot1((foot) => {
                Iterate.Part2(foot, a.index, b.index, (_0, _1) => {
                    if (is_force_tap && (_0 == Limb.INDEX_EXTRA || _1 == Limb.INDEX_EXTRA)) {
                        return;
                    }
                    Node state = new Node(beat.second, distance_from_start);

                    state.limbs[foot] = new Limb();
                    state.limbs[foot][_0] = new Part(beat, a);
                    state.limbs[foot][_1] = new Part(beat, b);
                    state.sanityCheck();
                    result.Add(state);
                });
            });
        }
    }
}
