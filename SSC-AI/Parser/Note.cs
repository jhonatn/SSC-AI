using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public class Note {
        public const char RAW_NONE = '0';
        public const char RAW_HOLD_END = '3';
        public const char RAW_HIDDEN = '5';
        public const char RAW_FAKE = 'F';
        public const char RAW_LIFT = 'L';
        public const char RAW_MINE = 'M';

        public const char RAW_P0_TAP = '1';
        public const char RAW_P0_HOLD_HEAD = '2';
        public const char RAW_P1_TAP = 'X';
        public const char RAW_P1_HOLD_HEAD = 'x';
        public const char RAW_P2_TAP = 'Y';
        public const char RAW_P2_HOLD_HEAD = 'y';
        public const char RAW_P3_TAP = 'Z';
        public const char RAW_P3_HOLD_HEAD = 'z';

        public string name;
        public TapType tap;
        public Note (string name, TapType tap) {
            this.name = name;
            this.tap = tap;
        }
    }
}
