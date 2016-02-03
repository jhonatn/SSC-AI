using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHS.SSC {
    using Parser;
    public partial class MainForm {
        private void InitOnSSCParsedTreeUpdater () {
            OnSSCParsed += MainForm_OnSSCParsed;
        }

        void MainForm_OnSSCParsed (Parser.SSC ssc) {

            tree.Nodes.Clear();
            {
                TreeNode raw_node = new TreeNode("Raw Data");
                tree.Nodes.Add(raw_node);
                foreach (var kvp in ssc.raw_data) {
                    TreeNode key_node = new TreeNode(kvp.Key);
                    TreeNode val_node = new TreeNode(kvp.Value);
                    raw_node.Nodes.Add(key_node);
                    key_node.Nodes.Add(val_node);
                }
            }
            {
                TreeNode chart_node = new TreeNode("Charts");
                tree.Nodes.Add(chart_node);
                foreach (Chart c in ssc.charts) {
                    TreeNode c_node = new TreeNode(string.Format("{0} {1}", c.raw_data[Chart.KEY_STEPSTYPE], c.raw_data[Chart.KEY_METER]));
                    c_node.Tag = c;
                    chart_node.Nodes.Add(c_node);
                    c_node.Nodes.Add(new TreeNode(string.Format("Total Second = {0}", c.total_second)));
                    {
                        foreach (var kvp in c.raw_data) {
                            TreeNode key_node = new TreeNode(kvp.Key);
                            TreeNode val_node = new TreeNode(kvp.Value);
                            c_node.Nodes.Add(key_node);
                            key_node.Nodes.Add(val_node);
                        }
                    }
                    {
                        TreeNode measure_node = new TreeNode("Measures");
                        c_node.Nodes.Add(measure_node);
                        foreach (Measure m in c.measures) {
                            TreeNode m_node = new TreeNode(string.Format("Measure {0}: Size {1}", measure_node.Nodes.Count, m.beats.Count));
                            measure_node.Nodes.Add(m_node);
                            foreach (Beat b in m.beats) {
                                TreeNode b_node = new TreeNode(string.Format("Beat {0}", m_node.Nodes.Count));
                                m_node.Nodes.Add(b_node);
                                b_node.Nodes.Add(new TreeNode(b.beat.ToString()));
                                b_node.Nodes.Add(new TreeNode(b.second.ToString()));

                                foreach (Note n in b.notes) {
                                    TreeNode n_node = new TreeNode(n.name);
                                    b_node.Nodes.Add(n_node);
                                }
                            }
                        }
                    }
                    {
                        TreeNode bpm_node = new TreeNode("BPMS");
                        c_node.Nodes.Add(bpm_node);
                        foreach (BPMS.Data data in c.bpms.data) {
                            TreeNode d_node = new TreeNode(string.Format("{0}={1}BPM", data.beat, data.bpm));
                            bpm_node.Nodes.Add(d_node);
                        }
                    }
                    {
                        TreeNode delay_node = new TreeNode("DELAYS");
                        c_node.Nodes.Add(delay_node);
                        foreach (DELAYS.Data data in c.delays.data) {
                            TreeNode d_node = new TreeNode(string.Format("{0}={1}", data.beat, data.duration_second));
                            delay_node.Nodes.Add(d_node);
                        }
                    }
                }
            }
        }
    }
}
