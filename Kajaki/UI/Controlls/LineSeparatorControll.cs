namespace Kajaki
{
    class LineSeparatorControll : MenuControll
    {

        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = "";
                PrintableText = "";
            }
        }

        public LineSeparatorControll(string name, string identificator) : base(name, identificator)
        {
            accessable = false;
            visable = false;
        }

        override public void SwitchLeft() { }
        override public void SwitchRight() { }
        override public void Enter() { }

        override public int GetValue() { return 0; }
    }
}