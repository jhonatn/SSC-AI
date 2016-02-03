using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AHS.SSC.Parser {
    public class BPMS {
        public class Data {
            public float beat;
            public float bpm;
            public Data (float beat, float bpm) {
                this.beat = beat;
                this.bpm = bpm;
            }
        }
        public List<Data> data = new List<Data>();
        private static readonly Regex BPMSRegex = new Regex(@"(\d+.\d+)=(\d+.\d+)");
        public static BPMS Parse (string raw) {
            BPMS result = new BPMS();
            MatchCollection matches = BPMSRegex.Matches(raw);
            foreach (Match m in matches) {
                result.data.Add(new Data(
                    float.Parse(m.Groups[1].Value),
                    float.Parse(m.Groups[2].Value)
                ));
                if (result.data.Count == 1) {
                    if (result.data[0].beat != 0.0f) {
                        throw new FormatException("First BPM must be at beat zero");
                    }
                }
            }
            return result;
        }
    }
}
