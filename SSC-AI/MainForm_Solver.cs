using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHS.SSC {
    using Parser;
    using Analyzer.State;
    using Analyzer.Solver;
    public partial class MainForm {
        public delegate void OnSolvedDelegate (CalculatePlayArgs args);
        public event OnSolvedDelegate OnSolved = null;
        private void InitSolver () {
            btn_dfs_solve.Click += btn_dfs_solve_Click;
            btn_h_solve.Click += btn_h_solve_Click;
            btn_cancel.Click += btn_cancel_Click;
            bg_worker.DoWork += bg_worker_DoWork;
            bg_worker.ProgressChanged += bg_worker_ProgressChanged;
            bg_worker.RunWorkerCompleted += bg_worker_RunWorkerCompleted;
        }

        void bg_worker_RunWorkerCompleted (object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            CalculatePlayArgs args = (CalculatePlayArgs)e.Result;
            if (OnSolved != null) {
                OnSolved(args);
            }
        }

        void bg_worker_ProgressChanged (object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            progress.Value = e.ProgressPercentage;
            CalculatePlayArgs args = (CalculatePlayArgs)e.UserState;
            OnProgressArg arg = args.on_progress_arg;
            this.Text = string.Format(
                "{0}% - {1}/{2} - {3}/{4}/{5}",
                e.ProgressPercentage,
                arg.second_cur,
                arg.second_max,
                arg.opened_total,
                arg.closed,
                arg.discarded
            );
        }

        void bg_worker_DoWork (object sender, System.ComponentModel.DoWorkEventArgs e) {
            CalculatePlayArgs args = (CalculatePlayArgs)e.Argument;
            List<Measure> measures = new List<Measure>(args.chart.measures);
            if (args.end_measure > 0) {
                measures.RemoveRange(args.end_measure, measures.Count-args.end_measure);
            }
            if (args.start_measure > 0) {
                measures.RemoveRange(0, args.start_measure);
            }
            args.measures = measures;
            List<State> result = args.solver.Solve(measures, m_CostFactory, (OnProgressArg arg) => {
                bg_worker.ReportProgress(arg.progress_cur, args);
                arg.carry_on = !bg_worker.CancellationPending;
                args.on_progress_arg = arg;
            });
            args.result = result;
            e.Result = args;
        }

        void btn_cancel_Click (object sender, EventArgs e) {
            bg_worker.CancelAsync();
        }
        public class CalculatePlayArgs {
            public Chart chart;
            public TreeNode node;
            public List<State> result;
            public ISolver solver;
            public OnProgressArg on_progress_arg;

            public int start_measure;
            public int end_measure;
            public List<Measure> measures;
        }
        private void solve (ISolver solver) {
            if (tree.SelectedNode == null) { return; }
            object tag = tree.SelectedNode.Tag;
            Chart chart = tag as Chart;
            if (chart == null) { return; }
            CalculatePlayArgs args = new CalculatePlayArgs();
            args.solver = solver;
            args.chart = chart;
            args.node = tree.SelectedNode;
            args.start_measure = (int)num_start_measure.Value;
            args.end_measure = (int)num_end_measure.Value;
            bg_worker.RunWorkerAsync(args);
        }
        void btn_dfs_solve_Click (object sender, EventArgs e) {
            solve(new Analyzer.Solver.DepthFirstBound.Solver(25));
        }
        void btn_h_solve_Click (object sender, EventArgs e) {
            solve(new Analyzer.Solver.Heuristics.Solver(2000, 25));
        }
    }
}
