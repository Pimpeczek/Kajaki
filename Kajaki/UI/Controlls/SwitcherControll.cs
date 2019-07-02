using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kajaki
{
    abstract class SwitcherControll : MenuControll
    {

        protected string LAS;
        protected string RAS;
        protected string LNS;
        protected string RNS;

        public string LeftAvlaiableSymbol
        {
            get
            {
                return LAS;
            }
            set
            {
                LAS = value;
                SetPrintableText();
            }
        }

        public string RightAvaliableSymbol
        {
            get
            {
                return RAS;
            }
            set
            {
                RAS = value;
                SetPrintableText();
            }
        }

        public string LeftNonavaliableSymbol
        {
            get
            {
                return LNS;
            }
            set
            {
                LNS = value;
                SetPrintableText();
            }
        }

        public string RightNonavaliableSymbol
        {
            get
            {
                return RNS;
            }
            set
            {
                RNS = value;
                SetPrintableText();
            }
        }



        public SwitcherControll(string name, string identificator) : base(name, identificator)
        {
            LAS = "◄";
            RAS = "►";
            LNS = " ";
            RNS = " ";
        }

        abstract protected void SetPrintableText();
    }
}
