using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public class Part {
        public readonly Movement movement;
        public readonly Panel panel;
        public Part (Movement movement, Panel panel) {
            if (panel == null) { throw new ArgumentNullException(); }
            this.movement = movement;
            this.panel = panel;
        }

        public static Movement TapTypeToMovement (TapType tap) {
            switch (tap) {
                case TapType.Force: return Movement.JustDown;
                case TapType.PassiveBegin: return Movement.JustDownOrStayDown;
                case TapType.PassiveStay: return Movement.StayDown;
                case TapType.PassiveEnd: return Movement.Relax;
            }
            throw new ArgumentException();
        }
        public Part (TapType tap, Panel panel) :
            this(TapTypeToMovement(tap), panel) { }
        public Part (Beat beat, Panel panel) :
            this(beat.notes[panel.index_playable].tap, panel) { }
    }
}
