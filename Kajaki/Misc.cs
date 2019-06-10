﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Kajaki
{
    class Misc
    {
        public static int CountChars(string str, char ch)
        {
            int count = 0;
            foreach (char c in str)
                if (c == ch) count++;
            return count;
        }

        public static string GetFilledString(int len, char c)
        {
            string str = "";
            for (int i = 0; i < len; i++)
                str += c;
            return str;
        }
    }

    class Arit
    {
        public static int Clamp(int value, int inclusiveMin, int inclusiveMax)
        {
            if (value < inclusiveMin)
                return inclusiveMin;
            if (value > inclusiveMax)
                return inclusiveMax;
            return value;
        }

        public static int CountTrues(bool[] boolArr)
        {
            int counter = 0;
            for (int i = 0; i < boolArr.Length; i++)
                if (boolArr[i])
                    counter++;
            return counter;
        }

        public static bool[] NegateBoolArray(bool[] boolArr)
        {
            for (int i = 0; i < boolArr.Length; i++)
                boolArr[i] = !boolArr[i];
            return boolArr;
        }

        public static bool[] GetBoolArray(int lenght, bool state)
        {
            bool[] boolArr = new bool[lenght];
            for (int i = 0; i < boolArr.Length; i++)
                boolArr[i] = state;
            return boolArr;
        }

        public static int[] GetIntArray(int lenght, int state)
        {
            int[] boolArr = new int[lenght];
            for (int i = 0; i < boolArr.Length; i++)
                boolArr[i] = state;
            return boolArr;
        }

        public static int TakeGreater(int x, int y)
        {
            return (x > y ? x : y);
        }

        public static int TakeLower(int x, int y)
        {
            return (x < y ? x : y);
        }
    }

    class Rand
    {
        static Random rng = new Random();

        public static void SetSeed(int seed)
        {
            rng = new Random(seed);
        }

        public static int Int()
        {
            return rng.Next();
        }
        public static int Int(int exclusiveMax)
        {
            return rng.Next(exclusiveMax);
        }
        public static int Int(int inclusiveMin, int exclusiveMax)
        {
            return rng.Next(inclusiveMin, exclusiveMax);
        }

    }

    class MyColor
    {
        public static Color NegateColor(Color c)
        {
            return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
        }
        public static Color MultplyColor(Color c, float multi)
        {
            int iMulti = (int)(multi * 255);
            return Color.FromArgb(c.R * iMulti / 255, c.G * iMulti / 255, c.B * iMulti / 255);
        }
        public static Color RandomColor()
        {
            return Color.FromArgb(Rand.Int(256), Rand.Int(256), Rand.Int(256));
        }
        public static Color RandomColor(int seed)
        {
            Random rng = new Random(seed);
            return Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256));
        }
    }

    public class Int2
    {
        public int x, y;
        public static Int2 zero = new Int2(0, 0);
        public static Int2 one = new Int2(1, 1);

        public Int2()
        {
            x = 0;
            y = 0;
        }
        public Int2(int _x, int _y)
        {
            x = _x; y = _y;
        }

        override public string ToString()
        {
            return x + ", " + y;
        }

        public static Int2 Random()
        {
            return new Int2(Rand.Int(), Rand.Int());
        }
        public static Int2 Random(int exclusiveMax)
        {
            return new Int2(Rand.Int(exclusiveMax), Rand.Int(exclusiveMax));
        }
        public static Int2 Random(Int2 exclusiveMax)
        {
            return new Int2(Rand.Int(exclusiveMax.x), Rand.Int(exclusiveMax.y));
        }
        public static Int2 Random(int exclusiveMin, int exclusiveMax)
        {
            return new Int2(Rand.Int(exclusiveMin, exclusiveMax), Rand.Int(exclusiveMin, exclusiveMax));
        }
        public static Int2 Random(Int2 exclusiveMin, Int2 exclusiveMax)
        {
            return new Int2(Rand.Int(exclusiveMin.x, exclusiveMax.y), Rand.Int(exclusiveMin.y, exclusiveMax.y));
        }

        public static bool InBox(Int2 aSize, Int2 coords)
        {
            return coords <= aSize && coords >= zero;
        }
        public static bool InBox(Int2 aSize, Int2 aCoords, Int2 bCoords)
        {
            return bCoords <= aSize + aCoords && bCoords >= aCoords;
        }
        public static bool InBoxInBox(Int2 aSize, Int2 aCoords, Int2 bSize, Int2 bCoords)
        {
            return bCoords + bSize <= aSize + aCoords && bCoords >= aCoords;
        }



        //Definicje operatorów
        public static Int2 operator +(Int2 i1, Int2 i2) { return new Int2(i1.x + i2.x, i1.y + i2.y); }
        public static Int2 operator +(Int2 i1, int x) { return new Int2(i1.x + x, i1.y + x); }

        public static Int2 operator -(Int2 i1, Int2 i2) { return new Int2(i1.x - i2.x, i1.y - i2.y); }
        public static Int2 operator -(Int2 i1, int x) { return new Int2(i1.x - x, i1.y - x); }

        public static Int2 operator *(Int2 i1, Int2 i2) { return new Int2(i1.x * i2.x, i1.y * i2.y); }
        public static Int2 operator *(Int2 i1, int x) { return new Int2(i1.x * x, i1.y * x); }

        public static Int2 operator /(Int2 i1, Int2 i2) { return new Int2(i1.x / i2.x, i1.y / i2.y); }
        public static Int2 operator /(Int2 i1, int x) { return new Int2(i1.x / x, i1.y / x); }

        public static bool operator ==(Int2 i1, Int2 i2) {  if ((object)i2 == null) return (object)i1 == null; if ((object)i1 == null) return (object)i2 == null;
                                                            if (i1.x == i2.x && i1.y == i2.y) return true; return false; }
        public static bool operator !=(Int2 i1, Int2 i2) {  if ((object)i2 == null) return (object)i1 != null; if ((object)i1 == null) return (object)i2 != null;
                                                            if (i1.x == i2.x && i1.y == i2.y) return false; return true; }
        public static bool operator >=(Int2 i1, Int2 i2) {  if (i1.x >= i2.x && i1.y >= i2.y) return true; return false; }
        public static bool operator <=(Int2 i1, Int2 i2) {  if (i1.x <= i2.x && i1.y <= i2.y) return true; return false; }
        public static bool operator >(Int2 i1, Int2 i2) {   if (i1.x > i2.x && i1.y > i2.y) return true; return false; }
        public static bool operator <(Int2 i1, Int2 i2) {   if (i1.x < i2.x && i1.y < i2.y) return true; return false; }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            Int2 p = (Int2)obj;
            return this == p;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ x;
                result = (result * 397) ^ y;
                return result;
            }
        }



    }

    class Boxes
    {
        public enum BoxType { light, round, normal, doubled, dashed, dashedLight};
        public static readonly Char cor_b_lu = '╔';
        public static readonly Char cor_b_ru = '╗';
        public static readonly Char cor_b_rl = '╝';
        public static readonly Char cor_b_ll = '╚';
        public static readonly Char cor_b_notd = '╩';
        public static readonly Char cor_b_notu = '╦';
        public static readonly Char cor_b_notl = '╠';
        public static readonly Char cor_b_notr = '╣';
        public static readonly Char cor_b_cross = '╬';
        public static readonly Char wal_b_h = '║';
        public static readonly Char wal_b_v = '═';

        public static readonly Char cor_lu = '┏';
        public static readonly Char cor_ru = '┓';
        public static readonly Char cor_rl = '┛';
        public static readonly Char cor_ll = '┗';
        public static readonly Char cor_notd = '┻';
        public static readonly Char cor_notu = '┳';
        public static readonly Char cor_notl = '┣';
        public static readonly Char cor_notr = '┫';
        public static readonly Char cor_cross = '╋';
        public static readonly Char wal_h = '┃';
        public static readonly Char wal_v = '━';

        public static readonly Char[] doubleBoxChars =      { '╗', ' ', '═', '╔', '╦', ' ', '╝', '║', '╣', '╚', '╩', '╠', '╬' };
        public static readonly Char[] boxChars =            { '┓', ' ', '━', '┏', '┳', ' ', '┛', '┃', '┫', '┗', '┻', '┣', '╋' };
        public static readonly Char[] lightBoxChars =       { '┐', ' ', '─', '┌', '┬', ' ', '┘', '│', '┤', '└', '┴', '├', '┼' };
        public static readonly Char[] roundBoxChars =       { '╮', ' ', '─', '╭', '┬', ' ', '╯', '│', '┤', '╰', '┴', '├', '┼' };
        public static readonly Char[] dashedLightBoxChars = { '┐', ' ', '┄', '┌', '┬', ' ', '┘', '┊', '┤', '└', '┴', '├', '┼' };
        public static readonly Char[] dashedBoxChars =      { '┓', ' ', '┅', '┏', '┳', ' ', '┛', '┋', '┫', '┗', '┻', '┣', '╋' };


        public static Char GetBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return boxChars[id];
        }
        public static Char GetDoubleBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return doubleBoxChars[id];
        }
        public static Char GetLightBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return lightBoxChars[id];
        }
        public static Char GetDashedBoxChar(int id)
        {
            id -= 3;
            if (id < 0 || id > 12)
                return ' ';
            return dashedBoxChars[id];
        }


        /*
        protected char GetBorderChar(Int2 pos, char[] boxes)
        {
            int id = 0;
            if (pos.x > 0 && borderMap[pos.x - 1, pos.y])
                id += 1;
            if (pos.y < Size.y - 1 && borderMap[pos.x, pos.y + 1])
                id += 2;
            if (pos.x < Size.x - 1 && borderMap[pos.x + 1, pos.y])
                id += 4;
            if (pos.y > 0 && borderMap[pos.x, pos.y - 1])
                id += 8;
            id -= 3;
            return boxes[id];
        }
        */

        public static Char[] GetBoxArray(BoxType boxType)
        {
            Char[] bs;
            switch (boxType)
            {
                case BoxType.dashed:
                    bs = dashedBoxChars;
                    break;
                case BoxType.dashedLight:
                    bs = dashedLightBoxChars;
                    break;
                case BoxType.doubled:
                    bs = doubleBoxChars;
                    break;
                case BoxType.light:
                    bs = lightBoxChars;
                    break;
                case BoxType.round:
                    bs = roundBoxChars;
                    break;
                default:
                    bs = boxChars;
                    break;
            }
            return bs;
        }

        public static void DrawBox(BoxType boxType, int x, int y, int sx, int sy)
        {
            DrawBox(GetBoxArray(boxType), x, y, sx, sy);
        }

        public static void DrawBox(Char[] bs, int x, int y, int sx, int sy)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"{bs[3]}{"".PadLeft(sx - 2, bs[2])}{bs[0]}");
            string midBorder = $"{bs[7]}{"".PadLeft(sx - 2)}{bs[7]}";
            for (int i = 1; i < sy - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(midBorder);

            }
            Console.SetCursorPosition(x, y + sy - 1);
            Console.Write($"{bs[9]}{"".PadLeft(sx - 2, bs[2])}{bs[6]}");
            
        }

        public static string GetBoxName(string name, BoxType boxType)
        {
            return GetBoxName(name, GetBoxArray(boxType));
        }

        public static string GetBoxName(string name, Char[] boxes)
        {
            return $"{boxes[8]}{name}{boxes[11]}";
        }
    }

    class Bars
    {
        public static readonly Char b_0 = '▏';
        public static readonly Char b_1 = '▎';
        public static readonly Char b_2 = '▍';
        public static readonly Char b_3 = '▌';
        public static readonly Char b_4 = '▋';
        public static readonly Char b_5 = '▊';
        public static readonly Char b_6 = '▉';
        public static readonly Char b_7 = '█';
        public static readonly Char[] bars = { '▏', '▎', '▍', '▌', '▋', '▊', '▉', '█' };
        public static readonly Char bv_0 = '▁';
        public static readonly Char bv_1 = '▂';
        public static readonly Char bv_2 = '▃';
        public static readonly Char bv_3 = '▄';
        public static readonly Char bv_4 = '▅';
        public static readonly Char bv_5 = '▆';
        public static readonly Char bv_6 = '▇';
        public static readonly Char bv_7 = '█';
        public static readonly Char[] barsVertical = { '▁', '▂', '▃', '▄', '▅', '▆', '▇', '█' };
        public static readonly Char t0 = ' ';
        public static readonly Char t1 = '░';
        public static readonly Char t2 = '▒';
        public static readonly Char t3 = '▓';
        public static readonly Char t4 = '█';
        public static readonly Char[] transparent = { ' ', '░', '▒', '▓', '█'};


        public static Char GetBarSegment(int id)
        {
            if (id < 0 || id > 7)
            {
                return ' ';
            }
            return bars[id];
        }
        public static Char GetVerticalBarSegment(int id)
        {
            if (id < 0 || id > 7)
            {
                return ' ';
            }
            return barsVertical[id];
        }
    }
}
