using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public static class Node5Calculator {
        public static void Calculate32 (List<Node> result, Beat beat, int distance_from_start, Panel a, Panel b, Panel c, Panel d, Panel e) {
            if (!Panel.IsBracketable(a.index, b.index, c.index) || !Panel.IsBracketable(d.index, e.index)) { return; }
            bool is_force_tap =
                beat.notes[d.index_playable].tap == TapType.Force &&
                beat.notes[e.index_playable].tap == TapType.Force;

            Panel front = LimbHelper.GetFrontPanel(a, b, c);
            Panel back = LimbHelper.GetBackPanel(a, b, c);
            Panel center = LimbHelper.GetCenterPanel(a, b, c);

            Iterate.Foot2((foot_a, foot_b) => {
                bool face_front = (back.index < d.index || back.index < e.index);
                if (foot_a == Node.INDEX_RIGHT_FOOT) { face_front = !face_front; }
                Iterate.Part2(foot_b, d.index, e.index, (b_0, b_1) => {
                    if (
                        is_force_tap &&
                        (
                            b_0 == Limb.INDEX_EXTRA ||
                            b_1 == Limb.INDEX_EXTRA
                        )
                    ) {
                        return;
                    }

                    Node state = new Node(beat.second, distance_from_start);
                    state.limbs[foot_a] = new Limb();
                    LimbHelper.Do3Bracket(state, foot_a, face_front, beat, a, b, c);
                    state.limbs[foot_b] = new Limb();
                    state.limbs[foot_b][b_0] = new Part(beat, d);
                    state.limbs[foot_b][b_1] = new Part(beat, e);
                    state.sanityCheck();
                    result.Add(state);
                });
            });
        }
    }
}
