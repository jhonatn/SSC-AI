using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AHS.SSC {
    using Parser;
    public partial class MainForm {
        public delegate void OnSSCParsedDelegate (SSC ssc);
        public event OnSSCParsedDelegate OnSSCParsed = null;

        private void InitDragDrop () {
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;
        }

        void MainForm_DragEnter (object sender, System.Windows.Forms.DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) {
                e.Effect = DragDropEffects.None;
                return;
            }
            string f = files[0];
            if (Path.GetExtension(f) == SSC.EXTENSION) {
                e.Effect = DragDropEffects.Link;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        void MainForm_DragDrop (object sender, System.Windows.Forms.DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) { return; }
            string f = files[0];
            if (Path.GetExtension(f) != SSC.EXTENSION) { return; }
            //Parse the file
            string raw = "";
            try {
                using (StreamReader input = new StreamReader(f)) {
                    raw = input.ReadToEnd();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return;
            }
            SSC ssc = SSC.Parse(raw);
            if (OnSSCParsed != null) {
                OnSSCParsed(ssc);
            }
        }
    }
}
