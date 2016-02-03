using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Node {
    using Parser;
    public static class LimbHelper {
        public static Panel GetFrontPanel (Panel a, Panel b, Panel c) {
            if (a.direction_y == PanelDirectionY.Front) { return a; }
            if (b.direction_y == PanelDirectionY.Front) { return b; }
            if (c.direction_y == PanelDirectionY.Front) { return c; }
            throw new ArgumentException();
        }
        public static Panel GetBackPanel (Panel a, Panel b, Panel c) {
            if (a.direction_y == PanelDirectionY.Back) { return a; }
            if (b.direction_y == PanelDirectionY.Back) { return b; }
            if (c.direction_y == PanelDirectionY.Back) { return c; }
            throw new ArgumentException();
        }
        public static Panel GetCenterPanel (Panel a, Panel b, Panel c) {
            if (a.direction_y == PanelDirectionY.Center) { return a; }
            if (b.direction_y == PanelDirectionY.Center) { return b; }
            if (c.direction_y == PanelDirectionY.Center) { return c; }
            throw new ArgumentException();
        }
        public static void Do3Bracket (Node state, int limb_index, bool face_front, Beat beat, Panel a, Panel b, Panel c) {
            Limb limb = state.limbs[limb_index];

            Panel front = GetFrontPanel(a, b, c);
            Panel back = GetBackPanel(a, b, c);
            Panel center = GetCenterPanel(a, b, c);

            if (limb_index == Node.INDEX_LEFT_FOOT) {
                if (face_front) {
                    limb.setMain(beat, back);
                    if (back.direction_x == PanelDirectionX.Left) {
                        limb.setSub(beat, center);
                        limb.setExtra(beat, front);
                    } else {
                        limb.setSub(beat, front);
                        limb.setExtra(beat, center);
                    }
                } else {
                    limb.setMain(beat, front);
                    if (front.direction_x == PanelDirectionX.Right) {
                        limb.setSub(beat, center);
                        limb.setExtra(beat, back);
                    } else {
                        limb.setSub(beat, back);
                        limb.setExtra(beat, center);
                    }
                }
            } else if (limb_index == Node.INDEX_RIGHT_FOOT) {
                if (face_front) {
                    limb.setMain(beat, back);
                    if (back.direction_x == PanelDirectionX.Right) {
                        limb.setSub(beat, center);
                        limb.setExtra(beat, front);
                    } else {
                        limb.setSub(beat, front);
                        limb.setExtra(beat, center);
                    }
                } else {
                    limb.setMain(beat, front);
                    if (front.direction_x == PanelDirectionX.Left) {
                        limb.setSub(beat, center);
                        limb.setExtra(beat, back);
                    } else {
                        limb.setSub(beat, back);
                        limb.setExtra(beat, center);
                    }
                }
            } else {
                throw new ArgumentException();
            }
        }
    }
}
