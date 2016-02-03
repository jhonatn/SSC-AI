using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public class NoteFactory {
        public static readonly Note NONE = new Note("None", TapType.None);
        public static readonly NoteFactory Instance = new NoteFactory();
        private Dictionary<char, Note> m_Notes = new Dictionary<char, Note>();
        private NoteFactory () {
            m_Notes.Add(Note.RAW_NONE, NONE);
            m_Notes.Add(Note.RAW_HOLD_END, new Note("Hold End", TapType.PassiveEnd));
            m_Notes.Add(Note.RAW_HIDDEN, new Note("Hidden", TapType.Force));
            m_Notes.Add(Note.RAW_FAKE, new Note("Fake", TapType.None));
            m_Notes.Add(Note.RAW_LIFT, new Note("Lift", TapType.None));
            m_Notes.Add(Note.RAW_MINE, new Note("Mine", TapType.None));

            m_Notes.Add(Note.RAW_P0_TAP, new Note("P0 Tap", TapType.Force));
            m_Notes.Add(Note.RAW_P0_HOLD_HEAD, new Note("P0 Hold Head", TapType.PassiveBegin));
            m_Notes.Add(Note.RAW_P1_TAP, new Note("P1 Tap", TapType.Force));
            m_Notes.Add(Note.RAW_P1_HOLD_HEAD, new Note("P1 Hold Head", TapType.PassiveBegin));
            m_Notes.Add(Note.RAW_P2_TAP, new Note("P2 Tap", TapType.Force));
            m_Notes.Add(Note.RAW_P2_HOLD_HEAD, new Note("P2 Hold Head", TapType.PassiveBegin));
            m_Notes.Add(Note.RAW_P3_TAP, new Note("P3 Tap", TapType.Force));
            m_Notes.Add(Note.RAW_P3_HOLD_HEAD, new Note("P3 Hold Head", TapType.PassiveBegin));
        }
        public Note GetNote (char c) {
            return m_Notes[c];
        }
    }
}
