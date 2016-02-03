using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Parser;
    using Node;
    public class State {
        public readonly Limb[] limbs = new Limb[] { 
            null, null
        };
        public readonly State parent;
        public readonly float second;
        public readonly int distance_from_start;
        public readonly bool limbs_crossed;

        private static long NxtId = 0;
        public readonly long id;
        public State (State parent, float second, int distance_from_start, bool limbs_crossed) {
            this.parent = parent;
            this.second = second;
            this.distance_from_start = distance_from_start;
            this.limbs_crossed = limbs_crossed;

            this.id = NxtId++;
        }

        public Beat beat = null;
        public Vector facing = Vector.Forward;
        public Vector facing_desired = Vector.Forward;

        public ICost cost = null;

        public void sanityCheck () {
            if (facing == null) {
                throw new SanityException("Null facing");
            }
            if (facing_desired == null) {
                throw new SanityException("Null desired facing");
            }
            foreach (Limb limb in limbs) {
                if (limb == null) {
                    throw new SanityException("Cannot have undefined limbs");
                }
                limb.sanityCheck();
                for (int i = 0; i < Limb.PART_COUNT; ++i) {
                    Part p = limb[i];
                    if (p.cur_second != second) {
                        throw new SanityException("Parts are from a different time");
                    }
                }
            }
            if (parent != null) {
                if (second < parent.second) { //Consider what happens with ==
                    throw new SanityException("Cannot go back in time");
                }
                if (distance_from_start <= parent.distance_from_start) {
                    throw new SanityException("Cannot go backwards in chart");
                }
            }
            Iterate.Foot2((foot_a, foot_b) => {
                Iterate.Part1(foot_a, (a_0) => {
                    Iterate.Part1(foot_b, (b_0) => {
                        Part part_a = limbs[foot_a][a_0];
                        Part part_b = limbs[foot_b][b_0];
                        if (part_a.panel == part_b.panel) {
                            if (
                                part_a.movement != Movement.Unknown &&
                                part_b.movement != Movement.Unknown
                            ) {
                                throw new SanityException("Two parts cannot occupy the same panel");
                            }
                        }
                    });
                });
            });

            foreach (Limb limb in limbs) {
                if (
                    !limb.main.IsUnknown() &&
                    !limb.sub.IsUnknown() &&
                    !limb.extra.IsUnknown() &&
                    !Panel.IsValidOrNot3Bracket(
                        limb == limbs[0],
                        limb.main.panel.index,
                        limb.sub.panel.index,
                        limb.extra.panel.index
                    )
                ) {
                    throw new SanityException("Invalid 3 bracket");
                }
            }
        }

        public int getOccupyingLimbIndex (Panel panel) {
            for (int i = 0; i < limbs.Length; ++i) {
                if (limbs[i].occupies(panel)) {
                    return i;
                }
            }
            return -1;
        }
    }
}
