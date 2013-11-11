using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Story
{
    partial class CollisionBoxEditor : Form
    {

        public Level Level
        {
            get { return level; }
        }
        Level level;

        private int Selection;

        public CollisionBoxEditor(Level level)
        {
            InitializeComponent();
            this.level = level;
        }

        private void CollisionBoxEditor_Load(object sender, EventArgs e)
        {
            SpecialAttribute.SelectedIndex = 0;
            RefreshList();
        }

        private void RefreshList()
        {
            int Id = 0;
            string Name;
            int OldSelection = CollisionObjectsListBox.SelectedIndex;
            CollisionObjectsListBox.Items.Clear();

            //foreach (CollisionObject collisionObj in level.CollisionObjects)
            for (int ecx = 0; ecx < level.CollisionObjects.Count; ecx++)
            {
                if (level.CollisionObjects[ecx].complete == true)
                {
                    Name = Convert.ToString(Id) + " ";

                    if (level.CollisionObjects[ecx].IsTriangle)
                        Name += "T";
                    else
                        Name += "R";

                    if (level.CollisionObjects[ecx].IsNPCOnly)
                        Name += " (NPC)";

                    CollisionObjectsListBox.Items.Add(Name);
                }
                Id++;
            }
            if (OldSelection > CollisionObjectsListBox.Items.Count - 1)
                OldSelection = CollisionObjectsListBox.Items.Count - 1;
            CollisionObjectsListBox.SelectedIndex = OldSelection;
        }

        //Read in values from the selected object
        private void collisionObjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Selection < level.CollisionObjects.Count)
                if (level.CollisionObjects[Selection].IsNPCOnly)
                    level.CollisionObjects[Selection].Color = Microsoft.Xna.Framework.Color.Blue;
                else if (level.CollisionObjects[Selection].Liquid)
                    level.CollisionObjects[Selection].Color = Microsoft.Xna.Framework.Color.Pink;
                else if (level.CollisionObjects[Selection].Passable)
                    level.CollisionObjects[Selection].Color = Microsoft.Xna.Framework.Color.LightGreen;
                else if (level.CollisionObjects[Selection].Damaging)
                    level.CollisionObjects[Selection].Color = Microsoft.Xna.Framework.Color.Gray;
                else
                    level.CollisionObjects[Selection].Color = Microsoft.Xna.Framework.Color.Yellow;


            Selection = CollisionObjectsListBox.SelectedIndex;

            if (level.CollisionObjects[Selection].Passable)
                SpecialAttribute.SelectedIndex = 1;
            else if (level.CollisionObjects[Selection].Liquid)
                SpecialAttribute.SelectedIndex = 2;
            else if (level.CollisionObjects[Selection].Damaging)
                SpecialAttribute.SelectedIndex = 3;
            else
                SpecialAttribute.SelectedIndex = 0;

            DamageVal.Value = level.CollisionObjects[Selection].Damage;
            XTB.Text = Convert.ToString(level.CollisionObjects[Selection].Start.X);
            YTB.Text = Convert.ToString(level.CollisionObjects[Selection].Start.Y);
            WidthTB.Text = Convert.ToString(level.CollisionObjects[Selection].Width);
            HeightTB.Text = Convert.ToString(level.CollisionObjects[Selection].Height);

            IsMovingCB.Checked = level.CollisionObjects[Selection].Moving;
            MoveSpeedX.Text = Convert.ToString(level.CollisionObjects[Selection].VelocityX);
            MoveSpeedY.Text = Convert.ToString(level.CollisionObjects[Selection].VelocityY);
            MoveOffXNeg.Text = Convert.ToString(level.CollisionObjects[Selection].MinOffset.X);
            MoveOffXPos.Text = Convert.ToString(level.CollisionObjects[Selection].MaxOffset.X);
            MoveOffYNeg.Text = Convert.ToString(level.CollisionObjects[Selection].MinOffset.Y);
            MoveOffYPos.Text = Convert.ToString(level.CollisionObjects[Selection].MaxOffset.Y);

            if (level.CollisionObjects[Selection].IsTriangle)
                IsTriangle.Checked = true;
            else
                IsTriangle.Checked = false;

            if (level.CollisionObjects[Selection].IsNPCOnly)
                IsNPC.Checked = true;
            else
                IsNPC.Checked = false;



            level.CollisionObjects[Selection].Color = Microsoft.Xna.Framework.Color.Red;

        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (CollisionObjectsListBox.SelectedIndex != -1)
                level.CollisionObjects.Remove(level.CollisionObjects[CollisionObjectsListBox.SelectedIndex]);
            RefreshList();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (CollisionObjectsListBox.SelectedIndex != -1)
            {

                if (SpecialAttribute.SelectedIndex == 1)
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Passable = true;
                else
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Passable = false;

                if (SpecialAttribute.SelectedIndex == 2)
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Liquid = true;
                else
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Liquid = false;

                if (SpecialAttribute.SelectedIndex == 3)
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Damaging = true;
                else
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Damaging = false;


                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Damage = (int)DamageVal.Value;

                int x = Convert.ToInt32(XTB.Text);
                int y = Convert.ToInt32(YTB.Text);
                int w = Convert.ToInt32(WidthTB.Text);
                int h = Convert.ToInt32(HeightTB.Text);

                if (w < 0 && !IsTriangle.Checked)
                {
                    x = x + w;
                    w = System.Math.Abs(w);
                }
                if (h < 0)
                {
                    y = y + h;
                    h = System.Math.Abs(h);
                }

                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Start.X =
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[0].X + Main.BackBufferWidth / 2;
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Start.Y =
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[0].Y + Main.BackBufferHeight / Tile.VerticalScale;
             
                if (IsNPC.Checked)
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].IsNPCOnly = true;
                else
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].IsNPCOnly = false;

                if (IsTriangle.Checked)
                {
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].IsTriangle = true;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[0].X = x;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[1].X = x;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[2].X = x;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[3].X = x + w;
                }
                else
                {
                    //Width is negative (happens when switching from left triangle to rectangle)
                    if (IsTriangle.Checked)
                        level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].IsTriangle = true;
                    else
                         level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].IsTriangle = false;

                    //x = x + w;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[0].X = x;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[1].X = x + w;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[2].X = x;
                    level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[3].X = x + w;
                }

                //Y Parts
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[0].Y = y;
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[1].Y = y;
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[2].Y = y + h;
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Coords[3].Y = y + h;

                //Movement related
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].Moving = IsMovingCB.Checked;
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].VelocityX = Convert.ToInt32(MoveSpeedX.Text);
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].VelocityY = Convert.ToInt32(MoveSpeedY.Text);
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].MinOffset.X = Convert.ToInt32(MoveOffXNeg.Text);
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].MinOffset.Y = Convert.ToInt32(MoveOffYNeg.Text);
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].MaxOffset.X = Convert.ToInt32(MoveOffXPos.Text);
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].MaxOffset.Y = Convert.ToInt32(MoveOffYPos.Text);

                //Annnd update
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].UpdateObject();
                level.CollisionObjects[CollisionObjectsListBox.SelectedIndex].ChangeLines = true;
            }
            RefreshList();
        }

        private void CollisionBoxEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Restore color to all objects
            for (int ecx = 0; ecx < level.CollisionObjects.Count; ecx++)
                if (level.CollisionObjects[ecx].IsNPCOnly)
                    level.CollisionObjects[ecx].Color = Microsoft.Xna.Framework.Color.Blue;
                else if (level.CollisionObjects[ecx].Liquid)
                    level.CollisionObjects[ecx].Color = Microsoft.Xna.Framework.Color.Pink;
                else if (level.CollisionObjects[ecx].Passable)
                    level.CollisionObjects[ecx].Color = Microsoft.Xna.Framework.Color.LightGreen;
                else if (level.CollisionObjects[ecx].Damaging)
                    level.CollisionObjects[ecx].Color = Microsoft.Xna.Framework.Color.Gray;
                else
                    level.CollisionObjects[ecx].Color = Microsoft.Xna.Framework.Color.Yellow;
        }

        private void shiftButton_Click(object sender, EventArgs e)
        {
            if (XTB.Text == "")
                XTB.Text = "0";
            if (YTB.Text == "")
                YTB.Text = "0";
            for (int ecx = 0; ecx < level.CollisionObjects.Count; ecx++)
            {
                level.CollisionObjects[ecx].Start.X += Convert.ToInt32(XTB.Text);
                level.CollisionObjects[ecx].Start.Y += Convert.ToInt32(YTB.Text);
                level.CollisionObjects[ecx].Coords[0].X += Convert.ToInt32(XTB.Text);
                level.CollisionObjects[ecx].Coords[0].Y += Convert.ToInt32(YTB.Text);
                level.CollisionObjects[ecx].Coords[1].X += Convert.ToInt32(XTB.Text);
                level.CollisionObjects[ecx].Coords[1].Y += Convert.ToInt32(YTB.Text);
                level.CollisionObjects[ecx].Coords[2].X += Convert.ToInt32(XTB.Text);
                level.CollisionObjects[ecx].Coords[2].Y += Convert.ToInt32(YTB.Text);
                level.CollisionObjects[ecx].Coords[3].X += Convert.ToInt32(XTB.Text);
                level.CollisionObjects[ecx].Coords[3].Y += Convert.ToInt32(YTB.Text);
                level.CollisionObjects[ecx].ChangeLines = true;
            }
        }


    }
}
