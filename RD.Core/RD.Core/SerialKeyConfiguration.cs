using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDCore
{
    public class SerialKeyConfiguration : BaseConfiguration
    {
        private bool[] _Features = new bool[8];
        private bool _addSplitChar = true;

        public virtual bool[] Features
        {
            get
            {
                return this._Features;
            }
            set
            {
                this._Features = value;
            }
        }

        public bool addSplitChar
        {
            get
            {
                return this._addSplitChar;
            }
            set
            {
                this._addSplitChar = value;
            }
        }
    }
}
