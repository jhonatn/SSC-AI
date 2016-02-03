using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHS.SSC {
    using Parser;
    public partial class MainForm : Form {
        public MainForm () {
            InitializeComponent();
            tree.DrawNode += tree_DrawNode;
            InitDragDrop();
            InitOnSSCParsedTreeUpdater();
            InitCostFactory();
            InitSolver();
            InitOnSolvedTreeUpdater();
            InitFindArcs();
            InitIsLimbValid();
            InitCalculateCost();
        }

        void tree_DrawNode (object sender, DrawTreeNodeEventArgs e) {
            e.DrawDefault = true;
        }
    }
}
