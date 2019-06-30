using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Pastel;

namespace Kajaki
{
    class Menu
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
        public bool waitForInput;
        Boxes.BoxType boxType;
        List<MenuControll> controlls;
        List<(string, int)> bottomButtons;
        int hPoint;
        int scrollPoint;
        string emptyLine;
        List<(ConsoleKey, Func<MenuEvent, bool>)> bindings;
        public bool IsVIsable { get; protected set; }
        public Menu(Int2 position, Int2 size, string name, Boxes.BoxType boxType)
        {
            waitForInput = false;
            controlls = new List<MenuControll>();
            bindings = new List<(ConsoleKey, Func<MenuEvent, bool>)>();
            bottomButtons = new List<(string, int)>();
            hPoint = 0;
            scrollPoint = 0;
            Position = position;
            Size = size;
            Name = name;
            emptyLine = Misc.GetFilledString(size.x - 2, ' ');
            this.boxType = boxType;
        }

        

        public void AddControll(MenuControll switcherOption)
        {
            for (int i = 0; i < controlls.Count; i++)
            {
                if (controlls[i].identificator == switcherOption.identificator)
                {
                    controlls[i] = switcherOption;
                    return;
                }
            }
            switcherOption.SetParent(this);
            controlls.Add(switcherOption);
        }

        public int GetValue(string identificator)
        {
            for (int i = 0; i < controlls.Count; i++)
            {
                if (controlls[i].identificator == identificator)
                {
                    return controlls[i].GetValue();
                }
            }
            throw new Exception("NoSuchIdentificator");
        }

        public void AddButton(string name, int returnVal)
        {
            bottomButtons.Add((name, returnVal));
        }

        public bool Exit(MenuEvent e)
        {
            waitForInput = false;
            
            return true;
        }

        public void WaitForInput()
        {
            waitForInput = true;
            ConsoleKey response;
            Draw();
            DrawControlls();
            //Renderer.Write($"{controlls[0].Name} {controlls[0].GetValue()} {((IntSwitcherControll)controlls[0]).Min} {((IntSwitcherControll)controlls[0]).Max}", new Int2(41, 1));
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
                        if (hPoint < controlls.Count)
                        {
                            controlls[hPoint].SwitchLeft();
                        }
                        else
                            hPoint--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (hPoint < controlls.Count)
                        {
                            controlls[hPoint].SwitchRight();
                        }
                        else
                            hPoint++;
                        break;
                    case ConsoleKey.Enter:
                        if (hPoint < controlls.Count)
                        {
                            controlls[hPoint].Enter();
                        }
                        else
                            hPoint++;
                        break;
                }
                if (hPoint < 0)
                    hPoint += (controlls != null && controlls.Count > 0 ? controlls.Count : 1);
                hPoint %= (controlls != null && controlls.Count > 0 ? controlls.Count + bottomButtons.Count : 1);


                DrawControlls();

            } while (waitForInput);

            Erase();
        }

        void RunBindings(ConsoleKey key)
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                if(key == bindings[i].Item1)
                    bindings[i].Item2.Invoke(new MenuBindingEvent(this, key));
            }
        }

        public void Draw()
        {
            IsVIsable = true;
            Console.ForegroundColor = ConsoleColor.White;
            Boxes.DrawBox(boxType, Position.x, Position.y, Size.x, Size.y);
            Renderer.Write(Name, Position.x + (Size.x - Name.Length) / 2, Position.y);

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


        void DrawControlls()
        {
            int startHeight = Arit.Clamp((Size.y - 2 - controlls.Count) / 2, 0, Size.y);
            string printText;
            Int2 pos;
            for (int i = 0; i < controlls.Count && i < Size.y - 2; i++)
            {
                printText = controlls[i + scrollPoint].PrintableText;
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
                if (hPoint - controlls.Count == i)
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

        public MenuControll GetControll(string identificator)
        {
            for(int i = 0; i < controlls.Count; i++)
            {
                if (identificator == controlls[i].identificator)
                    return controlls[i];
            }
            return null;
        }
    }

    class MenuControll
    {
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
                PrintableText = $"{name}";
            }
        }
        public string identificator;
        public string PrintableText { get; protected set; }
        public Menu parentMenu { get; protected set; }
        public List<Func<MenuEvent, bool>> actions;

        public MenuControll(string name, string identificator)
        {
            actions = new List<Func<MenuEvent, bool>>();
            Name = name;
            this.identificator = identificator;
            
        }

        public virtual void AddAction(Func<MenuEvent, bool> action)
        {
            actions.Add(action);
        }
        public virtual void AddAction(IEnumerable< Func<MenuEvent, bool>> action)
        {
            actions.AddRange(action);
        }

        public virtual void SetParent(Menu menu)
        {
            parentMenu = menu;
        }

        public virtual void SwitchLeft()
        {
            //RunActions();
        }
        public virtual void SwitchRight()
        {
            //RunActions();
        }
        public virtual void Enter()
        {
            RunActions();
        }

        protected virtual void RunActions()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Invoke(new MenuControllEvent(parentMenu, this));
            }
        }

        public virtual int GetValue()
        {
            return 0;
        }

    }

    class IntSwitcherControll : MenuControll
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

        public IntSwitcherControll(string name, string identificator, int value, int min, int max, int step) : base(name, identificator)
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
            RunActions();
        }
        override public void SwitchRight()
        {
            Value += Step;
            RunActions();
        }

        public override void Enter(){return;}

        override public int GetValue()
        {
            return Value;
        }
    }

    class StringSwitcherControll : MenuControll
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

        public StringSwitcherControll(string name, string identificator, string options) : base(name, identificator)
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
            RunActions();
        }
        override public void SwitchRight()
        {
            Value += step;
            RunActions();
        }

        public override void Enter() { return; }

        override public int GetValue()
        {
            return Value;
        }
    }

    class CheckBoxControll : MenuControll
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
                this.value = Arit.Clamp(value, 0, 1);
                PrintableText = $"{name}: [{(this.value == 0 ? ' ' : '+')}]";
            }
        }

        public CheckBoxControll(string name, string identificator, bool startValue) : base(name, identificator)
        {
            Value = (startValue? 1 : 0);
        }

        override public void SwitchLeft()
        {
            Toggle();
            RunActions();
        }
        override public void SwitchRight()
        {
            Toggle();
            RunActions();
        }

        public override void Enter()
        {
            Toggle();
            RunActions();
            return;
        }

        void Toggle()
        {
            if (value == 0)
                Value = 1;
            else
                Value = 0;
        }

        override public int GetValue()
        {

            return Value;
        }
    }



    class MenuEvent
    {
        public string Name { get; protected set; }
        public Menu Menu { get; protected set; }
        public MenuEvent(Menu menu)
        {
            Menu = menu;
        }
    }

    class MenuControllEvent : MenuEvent
    {
        public MenuControll Controll { get; protected set; }
        public MenuControllEvent(Menu menu, MenuControll controll) : base(menu)
        {
            Controll = controll;
        }
    }

    class MenuBindingEvent : MenuEvent
    {
        public ConsoleKey Key { get; protected set; }
        public MenuBindingEvent(Menu menu, ConsoleKey key) : base(menu)
        {
            Key = key;
        }
    }
}
