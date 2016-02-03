using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace AHS.SSC.Parser {
    public class Chart {
        public const string KEY_NOTES = "NOTES";
        public const string KEY_BPMS = "BPMS";
        public const string KEY_STOPS = "STOPS";
        public const string KEY_DELAYS = "DELAYS";
        public const string KEY_WARPS = "WARPS";
        public const string KEY_OFFSET = "OFFSET";

        public const string KEY_STEPSTYPE = "STEPSTYPE";
        public const string KEY_METER = "METER";

        public const double SMALLEST_BEAT_STEP = (1.0 / (192.0 / 4.0));
        public const double SMALLEST_SUB_BEAT = 0.020835;

        public readonly Dictionary<string, string> raw_data = new Dictionary<string, string>();
        public List<Measure> measures = new List<Measure>();
        public BPMS bpms = new BPMS();
        public DELAYS delays = new DELAYS();
        public WARPS warps = new WARPS();
        public float offset = 0.0f;

        public float total_second = 0.0f;

        public bool IsDouble() {
            if (measures.Count == 0) { return false; }
            return measures[0].beats[0].notes.Count == 10;
        }

        public static Chart Parse (IEnumerator etor, BPMS default_bpms) {
            Chart result = new Chart();
            while (etor.MoveNext()) {
                Match m = (Match)etor.Current;
                string key = m.Groups[1].Value;
                string val = m.Groups[2].Value;
                if (key == KEY_NOTES) {
                    result.measures = Measure.ParseMeasures(val);
                    break;
                }
                if (key == KEY_BPMS) {
                    result.bpms = BPMS.Parse(val);
                }
                if (key == KEY_DELAYS) {
                    result.delays = DELAYS.Parse(val);
                }
                if (key == KEY_STOPS && val != string.Empty) {
                    throw new NotImplementedException();
                }
                if (key == KEY_WARPS && val != string.Empty) {
                    result.warps = WARPS.Parse(val);
                }
                if (key == KEY_OFFSET) {
                    result.offset = float.Parse(val);
                }
                result.raw_data.Add(key, val);
            }
            if (result.bpms.data.Count == 0) {
                result.bpms = default_bpms;
            }
            result.calculateHoldBodies();
            result.total_second = CalculateTimes(
                result.offset,
                result.measures,
                new List<BPMS.Data>(result.bpms.data),
                new List<DELAYS.Data>(result.delays.data),
                new List<WARPS.Data>(result.warps.data)
            );
            return result;
        }
        public static readonly Note HoldBody = new Note("Hold Body", TapType.PassiveStay);
        public void calculateHoldBodies () {
            CalculateHoldBodies(measures);
        }
        private static void CalculateHoldBodies (List<Measure> measures, int note_index) {
            List<Beat> beats = new List<Beat>();
            foreach (Measure m in measures) {
                beats.AddRange(m.beats);
            }
            bool finding_hold_head = true;
            foreach (Beat b in beats) {
                Note n = b.notes[note_index];
                if (finding_hold_head) {
                    if (n.tap == TapType.PassiveBegin) {
                        finding_hold_head = false;
                    }
                } else {
                    if (n.tap == TapType.PassiveEnd) {
                        finding_hold_head = true;
                    } else {
                        b.notes[note_index] = HoldBody;
                    }
                }
            }
        }
        public static void CalculateHoldBodies (List<Measure> measures) {
            if (measures.Count == 0) { return; }
            if (measures[0].beats.Count == 0) { return; }
            Beat first_beat = measures[0].beats[0];
            for (int i = 0; i < first_beat.notes.Count; ++i) {
                CalculateHoldBodies(measures, i);
            }
        }
        public static float CalculateTimes (float offset, List<Measure> measures, List<BPMS.Data> tmp_bpms, List<DELAYS.Data> tmp_delays, List<WARPS.Data> tmp_warps) {
            double beat = 0.0f;
            double second = -offset;
            double prv_second = -offset;

            //Set BPM
            if (tmp_bpms.Count == 0) {
                throw new IndexOutOfRangeException("Must have at least one BPM entry");
            }
            double bpm = tmp_bpms[0].bpm;
            tmp_bpms.RemoveAt(0);
            double seconds_per_beat = 60.0 / bpm;
            //Check initial bpm
            while (tmp_bpms.Count > 0 && tmp_bpms[0].beat <= beat) {
                BPMS.Data data = tmp_bpms[0];
                tmp_bpms.RemoveAt(0);

                bpm = data.bpm;
                seconds_per_beat = 60.0 / bpm;
            }
            //Check initial delay
            while (tmp_delays.Count > 0 && tmp_delays[0].beat <= beat) {
                DELAYS.Data data = tmp_delays[0];
                tmp_delays.RemoveAt(0);
                second += data.duration_second;
            }
            //Check initial warp
            while (tmp_warps.Count > 0 && tmp_warps[0].beat <= beat) {
                WARPS.Data data = tmp_warps[0];
                tmp_warps.RemoveAt(0);
                throw new NotImplementedException("Unsure how to handle warp at 0.0 beat");
            }
            WARPS.Data cur_warp = null;

            foreach (Measure m in measures) {
                foreach (Beat b in m.beats) {
                    while (tmp_delays.Count > 0 && tmp_delays[0].beat <= beat) {
                        DELAYS.Data data = tmp_delays[0];
                        tmp_delays.RemoveAt(0);
                        if (cur_warp != null) {
                            throw new NotImplementedException("Unsure how to handle");
                        }

                        second += data.duration_second;
                    }
                    while (tmp_warps.Count > 0 && tmp_warps[0].beat <= beat) {
                        WARPS.Data data = tmp_warps[0];
                        tmp_warps.RemoveAt(0);
                        if (cur_warp != null) {
                            throw new NotImplementedException("Unsure how to handle");
                        }

                        cur_warp = data;
                    }
                    b.bpm = (float)bpm;
                    b.seconds_per_beat = (float)seconds_per_beat;
                    b.beat = (float)beat;
                    b.second = (float)second;
                    b.delta_second = (float)second - (float)prv_second;
                    b.beat_interval = m.beats.Count;
                    prv_second = second;
                    if (cur_warp != null) {
                        for (int i = 0; i < b.notes.Count; ++i) {
                            b.notes[i] = NoteFactory.NONE;
                        }
                    }

                    double max_delta_beat = 4.0 / (double)m.beats.Count;
                    double step = 0.0;
                    double sub_step = 0.0;
                    if (m.beats.Count == 62) {
                        //System.Windows.Forms.MessageBox.Show("Test");
                    }
                    for (; step < max_delta_beat; step += SMALLEST_BEAT_STEP, sub_step += SMALLEST_SUB_BEAT) {
                        double nxt_beat = beat + sub_step;
                        if (cur_warp != null && nxt_beat >= cur_warp.beat + cur_warp.skip_beat) {
                            cur_warp = null;
                        }

                        if (tmp_bpms.Count > 0 && tmp_delays.Count > 0) {
                            if (tmp_bpms[0].beat == tmp_delays[0].beat) {
                                //Assume delay first, BPM next
                                //throw new NotImplementedException("Unsure which goes first");
                            }
                        }
                        if (tmp_delays.Count > 0 && tmp_warps.Count > 0) {
                            if (tmp_delays[0].beat == tmp_warps[0].beat) {
                                throw new NotImplementedException("Unsure which goes first");
                            }
                        }
                        while (tmp_bpms.Count > 0 && tmp_bpms[0].beat <= nxt_beat) {
                            BPMS.Data data = tmp_bpms[0];
                            if (data.beat == 209.333328f && data.bpm == 180.0f) {
                                //System.Windows.Forms.MessageBox.Show("Test");
                            }
                            tmp_bpms.RemoveAt(0);
                            if (cur_warp != null) {
                                //throw new NotImplementedException("Unsure how to handle");
                                //Just change the BPM
                            }

                            bpm = data.bpm;
                            seconds_per_beat = 60.0 / bpm;
                        }
                        if (cur_warp == null) {
                            second += SMALLEST_BEAT_STEP * seconds_per_beat;
                        }
                    }
                    beat += max_delta_beat;
                    if (cur_warp != null && beat >= cur_warp.beat + cur_warp.skip_beat) {
                        cur_warp = null;
                    }
                    if (cur_warp == null) {
                        if (max_delta_beat == 4.0f / 4.0f) {
                            //second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 8.0f) {
                            second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 12.0f) {
                            second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //throw new NotImplementedException();
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 16.0f) {
                            //second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 24.0f) {
                            //second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //throw new NotImplementedException();
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 32.0f) {
                            second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //throw new NotImplementedException();
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 48.0f) {
                            //second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //throw new NotImplementedException();
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 64.0f) {
                            //second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //throw new NotImplementedException();
                            //CHECKED
                        } else if (max_delta_beat == 4.0f / 192.0f) {
                            //second -= SMALLEST_BEAT_STEP * seconds_per_beat;
                            //beat -= SMALLEST_BEAT_STEP;
                            //throw new NotImplementedException();
                        } else {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
            return (float)second;
        }
    }
}
