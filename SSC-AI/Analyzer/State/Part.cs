using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Parser;
    public class Part {
        public readonly Movement movement;
        public readonly Panel panel;
        public readonly float cur_second;
        public readonly float cur_moved_second;
        public readonly float prv_moved_second;

        public Part (
            Movement movement, Panel panel,
            float cur_second, float cur_moved_second, float prv_moved_second
        ) {
            bool has_movement = movement != Movement.Unknown;
            bool has_panel = panel != null;
            if (has_movement != has_panel) {
                throw new ArgumentException();
            }
            this.movement = movement;
            this.panel = panel;
            this.cur_second = cur_second;
            this.cur_moved_second = cur_moved_second;
            this.prv_moved_second = prv_moved_second;
        }
        public static Part DirectCopy (Part src) {
            Part result = new Part(
                src.movement,
                src.panel,
                src.cur_second,
                src.cur_moved_second,
                src.prv_moved_second
            );
            return result;
        }
        public static Part ToPassiveDown (Part src, float cur_second) {
            Part result = new Part(
                Movement.PassiveDown,
                src.panel,
                cur_second,
                src.cur_moved_second,
                src.prv_moved_second
            );
            return result;
        }
        public static Part ToUnknown (Part src, float cur_second) {
            Part result = new Part(
                Movement.Unknown,
                null,
                cur_second,
                src.cur_moved_second, //TODO decide if we consider going to unknown "just moving"
                src.prv_moved_second
            );
            return result;
        }
        public static Part ToUnknown (Part src) {
            return ToUnknown(src, src.cur_second);
        }

        public bool JustMoved () {
            return cur_second == cur_moved_second;
        }
        public bool IsUnknown () {
            return movement == Movement.Unknown;
        }
        public bool IsPassiveDown () {
            return movement == Movement.PassiveDown;
        }
    }
}
