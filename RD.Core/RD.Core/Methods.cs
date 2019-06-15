using System;
using System.Numerics;
using System.Text;

namespace RDCore
{
    internal class methods : SerialKeyConfiguration
    {
        protected internal string _encrypt(int _days, bool[] _tfg, string _secretPhase, int ID, DateTime _creationDate)
        {
            Decimal num = (((new Decimal(0) + (Decimal)Convert.ToInt32(_creationDate.ToString("yyyyMMdd"))) * new Decimal(1000) + (Decimal)_days) * new Decimal(1000) + (Decimal)this.booleanToInt(_tfg)) * new Decimal(100000) + (Decimal)ID;
            if (string.IsNullOrEmpty(_secretPhase) | _secretPhase == null)
                return this.base10ToBase26(this.getEightByteHash(num.ToString(), 1000000000).ToString() + num.ToString());
            return this.base10ToBase26(this.getEightByteHash(num.ToString(), 1000000000).ToString() + this._encText(num.ToString(), _secretPhase));
        }

        protected internal string _decrypt(string _key, string _secretPhase)
        {
            if (string.IsNullOrEmpty(_secretPhase) | _secretPhase == null)
                return this.base26ToBase10(_key);
            string base10 = this.base26ToBase10(_key);
            return base10.Substring(0, 9) + this._decText(base10.Substring(9), _secretPhase);
        }

        protected internal int booleanToInt(bool[] _booleanArray)
        {
            int num = 0;
            for (int index = 0; index < _booleanArray.Length; ++index)
            {
                if (_booleanArray[index])
                    num += Convert.ToInt32(Math.Pow(2.0, (double)(_booleanArray.Length - index - 1)));
            }
            return num;
        }

        protected internal bool[] intToBoolean(int _num)
        {
            string str = this.Return_Lenght(Convert.ToInt32(Convert.ToString(_num, 2)).ToString(), 8);
            bool[] flagArray = new bool[8];
            for (int startIndex = 0; startIndex <= 7; ++startIndex)
                flagArray[startIndex] = str.ToString().Substring(startIndex, 1) == "1";
            return flagArray;
        }

        protected internal string _encText(string _inputPhase, string _secretPhase)
        {
            string str = "";
            for (int index = 0; index <= _inputPhase.Length - 1; ++index)
                str += Convert.ToString((object)this.modulo(Convert.ToInt32(_inputPhase.Substring(index, 1)) + Convert.ToInt32(_secretPhase.Substring(this.modulo(index, _secretPhase.Length), 1)), 10));
            return str;
        }

        protected internal string _decText(string _encryptedPhase, string _secretPhase)
        {
            string str = "";
            for (int index = 0; index <= _encryptedPhase.Length - 1; ++index)
                str += Convert.ToString((object)this.modulo(Convert.ToInt32(_encryptedPhase.Substring(index, 1)) - Convert.ToInt32(_secretPhase.Substring(this.modulo(index, _secretPhase.Length), 1)), 10));
            return str;
        }

        protected internal string Return_Lenght(string Number, int Lenght)
        {
            if (Number.ToString().Length != Lenght)
            {
                while (Number.ToString().Length != Lenght)
                    Number = "0" + Number;
            }
            return Number;
        }

        protected internal int modulo(int _num, int _base)
        {
            return _num - _base * Convert.ToInt32(Math.Floor((Decimal)_num / (Decimal)_base));
        }

        protected internal string twentyfiveByteHash(string s)
        {
            int num = s.Length / 5;
            string[] strArray = new string[num + 1];
            if (s.Length <= 5)
                strArray[0] = this.getEightByteHash(s, 1000000000).ToString();
            else if (s.Length > 5)
            {
                for (int index = 0; index <= num - 2; ++index)
                    strArray[index] = this.getEightByteHash(s.Substring(index * 5, 5), 1000000000).ToString();
                strArray[strArray.Length - 2] = this.getEightByteHash(s.Substring((strArray.Length - 2) * 5, s.Length - (strArray.Length - 2) * 5), 1000000000).ToString();
            }
            return string.Join("", strArray);
        }

        protected internal int getEightByteHash(string s, int MUST_BE_LESS_THAN = 1000000000)
        {
            uint num1 = 0;
            foreach (byte @byte in Encoding.Unicode.GetBytes(s))
            {
                uint num2 = num1 + (uint)@byte;
                uint num3 = num2 + (num2 << 10);
                num1 = num3 ^ num3 >> 6;
            }
            uint num4 = num1 + (num1 << 3);
            uint num5 = num4 ^ num4 >> 11;
            int num6 = (int)((long)(num5 + (num5 << 15)) % (long)MUST_BE_LESS_THAN);
            int num7 = MUST_BE_LESS_THAN / num6;
            if (num7 > 1)
                num6 *= num7;
            return num6;
        }

        protected internal string base10ToBase26(string s)
        {
            char[] charArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Decimal num = Convert.ToDecimal(s);
            char[] chArray = new char[s.ToString().Length + 1];
            int index1 = 0;
            while (num >= new Decimal(26))
            {
                int int32 = Convert.ToInt32(num % new Decimal(26));
                chArray[index1] = charArray[int32];
                num = (num - (Decimal)int32) / new Decimal(26);
                ++index1;
            }
            chArray[index1] = charArray[Convert.ToInt32(num)];
            string str = "";
            for (int index2 = index1; index2 >= 0; --index2)
                str += Convert.ToString((object)chArray[index2]);
            return str;
        }

        protected internal string base26ToBase10(string s)
        {
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            BigInteger bigInteger1 = new BigInteger();
            for (int startIndex = 0; startIndex <= s.Length - 1; ++startIndex)
            {
                BigInteger bigInteger2 = this.powof(26, s.Length - startIndex - 1);
                bigInteger1 += (BigInteger)str.IndexOf(s.Substring(startIndex, 1)) * bigInteger2;
            }
            return bigInteger1.ToString();
        }

        protected internal BigInteger powof(int x, int y)
        {
            BigInteger bigInteger = (BigInteger)1;
            if (y == 0)
                return (BigInteger)1;
            if (y == 1)
                return (BigInteger)x;
            for (int index = 0; index <= y - 1; ++index)
                bigInteger *= (BigInteger)x;
            return bigInteger;
        }
    }
}
