using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Story
{
    class LevelEditor
    {
        private MouseState MouseStateCurrent, MouseStatePrevious;
        private KeyboardState KeyboardStateCurrent, KeyboardStatePrevious;

        public Level Level
        {
            get { return level; }
        }
        Level level;


        CollisionBoxEditor CollisionBoxEditor;
        ObjectEditor ObjectEditor;
        bool IsPlacingObj = false;
        Vector2 StartCoords;
        Vector2 CursorCoords;

        public LevelEditor(Level level)
        {
            this.level = level;
            CollisionBoxEditor = new CollisionBoxEditor(level);
            ObjectEditor = new ObjectEditor(level);
        }

        public void Update(GameTime GameTime)
        {
            MouseStatePrevious = MouseStateCurrent;
            MouseStateCurrent = Mouse.GetState();
            KeyboardStatePrevious = KeyboardStateCurrent;
            KeyboardStateCurrent = Keyboard.GetState();
            CursorCoords.X = MouseStateCurrent.X + Convert.ToInt32(level.Player.Position.X - Main.BackBufferWidth / 2);
            CursorCoords.Y = MouseStateCurrent.Y + Convert.ToInt32(level.Player.Position.Y - level.Player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale);

            if (CollisionBoxEditor.Visible)
            {
                StartShape();
                UpdateShape();
                FinishShape();
            }
            else if (ObjectEditor.Visible)
            {
                StartObject();
                UpdateObject();
                FinishObject();
            }

            UpdateEditors();

            //and other editing things.
            Save();
        }

        private void UpdateEditors()
        {
            if (KeyboardStateCurrent.IsKeyDown(Keys.LeftControl) && KeyboardStateCurrent.IsKeyDown(Keys.E))
                if (!KeyboardStatePrevious.IsKeyDown(Keys.E))
                {
                    if (CollisionBoxEditor.IsDisposed)
                        CollisionBoxEditor = new CollisionBoxEditor(level);

                    CollisionBoxEditor.Show();
                }

            if (KeyboardStateCurrent.IsKeyDown(Keys.LeftControl) && KeyboardStateCurrent.IsKeyDown(Keys.O))
                if (!KeyboardStatePrevious.IsKeyDown(Keys.O))
                {
                    if (ObjectEditor.IsDisposed)
                        ObjectEditor = new ObjectEditor(level);

                    ObjectEditor.Show();
                }
        }

        private bool MouseOnScreen()
        {

            if (MouseStateCurrent.X >= 0 && MouseStateCurrent.Y >= 0)
            {
                if (MouseStateCurrent.X <= Main.BackBufferWidth && MouseStateCurrent.Y <= Main.BackBufferHeight)
                    return true;
            }


            return false;
        }

        private void StartObject()
        {
            if (MouseOnScreen() == true)
            {
                if (MouseStateCurrent.LeftButton == ButtonState.Pressed && MouseStatePrevious.LeftButton == ButtonState.Released)
                {
                    //Mouse was clicked
                    if (IsPlacingObj == false)
                    {
                        StartCoords.X = CursorCoords.X;
                        StartCoords.Y = CursorCoords.Y;

                        //Background object
                        if (KeyboardStateCurrent.IsKeyDown(Keys.LeftShift) == true)
                        {
                            level.WorldObjects.Add(new WorldObject(0, level.ObjectNames[0], level.ObjectTextures[0], CursorCoords));
                            level.WorldObjects[level.WorldObjects.Count - 1].layerDepth = WorldObject.LayerDepth.Background;
                        }
                        //Foreground object
                        else if (KeyboardStateCurrent.IsKeyDown(Keys.LeftAlt) == true)
                        {
                            level.WorldObjects.Add(new WorldObject(2, level.ObjectNames[2], level.ObjectTextures[2], CursorCoords));
                            level.WorldObjects[level.WorldObjects.Count - 1].layerDepth = WorldObject.LayerDepth.Foreground;
                        }
                        //Normal object
                        else
                        {
                            level.WorldObjects.Add(new WorldObject(1, level.ObjectNames[1], level.ObjectTextures[1], CursorCoords));
                            level.WorldObjects[level.WorldObjects.Count - 1].layerDepth = WorldObject.LayerDepth.Normal;
                        }

                        IsPlacingObj = true;
                    }

                }
            }
        }

        private void UpdateObject()
        {
            //Mouse continued to be pressed; update object coords
            if (MouseStateCurrent.LeftButton == ButtonState.Pressed)
            {
                if (IsPlacingObj == true)
                {
                    level.WorldObjects[level.WorldObjects.Count - 1].Position = CursorCoords;
                }
            }
        }

        private void FinishObject()
        {
            //Mouse released; done placing
            if (MouseStateCurrent.LeftButton == ButtonState.Released)
            {
                if (IsPlacingObj)
                {
                    //Stop placing object and place final coords
                    level.WorldObjects[level.WorldObjects.Count - 1].Position = CursorCoords;
                    level.WorldObjects[level.WorldObjects.Count - 1].Start = CursorCoords;
                    IsPlacingObj = false;
                }
            }
        }


        private void StartShape()
        {
            if (MouseOnScreen() == true)
            {
                if (MouseStateCurrent.LeftButton == ButtonState.Pressed && MouseStatePrevious.LeftButton == ButtonState.Released)
                {
                    //Mouse was clicked
                    if (IsPlacingObj == false)
                    {
                        //Add new collision object at cursor coords
                        level.CollisionObjects.Add(new CollisionObject(CursorCoords.X, CursorCoords.Y, this.level, false));

                        StartCoords.X = CursorCoords.X;
                        StartCoords.Y = CursorCoords.Y;

                        //Shift to make a ramp instead

                        //Alt to make an NPC-only collision square
                        if (KeyboardStateCurrent.IsKeyDown(Keys.LeftAlt) == true)
                            level.CollisionObjects[level.CollisionObjects.Count - 1].IsNPCOnly = true;

                        if (KeyboardStateCurrent.IsKeyDown(Keys.LeftShift) == true)
                            level.CollisionObjects[level.CollisionObjects.Count - 1].IsTriangle = true;
                        else
                            level.CollisionObjects[level.CollisionObjects.Count - 1].IsTriangle = false;

                        IsPlacingObj = true;
                    }

                }
            }
        }

        private void UpdateShape()
        {

            if (MouseStateCurrent.LeftButton == ButtonState.Pressed)
            {
                if (IsPlacingObj == true)
                {
                    //Currently placing new collision object, update it.
                    //Update coords[3]
                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3].X = CursorCoords.X;

                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3].Y = CursorCoords.Y;

                    //Update coords[2]
                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[2].Y = CursorCoords.Y;

                    //Update coords[1] (if rectangle)
                    if (!level.CollisionObjects[level.CollisionObjects.Count - 1].IsTriangle)
                        level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1].X = CursorCoords.X;


                    level.CollisionObjects[level.CollisionObjects.Count - 1].UpdateObject();
                    level.CollisionObjects[level.CollisionObjects.Count - 1].ChangeLines = true;
                }
            }
        }

        private void FinishShape()
        {
            //Mouse released
            if (MouseStateCurrent.LeftButton == ButtonState.Released)
            {
                if (IsPlacingObj)
                {
                    level.CollisionObjects[level.CollisionObjects.Count - 1].complete = true;
                    bool Deleted = false;

                    //Mark for deletion if the created rectangle was too small
                    if (System.Math.Abs(level.CollisionObjects[level.CollisionObjects.Count - 1].Width) < 8 || System.Math.Abs(level.CollisionObjects[level.CollisionObjects.Count - 1].Height) < 8)
                    {
                        level.CollisionObjects.Remove(level.CollisionObjects[level.CollisionObjects.Count - 1]);
                        Deleted = true;
                    }

                    if (level.CollisionObjects.Count == 0)
                    {
                        IsPlacingObj = false;
                        return;
                    }
                    //Only allow triangle with a slope less than or equal to 1
                    if (level.CollisionObjects[level.CollisionObjects.Count - 1].IsTriangle)
                    {
                        float Slope = (level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1].Y - level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3].Y) / (level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1].X - level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3].X);
                        if (Math.Abs(Slope) > 1)
                        {
                            level.CollisionObjects.Remove(level.CollisionObjects[level.CollisionObjects.Count - 1]);
                            Deleted = true;
                        }
                    }

                    //Adjust box so the coords are like this:
                    //  0-----1      01\           /01
                    //  |.....|  OR  |..\   OR    /..|
                    //  |.....|      |...\       /...|
                    //  2-----3      2----3     2----3

                    //Only if its a rectangle bro
                    if (Deleted == false)
                    {
                        if (level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[0].X != level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1].X)
                        {
                            Vector2 Temp;

                            if (CursorCoords.X < StartCoords.X)
                            {

                                if (CursorCoords.Y < StartCoords.Y)
                                {
                                    //Top left
                                    //Switch [0] & [3]
                                    Temp = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[0];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[0] = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3] = Temp;

                                    //Switch [1] & [2]
                                    Temp = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1] = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[2];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[2] = Temp;
                                }
                                else
                                {
                                    //Left side

                                    //Switch [0] & [1]
                                    Temp = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[0];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[0] = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1] = Temp;

                                    //Switch [2] & [3]
                                    Temp = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[2];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[2] = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3];
                                    level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3] = Temp;

                                }
                            }
                            else if (CursorCoords.Y < StartCoords.Y)
                            {
                                //Top Right

                                //Switch [0] & [2]
                                Temp = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[0];
                                level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[0] = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[2];
                                level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[2] = Temp;

                                //Switch [1] & [3]
                                Temp = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1];
                                level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[1] = level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3];
                                level.CollisionObjects[level.CollisionObjects.Count - 1].Coords[3] = Temp;
                            }
                        }

                        //Mouse release
                        IsPlacingObj = false;

                        level.CollisionObjects[level.CollisionObjects.Count - 1].UpdateObject();
                        level.CollisionObjects[level.CollisionObjects.Count - 1].ChangeLines = true;
                    }
                    else
                        IsPlacingObj = false;
                }
            }
        }

        private void Save()
        {
            //CTRL+S to save
            if (KeyboardStateCurrent != KeyboardStatePrevious)
            {
                if (KeyboardStateCurrent.IsKeyDown(Keys.LeftControl) && KeyboardStateCurrent.IsKeyDown(Keys.S))
                {
                    //Clear old data
                    System.IO.File.WriteAllText("Content/Levels/" + Convert.ToString(level.LevelID) + ".col", "");

                    //Save new data
                    //foreach (CollisionObject collisionObj in Level.CollisionObjects)
                    for (int ecx = 0; ecx < Level.CollisionObjects.Count; ecx++)
                    {
                        string SaveData = Convert.ToString(Level.CollisionObjects[ecx].Start.X) + " " +
                              Convert.ToString(Level.CollisionObjects[ecx].Start.Y) + " " +
                              Convert.ToString(Level.CollisionObjects[ecx].Width) + " " +
                              Convert.ToString(Level.CollisionObjects[ecx].Height);

                        if (Level.CollisionObjects[ecx].IsTriangle)
                            SaveData += "T";
                        else if (!Level.CollisionObjects[ecx].IsTriangle)
                            SaveData += "R";

                        SaveData += Convert.ToString(Level.CollisionObjects[ecx].MinOffset.X) + " " +
                              Convert.ToString(Level.CollisionObjects[ecx].MinOffset.Y) + " " +
                              Convert.ToString(Level.CollisionObjects[ecx].MaxOffset.X) + " " +
                              Convert.ToString(Level.CollisionObjects[ecx].MaxOffset.Y);

                        if (Level.CollisionObjects[ecx].Moving)
                            SaveData += "M";
                        else
                            SaveData += "S";

                        SaveData += Convert.ToString(Level.CollisionObjects[ecx].VelocityX) + " " +
                              Convert.ToString(Level.CollisionObjects[ecx].VelocityY);

                        if (Level.CollisionObjects[ecx].Passable)
                            SaveData += "P";
                        else if (Level.CollisionObjects[ecx].Liquid)
                            SaveData += "L";
                        else if (Level.CollisionObjects[ecx].Damaging)
                            SaveData += "D";
                        else
                            SaveData += "O";

                        SaveData += Convert.ToString(Level.CollisionObjects[ecx].Damage);

                        if (Level.CollisionObjects[ecx].IsNPCOnly)
                            SaveData += "E";
                        else
                            SaveData += "N";

                        System.IO.File.AppendAllText("Content/Levels/" + Convert.ToString(level.LevelID) + ".col", SaveData);
                    }

                    //Clear old data
                    System.IO.File.WriteAllText("Content/Levels/" + Convert.ToString(level.LevelID) + ".woj", "");

                    //Save new data
                    //foreach (WorldObject WorldObj in Level.WorldObjects)
                    for (int ecx = 0; ecx < Level.WorldObjects.Count; ecx++)
                    {
                        string SaveData = Convert.ToString(Level.WorldObjects[ecx].ObjectID) + " ";
                        SaveData += Convert.ToString(Level.WorldObjects[ecx].Start.X) + " " +
                              Convert.ToString(Level.WorldObjects[ecx].Start.Y);

                        if (Level.WorldObjects[ecx].Moving)
                            SaveData += "M";
                        else
                            SaveData += "S";

                        SaveData += Convert.ToString(Level.WorldObjects[ecx].MinOffset.X) + " " +
                            Convert.ToString(Level.WorldObjects[ecx].MinOffset.Y) + " " +
                            Convert.ToString(Level.WorldObjects[ecx].MaxOffset.X) + " " +
                            Convert.ToString(Level.WorldObjects[ecx].MaxOffset.Y);

                        if (Level.WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Background)
                            SaveData += "B";
                        else if (Level.WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Normal)
                            SaveData += "N";
                        else
                            SaveData += "F";

                        SaveData += Convert.ToString(Level.WorldObjects[ecx].VelocityX) + " " +
                            Convert.ToString(Level.WorldObjects[ecx].VelocityY) + " ";

                        System.IO.File.AppendAllText("Content/Levels/" + Convert.ToString(level.LevelID) + ".woj", SaveData);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (level.CollisionObjects != null)
            {
                //Draw all rectangles in list
                //foreach (CollisionObject currentObj in level.CollisionObjects)
                for (int ecx = 0; ecx < level.CollisionObjects.Count; ecx++)
                    level.CollisionObjects[ecx].Draw(spriteBatch);
            }
        }
    }
}
