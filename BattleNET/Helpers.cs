using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace BattleNET
{
    class Helpers
    {
        public static string Hex2Ascii(string hexString)
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

        public class EnumUtils
        {
            public static string StringValueOf(Enum value)
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
                else
                {
                    return value.ToString();
                }
            }

            public static object EnumValueOf(string value, Type enumType)
            {
                string[] names = Enum.GetNames(enumType);
                foreach (string name in names)
                {
                    if (StringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
                    {
                        return Enum.Parse(enumType, name);
                    }
                }

                throw new ArgumentException("The string is not a description or value of the specified enum.");
            }
        }
    }
}
