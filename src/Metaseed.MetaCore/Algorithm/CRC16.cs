using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Alogrithm
{
    public class CRC16
    {
        static readonly UInt16 POLY = unchecked((UInt16)0x11021);
        public static UInt16 Slow_CRC16(UInt16 sum, List<Byte> list, Int32 startIndex, Int32 Length)
        {
            Int32 len = Length;
            Int32 k = startIndex;
            while (len-- != 0)
            {
                int i;
                Byte b = list[k++];//*(p++);
                for (i = 0; i < 8; ++i)
                {
                    UInt32 osum = sum;
                    sum <<= 1;
                    if ((b & 0x80) != 0)
                        sum |= 1;
                    if ((osum & 0x8000) != 0)
                        sum ^= POLY;
                    b <<= 1;
                }
            }
            return sum;
        }
        /// <summary>
        /// calculate the final CRC16 value of the data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        static public UInt16 GetCRC16(List<byte> data, int startIndex, int length) {
            UInt16 chk = Slow_CRC16(0, data, startIndex, length);
            List<byte> zeros = new List<byte> { 0, 0 };
            UInt16 sum = Slow_CRC16(chk, zeros, 0, 2);
            return sum;
        }
    }
}
