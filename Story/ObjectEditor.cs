using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Story
{
    partial class ObjectEditor : Form
    {

        public Level Level
        {
            get { return level; }
        }
        Level level;

        private int Selection;

        public ObjectEditor(Level level)
        {
            InitializeComponent();
            this.level = level;
        }

        private void ObjectEditor_Load(object sender, EventArgs e)
        {
            for (int ecx = 0; ecx < level.ObjectNames.Length; ecx++)
                if (level.ObjectNames[ecx] != null)
                    ObjectComboBox.Items.Add(level.ObjectNames[ecx]);
                else
                    break;

            ObjectComboBox.SelectedIndex = 0;

            RefreshList();
        }

        private void RefreshList()
        {
            int Id = 0;
            string Name;
            int OldSelection = ObjectsListBox.SelectedIndex;
            ObjectsListBox.Items.Clear();

            //foreach (WorldObject WorldObj in level.WorldObjects)
            for (int ecx = 0; ecx < level.WorldObjects.Count; ecx++)
            {
                Name = Convert.ToString(Id) + " ";

                Name += level.WorldObjects[ecx].TextureName + " ";

                if (level.WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Background)
                    Name += "(B)";
                else if (level.WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Normal)
                    Name += "(N)";
                else if (level.WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Foreground)
                    Name += "(F)";

                ObjectsListBox.Items.Add(Name);

                Id++;
            }

            if (OldSelection > ObjectsListBox.Items.Count - 1)
                OldSelection = ObjectsListBox.Items.Count - 1;

            ObjectsListBox.SelectedIndex = OldSelection;
        }

        private void objectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Variable selection will contain previous selection -- revert that back to old color
            if (Selection < level.WorldObjects.Count)
                //Setter involves recoloring
                level.WorldObjects[Selection].ObjectID = level.WorldObjects[Selection].ObjectID;

            Selection = ObjectsListBox.SelectedIndex;

            XTB.Text = Convert.ToString(level.WorldObjects[Selection].Start.X);
            YTB.Text = Convert.ToString(level.WorldObjects[Selection].Start.Y);

            IsMovingCB.Checked = level.WorldObjects[Selection].Moving;
            MoveSpeedX.Text = Convert.ToString(level.WorldObjects[Selection].VelocityX);
            MoveSpeedY.Text = Convert.ToString(level.WorldObjects[Selection].VelocityY);
            MoveOffXNeg.Text = Convert.ToString(level.WorldObjects[Selection].MinOffset.X);
            MoveOffXPos.Text = Convert.ToString(level.WorldObjects[Selection].MaxOffset.X);
            MoveOffYNeg.Text = Convert.ToString(level.WorldObjects[Selection].MinOffset.Y);
            MoveOffYPos.Text = Convert.ToString(level.WorldObjects[Selection].MaxOffset.Y);

            if (level.WorldObjects[Selection].layerDepth == WorldObject.LayerDepth.Background)
                BackgroundRB.Checked = true;
            else if (level.WorldObjects[Selection].layerDepth == WorldObject.LayerDepth.Normal)
                NormalRB.Checked = true;
            else if (level.WorldObjects[Selection].layerDepth == WorldObject.LayerDepth.Foreground)
                ForegroundRB.Checked = true;

            Selection = ObjectsListBox.SelectedIndex;
            ObjectComboBox.SelectedIndex = level.WorldObjects[Selection].ObjectID;
            level.WorldObjects[Selection].DrawColor = new Color(255, 255, 255, 128);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (ObjectsListBox.SelectedIndex != -1)
                level.WorldObjects.Remove(level.WorldObjects[ObjectsListBox.SelectedIndex]);
            RefreshList();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (ObjectsListBox.SelectedIndex != -1)
            {
                if (BackgroundRB.Checked)
                    level.WorldObjects[ObjectsListBox.SelectedIndex].layerDepth = WorldObject.LayerDepth.Background;
                else if (NormalRB.Checked)
                    level.WorldObjects[ObjectsListBox.SelectedIndex].layerDepth = WorldObject.LayerDepth.Normal;
                else if (ForegroundRB.Checked)
                    level.WorldObjects[ObjectsListBox.SelectedIndex].layerDepth = WorldObject.LayerDepth.Foreground;

                level.WorldObjects[ObjectsListBox.SelectedIndex].ObjectID = ObjectComboBox.SelectedIndex;
                level.WorldObjects[ObjectsListBox.SelectedIndex].TextureName = level.ObjectNames[ObjectComboBox.SelectedIndex];
                level.WorldObjects[ObjectsListBox.SelectedIndex].ObjectTexture = level.ObjectTextures[ObjectComboBox.SelectedIndex];

                int x = Convert.ToInt32(XTB.Text);
                int y = Convert.ToInt32(YTB.Text);

                //Movement related
                level.WorldObjects[ObjectsListBox.SelectedIndex].Moving = IsMovingCB.Checked;
                level.WorldObjects[ObjectsListBox.SelectedIndex].VelocityX = Convert.ToInt32(MoveSpeedX.Text);
                level.WorldObjects[ObjectsListBox.SelectedIndex].VelocityY = Convert.ToInt32(MoveSpeedY.Text);
                level.WorldObjects[ObjectsListBox.SelectedIndex].MinOffset.X = Convert.ToInt32(MoveOffXNeg.Text);
                level.WorldObjects[ObjectsListBox.SelectedIndex].MinOffset.Y = Convert.ToInt32(MoveOffYNeg.Text);
                level.WorldObjects[ObjectsListBox.SelectedIndex].MaxOffset.X = Convert.ToInt32(MoveOffXPos.Text);
                level.WorldObjects[ObjectsListBox.SelectedIndex].MaxOffset.Y = Convert.ToInt32(MoveOffYPos.Text);


                level.WorldObjects[ObjectsListBox.SelectedIndex].Position.X = x;
                level.WorldObjects[ObjectsListBox.SelectedIndex].Position.Y = y;
                level.WorldObjects[ObjectsListBox.SelectedIndex].Start.X = x;
                level.WorldObjects[ObjectsListBox.SelectedIndex].Start.Y = y;

                RefreshList();

            }
        }

        private void ObjectEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Restore color to all objects
            for (int ecx = 0; ecx < level.WorldObjects.Count; ecx++)
                //Setter involves recoloring
                level.WorldObjects[ecx].ObjectID = level.WorldObjects[ecx].ObjectID;
        }

        private void shiftButton_Click(object sender, EventArgs e)
        {
            for (int ecx = 0; ecx < level.WorldObjects.Count; ecx++)
            {
                level.WorldObjects[ecx].Start.X += Convert.ToInt32(XTB.Text);
                level.WorldObjects[ecx].Start.Y += Convert.ToInt32(YTB.Text);
                level.WorldObjects[ecx].Position.X += Convert.ToInt32(XTB.Text);
                level.WorldObjects[ecx].Position.Y += Convert.ToInt32(YTB.Text);
            }
        }
    }
}
