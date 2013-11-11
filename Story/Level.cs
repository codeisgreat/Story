using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Story
{
    class Level : IDisposable
    {
        #region Variables

        public enum LiquidType
        {
            Water,
            Lava,
        } public LiquidType CurrentLiquidType = LiquidType.Water;

        public enum Environment
        {
            Grass,
            Snow,
            Desert,
        } public Environment CurrentEnvironment = Environment.Grass;

        public int LevelID = 1;
        public Vector2 LastCheckPoint = Vector2.Zero;
        private bool Gem1Collected = false;
        private bool Gem2Collected = false;
        private bool Gem3Collected = false;

        public Random Random = new Random(666);

        // Physical structure of the level.
        public Tile[,] Tiles;
        private Texture2D[] WaterTiles;
        private Texture2D[] LavaTiles;
        private Texture2D[] Layers;
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        public int WaterLine = 0;

        // Entities in the level.
        public Player Player
        {
            get { return player; }
        }
        Player player;
        public Texture2D[] HeartTexture = new Texture2D[Player.MaxPlayerHP + 1];

        //Particlez
        public ParticleEmitter MindPowerEmitter;
        private ParticleEmitter EnvironmentEmitterBG;
        private ParticleEmitter EnvironmentEmitterFG;

        private List<Collectable> EggCollectables = new List<Collectable>();
        private List<Collectable> GemCollectables = new List<Collectable>();
        public List<Enemy> Enemies = new List<Enemy>();

        private List<Outhouse> Outhouses = new List<Outhouse>();
        private List<WaterBubbles> WaterBubbles = new List<WaterBubbles>();
        public List<CollisionObject> CollisionObjects = new List<CollisionObject>();
        public List<WorldObject> WorldObjects = new List<WorldObject>();

        public Texture2D[] ObjectTextures = new Texture2D[50];
        public String[] ObjectNames = new String[50];

        private LevelEditor LevelEditor;

        // Key locations in the level.        
        public Vector2 LevelStartPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);
        private Point Exit = InvalidPosition;

        public GamePadState LastGamePadState;
        public KeyboardState LastKeyboardState;

        public int Eggs = 0;
        public Texture2D EggUITexture;
        public Texture2D MindPowerCoverTexture;
        public Texture2D MindPowerBackTexture;
        private const int MindPowerEffectStartY = 18;
        private int MindPowerEffectEndY = 0;
        private float MindPowerEffectY = 18;

        public bool ReachedExit;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private SoundEffect ExitReachedSound;

        #endregion

        #region Loading

        public Level(IServiceProvider serviceProvider)
        {
            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "Content");

            LoadContent();

#if WINDOWS
            if (Main.DebugMode)
                LevelEditor = new LevelEditor(this);
#endif
        }

        //Load textures and whatnot
        private void LoadContent()
        {
            ObjectTextures[0] = Content.Load<Texture2D>("Objects/NullObjectB");
            ObjectNames[0] = "Null Object B";
            ObjectTextures[1] = Content.Load<Texture2D>("Objects/NullObjectN");
            ObjectNames[1] = "Null Object N";
            ObjectTextures[2] = Content.Load<Texture2D>("Objects/NullObjectF");
            ObjectNames[2] = "Null Object F";
            ObjectTextures[3] = Content.Load<Texture2D>("Objects/Water");
            ObjectNames[3] = "Water";
            ObjectTextures[4] = Content.Load<Texture2D>("Objects/WaterTop");
            ObjectNames[4] = "Water Surface";
            ObjectTextures[5] = Content.Load<Texture2D>("Objects/Bridge");
            ObjectNames[5] = "Bridge";
            ObjectTextures[6] = Content.Load<Texture2D>("Objects/SpikesB");
            ObjectNames[6] = "Spikes Bottom";
            ObjectTextures[7] = Content.Load<Texture2D>("Objects/SpikesT");
            ObjectNames[7] = "Spikes Top";
            ObjectTextures[8] = Content.Load<Texture2D>("Objects/PlatformRuins");
            ObjectNames[8] = "Platform (Ruins)";
            ObjectTextures[9] = Content.Load<Texture2D>("Objects/PillarLargeRuins");
            ObjectNames[9] = "Pillar [L] (Ruins)";
            ObjectTextures[10] = Content.Load<Texture2D>("Objects/PillarSmallRuins");
            ObjectNames[10] = "Pillar [S] (Ruins)";
            ObjectTextures[11] = Content.Load<Texture2D>("Objects/PlatformBridge");
            ObjectNames[11] = "Platform (Bridge)";
            ObjectTextures[12] = Content.Load<Texture2D>("Objects/Mine");
            ObjectNames[12] = "Ocean Mine";
            ObjectTextures[13] = Content.Load<Texture2D>("Objects/MineLarge");
            ObjectNames[13] = "Ocean Mine Large";
            ObjectTextures[14] = Content.Load<Texture2D>("Objects/SpikesSmallB");
            ObjectNames[14] = "Spikes Bottom [S]";
            ObjectTextures[15] = Content.Load<Texture2D>("Objects/SpikesSmallT");
            ObjectNames[15] = "Spikes Top [S]";
            ObjectTextures[16] = Content.Load<Texture2D>("Objects/SpikeBall");
            ObjectNames[16] = "Spiked Ball";
            ObjectTextures[17] = Content.Load<Texture2D>("Objects/CloudSmall");
            ObjectNames[17] = "Cloud Small";
            ObjectTextures[18] = Content.Load<Texture2D>("Objects/CloudLarge");
            ObjectNames[18] = "Cloud Large";
            ObjectTextures[19] = Content.Load<Texture2D>("Objects/SignGlideLeft");
            ObjectNames[19] = "Sign (Glide Left)";
            ObjectTextures[20] = Content.Load<Texture2D>("Objects/SignGlideRight");
            ObjectNames[20] = "Sign (Glide Right)";
            ObjectTextures[21] = Content.Load<Texture2D>("Objects/SignSkullLeft");
            ObjectNames[21] = "Sign (Skull Left)";
            ObjectTextures[22] = Content.Load<Texture2D>("Objects/SignSkullRight");
            ObjectNames[22] = "Sign (Skull Left)";


            WaterTiles = new Texture2D[2];
            WaterTiles[0] = Content.Load<Texture2D>("Tiles/WaterT");
            WaterTiles[1] = Content.Load<Texture2D>("Tiles/WaterM");
            LavaTiles = new Texture2D[2];
            LavaTiles[0] = Content.Load<Texture2D>("Tiles/LavaT");
            LavaTiles[1] = Content.Load<Texture2D>("Tiles/LavaM");

            EggUITexture = Content.Load<Texture2D>("Sprites/Egg");

            MindPowerBackTexture = Content.Load<Texture2D>("Sprites/Player/MindPowerBack");
            MindPowerCoverTexture = Content.Load<Texture2D>("Sprites/Player/MindPowerCover");

            MindPowerEffectEndY = MindPowerCoverTexture.Height - 34; //Arbitrary offset that works
            MindPowerEmitter = new ParticleEmitter(new Vector2(Main.BackBufferWidth - MindPowerCoverTexture.Width / 2, MindPowerEffectStartY),
                new Color(32, 0, 255, 32), new Vector2(Main.BackBufferWidth - MindPowerCoverTexture.Width + 28, MindPowerEffectStartY), new Vector2(Main.BackBufferWidth, MindPowerEffectEndY),
                new Vector2(-0.2f, 0.4f), new Vector2(0.2f, 1.40f), new Vector2(-32, 0), new Vector2(32, 0), 6000, 40, 32, 0.0f, 0.00f, 0.004f, .25f, false, 0.0f);

            MindPowerEmitter.LoadContent(Content.Load<Texture2D>("Particles/MindPowerParticle"));
            MindPowerEmitter.StartEffect();

            //Heart texture
            for (int i = 0; i < Player.MaxPlayerHP + 1; ++i)
                HeartTexture[i] = Content.Load<Texture2D>("Sprites/Player/Heart" + i);

            //Raptor health texture
            //for (int i = 0; i < Entity.MaxRaptorHP + 1; ++i)
            //    RaptorHPTexture[i] = Content.Load<Texture2D>("Sprites/Raptor/RaptorHP" + i);

            //Add the raptors
            //for (int i = 0; i < MaxRaptors; i++)
            //    Raptors.Add(new Entity(this, LevelStartPosition, EntityType.Raptor));

            // Load sounds.
            ExitReachedSound = Content.Load<SoundEffect>("Sounds/ExitReached");
        }

        //Loads some crap for level (rest of it is in class Main)
        public void LoadLevel()
        {
            //Cleanup old stuff
            WorldObjects.Clear();
            CollisionObjects.Clear();
            Enemies.Clear();
            EggCollectables.Clear();
            GemCollectables.Clear();
            Outhouses.Clear();
            WaterBubbles.Clear();

            ReachedExit = false;

            #region Read level file

            string LevelPath = String.Format("Content/Levels/{0}.txt", LevelID);
            string ConfigPath = String.Format("Content/Levels/{0}.cfg", LevelID);

            string ConfigData = "";
            //Check if level is snow
            if (File.Exists(ConfigPath))
            {
                ConfigData = System.IO.File.ReadAllText(ConfigPath);
                int index;
                index = ConfigData.IndexOf("Waterline=", 0) + ("Waterline=").Length;
                WaterLine = Convert.ToInt32(ConfigData.Substring(index, 3));

                index = ConfigData.IndexOf("Environment=", 0) + 1 + ConfigData.LastIndexOf("Environment=", 0) + ("Environment=").Length;
                string Env = ConfigData.Substring(index, 1);
                if (Env == "S")
                    CurrentEnvironment = Level.Environment.Snow;
                else if (Env == "D")
                    CurrentEnvironment = Level.Environment.Desert;
                else // if (Env == "G")
                    CurrentEnvironment = Level.Environment.Grass;

                index = ConfigData.IndexOf("Liquid=", 0) + 1 + ConfigData.LastIndexOf("Liquid=", 0) + ("Liquid=").Length;
                string Liq = ConfigData.Substring(index, 1);
                if (Liq == "L")
                    CurrentLiquidType = Level.LiquidType.Lava;
                //else if (Liq == "A")
                //environment = Level.LiquidType.Acid;
                else // if (Liq == "W")
                    CurrentLiquidType = Level.LiquidType.Water;

            }

            string collisionDataPath;
            string collisionData;

            collisionDataPath = "Content/Levels/" + Convert.ToString(LevelID) + ".col";
            if (File.Exists(collisionDataPath))
            {
                //Load a shape with some default values to change later
                CollisionObject loadedShape = new CollisionObject(0, 0, this, false);

                int value;
                int width = 0;

                int alt = 1;
                int first = 0;

                //Read in all collision data for the map
                collisionData = System.IO.File.ReadAllText(collisionDataPath);
                for (int last = 1; last < collisionData.Length + 1; last++)
                {
                    //Check if the last digit of those being checked is an end marker
                    string EndMarker = collisionData.Substring(last - 1, 1);
                    if (EndMarker == " " || EndMarker == "R" || EndMarker == "T" || EndMarker == "E" || EndMarker == "O" ||
                        EndMarker == "L" || EndMarker == "S" || EndMarker == "M" || EndMarker == "N" || EndMarker == "P" || EndMarker == "D")
                    {
                        //Do whatever the marker specifies
                        if (EndMarker == "T")
                            loadedShape.IsTriangle = true;
                        else if (EndMarker == "R")
                            loadedShape.IsTriangle = false;
                        else if (EndMarker == "E")
                            loadedShape.IsNPCOnly = true;
                        else if (EndMarker == "N")
                            loadedShape.IsNPCOnly = false;
                        else if (EndMarker == "M")
                            loadedShape.Moving = true;
                        else if (EndMarker == "S")
                            loadedShape.Moving = false;
                        else if (EndMarker == "D")
                            loadedShape.Damaging = true;
                        else if (EndMarker == "L")
                            loadedShape.Liquid = true;
                        else if (EndMarker == "P")
                            loadedShape.Passable = true;
                        //else EndMarker is just a space seperating integer values


                        string strValue = collisionData.Substring(first, (last - first - 1));
                        value = Convert.ToInt32(strValue);

                        //No more digits, add value
                        switch (alt)
                        {
                            case 1:
                                //Value = X
                                //set X coord[0/2]
                                loadedShape.Coords[0].X = value;
                                loadedShape.Coords[2].X = value;
                                loadedShape.Start.X = value;
                                break;
                            case 2:
                                //Value = Y
                                //set Y coord [0/1]
                                loadedShape.Coords[0].Y = value;
                                loadedShape.Coords[1].Y = value;
                                loadedShape.Start.Y = value;
                                break;
                            case 3:
                                //Value = width
                                //set X coord [3]
                                loadedShape.Coords[3].X = loadedShape.Coords[0].X + value;
                                width = value;

                                break;
                            case 4:
                                //Value = height
                                //set Y coord [3]
                                loadedShape.Coords[3].Y = loadedShape.Coords[0].Y + value;
                                loadedShape.Coords[2].Y = loadedShape.Coords[0].Y + value;

                                //Finish coord [1]
                                if (!loadedShape.IsTriangle)
                                    loadedShape.Coords[1].X = loadedShape.Coords[0].X + width;
                                else
                                    loadedShape.Coords[1].X = loadedShape.Coords[0].X;
                                break;
                            //Movement offsets
                            case 5:
                                loadedShape.MinOffset.X = value;
                                break;
                            case 6:
                                loadedShape.MinOffset.Y = value;
                                break;
                            case 7:
                                loadedShape.MaxOffset.X = value;
                                break;
                            case 8:
                                loadedShape.MaxOffset.Y = value;
                                break;
                            //Velocities
                            case 9:
                                loadedShape.VelocityX = value;
                                break;
                            case 10:
                                loadedShape.VelocityY = value;
                                break;
                            //Damage
                            case 11:
                                loadedShape.Damage = value;

                                //Add object & update it.
                                loadedShape.UpdateObject();
                                loadedShape.ChangeLines = true;
                                loadedShape.complete = true;
                                CollisionObjects.Add(loadedShape);

                                //If the loaded collision box is underwater make the tiles beneath it marked as such
                                //Note: Damaging box with 0 damage is what indicates underwater.
                                /*
                                if (loadedShape.Damaging && loadedShape.Damage == 0)
                                {
                                    int x = (int)(loadedShape.Coords[0].X / Tile.Width);
                                    int y = (int)(loadedShape.Coords[0].Y / Tile.Height);
                                    int w = (int)(loadedShape.Coords[3].X / Tile.Width);
                                    int h = (int)(loadedShape.Coords[3].Y / Tile.Height);

                                    for (int ecx = x; ecx <= w; ecx++)
                                        for (int edx = y; edx <= h; edx++)
                                        {
                                            level.Tiles[ecx, edx].UnderWater = true;
                                        }
                                }
                                */
                                //Reset variables that need to be
                                loadedShape = new CollisionObject(0, 0, this, false);
                                alt = 0;
                                break;
                        }
                        alt++;
                        first = last;
                    }
                    else { } //Still need to read in more digits


                }
            }//End load collision data


            string ObjectDataPath;
            string ObjectData;

            ObjectDataPath = "Content/Levels/" + Convert.ToString(LevelID) + ".woj";
            if (File.Exists(ObjectDataPath))
            {
                //Load a shape with some default values to change later
                WorldObject LoadedObject = new WorldObject(2, "Null(loading)", null, new Vector2(0, 0));

                int value;

                int alt = 1;
                int first = 0;

                //Read in all collision data for the map
                ObjectData = System.IO.File.ReadAllText(ObjectDataPath);
                for (int last = 1; last < ObjectData.Length + 1; last++)
                {

                    if (ObjectData.Substring(last - 1, 1) == " " || ObjectData.Substring(last - 1, 1) == "B" || ObjectData.Substring(last - 1, 1) == "N" ||
                        ObjectData.Substring(last - 1, 1) == "F" || ObjectData.Substring(last - 1, 1) == "M" || ObjectData.Substring(last - 1, 1) == "S")
                    {
                        if (ObjectData.Substring(last - 1, 1) == "B")
                            LoadedObject.layerDepth = WorldObject.LayerDepth.Background;
                        else if (ObjectData.Substring(last - 1, 1) == "N")
                            LoadedObject.layerDepth = WorldObject.LayerDepth.Normal;
                        else if (ObjectData.Substring(last - 1, 1) == "F")
                            LoadedObject.layerDepth = WorldObject.LayerDepth.Foreground;
                        else if (ObjectData.Substring(last - 1, 1) == "M")
                            LoadedObject.Moving = true;
                        else if (ObjectData.Substring(last - 1, 1) == "S")
                            LoadedObject.Moving = false;

                        string strValue = ObjectData.Substring(first, (last - first - 1));
                        value = Convert.ToInt32(strValue);
                        //alt++;
                        //No more digits, add value
                        switch (alt)
                        {
                            case 1:
                                //Value = ObjectID
                                LoadedObject.ObjectID = value;
                                //Update texture & name
                                LoadedObject.TextureName = ObjectNames[LoadedObject.ObjectID];
                                LoadedObject.ObjectTexture = ObjectTextures[LoadedObject.ObjectID];
                                break;
                            case 2:
                                //Value = X
                                LoadedObject.Position.X = value;
                                LoadedObject.Start.X = value;
                                break;
                            case 3:
                                //Value = Y
                                LoadedObject.Position.Y = value;
                                LoadedObject.Start.Y = value;
                                break;
                            case 4:
                                LoadedObject.MinOffset.X = value;
                                break;
                            case 5:
                                LoadedObject.MinOffset.Y = value;
                                break;
                            case 6:
                                LoadedObject.MaxOffset.X = value;
                                break;
                            case 7:
                                LoadedObject.MaxOffset.Y = value;

                                /*
                                //Add object
                                WorldObjects.Add(LoadedObject);
                                //Reset
                                LoadedObject = new WorldObject(0, "Null(loading)", null, new Vector2(0, 0));
                                alt = 0;
                                */
                                break;
                            case 8:
                                LoadedObject.VelocityX = value;
                                break;
                            case 9:
                                LoadedObject.VelocityY = value;
                                //Add object
                                WorldObjects.Add(LoadedObject);
                                //Reset
                                LoadedObject = new WorldObject(0, "Null(loading)", null, new Vector2(0, 0));
                                alt = 0;
                                break;
                        }
                        alt++;
                        first = last;
                    }
                    else { } //Still need to read in more digits
                }
            }//End load object data

            #endregion

            LoadTiles(LevelPath);

            //for (int ecx = 0; ecx < MaxRaptors; ecx++)
            //    Raptors.Add(new Entity(this, LevelStartPosition, EntityType.Raptor));

            LastCheckPoint = LevelStartPosition;

            //Load environment specific effects
            LoadEnvironmentEffects();

            // Load background layer textures.
            Layers = new Texture2D[3];
            switch (CurrentEnvironment)
            {
                case Environment.Grass:

                    for (int i = 0; i < Layers.Length; ++i)
                    {
                        // Choose a random segment if each background layer for level variety.
                        Layers[i] = Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + Random.Next(3));
                    }
                    break;
                case Environment.Snow:
                    Layers[0] = Content.Load<Texture2D>("Backgrounds/SnowBG");
                    Layers[1] = Content.Load<Texture2D>("Backgrounds/SnowBG");
                    Layers[2] = Content.Load<Texture2D>("Backgrounds/SnowBG");
                    break;
                case Environment.Desert:
                    Layers[0] = Content.Load<Texture2D>("Backgrounds/DesertBG");
                    Layers[1] = Content.Load<Texture2D>("Backgrounds/DesertBG");
                    Layers[2] = Content.Load<Texture2D>("Backgrounds/DesertBG");
                    break;
            }
        }

        private void LoadTiles(string Path)
        {
            // Load the level and ensure all of the lines are the same length.
            int Width;
            List<string> Lines = new List<string>();
            using (StreamReader reader = new StreamReader(Path))
            {
                string Line = reader.ReadLine();
                Width = Line.Length;
                while (Line != null)
                {
                    Lines.Add(Line);
                    if (Line.Length != Width)
                        throw new Exception(String.Format("Line {0} is a different length than the others.", Lines.Count));
                    Line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            Tiles = new Tile[Width, Lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = Lines[y][x];
                    Tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");

        }

        private Tile LoadTile(char TileType, int x, int y)
        {
            switch (TileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, false);

                // Also a blank space when not in test mode
                case 'M':
                    if (!Main.DebugMode)
                        return new Tile(null, false);
                    else
                        return LoadTile("Invisible", false);

                // Player 1 start point
                case '0':
                    return LoadStartTile(x, y);

                // Outhouse
                case 'O':
                    Outhouses.Add(new Outhouse(this, new Vector2(x * Tile.Width, y * Tile.Height)));
                    return new Tile(null, false);

                // Outhouse
                case '^':
                    WaterBubbles.Add(new WaterBubbles(this, new Vector2(x * Tile.Width, y * Tile.Height)));
                    return new Tile(null, false);

                // Exit
                case 'X':
                    return LoadExitTile(x, y);

                // Egg
                case '*':
                    return LoadEggTile(x, y);

                // Gem1
                case '@':
                    return LoadGemTile(x, y, 1);
                case '%':
                    return LoadGemTile(x, y, 2);
                case '&':
                    return LoadGemTile(x, y, 3);

                // Various enemies
                case 'V':
                    return LoadEnemyTile(x, y, Story.Enemy.EnemyType.Velociraptor);
                case 'P':
                    return LoadEnemyTile(x, y, Story.Enemy.EnemyType.Pterodactyl);
                case 'D':
                    return LoadEnemyTile(x, y, Story.Enemy.EnemyType.Dunkleosteus);

                //Standard tiles
                case '#':
                    return LoadTile("BridgeL", false);
                case '~':
                    return LoadTile("BridgeM", false);
                case '+':
                    return LoadTile("BridgeR", false);

                case '9':
                    return LoadTile("TLRuins", false);
                case '8':
                    return LoadTile("TMRuins", false);
                case '7':
                    return LoadTile("TRRuins", false);
                case '6':
                    return LoadTile("MLRuins", false);
                case '5':
                    return LoadTile("MMRuins", false);
                case '4':
                    return LoadTile("MRRuins", false);
                case '3':
                    return LoadTile("BLRuins", false);
                case '2':
                    return LoadTile("BMRuins", false);
                case '1':
                    return LoadTile("BRRuins", false);
                case 'q':
                    return LoadTile("RR1Ruins", false);
                case 'w':
                    return LoadTile("RR2Ruins", false);
                case 'y':
                    return LoadTile("RR3Ruins", false);
                case 'e':
                    return LoadTile("RL1Ruins", false);
                case 'r':
                    return LoadTile("RL2Ruins", false);
                case 't':
                    return LoadTile("RL3Ruins", false);

                //Terrain tiles
                case '(':
                    return LoadSeasonalTile("TL");
                case '-':
                    return LoadSeasonalTile("TM");
                case ')':
                    return LoadSeasonalTile("TR");

                case '{':
                    return LoadSeasonalTile("ML");
                case ',':
                    return LoadSeasonalTile("MM");
                case '}':
                    return LoadSeasonalTile("MR");

                case '[':
                    return LoadSeasonalTile("BL");
                case '_':
                    return LoadSeasonalTile("BM");
                case ']':
                    return LoadSeasonalTile("BR");

                case '>':
                    return LoadSeasonalTile("RR1");
                case '/':
                    return LoadSeasonalTile("RR2");
                case ':':
                    return LoadSeasonalTile("RR3");

                case '<':
                    return LoadSeasonalTile("RL1");

                case (char)92: // '\' -- have to do it this way due to weird backslash string constants
                    return LoadSeasonalTile("RL2");
                case ';':
                    return LoadSeasonalTile("RL3");

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", TileType, x, y));
            }
        }

        //Tile that changes based on weather
        private Tile LoadSeasonalTile(string name)
        {
            bool Transparent = false;
            if (CurrentEnvironment == Environment.Snow)
                Transparent = true;
            return new Tile(Content.Load<Texture2D>("Tiles/" + CurrentEnvironment.ToString() + "/" + name + CurrentEnvironment.ToString()), Transparent);
        }

        //Misc tile loading
        private Tile LoadTile(string Name, bool Transparent)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + Name), Transparent);
        }

        private Tile LoadStartTile(int x, int y)
        {
            LevelStartPosition = GetBottomCenter(GetBounds(x, y));
            player = new Player(this, LevelStartPosition);

            return new Tile(null, false);
        }

        private Tile LoadExitTile(int x, int y)
        {
            Exit = GetBounds(x, y).Center;

            return new Tile(Content.Load<Texture2D>("Tiles/Exit"), false);
        }

        private Tile LoadEnemyTile(int x, int y, Story.Enemy.EnemyType EnemyType)
        {
            Vector2 position = GetBottomCenter(GetBounds(x, y));

            Enemies.Add(new Enemy(this, position, EnemyType));

            return new Tile(null, false);
        }

        private void LoadEnvironmentEffects()
        {
            switch (CurrentEnvironment)
            {
                case Environment.Snow:
                    //Snow
                    EnvironmentEmitterBG = new ParticleEmitter(new Vector2(0, 2),
           new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, Main.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
           new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, 0), 2500, 2, 32, 0.0005f, 0.0f, 0.006f, 0.1f, false, 0.0f);
                    EnvironmentEmitterBG.LoadContent(Content.Load<Texture2D>("Particles/SnowflakeParticle"));

                    EnvironmentEmitterFG = new ParticleEmitter(new Vector2(0, 2),
                        new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, Main.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
                        new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, 0), 2500, 2, 32, 0.0005f, 0.0f, 0.006f, 0.2f, false, 0.0f);
                    EnvironmentEmitterFG.LoadContent(Content.Load<Texture2D>("Particles/SnowflakeParticle"));
                    break;
                case Environment.Grass:
                    //Rain
                    EnvironmentEmitterBG = new ParticleEmitter(new Vector2(0, 2),
           new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, Main.BackBufferHeight + 32), new Vector2(-6.0f, 3.0f), new Vector2(-1.0f, 1.0f),
           new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, 0), 2500, 8, 32, 0f, 0.0f, 0.006f, 0.1f, false, 0.0f);
                    EnvironmentEmitterBG.LoadContent(Content.Load<Texture2D>("Particles/RainParticle"));

                    EnvironmentEmitterFG = new ParticleEmitter(new Vector2(0, 2),
                        new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, Main.BackBufferHeight + 32), new Vector2(-6.0f, 12.0f), new Vector2(-1.0f, 1.0f),
                        new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, 0), 2500, 8, 32, 0f, 0.0f, 0.006f, 0.2f, false, 0.0f);
                    EnvironmentEmitterFG.LoadContent(Content.Load<Texture2D>("Particles/RainParticle"));
                    break;
                case Environment.Desert:
                    //Sand
                    EnvironmentEmitterBG = new ParticleEmitter(new Vector2(0, 2),
           new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, Main.BackBufferHeight + 32), new Vector2(-6.0f, 4.0f), new Vector2(-1.0f, 1.0f),
           new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, 0), 2500, 18, 32, 0f, 0.0f, 0.006f, 0.05f, false, 0.0f);
                    EnvironmentEmitterBG.LoadContent(Content.Load<Texture2D>("Particles/SandParticle"));

                    EnvironmentEmitterFG = new ParticleEmitter(new Vector2(0, 2),
                        new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, Main.BackBufferHeight + 32), new Vector2(-6.0f, 4.0f), new Vector2(-1.0f, 1.0f),
                        new Vector2(0, 0), new Vector2(Main.BackBufferWidth + 256, 0), 2500, 18, 32, 0f, 0.0f, 0.006f, 0.05f, false, 0.0f);
                    EnvironmentEmitterFG.LoadContent(Content.Load<Texture2D>("Particles/SandParticle"));
                    break;
            }
            EnvironmentEmitterBG.StartEffect();
            EnvironmentEmitterFG.StartEffect();
        }

        private Vector2 GetBottomCenter(Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }

        private Tile LoadEggTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            EggCollectables.Add(new Collectable(this, new Vector2(position.X, position.Y), 0));

            return new Tile(null, false);
        }

        private Tile LoadGemTile(int x, int y, int GemID)
        {
            Point position = GetBounds(x, y).Center;
            GemCollectables.Add(new Collectable(this, new Vector2(position.X, position.Y), GemID));

            return new Tile(null, false);
        }

        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return Tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return Tiles.GetLength(1); }
        }

        #endregion

        #region Update / Methods

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime GameTime)
        {
            // Only apply physics while level finished (and NOTHING else)
            if (ReachedExit)
                Player.ApplyPhysics(GameTime, true);
            else // Update level
            {
                GamePadState GamePadState = GamePad.GetState(PlayerIndex.One);
                KeyboardState keyboardState = Keyboard.GetState();

                //Set the height of the particle emitter to that of the percentage of the players mindpower out of their max
                MindPowerEffectY = Player.MindPower / Player.MindPowerMax * (MindPowerEffectStartY - MindPowerEffectEndY) + MindPowerEffectEndY;
                MindPowerEmitter.StartPosition.Y = MindPowerEffectY;
                EnvironmentEmitterBG.Update(GameTime);
                EnvironmentEmitterFG.Update(GameTime);

                //Update all objects and entities necessary
                UpdateWaterBubbles(GameTime);

                Player.UpdatePlayer(GameTime);
                UpdateObjectMovement(GameTime);
                UpdateOuthouses(GameTime);
                UpdateEnemies(GameTime);
                UpdateCollectables(GameTime);

                //And some more player stuff (level exit reached)
                if (Player.IsAlive &&
                    Player.IsOnGround &&
                    Player.BoundingRectangle.Contains(Exit))
                {
                    OnExitReached();
                }

                LastGamePadState = GamePadState;
                LastKeyboardState = keyboardState;
            }

            //Update Mind Power UI particle system
            MindPowerEmitter.Update(GameTime);

#if WINDOWS
            if (Main.DebugMode)
                LevelEditor.Update(GameTime);
#endif
        }

        private void UpdateObjectMovement(GameTime GameTime)
        {
            float Elapsed = GameTime.ElapsedGameTime.Milliseconds;

            for (int ecx = 0; ecx < CollisionObjects.Count; ecx++)
            {
                if (CollisionObjects[ecx].Moving)
                {
                    //X
                    if (CollisionObjects[ecx].VelocityX != 0)
                    {
                        CollisionObjects[ecx].Movement.X = Elapsed * (CollisionObjects[ecx].VelocityX / 1000) * CollisionObjects[ecx].DirectionX;

                        if (CollisionObjects[ecx].line != null)
                            CollisionObjects[ecx].line.Position.X += CollisionObjects[ecx].Movement.X;

                        CollisionObjects[ecx].Coords[0].X += CollisionObjects[ecx].Movement.X;
                        CollisionObjects[ecx].Coords[1].X += CollisionObjects[ecx].Movement.X;
                        CollisionObjects[ecx].Coords[2].X += CollisionObjects[ecx].Movement.X;
                        CollisionObjects[ecx].Coords[3].X += CollisionObjects[ecx].Movement.X;
                        CollisionObjects[ecx].X += CollisionObjects[ecx].Movement.X;

                        if (CollisionObjects[ecx].X - Main.BackBufferWidth / 2 > CollisionObjects[ecx].Start.X + CollisionObjects[ecx].MaxOffset.X ||
                           CollisionObjects[ecx].X - Main.BackBufferWidth / 2 < CollisionObjects[ecx].Start.X + CollisionObjects[ecx].MinOffset.X)
                            CollisionObjects[ecx].DirectionX *= -1;
                    }
                    //Y
                    if (CollisionObjects[ecx].VelocityY != 0)
                    {
                        CollisionObjects[ecx].Movement.Y = Elapsed * (CollisionObjects[ecx].VelocityY / 1000) * CollisionObjects[ecx].DirectionY;

                        if (CollisionObjects[ecx].line != null)
                            CollisionObjects[ecx].line.Position.Y += CollisionObjects[ecx].Movement.Y;

                        CollisionObjects[ecx].Coords[0].Y += CollisionObjects[ecx].Movement.Y;
                        CollisionObjects[ecx].Coords[1].Y += CollisionObjects[ecx].Movement.Y;
                        CollisionObjects[ecx].Coords[2].Y += CollisionObjects[ecx].Movement.Y;
                        CollisionObjects[ecx].Coords[3].Y += CollisionObjects[ecx].Movement.Y;
                        CollisionObjects[ecx].Y += CollisionObjects[ecx].Movement.Y;

                        if (CollisionObjects[ecx].Y > CollisionObjects[ecx].Start.Y + CollisionObjects[ecx].MaxOffset.Y ||
                            CollisionObjects[ecx].Y < CollisionObjects[ecx].Start.Y + CollisionObjects[ecx].MinOffset.Y)
                            CollisionObjects[ecx].DirectionY *= -1;
                    }
                }
            }

            for (int ecx = 0; ecx < WorldObjects.Count; ecx++)
            {
                if (WorldObjects[ecx].Moving)
                {
                    //X
                    if (WorldObjects[ecx].VelocityX != 0)
                    {
                        WorldObjects[ecx].Movement.X = Elapsed * (WorldObjects[ecx].VelocityX / 1000) * WorldObjects[ecx].DirectionX;

                        WorldObjects[ecx].Position.X += WorldObjects[ecx].Movement.X;

                        if (WorldObjects[ecx].Position.X > WorldObjects[ecx].Start.X + WorldObjects[ecx].MaxOffset.X ||
                           WorldObjects[ecx].Position.X < WorldObjects[ecx].Start.X + WorldObjects[ecx].MinOffset.X)
                            WorldObjects[ecx].DirectionX *= -1;
                    }
                    //Y
                    if (WorldObjects[ecx].VelocityY != 0)
                    {
                        WorldObjects[ecx].Movement.Y = Elapsed * (WorldObjects[ecx].VelocityY / 1000) * WorldObjects[ecx].DirectionY;

                        WorldObjects[ecx].Position.Y += WorldObjects[ecx].Movement.Y;

                        if (WorldObjects[ecx].Position.Y > WorldObjects[ecx].Start.Y + WorldObjects[ecx].MaxOffset.Y ||
                            WorldObjects[ecx].Position.Y < WorldObjects[ecx].Start.Y + WorldObjects[ecx].MinOffset.Y)
                            WorldObjects[ecx].DirectionY *= -1;
                    }
                }
            }
        }

        private void UpdateWaterBubbles(GameTime GameTime)
        {
            for (int ecx = 0; ecx < WaterBubbles.Count; ecx++)
            {
                WaterBubbles[ecx].Update(GameTime);
                if (player.BoundingRectangle.Intersects(WaterBubbles[ecx].BoundingRectangle))
                    player.BreatheUnderWater();
            }
        }

        private void UpdateOuthouses(GameTime GameTime)
        {
            // foreach (Outhouse outhouse in Outhouses)
            for (int ecx = 0; ecx < Outhouses.Count; ecx++)
            {
                //Test if touching raptor spawner
                if (player.BoundingRectangle.Intersects(Outhouses[ecx].BoundingRectangle))
                {
                    //Only go through the code for when the outhouse is just opened -- not continously
                    if (!Outhouses[ecx].isOpen)
                    {
                        //open outouse
                        Outhouses[ecx].OutHouseOpenClose(true);
                        player.Health = Player.MaxPlayerHP;
                        player.MindPower = Player.MindPowerMax;
                        //Set new check point
                        LastCheckPoint = new Vector2(Outhouses[ecx].Position.X +
                            Outhouses[ecx].BoundingRectangle.Width / 2 + player.BoundingRectangle.Width / 2,
                            Outhouses[ecx].Position.Y + Outhouses[ecx].BoundingRectangle.Height / 2);
                    }
                }
                else //close outhouse
                    Outhouses[ecx].OutHouseOpenClose(false);
            }
        }

        /// <summary>
        /// Animates each gem and checks to allows the player to collect them.
        /// </summary>
        private void UpdateCollectables(GameTime GameTime)
        {
            for (int ecx = 0; ecx < EggCollectables.Count; ++ecx)
            {
                EggCollectables[ecx].Update(GameTime);

                if (!EggCollectables[ecx].Collected)
                    if (EggCollectables[ecx].BoundingRectangle.Intersects(Player.BoundingRectangle))
                        ItemCollected(EggCollectables[ecx]);
            }

            for (int ecx = 0; ecx < GemCollectables.Count; ++ecx)
            {
                Collectable Collectable = GemCollectables[ecx];

                Collectable.Update(GameTime);

                if (Collectable.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    GemCollectables.RemoveAt(ecx--);
                    ItemCollected(Collectable);
                }
            }
        }

        private void UpdateEnemies(GameTime GameTime)
        {
            //foreach (Entity enemy in Enemies)
            for (int ecx = 0; ecx < Enemies.Count; ecx++)
            {
                //Still update when dead for death effect and such
                Enemies[ecx].UpdateEnemy(GameTime);

                if (Enemies[ecx].IsAlive)
                {
                    if (Enemies[ecx].BoundingRectangle.Intersects(Player.BoundingRectangle) && !Main.GodMode)
                        player.GetHit(1);
                }
            }

        }

        private void ItemCollected(Collectable CollectedObj)
        {
            if (CollectedObj.ObjectID == 0)
                Eggs += Collectable.PointValue;
            else if (CollectedObj.ObjectID == 1)
                Gem1Collected = true;
            else if (CollectedObj.ObjectID == 2)
                Gem2Collected = true;
            else if (CollectedObj.ObjectID == 3)
                Gem3Collected = true;

            CollectedObj.OnCollected();
        }

        private void OnExitReached()
        {
            Player.OnReachedExit();
            if (!Main.Muted)
                ExitReachedSound.Play();
            ReachedExit = true;
        }

        #endregion

        #region Draw

        public bool IsOnScreen(Vector2 Position, int Width, int Height)
        {
            if (Position.X + Width > 0 && Position.Y + Height > 0 &&
                Position.X - Width < Main.BackBufferWidth && Position.Y - Height < Main.BackBufferHeight)
                return true;
            return false;
        }

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            for (int ecx = 0; ecx <= EntityLayer; ecx++)
                SpriteBatch.Draw(Layers[ecx], Vector2.Zero, Color.White);

            EnvironmentEmitterBG.Draw(GameTime, SpriteBatch, Vector2.Zero);

            // foreach (WorldObject WorldObj in WorldObjects)
            for (int ecx = 0; ecx < WorldObjects.Count; ecx++)
                if (WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Background)
                    WorldObjects[ecx].Draw(SpriteBatch, Player.Position, Player.HoverDistance);

            DrawTiles(SpriteBatch);

