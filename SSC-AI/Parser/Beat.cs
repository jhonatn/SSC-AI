using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AHS.SSC.Parser {
    public class Beat {
        public float bpm;
        public float seconds_per_beat;

        public float beat;
        public float second;
        public float delta_second;

        public int beat_interval = -1;

        public List<Note> notes = new List<Note>();
        public int getAnyTapCount () {
            int result = 0;
            foreach (Note n in notes) {
                if (n.tap != TapType.None) {
                    ++result;
                }
            }
            return result;
        }
        public int getCount (TapType tap_type) {
            int result = 0;
            foreach (Note n in notes) {
                if (n.tap == tap_type) {
                    ++result;
                }
            }
            return result;
        }
        public int getCount (params TapType[] tap_type) {
            TapTypeFlag flag = tap_type.toFlag();

            int result = 0;
            foreach (Note n in notes) {
                if (flag.has(n.tap)) {
                    ++result;
                }
            }
            return result;
        }
        public bool isEmpty () {
            return getCount(TapType.None) == notes.Count;
        }
        public int getNotCount (TapType tap_type) {
            return notes.Count - getCount(tap_type);
        }
        public int getNotCount (params TapType[] tap_type) {
            return notes.Count - getCount(tap_type);
        }
        public int indexOf (TapType tap_type) {
            for (int i = 0; i < notes.Count; ++i) {
                if (notes[i].tap == tap_type) { return i; }
            }
            return -1;
        }
        public int indexOf (params TapType[] tap_type) {
            TapTypeFlag flag = tap_type.toFlag();

            for (int i = 0; i < notes.Count; ++i) {
                if (flag.has(notes[i].tap)) { return i; }
            }
            return -1;
        }
        public List<int> getIndices (TapType tap_type) {
            List<int> result = new List<int>();
            for (int i = 0; i < notes.Count; ++i) {
                if (notes[i].tap == tap_type) {
                    result.Add(i);
                }
            }
            return result;
        }
        public List<int> getIndices (params TapType[] tap_type) {
            TapTypeFlag flag = tap_type.toFlag();

            List<int> result = new List<int>();
            for (int i = 0; i < notes.Count; ++i) {
                if (flag.has(notes[i].tap)) {
                    result.Add(i);
                }
            }
            return result;
        }
        public bool is2Jump () {
            return getCount(TapType.PassiveBegin, TapType.Force) == 2;
        }
        public bool isBracketable2Jump () {
            if (!is2Jump()) { return false; }
            List<int> taps = getIndices(TapType.PassiveBegin, TapType.Force);
            return Panel.IsBracketable(
                Panel.Panels_1D_Playable[taps[0]].index,
                Panel.Panels_1D_Playable[taps[1]].index
            );
        }
        public static Beat Parse (string raw) {
            Beat result = new Beat();
            foreach (char c in raw) {
                result.notes.Add(NoteFactory.Instance.GetNote(c));
            }
            return result;
        }
        private static readonly Regex BeatRegex = new Regex(@"^(\w+)", RegexOptions.Multiline);
        public static List<Beat> ParseBeats (string raw) {
            List<Beat> result = new List<Beat>();

            MatchCollection matches = BeatRegex.Matches(raw);
            foreach (Match m in matches) {
                result.Add(Parse(m.Groups[1].Value));
            }
            return result;
        }

        public bool hasTapTypeOrNoneOnly (TapType tap_type) {
            foreach (Note n in notes) {
                if (n.tap != tap_type && n.tap != TapType.None) {
                    return false;
                }
            }
            return true;
        }
        public bool hasTapTypeOrNoneOnly (params TapType[] tap_type) {
            TapTypeFlag flag = tap_type.toFlag();
            foreach (Note n in notes) {
                if (!flag.has(n.tap) && n.tap != TapType.None) {
                    return false;
                }
            }
            return true;
        }
        public bool hasSameNotes (Beat other) {
            if (notes.Count != other.notes.Count) { return false; }
            for (int i = 0; i < notes.Count; ++i) {
                if (notes[i].tap != other.notes[i].tap) {
                    return false;
                }
            }
            return true;
        }
    }
}
