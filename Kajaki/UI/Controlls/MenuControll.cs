using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kajaki
{
    class MenuControll
    {
        protected string name;
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NeedsRedraw = true;
                PrintableText = $"{name}";
            }
        }
        public string Identificator { get; protected set; }
        public string PrintableText { get; protected set; }
        public Menu parentMenu { get; protected set; }
        public List<Func<MenuEvent, bool>> actions;
        public bool accessable;
        public bool visable;
        public bool NeedsRedraw { get; set; }
        public MenuControll(string name, string identificator)
        {
            NeedsRedraw = true;
            visable = true;
            accessable = true;
            actions = new List<Func<MenuEvent, bool>>();
            Name = name;
            Identificator = identificator;

        }

        public virtual void AddAction(Func<MenuEvent, bool> action)
        {
            actions.Add(action);
        }
        public virtual void AddAction(IEnumerable<Func<MenuEvent, bool>> action)
        {
            actions.AddRange(action);
        }

        public virtual void SetParent(Menu menu)
        {
            parentMenu = menu;
        }

        public virtual void SwitchLeft()
        {
            NeedsRedraw = true;
            //RunActions();
        }
        public virtual void SwitchRight()
        {
            NeedsRedraw = true;
            //RunActions();
        }
        public virtual void Enter()
        {
            NeedsRedraw = true;
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
}
