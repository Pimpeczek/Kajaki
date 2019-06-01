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
        static bool clearOnExit;
        static bool redrawBoxes;
        static void Main(string[] args)
        {
            clearOnExit = true;
            Console.SetWindowSize(40, 20);
            Menu mainMenu = new Menu(new Int2(0,0), new Int2(40, 4), "Main menu", Boxes.BoxType.doubled);
            mainMenu.AddOption("Host a game", 0);
            mainMenu.AddOption("Join a game", 1);
            mainMenu.AddOption("Ship designer", 2);
            mainMenu.AddOption("Exit", -1);
            int response;
            do
            {
                response = mainMenu.WaitForInput();
            } while (response != -1);

        }

    }
    class Renderer
    {
        public void Write(string text, Int2 position)
        {
            Write(text, position.x, position.y);
        }
        public void Write(string text, int x, int y)
        {

        }
    }

    class Menu
    {
        public Int2 Position { get; protected set; }
        public Int2 Size { get; protected set; }
        public string Name { get; protected set; }
        Boxes.BoxType boxType;
        List<(string, int)> options;
        int hPoint;
        int scrollPoint;
        public Menu(Int2 position, Int2 size, string name, Boxes.BoxType boxType)
        {
            options = new List<(string, int)>();
            hPoint = 0;
            scrollPoint = 0;
            Position = position;
            Size = size;
            Name = name;
            this.boxType = boxType;
        }

        public void AddOption(string text, int id)
        {
            options.Add((text, id));
        }

        public int WaitForInput()
        {
            ConsoleKey response;
            Draw();
            DrawOptions();
            do
            {
                response = Console.ReadKey(true).Key;
                switch (response)
                {
                    case ConsoleKey.UpArrow:
                        hPoint--;
                        break;
                    case ConsoleKey.DownArrow:
                        hPoint++;
                        break;
                }
                if( hPoint < 0)
                    hPoint += (options != null && options.Count > 0 ? options.Count : 1);
                hPoint %= (options != null && options.Count > 0 ? options.Count : 1);
                
                
                DrawOptions();

            } while (response != ConsoleKey.Enter);

            return options[hPoint].Item2;
        }
        

        public void Draw()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Boxes.DrawBox(boxType, Position.x, Position.y, Size.x, Size.y);
            Console.SetCursorPosition(Position.x + (Size.x - Name.Length) / 2, Position.y);
            Console.Write(Name);
            
        }


        void DrawOptions()
        {
            int startHeight = Arit.Clamp((Size.y - 2 - options.Count) / 2, 0, Size.y);
            if(hPoint >= Size.y - 2)
            {
                scrollPoint = Arit.Clamp(Size.y - 2 - hPoint, 0, options.Count - Size.y + 2);
            }
            Console.SetCursorPosition(45, 0);
            Console.Write(scrollPoint);
            Console.SetCursorPosition(45, 1);
            Console.Write(hPoint);
            for (int i = 0; i < options.Count && i < Size.y - 2; i++)
            {
                Console.SetCursorPosition(Position.x + (Size.x - options[i + scrollPoint].Item1.Length) / 2, Position.y + startHeight + i - scrollPoint+ 1);
                if (hPoint - scrollPoint == i)
                {
                    Console.Write(options[i + scrollPoint].Item1.PastelBg(Color.Gray));
                }
                else
                {
                    Console.Write(options[i + scrollPoint].Item1);
                }

            }
        }
    }


    class Map
    {
        public Int2 Position { get; protected set; }
        public Int2 Size { get; protected set; }
        List<Ship> ships;

        Int2 contentPosition;
        Int2 contentSize;
        string waterLane;
        string upBorder;
        string midBorder;
        string downBorder;
        int frame;


        public Map(Int2 size, List<Ship> ships)
        {
            this.ships = ships;
            Position = new Int2(2, 2);
            contentPosition = new Int2(Position.x + 1, Position.y + 1);
            contentSize = new Int2(size.x - 2, size.y - 2);
            Size = size;
            frame = 0;

            waterLane = Misc.GetFilledString(size.x - 2, Bars.transparent[1]);
            upBorder = $"█{Misc.GetFilledString(size.x - 2, '█')}█";
            midBorder = $"█{waterLane}█";
            downBorder = $"█{Misc.GetFilledString(size.x - 2, '█')}█";
        }


        public void Draw()
        {
            DrawMapRaw();
            Ship tShip;
            string shipLine;
            for(int i = 0; i < ships.Count; i++)
            {
                tShip = ships[i];
                if (tShip.Changed)
                {
                    for (int y = 0; y < tShip.Size.y; y++)
                    {
                        shipLine = "";
                        for (int x = 0; x < tShip.Size.x; x++)
                        {
                            shipLine += (tShip.Decks[x, y] == null ? waterLane[x] : tShip.Decks[x, y].symbol);
                        }
                        Console.SetCursorPosition(contentPosition.x + tShip.Position.x, contentPosition.y + tShip.Position.y + y);
                        Console.Write(shipLine);
                    }
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
        public char symbol;
        public Deck(char symbol)
        {
            this.symbol = symbol;
        }
    }

    class Ship
    {
        public Int2 Position { get; protected set; }
        public Int2 Size { get; protected set; }
        public Deck[,] Decks { get; protected set; }
        public bool Changed { get; protected set; }
        Deck[,] oryginalDecks;
        Int2 oryginalSize;
        int dir; // 0 = right 1 = down ...
        public Ship(Int2 position, Deck[,] decks)
        {
            Random rng = new Random();
            Position = position;
            Changed = true;
            decks = new Deck[5, 8];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (rng.Next(2) == 0)
                        decks[i, j] = new Deck('#');
                }
            oryginalSize = new Int2(decks.GetLength(0), decks.GetLength(1));
            Size = oryginalSize;
            oryginalDecks = decks;
            this.Decks = decks;
            dir = 0;
        }

        public bool Rotate(int right)
        {
            dir += right;
            dir %= 4;
            return true;
        }

        public bool RotateRight()
        {
            oryginalDecks = Decks;
            oryginalSize = Size;
            Decks = new Deck[Size.y, Size.x];
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Decks[Size.y - y - 1, x] = oryginalDecks[x, y];
                }
            }
            Size = new Int2(oryginalSize.y, oryginalSize.x);
            return true;
        }

        public bool RotateLeft()
        {
            oryginalDecks = Decks;

            Decks = new Deck[Size.y, Size.x];

            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Decks[y, Size.x - x - 1] = oryginalDecks[x, y];
                }
            }
            Size = new Int2(oryginalSize.y, oryginalSize.x);
            return true;
        }

        public void Draw()
        {
            string line;
            for (int y = 0; y < Size.y; y++)
            {
                line = "";
                for (int x = 0; x < Size.x; x++)
                {
                    line += (Decks[x, y] == null ? ' ' : '#');
                }
                Console.SetCursorPosition(2, 2 + y);
                Console.Write(line);
            }

        }

    }


}
