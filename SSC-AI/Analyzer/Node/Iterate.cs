using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public static class Iterate {
        public delegate void Foot1IterateDelegate (int foot);
        public static void Foot1 (Foot1IterateDelegate d) {
            for (int foot = 0; foot < Node.LIMB_COUNT; ++foot) {
                d(foot);
            }
        }
        public delegate void Foot2IterateDelegate (int foot_a, int foot_b);
        public static void Foot2 (Foot2IterateDelegate d) {
            for (int foot_a = 0; foot_a < Node.LIMB_COUNT; ++foot_a) {
                for (int foot_b = 0; foot_b < Node.LIMB_COUNT; ++foot_b) {
                    if (foot_a == foot_b) { continue; }
                    d(foot_a, foot_b);
                }
            }
        }

        public delegate void Part1IterateDelegate (int part);
        public delegate void Part2IterateDelegate (int part_a, int part_b);
        public static void Part1 (int limb, Part1IterateDelegate d) {
            for (int part = 0; part < Node.PART_COUNTS[limb]; ++part) {
                d(part);
            }
        }
        public static void Part2 (int limb, int a, int b, Part2IterateDelegate d) {
            for (int part_a = 0; part_a < Node.PART_COUNTS[limb]; ++part_a) {
                for (int part_b = 0; part_b < Node.PART_COUNTS[limb]; ++part_b) {
                    if (part_a == part_b) { continue; }
                    bool is_sub_and_extra = part_a != Limb.INDEX_MAIN && part_b != Limb.INDEX_MAIN;
                    bool is_bracketable = is_sub_and_extra ?
                        Panel.IsSubBracketable(a, b) :
                        Panel.IsBracketable(a, b);
                    if (!is_bracketable) { continue; }
                    d(part_a, part_b);
                }
            }
        }
    }
}
