using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AHS.SSC.Parser {
    public class WARPS {
        public class Data {
            public float beat;
            public float skip_beat;
            public Data (float beat, float skip_beat) {
                this.beat = beat;
                this.skip_beat = skip_beat;
            }
        }
        public List<Data> data = new List<Data>();
        private static readonly Regex WARPSRegex = new Regex(@"(\d+.\d+)=(\d+.\d+)");
        public static WARPS Parse (string raw) {
            WARPS result = new WARPS();
            MatchCollection matches = WARPSRegex.Matches(raw);
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
