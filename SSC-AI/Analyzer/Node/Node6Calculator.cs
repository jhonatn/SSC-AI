using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public static class Node6Calculator {
        public static void Calculate33 (List<Node> result, Beat beat, int distance_from_start, Panel a, Panel b, Panel c, Panel d, Panel e, Panel f) {
            if (!Panel.IsBracketable(a.index, b.index, c.index) || !Panel.IsBracketable(d.index, e.index, f.index)) { return; }
            Panel front_a = LimbHelper.GetFrontPanel(a, b, c);
            Panel back_a = LimbHelper.GetBackPanel(a, b, c);
            Panel center_a = LimbHelper.GetCenterPanel(a, b, c);

            Panel front_b = LimbHelper.GetFrontPanel(d, e, f);
            Panel back_b = LimbHelper.GetBackPanel(d, e, f);
            Panel center_b = LimbHelper.GetCenterPanel(d, e, f);

            Iterate.Foot2((foot_a, foot_b) => {
                bool face_front = (back_a.index < back_b.index);
                if (foot_a == Node.INDEX_RIGHT_FOOT) { face_front = !face_front; }

                Node state = new Node(beat.second, distance_from_start);
                state.limbs[foot_a] = new Limb();
                LimbHelper.Do3Bracket(state, foot_a, face_front, beat, front_a, back_a, center_a);
                state.limbs[foot_b] = new Limb();
                LimbHelper.Do3Bracket(state, foot_b, face_front, beat, front_b, back_b, center_b);
                state.sanityCheck();
                result.Add(state);
            });
        }
    }
}
