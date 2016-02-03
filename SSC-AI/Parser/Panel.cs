using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Parser {
    public class Panel {
        #region Data
        public readonly Position position;
        public readonly PanelDirectionX direction_x;
        public readonly PanelDirectionY direction_y;
        public readonly bool playable;
        public readonly int player_index;

        public readonly int index_x;
        public readonly int index_y;
        public readonly int index;
        public readonly int index_playable;

        private Panel (
            Position position,
            PanelDirectionX direction_x, PanelDirectionY direction_y,
            bool playable, int player_index,
            int index_x, int index_y, int index, int index_playable
        ) {
            this.position = position;
            this.direction_x = direction_x;
            this.direction_y = direction_y;
            this.playable = playable;
            this.player_index = player_index;
            this.index_x = index_x;
            this.index_y = index_y;
            this.index = index;
            this.index_playable = index_playable;
        }
        #endregion
        #region Static Calculation Cache
        public const float START_X = 0.0f;
        public const float START_Y = 0.0f;
        public const float DELTA_X = 1.1f;
        public const float DELTA_Y = 1.0f;

        public const int COUNT_X = 6;
        public const int COUNT_Y = 3;
        public static readonly Panel[,] Panels = new Panel[COUNT_X, COUNT_Y];
        public static readonly Panel[] Panels_1D = new Panel[COUNT_X * COUNT_Y];
        public static readonly Panel[] Panels_1D_Playable = new Panel[10];
        public static readonly Vector[, , ,] Vectors = new Vector[COUNT_X, COUNT_Y, COUNT_X, COUNT_Y];
        public static readonly float[, , ,] Distances = new float[COUNT_X, COUNT_Y, COUNT_X, COUNT_Y];

        public static readonly Vector[, , ,] Directions = new Vector[COUNT_X, COUNT_Y, COUNT_X, COUNT_Y];
        public static readonly float[, , ,] Radians = new float[COUNT_X, COUNT_Y, COUNT_X, COUNT_Y];
        public static readonly float[, , ,] Degrees = new float[COUNT_X, COUNT_Y, COUNT_X, COUNT_Y];

        public static readonly Vector[, , ,] Facings = new Vector[COUNT_X, COUNT_Y, COUNT_X, COUNT_Y];
        public static readonly Vector[, , ,] ReverseFacings = new Vector[COUNT_X, COUNT_Y, COUNT_X, COUNT_Y];

        public static readonly List<Panel>[] Neighbours_1D = new List<Panel>[COUNT_X * COUNT_Y];
        public static readonly List<Panel>[] SubNeighbours_1D = new List<Panel>[COUNT_X * COUNT_Y];

        private static void InitPanels () {
            int playable_count = 0;
            for (int x = 0; x < COUNT_X; ++x) {
                for (int y = 0; y < COUNT_Y; ++y) {
                    Position position = new Position(
                        START_X + DELTA_X * x,
                        START_Y + DELTA_Y * y
                    );
                    PanelDirectionX direction_x = (PanelDirectionX)(x % 3);
                    PanelDirectionY direction_y = (PanelDirectionY)y;
                    bool playable = (
                        (direction_x == PanelDirectionX.Center) ==
                        (direction_y == PanelDirectionY.Center)
                    );
                    int player_index = (x < COUNT_X / 2) ? 0 : 1;
                    int index = x + (y * COUNT_X);
                    int index_playable = playable ?
                        (playable_count++) : -1;
                    #region Hard Coded Hack
                    if (index_playable == 3) {
                        index_playable = 4;
                    } else if (index_playable == 4) {
                        index_playable = 3;
                    } else if (index_playable == 8) {
                        index_playable = 9;
                    } else if (index_playable == 9) {
                        index_playable = 8;
                    }
                    #endregion
                    Panel panel = new Panel(
                        position,
                        direction_x,
                        direction_y,
                        playable,
                        player_index,
                        x, y,
                        index,
                        index_playable
                    );
                    Panels[x, y] = panel;
                    Panels_1D[index] = panel;
                    if (playable) {
                        Panels_1D_Playable[index_playable] = panel;
                    }
                }
            }
        }
        private static void InitCalculations () {
            for (int from_x = 0; from_x < COUNT_X; ++from_x) {
                for (int from_y = 0; from_y < COUNT_Y; ++from_y) {
                    for (int to_x = 0; to_x < COUNT_X; ++to_x) {
                        for (int to_y = 0; to_y < COUNT_Y; ++to_y) {
                            Position from = Panels[from_x, from_y].position;
                            Position to = Panels[to_x, to_y].position;
                            Vector v = from.vectorTo(to);
                            Vector dir = v.normalized();
                            Vectors[
                                from_x, from_y,
                                to_x, to_y
                            ] = v;
                            Distances[
                                from_x, from_y,
                                to_x, to_y
                            ] = v.magnitude();

                            Directions[
                                from_x, from_y,
                                to_x, to_y
                            ] = dir;
                            Radians[
                                from_x, from_y,
                                to_x, to_y
                            ] = dir.toRadian();
                            Degrees[
                                from_x, from_y,
                                to_x, to_y
                            ] = dir.toDegree();

                            Vector facing = v.perpendicular().normalized();
                            Facings[
                                from_x, from_y,
                                to_x, to_y
                            ] = facing;
                            ReverseFacings[
                                from_x, from_y,
                                to_x, to_y
                            ] = facing.reverse();
                        }
                    }
                }
            }
        }
        private static void InitNeighbours () {
            for (int i = 0; i < Panels_1D.Length; ++i) {
                List<Panel> neighbours = new List<Panel>();
                List<Panel> sub_neighbours = new List<Panel>();
                for (int n = 0; n < Panels_1D.Length; ++n) {
                    if (i == n) { continue; }
                    if (IsBracketable(i, n)) {
                        neighbours.Add(Panels_1D[n]);
                    }
                    if (IsSubBracketable(i, n)) {
                        sub_neighbours.Add(Panels_1D[n]);
                    }
                }
                Neighbours_1D[i] = neighbours;
                SubNeighbours_1D[i] = sub_neighbours;
            }
        }
        static Panel () {
            try {
                InitPanels();
                InitCalculations();
                InitNeighbours();
            } catch (Exception ex) {
                throw ex;
            }
        }
        #endregion
        public static Vector VectorTo (int from_index, int to_index) {
#if DEBUG
            if (from_index == to_index) { throw new ArgumentException(); }
#endif
            Panel from = Panels_1D[from_index];
            Panel to = Panels_1D[to_index];
            return Vectors[
                from.index_x, from.index_y,
                to.index_x, to.index_y
            ];
        }
        public static Vector DirectionTo (int from_index, int to_index) {
#if DEBUG
            if (from_index == to_index) { throw new ArgumentException(); }
#endif
            Panel from = Panels_1D[from_index];
            Panel to = Panels_1D[to_index];
            return Directions[
                from.index_x, from.index_y,
                to.index_x, to.index_y
            ];
        }
        public static float DistanceTo (int from_index, int to_index) {
#if DEBUG
            if (from_index == to_index) { throw new ArgumentException(); }
#endif
            Panel from = Panels_1D[from_index];
            Panel to = Panels_1D[to_index];
            return Distances[
                from.index_x, from.index_y,
                to.index_x, to.index_y
            ];
        }
        public static Vector GetFacing (int left_index, int right_index) {
#if DEBUG
            if (left_index == right_index) { throw new ArgumentException(); }
#endif
            Panel from = Panels_1D[left_index];
            Panel to = Panels_1D[right_index];
            return Facings[
                from.index_x, from.index_y,
                to.index_x, to.index_y
            ];
        }
        public static Vector GetReverseFacing (int left_index, int right_index) {
#if DEBUG
            if (left_index == right_index) { throw new ArgumentException(); }
#endif
            Panel from = Panels_1D[left_index];
            Panel to = Panels_1D[right_index];
            return ReverseFacings[
                from.index_x, from.index_y,
                to.index_x, to.index_y
            ];
        }
        #region Util
        public static Vector GetFacing (bool a_is_left_leg, int a, int b) {
            return a_is_left_leg ?
                GetFacing(a, b) : GetFacing(b, a);
        }
        #endregion
        #region One-Foot Bracketable
        public const float BRACKETABLE_MAX_DISTANCE = DELTA_Y * 2.0f;
        public const float SUB_BRACKETABLE_MAX_DISTANCE = BRACKETABLE_MAX_DISTANCE - 0.1f;
        public static bool IsBracketable (int from_index, int to_index) {
            return DistanceTo(from_index, to_index) <= BRACKETABLE_MAX_DISTANCE;
        }
        public static bool IsSubBracketable (int from_index, int to_index) {
            return DistanceTo(from_index, to_index) <= SUB_BRACKETABLE_MAX_DISTANCE;
        }
        public static bool IsBracketable (int _0, int _1, int _2) {
            if (
                IsSubBracketable(_0, _1) &&
                IsBracketable(_1, _2) &&
                IsBracketable(_2, _0)
            ) {
                return true;
            } else if (
                IsBracketable(_0, _1) &&
                IsSubBracketable(_1, _2) &&
                IsBracketable(_2, _0)
            ) {

                return true;
            } else if (
                IsBracketable(_0, _1) &&
                IsBracketable(_1, _2) &&
                IsSubBracketable(_2, _0)
            ) {
                return true;
            } else {
                return false;
            }
        }
        #endregion
        #region One-Foot Bracket Directions
        public static Vector GetBracketDirection (int _0, int _1) {
            return DirectionTo(_0, _1);
        }
        public static Vector GetBracketDirectionMainSub (int _0, int _1) {
            return DirectionTo(_0, _1);
        }
        public static Vector GetBracketDirectionMainExtra (int _0, int _2) {
            return DirectionTo(_0, _2);
        }
        public static Vector GetBracketDirectionSubExtra (bool is_left_leg, int _1, int _2) {
            return is_left_leg ?
                GetReverseFacing(_1, _2) :
                GetFacing(_1, _2);
        }
        public static Vector GetBracketDirection (bool is_left_leg, int _0, int _1, int _2) {
            if (_0 >= 0) {
                if (_1 >= 0) {
                    return GetBracketDirectionMainSub(_0, _1);
                } else if (_2 >= 0) {
                    return GetBracketDirectionMainExtra(_0, _2);
                } else {
                    throw new ArgumentException();
                }
            } else if (_1 >= 0) {
                if (_2 >= 0) {
                    return GetBracketDirectionSubExtra(is_left_leg, _1, _2);
                } else {
                    throw new ArgumentException();
                }
            } else {
                throw new ArgumentException();
            }
        }

        public const float BRACKETABLE_LEFT_CCW = 136.0f * CONSTS.DEG2RAD;
        public const float BRACKETABLE_LEFT_CW  = 91.0f  * CONSTS.DEG2RAD;//-65.0f * CONSTS.DEG2RAD;
        public const float BRACKETABLE_RIGHT_CCW = 91.0f * CONSTS.DEG2RAD;//+65.0f * CONSTS.DEG2RAD;
        public const float BRACKETABLE_RIGHT_CW  = 136.0f * CONSTS.DEG2RAD;
        public static bool IsBracketableDirection (float facing_rad, float bracket_rad, bool is_left_leg) {
            if (facing_rad < 0.0f) {
                facing_rad += CONSTS.TAU;
            }
            if (bracket_rad < 0.0f) {
                bracket_rad += CONSTS.TAU;
            }
            if (is_left_leg) {
                return
                    Angle.CCWRadTo(facing_rad, bracket_rad) < BRACKETABLE_LEFT_CCW ||
                    Angle.CWRadTo(facing_rad, bracket_rad)  < BRACKETABLE_LEFT_CW;
            } else {
                return
                    Angle.CCWRadTo(facing_rad, bracket_rad) < BRACKETABLE_RIGHT_CCW ||
                    Angle.CWRadTo(facing_rad, bracket_rad)  < BRACKETABLE_RIGHT_CW;
            }
        }
        public static bool IsBracketableDirection (float facing_rad, bool is_left_leg, int _0, int _1) {
            Vector bracket_dir = GetBracketDirection(_0, _1);
            float bracket_rad = bracket_dir.toRadian();

            return IsBracketableDirection(facing_rad, bracket_rad, is_left_leg);
        }
        #endregion
        #region Two-Feet Bracketable
        public static bool IsBracketableDirection (
            float facing_rad,
            bool a_is_left_leg,
            int a_0, int a_1,
            int b_0, int b_1
        ) {
            return
                IsBracketableDirection(facing_rad, a_is_left_leg, a_0, a_1) &&
                IsBracketableDirection(facing_rad, !a_is_left_leg, b_0, b_1);
        }
        #endregion
        public static Panel CalculateBestBracketable (Panel anchor, Panel other) {
            List<Panel> neighbours = Neighbours_1D[anchor.index];
            Panel nearest = null;
            float distance = float.PositiveInfinity;
            foreach (Panel n in neighbours) {
                float d = Panel.DistanceTo(other.index, n.index);
                if (d < distance) {
                    nearest = n;
                    distance = d;
                }
            }
            if (nearest == null) {
                throw new ArgumentException();
            }
            return nearest;
        }
        public static Panel CalculateBestSubBracketable (Panel anchor, Panel other) {
            List<Panel> neighbours = SubNeighbours_1D[anchor.index];
            Panel nearest = null;
            float distance = float.PositiveInfinity;
            foreach (Panel n in neighbours) {
                float d = Panel.DistanceTo(other.index, n.index);
                if (d < distance) {
                    nearest = n;
                    distance = d;
                }
            }
            if (nearest == null) {
                throw new ArgumentException();
            }
            return nearest;
        }
        public static bool IsValidOrNot3Bracket (bool is_left_leg, int _0, int _1, int _2) {
            if (_0 >= 0 && _1 >= 0 && _2 >= 0) {
                if (!IsBracketable(_0, _1)) { return false; }
                if (!IsBracketable(_0, _2)) { return false; }
                if (!IsSubBracketable(_1, _2)) { return false; }
                Vector dir_sub   = DirectionTo(_0, _1);
                Vector dir_extra = DirectionTo(_0, _2);
                float determinant = dir_sub.determinant(dir_extra);
                if (is_left_leg) {
                    return determinant > 0.0f;
                } else {
                    return determinant < 0.0f;
                }
            } else {
                return true; //not 3-bracket
            }
        }
    }
}
