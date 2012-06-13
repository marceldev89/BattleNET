using System;
using System.Globalization;
using System.Text;

namespace BattleNET
{
    class Helpers
    {
        public static string HexString2Ascii(string hexString)
        {
            byte[] tmp;
            int j = 0;
            tmp = new byte[(hexString.Length) / 2];
            for (int i = 0; i <= hexString.Length - 2; i += 2)
            {
                tmp[j] = (byte)Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber));

                j++;
            }
            return Encoding.GetEncoding(1252).GetString(tmp);
        }
    }
}
