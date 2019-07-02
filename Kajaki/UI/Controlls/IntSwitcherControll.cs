namespace Kajaki
{
    class IntSwitcherControll : SwitcherControll
    {
        private int value = 0;
        public int Value
        {
            get
            {
                return value;
            }
            set
            {

                this.value = Arit.Clamp(value, min, max);
                SetPrintableText();
            }
        }

        private int min = 0;
        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                this.min = Arit.Clamp(value, int.MinValue, max);
                SetPrintableText();
            }
        }

        private int max = 10;
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                this.max = Arit.Clamp(value, min, int.MaxValue);
                SetPrintableText();
            }
        }

        private string minSpecialText = "";
        public string MinSpecialText
        {
            get
            {
                return minSpecialText;
            }
            set
            {
                minSpecialText = value;
                SetPrintableText();
            }
        }

        private string maxSpecialText = "";
        public string MaxSpecialText
        {
            get
            {
                return maxSpecialText;
            }
            set
            {
                maxSpecialText = value;
                SetPrintableText();
            }
        }

        public int Step { get; set; }


        public IntSwitcherControll(string name, string identificator) : base(name, identificator)
        {

        }

        public IntSwitcherControll(string name, string identificator, int value, int min, int max, int step) : base(name, identificator)
        {
            Setup(value, min, max, step);
        }

        void Setup(int value, int min, int max, int step)
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

        public override void Enter() { return; }

        override public int GetValue()
        {
            return Value;
        }

        protected override void SetPrintableText()
        {
            string valueText;

            if (value == max)
            {
                valueText = (maxSpecialText == "" ? value.ToString() : maxSpecialText);
            }
            else if (value == min)
            {
                valueText = (minSpecialText == "" ? value.ToString() : minSpecialText);
            }
            else
            {
                valueText = value.ToString();
            }

            PrintableText = $"{name}: {(value > min ? LAS : LNS)} {valueText} {(value < max ? RAS : RNS)}";
        }

        override protected void RunActions()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Invoke(new IntSwitcherEvent(parentMenu, this, value));
            }
        }
    }
}