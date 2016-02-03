using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public class Node {
        public const int INDEX_LEFT_FOOT  = 0;
        public const int INDEX_RIGHT_FOOT = 1;
        public const int LIMB_COUNT = 2;
        public static readonly int[] PART_COUNTS = new int[] { 3, 3 };

        public readonly Limb[] limbs = new Limb[] { 
            null, null
        };
        public float second = float.NaN;
        public readonly int distance_from_start;
        public Node (float second, int distance_from_start) {
            this.second = second;
            this.distance_from_start = distance_from_start;
        }
        public void sanityCheck () {
            int defined_limb_count = 0;
            foreach (Limb limb in limbs) {
                if (limb == null) { continue; }
                ++defined_limb_count;
                limb.sanityCheck();
            }
            if (defined_limb_count == 0) {
                throw new SanityException("Can't have zero defined possible limbs");
            }
            foreach (Limb limb in limbs) {
                if (limb == null) { continue; }
                if (
                    limb.main != null &&
                    limb.sub != null &&
                    limb.extra != null &&
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
    }
}
