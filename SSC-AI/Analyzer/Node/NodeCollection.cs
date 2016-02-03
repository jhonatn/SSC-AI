using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public class NodeCollection {
        public Beat beat;
        public List<Node> items = new List<Node>();
        public NodeCollection (Beat beat) {
            this.beat = beat;
        }

        public static NodeCollection Calculate (Beat beat, int distance_from_start) {
            if (beat.isEmpty()) {
                throw new ArgumentException();
            }
            NodeCollection result = new NodeCollection(beat);
            List<Node> items = result.items;
            int any_tap_count = beat.getAnyTapCount();
            List<int> any_tap_indices = beat.getIndices(TapType.Force, TapType.PassiveBegin, TapType.PassiveStay, TapType.PassiveEnd);

            if (any_tap_count == 1) {
                Panel a = Panel.Panels_1D_Playable[any_tap_indices[0]];
                Node1Calculator.Calculate1(items, beat, distance_from_start, a);
            } else if (any_tap_count == 2) {
                Panel a = Panel.Panels_1D_Playable[any_tap_indices[0]];
                Panel b = Panel.Panels_1D_Playable[any_tap_indices[1]];

                Node2Calculator.Calculate11(items, beat, distance_from_start, a, b);
                Node2Calculator.Calculate2(items, beat, distance_from_start, a, b);
            } else if (any_tap_count == 3) {
                Panel a = Panel.Panels_1D_Playable[any_tap_indices[0]];
                Panel b = Panel.Panels_1D_Playable[any_tap_indices[1]];
                Panel c = Panel.Panels_1D_Playable[any_tap_indices[2]];

                //3
                Node3Calculator.Calculate3(items, beat, distance_from_start, a, b, c);
                //21
                Node3Calculator.Calculate21(items, beat, distance_from_start, a, b, c);
                Node3Calculator.Calculate21(items, beat, distance_from_start, a, c, b);

                Node3Calculator.Calculate21(items, beat, distance_from_start, b, c, a);

                if (items.Count == 0) {
                    throw new NotImplementedException();
                }
            } else if (any_tap_count == 4) {
                Panel a = Panel.Panels_1D_Playable[any_tap_indices[0]];
                Panel b = Panel.Panels_1D_Playable[any_tap_indices[1]];
                Panel c = Panel.Panels_1D_Playable[any_tap_indices[2]];
                Panel d = Panel.Panels_1D_Playable[any_tap_indices[3]];

                //31
                Node4Calculator.Calculate31(items, beat, distance_from_start, a, b, c, d);
                Node4Calculator.Calculate31(items, beat, distance_from_start, b, c, d, a);
                Node4Calculator.Calculate31(items, beat, distance_from_start, c, d, a, b);
                Node4Calculator.Calculate31(items, beat, distance_from_start, d, a, b, c);
                //22
                Node4Calculator.Calculate22(items, beat, distance_from_start, a, b, c, d);
                Node4Calculator.Calculate22(items, beat, distance_from_start, a, c, d, b);
                Node4Calculator.Calculate22(items, beat, distance_from_start, a, d, b, c);

                Node4Calculator.Calculate22(items, beat, distance_from_start, b, c, d, a);
                Node4Calculator.Calculate22(items, beat, distance_from_start, b, d, a, c);

                Node4Calculator.Calculate22(items, beat, distance_from_start, c, d, a, b);

                if (items.Count == 0) {
                    throw new NotImplementedException();
                }
            } else if (any_tap_count == 5) {
                Panel a = Panel.Panels_1D_Playable[any_tap_indices[0]];
                Panel b = Panel.Panels_1D_Playable[any_tap_indices[1]];
                Panel c = Panel.Panels_1D_Playable[any_tap_indices[2]];
                Panel d = Panel.Panels_1D_Playable[any_tap_indices[3]];
                Panel e = Panel.Panels_1D_Playable[any_tap_indices[4]];

                Node5Calculator.Calculate32(items, beat, distance_from_start, a, b, c, d, e);
                Node5Calculator.Calculate32(items, beat, distance_from_start, a, b, d, e, c);
                Node5Calculator.Calculate32(items, beat, distance_from_start, a, b, e, c, d);

                Node5Calculator.Calculate32(items, beat, distance_from_start, a, c, d, e, b);
                Node5Calculator.Calculate32(items, beat, distance_from_start, a, c, e, b, d);

                Node5Calculator.Calculate32(items, beat, distance_from_start, a, d, e, b, c);

                Node5Calculator.Calculate32(items, beat, distance_from_start, b, c, d, e, a);
                Node5Calculator.Calculate32(items, beat, distance_from_start, b, c, e, a, d);

                Node5Calculator.Calculate32(items, beat, distance_from_start, b, d, e, a, c);

                Node5Calculator.Calculate32(items, beat, distance_from_start, c, d, e, a, b);

                if (items.Count == 0) {
                    throw new NotImplementedException();
                }
            } else if (any_tap_count == 6) {
                Panel a = Panel.Panels_1D_Playable[any_tap_indices[0]];
                Panel b = Panel.Panels_1D_Playable[any_tap_indices[1]];
                Panel c = Panel.Panels_1D_Playable[any_tap_indices[2]];
                Panel d = Panel.Panels_1D_Playable[any_tap_indices[3]];
                Panel e = Panel.Panels_1D_Playable[any_tap_indices[4]];
                Panel f = Panel.Panels_1D_Playable[any_tap_indices[5]];

                Node6Calculator.Calculate33(items, beat, distance_from_start, a, b, c, d, e, f);
                Node6Calculator.Calculate33(items, beat, distance_from_start, d, e, f, a, b, c);

                if (items.Count == 0) {
                    throw new NotImplementedException();
                }
            } else {
                throw new NotImplementedException();
            }

            if (items.Count == 0) {
                throw new ExecutionEngineException();
            }
            return result;
        }
        private static bool CanSkip (Beat cur, Beat prv) {
            if (cur.notes.Count != prv.notes.Count) { return false; }
            for (int i = 0; i < cur.notes.Count; ++i) {
                TapType cur_tap = cur.notes[i].tap;
                TapType prv_tap = prv.notes[i].tap;
                bool cur_is_body = (cur_tap == TapType.PassiveStay);
                bool prv_is_head_or_body = (
                    prv_tap == TapType.PassiveBegin ||
                    prv_tap == TapType.PassiveStay
                );
                if (cur_is_body != prv_is_head_or_body) {
                    return false;
                }
            }
            return true;
        }
        public static List<NodeCollection> CalculateNodes (List<Measure> measures) {
            List<NodeCollection> result = new List<NodeCollection>();
            Beat prv_beat = null;
            foreach (Measure m in measures) {
                foreach (Beat b in m.beats) {
                    if (b.isEmpty()) { continue; }
                    if (
                        prv_beat != null &&
                        prv_beat.hasTapTypeOrNoneOnly(TapType.PassiveBegin, TapType.PassiveStay) &&
                        b.hasTapTypeOrNoneOnly(TapType.PassiveStay) &&
                        CanSkip(b, prv_beat)
                    ) {
                        continue;
                    } else {
                        result.Add(Calculate(b, result.Count));
                        prv_beat = b;
                    }
                }
            }
            return result;
        }
        private enum SectionState {
            FindJumps,
            FindNonJumps
        }
        public static List<List<NodeCollection>> CalculateSections (
            List<NodeCollection> input,
            int desired_section_size
        ) {
            List<List<NodeCollection>> result = new List<List<NodeCollection>>();
            List<NodeCollection> cur = new List<NodeCollection>();
            SectionState state = SectionState.FindJumps;

            int prv_tap_panel = -1;

            foreach (NodeCollection collection in input) {
                int cur_tap_panel = -1;
                Beat b = collection.beat;
                if (state == SectionState.FindJumps) {
                    cur.Add(collection);
                    if (b.hasTapTypeOrNoneOnly(TapType.Force, TapType.PassiveBegin)) {
                        if (
                            b.getAnyTapCount() < 2 ||
                            b.isBracketable2Jump()
                        ) {
                            state = SectionState.FindNonJumps;
                        }
                    }
                } else {
                    if (b.hasTapTypeOrNoneOnly(TapType.Force, TapType.PassiveBegin)) {
                        if (b.getAnyTapCount() == 1) {
                            if (b.hasTapTypeOrNoneOnly(TapType.Force)) {
                                cur_tap_panel = b.getIndices(TapType.Force)[0];
                            }
                            cur.Add(collection);

                            if (
                                cur.Count >= desired_section_size &&
                                prv_tap_panel != cur_tap_panel &&
                                prv_tap_panel >= 0 &&
                                cur_tap_panel >= 0
                            ) {
                                Panel prv_panel = Panel.Panels_1D_Playable[prv_tap_panel];
                                Panel cur_panel = Panel.Panels_1D_Playable[cur_tap_panel];
                                if (
                                    prv_panel.direction_y == cur_panel.direction_y &&
                                    prv_panel.direction_y != PanelDirectionY.Center
                                ) {
                                    cur_tap_panel = -1;
                                    result.Add(cur);
                                    cur = new List<NodeCollection>();
                                    state = SectionState.FindJumps;
                                }
                            }
                        } else if (b.isBracketable2Jump()) {
                            cur.Add(collection);
                        } else {
                            result.Add(cur);
                            cur = new List<NodeCollection>();
                            cur.Add(collection);
                            state = SectionState.FindJumps;
                        }
                    } else {
                        cur.Add(collection);
                    }
                }
                prv_tap_panel = cur_tap_panel;
            }
            if (cur.Count == 0) {
                throw new ArgumentException();
            }
            result.Add(cur);
            return result;
        }
    }
}
