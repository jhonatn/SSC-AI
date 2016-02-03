using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public static class TapTypeExt {
        public static bool isPassiveBeginOrForce (this TapType e) {
            return e == TapType.PassiveBegin || e == TapType.Force;
        }
        public static TapTypeFlag toFlag (this TapType[] arr) {
            int flag = 0;
            foreach (TapType ele in arr) {
                flag |= (1 << (int)ele);
            }
            return new TapTypeFlag(flag);
        }

    }
}
