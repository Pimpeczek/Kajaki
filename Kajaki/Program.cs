using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Drawing;
using Pastel;




namespace Kajaki
{
    class Program
    {
        static int someCounter = 0;

        static Logo l;
        static void Main(string[] args)
        {
            Renderer.AsyncFrameLenght = 16;
            Renderer.WindowSize = new Int2(160, 50);
            Renderer.DebugMode = true;
            /*
            for(int i = 0; i < 711; i++)
            {
                Console.Write($"{$"{i*16}".PadLeft(5, '0')}: ");
                for(int j = 0; j < 16; j++)
                {
                    Console.Write(((Char)(i*16+j)) + (j < 15?"  |  ":"\n"));
                }
            }
            Console.ReadKey(true);
            */
            Int2 menuSize = new Int2(40, 10);

            l = new Logo();
            l.Text = "______       _   _   _           _     _           ";
            l.Text += "\n| ___ \\     | | | | | |         | |   (_)          ";
            l.Text += "\n| |_/ / __ _| |_| |_| | ___  ___| |__  _ _ __  ___ ";
            l.Text += "\n| ___ \\/ _` | __| __| |/ _ \\/ __| '_ \\| | '_ \\/ __|";
            l.Text += "\n| |_/ / (_| | |_| |_| |  __/\\__ \\ | | | | |_) \\__ \\";
            l.Text += "\n\\____/ \\__,_|\\__|\\__|_|\\___||___/_| |_|_| .__/|___/";
            l.Text += "\n                                        | |        ";
            l.Text += "\n                                        |_|        ";
            l.Position = new Int2((Renderer.WindowSize.x - l.Size.x) / 2, 2);
            l.Draw();


            Menu mainMenu = new Menu(new Int2((Renderer.WindowSize.x - menuSize.x) /2, (Renderer.WindowSize.y - menuSize.y) / 2), menuSize, "Main menu", Boxes.BoxType.doubled);
            mainMenu.AddControll(new LineSeparatorControll("sep0", "sep0"));
            mainMenu.AddControll(new MenuControll("Host a game", "host"));
            mainMenu.GetControll("host").AddAction(HostAGame);
            mainMenu.AddControll(new MenuControll("Ships", "ships"));
            mainMenu.AddControll(new MenuControll("Ship designer", "designer"));
            mainMenu.AddControll(new LineSeparatorControll("sep1", "sep1"));
            mainMenu.AddControll(new MenuControll("Exit", "exit"));

            mainMenu.GetControll("exit").AddAction(mainMenu.Exit);

            mainMenu.AddControll(new LineSeparatorControll("sep_fin", "sep_fin"));

            mainMenu.WaitForInput();

            Renderer.AsyncMode = false;

        }

        static bool HostAGame(MenuEvent e)
        {
            l.Erase();
            
            GameSetup.SetupGame(e);
            l.Draw();

            return true;
        }
        

        
    
    }

    class Logo
    {
        protected string[] textLines;
        protected string text;
        public Int2 Size {get; protected set;}
        public bool visable;
        string emptyLine = "";
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                bool redraw = visable;
                if(redraw)
                {
                    Erase();
                }