#if WINDOWS
            if (Main.DebugMode)
                LevelEditor.Draw(SpriteBatch);
#endif

            //foreach (WorldObject WorldObj in WorldObjects)
            for (int ecx = 0; ecx < WorldObjects.Count; ecx++)
                if (WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Normal)
                    WorldObjects[ecx].Draw(SpriteBatch, Player.Position, Player.HoverDistance);

            //foreach (Outhouse outhouse in Outhouses)
            for (int ecx = 0; ecx < Outhouses.Count; ecx++)
                Outhouses[ecx].Draw(GameTime, SpriteBatch);

            //foreach (Collectable eggObj in EggObjects)
            for (int ecx = 0; ecx < EggCollectables.Count; ecx++)
                if (!EggCollectables[ecx].Collected)
                    EggCollectables[ecx].Draw(GameTime, SpriteBatch);

            //foreach (Collectable gemObj in GemCollectables)
            for (int ecx = 0; ecx < GemCollectables.Count; ecx++)
                GemCollectables[ecx].Draw(GameTime, SpriteBatch);

            Player.DrawPlayer(GameTime, SpriteBatch);

            //foreach (Entity enemy in Enemies)
            for (int ecx = 0; ecx < Enemies.Count; ecx++)
                Enemies[ecx].Draw(GameTime, SpriteBatch);


            //foreach (WaterBubbles waterBubbles in Outhouses)
            for (int ecx = 0; ecx < WaterBubbles.Count; ecx++)
                WaterBubbles[ecx].Draw(GameTime, SpriteBatch);

            for (int ecx = EntityLayer + 1; ecx < Layers.Length; ecx++)
                SpriteBatch.Draw(Layers[ecx], Vector2.Zero, Color.White);

            //foreach (WorldObject WorldObj in WorldObjects)
            for (int ecx = 0; ecx < WorldObjects.Count; ecx++)
                if (WorldObjects[ecx].layerDepth == WorldObject.LayerDepth.Foreground)
                    WorldObjects[ecx].Draw(SpriteBatch, Player.Position, Player.HoverDistance);

            EnvironmentEmitterFG.Draw(GameTime, SpriteBatch, Vector2.Zero);

            DrawWater(SpriteBatch);
        }

        int StartX, StartY, EndX, EndY;
        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            StartX = (int)((player.Position.X - Main.BackBufferWidth / 2) / Tile.Width);
            StartY = (int)((player.Position.Y - player.BoundingRectangle.Height - Main.BackBufferHeight / Tile.VerticalScale) / Tile.Height);
            EndX = (int)((player.Position.X + player.BoundingRectangle.Width + Main.BackBufferWidth / 2) / Tile.Width);
            EndY = (int)((player.Position.Y + Main.BackBufferHeight / Tile.VerticalScale) / Tile.Height);

            int StartTileX = StartX;
            int EndTileX = EndX;
            int EndTileY = EndY;

            //Correct bounds
            if (StartX < 0)
                StartTileX = 0;
            if (StartY < 0)
                StartY = 0;
            if (StartX > Width)
                StartTileX = Width;
            if (StartY > Height)
                StartY = Height;
            if (EndX > Width)
                EndTileX = Width;
            if (EndY > Height)
                EndTileY = Height;

            // For each tile position
            for (int y = StartY; y < EndTileY; y++)
            {
                for (int x = StartTileX; x < EndTileX; x++)
                {
                    // If there is a visible tile in that position
                    // Draw it in screen space.

                    Texture2D texture = Tiles[x, y].Texture;
                    if (texture != null)
                    {
                        Vector2 position = new Vector2(x - ((player.Position.X - Main.BackBufferWidth / 2) / Tile.Size.X),
                       y - ((player.Position.Y - player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale) / Tile.Size.Y)) * Tile.Size;
                        //int Opacity = 255;
                        //if (Tiles[x, y].Transparent)
                        //    Opacity -= 48;
                        spriteBatch.Draw(texture, position, Color.FromNonPremultiplied(255, 255, 255, 255));
                    }
                }
            }
        }

        //Draws water at the given water line
        private void DrawWater(SpriteBatch SpriteBatch)
        {
            // StartX, EndX, StartY, EndY are computed in DrawTiles(), and they wont change between then & now so we can reuse them
            for (int y = StartY; y < EndY; y++)
            {
                for (int x = StartX - 1; x < EndX; x++)
                {
                    Vector2 position = new Vector2(x - ((player.Position.X - Main.BackBufferWidth / 2) / Tile.Size.X),
                    y - ((player.Position.Y - player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale) / Tile.Size.Y)) * Tile.Size;

                    switch (CurrentLiquidType)
                    {
                        case LiquidType.Water:
                            if (y == WaterLine)
                                SpriteBatch.Draw(WaterTiles[0], position, Color.FromNonPremultiplied(255, 255, 255, 128));
                            else if (y > WaterLine)
                                SpriteBatch.Draw(WaterTiles[1], position, Color.FromNonPremultiplied(255, 255, 255, 128));
                            break;
                        case LiquidType.Lava:
                            if (y == WaterLine)
                                SpriteBatch.Draw(LavaTiles[0], position, Color.FromNonPremultiplied(255, 255, 255, 196));
                            else if (y > WaterLine)
                                SpriteBatch.Draw(LavaTiles[1], position, Color.FromNonPremultiplied(255, 255, 255, 196));
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
