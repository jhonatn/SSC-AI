using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public static class Node1Calculator {
        public static void Calculate1 (List<Node> result, Beat beat, int distance_from_start, Panel a) {
            bool is_force_tap = beat.hasTapTypeOrNoneOnly(TapType.Force);

            Iterate.Foot1((foot) => {
                Iterate.Part1(foot, (_0) => {
                    if (is_force_tap && _0 == Limb.INDEX_EXTRA) { return; }
                    Node state = new Node(beat.second, distance_from_start);
                    state.limbs[foot] = new Limb();
                    state.limbs[foot][_0] = new Part(beat, a);
                    state.sanityCheck();
                    result.Add(state);
                });
            });
        }
    }
}
