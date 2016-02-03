using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public static class FloatExt {
        //From: http://stackoverflow.com/questions/4633177/c-how-to-wrap-a-float-to-the-interval-pi-pi
        public static float mod (this float x, float divisor) {
            if (divisor == 0.0f) { return x; }
            float m = x - divisor * (float)Math.Floor(x / divisor);

            //Handle boundary cases resulted from floating-point cut off:
            if (divisor > 0.0f) {
                //Modulo range: [0..divisor)
                if (m >= divisor) {
                    //mod(-1e-16, 360.0): m= 360.0
                    return 0.0f;
                }
                if (m < 0.0f) {
                    if (divisor + m == divisor) {
                        return 0.0f; //Just in case
                    } else {
                        //mod(106.81415022205296, _TWO_PI): m= -1.421e-14 
                        return divisor + m;
                    }
                }
            } else {
                //Modulo range: (divisor..0]
                if (m <= divisor) {
                    //mod(1e-16, -360.0): m= -360.0
                    return 0.0f;
                } else {
                    //mod(-106.81415022205296, -_TWO_PI): m= 1.421e-14 
                    return divisor + m;
                }
            }

            return m;
        }
    }
}
