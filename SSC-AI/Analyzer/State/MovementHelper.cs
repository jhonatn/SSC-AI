using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Node;
    public static class MovementHelper {
        public static bool IsArcValid (Movement from, Analyzer.Node.Movement to) {
            if (from == Movement.Unknown) {
                return
                    to == Analyzer.Node.Movement.JustDown ||
                    to == Analyzer.Node.Movement.JustDownOrStayDown;
            } else if (from == Movement.Tap) {
                return
                    to == Analyzer.Node.Movement.JustDown ||

                    to == Analyzer.Node.Movement.JustDownOrStayDown;
            } else if (from == Movement.ForceDownStart) {
                return
                    to == Analyzer.Node.Movement.StayDown ||
                    to == Analyzer.Node.Movement.Relax;
            } else if (from == Movement.ForceDown) {
                return
                    to == Analyzer.Node.Movement.StayDown ||

                    to == Analyzer.Node.Movement.Relax;
            } else if (from == Movement.PassiveDown) {
                return
                    to == Analyzer.Node.Movement.JustDown ||
                    to == Analyzer.Node.Movement.JustDownOrStayDown;
            } else {
                throw new ArgumentException();
            }
        }
        public static Movement TransitionTo (Movement from, Analyzer.Node.Movement to) {
            if (from == Movement.Unknown) {
                if (to == Analyzer.Node.Movement.JustDown) {
                    return Movement.Tap;
                } else if (to == Analyzer.Node.Movement.JustDownOrStayDown) {
                    return Movement.ForceDownStart;
                } else {
                    throw new ArgumentException();
                }
            } else if (from == Movement.Tap) {
                if (to == Analyzer.Node.Movement.JustDown) {
                    return Movement.Tap;
                } else if (to == Analyzer.Node.Movement.JustDownOrStayDown) {
                    return Movement.ForceDownStart;
                } else {
                    throw new ArgumentException();
                }
            } else if (from == Movement.ForceDownStart) {
                if (to == Analyzer.Node.Movement.StayDown) {
                    return Movement.ForceDown;
                } else if (to == Analyzer.Node.Movement.Relax) {
                    return Movement.PassiveDown;
                } else {
                    throw new ArgumentException();
                }
            } else if (from == Movement.ForceDown) {
                if (to == Analyzer.Node.Movement.StayDown) {
                    return Movement.ForceDown;
                } else if (to == Analyzer.Node.Movement.Relax) {
                    return Movement.PassiveDown;
                } else {
                    throw new ArgumentException();
                }
            } else if (from == Movement.PassiveDown) {
                if (to == Analyzer.Node.Movement.JustDown) {
                    return Movement.Tap;
                } else if (to == Analyzer.Node.Movement.JustDownOrStayDown) {
                    return Movement.ForceDownStart;
                } else {
                    throw new ArgumentException();
                }
            } else {
                throw new ArgumentException();
            }
        }
    }
}
