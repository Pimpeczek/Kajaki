using System;
using System.Collections.Generic;
using System.Drawing;
using Pastel;

namespace Kajaki
{
    class Menu_dep
    {
        public Int2 Position { get; protected set; }
        public Int2 Size { get; protected set; }
        Int2 contentSize;
        Renderer.HorizontalTextAlignment horizontalAlignment;
        Renderer.VerticalTextAlignment verticallAlignment;
        public Renderer.HorizontalTextAlignment HorizontalAlignment
        {
            get
            {
                return horizontalAlignment;
            }
            set
            {
                if (value == horizontalAlignment)
                    return;
                horizontalAlignment = value;
                if (isDrawn)
                    Draw();
            }
        }
        public Renderer.VerticalTextAlignment VerticallAlignment
        {
            get
            {
                return verticallAlignment;
            }
            set
            {
                if (value == verticallAlignment)
                    return;
                verticallAlignment = value;
                if (isDrawn)
                    Draw();
            }
        }
        public string Name { get; protected set; }
        public string BottomText { get; set; }
        Boxes.BoxType boxType;
        List<(string, int)> options;
        List<(ConsoleKey, int)> bindings;
        int hPoint;
        int scrollPoint;
        bool isDrawn;

        char[] boxes;

        public Menu_dep(Int2 position, Int2 size, string name, Boxes.BoxType boxType)
        {
            isDrawn = false;
            options = new List<(string, int)>();
            bindings = new List<(ConsoleKey, int)>();
            boxes = Boxes.GetBoxArray(boxType);
            hPoint = 0;
            scrollPoint = 0;
            Position = position;
            Size = size;
            contentSize = new Int2(size.x - 2, size.y - 2);
            Name = name;
            BottomText = "";
            this.boxType = boxType;
        }

        public void AddOption(string text, int id)
        {
            options.Add((text, id));
        }

        public void AddBinding(int id, ConsoleKey key)
        {
            bindings.Add((key, id));
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
                if (hPoint < 0)
                    hPoint += (options != null && options.Count > 0 ? options.Count : 1);
                hPoint %= (options != null && options.Count > 0 ? options.Count : 1);


                DrawOptions();

            } while (response != ConsoleKey.Enter);
            return options[hPoint].Item2;
        }



        public void Draw()
        {
            isDrawn = true;
            Console.ForegroundColor = ConsoleColor.White;
            Boxes.DrawBox(boxes, Position.x, Position.y, Size.x, Size.y);
            string boxName = Boxes.GetBoxName(Name, boxes);
            Renderer.Write(boxName, Position.x + (Size.x - boxName.Length) / 2, Position.y);
            if (BottomText.Length > 0)
            {
                Renderer.Write(BottomText, Position.x + 1, Position.y + Size.y - 1);
            }
        }

        public void Erase()
        {
            if (!isDrawn)
                return;
            isDrawn = false;
            string emptyLine = Misc.GetFilledString(Size.x, ' ');
            for (int y = 0; y < Size.y; y++)
            {
                Renderer.Write(emptyLine, Position.x, Position.y + y);
            }

        }

        void DrawOptions()
        {

            int startHeight = 0;
            switch (verticallAlignment)
            {
                case Renderer.VerticalTextAlignment.upper:
                    startHeight = 0;
                    break;
                case Renderer.VerticalTextAlignment.middle:
                    startHeight = (contentSize.y - options.Count) / 2;
                    break;
                case Renderer.VerticalTextAlignment.lower:
                    startHeight = contentSize.y - options.Count;
                    break;
            }
            startHeight = Arit.Clamp(startHeight, 0, contentSize.y - 1);
            string option;
            for (int i = scrollPoint; i < contentSize.y + scrollPoint && i < options.Count; i++)
            {
                Renderer.Write(Misc.GetFilledString(options[i].Item1.Length, ' '), Position.x + 1 + HorizontalDist(i), Position.y + i + 1 - scrollPoint + startHeight);
            }



            scrollPoint = Arit.Clamp(hPoint - contentSize.y + 1, 0, options.Count);

            for (int i = scrollPoint; i < contentSize.y + scrollPoint && i < options.Count; i++)
            {
                if (i == hPoint)
                {
                    option = options[i].Item1.PastelBg(Color.Gray);
                }
                else
                {
                    option = options[i].Item1;
                }
                Renderer.Write(option, Position.x + 1 + HorizontalDist(i), Position.y + i + 1 - scrollPoint + startHeight);
            }
        }

        int HorizontalDist(int id)
        {
            return HorizontalDist(options[id].Item1);
        }

        int HorizontalDist(string text)
        {
            return HorizontalDist(text, HorizontalAlignment);
        }

        int HorizontalDist(string text, Renderer.HorizontalTextAlignment alignment)
        {
            int dist = 0;
            switch (alignment)
            {
                case Renderer.HorizontalTextAlignment.left:
                    dist = 0;
                    break;
                case Renderer.HorizontalTextAlignment.middle:
                    dist = (contentSize.x - text.Length) / 2;
                    break;
                case Renderer.HorizontalTextAlignment.right:
                    dist = contentSize.x - text.Length;
                    break;
            }
            return dist;
        }
    }
}
