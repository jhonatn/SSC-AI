using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Solver.DepthFirstBound {
    using Parser;
    using Node;
    using State;
    public class Solver : ISolver {
        private int m_DesiredSectionSize;
        public Solver (int desired_section_size) {
            m_DesiredSectionSize = desired_section_size;
        }
        private void Calculate (List<NodeCollection> node_collections, State cur, ICostFactory cost_factory, OnProgressDelegate on_progress, OnProgressArg on_progress_arg, ref State best) {
            on_progress_arg.progress_cur = (int)(
                (
                    (float)(cur.distance_from_start + 1) /
                    (float)(on_progress_arg.depth_max)
                ) * 100.0f
            );
            on_progress(on_progress_arg);

            if (cur.distance_from_start == node_collections.Count - 1) {
                //This is a completed play
                if (best == null) {
                    best = cur;
                } else if (cur.cost.GetTotalCost() < best.cost.GetTotalCost()) {
                    best = cur;
                }
                return;
            }
            if (!on_progress_arg.carry_on) {
                return;
            }
            int index = cur.distance_from_start + 1;
            NodeCollection collection = node_collections[index];
            Beat beat = collection.beat;

            List<State> nxt_list = new List<State>();
            foreach (Node node in collection.items) {
                List<Analyzer.State.Limb[]> neighbours = new List<Analyzer.State.Limb[]>();
                ArcCalculator.Calculate(cur, node, neighbours);
                foreach (Analyzer.State.Limb[] n in neighbours) {
                    State nxt = ArcCalculator.TransitionTo(cur, n, collection.beat.second, index, beat, cost_factory);
                    if (nxt == null) {
                        continue;
                    }
                    if (best == null || nxt.cost.GetTotalCost() < best.cost.GetTotalCost()) {
                        nxt_list.Add(nxt);
                    }
                }
            }
            nxt_list.Sort((State a, State b) => {
                return a.cost.GetTotalCost().CompareTo(b.cost.GetTotalCost());
            });
            foreach (State n in nxt_list) {
                Calculate(node_collections, n, cost_factory, on_progress, on_progress_arg, ref best);
            }
        }
        public List<State> Solve (List<Measure> measures, ICostFactory cost_factory, OnProgressDelegate on_progress) {
            List<NodeCollection> node_collections = NodeCollection.CalculateNodes(measures);
            List<List<NodeCollection>> sections = NodeCollection.CalculateSections(node_collections, m_DesiredSectionSize);
            State initial = SolverHelper.GenerateInitialNode(measures[0].beats[0].notes.Count == 10, cost_factory);

            OnProgressArg on_progress_arg = new OnProgressArg();
            on_progress_arg.second_max = node_collections[node_collections.Count - 1].items[0].second;
            on_progress_arg.depth_max = node_collections.Count - 1;
            on_progress_arg.depth_cur = 0;
            foreach (List<NodeCollection> path in sections) {
                State nxt = null;
                while (nxt == null) {
                    Calculate(path, initial, cost_factory, on_progress, on_progress_arg, ref nxt);

                    on_progress(on_progress_arg);
                    if (!on_progress_arg.carry_on) {
                        return SolverHelper.GeneratePlay(nxt == null ? initial : nxt);
                    }
                }
                on_progress_arg.prv_iter_opened_total = on_progress_arg.opened_total;
                on_progress_arg.prv_iter_discarded = on_progress_arg.discarded;

                initial = nxt;
                on_progress_arg.depth_cur += path.Count;
            }
            return SolverHelper.GeneratePlay(initial);
        }
    }
}
