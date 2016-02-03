using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Parser;
    using Node;
    public static class ArcCalculator {
        public static bool IsArcValid (State from, Node to) {
            for (int i = 0; i < from.limbs.Length; ++i) {
                if (to.limbs[i] == null) { continue; }
                if (!LimbHelper.IsArcValid(from.limbs[i], to.limbs[i])) {
                    return false;
                }
            }
            return true;
        }
        private static bool IsOverlapping (Limb[] limbs) {
            for (int a = 0; a < Limb.PART_COUNT; ++a) {
                Part part_a = limbs[0][a];
                if (part_a.IsUnknown()) { continue; }
                for (int b = 0; b < Limb.PART_COUNT; ++b) {
                    Part part_b = limbs[1][b];
                    if (part_b.IsUnknown()) { continue; }
                    if (part_a.panel == part_b.panel) {
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool IsValidOrNot3Bracket (bool is_left_leg, Limb limb) {
            return
                (
                    limb.main.IsUnknown() ||
                    limb.sub.IsUnknown() ||
                    limb.extra.IsUnknown()
                ) ||
                Panel.IsValidOrNot3Bracket(
                    is_left_leg,
                    limb.main.panel.index,
                    limb.sub.panel.index,
                    limb.extra.panel.index
                );
        }
        private static bool IsValidOrNot3Bracket (Limb[] limbs) {
            foreach (Limb limb in limbs) {
                if (!IsValidOrNot3Bracket(limb == limbs[0], limb)) {
                    return false;
                }
            }
            return true;
        }
        public static Limb[] CloneWithHover (Limb[] src, int limb_index, float cur_second, params int[] part_index) {
            Limb[] dst = new Limb[src.Length];
            for (int i = 0; i < src.Length; ++i) {
                dst[i] = Limb.DirectCopy(src[i]);
                if (i == limb_index) {
                    foreach (int p in part_index) {
                        dst[i][p] = new Part(
                            Movement.Unknown,
                            null,
                            dst[i][p].cur_second,
                            dst[i][p].cur_moved_second,
                            dst[i][p].prv_moved_second
                        );
                    }
                }
                dst[i].sanityCheck();
            }
            return dst;
        }
        public static bool IsHoverable (Movement e) {
            return e == Movement.PassiveDown;
        }
        public static void Calculate (State from, Node to, List<Limb[]> output) {
            if (!IsArcValid(from, to)) {
                return;
            }
            int hoverable_count = 0;
            Limb[] original = new Limb[from.limbs.Length];
            for (int i = 0; i < from.limbs.Length; ++i) {
                Limb from_limb = from.limbs[i];
                Analyzer.Node.Limb to_limb = to.limbs[i];
                if (to_limb == null) {
                    original[i] = Limb.ToPassiveDown(from_limb, to.second);
                } else {
                    original[i] = LimbHelper.TransitionToV3(from_limb, to_limb, to.second);
                }
                if (IsHoverable(original[i].main.movement)) {
                    ++hoverable_count;
                }
                if (IsHoverable(original[i].sub.movement)) {
                    ++hoverable_count;
                } else if (IsHoverable(original[i].extra.movement)) {
                    ++hoverable_count;
                }
                original[i].sanityCheck();
            }
            if (!IsOverlapping(original) && IsValidOrNot3Bracket(original)) {
                output.Add(original);
            }
            if (hoverable_count >= 2) {
                for (int limb_index = 0; limb_index < from.limbs.Length; ++limb_index) {
                    Limb limb = original[limb_index];
                    if (IsHoverable(limb.main.movement)) {
                        Limb[] with_unknown = CloneWithHover(original, limb_index, to.second, 0);
                        if (!IsOverlapping(with_unknown) && IsValidOrNot3Bracket(with_unknown)) {
                            output.Add(with_unknown);
                        }
                    }
                    if (IsHoverable(limb.sub.movement)) {
                        if (IsHoverable(limb.extra.movement)) {
                            Limb[] with_unknown = CloneWithHover(original, limb_index, to.second, 1, 2);
                            if (!IsOverlapping(with_unknown) && IsValidOrNot3Bracket(with_unknown)) {
                                output.Add(with_unknown);
                            }
                        } else {
                            Limb[] with_unknown = CloneWithHover(original, limb_index, to.second, 1);
                            if (!IsOverlapping(with_unknown) && IsValidOrNot3Bracket(with_unknown)) {
                                output.Add(with_unknown);
                            }
                        }
                    } else if (IsHoverable(limb.extra.movement)) {
                        Limb[] with_unknown = CloneWithHover(original, limb_index, to.second, 2);
                        if (!IsOverlapping(with_unknown) && IsValidOrNot3Bracket(with_unknown)) {
                            output.Add(with_unknown);
                        }
                    }
                }
            }
        }
        public static bool IsArcValidWithoutCrossing (Limb[] to, Vector facing) {
            float facing_rad = facing.toRadian();
            foreach (Limb limb in to) {
                if (
                    !limb.main.IsUnknown() &&
                    !limb.sub.IsUnknown() &&
                    !Panel.IsBracketableDirection(
                        facing_rad,
                        limb == to[0],
                        limb.main.panel.index,
                        limb.sub.panel.index
                    )
                ) {
                    return false;
                }
                if (
                    !limb.main.IsUnknown() &&
                    !limb.extra.IsUnknown() &&
                    !Panel.IsBracketableDirection(
                        facing_rad,
                        limb == to[0],
                        limb.main.panel.index,
                        limb.extra.panel.index
                    )
                ) {
                    return false;
                }
            }
            return true;
        }
        public static bool IsArcValidWithCrossing (Limb[] to, Vector facing) {
            float facing_rad = facing.toRadian();
            foreach (Limb limb in to) {
                if (
                    !limb.main.IsUnknown() &&
                    !limb.sub.IsUnknown() &&
                    !Panel.IsBracketableDirection(
                        facing_rad,
                        limb == to[1],
                        limb.main.panel.index,
                        limb.sub.panel.index
                    )
                ) {
                    return false;
                }
                if (
                    !limb.main.IsUnknown() &&
                    !limb.extra.IsUnknown() &&
                    !Panel.IsBracketableDirection(
                        facing_rad,
                        limb == to[1],
                        limb.main.panel.index,
                        limb.extra.panel.index
                    )
                ) {
                    return false;
                }
            }
            return true;
        }
        public static State TransitionTo (State from, Limb[] to, float second, int distance_from_start, Beat beat, ICostFactory cost_factory) {
            State nxt = null;
            Vector facing = FacingCalculator.Calculate(to, from.facing);
            Vector facing_desired = FacingCalculator.CalculateDesiredFacing(facing, from.facing_desired);
            if (IsArcValidWithoutCrossing(to, facing_desired)) {
                nxt = new State(from, second, distance_from_start, false);
            } else if (IsArcValidWithCrossing(to, facing_desired)) {
                //nxt = new State(from, second, distance_from_start, true);
                return null;
            } else {
                return null;
            }
            //Transition
            for (int i = 0; i < from.limbs.Length; ++i) {
                nxt.limbs[i] = to[i];
            }
            nxt.beat = beat;
            nxt.facing = facing;
            nxt.facing_desired = facing_desired;
            nxt.sanityCheck();
            nxt.cost = cost_factory.Calculate(nxt);
            return nxt;
        }
    }
}
