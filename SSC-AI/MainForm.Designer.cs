namespace AHS.SSC {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.tree = new System.Windows.Forms.TreeView();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.bg_worker = new System.ComponentModel.BackgroundWorker();
            this.btn_dfs_solve = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_find_arcs = new System.Windows.Forms.Button();
            this.btn_is_limb_valid = new System.Windows.Forms.Button();
            this.btn_h_solve = new System.Windows.Forms.Button();
            this.btn_calculate_cost = new System.Windows.Forms.Button();
            this.num_bracket_degree_cost = new System.Windows.Forms.NumericUpDown();
            this.num_bracket_degree_exp_const = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.num_start_measure = new System.Windows.Forms.NumericUpDown();
            this.num_end_measure = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_bracket_degree_cost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_bracket_degree_exp_const)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_start_measure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_end_measure)).BeginInit();
            this.SuspendLayout();
            // 
            // tree
            // 
            this.tree.Dock = System.Windows.Forms.DockStyle.Left;
            this.tree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.tree.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.tree.Location = new System.Drawing.Point(0, 0);
            this.tree.Name = "tree";
            this.tree.Size = new System.Drawing.Size(320, 540);
            this.tree.TabIndex = 0;
            // 
            // progress
            // 
            this.progress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progress.Location = new System.Drawing.Point(320, 517);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(447, 23);
            this.progress.TabIndex = 1;
            // 
            // bg_worker
            // 
            this.bg_worker.WorkerReportsProgress = true;
            this.bg_worker.WorkerSupportsCancellation = true;
            // 
            // btn_dfs_solve
            // 
            this.btn_dfs_solve.Location = new System.Drawing.Point(326, 12);
            this.btn_dfs_solve.Name = "btn_dfs_solve";
            this.btn_dfs_solve.Size = new System.Drawing.Size(75, 23);
            this.btn_dfs_solve.TabIndex = 2;
            this.btn_dfs_solve.Text = "DFS Solve";
            this.btn_dfs_solve.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(488, 12);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 3;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // btn_find_arcs
            // 
            this.btn_find_arcs.Location = new System.Drawing.Point(326, 41);
            this.btn_find_arcs.Name = "btn_find_arcs";
            this.btn_find_arcs.Size = new System.Drawing.Size(75, 23);
            this.btn_find_arcs.TabIndex = 4;
            this.btn_find_arcs.Text = "Find Arcs";
            this.btn_find_arcs.UseVisualStyleBackColor = true;
            // 
            // btn_is_limb_valid
            // 
            this.btn_is_limb_valid.Location = new System.Drawing.Point(407, 41);
            this.btn_is_limb_valid.Name = "btn_is_limb_valid";
            this.btn_is_limb_valid.Size = new System.Drawing.Size(75, 23);
            this.btn_is_limb_valid.TabIndex = 5;
            this.btn_is_limb_valid.Text = "Is Limb Valid";
            this.btn_is_limb_valid.UseVisualStyleBackColor = true;
            // 
            // btn_h_solve
            // 
            this.btn_h_solve.Location = new System.Drawing.Point(407, 12);
            this.btn_h_solve.Name = "btn_h_solve";
            this.btn_h_solve.Size = new System.Drawing.Size(75, 23);
            this.btn_h_solve.TabIndex = 6;
            this.btn_h_solve.Text = "H Solve";
            this.btn_h_solve.UseVisualStyleBackColor = true;
            // 
            // btn_calculate_cost
            // 
            this.btn_calculate_cost.Location = new System.Drawing.Point(326, 70);
            this.btn_calculate_cost.Name = "btn_calculate_cost";
            this.btn_calculate_cost.Size = new System.Drawing.Size(156, 23);
            this.btn_calculate_cost.TabIndex = 7;
            this.btn_calculate_cost.Text = "Calculate Cost";
            this.btn_calculate_cost.UseVisualStyleBackColor = true;
            // 
            // num_bracket_degree_cost
            // 
            this.num_bracket_degree_cost.Location = new System.Drawing.Point(488, 98);
            this.num_bracket_degree_cost.Name = "num_bracket_degree_cost";
            this.num_bracket_degree_cost.Size = new System.Drawing.Size(120, 20);
            this.num_bracket_degree_cost.TabIndex = 8;
            // 
            // num_bracket_degree_exp_const
            // 
            this.num_bracket_degree_exp_const.Location = new System.Drawing.Point(488, 124);
            this.num_bracket_degree_exp_const.Name = "num_bracket_degree_exp_const";
            this.num_bracket_degree_exp_const.Size = new System.Drawing.Size(120, 20);
            this.num_bracket_degree_exp_const.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(323, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Bracket Degree Cost";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(323, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Bracket Degree Exp Const";
            // 
            // num_start_measure
            // 
            this.num_start_measure.Location = new System.Drawing.Point(488, 150);
            this.num_start_measure.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.num_start_measure.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.num_start_measure.Name = "num_start_measure";
            this.num_start_measure.Size = new System.Drawing.Size(120, 20);
            this.num_start_measure.TabIndex = 12;
            this.num_start_measure.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // num_end_measure
            // 
            this.num_end_measure.Location = new System.Drawing.Point(488, 176);
            this.num_end_measure.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.num_end_measure.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.num_end_measure.Name = "num_end_measure";
            this.num_end_measure.Size = new System.Drawing.Size(120, 20);
            this.num_end_measure.TabIndex = 13;
            this.num_end_measure.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(323, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Start Measure";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(323, 183);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "End Measure";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 540);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.num_end_measure);
            this.Controls.Add(this.num_start_measure);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.num_bracket_degree_exp_const);
            this.Controls.Add(this.num_bracket_degree_cost);
            this.Controls.Add(this.btn_calculate_cost);
            this.Controls.Add(this.btn_h_solve);
            this.Controls.Add(this.btn_is_limb_valid);
            this.Controls.Add(this.btn_find_arcs);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_dfs_solve);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.tree);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.num_bracket_degree_cost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_bracket_degree_exp_const)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_start_measure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_end_measure)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tree;
        private System.Windows.Forms.ProgressBar progress;
        private System.ComponentModel.BackgroundWorker bg_worker;
        private System.Windows.Forms.Button btn_dfs_solve;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_find_arcs;
        private System.Windows.Forms.Button btn_is_limb_valid;
        private System.Windows.Forms.Button btn_h_solve;
        private System.Windows.Forms.Button btn_calculate_cost;
        private System.Windows.Forms.NumericUpDown num_bracket_degree_cost;
        private System.Windows.Forms.NumericUpDown num_bracket_degree_exp_const;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_start_measure;
        private System.Windows.Forms.NumericUpDown num_end_measure;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

