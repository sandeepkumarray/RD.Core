using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RDCore
{
    public class GenerateSerialKey : BaseConfiguration
    {
        private SerialKeyConfiguration skc = new SerialKeyConfiguration();
        private methods m = new methods();
        private Random r = new Random();
        private string _secretPhase;

        public string secretPhase
        {
            get
            {
                return this._secretPhase;
            }
            set
            {
                if (!(value != this._secretPhase))
                    return;
                this._secretPhase = this.m.twentyfiveByteHash(value);
            }
        }

        public GenerateSerialKey()
        {
        }

        public GenerateSerialKey(SerialKeyConfiguration _serialKeyConfiguration)
        {
            this.skc = _serialKeyConfiguration;
        }

        public string doKey(int timeLeft)
        {
            return this.doKey(timeLeft, DateTime.Today, 0);
        }

        public object doKey(int timeLeft, int useMachineCode)
        {
            return (object)this.doKey(timeLeft, DateTime.Today, useMachineCode);
        }

        public string doKey(int timeLeft, DateTime creationDate, int useMachineCode = 0)
        {
            if (timeLeft > 999)
                throw new ArgumentException("The timeLeft is larger than 999. It can only consist of three digits.");
            if (!string.IsNullOrEmpty(this.secretPhase) | this.secretPhase != null && new Regex("^\\d$").IsMatch(this.secretPhase))
                throw new ArgumentException("The secretPhase consist of non-numerical letters.");
            string str = !(useMachineCode > 0 & useMachineCode <= 99999) ? this.m._encrypt(timeLeft, this.skc.Features, this.secretPhase, this.r.Next(0, 99999), creationDate) : this.m._encrypt(timeLeft, this.skc.Features, this.secretPhase, useMachineCode, creationDate);
            if (this.skc.addSplitChar)
                this.Key = str.Substring(0, 5) + "-" + str.Substring(5, 5) + "-" + str.Substring(10, 5) + "-" + str.Substring(15, 5);
            else
                this.Key = str;
            return this.Key;
        }
    }
}
