using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RDCore
{
    public class ValidateSerialKey : BaseConfiguration
    {
        private SerialKeyConfiguration skc = new SerialKeyConfiguration();
        private methods _a = new methods();
        private string _secretPhase = "";
        private string _res = "";

        public new string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._res = "";
                this._key = value;
            }
        }

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
                this._secretPhase = this._a.twentyfiveByteHash(value);
            }
        }

        public bool IsValid
        {
            get
            {
                return this._IsValid();
            }
        }

        public bool IsExpired
        {
            get
            {
                return this._IsExpired();
            }
        }

        public DateTime CreationDate
        {
            get
            {
                return this._CreationDay();
            }
        }

        public int DaysLeft
        {
            get
            {
                return this._DaysLeft();
            }
        }

        public int SetTime
        {
            get
            {
                return this._SetTime();
            }
        }

        public DateTime ExpireDate
        {
            get
            {
                return this._ExpireDate();
            }
        }

        public bool[] Features
        {
            get
            {
                return this._Features();
            }
        }

        public bool IsOnRightMachine
        {
            get
            {
                return Convert.ToInt32(this._res.Substring(23, 5)) == this.MachineCode;
            }
        }

        public ValidateSerialKey()
        {
        }

        public ValidateSerialKey(SerialKeyConfiguration _serialKeyConfiguration)
        {
            this.skc = _serialKeyConfiguration;
        }

        private void decodeKeyToString()
        {
            if (!(string.IsNullOrEmpty(this._res) | this._res == null))
                return;
            string str = "";
            this.Key = this.Key.Replace("-", "");
            str = this.Key;
            string key = this.Key;
            if (!string.IsNullOrEmpty(this.secretPhase) | this.secretPhase != null && new Regex("^\\d$").IsMatch(this.secretPhase))
                throw new ArgumentException("The secretPhase consist of non-numerical letters.");
            this._res = this._a._decrypt(key, this.secretPhase);
        }

        private bool _IsValid()
        {
            try
            {
                if (this.Key.Contains("-"))
                {
                    if (this.Key.Length != 23)
                        return false;
                }
                else if (this.Key.Length != 20)
                    return false;
                this.decodeKeyToString();
                return this._res.Substring(0, 9) == this._a.getEightByteHash(this._res.Substring(9, 19), 1000000000).ToString().Substring(0, 9);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool _IsExpired()
        {
            return this.DaysLeft <= 0;
        }

        private DateTime _CreationDay()
        {
            this.decodeKeyToString();
            DateTime dateTime = new DateTime();
            dateTime = new DateTime(Convert.ToInt32(this._res.Substring(9, 4)), Convert.ToInt32(this._res.Substring(13, 2)), Convert.ToInt32(this._res.Substring(15, 2)));
            return dateTime;
        }

        private int _DaysLeft()
        {
            this.decodeKeyToString();
            int setTime = this.SetTime;
            return Convert.ToInt32((this.ExpireDate - DateTime.Today).TotalDays);
        }

        private int _SetTime()
        {
            this.decodeKeyToString();
            return Convert.ToInt32(this._res.Substring(17, 3));
        }

        private DateTime _ExpireDate()
        {
            this.decodeKeyToString();
            DateTime dateTime = new DateTime();
            return this.CreationDate.AddDays((double)this.SetTime);
        }

        private bool[] _Features()
        {
            this.decodeKeyToString();
            return this._a.intToBoolean(Convert.ToInt32(this._res.Substring(20, 3)));
        }
    }
}
