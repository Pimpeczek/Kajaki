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
        static int[] ships = new int[8];
        static Int2 mapSize;
        static Map prepareMap;

        public static bool SetupGame(MenuEvent e)
        {
            e.Menu.Erase();
            GameSetup GS = new GameSetup();
            prepareMap = new Map(new Int2(10, 10), new Int2());
            Menu gameMenu = new Menu(new Int2(1, 1), new Int2(30, 45), "Game Setup", Boxes.BoxType.doubled)
            {
                VerticalTextWrapping = Menu.Wrapping.wrapping
            };
            gameMenu.AddControll(new LineSeparatorControll("sep0", "sep0"));
            gameMenu.AddControll(new LabelControll("- Game options -", "game_label"));
            gameMenu.AddControll(new IntSwitcherControll("Turn timer", "timer")
            {
                Max = 600,
                Min = 0,
                Step = 10,
                Value = 0,
                MinSpecialText = "No timer",
            });
            gameMenu.AddControll(new IntSwitcherControll("Collision distance", "col_dist")
            {
                Value = 1,
                Min = 0,
                Max = 16,
                Step = 1,
            });
            gameMenu.AddControll(new CheckBoxControll("Corner collisions", "cor_coll", true)
            {
                TrueValue = "■",
                FalseValue = " "
            });

            gameMenu.AddControll(new LineSeparatorControll("sep1", "sep1"));
            gameMenu.AddControll(new LabelControll("- Board -", "board_label"));
            
            gameMenu.AddControll(new IntSwitcherControll("Board width", "width", 10, 4, 50, 1));
            gameMenu.GetControll("width").AddAction(MapSwitcherChanged);
            gameMenu.AddControll(new IntSwitcherControll("Board height", "height", 10, 4, 40, 1));
            gameMenu.GetControll("height").AddAction(MapSwitcherChanged);

            gameMenu.AddControll(new LineSeparatorControll("sep2", "sep2"));
            gameMenu.AddControll(new LabelControll("- Ships -", "ships_label"));

            for(int i = 0; i < 8; i++)
            {
                gameMenu.AddControll(new IntSwitcherControll($"Lenght {i+1}", $"{i+1}len", Arit.Clamp(4 - i, 0, 8), 0, 32, 1));
                gameMenu.GetControll($"{i+1}len").AddAction(ShipCountChanged);
                ships[i] = Arit.Clamp(4 - i, 0, 8);
            }
            gameMenu.AddControll(new MenuControll("TEST", "test"));
            gameMenu.GetControll("test").AddAction(TESTING);
            gameMenu.AddControll(new LineSeparatorControll("sep3", "sep3"));
            gameMenu.AddControll(new MenuControll("◊ START THE GAME ◊", "start"));
            gameMenu.AddControll(new LineSeparatorControll("sep4", "sep4"));
            gameMenu.AddControll(new MenuControll("Go back", "exit"));
            gameMenu.GetControll("exit").AddAction(gameMenu.Exit);

            gameMenu.AddControll(new LineSeparatorControll("sep_fin", "sep_fin"));

            prepareMap = new Map(new Int2(10, 10), new Int2(35, 1));
            prepareMap.Draw();

            DrawShips();

            gameMenu.WaitForInput();

            EraseShips();
            prepareMap.Erase();
            e.Menu.Draw();
            return true;
        }


        public static bool TESTING(MenuEvent e)
        {
            for(int i = 0; i < 1; i++)
            {
                prepareMap.Draw();
            }

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
            mapSize = new Int2(prepareMap.Size.x, prepareMap.Size.y);
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


        public static bool ShipSetup(MenuEvent e)
        {
            e.Menu.Erase();
            ConsoleKey response;
            TextBox tb = new TextBox(new Int2(1, 1), new Int2(32, 6), "Info", Boxes.BoxType.doubled);
            tb.Text = "WSAD/Arrows - cursor movement\nR - Rotate ship\nENTER - select/accept\nESCAPE - go back";

            Menu shipMenu = new Menu(new Int2(1, 8), new Int2(32, 10), "Ships", Boxes.BoxType.doubled);


            tb.Draw();
            Int2 cursorPosition = new Int2();
            int hPoint = 0;
            bool boardFocus = false;
            prepareMap.Position = new Int2(34, 1);
            prepareMap.Draw();
            do
            {

                response = Console.ReadKey(true).Key;

                switch (response)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        if (boardFocus)
                        {
                            cursorPosition.x--;
                        }
                        break;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        if (boardFocus)
                        {
                            cursorPosition.y--;
                        }
                        else
                        {
                            hPoint--;
                        }
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (boardFocus)
                        {
                            cursorPosition.x++;
                        }
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        if (boardFocus)
                        {
                            cursorPosition.y++;
                        }
                        else
                        {
                            hPoint++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (boardFocus)
                        {

                        }
                        else
                        {

                        }
                        break;
                    case ConsoleKey.R:
                        if (!boardFocus)
                            break;
                        break;
                }

            } while (response != ConsoleKey.Escape);
            tb.Erase();

            prepareMap.Erase();

            e.Menu.Draw();
            return true;
        }

        static void PrintSetup()
        {
            Boxes.DrawBox(Boxes.BoxType.doubled, 1, 17, 30, 30);
        }
    }


    


    


    


    class TextBox
    {
        public Int2 Position { get; protected set; }
        public Int2 Size { get; protected set; }
        protected string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;

                if(IsVIsable)
                {
                    Draw();
                }

            }
        }
        protected string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                FormatText();
                DrawText();
            }
        }
        protected string[] textLines;
        Boxes.BoxType boxType;
        string emptyLine;
        public bool IsVIsable { get; protected set; }
        public TextBox(Int2 position, Int2 size, string name, Boxes.BoxType boxType)
        {
            Position = position;
            Size = size;
            Name = name;
            emptyLine = Stringer.GetFilledString(size.x - 2, ' ');
            this.boxType = boxType;
        }

        public void Append(string str)
        {
            Text = Text + '\n' + str;
        }

        public void Draw()
        {
            IsVIsable = true;
            Console.ForegroundColor = ConsoleColor.White;
            Boxes.DrawBox(boxType, Position.x, Position.y, Size.x, Size.y);
            Renderer.Write(Name, Position.x + (Size.x - Name.Length) / 2, Position.y);
            DrawText();
        }

        protected void FormatText()
        {
            textLines = text.Split('\n');
            for(int y = 0; y < textLines.Length; y++)
            {
                textLines[y] = textLines[y].Substring(0, Arit.TakeLower(Size.x - 2, textLines[y].Length));
            }
        }

        protected void DrawText()
        {
            for (int y = 0; y < Size.y - 2; y++)
            {
                Renderer.Write(textLines[y], Position.x + 1, Position.y + 1 + y);
            }
        }
        public void Erase()
        {
            IsVIsable = false;
            string fullEmptyLine = emptyLine + "  ";

            for(int y = 0; y < Size.y; y++)
            {
                Renderer.Write(fullEmptyLine, Position.x, Position.y + y);
            }
            

        }
    }

}
