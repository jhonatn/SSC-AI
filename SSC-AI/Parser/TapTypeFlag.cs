using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public class TapTypeFlag {
        private int m_Value = 0;
        public TapTypeFlag (int value) {
            m_Value = value;
        }
        public bool has (TapType tap_type) {
            int ele = (1 << (int)tap_type);
            return (m_Value & ele) != 0;
        }
    }
}