                text = value;
                textLines = text.Split('\n');
                Size = new Int2(0, textLines.Length);
                for(int i = 0; i < textLines.Length; i++)
                {
                    if (textLines[i].Length > Size.x)
                        Size.x = textLines[i].Length;
                }
                emptyLine = Stringer.GetFilledString(Size.x, ' ');
                if (redraw)
                {
                    Draw();
                }
            }
        }

        protected Int2 position;
        public Int2 Position
        {
            get
            {
                return position;
            }
            set
            {
                bool redraw = visable;
                if (redraw)
                {
                    Erase();
                }

                position = value;

                if (redraw)
                {
                    Draw();
                }
            }
        }


        public void Draw()
        {
            visable = true;
            for(int y = 0; y < textLines.Length; y++)
            {
                Renderer.Write(textLines[y], position.x, position.y + y);
            }

        }

        public void Erase()
        {
            visable = false;
            
            for (int y = 0; y < textLines.Length; y++)
            {
                Renderer.Write(emptyLine, position.x, position.y + y);
            }
        }
    }

    class GameSetup
    {
        public static int[] ships { get; protected set; } = new int[8];
        public static int[] shipsOnBoard { get; protected set; } = new int[8];
        static List<MenuControll> shipControlls;
        public static Map prepareMap { get; protected set; }
        public static int TurnTimer { get; protected set; }
        public static bool SetupGame(MenuEvent e)
        {
            e.Menu.Erase();
            GameSetup GS = new GameSetup();
            
            Menu gameMenu = new Menu(new Int2(1, 1), new Int2(32, 45), "Game Setup", Boxes.BoxType.doubled)
            {
                VerticalTextWrapping = Menu.Wrapping.wrapping
            };
            gameMenu.AddControll(new LineSeparatorControll("sep0", "sep0"));
            gameMenu.AddControll(new LabelControll("- Game options -", "game_label"));
            gameMenu.AddControll(new IntSwitcherControll("Turn timer", "timer")
            {
                Max = 600,
                Min = 0,
                Step = 1,
                Value = 0,
                MinSpecialText = "∞",
                FastStepTime = 200
            });
            gameMenu.AddControll(new IntSwitcherControll("Total lenght limit", "len_lim")
            {
                Max = 1152,
                Min = 0,
                Step = 1,
                Value = 0,
                MinSpecialText = "∞",
                FastStepTime = 200

            });
            gameMenu.AddControll(new IntSwitcherControll("Collision distance", "col_dist")
            {
                Value = 1,
                Min = 0,
                Max = 32,
                Step = 1,
                FastStepTime = 200
            }, CollisionDistanceChanges);
            gameMenu.AddControll(new CheckBoxControll("Corner collisions", "cor_coll", true)
            {
                TrueValue = "■",
                FalseValue = " "
            }, CollisionChanges);

            gameMenu.AddControll(new LineSeparatorControll("sep1", "sep1"));
            gameMenu.AddControll(new LabelControll("- Board -", "board_label"));
            
            gameMenu.AddControll(new IntSwitcherControll("Board width", "width", 10, 4, 50, 1), MapSwitcherChanged);
            gameMenu.AddControll(new IntSwitcherControll("Board height", "height", 10, 4, 40, 1), MapSwitcherChanged);

            gameMenu.AddControll(new LineSeparatorControll("sep2", "sep2"));
            gameMenu.AddControll(new LabelControll("- Ships -", "ships_label"));

            for(int i = 0; i < 8; i++)
            {
                gameMenu.AddControll(new IntSwitcherControll($"Lenght {i+1}", $"{i+1}len", Arit.Clamp(4 - i, 0, 8), 0, 32, 1), ShipCountChanged);
                ships[i] = Arit.Clamp(4 - i, 0, 8);
                shipsOnBoard[i] = 0;
            }

            gameMenu.AddControll(new LineSeparatorControll("sep3", "sep3"));
            gameMenu.AddControll(new MenuControll("◊ START THE GAME ◊", "start"), ShipSetup);

            gameMenu.AddControll(new LineSeparatorControll("sep4", "sep4"));
            gameMenu.AddControll(new MenuControll("Go back", "exit"), gameMenu.Exit);

            gameMenu.AddControll(new LineSeparatorControll("sep_fin", "sep_fin"));

            prepareMap = new Map(new Int2(10, 10), new Int2(34, 1));
            prepareMap.Draw();

            DrawShips();
            gameMenu.WaitForInput();

            EraseShips();
            prepareMap.Erase();
            e.Menu.Draw();
            return true;
        }


        public static bool CollisionChanges(MenuEvent e)
        {
            CheckBoxControll c = (CheckBoxControll)((MenuControllEvent)e).Controll;
            prepareMap.DiagonalCollisions = c.Value == 1;

            prepareMap.Draw();
            return true;
        }

        public static bool CollisionDistanceChanges(MenuEvent e)
        {
            prepareMap.CollisionDistance = ((MenuControllEvent)e).Controll.GetValue();
            prepareMap.Draw();
            return true;
        }

        public static bool TurnTimerChanges(MenuEvent e)
        {
            TurnTimer = ((MenuControllEvent)e).Controll.GetValue();
            return true;
        }

        public static bool MapSwitcherChanged(MenuEvent e)
        {
            
            MenuControll controll = ((MenuControllEvent)e).Controll;
            Int2 newSize = new Int2();
            if (controll.Identificator == "width")
            {
                if (controll.GetValue() == prepareMap.ContentSize.x)
                    return false;
                newSize = new Int2(controll.GetValue(), prepareMap.ContentSize.y);
            }
            if (controll.Identificator == "height")
            {
                if (controll.GetValue() == prepareMap.ContentSize.y)
                    return false;
                newSize = new Int2(prepareMap.ContentSize.x, controll.GetValue());
            }

            EraseShips();
            prepareMap.ContentSize = newSize;
            prepareMap.Draw();
            DrawShips();
            return true;
        }

        public static bool ShipCountChanged(MenuEvent e)
        {
            EraseShips();
            IntSwitcherEvent ise = (IntSwitcherEvent)e;
            int value = ise.Value;
            string id = ise.Controll.Identificator;
            id = id.Replace("len", "");
            int shipNo;
            if(!int.TryParse(id, out shipNo))
            {
                return false;
            }
            ships[shipNo - 1] = value;
            DrawShips();
            return true;
        }

        static void DrawShips()
        {
            Int2 startPos = new Int2(36 + prepareMap.Size.x * 2, 1);
            for(int i = 0; i < ships.Length; i++)
            {
                if (ships[i] > 0)
                {
                    Renderer.Write(Stringer.GetFilledString(i * 2 + 2, "░░▒▒").PastelBg(Color.DarkGray).Pastel(Color.LightGray) + $" x{ships[i]}", startPos);
                    startPos.y += 2;
                }
            }
        }

        static void EraseShips()
        {
            Int2 startPos = new Int2(36 + prepareMap.Size.x * 2, 1);
            string shipStr;
            for (int i = 0; i < ships.Length; i++)
            {
                if (ships[i] > 0)
                {
                    shipStr = Stringer.GetFilledString(i * 2 + 2, ' ');
                    Renderer.Write($"{shipStr}     ", startPos);
                    startPos.y += 2;
                }
            }
        }


        public static bool PutDownShip(MenuEvent e)
        {

            MenuControllEvent ce = (MenuControllEvent)e;
            TextLine comm = new TextLine(new Int2(prepareMap.Position.x, prepareMap.Size.y + 1), " ", true);
            comm.Size = new Int2(prepareMap.Size.x, 1);
            comm.Alignment = Stringer.TextAlignment.Middle;
            string id = ce.Controll.Identificator;
            id = id.Replace("len", "");
            int len;
            bool done = false;
            bool save = false;
            if (!int.TryParse(id, out len))
                return false; ;
            Ship s = prepareMap.AddShip(len, new Int2(0,0));
            s.PickUp();
            prepareMap.GenerateCollisionsMap();
            s.UpdateCollisionState();
            Int2 cursorPosition = new Int2();
            bool wereColliding = false;
            bool doUpdate = true;
            ConsoleKey response;
            do
            {
                //prepareMap.GenerateCollisionsMap();
                if (s.Collision == Ship.CollisionState.overlap)
                {
                    wereColliding = true;
                    comm.SetText("SHIPS ARE OVERLAPPING!", Color.Red, Color.Black);
                }
                else if (s.Collision == Ship.CollisionState.zone)
                {
                    wereColliding = true;
                    comm.SetText("SHIP IN COLLISION ZONE!", Color.DarkRed, Color.Black);
                }
                else
                {
                    if (wereColliding)
                    {
                        wereColliding = false;
                        comm.SetText("");
                    }
                }
                if (doUpdate)
                {
                    prepareMap.Draw();
                }
                response = Console.ReadKey(true).Key;

                switch (response)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        doUpdate = s.MoveBy(Int2.Left);
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        doUpdate = s.MoveBy(Int2.Down);
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        doUpdate = s.MoveBy(Int2.Right);
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        doUpdate = s.MoveBy(Int2.Up);
                        break;
                    case ConsoleKey.Enter:
                        done = true;
                        break;
                    case ConsoleKey.Escape:
                        done = true;
                        s.Delete();
                        s = null;
                        doUpdate = true;
                        break;
                    case ConsoleKey.R:
                        doUpdate = s.RotateShip();
                        break;
                }
                
                if(s != null && s.Collision != Ship.CollisionState.none)
                {
                    done = false;
                }
                

            } while (!done);
            if (s != null)
            {
                s.PutDown();
                shipsOnBoard[s.lenght - 1]++;
                Renderer.AddDissapearingText("Added a new ship", 1000, new Int2(prepareMap.Position.x + prepareMap.Size.x - 8, 0));
            }
            prepareMap.GenerateCollisionsMap();
            prepareMap.Draw();
            
            return save;
        }

        public static bool ShipSetup(MenuEvent e)
        {
            EraseShips();
            e.Menu.Erase();
            shipControlls = new List<MenuControll>();
            prepareMap.Erase();

            Menu boardMenu = new Menu(new Int2(1, 1), new Int2(32, 45), "Board Setup", Boxes.BoxType.doubled)
            {
                VerticalTextWrapping = Menu.Wrapping.wrapping
            };

            boardMenu.AddControll(new LineSeparatorControll("sep0", "sep0"));
            boardMenu.AddControll(new LabelControll("- Select ship -", "ship_label"));
            for (int i = 0; i < ships.Length; i++)
            {
                if (ships[i] > 0)
                {

                    shipControlls.Add(boardMenu.AddControll(new MenuControll($"Lenght {i + 1} - {shipsOnBoard[i]}/{ships[i]} on board", $"len{i + 1}"), PutDownShip));
                }
            }

            boardMenu.AddControll(new LineSeparatorControll("sep4", "sep4"));
            boardMenu.AddControll(new MenuControll("Go back", "exit"), boardMenu.Exit);

            boardMenu.AddControll(new LineSeparatorControll("sep_fin", "sep_fin"));

            prepareMap.ShowCollisions = true;
            prepareMap.Draw();

            boardMenu.WaitForInput();

            prepareMap.Erase();
            prepareMap.ShowCollisions = false;
            e.Menu.Draw();
            DrawShips();
            prepareMap.Draw();
            return true;
        }
    }


    class TextLine
    {
        protected Stringer.TextAlignment alignment;
        public Stringer.TextAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (alignment == value)
                    return;
                Hide();
                alignment = value;
                Show();
            }
        }
        Int2 position;
        public Int2 Position { get
            {
                return position;
            }
            set
            {
                if (value == position)
                    return;
                Hide();
                position = value;
                Show();
            }
        }
        Int2 size;
        public Int2 Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value == size)
                    return;
                if (alignment == Stringer.TextAlignment.Wrapp)
                    return;
                Hide();
                size = value;
                Show();
            }
        }


        protected Color backgroundColor;
        protected Color textColor;


        protected string emptyLine;
        protected string previousText;
        public string Text { get; protected set; }
        public bool IsVIsable { get; protected set; }

        public TextLine(Int2 position, string text, bool visable)
        {
            this.position = position;
            previousText = "";
            IsVIsable = visable;
            emptyLine = "";
            UpdateText(text);
        }

        public void SetText(string text)
        {
            SetText(text, textColor, backgroundColor);
        }
        public void SetText(string text, Color textColor, Color backgroundColor)
        {
            
            this.textColor = textColor;
            this.backgroundColor = backgroundColor;
            UpdateText(text);
        }

        public void SetColor(Color textColor, Color backgroundColor)
        {
            this.textColor = textColor;
            this.backgroundColor = backgroundColor;
            UpdateText(Text);
        }

        void UpdateText(string newText)
        {

            previousText = Text;
            Text = newText;
            if (IsVIsable)
                Renderer.Write(emptyLine, Position);
            emptyLine = Stringer.GetFilledString(newText.Length, ' ');
            Size = new Int2(Text.Length, 1);
            Refresh();
        }



        public void Show()
        {
            IsVIsable = true;
            Refresh();
        }

        void Refresh()
        {//Problemy z kolorami w alignmencie, bo pastel itd... zrobić align lokalny albo w stringerze z pastelem
            if (IsVIsable && Text.Length > 0)
            {
                Renderer.Write(Stringer.AlighnString(Text, size.x, alignment).Replace(Text, Text.Pastel(textColor).PastelBg(backgroundColor)), position);
            }
        }

        void Erase()
        {

        }

        public void Hide()
        {
            IsVIsable = false;
                Renderer.Write(emptyLine, Position);
        }
    }

}
