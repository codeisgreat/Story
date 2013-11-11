namespace Story
{
    partial class CollisionBoxEditor
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
            this.CollisionObjectsListBox = new System.Windows.Forms.ListBox();
            this.XTB = new System.Windows.Forms.TextBox();
            this.AttributeGroupBox = new System.Windows.Forms.GroupBox();
            this.IsNPC = new System.Windows.Forms.CheckBox();
            this.IsTriangle = new System.Windows.Forms.CheckBox();
            this.IsMovingCB = new System.Windows.Forms.CheckBox();
            this.DamageVal = new System.Windows.Forms.NumericUpDown();
            this.SpecialAttribute = new System.Windows.Forms.ComboBox();
            this.MoveSpeedY = new System.Windows.Forms.TextBox();
            this.MoveSpeedX = new System.Windows.Forms.TextBox();
            this.MoveOffYPos = new System.Windows.Forms.TextBox();
            this.HeightTB = new System.Windows.Forms.TextBox();
            this.MoveOffXPos = new System.Windows.Forms.TextBox();
            this.WidthTB = new System.Windows.Forms.TextBox();
            this.MoveOffYNeg = new System.Windows.Forms.TextBox();
            this.YTB = new System.Windows.Forms.TextBox();
            this.MoveOffXNeg = new System.Windows.Forms.TextBox();
            this.ShiftButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.AttributeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DamageVal)).BeginInit();
            this.SuspendLayout();
            // 
            // CollisionObjectsListBox
            // 
            this.CollisionObjectsListBox.FormattingEnabled = true;
            this.CollisionObjectsListBox.Location = new System.Drawing.Point(12, 12);
            this.CollisionObjectsListBox.Name = "CollisionObjectsListBox";
            this.CollisionObjectsListBox.Size = new System.Drawing.Size(154, 186);
            this.CollisionObjectsListBox.TabIndex = 0;
            this.CollisionObjectsListBox.SelectedIndexChanged += new System.EventHandler(this.collisionObjectsListBox_SelectedIndexChanged);
            // 
            // XTB
            // 
            this.XTB.Location = new System.Drawing.Point(6, 19);
            this.XTB.Name = "XTB";
            this.XTB.Size = new System.Drawing.Size(55, 20);
            this.XTB.TabIndex = 0;
            // 
            // AttributeGroupBox
            // 
            this.AttributeGroupBox.Controls.Add(this.IsNPC);
            this.AttributeGroupBox.Controls.Add(this.IsTriangle);
            this.AttributeGroupBox.Controls.Add(this.IsMovingCB);
            this.AttributeGroupBox.Controls.Add(this.DamageVal);
            this.AttributeGroupBox.Controls.Add(this.SpecialAttribute);
            this.AttributeGroupBox.Controls.Add(this.MoveSpeedY);
            this.AttributeGroupBox.Controls.Add(this.MoveSpeedX);
            this.AttributeGroupBox.Controls.Add(this.MoveOffYPos);
            this.AttributeGroupBox.Controls.Add(this.HeightTB);
            this.AttributeGroupBox.Controls.Add(this.MoveOffXPos);
            this.AttributeGroupBox.Controls.Add(this.WidthTB);
            this.AttributeGroupBox.Controls.Add(this.MoveOffYNeg);
            this.AttributeGroupBox.Controls.Add(this.YTB);
            this.AttributeGroupBox.Controls.Add(this.MoveOffXNeg);
            this.AttributeGroupBox.Controls.Add(this.XTB);
            this.AttributeGroupBox.Location = new System.Drawing.Point(172, 12);
            this.AttributeGroupBox.Name = "AttributeGroupBox";
            this.AttributeGroupBox.Size = new System.Drawing.Size(170, 186);
            this.AttributeGroupBox.TabIndex = 5;
            this.AttributeGroupBox.TabStop = false;
            this.AttributeGroupBox.Text = "x, w, \\n, y, h";
            // 
            // IsNPC
            // 
            this.IsNPC.AutoSize = true;
            this.IsNPC.Location = new System.Drawing.Point(128, 48);
            this.IsNPC.Name = "IsNPC";
            this.IsNPC.Size = new System.Drawing.Size(34, 17);
            this.IsNPC.TabIndex = 17;
            this.IsNPC.Text = "N";
            this.IsNPC.UseVisualStyleBackColor = true;
            // 
            // IsTriangle
            // 
            this.IsTriangle.AutoSize = true;
            this.IsTriangle.Location = new System.Drawing.Point(128, 22);
            this.IsTriangle.Name = "IsTriangle";
            this.IsTriangle.Size = new System.Drawing.Size(33, 17);
            this.IsTriangle.TabIndex = 16;
            this.IsTriangle.Text = "T";
            this.IsTriangle.UseVisualStyleBackColor = true;
            // 
            // IsMovingCB
            // 
            this.IsMovingCB.AutoSize = true;
            this.IsMovingCB.Location = new System.Drawing.Point(128, 74);
            this.IsMovingCB.Name = "IsMovingCB";
            this.IsMovingCB.Size = new System.Drawing.Size(35, 17);
            this.IsMovingCB.TabIndex = 15;
            this.IsMovingCB.Text = "M";
            this.IsMovingCB.UseVisualStyleBackColor = true;
            // 
            // DamageVal
            // 
            this.DamageVal.Location = new System.Drawing.Point(116, 159);
            this.DamageVal.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.DamageVal.Name = "DamageVal";
            this.DamageVal.Size = new System.Drawing.Size(48, 20);
            this.DamageVal.TabIndex = 11;
            // 
            // SpecialAttribute
            // 
            this.SpecialAttribute.FormattingEnabled = true;
            this.SpecialAttribute.Items.AddRange(new object[] {
            "Normal",
            "Passable",
            "Liquid",
            "Damaging"});
            this.SpecialAttribute.Location = new System.Drawing.Point(6, 159);
            this.SpecialAttribute.Name = "SpecialAttribute";
            this.SpecialAttribute.Size = new System.Drawing.Size(104, 21);
            this.SpecialAttribute.TabIndex = 10;
            // 
            // MoveSpeedY
            // 
            this.MoveSpeedY.Location = new System.Drawing.Point(67, 123);
            this.MoveSpeedY.Name = "MoveSpeedY";
            this.MoveSpeedY.Size = new System.Drawing.Size(55, 20);
            this.MoveSpeedY.TabIndex = 9;
            // 
            // MoveSpeedX
            // 
            this.MoveSpeedX.Location = new System.Drawing.Point(6, 123);
            this.MoveSpeedX.Name = "MoveSpeedX";
            this.MoveSpeedX.Size = new System.Drawing.Size(55, 20);
            this.MoveSpeedX.TabIndex = 8;
            // 
            // MoveOffYPos
            // 
            this.MoveOffYPos.Location = new System.Drawing.Point(67, 97);
            this.MoveOffYPos.Name = "MoveOffYPos";
            this.MoveOffYPos.Size = new System.Drawing.Size(55, 20);
            this.MoveOffYPos.TabIndex = 7;
            // 
            // HeightTB
            // 
            this.HeightTB.Location = new System.Drawing.Point(67, 45);
            this.HeightTB.Name = "HeightTB";
            this.HeightTB.Size = new System.Drawing.Size(55, 20);
            this.HeightTB.TabIndex = 3;
            // 
            // MoveOffXPos
            // 
            this.MoveOffXPos.Location = new System.Drawing.Point(6, 97);
            this.MoveOffXPos.Name = "MoveOffXPos";
            this.MoveOffXPos.Size = new System.Drawing.Size(55, 20);
            this.MoveOffXPos.TabIndex = 6;
            // 
            // WidthTB
            // 
            this.WidthTB.Location = new System.Drawing.Point(67, 19);
            this.WidthTB.Name = "WidthTB";
            this.WidthTB.Size = new System.Drawing.Size(55, 20);
            this.WidthTB.TabIndex = 1;
            // 
            // MoveOffYNeg
            // 
            this.MoveOffYNeg.Location = new System.Drawing.Point(67, 71);
            this.MoveOffYNeg.Name = "MoveOffYNeg";
            this.MoveOffYNeg.Size = new System.Drawing.Size(55, 20);
            this.MoveOffYNeg.TabIndex = 5;
            // 
            // YTB
            // 
            this.YTB.Location = new System.Drawing.Point(6, 45);
            this.YTB.Name = "YTB";
            this.YTB.Size = new System.Drawing.Size(55, 20);
            this.YTB.TabIndex = 2;
            // 
            // MoveOffXNeg
            // 
            this.MoveOffXNeg.Location = new System.Drawing.Point(6, 71);
            this.MoveOffXNeg.Name = "MoveOffXNeg";
            this.MoveOffXNeg.Size = new System.Drawing.Size(55, 20);
            this.MoveOffXNeg.TabIndex = 4;
            // 
            // ShiftButton
            // 
            this.ShiftButton.Location = new System.Drawing.Point(273, 204);
            this.ShiftButton.Name = "ShiftButton";
            this.ShiftButton.Size = new System.Drawing.Size(63, 23);
            this.ShiftButton.TabIndex = 4;
            this.ShiftButton.Text = "Shift All";
            this.ShiftButton.UseVisualStyleBackColor = true;
            this.ShiftButton.Click += new System.EventHandler(this.shiftButton_Click);
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
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(176, 204);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 3;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.applyButton_Click);
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
            // CollisionBoxEditor
            // 
            this.AcceptButton = this.ApplyButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 234);
            this.Controls.Add(this.ShiftButton);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.AttributeGroupBox);
            this.Controls.Add(this.CollisionObjectsListBox);
            this.Name = "CollisionBoxEditor";
            this.Text = "CollisionBoxEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CollisionBoxEditor_FormClosing);
            this.Load += new System.EventHandler(this.CollisionBoxEditor_Load);
            this.AttributeGroupBox.ResumeLayout(false);
            this.AttributeGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DamageVal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox CollisionObjectsListBox;
        private System.Windows.Forms.TextBox XTB;
        private System.Windows.Forms.GroupBox AttributeGroupBox;
        private System.Windows.Forms.TextBox HeightTB;
        private System.Windows.Forms.TextBox WidthTB;
        private System.Windows.Forms.TextBox YTB;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.ComboBox SpecialAttribute;
        private System.Windows.Forms.NumericUpDown DamageVal;
        private System.Windows.Forms.Button ShiftButton;
        private System.Windows.Forms.CheckBox IsMovingCB;
        private System.Windows.Forms.TextBox MoveOffYPos;
        private System.Windows.Forms.TextBox MoveOffXPos;
        private System.Windows.Forms.TextBox MoveOffYNeg;
        private System.Windows.Forms.TextBox MoveOffXNeg;
        private System.Windows.Forms.TextBox MoveSpeedX;
        private System.Windows.Forms.TextBox MoveSpeedY;
        private System.Windows.Forms.CheckBox IsNPC;
        private System.Windows.Forms.CheckBox IsTriangle;
    }
}