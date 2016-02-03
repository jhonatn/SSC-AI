using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public static class Angle {

        public static float CCWRadTo (float src_rad, float dst_rad) {
            src_rad = src_rad.mod(CONSTS.TAU);
            dst_rad = dst_rad.mod(CONSTS.TAU);
            if (dst_rad < src_rad) {
                dst_rad += CONSTS.TAU;
            }
            return dst_rad - src_rad;
        }
        public static float CWRadTo (float src_rad, float dst_rad) {
            return CCWRadTo(dst_rad, src_rad);
        }
        public static float CCWDegTo (float src_deg, float dst_deg) {
            return CCWRadTo(src_deg * CONSTS.DEG2RAD, dst_deg * CONSTS.DEG2RAD) * CONSTS.RAD2DEG;
        }
        public static float CWDegTo (float src_deg, float dst_deg) {
            return CCWDegTo(dst_deg, src_deg);
        }
    }
}
