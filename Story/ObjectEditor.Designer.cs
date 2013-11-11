namespace Story
{
    partial class ObjectEditor
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
            this.RefreshButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.AttributeGroupBox = new System.Windows.Forms.GroupBox();
            this.MoveSpeedY = new System.Windows.Forms.TextBox();
            this.MoveSpeedX = new System.Windows.Forms.TextBox();
            this.MoveOffYPos = new System.Windows.Forms.TextBox();
            this.MoveOffXPos = new System.Windows.Forms.TextBox();
            this.MoveOffYNeg = new System.Windows.Forms.TextBox();
            this.MoveOffXNeg = new System.Windows.Forms.TextBox();
            this.IsMovingCB = new System.Windows.Forms.CheckBox();
            this.ForegroundRB = new System.Windows.Forms.RadioButton();
            this.NormalRB = new System.Windows.Forms.RadioButton();
            this.BackgroundRB = new System.Windows.Forms.RadioButton();
            this.ObjectComboBox = new System.Windows.Forms.ComboBox();
            this.YTB = new System.Windows.Forms.TextBox();
            this.XTB = new System.Windows.Forms.TextBox();
            this.ShiftButton = new System.Windows.Forms.Button();
            this.ObjectsListBox = new System.Windows.Forms.ListBox();
            this.AttributeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(91, 204);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshButton.TabIndex = 2;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(172, 204);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 3;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(10, 204);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteButton.TabIndex = 1;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // AttributeGroupBox
            // 
            this.AttributeGroupBox.Controls.Add(this.MoveSpeedY);
            this.AttributeGroupBox.Controls.Add(this.MoveSpeedX);
            this.AttributeGroupBox.Controls.Add(this.MoveOffYPos);
            this.AttributeGroupBox.Controls.Add(this.MoveOffXPos);
            this.AttributeGroupBox.Controls.Add(this.MoveOffYNeg);
            this.AttributeGroupBox.Controls.Add(this.MoveOffXNeg);
            this.AttributeGroupBox.Controls.Add(this.IsMovingCB);
            this.AttributeGroupBox.Controls.Add(this.ForegroundRB);
            this.AttributeGroupBox.Controls.Add(this.NormalRB);
            this.AttributeGroupBox.Controls.Add(this.BackgroundRB);
            this.AttributeGroupBox.Controls.Add(this.ObjectComboBox);
            this.AttributeGroupBox.Controls.Add(this.YTB);
            this.AttributeGroupBox.Controls.Add(this.XTB);
            this.AttributeGroupBox.Location = new System.Drawing.Point(172, 12);
            this.AttributeGroupBox.Name = "AttributeGroupBox";
            this.AttributeGroupBox.Size = new System.Drawing.Size(147, 186);
            this.AttributeGroupBox.TabIndex = 6;
            this.AttributeGroupBox.TabStop = false;
            this.AttributeGroupBox.Text = "x,y,etc";
            // 
            // MoveSpeedY
            // 
            this.MoveSpeedY.Location = new System.Drawing.Point(67, 124);
            this.MoveSpeedY.Name = "MoveSpeedY";
            this.MoveSpeedY.Size = new System.Drawing.Size(55, 20);
            this.MoveSpeedY.TabIndex = 9;
            // 
            // MoveSpeedX
            // 
            this.MoveSpeedX.Location = new System.Drawing.Point(7, 124);
            this.MoveSpeedX.Name = "MoveSpeedX";
            this.MoveSpeedX.Size = new System.Drawing.Size(55, 20);
            this.MoveSpeedX.TabIndex = 8;
            // 
            // MoveOffYPos
            // 
            this.MoveOffYPos.Location = new System.Drawing.Point(67, 98);
            this.MoveOffYPos.Name = "MoveOffYPos";
            this.MoveOffYPos.Size = new System.Drawing.Size(55, 20);
            this.MoveOffYPos.TabIndex = 7;
            // 
            // MoveOffXPos
            // 
            this.MoveOffXPos.Location = new System.Drawing.Point(7, 98);
            this.MoveOffXPos.Name = "MoveOffXPos";
            this.MoveOffXPos.Size = new System.Drawing.Size(55, 20);
            this.MoveOffXPos.TabIndex = 6;
            // 
            // MoveOffYNeg
            // 
            this.MoveOffYNeg.Location = new System.Drawing.Point(67, 72);
            this.MoveOffYNeg.Name = "MoveOffYNeg";
            this.MoveOffYNeg.Size = new System.Drawing.Size(55, 20);
            this.MoveOffYNeg.TabIndex = 5;
            // 
            // MoveOffXNeg
            // 
            this.MoveOffXNeg.Location = new System.Drawing.Point(6, 72);
            this.MoveOffXNeg.Name = "MoveOffXNeg";
            this.MoveOffXNeg.Size = new System.Drawing.Size(55, 20);
            this.MoveOffXNeg.TabIndex = 4;
            // 
            // IsMovingCB
            // 
            this.IsMovingCB.AutoSize = true;
            this.IsMovingCB.Location = new System.Drawing.Point(7, 148);
            this.IsMovingCB.Name = "IsMovingCB";
            this.IsMovingCB.Size = new System.Drawing.Size(35, 17);
            this.IsMovingCB.TabIndex = 10;
            this.IsMovingCB.Text = "M";
            this.IsMovingCB.UseVisualStyleBackColor = true;
            // 
            // ForegroundRB
            // 
            this.ForegroundRB.AutoSize = true;
            this.ForegroundRB.Location = new System.Drawing.Point(101, 163);
            this.ForegroundRB.Name = "ForegroundRB";
            this.ForegroundRB.Size = new System.Drawing.Size(39, 17);
            this.ForegroundRB.TabIndex = 11;
            this.ForegroundRB.TabStop = true;
            this.ForegroundRB.Text = "FG";
            this.ForegroundRB.UseVisualStyleBackColor = true;
            // 
            // NormalRB
            // 
            this.NormalRB.AutoSize = true;
            this.NormalRB.Location = new System.Drawing.Point(53, 163);
            this.NormalRB.Name = "NormalRB";
            this.NormalRB.Size = new System.Drawing.Size(42, 17);
            this.NormalRB.TabIndex = 11;
            this.NormalRB.TabStop = true;
            this.NormalRB.Text = "MG";
            this.NormalRB.UseVisualStyleBackColor = true;
            // 
            // BackgroundRB
            // 
            this.BackgroundRB.AutoSize = true;
            this.BackgroundRB.Location = new System.Drawing.Point(7, 163);
            this.BackgroundRB.Name = "BackgroundRB";
            this.BackgroundRB.Size = new System.Drawing.Size(40, 17);
            this.BackgroundRB.TabIndex = 11;
            this.BackgroundRB.TabStop = true;
            this.BackgroundRB.Text = "BG";
            this.BackgroundRB.UseVisualStyleBackColor = true;
            // 
            // ObjectComboBox
            // 
            this.ObjectComboBox.FormattingEnabled = true;
            this.ObjectComboBox.Location = new System.Drawing.Point(6, 45);
            this.ObjectComboBox.Name = "ObjectComboBox";
            this.ObjectComboBox.Size = new System.Drawing.Size(134, 21);
            this.ObjectComboBox.TabIndex = 3;
            // 
            // YTB
            // 
            this.YTB.Location = new System.Drawing.Point(76, 19);
            this.YTB.Name = "YTB";
            this.YTB.Size = new System.Drawing.Size(64, 20);
            this.YTB.TabIndex = 2;
            // 
            // XTB
            // 
            this.XTB.Location = new System.Drawing.Point(6, 19);
            this.XTB.Name = "XTB";
            this.XTB.Size = new System.Drawing.Size(64, 20);
            this.XTB.TabIndex = 1;
            // 
            // ShiftButton
            // 
            this.ShiftButton.Location = new System.Drawing.Point(253, 204);
            this.ShiftButton.Name = "ShiftButton";
            this.ShiftButton.Size = new System.Drawing.Size(59, 23);
            this.ShiftButton.TabIndex = 4;
            this.ShiftButton.Text = "Shift All";
            this.ShiftButton.UseVisualStyleBackColor = true;
            this.ShiftButton.Click += new System.EventHandler(this.shiftButton_Click);
            // 
            // ObjectsListBox
            // 
            this.ObjectsListBox.FormattingEnabled = true;
            this.ObjectsListBox.Location = new System.Drawing.Point(12, 12);
            this.ObjectsListBox.Name = "ObjectsListBox";
            this.ObjectsListBox.Size = new System.Drawing.Size(154, 186);
            this.ObjectsListBox.TabIndex = 0;
            this.ObjectsListBox.SelectedIndexChanged += new System.EventHandler(this.objectsListBox_SelectedIndexChanged);
            // 
            // ObjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 236);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.AttributeGroupBox);
            this.Controls.Add(this.ObjectsListBox);
            this.Controls.Add(this.ShiftButton);
            this.Name = "ObjectEditor";
            this.Text = "ObjectEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectEditor_FormClosing);
            this.Load += new System.EventHandler(this.ObjectEditor_Load);
            this.AttributeGroupBox.ResumeLayout(false);
            this.AttributeGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.GroupBox AttributeGroupBox;
        private System.Windows.Forms.TextBox YTB;
        private System.Windows.Forms.TextBox XTB;
        private System.Windows.Forms.ListBox ObjectsListBox;
        private System.Windows.Forms.ComboBox ObjectComboBox;
        private System.Windows.Forms.RadioButton ForegroundRB;
        private System.Windows.Forms.RadioButton NormalRB;
        private System.Windows.Forms.RadioButton BackgroundRB;
        private System.Windows.Forms.Button ShiftButton;
        private System.Windows.Forms.CheckBox IsMovingCB;
        private System.Windows.Forms.TextBox MoveOffYPos;
        private System.Windows.Forms.TextBox MoveOffXPos;
        private System.Windows.Forms.TextBox MoveOffYNeg;
        private System.Windows.Forms.TextBox MoveOffXNeg;
        private System.Windows.Forms.TextBox MoveSpeedX;
        private System.Windows.Forms.TextBox MoveSpeedY;
    }
}