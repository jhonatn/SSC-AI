using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Parser;
    public static class FacingCalculator {
        public static Vector Calculate (Limb[] limbs, Vector default_facing) {
            if (default_facing == null) { throw new ArgumentException(); }
            float dx = 0.0f;
            float dy = 0.0f;
            int count = 0;
            for (int a = 0; a < Limb.PART_COUNT; ++a) {
                Part part_a = limbs[0][a];
                if (part_a.IsUnknown()) { continue; }
                for (int b = 0; b < Limb.PART_COUNT; ++b) {
                    Part part_b = limbs[1][b];
                    if (part_b.IsUnknown()) { continue; }
                    Vector f = Panel.GetFacing(
                        part_a.panel.index, part_b.panel.index
                    );
                    dx += f.dx;
                    dy += f.dy;
                    ++count;
                }
            }
            if (count == 0) {
                return default_facing;
            } else {
                return new Vector(dx/count, dy/count);
            }
        }
        private static Vector[] DesiredFacings = new Vector[] { 
            new Vector(0.0f, 1.0f),
            new Vector(1.0f, 0.0f),
            new Vector(-1.0f, 0.0f),
            new Vector(0.0f, -1.0f)
        };
        private const float DESIRED_FACING_THRESHOLD = 0.3f;
        public static Vector CalculateDesiredFacing (Vector facing, Vector prv_facing_desired) {
            Vector desired_facing = null;
            float best_val = float.NegativeInfinity;
            foreach (Vector desired in DesiredFacings) {
                float v = facing.dot(desired);
                if (v > best_val + DESIRED_FACING_THRESHOLD) {
                    desired_facing = desired;
                    best_val = v;
                }
            }
            if (desired_facing == null) { throw new ExecutionEngineException(); }
            if (prv_facing_desired.dot(desired_facing) == -1.0f) {
                //WARNING, HARDCODED
                if (desired_facing == DesiredFacings[1] || desired_facing == DesiredFacings[2]) {
                    desired_facing = DesiredFacings[3];
                }
            }
            return desired_facing;
        }
    }
}
