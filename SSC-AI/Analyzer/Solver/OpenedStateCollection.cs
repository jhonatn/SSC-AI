using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Solver {
    using State;
    public class OpenedStateCollection {
        private readonly State[] m_Items;
        private readonly int m_Size;
        private readonly int m_HalfSize;
        private readonly StateComparer m_Comparer;

        private int m_Count = 0;
        private State m_First = null;
        public OpenedStateCollection (int size, int buffer, StateComparer comparer) {
            m_Items = new State[size + buffer];
            m_Size = size;
            m_HalfSize = size / 2;
            m_Comparer = comparer;
        }

        private int m_TotalAdded = 0;
        private int m_TotalDiscarded = 0;

        public void Add (State item) {
            m_Items[m_Count++] = item;
            ++m_TotalAdded;
        }
        public State First () {
            return m_First;
        }
        public int Count () {
            return m_Count;
        }
        public int GetTotalAdded () { return m_TotalAdded; }
        public int GetTotalDiscarded () { return m_TotalDiscarded; }
        public bool Commit () {
            Array.Sort(m_Items, 0, m_Count, m_Comparer);
            m_First = m_Items[0];
            m_Items[0] = null;
            if (m_Count > m_Size) {
                m_TotalDiscarded += m_Count - m_Size;
                m_Count = m_Size;
            }
            while (m_Count > 0 && m_Items[m_Count - 1] == null) {
                --m_Count;
            }
            return m_First != null;
        }
    }
}
