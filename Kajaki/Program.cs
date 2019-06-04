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
        static Int2 windowSize;
        static Map prepareMap;
        static void Main(string[] args)
        {
            windowSize = new Int2(200, 50);
            Console.SetWindowSize(200, 50);

            Console.CursorVisible = false;
            Int2 menuSize = new Int2(40, 10);
            Menu mainMenu = new Menu(new Int2((windowSize.x - menuSize.x) /2, (windowSize.y - menuSize.y) / 2), menuSize, "Main menu", Boxes.BoxType.doubled);
            mainMenu.HorizontalAlignment = Renderer.HorizontalTextAlignment.middle;
            mainMenu.VerticallAlignment = Renderer.VerticalTextAlignment.middle;
            mainMenu.AddOption("Host a game", 0);
            mainMenu.AddOption("Join a game", 1);
            mainMenu.AddOption("Ship designer", 2);
            mainMenu.AddOption("Exit the game", -1);
            //mainMenu.BottomText = "[Enter - confirm]";
            
            int response;
            int all = 0;
            int all2 = 0;
            do
            {
                response = mainMenu.WaitForInput();

                if(response == 0)
                {
                    mainMenu.Erase();
                    SetupBoard();
                }

            } while (response != -1);

        }

        static void SetupBoard()
        {

            Switcher switcher = new Switcher(new Int2(1, 1), new Int2(30, 15), "Board Options", Boxes.BoxType.doubled);
            switcher.AddOption(new IntSwitcherOption("Board width", "width", 10, 10, 50, 1));
            switcher.AddOption(new IntSwitcherOption("Board height", "height", 10, 10, 40, 1));
            switcher.AddChangeAction(MapSwitcherChanged);

            

            prepareMap = new Map(new Int2(10, 10), new Int2(35, 1));
            switcher.WaitForInput();

            Console.ReadKey(true);
        }

        static bool MapSwitcherChanged(Switcher switcher, SwitcherOption switcherOption)
        {
            if (switcherOption.identificator == "width")
            {
                prepareMap.Size.x = switcherOption.GetValue();
            }
            if (switcherOption.identificator == "height")
            {
                prepareMap.Size.y = switcherOption.GetValue();
            }
            prepareMap.Draw();
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
        public Int2 Position { get; protected set; }
        Int2 position;
        public Int2 Size
        {
            get
            {
                return position;
            }
            set
            {
                value.x = Arit.Clamp(value.x, 1, int.MaxValue);
                value.y = Arit.Clamp(value.y, 1, int.MaxValue);
                position = value;
            }
        }
        List<Ship> ships;

        Int2 contentPosition;
        Int2 contentSize;
        string waterLane;
        string upBorder;
        string midBorder;
        string downBorder;
        int frame;


        public Map(Int2 size, Int2 position)
        {
            ships = new List<Ship>();
            Position = position;
            contentPosition = new Int2(Position.x + 1, Position.y + 1);
            contentSize = size;
            Size = new Int2(size.x + 2, size.y + 2);
            frame = 0;

            waterLane = Misc.GetFilledString(Size.x - 2, Bars.transparent[1]);
            upBorder = $"█{Misc.GetFilledString(Size.x - 2, '█')}█";
            midBorder = $"█{waterLane}█";
            downBorder = $"█{Misc.GetFilledString(Size.x - 2, '█')}█";
        }


        public void Draw()
        {
            DrawMapRaw();

            for(int i = 0; i < ships.Count; i++)
            {
                for(int j = 0; i < ships[i].decks.Length; j++)
                {
                    Renderer.Write("█", ships[i].decks[j].position);
                }
            }
        }

        void DrawMapRaw()
        {
            Console.SetCursorPosition(Position.x, Position.y);
            Console.Write(upBorder);
            Console.SetCursorPosition(Position.x, Position.y+ Size.y -1);
            Console.Write(downBorder);
            for (int y = 1; y < Size.y - 1; y++)
            {
                Console.SetCursorPosition(Position.x, Position.y + y);
                Console.Write(midBorder);
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


}
