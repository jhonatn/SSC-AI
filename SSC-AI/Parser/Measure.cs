using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AHS.SSC.Parser {
    public class Measure {
        public List<Beat> beats = new List<Beat>();

        public static Measure Parse (string raw) {
            Measure result = new Measure();
            result.beats = Beat.ParseBeats(raw);
            return result;
        }
        private static readonly Regex MeasureRegex = new Regex(@",?([^,]+),?");
        public static List<Measure> ParseMeasures (string raw) {
            List<Measure> result = new List<Measure>();

            MatchCollection matches = MeasureRegex.Matches(raw);
            foreach (Match m in matches) {
                result.Add(Parse(m.Groups[1].Value));
            }
            return result;
        }
    }
}
