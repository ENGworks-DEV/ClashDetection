namespace RVT_AutomateClash
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
            this.okButton = new System.Windows.Forms.Button();
            this.LinkCategories = new System.Windows.Forms.CheckedListBox();
            this.linkedModelsCombo = new System.Windows.Forms.ComboBox();
            this.hostCategories = new System.Windows.Forms.CheckedListBox();
            this.hideCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(17, 336);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(318, 25);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // LinkCategories
            // 
            this.LinkCategories.CheckOnClick = true;
            this.LinkCategories.FormattingEnabled = true;
            this.LinkCategories.Location = new System.Drawing.Point(17, 67);
            this.LinkCategories.Margin = new System.Windows.Forms.Padding(2);
            this.LinkCategories.Name = "LinkCategories";
            this.LinkCategories.Size = new System.Drawing.Size(152, 184);
            this.LinkCategories.TabIndex = 1;
            this.LinkCategories.SelectedIndexChanged += new System.EventHandler(this.categoryCheckList_SelectedIndexChanged);
            // 
            // linkedModelsCombo
            // 
            this.linkedModelsCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.linkedModelsCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.linkedModelsCombo.FormattingEnabled = true;
            this.linkedModelsCombo.Location = new System.Drawing.Point(17, 27);
            this.linkedModelsCombo.Margin = new System.Windows.Forms.Padding(2);
            this.linkedModelsCombo.Name = "linkedModelsCombo";
            this.linkedModelsCombo.Size = new System.Drawing.Size(318, 21);
            this.linkedModelsCombo.TabIndex = 2;
            this.linkedModelsCombo.SelectedIndexChanged += new System.EventHandler(this.linkedModelsCombo_SelectedIndexChanged);
            // 
            // hostCategories
            // 
            this.hostCategories.CheckOnClick = true;
            this.hostCategories.FormattingEnabled = true;
            this.hostCategories.Location = new System.Drawing.Point(183, 67);
            this.hostCategories.Margin = new System.Windows.Forms.Padding(2);
            this.hostCategories.Name = "hostCategories";
            this.hostCategories.Size = new System.Drawing.Size(152, 184);
            this.hostCategories.TabIndex = 3;
            this.hostCategories.SelectedIndexChanged += new System.EventHandler(this.hostCategories_SelectedIndexChanged);
            // 
            // hideCheckBox
            // 
            this.hideCheckBox.AutoSize = true;
            this.hideCheckBox.Location = new System.Drawing.Point(17, 276);
            this.hideCheckBox.Name = "hideCheckBox";
            this.hideCheckBox.Size = new System.Drawing.Size(113, 17);
            this.hideCheckBox.TabIndex = 4;
            this.hideCheckBox.Text = "Hide All / Override";
            this.hideCheckBox.UseVisualStyleBackColor = true;
            this.hideCheckBox.CheckedChanged += new System.EventHandler(this.hideCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 372);
            this.Controls.Add(this.hideCheckBox);
            this.Controls.Add(this.hostCategories);
            this.Controls.Add(this.linkedModelsCombo);
            this.Controls.Add(this.LinkCategories);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "ClashSet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckedListBox LinkCategories;
        private System.Windows.Forms.ComboBox linkedModelsCombo;
        private System.Windows.Forms.CheckedListBox hostCategories;
        private System.Windows.Forms.CheckBox hideCheckBox;
    }
}