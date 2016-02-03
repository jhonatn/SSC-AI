using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace AHS.SSC {
    using Parser;
    using Analyzer.State;
    using Analyzer.Node;
    public partial class MainForm {
        public void InitOnSolvedTreeUpdater () {
            OnSolved += MainForm_OnSolved;
            tree.DrawNode += State_DrawNode;
        }

        void State_DrawNode (object sender, DrawTreeNodeEventArgs e) {
            if (!(e.Node.Tag is TreeNode_StateTag)) {
                return;
            }
            e.DrawDefault = false;
            TreeNode_StateTag state_tag = (TreeNode_StateTag)e.Node.Tag;
            State state = state_tag.state;
            string[] texts = e.Node.Text.Split();
            using (Brush black = new SolidBrush(Color.Black)) {
                using (Brush blue = new SolidBrush(Color.Blue)) {
                    using (Brush red = new SolidBrush(Color.Red)) {
                        using (Brush yellow = new SolidBrush(Color.DarkOrange)) {
                            Brush[] brushes = new Brush[] { 
                                    blue, red, yellow, red, blue,
                                    blue, red, yellow, red, blue
                                };
                            SizeF offset = new SizeF();

                            string panels = texts[0];
                            for (int i = 0; i < panels.Length; ++i) {
                                String p = "" + panels[i];
                                e.Graphics.DrawString(p, tree.Font, brushes[i], e.Bounds.Left + (int)offset.Width, e.Bounds.Top);
                                offset += e.Graphics.MeasureString(p, tree.Font);
                            }
                            for (int i = 1; i < texts.Length; ++i) {
                                string txt = texts[i];
                                e.Graphics.DrawString(txt, tree.Font, black, e.Bounds.Left + (int)offset.Width, e.Bounds.Top);
                                offset += e.Graphics.MeasureString(txt, tree.Font);
                            }
                        }
                    }
                }
            }
        }

        void MainForm_OnSolved (MainForm.CalculatePlayArgs args) {
            TreeNode play_node = new TreeNode("Play");
            args.node.Nodes.Add(play_node);
            List<NodeCollection> node_collections = NodeCollection.CalculateNodes(args.measures);
            for (int i = 0; i < args.result.Count; ++i) {
                State state = args.result[i];
                play_node.Nodes.Add(CreateStateNode(state, node_collections));
            }
            this.Text = "Analyzed! " + this.Text;
        }
        public class TreeNode_StateTag {
            public State state;
            public List<NodeCollection> node_collections;
        }
        private static TreeNode CreateStateNode (State p, List<NodeCollection> node_collections) {
            NodeCollection collection = p.distance_from_start >= 0 ?
                node_collections[p.distance_from_start] :
                null;
            Beat b = collection == null ?
                new Beat() :
                collection.beat;

            String[] limb_names = new String[] { 
                "-", "L", "R", "0", "1"
            };
            String s = "";
            for (int i = 0; i < b.notes.Count; ++i) {
                if (b.notes[i].tap == TapType.None) {
                    s += limb_names[0];
                } else {
                    int limb_index = p.getOccupyingLimbIndex(Panel.Panels_1D_Playable[i]);
                    if (limb_index < 0) {
                        s += limb_names[0];
                        continue;
                    }
                    Analyzer.State.Limb limb = p.limbs[limb_index];
                    int part_index = limb.getOccupyingPartIndex(Panel.Panels_1D_Playable[i]);
                    Analyzer.State.Part part = limb[part_index];
                    if (
                        part.movement == Analyzer.State.Movement.Tap ||
                        part.movement == Analyzer.State.Movement.ForceDownStart ||
                        part.movement == Analyzer.State.Movement.ForceDown
                    ) {
                        s += limb_names[limb_index + 1];
                    } else {
                        s += limb_names[0];
                    }
                }
            }
            TreeNode state_node = new TreeNode(string.Format("{0} - {1}s", s, p.second));
            TreeNode_StateTag tag = new TreeNode_StateTag();
            tag.state = p;
            tag.node_collections = node_collections;
            state_node.Tag = tag;
            state_node.Nodes.Add(new TreeNode(string.Format("Facing Degree: {0}", p.facing.toDegree())));
            state_node.Nodes.Add(new TreeNode(string.Format("Facing Desired Degree: {0}", p.facing_desired.toDegree())));
            state_node.Nodes.Add(new TreeNode(string.Format("Total   : {0}", p.cost.GetTotalCost())));
            state_node.Nodes.Add(new TreeNode(string.Format("Facing  : {0}", p.cost.GetCost(Cost.INDEX_FACING))));
            state_node.Nodes.Add(new TreeNode(string.Format("Move    : {0}", p.cost.GetCost(Cost.INDEX_MOVE))));
            state_node.Nodes.Add(new TreeNode(string.Format("Rest    : {0}", p.cost.GetCost(Cost.INDEX_REST))));
            state_node.Nodes.Add(new TreeNode(string.Format("Dbl Step: {0}", p.cost.GetCost(Cost.INDEX_DOUBLE_STEP))));
            state_node.Nodes.Add(new TreeNode(string.Format("Crossed : {0}", p.cost.GetCost(Cost.INDEX_CROSSED))));
            state_node.Nodes.Add(new TreeNode(string.Format("B. Angle: {0}", p.cost.GetCost(Cost.INDEX_BRACKET_ANGLE))));
            state_node.Nodes.Add(new TreeNode(string.Format("Xtra Use: {0}", p.cost.GetCost(Cost.INDEX_EXTRA_USE))));
            state_node.Nodes.Add(new TreeNode(string.Format("U. Limb : {0}", p.cost.GetCost(Cost.INDEX_UNKNOWN_LIMB))));
            state_node.Nodes.Add(new TreeNode(string.Format("Too Fast: {0}", p.cost.GetCost(Cost.INDEX_TOO_FAST))));
            {
                TreeNode limb_node = new TreeNode("Limbs");
                limb_node.Tag = p.limbs;
                state_node.Nodes.Add(limb_node);
                for (int i = 0; i < p.limbs.Length; ++i) {
                    Analyzer.State.Limb limb = p.limbs[i];
                    TreeNode l_node = new TreeNode(string.Format("Limb {0}", i));
                    limb_node.Nodes.Add(l_node);
                    for (int part_index = 0; part_index < Analyzer.State.Limb.PART_COUNT; ++part_index) {
                        Analyzer.State.Part part = limb[part_index];
                        int panel_index = part.panel == null ? -1 : part.panel.index;
                        int panel_index_playable = part.panel == null ? -1 : part.panel.index_playable;
                        l_node.Nodes.Add(string.Format(
                            "{0:00} - {1:00} - {2}",
                            panel_index,
                            panel_index_playable,
                            part.movement.ToString())
                        );

                    }
                }
            }
            return state_node;
        }
    }
}
