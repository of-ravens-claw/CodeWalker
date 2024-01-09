namespace CodeWalker.Forms
{
    partial class HexForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
				  System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexForm));
				  this.LineSizeDropDown = new System.Windows.Forms.ComboBox();
				  this.label1 = new System.Windows.Forms.Label();
				  this.statusStrip1 = new System.Windows.Forms.StatusStrip();
				  this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
				  this.HexTextBox = new CodeWalker.WinForms.TextBoxFix();
				  this.ignoreChecks = new System.Windows.Forms.CheckBox();
				  this.statusStrip1.SuspendLayout();
				  this.SuspendLayout();
				  // 
				  // LineSizeDropDown
				  // 
				  this.LineSizeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
				  this.LineSizeDropDown.FormattingEnabled = true;
				  this.LineSizeDropDown.Items.AddRange(new object[] {
            "8",
            "16",
            "32",
            "Text"});
				  this.LineSizeDropDown.Location = new System.Drawing.Point(105, 9);
				  this.LineSizeDropDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
				  this.LineSizeDropDown.Name = "LineSizeDropDown";
				  this.LineSizeDropDown.Size = new System.Drawing.Size(64, 24);
				  this.LineSizeDropDown.TabIndex = 102;
				  this.LineSizeDropDown.SelectedIndexChanged += new System.EventHandler(this.LineSizeDropDown_SelectedIndexChanged);
				  // 
				  // label1
				  // 
				  this.label1.AutoSize = true;
				  this.label1.Location = new System.Drawing.Point(29, 12);
				  this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
				  this.label1.Name = "label1";
				  this.label1.Size = new System.Drawing.Size(62, 16);
				  this.label1.TabIndex = 103;
				  this.label1.Text = "Line size:";
				  // 
				  // statusStrip1
				  // 
				  this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
				  this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
				  this.statusStrip1.Location = new System.Drawing.Point(0, 606);
				  this.statusStrip1.Name = "statusStrip1";
				  this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
				  this.statusStrip1.Size = new System.Drawing.Size(997, 22);
				  this.statusStrip1.TabIndex = 104;
				  this.statusStrip1.Text = "statusStrip1";
				  // 
				  // StatusLabel
				  // 
				  this.StatusLabel.Name = "StatusLabel";
				  this.StatusLabel.Size = new System.Drawing.Size(977, 16);
				  this.StatusLabel.Spring = true;
				  this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
				  // 
				  // HexTextBox
				  // 
				  this.HexTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
				  this.HexTextBox.Font = new System.Drawing.Font("Courier New", 8.25F);
				  this.HexTextBox.HideSelection = false;
				  this.HexTextBox.Location = new System.Drawing.Point(0, 42);
				  this.HexTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
				  this.HexTextBox.Multiline = true;
				  this.HexTextBox.Name = "HexTextBox";
				  this.HexTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
				  this.HexTextBox.Size = new System.Drawing.Size(996, 554);
				  this.HexTextBox.TabIndex = 105;
				  this.HexTextBox.WordWrap = false;
				  // 
				  // ignoreChecks
				  // 
				  this.ignoreChecks.AutoSize = true;
				  this.ignoreChecks.Location = new System.Drawing.Point(186, 11);
				  this.ignoreChecks.Name = "ignoreChecks";
				  this.ignoreChecks.Size = new System.Drawing.Size(195, 20);
				  this.ignoreChecks.TabIndex = 106;
				  this.ignoreChecks.Text = "Ignore Performance Checks";
				  this.ignoreChecks.UseVisualStyleBackColor = true;
				  this.ignoreChecks.CheckedChanged += new System.EventHandler(this.ignoreChecks_CheckedChanged);
				  // 
				  // HexForm
				  // 
				  this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
				  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				  this.ClientSize = new System.Drawing.Size(997, 628);
				  this.Controls.Add(this.ignoreChecks);
				  this.Controls.Add(this.HexTextBox);
				  this.Controls.Add(this.statusStrip1);
				  this.Controls.Add(this.label1);
				  this.Controls.Add(this.LineSizeDropDown);
				  this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
				  this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
				  this.Name = "HexForm";
				  this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
				  this.Text = "Hex Viewer - CodeWalker by dexyfex";
				  this.statusStrip1.ResumeLayout(false);
				  this.statusStrip1.PerformLayout();
				  this.ResumeLayout(false);
				  this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox LineSizeDropDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private WinForms.TextBoxFix HexTextBox;
			private System.Windows.Forms.CheckBox ignoreChecks;
	  }
}