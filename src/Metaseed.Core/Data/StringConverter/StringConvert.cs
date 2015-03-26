using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Data
{
    public static class StringConvert
    {
        public static UInt16 ToUInt16(string str)
        {
            try
            {
                str = str.Trim();
                if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
                {
                    var str1 = str.TrimEnd(new char[] { 'H', 'h' });
                    if (str1.Length == 0)
                    {
                        return 0;
                    }
                    return Convert.ToUInt16(str1, 16);
                    //return Convert.ToUInt16(str.TrimEnd(new char[] { 'H', 'h' }), 16);
                }
                else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                {
                    return Convert.ToUInt16(str.Remove(0, 2), 16);
                }
                else
                {
                    return Convert.ToUInt16(str, 10);
                }
            }
            catch (Exception)
            {
                
                throw;
            }

        }
        public static UInt32 ToUInt32(string str)
        {
            str = str.Trim();
            if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
            {
                var str1 = str.TrimEnd(new char[] { 'H', 'h' });
                if (str1.Length == 0)
                {
                    return 0;
                }
                return Convert.ToUInt32(str1, 16);
                //return Convert.ToUInt32(str.TrimEnd(new char[] { 'H', 'h' }), 16);
            }
            else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToUInt32(str.Remove(0, 2), 16);
            }
            else
            {
                return Convert.ToUInt32(str, 10);
            }
        }
        public static Byte ToByte(string str)
        {
            str = str.Trim();
            if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
            {
                var str1=str.TrimEnd(new char[] { 'H', 'h' });
                if (str1.Length==0)
                {
                    return 0;
                }
                return Convert.ToByte(str1, 16);
            }
            else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                string a = str.Remove(0, 2);
                // string b=str.TrimStart(new char[] { '0', 'x', 'X' });
                return Convert.ToByte(a, 16);
            }
            else
            {
                return Convert.ToByte(str, 10);
            }
        }
        public static SByte ToSByte(string str)
        {
            str = str.Trim();
            if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
            {
                var str1 = str.TrimEnd(new char[] { 'H', 'h' });
                if (str1.Length == 0)
                {
                    return 0;
                }
                return Convert.ToSByte(str1, 16);
                //return Convert.ToSByte(str.TrimEnd(new char[] { 'H', 'h' }), 16);
            }
            else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToSByte(str.Remove(0, 2), 16);
            }
            else
            {
                return Convert.ToSByte(str, 10);
            }
        }
        public static Int16 ToInt16(string str)
        {
            str = str.Trim();
            if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
            {
                var str1 = str.TrimEnd(new char[] { 'H', 'h' });
                if (str1.Length == 0)
                {
                    return 0;
                }
                return Convert.ToInt16(str1, 16);
               // return Convert.ToInt16(str.TrimEnd(new char[] { 'H', 'h' }), 16);
            }
            else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToInt16(str.Remove(0, 2), 16);
            }
            else
            {
                return Convert.ToInt16(str, 10);
            }
        }
        public static Int32 ToInt32(string str)
        {
            str = str.Trim();
            if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
            {
                var str1 = str.TrimEnd(new char[] { 'H', 'h' });
                if (str1.Length == 0)
                {
                    return 0;
                }
                return Convert.ToInt32(str1, 16);
                //return Convert.ToInt32(str.TrimEnd(new char[] { 'H', 'h' }), 16);
            }
            else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToInt32(str.Remove(0, 2), 16);
            }
            else
            {
                return Convert.ToInt32(str, 10);
            }
        }
        public static Int64 ToInt64(string str)
        {
            str = str.Trim();
            if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
            {
                var str1 = str.TrimEnd(new char[] { 'H', 'h' });
                if (str1.Length == 0)
                {
                    return 0;
                }
                return Convert.ToInt64(str1, 16);
                //return Convert.ToInt64(str.TrimEnd(new char[] { 'H', 'h' }), 16);
            }
            else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToInt64(str.Remove(0, 2), 16);
            }
            else
            {
                return Convert.ToInt64(str, 10);
            }
        }
        public static UInt64 ToUInt64(string str)
        {
            str = str.Trim();
            if (str.EndsWith("H", StringComparison.CurrentCultureIgnoreCase))
            {
                var str1 = str.TrimEnd(new char[] { 'H', 'h' });
                if (str1.Length == 0)
                {
                    return 0;
                }
                return Convert.ToUInt64(str1, 16);
                //return Convert.ToUInt64(str.TrimEnd(new char[] { 'H', 'h' }), 16);
            }
            else if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToUInt64(str.Remove(0, 2), 16);
            }
            else
            {
                return Convert.ToUInt64(str, 10);
            }
        }
        public static Int64 ToInteger(string str, int bytes)
        {
            Int64 v = ToInt64(str);
            Int64 vv = (Int64)(Math.Pow(2, bytes * 8 - 1));
            Int64 min = -vv;
            Int64 max = vv - 1;
            if (v < min || v > max)
            {
                throw new ArgumentOutOfRangeException("the value is out of range");
            }
            else
            {
                return v;
            }
        }
        public static UInt64 ToUInteger(string str, int bytes)
        {
            UInt64 v = ToUInt64(str);
            UInt64 vv = (UInt64)(Math.Pow(2, bytes * 8));
            UInt64 min = 0;
            UInt64 max = vv;
            if (v < min || v > max)
            {
                throw new ArgumentOutOfRangeException("the value is out of range");
            }
            else
            {
                return v;
            }
        }
        public static Byte[] ToByteArray(string str)
        {
            string[] saData = str.Trim().Split(new char[] { ' ', ',' },StringSplitOptions.RemoveEmptyEntries);
            List<Byte> data = new List<byte>();
            for (int i = 0; i < saData.Length; i++)
            {
                if (saData[i] == String.Empty)
                {
                    continue;
                }
                data.Add(StringConvert.ToByte(saData[i]));
            }
            return data.ToArray();
        }
    }
}
