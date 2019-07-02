using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kajaki
{
    class MenuControllEvent : MenuEvent
    {
        public MenuControll Controll { get; protected set; }
        public MenuControllEvent(Menu menu, MenuControll controll) : base(menu)
        {
            Controll = controll;
        }
    }
}
