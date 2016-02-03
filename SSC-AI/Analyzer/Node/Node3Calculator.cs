using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public static class Node3Calculator {
        public static void Calculate3 (List<Node> result, Beat beat, int distance_from_start, Panel a, Panel b, Panel c) {
            if (!Panel.IsBracketable(a.index, b.index, c.index)) { return; }

            Iterate.Foot1((foot) => {
                {
                    Node state = new Node(beat.second, distance_from_start);
                    state.limbs[foot] = new Limb();
                    LimbHelper.Do3Bracket(state, foot, true, beat, a, b, c);
                    state.sanityCheck();
                    result.Add(state);
                }
                {
                    Node state = new Node(beat.second, distance_from_start);
                    state.limbs[foot] = new Limb();
                    LimbHelper.Do3Bracket(state, foot, false, beat, a, b, c);
                    state.sanityCheck();
                    result.Add(state);
                }
            });
        }
        public static void Calculate21 (List<Node> result, Beat beat, int distance_from_start, Panel a, Panel b, Panel c) {
            if (!Panel.IsBracketable(a.index, b.index)) { return; }
            bool is_force_tap = beat.hasTapTypeOrNoneOnly(TapType.Force);

            Iterate.Foot2((foot_a, foot_b) => {
                Iterate.Part2(foot_a, a.index, b.index, (a_0, a_1) => {
                    Iterate.Part1(foot_b, (b_0) => {
                        if (
                            is_force_tap &&
                            (
                                a_0 == Limb.INDEX_EXTRA ||
                                a_1 == Limb.INDEX_EXTRA ||
                                b_0 == Limb.INDEX_EXTRA
                            )
                        ) {
                            return;
                        }
                        Node state = new Node(beat.second, distance_from_start);
                        state.limbs[foot_a] = new Limb();
                        state.limbs[foot_b] = new Limb();
                        state.limbs[foot_a][a_0] = new Part(beat, a);
                        state.limbs[foot_a][a_1] = new Part(beat, b);
                        state.limbs[foot_b][b_0] = new Part(beat, c);
                        state.sanityCheck();
                        result.Add(state);
                    });
                });
            });
        }
    }
}
