using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHS.SSC {
    using Analyzer.Node;
    using Analyzer.State;
    using Analyzer.Solver;
    public partial class MainForm {
        private void InitFindArcs () {
            btn_find_arcs.Click += btn_find_arcs_Click;
        }

        void btn_find_arcs_Click (object sender, EventArgs e) {
            if (tree.SelectedNode == null) { return; }
            TreeNode_StateTag tag = (tree.SelectedNode.Tag) as TreeNode_StateTag;
            if (tag == null) { return; }
            State cur = tag.state;
            int index = cur.distance_from_start + 1;
            if (index >= tag.node_collections.Count) {
                MessageBox.Show("No more arcs");
                return;
            }
            NodeCollection collection = tag.node_collections[index];
            List<State> nxt_list = new List<State>();
            foreach (Node node in collection.items) {
                List<Analyzer.State.Limb[]> neighbours = new List<Analyzer.State.Limb[]>();
                ArcCalculator.Calculate(cur, node, neighbours);
                foreach (Analyzer.State.Limb[] n in neighbours) {
                    State nxt = ArcCalculator.TransitionTo(cur, n, collection.beat.second, index, collection.beat, m_CostFactory);
                    if (nxt == null) {
                        continue;
                    }
                    nxt_list.Add(nxt);
                }
            }
            {
                TreeNode root = tree.SelectedNode.Nodes.Add("Arcs - By Index");
                foreach (State n in nxt_list) {
                    root.Nodes.Add(CreateStateNode(n, tag.node_collections));
                }
            }
            nxt_list.Sort((State a, State b) => {
                return a.cost.GetTotalCost().CompareTo(b.cost.GetTotalCost());
            });
            {
                TreeNode root = tree.SelectedNode.Nodes.Add("Arcs - By Cost");
                foreach (State n in nxt_list) {
                    root.Nodes.Add(CreateStateNode(n, tag.node_collections));
                }
            }
        }
    }
}
