using System;
using System.Collections.Generic;
using System.Threading;
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
                CollisionMap = new int[contentSize.x, contentSize.y];
                SetupLines();
                GenerateCollisionsMap();
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
                CollisionMap = new int[contentSize.x, contentSize.y];
                SetupLines();
                GenerateCollisionsMap();
            }
        }

        public int[,] CollisionMap { get; protected set; }
        bool diagonalCollisions;
        public bool DiagonalCollisions
        {
            get
            {
                return diagonalCollisions;
            }
            set
            {
                if (value == diagonalCollisions)
                {
                    return;
                }
                diagonalCollisions = value;
                GenerateCollisionsMap();
            }
        }
        int collisionDistance;
        public int CollisionDistance
        {
            get
            {
                return collisionDistance;
            }
            set
            {
                if (value == collisionDistance)
                {
                    return;
                }
                collisionDistance = value;
                GenerateCollisionsMap();
            }
        }
        public bool ShowCollisions { get; set; }
        int maxDistance;
        public List<Ship> Ships { get; protected set; }
        Int2 contentPosition;
        string upBorder;
        string[] midBorder;
        string downBorder;
        string waterLine;
        string emptyLine;
        string emptyBoardLine;

        Color wavesColor;
        Color waterColor;

        public bool IsVisable { get; protected set; }
        bool redrawCollision;
        bool redrawBorders;
        List<Int2> redrawTileList;
        public Map(Int2 contentSize, Int2 position)
        {
            diagonalCollisions = true;
            redrawCollision = true;
            redrawBorders = true;
            collisionDistance = 1;
            Ships = new List<Ship>();
            Position = position;
            contentPosition = new Int2(Position.x + 2, Position.y + 1);
            ContentSize = contentSize;
            waterColor = Color.FromArgb(16, 16, 32);
            wavesColor = Color.SteelBlue;
            redrawTileList = new List<Int2>();
            SetupLines();
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
            waterLine = Stringer.GetFilledString(contentSize.x * 2, Bars.transparent[1]).Pastel(wavesColor).PastelBg(waterColor);
            emptyBoardLine = Stringer.GetFilledString(contentSize.x * 2, ' ');
            upBorder = "██".Pastel(Color.Gold);
            for (int i = 0; i < contentSize.x; i++)
            {
                upBorder += "██".Pastel((i % 2 == 1 ? Color.Gray : Color.LightGray));
            }
            upBorder += "██".Pastel(Color.Gold);
            midBorder = new string[2];
            midBorder[0] = "██".Pastel(Color.Gray) + emptyBoardLine + "██".Pastel(Color.Gray);
            midBorder[1] = "██".Pastel(Color.LightGray) + emptyBoardLine + "██".Pastel(Color.LightGray);
            emptyLine = Stringer.GetFilledString(size.x * 2, ' ');
            downBorder = (string)upBorder.Clone();
            redrawBorders = true;
        }

        public void Draw()
        {
            IsVisable = true;
            if (redrawBorders)
            {
                DrawMapRaw();
            }

            if (ShowCollisions)
            {
                if (redrawCollision)
                {
                    DrawCollisions();
                }
                else
                {
                    UpdateCollisions();
                }
            }
            else
            {
                DrawWater();
            }
            DrawShips();
        }

        public void DrawShips()
        {
            Color shipColor;
            string shipSymbol, editedShipSymbol;
            Ship s;
            Deck d;
            for (int i = 0; i < Ships.Count; i++)
            {
                s = Ships[i];
                shipSymbol = (Ships[i].BoardState == Ship.OnBoardState.putDown ? "██" : "▒▒");

                for (int j = 0; j < s.Decks.Length; j++)
                {
                    d = s.Decks[j];
                    if (IsTileOccupied(d.Position, s.Id))
                    {
                        editedShipSymbol = shipSymbol.PastelBg(Color.Red).Pastel(Color.White);
                    }
                    else if (IsTileColliding(d.Position))
                    {
                        editedShipSymbol = shipSymbol.PastelBg(Color.DarkRed).Pastel(Color.White);
                    }
                    else
                    {
                        editedShipSymbol = shipSymbol;
                    }
                    Renderer.Write(editedShipSymbol, Ships[i].Decks[j].Position.x * 2 + contentPosition.x, Ships[i].Decks[j].Position.y + contentPosition.y);
                }
            }
        }

        bool IsTileOccupied(Int2 position)
        {
            for (int s = 0; s < Ships.Count; s++)
            {
                if (Ships[s].TryHit(position))
                    return true;
            }
            return false;
        }

        public bool IsTileOccupied(Int2 position, int id)
        {
            for (int s = 0; s < Ships.Count; s++)
            {
                if (Ships[s].Id != id && Ships[s].TryHit(position))
                    return true;
            }
            return false;
        }

        public bool IsTileColliding(Int2 position)
        {
            return CollisionMap[position.x, position.y] <= collisionDistance;
        }

        void DrawCollisions()
        {
            string mapLine;
            float max = contentSize.x + contentSize.y;
            for (int y = 0; y < contentSize.y; y++)
            {
                mapLine = "";
                for (int x = 0; x < contentSize.x; x++)
                {
                    mapLine += GetCollisionTile(x, y);
                }
                Renderer.Write(mapLine, contentPosition.x, contentPosition.y + y);
            }
            redrawCollision = false;
        }
        public void UpdateTiles(Int2[] tilesToUpdate)
        {
            redrawTileList.AddRange(tilesToUpdate);
        }
        void UpdateCollisions()
        {
            for(int i = 0; i < redrawTileList.Count; i++)
            {
                Renderer.Write(GetCollisionTile(redrawTileList[i]), redrawTileList[i].x*2 + contentPosition.x, contentPosition.y + redrawTileList[i].y);
            }
            redrawTileList.Clear();
        }

        string GetCollisionTile(Int2 position)
        {
            return GetCollisionTile(position.x, position.y);
        }
        string GetCollisionTile(int x, int y)
        {
            return "▒▒".Pastel((CollisionMap[x, y] > collisionDistance ? Color.Green : Color.Red)).PastelBg(Color.Black);
        }

        void DrawDistances()
        {
            string mapLine;
            float max = contentSize.x + contentSize.y;
            Color dargDarkGray = Color.FromArgb(16, 16, 16);
            for (int y = 0; y < contentSize.y; y++)
            {
                mapLine = "";
                for (int x = 0; x < contentSize.x; x++)
                {
                    mapLine += CollisionMap[x, y].ToString().PadLeft(2, ' ').Pastel((CollisionMap[x, y] > collisionDistance ? Color.Green : Color.Red)).PastelBg(((x + y) % 2 == 0 ? Color.Black : dargDarkGray));
                }
                Renderer.Write(mapLine, contentPosition.x, contentPosition.y + y);
            }
        }

        public void Erase()
        {
            IsVisable = false;
            for (int y = 0; y < size.y; y++)
            {
                Renderer.Write(emptyLine, Position.x, Position.y + y);
            }
            redrawBorders = true;
        }



        void DrawMapRaw()
        {
            Renderer.Write(upBorder, Position);
            Renderer.Write(downBorder, Position.x, Position.y + size.y - 1);
            for (int y = 1; y < size.y - 1; y++)
            {
                Renderer.Write(midBorder[y % 2], Position.x, Position.y + y);
            }
            redrawBorders = false;
        }

        void DrawWater()
        {
            for (int y = 0; y < contentSize.y; y++)
            {
                Renderer.Write(waterLine, contentPosition.x, contentPosition.y + y);
            }
        }

        public Ship AddShip(int lenght, Int2 position)
        {
            Ship s = new Ship(lenght, position, this, DateTime.Now.Millisecond);
            Ships.Add(s);
            //GenerateCollisionsMap();
            return s;
        }

        public void RemoveShip(Ship ship)
        {
            Ships.Remove(ship);
            ship = null;
            GenerateCollisionsMap();
        }

        public void GenerateCollisionsMap()
        {

            Loops.ForLoop(contentSize.x, contentSize.y, ResetDistanceAtPoint);
            for (int i = 0; i < Ships.Count; i++)
            {
                GenerateShipCollisions(Ships[i]);
            }
            redrawCollision = true;
        }

        bool ResetDistanceAtPoint(int x, int y)
        {
            CollisionMap[x, y] = contentSize.x + contentSize.y;
            return true;
        }

        void GenerateShipCollisions(Ship ship)
        {
            if (ship.BoardState == Ship.OnBoardState.pickedUp)
                return;
            for(int i = 0; i < ship.Decks.Length; i++)
            {
                WalkFromPoint(ship.Decks[i].Position, 0, contentSize.x + contentSize.y);
            }
        }

        void WalkFromPoint(Int2 point, int distance, int energy)
        {
            
            
            if (energy < 0)
                return;

            if (!Int2.InBox(contentSize - Int2.One, point))
                return;
            if (distance >= CollisionMap[point.x, point.y])
            {
                return;
            }
            CollisionMap[point.x, point.y] = distance;
            if (distance > maxDistance)
                maxDistance = distance;
            if (diagonalCollisions)
            {
                WalkFromPoint(point + Int2.Up + Int2.Right, distance + 1, energy - 1);
                WalkFromPoint(point + Int2.Down + Int2.Right, distance + 1, energy - 1);
                WalkFromPoint(point + Int2.Up + Int2.Left, distance + 1, energy - 1);
                WalkFromPoint(point + Int2.Down + Int2.Left, distance + 1, energy - 1);
            }
            WalkFromPoint(point + Int2.Up, distance + 1, energy - 1);
            WalkFromPoint(point + Int2.Right, distance + 1, energy - 1);
            WalkFromPoint(point + Int2.Down, distance + 1, energy - 1);
            WalkFromPoint(point + Int2.Left, distance + 1, energy - 1);

            return;
        }
    }



    class Deck
    {
        public Int2 Position;
        public Ship Ship { get; protected set; }
        public Ship.ShipState State { get; protected set; }

        public Deck(Int2 position)
        {
            Position = position;
            State = Ship.ShipState.fine;
        }

        public void Hit()
        {
            State = Ship.ShipState.hit;
            Ship.Hit();
        }

        public void Sunk()
        {
            State = Ship.ShipState.sunk;
        }

    }

    class Ship
    {
        public enum ShipState { fine, hit, sunk};
        public enum OnBoardState {putDown, pickedUp}
        public enum CollisionState { none, zone, overlap}
        public int Id { get; protected set; }
        public int lenght;
        public Deck[] Decks { get; protected set; }
        public ShipState State { get; protected set; }
        public OnBoardState BoardState { get; protected set; }
        public CollisionState Collision { get; set; }
        public int hits;
        Int2 leftUpCorner;
        Int2 rightDownCorner;
        public Map MyMap { get; protected set; }

        public Ship(int shipLenght, Int2 position, Map myMap, int id)
        {
            Decks = new Deck[shipLenght];
            MyMap = myMap;
            Id = id;
            lenght = shipLenght;
            leftUpCorner = position;
            Int2 tPos = new Int2(leftUpCorner.x, leftUpCorner.y);
            for (int i = 0; i < shipLenght; i++)
            {
                Decks[i] = new Deck(tPos);
                rightDownCorner = tPos;
                tPos = new Int2(tPos.x + 1, tPos.y);
            }
            

        }

        public void PickUp()
        {
            BoardState = OnBoardState.pickedUp;
        }

        public void PutDown()
        {
            BoardState = OnBoardState.putDown;
        }

        public void Delete()
        {
            if (MyMap == null)
                return;
            MyMap.RemoveShip(this);
        }

        public bool MoveBy(Int2 delta)
        {
            if (BoardState == OnBoardState.putDown)
                return false;

            if (!Int2.InBox(MyMap.ContentSize - Int2.One, leftUpCorner + delta) || !Int2.InBox(MyMap.ContentSize - Int2.One, rightDownCorner + delta))
            {
                return false;
            }
            Int2[] prevPositions = new Int2[lenght];
            for(int i = 0; i < lenght; i++)
            {
                prevPositions[i] = new Int2(Decks[i].Position.x, Decks[i].Position.y);
                Decks[i].Position += delta;
            }
            leftUpCorner += delta;
            rightDownCorner += delta;
            UpdateCollisionState();
            MyMap.UpdateTiles(prevPositions);
            return true;
        }

        public bool RotateShip()
        {
            if (!Int2.InBox(MyMap.ContentSize - Int2.One, leftUpCorner + (rightDownCorner - leftUpCorner).Flipped()))
                return false;

            Int2[] prevPositions = new Int2[lenght - 1];
            for (int i = 0; i < Decks.Length; i++)
            {
                if(i > 0)
                {
                    prevPositions[i-1] = new Int2(Decks[i].Position.x, Decks[i].Position.y);
                }
                Decks[i].Position = Decks[i].Position - leftUpCorner;
                Decks[i].Position.Flip();
                Decks[i].Position = Decks[i].Position + leftUpCorner;
                rightDownCorner = Decks[lenght-1].Position;
            }
            UpdateCollisionState();
            MyMap.UpdateTiles(prevPositions);
            return true;
        }

        public void UpdateCollisionState()
        {
            Collision = CollisionState.none;
            for (int i = 0; i < lenght; i++)
            {
                if (MyMap.IsTileOccupied(Decks[i].Position, Id))
                {
                    Collision = CollisionState.overlap;
                    break;
                }
                if (Collision != CollisionState.zone && MyMap.IsTileColliding(Decks[i].Position))
                {
                    Collision = CollisionState.zone;

                }
            }
        }

        public bool TryHit(Int2 position)
        {
            for (int i = 0; i < lenght; i++)
            {
                if (Decks[i].Position == position)
                    return true;
            }

            return false;
        }


        public void Hit()
        {
            hits++;
            if (hits == Decks.Length)
            {
                for (int i = 0; i < Decks.Length; i++)
                {
                    Decks[i].Sunk();
                }
            }
            State = ShipState.sunk;
        }

        

    }
}
