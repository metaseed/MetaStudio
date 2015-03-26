using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Metaseed.Data
{
    using Metaseed.Data.Contracts;


    public abstract class SignalObject : MetaData_ValueObject, ISignalObject
    {
        protected void ctor(int startBit, int bitLength, SignalDataType dataType, Endianness endian, double factor, double offset)
        {
            _StartBit = startBit;
            _BitLength = bitLength;
            _SignalDataType = dataType;
            _Endianness = endian;
            _Factor = factor;
            _Offset = offset;
            OnSetDataID(string.Empty);
        }

        override public XElement XML
        {
            get
            {
                var x = base.XML;
                x.Add(
                                          new XElement("SignalDataType", SignalDataType),
                                          new XElement("StartBit", StartBit),
                                          new XElement("BitLength", BitLength),
                                          new XElement("Endianness", Endianness),
                                          new XElement("IsUsingLowHighLimit", IsUsingLowHighLimit),
                                          new XElement("Factor", Factor),
                                          new XElement("Offset", Offset),
                                          new XElement("RawValueLowLimit", RawValueLowLimit),
                                          new XElement("RawValueHighLimit", RawValueHighLimit),
                                          new XElement("RawDefaultValue", RawDefaultValue)
                                          );
                return x;
            }
            set
            {
                var sigXml = value;
                var dataType = (SignalDataType)Enum.Parse(typeof(SignalDataType), sigXml.Element("SignalDataType").Value);
                var startBit = Int32.Parse(sigXml.Element("StartBit").Value);
                var bitLength = Int32.Parse(sigXml.Element("BitLength").Value);
                var endian = (Endianness)Enum.Parse(typeof(Endianness), sigXml.Element("Endianness").Value);
                var isUsingLim = Boolean.Parse(sigXml.Element("IsUsingLowHighLimit").Value);
                double factor = Double.Parse(sigXml.Element("Factor").Value);
                double offset = Double.Parse(sigXml.Element("Offset").Value);
                var rawValueHighLimit = Int64.Parse(sigXml.Element("RawValueHighLimit").Value);
                var rawValueLowLimit = Int64.Parse(sigXml.Element("RawValueLowLimit").Value);
                var rawDefaultValue = Int64.Parse(sigXml.Element("RawDefaultValue").Value);
                ctor(startBit, bitLength, dataType, endian, factor, offset);
                IsUsingLowHighLimit = isUsingLim;
                RawValueHighLimit = rawValueHighLimit;
                RawValueLowLimit = rawValueLowLimit;
                RawDefaultValue = rawDefaultValue;
                base.XML = value;
            }
        }

        protected Endianness _Endianness = Endianness.LittleEndian;
        public Endianness Endianness
        {
            get { return _Endianness; }
            set
            {

                if (_Endianness != value)
                {
                    _Endianness = value;
                    OnSetDataID("Endianness");
                    RaisePropertyChanged("Endianness");
                }
            }
        }
        protected SignalDataType _SignalDataType = SignalDataType.Unsigned;
        public SignalDataType SignalDataType
        {
            get { return _SignalDataType; }
            set
            {
                if (_SignalDataType != value)
                {
                    _SignalDataType = value;
                    if (_SignalDataType == Data.SignalDataType.IEEE_Float)
                    {
                        BitLength = 32;
                    }
                    else if (_SignalDataType == SignalDataType.IEEE_Double)
                    {
                        BitLength = 64;
                    }
                    else
                    {
                        updateIntTypeMaxValue();
                    }
                    OnSetDataID("SignalDataType");
                    RaisePropertyChanged("SignalDataType");
                }
            }
        }
        protected int _StartBit = 0;
        public int StartBit
        {
            get { return _StartBit; }
            set
            {
                if (_StartBit != value)
                {
                    _StartBit = value;
                    OnSetDataID("StartBit");
                    RaisePropertyChanged("StartBit");
                }
            }
        }
        protected int _BitLength = 8;
        public int BitLength
        {
            get { return _BitLength; }
            set
            {
                if (_BitLength != value)
                {
                    if (value < 1 || value > 64)
                    {
                        MessageBox.Show("BitLength of SigalObject should be [1-64]");
                        return;
                    }
                    updateIntTypeMaxValue();
                    _BitLength = value;
                    OnSetDataID("BitLength");
                    RaisePropertyChanged("BitLength");
                }
            }
        }
        Int64 _SIntTypeMaxValue = 255;
        UInt64 _UIntTypeMaxValue = 256;
        void updateIntTypeMaxValue()
        {
            if (SignalDataType == Data.SignalDataType.Signed)
            {
                _SIntTypeMaxValue = (Int64)Math.Pow(2, (_BitLength - 1));
            }
            else if (SignalDataType == Data.SignalDataType.Unsigned)
            {
                _UIntTypeMaxValue = (UInt64)Math.Pow(2, _BitLength);
            }
        }

        /// <summary>
        /// the return value not consiger endian, here.
        /// the raw value is represented using Int64;
        /// </summary>
        /// <param name="valueTest"></param>
        /// <returns></returns>
        public Int64 GetInRangeRawValue(double valueTest, out bool isOutRange, bool considerManualSetLimits)
        {
            double value = ((valueTest - _Offset) / _Factor);
            switch (_SignalDataType)
            {
                case SignalDataType.Signed:
                    {
                        var value_s = GetRealRawValueOfSignedType((Int64)value);
                        var d = _SIntTypeMaxValue;
                        var max = d - 1;
                        var min = -d;
                        if (value_s > max)
                        {
                            isOutRange = true;
                            return max;
                        }
                        else if (value_s < min)
                        {
                            isOutRange = true;
                            return min;
                        }
                        else
                        {
                            if (considerManualSetLimits && _IsUsingLowHighLimit)
                            {
                                if (value_s > _RawValueHighLimit)
                                {
                                    isOutRange = true;
                                    return _RawValueHighLimit;
                                }
                                else if (value_s < _RawValueLowLimit)
                                {
                                    isOutRange = true;
                                    return _RawValueLowLimit;
                                }
                            }
                            isOutRange = false;
                            return value_s;
                        }
                        //break;
                    }
                case SignalDataType.Unsigned:
                    {
                        if (value < 0)
                        {
                            isOutRange = true;
                            return 0;
                        }
                        var value_u = (UInt64)value;
                        var max = _UIntTypeMaxValue;
                        if (value_u > max)
                        {
                            isOutRange = true;
                            return (Int64)max;
                        }
                        else
                        {
                            if (considerManualSetLimits && _IsUsingLowHighLimit)
                            {
                                if (value_u > (UInt64)_RawValueHighLimit)
                                {
                                    isOutRange = true;
                                    return _RawValueHighLimit;
                                }
                                else if (value_u < (UInt64)_RawValueLowLimit)
                                {
                                    isOutRange = true;
                                    return _RawValueLowLimit;
                                }
                            }
                            isOutRange = false;
                            return (Int64)value_u;//*(Int64*)&d;
                        }
                        //break;
                    }
                case SignalDataType.IEEE_Float:
                    unsafe
                    {
                        if (value > Single.MaxValue)
                        {
                            var v = Single.MaxValue;
                            isOutRange = true;
                            return *(Int64*)&v;
                        }
                        else if (value < Single.MinValue)
                        {
                            var v = Single.MinValue;
                            isOutRange = true;
                            return *(Int64*)&v;
                        }
                        else
                        {

                            if (considerManualSetLimits && _IsUsingLowHighLimit)
                            {
                                var high = _RawValueHighLimit;
                                var low = _RawValueLowLimit;

                                if (value > *(float*)&high)
                                {
                                    isOutRange = true;
                                    return _RawValueHighLimit;
                                }
                                else if (value < *(float*)&low)
                                {
                                    isOutRange = true;
                                    return _RawValueLowLimit;
                                }

                            }
                            isOutRange = false;
                            return *(Int64*)&value;
                        }
                    }
                //break;
                case SignalDataType.IEEE_Double:
                    unsafe
                    {
                        if (considerManualSetLimits && _IsUsingLowHighLimit)
                        {
                            var high = _RawValueHighLimit;
                            var low = _RawValueLowLimit;

                            if (value > *(double*)&high)
                            {
                                isOutRange = true;
                                return _RawValueHighLimit;
                            }
                            else if (value < *(double*)&low)
                            {
                                isOutRange = true;
                                return _RawValueLowLimit;
                            }

                        }
                        isOutRange = false;
                        return *(Int64*)&value;
                    }
                //break;
                default:
                    throw new Exception("No Such SignalDataType, SignalObject.GetInRangeRawValue");
                //isOutRange = true;
                //return Int64.MinValue;
                // break;
            }

        }
        unsafe public bool SetValue(byte[] Data, double value)
        {
            fixed (byte* p_DataStart = Data)
            {
                bool r = SetValue(p_DataStart, Data.Length, value);
                return r;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_DataStart">pointer to Data Start Location</param>
        /// <param name="DataLength">Byte Length Of the Data</param>
        /// <param name="value"></param>
        /// <returns></returns>
        unsafe public bool SetValue(byte* p_DataStart, int DataLength, double value)
        {
            int lastBitIndex = _StartBit + _BitLength - 1;
            int firstByteIndex = _StartBit / 8;
            int lastByteIndex = lastBitIndex / 8;
            if (_Endianness == Endianness.BigEndian)
            {
                lastByteIndex = firstByteIndex - (lastByteIndex - firstByteIndex);
                if (lastByteIndex < 0)
                {
                    MessageBox.Show("Signal Last Bit In Data Array Out Of Range, SignalObject.SetValue");
                    return false;
                }
            }
            else
            {
                if (lastByteIndex >= DataLength)
                {
                    MessageBox.Show("Signal Last Bit In Data Array Out Of Range, SignalObject.SetValue");
                    return false;
                }
            }
            int firstBitIndexInFirstByte = _StartBit % 8;
            bool isOutRange = false;
            UInt64 rawValue = (UInt64)GetInRangeRawValue(value, out isOutRange, true);
            UInt64 shiftedRawValue = rawValue << firstBitIndexInFirstByte;
            byte* p_ShifedRawValue = (byte*)&shiftedRawValue;
            //first byte
            Byte firstByteValueMask;
            if (lastByteIndex == firstByteIndex)
            {
                //methord 1:
                //firstByteValueMask = (byte)(0xFF - (((2 << (lastBitIndexInLastByte + 1)) - 1) - ((2 << firstBitIndexInFirstByte) - 1)));
                //methord 2:
                firstByteValueMask = (byte)(0xFF - (((1 << _BitLength) - 1) << firstBitIndexInFirstByte));
                p_DataStart[firstByteIndex] = (byte)(p_DataStart[firstByteIndex] & firstByteValueMask | (*p_ShifedRawValue));
                return true;
            }

            firstByteValueMask = (byte)((2 << firstBitIndexInFirstByte) - 1);
            p_DataStart[firstByteIndex] = (byte)(p_DataStart[firstByteIndex] & firstByteValueMask | (*p_ShifedRawValue));
            //middle
            int i = firstByteIndex;
            p_ShifedRawValue++;
            if (_Endianness == Endianness.BigEndian)
            {
                i--;
            }
            else
            {
                i++;
            }
            if (_Endianness == Endianness.BigEndian)
            {
                while (i > lastByteIndex)
                {
                    p_DataStart[i] = *p_ShifedRawValue;
                    p_ShifedRawValue++;
                    i--;
                }
            }
            else
            {
                while (i < lastByteIndex)
                {
                    p_DataStart[i] = *p_ShifedRawValue;
                    p_ShifedRawValue++;
                    i++;
                }
            }

            //int i = firstByteIndex;
            //do
            //{
            //    p_ShifedRawValue++;
            //    if (_Endianness == Endianness.BigEndian)
            //    {
            //        i--;
            //    }
            //    else
            //    {
            //        i++;
            //    }
            //    p_DataStart[i] = *p_ShifedRawValue;
            //} while (i < lastByteIndex);
            //last byte
            Byte lastByte_s;
            if (firstBitIndexInFirstByte + _BitLength >= 64)
            {
                Byte lastByte = ((byte*)&rawValue)[7];
                UInt16 lastByte_u16_s = (UInt16)(lastByte << firstBitIndexInFirstByte);
                lastByte_s = *(((Byte*)&lastByte_u16_s) + 1);
            }
            else
            {
                lastByte_s = *p_ShifedRawValue;
            }
            int lastBitIndexInLastByte = lastBitIndex % 8;
            Byte lastByteMask = (Byte)(0xFF - ((2 << (lastBitIndexInLastByte + 1)) - 1));
            p_DataStart[i] = (Byte)(p_DataStart[i] & lastByteMask | lastByte_s);
            return true;
        }
        public double GetInRangeValue(Int64 rawValue, out bool isOutRange, bool considerManualSetLimits)
        {
            switch (_SignalDataType)
            {
                case SignalDataType.Signed:
                    {
                        var d = _SIntTypeMaxValue;
                        var max = d - 1;
                        var min = -d;
                        Int64 value;
                        if (rawValue > max)
                        {
                            isOutRange = true;
                            value = max;
                        }
                        else if (rawValue < min)
                        {
                            isOutRange = true;
                            value = min;
                        }
                        else
                        {
                            if (considerManualSetLimits && _IsUsingLowHighLimit)
                            {
                                if (rawValue > _RawValueHighLimit)
                                {
                                    isOutRange = true;
                                    value = _RawValueHighLimit;
                                }
                                else if (rawValue < _RawValueLowLimit)
                                {
                                    isOutRange = true;
                                    value = _RawValueLowLimit;
                                }
                                else
                                {
                                    isOutRange = false;
                                    value = rawValue;
                                }
                            }
                            else
                            {
                                value = rawValue;
                                isOutRange = false;
                            }
                        }
                        return rawValue * _Factor + _Offset;
                        //break;
                    }
                case SignalDataType.Unsigned:
                    {
                        var rawValue_u = (UInt64)rawValue;
                        var max = _UIntTypeMaxValue;
                        UInt64 value;
                        if (rawValue_u > max)
                        {
                            isOutRange = true;
                            value = max;
                        }
                        else
                        {
                            if (considerManualSetLimits && _IsUsingLowHighLimit)
                            {
                                if (rawValue_u > (UInt64)_RawValueHighLimit)
                                {
                                    isOutRange = true;
                                    value = (UInt64)_RawValueHighLimit;
                                }
                                else if (rawValue_u < (UInt64)_RawValueLowLimit)
                                {
                                    isOutRange = true;
                                    value = (UInt64)_RawValueLowLimit;
                                }
                                else
                                {
                                    isOutRange = false;
                                    value = rawValue_u;
                                }
                            }
                            else
                            {
                                value = rawValue_u;
                                isOutRange = false;
                            }
                        }
                        return value * _Factor + _Offset;
                    }
                //break;
                case SignalDataType.IEEE_Float:
                    unsafe
                    {
                        var rawValue_float = *(float*)&rawValue;
                        if (considerManualSetLimits && _IsUsingLowHighLimit)
                        {
                            var high = _RawValueHighLimit;
                            var low = _RawValueLowLimit;
                            if (rawValue_float > *(float*)&high)
                            {
                                isOutRange = true;
                                return _RawValueHighLimit;
                            }
                            else if (rawValue_float < *(float*)&low)
                            {
                                isOutRange = true;
                                return _RawValueLowLimit;
                            }
                        }
                        var rv = rawValue_float * _Factor + _Offset;
                        if (rv > float.MaxValue)
                        {
                            isOutRange = true;
                            return float.MaxValue;
                        }
                        else if (rv < float.MinValue)
                        {
                            isOutRange = true;
                            return float.MinValue;
                        }
                        isOutRange = false;
                        return rv;
                    }
                //break;
                case SignalDataType.IEEE_Double:
                    unsafe
                    {
                        var rawValue_double = *(double*)&rawValue;
                        if (considerManualSetLimits && _IsUsingLowHighLimit)
                        {
                            var high = _RawValueHighLimit;
                            var low = _RawValueLowLimit;
                            if (rawValue_double > *(double*)&high)
                            {
                                isOutRange = true;
                                return _RawValueHighLimit;
                            }
                            else if (rawValue_double < *(double*)&low)
                            {
                                isOutRange = true;
                                return _RawValueLowLimit;
                            }
                        }
                        var rv = rawValue_double * _Factor + _Offset;
                        //if (rv > double.MaxValue)
                        //{
                        //    isOutRange = true;
                        //    return double.MaxValue;
                        //}
                        //else if (rv < double.MinValue)
                        //{
                        //    isOutRange = true;
                        //    return double.MinValue;
                        //}
                        isOutRange = false;
                        return rv;
                    }
                //break;
                default:
                    throw new Exception("No Such SignalDataType, SignalObject.GetInRangeValue");
                //break;
            }
        }
        public unsafe double GetValue(Byte[] data)
        {
            fixed (byte* p_DataStart = data)
            {
                return GetValue(p_DataStart, data.Length);
            }
        }
        public unsafe double GetValue(byte* p_DataStart, int DataLength)
        {
            int lastBitIndex = _StartBit + _BitLength - 1;
            int firstByteIndex = _StartBit / 8;
            int lastByteIndex = lastBitIndex / 8;
            if (_Endianness == Endianness.BigEndian)
            {
                lastByteIndex = firstByteIndex - (lastByteIndex - firstByteIndex);
                if (lastByteIndex < 0)
                {
                    MessageBox.Show("Signal Last Bit In Data Array Out Of Range, SignalObject.GetValue");
                    return double.NaN;
                }
            }
            else
            {
                if (lastByteIndex >= DataLength)
                {
                    MessageBox.Show("Signal Last Bit In Data Array Out Of Range, SignalObject.GetValue");
                    return double.NaN;
                }
            }
            int firstBitIndexInFirstByte = _StartBit % 8;
            int lastBitIndexInLastByte = lastBitIndex % 8;
            UInt64 shiftedRawValue;
            //first byte
            if (lastByteIndex == firstByteIndex)
            {
                ////methord 1: mask then shift
                //byte firstByteValueMask = (byte)(((2 << (lastBitIndexInLastByte + 1)) - 1) - ((2 << firstBitIndexInFirstByte) - 1));
                //shiftedRawValue = (byte)((p_DataStart[firstByteIndex] & firstByteValueMask) >> firstBitIndexInFirstByte);
                ////methord 2: shift then mask
                var firstByte_s = (byte)(p_DataStart[firstByteIndex] >> firstBitIndexInFirstByte);
                byte firstByteValueMask = (Byte)((1 << (lastBitIndexInLastByte - firstBitIndexInFirstByte + 1)) - 1);
                shiftedRawValue = (byte)(firstByte_s & firstByteValueMask);
            }
            else
            {
                //first byte
                UInt64 rawValue = p_DataStart[firstByteIndex];
                byte* p_RawValue = (byte*)&rawValue;
                //middle
                int i = firstByteIndex;
                p_RawValue++;
                if (_Endianness == Endianness.BigEndian)
                {
                    i--;
                }
                else
                {
                    i++;
                }
                if (_Endianness == Endianness.BigEndian)
                {
                    while (i > lastByteIndex)
                    {
                        *p_RawValue = p_DataStart[i];
                        p_RawValue++;
                        i--;
                    }
                }
                else
                {
                    while (i < lastByteIndex)
                    {
                        *p_RawValue = p_DataStart[i];
                        p_RawValue++;
                        i++;
                    }
                }

                //do
                //{
                //    p_RawValue++;
                //    if (_Endianness == Endianness.BigEndian)
                //    {
                //        i--;
                //    }
                //    else
                //    {
                //        i++;
                //    }
                //    *p_RawValue = p_DataStart[i];
                //} while (i < lastByteIndex);

                //last byte
                Byte lastByteMask = (Byte)((2 << (lastBitIndexInLastByte + 1)) - 1);
                Byte lastByte_s = (Byte)(p_DataStart[i] & lastByteMask);
                if (firstBitIndexInFirstByte + _BitLength >= 64)
                {
                    UInt16 lastByte_u16 = (UInt16)((lastByte_s >> 8) + (*(p_RawValue - 1)));
                    lastByte_s = (Byte)(lastByte_u16 >> firstBitIndexInFirstByte);
                    shiftedRawValue = rawValue >> firstBitIndexInFirstByte;
                    *(((Byte*)&shiftedRawValue) + 7) = lastByte_s;
                }
                else
                {
                    *p_RawValue = lastByte_s;
                    shiftedRawValue = rawValue >> firstBitIndexInFirstByte;
                }
            }
            bool isOutOfRange;
            var r = GetInRangeValue((Int64)shiftedRawValue, out isOutOfRange, true);
            return r;
        }

        protected Double _Factor = 1.0;
        public Double Factor
        {
            get { return _Factor; }
            set
            {
                if (_Factor != value)
                {
                    if (value == 0)
                    {
                        MessageBox.Show("Factor of SigalObject should not be 0.");
                        return;
                    }
                    _Factor = value;
                    OnSetDataID("Factor");
                    RaisePropertyChanged("Factor");
                }
            }
        }
        protected Double _Offset = 0.0;
        public Double Offset
        {
            get { return _Offset; }
            set
            {
                if (_Offset != value)
                {
                    _Offset = value;
                    OnSetDataID("Offset");
                    RaisePropertyChanged("Offset");
                }
            }
        }

        protected Int64 _RawDefaultValue;
        /// <summary>
        /// not consider endian, here
        /// if value type is signed,  the sign bit is extended
        /// </summary>
        public Int64 RawDefaultValue
        {
            get { return _RawDefaultValue; }
            set
            {
                if (value != _RawDefaultValue)
                {
                    _RawDefaultValue = value;
                    RaisePropertyChanged("RawDefaultValue");
                }
            }
        }


        protected Int64 _RawValueLowLimit;
        /// <summary>
        /// not consider endian, here
        /// if value type is signed, the sign bit is extended
        /// </summary>
        public Int64 RawValueLowLimit
        {
            get { return _RawValueLowLimit; }
            set
            {
                if (value != _RawValueLowLimit)
                {
                    _RawValueLowLimit = value;
                    RaisePropertyChanged("RawValueLowLimit");
                }
            }
        }

        public double GetRealRawValue(Int64 rawValue)
        {
            switch (_SignalDataType)
            {
                case SignalDataType.Signed:
                    return rawValue;
                //break;
                case SignalDataType.Unsigned:
                    return (UInt64)rawValue;
                // break;
                case SignalDataType.IEEE_Float:
                    unsafe
                    {
                        var rv = *(float*)&rawValue;//break;
                        return rv;
                    }

                case SignalDataType.IEEE_Double:
                    unsafe
                    {
                        var rv = *(double*)&rawValue;//break;
                        return rv;
                    }
                default:
                    throw new Exception("No Such SignalDataType --SignalObject.GetRealRawValue");
                //break;
            }
        }
        Int64 GetRealRawValueOfSignedType(Int64 rawValue)
        {
            if (_SignalDataType == Data.SignalDataType.Signed)
            {
                var singMask = (1 << (_BitLength - 1));
                if ((rawValue & singMask) != 0) //negitive
                {
                    Int64 rv = -1;
                    rv = rv | rawValue;
                    return rv;
                }
            }
            return rawValue;
        }

        protected bool _IsUsingLowHighLimit;

        public bool IsUsingLowHighLimit
        {
            get { return _IsUsingLowHighLimit; }
            set
            {
                if (value != _IsUsingLowHighLimit)
                {
                    _IsUsingLowHighLimit = value;
                    RaisePropertyChanged("IsUsingLowHighLimit");
                }
            }
        }

        protected Int64 _RawValueHighLimit;
        /// <summary>
        /// not consider endian, here
        /// if value type is signed,  the sign bit is extended
        /// </summary>
        public Int64 RawValueHighLimit
        {
            get { return _RawValueHighLimit; }
            set
            {
                if (value != _RawValueHighLimit)
                {
                    _RawValueHighLimit = value;
                    RaisePropertyChanged("RawValueHighLimit");
                }
            }
        }
    }
}
