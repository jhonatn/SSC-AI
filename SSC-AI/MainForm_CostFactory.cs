using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHS.SSC {
    using Parser;
    using Analyzer.State;
    public partial class MainForm {
        private CostFactory m_CostFactory = new CostFactory();
        private void InitCostFactory () {
            num_bracket_degree_cost.Value = (decimal)m_CostFactory.BAD_BRACKET_DEGREE_COST;
            num_bracket_degree_cost.ValueChanged += CostFactory_ValueChanged;
            num_bracket_degree_exp_const.Value = (decimal)m_CostFactory.BAD_BRACKET_DEGREE_EXP_CONST;
            num_bracket_degree_exp_const.ValueChanged += CostFactory_ValueChanged;
        }

        void CostFactory_ValueChanged (object sender, EventArgs e) {
            NumericUpDown num = (NumericUpDown)sender;
            float value = (float)num.Value;
            if (num == num_bracket_degree_cost) {
                m_CostFactory.BAD_BRACKET_DEGREE_COST = value;
            } else if (num == num_bracket_degree_exp_const) {
                m_CostFactory.BAD_BRACKET_DEGREE_EXP_CONST = value;
            }
        }

        public class Cost : ICost {
            public const int INDEX_FACING = 0;
            public const int INDEX_MOVE = 1;
            public const int INDEX_REST = 2;
            public const int INDEX_DOUBLE_STEP = 3;
            public const int INDEX_CROSSED = 4;
            public const int INDEX_BRACKET_ANGLE = 5;
            public const int INDEX_EXTRA_USE = 6;
            public const int INDEX_UNKNOWN_LIMB = 7;
            public const int INDEX_TOO_FAST = 8;

            public float facing = 0.0f;
            public float move = 0.0f;
            public float rest = 0.0f;
            public float double_step = 0.0f;
            public float crossed = 0.0f;
            public float bracket_angle = 0.0f;
            public float extra_use = 0.0f;
            public float unknown_limb = 0.0f;
            public float too_fast = 0.0f;
            //public float double_step_cost = 0.0f;
            //public float foot_switch_cost = 0.0f;
            //public float leg_and_hand_cost = 0.0f;
            //public float bad_bracketing_cost = 0.0f;
            //public float bracket_distance_cost = 0.0f;
            //public float part_preference_cost = 0.0f;
            //public float remove_from_use_cost = 0.0f;

            public float total_cost = 0.0f;

            public float GetCost (int index) {
                switch (index) {
                    case INDEX_FACING: return facing;
                    case INDEX_MOVE: return move;
                    case INDEX_REST: return rest;
                    case INDEX_DOUBLE_STEP: return double_step;
                    case INDEX_CROSSED: return crossed;
                    case INDEX_BRACKET_ANGLE: return bracket_angle;
                    case INDEX_EXTRA_USE: return extra_use;
                    case INDEX_UNKNOWN_LIMB: return unknown_limb;
                    case INDEX_TOO_FAST: return too_fast;
                }
                throw new IndexOutOfRangeException();
            }
            public float GetTotalCost () {
                return total_cost;
            }
        }
        public class CostFactory : ICostFactory {
            private static float ExpInterpolate (float t, float k) {
                return (float)(
                    (Math.Exp(t * k) - 1) /
                    (Math.Exp(k) - 1)
                );
            }



            private const float CROSSED_COST = 15.0f;
            public ICost Calculate (State state) {
                if (state.parent == null) {
                    return new Cost();
                }
                Cost result = new Cost();
                result.facing = CalculateFacing(state);
                result.move = CalculateMove(state);
                result.rest = CalculateRest(state);
                result.double_step = CalculateDoubleStep(state);
                result.crossed = state.limbs_crossed ? CROSSED_COST : 0.0f;
                result.bracket_angle = CalculateBracketAngle(state);
                result.extra_use = CalculateExtraUse(state);
                result.unknown_limb = CalculateUnknownLimb(state);
                result.too_fast = CalculateTooFast(state);
                #region Total Cost
                result.total_cost =
                    state.parent.cost.GetTotalCost() +
                    result.facing +
                    result.move +
                    result.rest +
                    result.double_step +
                    result.crossed +
                    result.bracket_angle +
                    result.extra_use +
                    result.unknown_limb +
                    result.too_fast;
                #endregion
                return result;
            }

            private static readonly Vector AVOID_FACING = new Vector(0.0f, -1.0f);
            private const float FACING_EXP_CONST = 8.0f;
            private const float FACING_COST = 20.0f;
            private const float FACING_MIN_CUTOFF = 0.4f;
            private float CalculateFacing (State state) {
                if (state.parent == null) { return 0.0f; }
                float cost = 0.0f;

                float facing_ratio = state.facing_desired.dot(AVOID_FACING) + 1.0f;
                facing_ratio /= 2.0f;
                facing_ratio = (float)((Math.Exp(facing_ratio * FACING_EXP_CONST) - 1) / (Math.Exp(FACING_EXP_CONST) - 1));
                cost += facing_ratio * FACING_COST;
                return (cost < FACING_MIN_CUTOFF) ? 0.0f : cost;
            }
            public const float MOVE_COST_MULTIPLIER = 0.05f;//0.7f;
            public const float NO_MOVE_COST = 0.0f;
            public const float MOVE_FROM_UNKNOWN_COST = 0.1f;
            public const float MOVE_TO_UNKNOWN_COST = 0.1f;
            private float CalculateMove (State state) {
                if (state.parent == null) { return 0.0f; }
                float cost = 0.0f;
                State prv = state.parent;
                for (int limb_index = 0; limb_index < state.limbs.Length; ++limb_index) {
                    Limb cur_limb = state.limbs[limb_index];
                    Limb prv_limb = prv.limbs[limb_index];

                    for (int part_index = 0; part_index < Limb.PART_COUNT; ++part_index) {
                        Part cur_part = cur_limb[part_index];
                        Part prv_part = prv_limb[part_index];

                        if (prv_part.panel == cur_part.panel) {
                            //It's a jack or didn't move
                            cost += NO_MOVE_COST;
                        } else {
                            //Calculate move cost
                            if (prv_part.IsUnknown()) {
                                cost += MOVE_FROM_UNKNOWN_COST;
                            } else if (cur_part.IsUnknown()) {
                                cost += MOVE_TO_UNKNOWN_COST;
                            } else {
                                float distance = Panel.DistanceTo(prv_part.panel.index, cur_part.panel.index);
                                cost += MOVE_COST_MULTIPLIER * distance;
                            }
                        }
                    }
                }
                return cost;
            }

            private const float MAX_REST_TIME = 3.0f;
            private const float MIN_REST_TIME = 0.0001f;
            private const float DELTA_REST_TIME = MAX_REST_TIME - MIN_REST_TIME;
            private const float REST_COST = 15.0f;
            private const float REST_EXP_CONST = 2.0f;
            private const float REST_JACK_COST_MULTIPLIER = 0.07f;
            private const float REST_JUMP_JACK_COST_MULTIPLIER = 1.0f;//0.3f;
            private const float REST_MIN_CUTOFF = 0.001f;
            private bool CalculateRest (float cur_second, float prv_second, out float result) {
                float rest_time = cur_second - prv_second;
                if (rest_time > MAX_REST_TIME) { rest_time = MAX_REST_TIME; }
                if (rest_time < MIN_REST_TIME) { rest_time = MIN_REST_TIME; }
                float rest_ratio = (rest_time - MIN_REST_TIME) / DELTA_REST_TIME;
                float cost_ratio = (1.0f - rest_ratio);
                cost_ratio = ExpInterpolate(cost_ratio, REST_EXP_CONST);
                float cur_cost = cost_ratio * REST_COST;
                result = cur_cost;
                if (cur_cost < REST_MIN_CUTOFF) {
                    result = REST_MIN_CUTOFF;
                }
                return true;
                //return cur_cost > REST_MIN_CUTOFF;
                //return (cur_cost < REST_MIN_CUTOFF) ? 0.0f : cur_cost;
            }
            private float CalculateRest (State state) {
                if (state.parent == null) { return 0.0f; }
                bool is_jump = IsJump(state.limbs);
                bool is_just_bracket = IsJustBracket(state.limbs);

                float cost = is_jump ? float.PositiveInfinity : float.NegativeInfinity;

                State prv = state.parent;
                for (int limb_index = 0; limb_index < state.limbs.Length; ++limb_index) {
                    Limb cur_limb = state.limbs[limb_index];
                    Limb prv_limb = prv.limbs[limb_index];
                    if (prv.distance_from_start >= 0 && IsGallop(cur_limb, prv_limb, state.beat, prv.beat)) {
                        continue;
                    }
                    for (int i = 0; i < Limb.PART_COUNT; ++i) {
                        Part part = cur_limb[i];
                        if (part.cur_moved_second != part.cur_second) { continue; }
                        {
                            float cur_cost;
                            bool has_cost = CalculateRest(part.cur_moved_second, part.prv_moved_second, out cur_cost);
                            if (prv_limb[i].panel == part.panel) {
                                if (is_jump) {
                                    cur_cost *= REST_JUMP_JACK_COST_MULTIPLIER;
                                } else {
                                    cur_cost *= REST_JACK_COST_MULTIPLIER;
                                }
                            }
                            if (has_cost) {
                                if (is_jump) {
                                    if (cur_cost < cost) {
                                        cost = cur_cost;
                                    }
                                } else {
                                    if (cur_cost > cost) {
                                        cost = cur_cost;
                                    }
                                }
                            }
                        }
                        if (float.IsInfinity(cost)) {
                            cost = float.NegativeInfinity;
                        }
                        for (int j = 0; j < Limb.PART_COUNT; ++j) {
                            if (i == j) { continue; }
                            Part other = cur_limb[j];
                            if (other.cur_moved_second == 0.0f) { continue; }
                            if (part.cur_moved_second == other.cur_moved_second) { continue; }

                            float cur_cost;
                            bool has_cost = CalculateRest(part.cur_moved_second, other.cur_moved_second, out cur_cost);
                            if (prv_limb[i].panel == part.panel) {
                                if (is_jump) {
                                    cur_cost *= REST_JUMP_JACK_COST_MULTIPLIER;
                                } else {
                                    cur_cost *= REST_JACK_COST_MULTIPLIER;
                                }
                            }
                            if (has_cost && cur_cost > cost) {
                                cost = cur_cost;
                            }
                        }
                    }
                }
                if (float.IsInfinity(cost)) {
                    return 0.0f;
                }
                if (is_jump || is_just_bracket) {
                    return 0.0f;
                }
                if (IsJump(prv.limbs) || IsJustBracket(prv.limbs)) {
                    return 0.0f;
                }
                return cost;
            }
            private const int GALLOP_BEAT_INTERVAL = 48;
            private const float GALLOP_BEATS_PER_MEASURE = (float)GALLOP_BEAT_INTERVAL / 4.0f;
            private const float GALLOP_SECONDS_EPOCH = 0.00001f;
            private bool IsGallop (
                Limb cur, Limb prv,
                Beat cur_beat, Beat prv_beat
            ) {
                if (
                    cur_beat.beat_interval < GALLOP_BEAT_INTERVAL ||
                    prv_beat.beat_interval < GALLOP_BEAT_INTERVAL
                ) {
                    return false; //Consider making it.. 24?
                }
                float delta_second = cur_beat.second - prv_beat.second;
                if (delta_second > cur_beat.seconds_per_beat / GALLOP_BEATS_PER_MEASURE + GALLOP_SECONDS_EPOCH) {
                    return false; //Unsure if this is correct..
                }
                if (cur.JustMovedCount() != 1 || prv.JustMovedCount() != 1) {
                    return false; //For now, gallops are two taps..
                }
                int cur_just_moved_index = cur.JustMovedIndex();
                int prv_just_moved_index = prv.JustMovedIndex();
                if (cur_just_moved_index == prv_just_moved_index) {
                    return false; //.. by different parts of the same limb
                }

                Part cur_part = cur[cur_just_moved_index];
                Part prv_part = prv[prv_just_moved_index];
                if (cur_part.panel == null || prv_part.panel == null) {
                    return false; //Must be hitting a note
                }
                if (cur_part.panel == prv_part.panel) {
                    return false; //Must hit different notes
                }
                if (!Panel.IsBracketable(
                    cur_part.panel.index, prv_part.panel.index
                )) {
                    return false; //Must be bracketable
                }
                return true;
            }
            private static bool IsJack (Limb cur, Limb prv) {
                int cur_moved_count = 0;
                int prv_moved_count = 0;
                for (int i = 0; i < Limb.PART_COUNT; ++i) {
                    if (cur[i].JustMoved() && !cur[i].IsUnknown()) {
                        ++cur_moved_count;
                    }
                }
                for (int i = 0; i < Limb.PART_COUNT; ++i) {
                    if (prv[i].JustMoved() && !prv[i].IsUnknown()) {
                        ++prv_moved_count;
                    }
                }
                bool is_jack = false;
                if (cur_moved_count >= prv_moved_count) {
                    int matches = 0;
                    for (int i = 0; i < Limb.PART_COUNT; ++i) {
                        if (!prv[i].JustMoved() || prv[i].IsUnknown()) {
                            continue;
                        }
                        bool is_currently_occupied = false;
                        for (int j = 0; j < Limb.PART_COUNT; ++j) {
                            if (cur[j].JustMoved() && !cur[j].IsUnknown()) {
                                //I can just compare panel instances but I'm too paranoid about null-ref errors
                                if (prv[i].panel.index == cur[j].panel.index) {
                                    is_currently_occupied = true;
                                }
                            }
                        }
                        if (is_currently_occupied) {
                            ++matches;
                        } else {
                            break;
                        }
                    }
                    if (matches == prv_moved_count) {
                        is_jack = true;
                    }
                }
                return is_jack;
            }
            private static bool IsJump (Limb[] limbs) {
                foreach (Limb limb in limbs) {
                    int moved = 0;
                    for (int p=0; p<Limb.PART_COUNT; ++p) {
                        Part part = limb[p];
                        if (part.JustMoved() && !part.IsUnknown() && !part.IsPassiveDown()) {
                            ++moved;
                        }
                    }
                    if (moved == 0) { return false; }
                }
                return true;
            }
            private static bool IsJustBracket (Limb[] limbs) {
                foreach (Limb limb in limbs) {
                    int moved = 0;
                    for (int p = 0; p < Limb.PART_COUNT; ++p) {
                        Part part = limb[p];
                        if (part.JustMoved() && !part.IsUnknown() && !part.IsPassiveDown()) {
                            ++moved;
                        }
                    }
                    if (moved >= 2) {
                        return true;
                    }
                }
                return false;
            }
            private const float DOUBLE_STEP_COST = 22.0f;
            private const float DOUBLE_STEP_COST_DELTA_PER_NODE = 0.001f;
            private const float JACK_COST = 5.0f;
            private float CalculateDoubleStep (Limb cur, Limb prv, int distance_from_start) {
                if (IsJack(cur, prv)) {
                    return 0.0f;
                }
                float cost = 0.0f;
                cost += DOUBLE_STEP_COST;
                cost += DOUBLE_STEP_COST_DELTA_PER_NODE * distance_from_start;
                return cost;
            }
            private float CalculateDoubleStep (State cur) {
                if (cur.parent == null) { return 0.0f; }
                if (cur.limbs[0].JustMoved() && cur.limbs[1].JustMoved()) {
                    return 0.0f;
                }

                State prv = cur.parent;
                if (prv.limbs[0].JustMoved() && prv.limbs[1].JustMoved()) {
                    return 0.0f;
                }

                float cost = 0.0f;
                for (int limb_index = 0; limb_index < cur.limbs.Length; ++limb_index) {
                    if (
                        !cur.limbs[limb_index].JustMoved() ||
                        !prv.limbs[limb_index].JustMoved()
                    ) {
                        continue;
                    }
                    Limb cur_limb = cur.limbs[limb_index];
                    Limb prv_limb = prv.limbs[limb_index];
                    if (prv.distance_from_start >= 0 && IsGallop(cur_limb, prv_limb, cur.beat, prv.beat)) {
                        continue;
                    }
                    float cur_cost = CalculateDoubleStep(cur_limb, prv_limb, cur.distance_from_start);
                    cost += cur_cost;
                    break;
                }
                return cost;
            }
            private float AVOID_BRACKET_DEGREE_LEFT_A = -180.0f + 45.0f;
            private float AVOID_BRACKET_DEGREE_LEFT_B = -90.0f;
            private float AVOID_BRACKET_DEGREE_RIGHT_A = +180.0f - 45.0f;
            private float AVOID_BRACKET_DEGREE_RIGHT_B = +90.0f;

            public float BAD_BRACKET_DEGREE_COST = 20.0f;
            public float BAD_BRACKET_DEGREE_EXP_CONST = 1.0f;
            private float CalculateBracketAngle (
                float facing_deg, Vector avoid_bracket_dir_a, Vector avoid_bracket_dir_b,
                bool is_left, Limb limb
            ) {
                if (!limb.IsActiveBracket()) { return 0.0f; }
                Vector bracket_dir = Panel.GetBracketDirection(
                    is_left,
                    limb.main.IsUnknown() || limb.main.IsPassiveDown() ? -1 : limb.main.panel.index,
                    limb.sub.IsUnknown() || limb.sub.IsPassiveDown() ? -1 : limb.sub.panel.index,
                    limb.extra.IsUnknown() || limb.extra.IsPassiveDown() ? -1 : limb.extra.panel.index
                );
                if (facing_deg == 0.0f) {
                    if (bracket_dir.dx == 1.0f && bracket_dir.dy == 0.0f) {
                        return 0.0f;
                    }
                } else if (facing_deg == 180.0f) {
                    if (bracket_dir.dx == -1.0f && bracket_dir.dy == 0.0f) {
                        return 0.0f;
                    }
                }
                float alignment_a = (bracket_dir.dot(avoid_bracket_dir_a) + 1.0f) / 2.0f;
                float alignment_b = (bracket_dir.dot(avoid_bracket_dir_b) + 1.0f) / 2.0f;
                float alignment = alignment_a < alignment_b ?
                    alignment_a : alignment_b;
                return BAD_BRACKET_DEGREE_COST * ExpInterpolate(alignment, BAD_BRACKET_DEGREE_EXP_CONST);
            }
            private float CalculateBracketAngle (State cur) {
                if (cur.parent == null) { return 0.0f; }

                float cost = 0.0f;
                float facing_deg = cur.facing_desired.toDegree();

                float avoid_bracket_deg_left_a = facing_deg + AVOID_BRACKET_DEGREE_LEFT_A;
                float avoid_bracket_deg_left_b = facing_deg + AVOID_BRACKET_DEGREE_LEFT_B;
                float avoid_bracket_deg_right_a = facing_deg + AVOID_BRACKET_DEGREE_RIGHT_A;
                float avoid_bracket_deg_right_b = facing_deg + AVOID_BRACKET_DEGREE_RIGHT_B;
                Vector avoid_bracket_dir_left_a = Vector.FromDegree(avoid_bracket_deg_left_a);
                Vector avoid_bracket_dir_left_b = Vector.FromDegree(avoid_bracket_deg_left_b);
                Vector avoid_bracket_dir_right_a = Vector.FromDegree(avoid_bracket_deg_right_a);
                Vector avoid_bracket_dir_right_b = Vector.FromDegree(avoid_bracket_deg_right_b);
                cost += CalculateBracketAngle(
                    facing_deg, avoid_bracket_dir_left_a, avoid_bracket_dir_left_b,
                    true, cur.limbs[0]
                );
                cost += CalculateBracketAngle(
                    facing_deg, avoid_bracket_dir_right_a, avoid_bracket_dir_right_b,
                    false, cur.limbs[1]
                );
                /*foreach (Limb limb in cur.limbs) {
                    cost += CalculateBracketAngle(
                        facing_deg, avoid_bracket_dir_a, avoid_bracket_dir_b,
                        limb == cur.limbs[0], limb
                    );
                }//*/
                return cost;
            }
            private float EXTRA_USE_COST = 30.0f;
            private float CalculateExtraUse (State state) {
                if (state.parent == null) { return 0.0f; }
                float cost = 0.0f;
                foreach (Limb limb in state.limbs) {
                    if (!limb.extra.IsUnknown()) {
                        cost += EXTRA_USE_COST;
                    }
                }
                return cost;
            }
            private float UNKNOWN_LIMB_COST = 10.0f;
            private float CalculateUnknownLimb (State state) {
                if (state.parent == null) { return 0.0f; }
                float cost = 0.0f;
                bool is_gallop = false;
                for (int limb_index = 0; limb_index < state.limbs.Length; ++limb_index) {
                    Limb limb = state.limbs[limb_index];
                    if (limb.IsUnknown()) {
                        cost += UNKNOWN_LIMB_COST;
                    }
                    if (state.parent.distance_from_start >= 0 && IsGallop(limb, state.parent.limbs[limb_index], state.beat, state.parent.beat)) {
                        is_gallop = true;
                    }
                }
                return is_gallop ? 0.0f : cost;
            }
            private float TOO_FAST_COST = 30.0f;
            private float CalculateTooFast (
                Limb cur, Limb prv,
                Beat cur_beat, Beat prv_beat
            ) {
                if (IsGallop(cur, prv, cur_beat, prv_beat)) {
                    return 0.0f;
                }
                if (
                    cur_beat.beat_interval < GALLOP_BEAT_INTERVAL ||
                    prv_beat.beat_interval < GALLOP_BEAT_INTERVAL
                ) {
                    return 0.0f; //Consider making it.. 24?
                }
                float delta_second = cur_beat.second - prv_beat.second;
                if (
                    delta_second > (cur_beat.seconds_per_beat / GALLOP_BEATS_PER_MEASURE + GALLOP_SECONDS_EPOCH) * 2.0f) {
                    return 0.0f; //Unsure if this is correct..
                }
                List<int> cur_indices = cur.JustMovedIndices();
                List<int> prv_indices = prv.JustMovedIndices();
                if (cur_indices.Count == 0 || prv_indices.Count == 0) {
                    return 0.0f;
                }
                float cost = 0.0f;
                for (int c = 0; c < cur_indices.Count; ++c) {
                    int cur_index = cur_indices[c];
                    Part cur_part = cur[cur_index];
                    for (int p = 0; p < prv_indices.Count; ++p) {
                        int prv_index = prv_indices[p];
                        Part prv_part = prv[prv_index];
                        if (cur_part.IsUnknown() || prv_part.IsUnknown()) { continue; }
                        if (cur_index == prv_index) {
                            if (cur_part.panel == prv_part.panel) {
                                //Do nothing?
                            } else {
                                cost += TOO_FAST_COST;
                            }
                        } else {
                            if (cur_part.panel == prv_part.panel) {
                                //Different parts but same panel, a.. part switch?
                                cost += TOO_FAST_COST;//For now, treat it as too fast
                            } else if (Panel.IsBracketable(cur_part.panel.index, prv_part.panel.index)) {
                                //Do nothing, it's a gallop-ish thing
                            } else {
                                //Too fast, probably
                                cost += TOO_FAST_COST;
                            }
                        }
                    }
                }
                return cost;
            }
            private float CalculateTooFast (State cur, State prv) {
                float cost = 0.0f;
                for (int limb_index = 0; limb_index < cur.limbs.Length; ++limb_index) {
                    Limb cur_limb = cur.limbs[limb_index];
                    Limb prv_limb = prv.limbs[limb_index];
                    cost += CalculateTooFast(cur_limb, prv_limb, cur.beat, prv.beat);
                }
                return cost;
            }
            private float CalculateTooFast (State state) {
                float cost = 0.0f;
                //Compare cur and prv
                //Impose cost if the limb is moving too fast and isn't a gallop
                if (state.parent == null) { return cost; }
                if (state.parent.beat == null) { return cost; }
                cost += CalculateTooFast(state, state.parent);
                //Compare cur and prv.prv
                //Impose cost if the limb is moving too fast and isn't a gallop
                if (state.parent.parent == null) { return cost; }
                if (state.parent.parent.beat == null) { return cost; }
                cost += CalculateTooFast(state, state.parent.parent);

                return cost;
            }
        }
    }
}
