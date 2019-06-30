using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Pastel;




namespace Kajaki
{
    class Program
    {
        static int someCounter = 0;
        static Int2 windowSize;
        static Map prepareMap;
        static void Main(string[] args)
        {
            windowSize = new Int2(160, 50);
            Console.SetWindowSize(windowSize.x, windowSize.y);

            Console.CursorVisible = false;
            Int2 menuSize = new Int2(40, 10);
            Menu mainMenu = new Menu(new Int2((windowSize.x - menuSize.x) /2, (windowSize.y - menuSize.y) / 2), menuSize, "Main menu", Boxes.BoxType.doubled);
            mainMenu.AddControll(new MenuControll("Host a game", "host"));
            mainMenu.GetControll("host").AddAction(SetupGame);
            mainMenu.AddControll(new MenuControll("Ships", "ships"));
            mainMenu.AddControll(new MenuControll("Ship designer", "designer"));
            mainMenu.AddControll(new MenuControll("Exit", "exit"));
            mainMenu.GetControll("ships").AddAction(ShipSetup);
            mainMenu.GetControll("exit").AddAction(mainMenu.Exit);
            mainMenu.WaitForInput();

        }

        static bool SetupGame(MenuEvent e)
        {
            e.Menu.Erase();
            Menu gameMenu = new Menu(new Int2(1, 1), new Int2(30, 15), "Game Setup", Boxes.BoxType.doubled);
            gameMenu.AddControll(new MenuControll("Game options", "game"));
            gameMenu.GetControll("game").AddAction(SetupGameOptions);
            gameMenu.AddControll(new MenuControll("Board options", "board"));
            gameMenu.GetControll("board").AddAction(SetupBoard);
            gameMenu.AddControll(new MenuControll("Ship counts", "ships"));
            gameMenu.GetControll("ships").AddAction(AddShips);
            gameMenu.AddControll(new MenuControll("Go back", "exit"));
            gameMenu.GetControll("exit").AddAction(gameMenu.Exit);



            prepareMap = new Map(new Int2(10, 10), new Int2(35, 1));
            prepareMap.Draw();
            gameMenu.WaitForInput();

            prepareMap.Erase();
            e.Menu.Draw();
            return true;
        }

        static bool SetupGameOptions(MenuEvent e)
        {
            e.Menu.Erase();
            Menu switcher = new Menu(new Int2(1, 1), new Int2(30, 15), "Game Options", Boxes.BoxType.doubled);
            switcher.AddControll(new CheckBoxControll("Test", "test", false));
            switcher.GetControll("test").AddAction(GameOptionsChagne);
            switcher.AddControll(new MenuControll("Go back", "exit"));
            switcher.GetControll("exit").AddAction(switcher.Exit);



            prepareMap = new Map(new Int2(10, 10), new Int2(35, 1));
            prepareMap.Draw();
            switcher.WaitForInput();

            //prepareMap.Erase();
            e.Menu.Draw();
            return true;
        }

        static bool SetupBoard(MenuEvent e)
        {
            e.Menu.Erase();
            Menu switcher = new Menu(new Int2(1, 1), new Int2(30, 15), "Board Options", Boxes.BoxType.doubled);
            switcher.AddControll(new IntSwitcherControll("Board width", "width", 10, 4, 50, 1));
            switcher.GetControll("width").AddAction(MapSwitcherChanged);
            switcher.AddControll(new IntSwitcherControll("Board height", "height", 10, 4, 40, 1));
            switcher.GetControll("height").AddAction(MapSwitcherChanged);
            switcher.AddControll(new MenuControll("Go back", "exit"));
            switcher.GetControll("exit").AddAction(switcher.Exit);



            prepareMap = new Map(new Int2(10, 10), new Int2(35, 1));
            prepareMap.Draw();
            switcher.WaitForInput();

            //prepareMap.Erase();
            e.Menu.Draw();
            return true;
        }

        static bool GameOptionsChagne(MenuEvent e)
        {
            e.Menu.Name = "HEHE";

            return true;
        }

        static bool MapSwitcherChanged(MenuEvent e)
        {
            MenuControll controll = ((MenuControllEvent)e).Controll;
            if (controll.identificator == "width")
            {
                prepareMap.ContentSize = new Int2(controll.GetValue(), prepareMap.ContentSize.y);
            }
            if (controll.identificator == "height")
            {
                prepareMap.ContentSize = new Int2(prepareMap.ContentSize.x, controll.GetValue());
            }
            prepareMap.Draw();
            return true;
        }

        static bool AddShips(MenuEvent e)
        {
            e.Menu.Erase();
            Menu adder = new Menu(new Int2(1, 1), new Int2(30, 15), "Ships", Boxes.BoxType.doubled);
            int maxLen = Arit.TakeGreater(prepareMap.ContentSize.x, prepareMap.ContentSize.y);
            adder.AddControll(new MenuControll("Go back", "exit"));
            adder.GetControll("exit").AddAction(adder.Exit);
            adder.AddControll(new IntSwitcherControll("√Lenght 1", "1len", 4, 0, 32, 1));
            adder.AddControll(new IntSwitcherControll("Lenght 2", "2len", 3, 0, 32, 1));
            adder.AddControll(new IntSwitcherControll("Lenght 3", "3len", 2, 0, 32, 1));
            adder.AddControll(new IntSwitcherControll("Lenght 4", "4len", 1, 0, 32, 1));
            adder.AddControll(new IntSwitcherControll("Lenght 5", "5len", 0, 0, 32, 1));
            adder.AddControll(new IntSwitcherControll("Lenght 6", "6len", 0, 0, 32, 1));
            adder.AddControll(new IntSwitcherControll("Lenght 7", "7len", 0, 0, 32, 1));
            adder.AddControll(new IntSwitcherControll("Lenght 8", "8len", 0, 0, 32, 1));
            adder.GetControll("exit").AddAction(adder.Exit);
            adder.WaitForInput();
            e.Menu.Draw();
            return true;
        }


