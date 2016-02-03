using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Parser;
    public class Limb {
        public const int INDEX_MAIN = 0;
        public const int INDEX_SUB = 1;
        public const int INDEX_EXTRA = 2;
        public const int PART_COUNT = 3;

        private Part m_Main = null;
        private Part m_Sub = null;
        private Part m_Extra = null;
        private Part[] m_Parts = new Part[] { null, null, null };

        public Part main {
            get { return m_Main; }
            set { m_Main = value; m_Parts[INDEX_MAIN] = value; }
        }
        public Part sub {
            get { return m_Sub; }
            set { m_Sub = value; m_Parts[INDEX_SUB] = value; }
        }
        public Part extra {
            get { return m_Extra; }
            set { m_Extra = value; m_Parts[INDEX_EXTRA] = value; }
        }
        public Part this[int index] {
            get { return m_Parts[index]; }
            set {
                m_Parts[index] = value;
                switch (index) {
                    case INDEX_MAIN: m_Main = value; return;
                    case INDEX_SUB: m_Sub = value; return;
                    case INDEX_EXTRA: m_Extra = value; return;
                }
            }
        }

        public Limb () { }
        public static Limb DirectCopy (Limb src) {
            Limb result = new Limb();
            for (int i = 0; i < result.m_Parts.Length; ++i) {
                result[i] = Part.DirectCopy(src.m_Parts[i]);
            }
            return result;
        }
        public static Limb ToPassiveDown (Limb src, float cur_second) {
            Limb result = new Limb();
            for (int i = 0; i < result.m_Parts.Length; ++i) {
                if (src.m_Parts[i].IsUnknown()) {
                    result[i] = Part.ToUnknown(src.m_Parts[i], cur_second);
                } else {
                    result[i] = Part.ToPassiveDown(src.m_Parts[i], cur_second);
                }
            }
            return result;
        }

        public void sanityCheck () {
            foreach (Part part in m_Parts) {
                if (part == null) {
                    throw new SanityException("Limb cannot have null parts");
                }
            }
            if (
                !main.IsUnknown() &&
                !sub.IsUnknown() &&
                main.panel == sub.panel
            ) {
                throw new SanityException("Main part cannot share same panel as sub part");
            }
            if (
                !main.IsUnknown() &&
                !extra.IsUnknown() &&
                main.panel == extra.panel
            ) {
                throw new SanityException("Main part cannot share same panel as extra part");
            }
            if (
                !sub.IsUnknown() &&
                !extra.IsUnknown() &&
                sub.panel == extra.panel
            ) {
                throw new SanityException("Sub part cannot share same panel as extra part");
            }
            if (
                !main.IsUnknown() &&
                !sub.IsUnknown() &&
                !Panel.IsBracketable(
                    main.panel.index,
                    sub.panel.index
                )
            ) {
                throw new SanityException("Main-Sub not bracketable");
            }
            if (
                !main.IsUnknown() &&
                !extra.IsUnknown() &&
                !Panel.IsBracketable(
                    main.panel.index,
                    extra.panel.index
                )
            ) {
                throw new SanityException("Main-Extra not bracketable");
            }
            if (
                !sub.IsUnknown() &&
                !extra.IsUnknown() &&
                !Panel.IsSubBracketable(
                    sub.panel.index,
                    extra.panel.index
                )
            ) {
                throw new SanityException("Sub-Extra not bracketable");
            }
        }
        public Part getFirstNonHover () {
            foreach (Part p in m_Parts) {
                if (p.movement != Movement.Unknown) {
                    return p;
                }
            }
            return null;
        }
        public bool occupies (Panel panel) {
            for (int i = 0; i < m_Parts.Length; ++i) {
                if (m_Parts[i].panel == panel && m_Parts[i].movement != Movement.Unknown) {
                    return true;
                }
            }
            return false;
        }
        public int getOccupyingPartIndex (Panel panel) {
            for (int i = 0; i < m_Parts.Length; ++i) {
                if (m_Parts[i].panel == panel) {
                    return i;
                }
            }
            return -1;
        }
        public bool JustMoved () {
            for (int i = 0; i < m_Parts.Length; ++i) {
                if (m_Parts[i].JustMoved()) {
                    return true;
                }
            }
            return false;
        }
        public bool IsActiveBracket () {
            int count = 0;
            foreach (Part part in m_Parts) {
                if (part.IsUnknown() || part.IsPassiveDown()) { continue; }
                ++count;
            }
            return count >= 2;
        }
        public bool IsUnknown () {
            foreach (Part part in m_Parts) {
                if (!part.IsUnknown()) {
                    return false;
                }
            }
            return true;
        }
        public int JustMovedCount () {
            int count = 0;
            for (int i = 0; i < m_Parts.Length; ++i) {
                if (m_Parts[i].JustMoved()) {
                    ++count;
                }
            }
            return count;
        }
        public int JustMovedIndex () {
            for (int i = 0; i < m_Parts.Length; ++i) {
                if (m_Parts[i].JustMoved()) {
                    return i;
                }
            }
            return -1;
        }
        public List<int> JustMovedIndices () {
            List<int> result = new List<int>();
            for (int i = 0; i < m_Parts.Length; ++i) {
                if (m_Parts[i].JustMoved()) {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}
