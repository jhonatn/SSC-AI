using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AHS.SSC.Parser {
    public class DELAYS {
        public class Data {
            public float beat;
            public float duration_second;
            public Data (float beat, float duration_second) {
                this.beat = beat;
                this.duration_second = duration_second;
            }
        }
        public List<Data> data = new List<Data>();
        private static readonly Regex DELAYSRegex = new Regex(@"(\d+.\d+)=(\d+.\d+)");
        public static DELAYS Parse (string raw) {
            DELAYS result = new DELAYS();
            MatchCollection matches = DELAYSRegex.Matches(raw);
            foreach (Match m in matches) {
                result.data.Add(new Data(
                    float.Parse(m.Groups[1].Value),
                    float.Parse(m.Groups[2].Value)
                ));
            }
            return result;
        }
    }
}
