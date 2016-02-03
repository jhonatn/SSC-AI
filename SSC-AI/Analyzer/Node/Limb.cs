using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public class Limb {
        public const int INDEX_MAIN = 0;
        public const int INDEX_SUB = 1;
        public const int INDEX_EXTRA = 2;

        private Part m_Main  = null;
        private Part m_Sub   = null;
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
        public void setMain (Beat beat, Panel panel) {
            main = new Part(beat, panel);
        }
        public void setSub (Beat beat, Panel panel) {
            sub = new Part(beat, panel);
        }
        public void setExtra (Beat beat, Panel panel) {
            extra = new Part(beat, panel);
        }

        public void sanityCheck () {
            int defined_count = 0;
            foreach (Part part in m_Parts) {
                if (part == null) {
                    continue;
                }
                ++defined_count;
            }
            if (defined_count == 0) {
                throw new SanityException("Must have at least one defined part");
            }
            if (
                main != null &&
                sub != null &&
                main.panel == sub.panel
            ) {
                throw new SanityException("Main part cannot share same panel as sub part");
            }
            if (
                main!=null &&
                extra!=null &&
                main.panel == extra.panel
            ) {
                throw new SanityException("Main part cannot share same panel as extra part");
            }
            if (
                main!=null &&
                sub!=null &&
                !Panel.IsBracketable(
                    main.panel.index,
                    sub.panel.index
                )
            ) {
                throw new SanityException("Main-Sub not bracketable");
            }
            if (
                main != null &&
                extra != null &&
                !Panel.IsBracketable(
                    main.panel.index,
                    extra.panel.index
                )
            ) {
                throw new SanityException("Main-Extra not bracketable");
            }
            if (
                sub != null &&
                extra != null &&
                !Panel.IsSubBracketable(
                    sub.panel.index,
                    extra.panel.index
                )
            ) {
                throw new SanityException("Sub-Extra not bracketable");
            }
        }
    }
}
