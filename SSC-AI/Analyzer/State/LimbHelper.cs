using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.State {
    using Parser;
    using Node;
    public static class LimbHelper {
        public static bool IsArcValid (Limb from, Analyzer.Node.Limb to) {
            if (to == null) { return true; }
            for (int i = 0; i < Limb.PART_COUNT; ++i) {
                if (!PartHelper.IsArcValid(from[i], to[i])) {
                    return false;
                }
            }
            return true;
        }
        public class Fail {
            public bool main_sub;
            public bool main_extra;
            public bool sub_extra;
            public Fail (Limb limb) {
                main_sub = !Panel.IsBracketable(
                        limb.main.panel.index,
                        limb.sub.panel.index
                    );
                main_extra = !Panel.IsBracketable(
                    limb.main.panel.index,
                    limb.extra.panel.index
                );
                sub_extra = (limb.sub.panel == limb.extra.panel) ?
                    false :
                    !Panel.IsSubBracketable(
                        limb.sub.panel.index,
                        limb.extra.panel.index
                    );
            }
        }
        public static Limb TransitionTo (Limb from, Analyzer.Node.Limb to, float cur_second) {
            if (!IsArcValid(from, to)) {
                throw new ArgumentException();
            }

            if (to == null) {
                throw new ArgumentException();
                /*Limb nxt = new Limb();
                for (int i = 0; i < Limb.PART_COUNT; ++i) {
                    Part part = PartHelper.TransitionTo(from[i], null, cur_second);
                    nxt[i] = part;
                }
                nxt.sanityCheck();
                return nxt;*/
            } else {
                Limb nxt = new Limb();
                for (int i = 0; i < Limb.PART_COUNT; ++i) {
                    Part part = PartHelper.TransitionTo(from[i], to[i], cur_second);
                    nxt[i] = part;
                }
                if (to[Analyzer.Node.Limb.INDEX_EXTRA] == null) {
                    nxt.extra = nxt.sub;
                }
                bool has_main = to.main != null;
                bool has_sub = to.sub != null;
                bool has_extra = to.extra != null;
                Fail fail = new Fail(nxt);
                if (has_main) {
                    if (has_sub) {
                        if (has_extra) {
                            //main+sub+extra
                            if (fail.main_sub) {
                                throw new ArgumentException();
                            }
                            if (fail.main_extra) {
                                throw new ArgumentException();
                            }
                            if (fail.sub_extra) {
                                throw new ArgumentException();
                            }
                        } else {
                            //main+sub
                            if (fail.main_sub) {
                                throw new ArgumentException();
                            }
                            if (fail.main_extra) {
                                nxt.extra = nxt.sub;
                                fail = new Fail(nxt);
                            }
                            if (fail.sub_extra) {
                                nxt.extra = nxt.sub;
                                fail = new Fail(nxt);
                            }
                        }
                    } else if (has_extra) {
                        //main+extra
                        if (fail.main_sub) {
                            nxt.sub = nxt.extra;
                            fail = new Fail(nxt);
                        }
                        if (fail.main_extra) {
                            throw new ArgumentException();
                        }
                        if (fail.sub_extra) {
                            nxt.sub = nxt.extra;
                            fail = new Fail(nxt);
                        }
                    } else {
                        //main
                        if (fail.main_sub) {
                            Panel n = Panel.CalculateBestBracketable(nxt.main.panel, nxt.sub.panel);
                            Part src = nxt.sub;
                            nxt.sub = new Part(
                                Movement.Unknown,
                                n,
                                src.cur_second,
                                cur_second,
                                src.cur_moved_second == cur_second ?
                                    src.prv_moved_second : src.cur_moved_second
                            );
                            fail = new Fail(nxt);
                        }
                        if (fail.main_extra) {
                            nxt.extra = nxt.sub;
                            fail = new Fail(nxt);
                        }
                        if (fail.sub_extra) {
                            throw new ArgumentException();
                        }
                    }
                } else if (has_sub) {
                    if (has_extra) {
                        //sub+extra
                        if (fail.main_sub) {
                            Panel n = Panel.CalculateBestBracketable(nxt.sub.panel, nxt.main.panel);
                            Part src = nxt.main;
                            nxt.main = new Part(
                                Movement.Unknown,
                                n,
                                src.cur_second,
                                cur_second,
                                src.cur_moved_second == cur_second ?
                                    src.prv_moved_second : src.cur_moved_second
                            );
                            fail = new Fail(nxt);
                        }
                        if (fail.main_extra) {
                            Panel n = Panel.CalculateBestBracketable(nxt.extra.panel, nxt.main.panel);
                            Part src = nxt.main;
                            nxt.main = new Part(
                                Movement.Unknown,
                                n,
                                src.cur_second,
                                cur_second,
                                src.cur_moved_second == cur_second ?
                                    src.prv_moved_second : src.cur_moved_second
                            );
                            fail = new Fail(nxt);
                        }
                        if (fail.sub_extra) {
                            throw new ArgumentException();
                        }
                    } else {
                        //sub
                        if (fail.main_sub) {
                            Panel n = Panel.CalculateBestBracketable(nxt.sub.panel, nxt.main.panel);
                            Part src = nxt.main;
                            nxt.main = new Part(
                                Movement.Unknown,
                                n,
                                src.cur_second,
                                cur_second,
                                src.cur_moved_second == cur_second ?
                                    src.prv_moved_second : src.cur_moved_second
                            );
                            fail = new Fail(nxt);
                        }
                        if (fail.main_extra) {
                            throw new ArgumentException();
                        }
                        if (fail.sub_extra) {
                            nxt.extra = nxt.sub;
                            fail = new Fail(nxt);
                        }
                    }
                } else if (has_extra) {
                    //extra
                    if (fail.main_sub) {
                        throw new ArgumentException();
                    }
                    if (fail.main_extra) {
                        Panel n = Panel.CalculateBestBracketable(nxt.extra.panel, nxt.main.panel);
                        Part src = nxt.main;
                        nxt.main = new Part(
                            Movement.Unknown,
                            n,
                            src.cur_second,
                            cur_second,
                            src.cur_moved_second == cur_second ?
                                src.prv_moved_second : src.cur_moved_second
                        );
                        fail = new Fail(nxt);
                    }
                    if (fail.sub_extra) {
                        nxt.sub = nxt.extra;
                        fail = new Fail(nxt);
                    }
                } else {
                    //NOTHING
                    if (fail.main_sub) {
                        throw new ArgumentException();
                    }
                    if (fail.main_extra) {
                        throw new ArgumentException();
                    }
                    if (fail.sub_extra) {
                        throw new ArgumentException();
                    }
                }
                if (fail.main_sub || fail.main_extra || fail.sub_extra) {
                    throw new ArgumentException();
                }
                nxt.sanityCheck();
                return nxt;
            }
        }
        private static void AddIfValid (List<Limb> list, Limb item) {
            Fail fail = new Fail(item);
            if (fail.main_sub || fail.main_extra || fail.sub_extra) {
                return;
            }
            item.sanityCheck();
            list.Add(item);
        }
        public static List<Limb> TransitionToV2 (Limb from, Analyzer.Node.Limb to, float cur_second) {
            if (!IsArcValid(from, to)) {
                throw new ArgumentException();
            }
            if (to == null) {
                throw new ArgumentException();
            }

            List<Limb> result = new List<Limb>();

            Limb nxt = new Limb();
            for (int i = 0; i < Limb.PART_COUNT; ++i) {
                Part part = PartHelper.TransitionTo(from[i], to[i], cur_second);
                nxt[i] = part;
            }
            if (to[Analyzer.Node.Limb.INDEX_EXTRA] == null) {
                nxt.extra = nxt.sub;
            }
            bool has_main = to.main != null;
            bool has_sub = to.sub != null;
            bool has_extra = to.extra != null;
            Fail fail = new Fail(nxt);
            if (has_main) {
                if (has_sub) {
                    if (has_extra) {
                        //main+sub+extra
                        if (fail.main_sub) {
                            //throw new ArgumentException();
                        }
                        if (fail.main_extra) {
                            //throw new ArgumentException();
                        }
                        if (fail.sub_extra) {
                            //throw new ArgumentException();
                        }
                    } else {
                        //main+sub
                        if (fail.main_sub) {
                            //throw new ArgumentException();
                        }
                        if (fail.main_extra) {
                            nxt.extra = nxt.sub;
                            fail = new Fail(nxt);
                        }
                        if (fail.sub_extra) {
                            nxt.extra = nxt.sub;
                            fail = new Fail(nxt);
                        }
                    }
                } else if (has_extra) {
                    //main+extra
                    if (fail.main_sub) {
                        nxt.sub = nxt.extra;
                        fail = new Fail(nxt);
                    }
                    if (fail.main_extra) {
                        //throw new ArgumentException();
                    }
                    if (fail.sub_extra) {
                        nxt.sub = nxt.extra;
                        fail = new Fail(nxt);
                    }
                } else {
                    //main
                    if (fail.main_sub) {
                        List<Panel> neighbours = Panel.Neighbours_1D[nxt.main.panel.index];
                        foreach (Panel n in neighbours) {
                            Limb copy = Limb.DirectCopy(nxt);
                            Part src = copy.sub;
                            copy.sub = new Part(
                                Movement.Unknown,
                                n,
                                src.cur_second,
                                cur_second,
                                src.cur_moved_second == cur_second ?
                                    src.prv_moved_second : src.cur_moved_second
                            );

                            Fail copy_fail = new Fail(copy);
                            if (copy_fail.main_extra) {
                                copy.extra = copy.sub;
                            }
                            AddIfValid(result, copy);
                        }
                    }
                    if (fail.main_extra) {
                        nxt.extra = nxt.sub;
                        fail = new Fail(nxt);
                    }
                    if (fail.sub_extra) {
                        //throw new ArgumentException();
                    }
                }
            } else if (has_sub) {
                if (has_extra) {
                    //sub+extra
                    if (fail.main_sub) {
                        List<Panel> neighbours = Panel.Neighbours_1D[nxt.sub.panel.index];
                        foreach (Panel n in neighbours) {
                            Limb copy = Limb.DirectCopy(nxt);
                            Part src = copy.main;
                            copy.main = new Part(
                                Movement.Unknown,
                                n,
                                src.cur_second,
                                cur_second,
                                src.cur_moved_second == cur_second ?
                                    src.prv_moved_second : src.cur_moved_second
                            );

                            Fail copy_fail = new Fail(copy);
                            if (copy_fail.main_extra) {
                                List<Panel> neighbours_2 = Panel.Neighbours_1D[nxt.extra.panel.index];
                                foreach (Panel n_2 in neighbours_2) {
                                    Limb copy_2 = Limb.DirectCopy(copy);
                                    Part src_2 = copy_2.main;
                                    copy_2.main = new Part(
                                        Movement.Unknown,
                                        n_2,
                                        src_2.cur_second,
                                        cur_second,
                                        src_2.cur_moved_second == cur_second ?
                                            src_2.prv_moved_second : src_2.cur_moved_second
                                    );
                                    AddIfValid(result, copy_2);
                                }
                            }
                            AddIfValid(result, copy);
                        }
                    }
                    if (fail.main_extra) {
                        List<Panel> neighbours_2 = Panel.Neighbours_1D[nxt.extra.panel.index];
                        foreach (Panel n_2 in neighbours_2) {
                            Limb copy_2 = Limb.DirectCopy(nxt);
                            Part src_2 = copy_2.main;
                            copy_2.main = new Part(
                                Movement.Unknown,
                                n_2,
                                src_2.cur_second,
                                cur_second,
                                src_2.cur_moved_second == cur_second ?
                                    src_2.prv_moved_second : src_2.cur_moved_second
                            );
                            AddIfValid(result, copy_2);
                        }
                    }
                    if (fail.sub_extra) {
                        //throw new ArgumentException();
                    }
                } else {
                    //sub
                    if (fail.main_sub) {
                        List<Panel> neighbours = Panel.Neighbours_1D[nxt.sub.panel.index];
                        foreach (Panel n in neighbours) {
                            Limb copy = Limb.DirectCopy(nxt);
                            Part src = copy.main;
                            copy.main = new Part(
                                Movement.Unknown,
                                n,
                                src.cur_second,
                                cur_second,
                                src.cur_moved_second == cur_second ?
                                    src.prv_moved_second : src.cur_moved_second
                            );

                            Fail copy_fail = new Fail(copy);
                            if (copy_fail.main_extra) {
                                //do nothing
                            }
                            if (copy_fail.sub_extra) {
                                copy.extra = copy.sub;
                            }
                            AddIfValid(result, copy);
                        }
                    }
                    if (fail.main_extra) {
                        //throw new ArgumentException();
                    }
                    if (fail.sub_extra) {
                        nxt.extra = nxt.sub;
                    }
                }
            } else if (has_extra) {
                //extra
                if (fail.main_sub) {
                    //throw new ArgumentException();
                }
                if (fail.main_extra) {
                    List<Panel> neighbours = Panel.Neighbours_1D[nxt.extra.panel.index];
                    foreach (Panel n in neighbours) {
                        Limb copy = Limb.DirectCopy(nxt);
                        Part src = copy.main;
                        copy.main = new Part(
                            Movement.Unknown,
                            n,
                            src.cur_second,
                            cur_second,
                            src.cur_moved_second == cur_second ?
                                src.prv_moved_second : src.cur_moved_second
                        );

                        Fail copy_fail = new Fail(copy);
                        if (copy_fail.sub_extra) {
                            copy.sub = copy.extra;
                        }
                        AddIfValid(result, copy);
                    }
                }
                if (fail.sub_extra) {
                    nxt.sub = nxt.extra;
                }
            } else {
                //NOTHING
                if (fail.main_sub) {
                    //throw new ArgumentException();
                }
                if (fail.main_extra) {
                    //throw new ArgumentException();
                }
                if (fail.sub_extra) {
                    //throw new ArgumentException();
                }
            }
            AddIfValid(result, nxt);
            return result;
        }
        private static void FixInvalidBracket (Limb limb) {
            if (
                !limb.main.IsUnknown() &&
                !limb.sub.IsUnknown() &&
                (
                    limb.main.panel == limb.sub.panel ||
                    !Panel.IsBracketable(
                        limb.main.panel.index,
                        limb.sub.panel.index
                    )
                )
            ) {
                if (limb.main.IsPassiveDown()) {
                    limb.main = Part.ToUnknown(limb.main);
                } else {
                    limb.sub = Part.ToUnknown(limb.sub);
                }
            }
            if (
                !limb.main.IsUnknown() &&
                !limb.extra.IsUnknown() &&
                (
                    limb.main.panel == limb.extra.panel ||
                    !Panel.IsBracketable(
                        limb.main.panel.index,
                        limb.extra.panel.index
                    )
                )
            ) {
                if (limb.main.IsPassiveDown()) {
                    limb.main = Part.ToUnknown(limb.main);
                } else {
                    limb.extra = Part.ToUnknown(limb.extra);
                }
            }
            if (
                !limb.sub.IsUnknown() &&
                !limb.extra.IsUnknown() &&
                (
                    limb.sub.panel == limb.extra.panel ||
                    !Panel.IsSubBracketable(
                        limb.sub.panel.index,
                        limb.extra.panel.index
                    )
                )
            ) {
                if (limb.sub.IsPassiveDown()) {
                    limb.sub = Part.ToUnknown(limb.sub);
                } else {
                    limb.extra = Part.ToUnknown(limb.extra);
                }
            }
        }
        public static Limb TransitionToV3 (Limb from, Analyzer.Node.Limb to, float cur_second) {
            if (!IsArcValid(from, to)) {
                throw new ArgumentException();
            }

            if (to == null) {
                throw new ArgumentException();
            } else {
                Limb nxt = new Limb();
                for (int i = 0; i < Limb.PART_COUNT; ++i) {
                    Part from_part = from[i];
                    Analyzer.Node.Part to_part = to[i];
                    Part part;
                    if (to_part == null) {
                        if (from_part.IsUnknown()) {
                            part = Part.ToUnknown(from_part, cur_second);
                        } else {
                            part = Part.ToPassiveDown(from_part, cur_second);
                        }
                    } else {
                        if (PartHelper.IsArcValid(from_part, to_part)) {
                            part = PartHelper.TransitionTo(from_part, to_part, cur_second);
                        } else {
                            throw new ArgumentException();
                        }
                    }
                    nxt[i] = part;
                }
                FixInvalidBracket(nxt);
                nxt.sanityCheck();
                return nxt;
            }
        }
    }
}
