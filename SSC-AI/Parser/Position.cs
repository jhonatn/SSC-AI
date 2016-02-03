using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public class Position {
        public float x;
        public float y;
        public Position (float x, float y) {
            this.x = x;
            this.y = y;
        }
        public float distanceTo (Position other) {
            float dx = other.x - x;
            float dy = other.y - y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public Vector vectorTo (Position other) {
            float dx = other.x - x;
            float dy = other.y - y;
            return new Vector(dx, dy);
        }
    }
}
