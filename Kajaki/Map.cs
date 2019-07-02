using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Pastel;

namespace Kajaki
{
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

                if (doRedraw)
                    Erase();

                position = value;

                if (doRedraw)
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
        public List<Ship> ships;
        Int2 contentPosition;
        public string waterLane;
        string upBorder;
        public string[] midBorder;
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
                    Renderer.Write("  ", Position.x + size.x * 2 - 2, Position.y + y);

                }
        }

        void SetupLines()
        {
            waterLane = Stringer.GetFilledString(contentSize.x * 2, Bars.transparent[1]);
            //upBorder = $"█{Misc.GetFilledString(contentSize.x, '█')}█";
            upBorder = "██".Pastel(Color.Gold);
            for (int i = 0; i < contentSize.x; i++)
            {
                upBorder += "██".Pastel((i % 2 == 1 ? Color.Gray : Color.LightGray));
            }
            upBorder += "██".Pastel(Color.Gold);
            midBorder = new string[2];
            midBorder[0] = "██".Pastel(Color.Gray) + $"{waterLane.Pastel(Color.SteelBlue)}" + "██".Pastel(Color.Gray);
            midBorder[1] = "██".Pastel(Color.LightGray) + $"{waterLane.Pastel(Color.SteelBlue)}" + "██".Pastel(Color.LightGray);
            emptyLine = Stringer.GetFilledString(size.x * 2, ' ');
            downBorder = (string)upBorder.Clone();
        }

        public void Draw()
        {
            IsVisable = true;
            DrawMapRaw();

            for (int i = 0; i < ships.Count; i++)
            {
                for (int j = 0; i < ships[i].decks.Length; j++)
                {
                    Renderer.Write("██", ships[i].decks[j].position.x * 2 + contentPosition.x, ships[i].decks[j].position.y + contentPosition.y);
                }
            }
        }

        public void Erase()
        {
            IsVisable = false;
            for (int y = 0; y < size.y; y++)
            {
                Renderer.Write(emptyLine, Position.x, Position.y + y);
            }
        }

        void DrawMapRaw()
        {
            Renderer.Write(upBorder, Position);
            Renderer.Write(downBorder, Position.x, Position.y + size.y - 1);
            for (int y = 1; y < size.y - 1; y++)
            {
                Renderer.Write(midBorder[y % 2], Position.x, Position.y + y);
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
        public enum ShipState { fine, hit, sunk };
        public int id;
        public Deck[] decks;
        public ShipState shipState;
        public int hits;

        public void Hit()
        {
            hits++;
            if (hits == decks.Length)
            {
                for (int i = 0; i < decks.Length; i++)
                {
                    decks[i].Sunk();
                }
            }
            shipState = ShipState.sunk;
        }
    }
}
