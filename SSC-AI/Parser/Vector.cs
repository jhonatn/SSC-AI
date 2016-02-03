using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public class Vector {
        public static readonly Vector Forward = new Vector(0.0f, 1.0f);
        public float dx;
        public float dy;
        public Vector (float dx, float dy) {
            this.dx = dx;
            this.dy = dy;
        }

        public const float DEG2RAD = (float)(Math.PI / 180.0);//180 deg = PI rad, therefore, 1 deg = PI/180 rad
        public const float RAD2DEG = (float)(180.0 / Math.PI);
        public static Vector FromRadian (float radian) {
            return new Vector(
                (float)Math.Cos(radian),
                (float)Math.Sin(radian)
            );
        }
        public static Vector FromDegree (float degree) {
            return FromRadian(degree * DEG2RAD);
        }
        public float magnitude () {
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        //>0 = right
        public float determinant (Vector other) {
            return dx * other.dy - dy * other.dx;
        }
        public Vector normalized () {
            float mag = magnitude();
            return new Vector(
                dx / mag,
                dy / mag
            );
        }
        public Vector perpendicular () {
            return new Vector(
                -dy,
                dx
            );
        }
        public float dot (Vector other) {
            return dx * other.dx + dy * other.dy;
        }
        public Vector reverse () {
            return new Vector(dx * -1.0f, dy * -1.0f);
        }
        public float toRadian () {
            return (float)Math.Atan2(dy, dx);
        }
        public float toDegree () {
            return toRadian() * RAD2DEG;
        }
    }
}