        static bool ShipSetup(MenuEvent e)
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

        
    
    }



    class Renderer
    {
        public enum HorizontalTextAlignment { left, middle, right }
        public enum VerticalTextAlignment { upper, middle, lower }
        public static void Write(string text, Int2 position)
        {
            Write(text, position.x, position.y);
        }
        public static void Write(string text, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }
        public static void Write(int text, Int2 position)
        {
            Write(text, position.x, position.y);
        }
        public static void Write(int text, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }

        public static bool IsBorder(Int2 pos, Int2 size)
        {
            return !(pos.x > 0 && pos.x < size.x - 1 && pos.y > 0 && pos.y < size.y - 1);
        }


    }


    


    class Map
    {
        protected Int2 position;
        public Int2 Position
        {
            get
            {
                return position;
            }
            set
            {
                bool doRedraw = IsVisable;

                if(doRedraw)
                    Erase();

                position = value;

                if(doRedraw)
                    Draw();
            }
        }
        Int2 size = new Int2(12, 12);
        public Int2 Size
        {
            get
            {
                return size;
            }
            set
            {
                EraseBorders(value);
                size = new Int2(Arit.Clamp(value.x, 1, int.MaxValue), Arit.Clamp(value.y, 1, int.MaxValue));
                contentSize = new Int2(size.x - 2, contentSize.y - 2);
                SetupLines();

            }
        }
        Int2 contentSize = new Int2(10, 10);
        public Int2 ContentSize
        {
            get
            {
                return contentSize;
            }
            set
            {
                EraseBorders(value);
                contentSize = new Int2(Arit.Clamp(value.x, 1, int.MaxValue), Arit.Clamp(value.y, 1, int.MaxValue));
                size = new Int2(contentSize.x + 2, contentSize.y + 2);
                SetupLines();

            }
        }
        List<Ship> ships;
        Int2 contentPosition;      
        public string waterLane;
        string upBorder;
        public string midBorder;
        string downBorder;
        string emptyLine;
        int frame;
        public bool IsVisable { get; protected set; }

        public Map(Int2 contentSize, Int2 position)
        {
            ships = new List<Ship>();
            Position = position;
            contentPosition = new Int2(Position.x + 1, Position.y + 1);
            ContentSize = contentSize;
            frame = 0;

            
        }

        void EraseBorders(Int2 newSize)
        {
            if (emptyLine == null)
                return;
            if (contentSize == null)
                return;
            if (newSize == null)
                return;
            //Renderer.Write(emptyLine, Position);
            if (contentSize.y > newSize.y)
            Renderer.Write(emptyLine, Position.x, Position.y + size.y - 1);
            if (contentSize.x > newSize.x)
                for (int y = 0; y < size.y; y++)
                {
                    //Renderer.Write(" ", Position.x, Position.y + y);
                    Renderer.Write(" ", Position.x + size.x - 1, Position.y + y);

                }
        }

        void SetupLines()
        {
            waterLane = Misc.GetFilledString(contentSize.x, Bars.transparent[1]);
            upBorder = $"█{Misc.GetFilledString(contentSize.x, '█')}█";
            midBorder = $"█{waterLane}█";
            emptyLine = Misc.GetFilledString(contentSize.x + 2, ' ');
            downBorder =(string) upBorder.Clone();
        }

        public void Draw()
        {
            IsVisable = true;
            DrawMapRaw();

            for(int i = 0; i < ships.Count; i++)
            {
                for(int j = 0; i < ships[i].decks.Length; j++)
                {
                    Renderer.Write("█", ships[i].decks[j].position);
                }
            }
        }

        public void Erase()
        {
            IsVisable = false;
            string fullEmptyLine = emptyLine + "  ";
            for (int y = 0; y < size.y; y++)
            {
                Renderer.Write(fullEmptyLine, Position.x, Position.y + y);
            }
        }

        void DrawMapRaw()
        {
            Renderer.Write(upBorder, Position);
            Renderer.Write(downBorder, Position.x, Position.y + size.y -1);
            for (int y = 1; y < size.y - 1; y++)
            {
                Renderer.Write(midBorder, Position.x, Position.y + y);
            }
        }


    }

    class Deck
    {
        public Int2 position;
        public Ship ship;
        public Ship.ShipState deckState;

        public void Hit()
        {
            deckState = Ship.ShipState.hit;
            ship.Hit();
        }

        public void Sunk()
        {
            deckState = Ship.ShipState.sunk;
        }

    }

    class Ship
    {
        public enum ShipState {fine, hit, sunk};
        public int id;
        public Deck[] decks;
        public ShipState shipState;
        public int hits;

        public void Hit()
        {
            hits++;
            if(hits == decks.Length)
            {
                for(int i = 0; i < decks.Length; i++)
                {
                    decks[i].Sunk();
                }
            }
            shipState = ShipState.sunk;
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
            emptyLine = Misc.GetFilledString(size.x - 2, ' ');
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
