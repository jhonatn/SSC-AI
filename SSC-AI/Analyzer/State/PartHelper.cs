using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Node;
    public static class PartHelper {
        public static bool IsArcValid (Part from, Analyzer.Node.Part to) {
            if (to == null) { return true; }
            if (!MovementHelper.IsArcValid(from.movement, to.movement)) {
                return false;
            }
            //Transition is valid if we consider movement only
            //Consider hold notes
            if (from.movement == Movement.ForceDownStart) {
                if (to.movement == Analyzer.Node.Movement.StayDown) {
                    return from.panel == to.panel; //Continue hold note
                } else if (to.movement == Analyzer.Node.Movement.Relax) {
                    return from.panel == to.panel; //End hold note
                } else {
                    throw new ArgumentException();
                }
            } else if (from.movement == Movement.ForceDown) {
                if (to.movement == Analyzer.Node.Movement.StayDown) {
                    return from.panel == to.panel; //Continue hold note
                } else if (to.movement == Analyzer.Node.Movement.Relax) {
                    return from.panel == to.panel; //End continued hold note
                } else {
                    throw new ArgumentException();
                }
            }
            return true;
        }
        public static Part TransitionTo (Part from, Analyzer.Node.Part to, float cur_second) {
            if (!IsArcValid(from, to)) {
                throw new ArgumentException();
            }
            if (to == null) {
                throw new ArgumentException();
                /*return new Part(
                    from.movement,
                    from.panel,
                    cur_second,
                    from.cur_moved_second,
                    from.prv_moved_second
                );*/
            } else {
                Movement nxt_movement = MovementHelper.TransitionTo(from.movement, to.movement);
                if (nxt_movement == Movement.PassiveDown) {
                    if (from.movement == Movement.PassiveDown) {
                        return new Part(nxt_movement, from.panel, cur_second, from.cur_moved_second, from.prv_moved_second);
                    } else {
                        return new Part(nxt_movement, from.panel, cur_second, cur_second, from.cur_moved_second);
                    }
                } else {
                    return new Part(nxt_movement, to.panel, cur_second, cur_second, from.cur_moved_second);
                }
            }
        }
    }
}
