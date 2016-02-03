using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC.Analyzer.Solver.Heuristics {
    using Parser;
    using Node;
    using State;
    public class Solver : ISolver {
        private int m_ItemLimit;
        private int m_DesiredSectionSize;
        public Solver (int item_limit, int desired_section_size) {
            m_ItemLimit = item_limit;
            m_DesiredSectionSize = desired_section_size;
        }
        public List<State> Solve (List<Measure> measures, ICostFactory cost_factory, OnProgressDelegate on_progress) {
            List<NodeCollection> node_collections = NodeCollection.CalculateNodes(measures);
            List<List<NodeCollection>> sections = NodeCollection.CalculateSections(node_collections, m_DesiredSectionSize);
            State initial = SolverHelper.GenerateInitialNode(measures[0].beats[0].notes.Count == 10, cost_factory);

            OnProgressArg on_progress_arg = new OnProgressArg();
            on_progress_arg.second_max = node_collections[node_collections.Count - 1].items[0].second;
            on_progress_arg.depth_max = node_collections.Count - 1;
            foreach (List<NodeCollection> path in sections) {
                State nxt = null;
                float storage_ratio = (float)path.Count / 30.0f;
                if (storage_ratio < 1.0f) {
                    storage_ratio = 1.0f;
                }
                int storage_limit = (int)((float)m_ItemLimit * storage_ratio);
                while (nxt == null) {
                    if (storage_limit > 100000) { //Arbitrary limit
                        throw new Exception("Storage limit reached without a solution");
                    }
                    nxt = Calculate(initial, path, storage_limit, cost_factory, on_progress, on_progress_arg);
                    storage_limit += m_ItemLimit;

                    on_progress(on_progress_arg);
                    if (!on_progress_arg.carry_on) {
                        return SolverHelper.GeneratePlay(nxt == null ? initial : nxt);
                    }
                }
                on_progress_arg.prv_iter_opened_total = on_progress_arg.opened_total;
                on_progress_arg.prv_iter_discarded = on_progress_arg.discarded;

                initial = nxt;
            }
            return SolverHelper.GeneratePlay(initial);
        }
        public static State Calculate (
            State initial, List<NodeCollection> path,
            int storage_limit, ICostFactory cost_factory,
            OnProgressDelegate on_progress, OnProgressArg arg
        ) {
            int path_offset = initial.distance_from_start + 1;

            OpenedStateCollection opened = new OpenedStateCollection(storage_limit, 1000/*magic number*/, new StateComparer(0.2f/*magic number*/));
            opened.Add(initial);
            State prv_opened = null;
            State result = null;

            List<State> tmp = new List<State>();
            while (opened.Commit()) {
                State cur = opened.First();
                int nxt_distance_from_start = cur.distance_from_start + 1;
                int path_index = nxt_distance_from_start - path_offset;
                if (path_index == path.Count) {
                    if (result == null || result.cost.GetTotalCost() > cur.cost.GetTotalCost()) {
                        result = cur;
                    }
                    continue;
                }
                prv_opened = cur;
                Beat beat = path[path_index].beat;
                List<Node> neighbours = path[path_index].items;

                if (on_progress != null) {
                    arg.progress_cur = (int)((float)nxt_distance_from_start / (float)arg.depth_max * 100.0f);
                    arg.depth_cur = nxt_distance_from_start;
                    arg.second_cur = cur.second;

                    ++arg.closed;

                    arg.opened_cur = opened.Count();
                    arg.opened_total = arg.prv_iter_opened_total + opened.GetTotalAdded();
                    arg.discarded = arg.prv_iter_discarded + opened.GetTotalDiscarded();

                    on_progress(arg);
                    if (!arg.carry_on) {
                        return result;
                    }
                }
                if (neighbours.Count == 0) {
                    throw new ExecutionEngineException();
                } else {
                    //tmp.Clear();
                    foreach (Node n in neighbours) {
                        if (!ArcCalculator.IsArcValid(cur, n)) {
                            continue;
                        }
                        List<Analyzer.State.Limb[]> possibilities = new List<Analyzer.State.Limb[]>();
                        ArcCalculator.Calculate(cur, n, possibilities);
                        foreach (Analyzer.State.Limb[] p in possibilities) {
                            State nxt = ArcCalculator.TransitionTo(cur, p, n.second, n.distance_from_start, beat, cost_factory);
                            if (nxt != null) {
                                //tmp.Add(nxt);
                                opened.Add(nxt);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
