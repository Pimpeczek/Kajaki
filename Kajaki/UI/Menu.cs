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
        public enum Wrapping { wrapping, scrolling};
        protected Wrapping verticalTextWrapping;
        public Wrapping VerticalTextWrapping
        {
            get
            {
                return verticalTextWrapping;
            }
            set
            {
                if (verticalTextWrapping == value)
                    return;
                Erase();
                if (verticalTextWrapping == Wrapping.wrapping)
                {
                    Size = new Int2(Size.x, controlls.Count + 2);
                }
                verticalTextWrapping = value;
                Draw();
            }
        }
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
        int hPoint;
        int scrollPoint;
        string emptyLine;
        List<(ConsoleKey, Func<MenuEvent, bool>)> bindings;
        public bool IsVIsable { get; protected set; }
        public Menu()
        {
            IsVIsable = false;
            Setup(new Int2(), new Int2(10, 10), "Menu", Boxes.BoxType.doubled);
            IsVIsable = true;
        }

        public Menu(Int2 position, Int2 size, string name, Boxes.BoxType boxType)
        {
            Setup(position, size, name, boxType);
        }

        void Setup(Int2 position, Int2 size, string name, Boxes.BoxType boxType)
        {
            waitForInput = false;
            controlls = new List<MenuControll>();
            bindings = new List<(ConsoleKey, Func<MenuEvent, bool>)>();
            hPoint = 0;
            scrollPoint = 0;
            Position = position;
            Size = size;
            Name = name;
            emptyLine = Stringer.GetFilledString(size.x - 2, ' ');
            this.boxType = boxType;
        }

        

        public void AddControll(MenuControll switcherOption)
        {
            for (int i = 0; i < controlls.Count; i++)
            {
                if (controlls[i].Identificator == switcherOption.Identificator)
                {
                    controlls[i] = switcherOption;
                    return;
                }
            }
            switcherOption.SetParent(this);
            controlls.Add(switcherOption);
            if (verticalTextWrapping == Wrapping.wrapping)
            {
                Size = new Int2(Size.x, controlls.Count + 2);
            }
        }

        public int GetValue(string identificator)
        {
            for (int i = 0; i < controlls.Count; i++)
            {
                if (controlls[i].Identificator == identificator)
                {
                    return controlls[i].GetValue();
                }
            }
            throw new Exception("NoSuchIdentificator");
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
            hPoint = -1;
            LoopToAccesable(1);
            DrawControlls();

            //Renderer.Write($"{controlls[0].Name} {controlls[0].GetValue()} {((IntSwitcherControll)controlls[0]).Min} {((IntSwitcherControll)controlls[0]).Max}", new Int2(41, 1));
            do
            {
                response = Console.ReadKey(true).Key;
                switch (response)
                {
                    case ConsoleKey.UpArrow:
                        LoopToAccesable(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        LoopToAccesable(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (hPoint < controlls.Count)
                        {
                            controlls[hPoint].SwitchLeft();
                        }
                        else
                            LoopToAccesable(-1);
                        break;
                    case ConsoleKey.RightArrow:
                        if (hPoint < controlls.Count)
                        {
                            controlls[hPoint].SwitchRight();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                    case ConsoleKey.Enter:
                        if (hPoint < controlls.Count)
                        {
                            controlls[hPoint].Enter();
                        }
                        else
                            LoopToAccesable(1);
                        break;
                }



                DrawControlls();

            } while (waitForInput);

            Erase();
        }

        void LoopToAccesable(int direction)
        {
            int overflowCounter = 0;
            do
            {
                hPoint += direction;
                if(hPoint < 0)
                {
                    hPoint = controlls.Count - 1;
                }
                else if( hPoint >= controlls.Count)
                {
                    hPoint = 0;
                }
                overflowCounter++;
            } while (!controlls[hPoint].accessable && overflowCounter < controlls.Count);
            if (overflowCounter >= controlls.Count)
                hPoint = 0;
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
                if (controlls[i + scrollPoint].visable)
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
            }

        }

        public MenuControll GetControll(string identificator)
        {
            for(int i = 0; i < controlls.Count; i++)
            {
                if (identificator == controlls[i].Identificator)
                    return controlls[i];
            }
            return null;
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
