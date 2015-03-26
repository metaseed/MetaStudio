using System;
using System.Runtime.InteropServices;

namespace Metaseed.Data
{
    /// <summary>
    /// raw value data type
    /// </summary>
    public enum SignalDataType
    {
        //Signed Integer of set length (two's complement)
        //(the most significant bit is the sign (+/-) bit)
        //value range: -2^(SigLength-1) to +2^(SigLength-1)-1
        //max SigLength is 64
        Signed,
        //Unsigned Integer of set length 
        //value range: 0 to 2^SigLength
        //max SigLength is 64
        Unsigned,
        //32 Bit IEEE Float
        //value range:-3.4 × 10^38 to +3.4 × 10^38 
        //precision: 7 digits
        IEEE_Float,
        //64 Bit IEEE Double
        //value range: ±5.0 × 10^−324 to 1.7 * 10^308
        //precision: 15-16 digits
        IEEE_Double
    }
    //[StructLayout(LayoutKind.Explicit)]
    //public struct SignalRawValue
    //{
    //    [FieldOffset(0)]
    //    public UInt64 UInt64;
    //    [FieldOffset(0)]
    //    public Int64 Int64;
    //}
    public enum Endianness
    {
        LittleEndian,//intel
        BigEndian//motorola
    }
    public interface ISignalObject : IValueObject
    {
        Endianness Endianness { get; }
        int StartBit { get; }
        int BitLength { get; }
        SignalDataType SignalDataType { get; }
        Int64 RawDefaultValue { get; }
        Boolean IsUsingLowHighLimit { get; }
        Int64 RawValueLowLimit { get; }
        Int64 RawValueHighLimit { get; }
        Double Factor { get; }
        Double Offset { get; }
    }
}
