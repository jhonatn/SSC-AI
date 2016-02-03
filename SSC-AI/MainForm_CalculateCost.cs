using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC {
    using Parser;
    using Analyzer.State;
    public partial class MainForm {
        private void InitCalculateCost () {
            btn_calculate_cost.Click += btn_calculate_cost_Click;
        }

        void btn_calculate_cost_Click (object sender, EventArgs e) {
            if (tree.SelectedNode == null) { return; }
            TreeNode_StateTag tag = (tree.SelectedNode.Tag) as TreeNode_StateTag;
            if (tag == null) { return; }
            State cur = tag.state;
            Vector facing = FacingCalculator.Calculate(cur.limbs, cur.parent.facing);
            Vector facing_desired = FacingCalculator.CalculateDesiredFacing(facing, cur.parent.facing_desired);
            Cost cost = (Cost)m_CostFactory.Calculate(cur);

        }
    }
}
