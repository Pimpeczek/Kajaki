namespace Kajaki
{
    class MenuEvent
    {
        public string Name { get; protected set; }
        public Menu Menu { get; protected set; }
        public MenuEvent(Menu menu)
        {
            Menu = menu;
        }
    }
}