using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Pastel;

namespace Kajaki
{
    class Switcher
    {
        public Int2 Position { get; protected set; }
        public Int2 Size { get; protected set; }
        public string Name { get; protected set; }
        Boxes.BoxType boxType;
        List<SwitcherOption> options;
        List<(string, int)> bottomButtons;
        int hPoint;
        int scrollPoint;
        string emptyLine;
        List<Func<Switcher, SwitcherOption, bool>> actions;
        public Switcher(Int2 position, Int2 size, string name, Boxes.BoxType boxType)
        {
            options = new List<SwitcherOption>();
            actions = new List<Func<Switcher, SwitcherOption, bool>>();
            bottomButtons = new List<(string, int)>();
            hPoint = 0;
            scrollPoint = 0;
            Position = position;
            Size = size;
            Name = name;
            emptyLine = Misc.GetFilledString(size.x - 2, ' ');
            this.boxType = boxType;
        }

        public void AddChangeAction(Func<Switcher, SwitcherOption, bool> action)
        {
            actions.Add(action);
        }

        public void AddOption(SwitcherOption switcherOption)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].identificator == switcherOption.identificator)
                {
                    options[i] = switcherOption;
                    return;
                }
            }
            options.Add(switcherOption);
        }

        public int GetValue(string identificator)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].identificator == identificator)
                {
                    return options[i].GetValue();
                }
            }
            throw new Exception("NoSuchIdentificator");
        }

        public void AddButton(string name, int returnVal)
        {
            bottomButtons.Add((name, returnVal));
        }

        public int WaitForInput()
        {
            ConsoleKey response;
            Draw();
            DrawOptions();
            Renderer.Write($"{options[0].name} {options[0].GetValue()} {((IntSwitcherOption)options[0]).Min} {((IntSwitcherOption)options[0]).Max}", new Int2(41, 1));
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
                    case ConsoleKey.LeftArrow:
                        if (hPoint < options.Count)
                        {
                            options[hPoint].SwitchLeft();
                            RunActions();
                        }
                        else
                            hPoint--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (hPoint < options.Count)
                        {
                            options[hPoint].SwitchRight();
                            RunActions();
                        }
                        else
                            hPoint++;
                        break;
                }
                if (hPoint < 0)
                    hPoint += (options != null && options.Count > 0 ? options.Count : 1);
                hPoint %= (options != null && options.Count > 0 ? options.Count + bottomButtons.Count : 1);


                DrawOptions();

            } while (response != ConsoleKey.Enter || hPoint < options.Count);
            return bottomButtons[hPoint - options.Count].Item2;
        }

        void RunActions()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Invoke(this, options[hPoint]);
            }
        }

        public void Draw()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Boxes.DrawBox(boxType, Position.x, Position.y, Size.x, Size.y);
            Console.Write(Name, Position.x + (Size.x - Name.Length) / 2, Position.y);

        }


        void DrawOptions()
        {
            int startHeight = Arit.Clamp((Size.y - 2 - options.Count) / 2, 0, Size.y);
            string printText;
            Int2 pos;
            for (int i = 0; i < options.Count && i < Size.y - 2; i++)
            {
                printText = options[i + scrollPoint].PrintableText;
                pos = new Int2(Position.x + 1, Position.y + startHeight + i + 1);
                Renderer.Write(emptyLine, pos);
                pos = new Int2(Position.x + (Size.x - printText.Length) / 2, Position.y + startHeight + i + 1);
                if (hPoint == i)
                {
                    Renderer.Write(printText.PastelBg(Color.Gray), pos);
                }
                else
                {
                    Renderer.Write(printText, pos);
                }
            }
            int leftDist = 1;
            for (int i = 0; i < bottomButtons.Count; i++)
            {
                pos = new Int2(Position.x + leftDist, Position.y + Size.y - 1);
                printText = $"[{bottomButtons[i].Item1}]";
                if (hPoint - options.Count == i)
                {
                    Renderer.Write(printText.PastelBg(Color.Gray), pos);
                }
                else
                {
                    Renderer.Write(printText, pos);
                }
                leftDist += printText.Length;
            }

        }
    }

    class SwitcherOption
    {
        public string name;
        public string identificator;
        public string PrintableText { get; protected set; }


        public SwitcherOption(string name, string identificator)
        {
            this.name = name;
            this.identificator = identificator;
            PrintableText = $"{name}: <?>";
        }

        public virtual void SwitchLeft()
        {

        }
        public virtual void SwitchRight()
        {

        }

        public virtual int GetValue()
        {
            return 0;
        }

    }

    class IntSwitcherOption : SwitcherOption
    {
        private int value;
        public int Value
        {
            get
            {
                return value;
            }
            set
            {

                this.value = Arit.Clamp(value, min, max);
                PrintableText = $"{name}: {(this.value > min ? '<' : '|')} {this.value} {(this.value < max ? '>' : '|')}";
            }
        }

        private int min;
        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                this.min = Arit.Clamp(value, int.MinValue, max);
            }
        }

        private int max;
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                this.max = Arit.Clamp(value, min, int.MaxValue);
            }
        }

        public int Step { get; set; }

        public IntSwitcherOption(string name, string identificator, int value, int min, int max, int step) : base(name, identificator)
        {
            if (min > max)
                min = max;
            this.min = min;
            this.max = max;
            Value = value;
            Step = step;
        }

        override public void SwitchLeft()
        {
            Value -= Step;
        }
        override public void SwitchRight()
        {
            Value += Step;
        }

        override public int GetValue()
        {
            return Value;
        }
    }

    class StringSwitcherOption : SwitcherOption
    {
        private int value;
        public int Value
        {
            get
            {
                return value;
            }
            set
            {

                this.value = Arit.Clamp(value, min, max);
                PrintableText = $"{name}: {(this.value > min ? '<' : '|')} {options[this.value]} {(this.value < max ? '>' : '|')}";
            }
        }
        string[] options;
        private int min;
        private int max;

        public int step;

        public StringSwitcherOption(string name, string identificator, string options) : base(name, identificator)
        {

            max = min = 0;
            SetOptions(options);
            Value = value;
            step = 1;
        }

        public void SetOptions(string options)
        {
            this.options = options.Split('\n');
            max = this.options.Length - 1;
            Value = value;

        }

        override public void SwitchLeft()
        {
            Value -= step;
        }
        override public void SwitchRight()
        {
            Value += step;
        }

        override public int GetValue()
        {
            return Value;
        }
    }
}
