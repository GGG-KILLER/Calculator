namespace Calculator.UI.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose ( );
            }
            base.Dispose ( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ( )
        {
			this.btnEquals = new System.Windows.Forms.Button();
			this.txtExpression = new System.Windows.Forms.TextBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.txtTimeLexing = new System.Windows.Forms.ToolStripStatusLabel();
			this.txtTimeParsing = new System.Windows.Forms.ToolStripStatusLabel();
			this.txtTimeExecuting = new System.Windows.Forms.ToolStripStatusLabel();
			this.txtTimeTotal = new System.Windows.Forms.ToolStripStatusLabel();
			this.txtResult = new System.Windows.Forms.TextBox();
			this.txtBench = new System.Windows.Forms.Button();
			this.txtAST = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnEquals
			// 
			this.btnEquals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnEquals.Location = new System.Drawing.Point(570, 37);
			this.btnEquals.Name = "btnEquals";
			this.btnEquals.Size = new System.Drawing.Size(43, 23);
			this.btnEquals.TabIndex = 1;
			this.btnEquals.Text = "=";
			this.btnEquals.UseVisualStyleBackColor = true;
			this.btnEquals.Click += new System.EventHandler(this.BtnEquals_Click);
			// 
			// txtExpression
			// 
			this.txtExpression.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtExpression.Location = new System.Drawing.Point(12, 13);
			this.txtExpression.Name = "txtExpression";
			this.txtExpression.Size = new System.Drawing.Size(601, 20);
			this.txtExpression.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(13, 97);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.listBox1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.listBox2);
			this.splitContainer1.Size = new System.Drawing.Size(600, 146);
			this.splitContainer1.SplitterDistance = 112;
			this.splitContainer1.TabIndex = 2;
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(112, 146);
			this.listBox1.TabIndex = 0;
			// 
			// listBox2
			// 
			this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox2.FormattingEnabled = true;
			this.listBox2.Location = new System.Drawing.Point(0, 0);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(484, 146);
			this.listBox2.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.txtTimeLexing,
            this.txtTimeParsing,
            this.txtTimeExecuting,
            this.txtTimeTotal});
			this.statusStrip1.Location = new System.Drawing.Point(0, 246);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(625, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(76, 17);
			this.toolStripStatusLabel1.Text = "Times Taken:";
			// 
			// txtTimeLexing
			// 
			this.txtTimeLexing.Name = "txtTimeLexing";
			this.txtTimeLexing.Size = new System.Drawing.Size(41, 17);
			this.txtTimeLexing.Text = "Lexing";
			// 
			// txtTimeParsing
			// 
			this.txtTimeParsing.Name = "txtTimeParsing";
			this.txtTimeParsing.Size = new System.Drawing.Size(46, 17);
			this.txtTimeParsing.Text = "Parsing";
			// 
			// txtTimeExecuting
			// 
			this.txtTimeExecuting.Name = "txtTimeExecuting";
			this.txtTimeExecuting.Size = new System.Drawing.Size(58, 17);
			this.txtTimeExecuting.Text = "Executing";
			// 
			// txtTimeTotal
			// 
			this.txtTimeTotal.Name = "txtTimeTotal";
			this.txtTimeTotal.Size = new System.Drawing.Size(33, 17);
			this.txtTimeTotal.Text = "Total";
			// 
			// txtResult
			// 
			this.txtResult.AllowDrop = true;
			this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtResult.Location = new System.Drawing.Point(12, 39);
			this.txtResult.Name = "txtResult";
			this.txtResult.ReadOnly = true;
			this.txtResult.Size = new System.Drawing.Size(484, 20);
			this.txtResult.TabIndex = 4;
			// 
			// txtBench
			// 
			this.txtBench.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtBench.Location = new System.Drawing.Point(502, 37);
			this.txtBench.Name = "txtBench";
			this.txtBench.Size = new System.Drawing.Size(62, 23);
			this.txtBench.TabIndex = 5;
			this.txtBench.Text = "bench";
			this.txtBench.UseVisualStyleBackColor = true;
			this.txtBench.Click += new System.EventHandler(this.TxtBench_Click);
			// 
			// txtAST
			// 
			this.txtAST.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAST.Location = new System.Drawing.Point(12, 65);
			this.txtAST.Name = "txtAST";
			this.txtAST.ReadOnly = true;
			this.txtAST.Size = new System.Drawing.Size(601, 20);
			this.txtAST.TabIndex = 6;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(625, 268);
			this.Controls.Add(this.txtAST);
			this.Controls.Add(this.txtBench);
			this.Controls.Add(this.txtResult);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.btnEquals);
			this.Controls.Add(this.txtExpression);
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.Text = "Calculator";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnEquals;
        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel txtTimeLexing;
        private System.Windows.Forms.ToolStripStatusLabel txtTimeParsing;
        private System.Windows.Forms.ToolStripStatusLabel txtTimeTotal;
		private System.Windows.Forms.TextBox txtResult;
		private System.Windows.Forms.ToolStripStatusLabel txtTimeExecuting;
		private System.Windows.Forms.Button txtBench;
		private System.Windows.Forms.TextBox txtAST;
	}
}