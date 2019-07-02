using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kajaki
{
    class StringSwitcherControll : SwitcherControll
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
                NeedsRedraw = true;
                SetPrintableText();
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

        protected override void SetPrintableText()
        {
            PrintableText = $"{name}: {(value > min ? LAS : LNS)} {options[value]} {(value < max ? RAS : RNS)}";
        }

    }
}
