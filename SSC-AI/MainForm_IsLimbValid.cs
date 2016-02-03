using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHS.SSC {
    using Analyzer.State;
    public partial class MainForm {
        private void InitIsLimbValid () {
            btn_is_limb_valid.Click += btn_is_limb_valid_Click;
        }

        void btn_is_limb_valid_Click (object sender, EventArgs e) {
            if (tree.SelectedNode == null) { return; }
            Limb[] tag = (tree.SelectedNode.Tag) as Limb[];
            if (tag == null) { return; }

            /*bool valid_without_crossing = ArcCalculator.IsArcValidWithoutCrossing(tag);
            bool valid_with_crossing = ArcCalculator.IsArcValidWithCrossing(tag);*/

            tag = tag;
        }
    }
}
