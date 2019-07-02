namespace Kajaki
{
    class IntSwitcherEvent : MenuControllEvent
    {
        public int Value { get; protected set; }
        public IntSwitcherEvent(Menu menu, MenuControll controll, int value) : base(menu, controll)
        {
            Value = value;
        }
    }
}