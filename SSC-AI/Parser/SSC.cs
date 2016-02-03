using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace AHS.SSC.Parser {
    public class SSC {
        public const string EXTENSION = ".ssc";
        public const string NOTEDATA_DELIMITER = "NOTEDATA";
        private static readonly Regex DataRegex = new Regex(@"#(.*):([^;]*);", RegexOptions.Multiline);

        public readonly Dictionary<string, string> raw_data = new Dictionary<string, string>();
        public readonly List<Chart> charts = new List<Chart>();

        public BPMS bpms = new BPMS();

        public static SSC Parse (string raw) {
            SSC result = new SSC();
            MatchCollection matches = DataRegex.Matches(raw);

            IEnumerator etor = matches.GetEnumerator();
            while (etor.MoveNext()) {
                Match m = (Match)etor.Current;
                if (m.Groups[1].Value == NOTEDATA_DELIMITER) {
                    Chart c = Chart.Parse(etor, result.bpms);
                    result.charts.Add(c);
                } else {
                    string key = m.Groups[1].Value;
                    string val = m.Groups[2].Value;
                    if (key == "BPMS") {
                        result.bpms = BPMS.Parse(val);
                    }
                    result.raw_data.Add(key, val);
                }
            }


            return result;
        }
    }
}
