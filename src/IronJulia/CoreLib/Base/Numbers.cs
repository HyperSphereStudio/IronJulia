public partial struct Base {
    public readonly struct Int8(sbyte value) : Signed, IEquatable<Int8> {
        public readonly sbyte Value = value;
        
        public static implicit operator Int8(sbyte value) => new(value);
        public static implicit operator sbyte(Int8 value) => value.Value;
        
        public static Int8 operator +(Int8 a, Int8 b) => (sbyte) (a.Value + b.Value);
        public static Int8 operator ++(Int8 a) => (sbyte) (a.Value + (sbyte)1);
        public static Int8 operator --(Int8 a) => (sbyte) (a.Value - (sbyte)1);


        public static Int8 operator -(Int8 a) => (sbyte) (-(sbyte)a.Value);


        public static Int8 operator +(Int8 a) => (sbyte) (+(sbyte)a.Value);
        public static Int8 operator -(Int8 a, Int8 b) => (sbyte) (a.Value - b.Value);
        public static Int8 operator *(Int8 a, Int8 b) => (sbyte) (a.Value * b.Value);
        public static Int8 operator /(Int8 a, Int8 b) => (sbyte) (a.Value / b.Value);
        public static bool operator <(Int8 a, Int8 b) => a.Value < b.Value;
        public static bool operator >(Int8 a, Int8 b) => a.Value > b.Value;
        public static bool operator ==(Int8 a, Int8 b) => a.Value == b.Value;
        public static bool operator !=(Int8 a, Int8 b) => !(a == b);
        
        
        public static explicit operator Int8(Half value) => new((sbyte) value);
        public static implicit operator Half(Int8 value) => (Half) value.Value;

        public static explicit operator Int8(float value) => new((sbyte) value);
        public static implicit operator float(Int8 value) => (float) value.Value;

        public static explicit operator Int8(double value) => new((sbyte) value);
        public static implicit operator double(Int8 value) => (double) value.Value;

        public static explicit operator Int8(decimal value) => new((sbyte) value);
        public static implicit operator decimal(Int8 value) => (decimal) value.Value;

        public static explicit operator Int8(short value) => new((sbyte) value);
        public static implicit operator short(Int8 value) => (short) value.Value;

        public static explicit operator Int8(int value) => new((sbyte) value);
        public static implicit operator int(Int8 value) => (int) value.Value;

        public static explicit operator Int8(long value) => new((sbyte) value);
        public static implicit operator long(Int8 value) => (long) value.Value;

        public static explicit operator Int8(nint value) => new((sbyte) value);
        public static implicit operator nint(Int8 value) => (nint) value.Value;

        public static explicit operator Int8(System.Int128 value) => new((sbyte) value);
        public static implicit operator System.Int128(Int8 value) => (System.Int128) value.Value;

        public static explicit operator Int8(byte value) => new((sbyte) value);
        public static explicit operator byte(Int8 value) => (byte) value.Value;

        public static explicit operator Int8(ushort value) => new((sbyte) value);
        public static explicit operator ushort(Int8 value) => (ushort) value.Value;

        public static explicit operator Int8(uint value) => new((sbyte) value);
        public static explicit operator uint(Int8 value) => (uint) value.Value;

        public static explicit operator Int8(ulong value) => new((sbyte) value);
        public static explicit operator ulong(Int8 value) => (ulong) value.Value;

        public static explicit operator Int8(nuint value) => new((sbyte) value);
        public static explicit operator nuint(Int8 value) => (nuint) value.Value;

        public static explicit operator Int8(System.UInt128 value) => new((sbyte) value);
        public static explicit operator System.UInt128(Int8 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Int8 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Int8 other && Equals(other);
    }


    public readonly struct Int16(short value) : Signed, IEquatable<Int16> {
        public readonly short Value = value;
        
        public static implicit operator Int16(short value) => new(value);
        public static implicit operator short(Int16 value) => value.Value;
        
        public static Int16 operator +(Int16 a, Int16 b) => (short) (a.Value + b.Value);
        public static Int16 operator ++(Int16 a) => (short) (a.Value + (short)1);
        public static Int16 operator --(Int16 a) => (short) (a.Value - (short)1);


        public static Int16 operator -(Int16 a) => (short) (-(short)a.Value);


        public static Int16 operator +(Int16 a) => (short) (+(short)a.Value);
        public static Int16 operator -(Int16 a, Int16 b) => (short) (a.Value - b.Value);
        public static Int16 operator *(Int16 a, Int16 b) => (short) (a.Value * b.Value);
        public static Int16 operator /(Int16 a, Int16 b) => (short) (a.Value / b.Value);
        public static bool operator <(Int16 a, Int16 b) => a.Value < b.Value;
        public static bool operator >(Int16 a, Int16 b) => a.Value > b.Value;
        public static bool operator ==(Int16 a, Int16 b) => a.Value == b.Value;
        public static bool operator !=(Int16 a, Int16 b) => !(a == b);
        
        
        public static explicit operator Int16(Half value) => new((short) value);
        public static implicit operator Half(Int16 value) => (Half) value.Value;

        public static explicit operator Int16(float value) => new((short) value);
        public static implicit operator float(Int16 value) => (float) value.Value;

        public static explicit operator Int16(double value) => new((short) value);
        public static implicit operator double(Int16 value) => (double) value.Value;

        public static explicit operator Int16(decimal value) => new((short) value);
        public static implicit operator decimal(Int16 value) => (decimal) value.Value;

        public static implicit operator Int16(sbyte value) => new((short) value);
        public static explicit operator sbyte(Int16 value) => (sbyte) value.Value;

        public static explicit operator Int16(int value) => new((short) value);
        public static implicit operator int(Int16 value) => (int) value.Value;

        public static explicit operator Int16(long value) => new((short) value);
        public static implicit operator long(Int16 value) => (long) value.Value;

        public static explicit operator Int16(nint value) => new((short) value);
        public static implicit operator nint(Int16 value) => (nint) value.Value;

        public static explicit operator Int16(System.Int128 value) => new((short) value);
        public static implicit operator System.Int128(Int16 value) => (System.Int128) value.Value;

        public static implicit operator Int16(byte value) => new((short) value);
        public static explicit operator byte(Int16 value) => (byte) value.Value;

        public static explicit operator Int16(ushort value) => new((short) value);
        public static explicit operator ushort(Int16 value) => (ushort) value.Value;

        public static explicit operator Int16(uint value) => new((short) value);
        public static explicit operator uint(Int16 value) => (uint) value.Value;

        public static explicit operator Int16(ulong value) => new((short) value);
        public static explicit operator ulong(Int16 value) => (ulong) value.Value;

        public static explicit operator Int16(nuint value) => new((short) value);
        public static explicit operator nuint(Int16 value) => (nuint) value.Value;

        public static explicit operator Int16(System.UInt128 value) => new((short) value);
        public static explicit operator System.UInt128(Int16 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Int16 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Int16 other && Equals(other);
    }


    public readonly struct Int32(int value) : Signed, IEquatable<Int32> {
        public readonly int Value = value;
        
        public static implicit operator Int32(int value) => new(value);
        public static implicit operator int(Int32 value) => value.Value;
        
        public static Int32 operator +(Int32 a, Int32 b) => (int) (a.Value + b.Value);
        public static Int32 operator ++(Int32 a) => (int) (a.Value + (int)1);
        public static Int32 operator --(Int32 a) => (int) (a.Value - (int)1);


        public static Int32 operator -(Int32 a) => (int) (-(int)a.Value);


        public static Int32 operator +(Int32 a) => (int) (+(int)a.Value);
        public static Int32 operator -(Int32 a, Int32 b) => (int) (a.Value - b.Value);
        public static Int32 operator *(Int32 a, Int32 b) => (int) (a.Value * b.Value);
        public static Int32 operator /(Int32 a, Int32 b) => (int) (a.Value / b.Value);
        public static bool operator <(Int32 a, Int32 b) => a.Value < b.Value;
        public static bool operator >(Int32 a, Int32 b) => a.Value > b.Value;
        public static bool operator ==(Int32 a, Int32 b) => a.Value == b.Value;
        public static bool operator !=(Int32 a, Int32 b) => !(a == b);
        
        
        public static implicit operator Int32(Half value) => new((int) value);
        public static explicit operator Half(Int32 value) => (Half) value.Value;

        public static explicit operator Int32(float value) => new((int) value);
        public static implicit operator float(Int32 value) => (float) value.Value;

        public static explicit operator Int32(double value) => new((int) value);
        public static implicit operator double(Int32 value) => (double) value.Value;

        public static explicit operator Int32(decimal value) => new((int) value);
        public static implicit operator decimal(Int32 value) => (decimal) value.Value;

        public static implicit operator Int32(sbyte value) => new((int) value);
        public static explicit operator sbyte(Int32 value) => (sbyte) value.Value;

        public static implicit operator Int32(short value) => new((int) value);
        public static explicit operator short(Int32 value) => (short) value.Value;

        public static explicit operator Int32(long value) => new((int) value);
        public static implicit operator long(Int32 value) => (long) value.Value;

        public static explicit operator Int32(nint value) => new((int) value);
        public static implicit operator nint(Int32 value) => (nint) value.Value;

        public static explicit operator Int32(System.Int128 value) => new((int) value);
        public static implicit operator System.Int128(Int32 value) => (System.Int128) value.Value;

        public static implicit operator Int32(byte value) => new((int) value);
        public static explicit operator byte(Int32 value) => (byte) value.Value;

        public static implicit operator Int32(ushort value) => new((int) value);
        public static explicit operator ushort(Int32 value) => (ushort) value.Value;

        public static explicit operator Int32(uint value) => new((int) value);
        public static explicit operator uint(Int32 value) => (uint) value.Value;

        public static explicit operator Int32(ulong value) => new((int) value);
        public static explicit operator ulong(Int32 value) => (ulong) value.Value;

        public static explicit operator Int32(nuint value) => new((int) value);
        public static explicit operator nuint(Int32 value) => (nuint) value.Value;

        public static explicit operator Int32(System.UInt128 value) => new((int) value);
        public static explicit operator System.UInt128(Int32 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Int32 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Int32 other && Equals(other);
    }


    public readonly struct Int64(long value) : Signed, IEquatable<Int64> {
        public readonly long Value = value;
        
        public static implicit operator Int64(long value) => new(value);
        public static implicit operator long(Int64 value) => value.Value;
        
        public static Int64 operator +(Int64 a, Int64 b) => (long) (a.Value + b.Value);
        public static Int64 operator ++(Int64 a) => (long) (a.Value + (long)1);
        public static Int64 operator --(Int64 a) => (long) (a.Value - (long)1);


        public static Int64 operator -(Int64 a) => (long) (-(long)a.Value);


        public static Int64 operator +(Int64 a) => (long) (+(long)a.Value);
        public static Int64 operator -(Int64 a, Int64 b) => (long) (a.Value - b.Value);
        public static Int64 operator *(Int64 a, Int64 b) => (long) (a.Value * b.Value);
        public static Int64 operator /(Int64 a, Int64 b) => (long) (a.Value / b.Value);
        public static bool operator <(Int64 a, Int64 b) => a.Value < b.Value;
        public static bool operator >(Int64 a, Int64 b) => a.Value > b.Value;
        public static bool operator ==(Int64 a, Int64 b) => a.Value == b.Value;
        public static bool operator !=(Int64 a, Int64 b) => !(a == b);
        
        
        public static implicit operator Int64(Half value) => new((long) value);
        public static explicit operator Half(Int64 value) => (Half) value.Value;

        public static explicit operator Int64(float value) => new((long) value);
        public static implicit operator float(Int64 value) => (float) value.Value;

        public static explicit operator Int64(double value) => new((long) value);
        public static implicit operator double(Int64 value) => (double) value.Value;

        public static explicit operator Int64(decimal value) => new((long) value);
        public static implicit operator decimal(Int64 value) => (decimal) value.Value;

        public static implicit operator Int64(sbyte value) => new((long) value);
        public static explicit operator sbyte(Int64 value) => (sbyte) value.Value;

        public static implicit operator Int64(short value) => new((long) value);
        public static explicit operator short(Int64 value) => (short) value.Value;

        public static implicit operator Int64(int value) => new((long) value);
        public static explicit operator int(Int64 value) => (int) value.Value;

        public static implicit operator Int64(nint value) => new((long) value);
        public static implicit operator nint(Int64 value) => (nint) value.Value;

        public static explicit operator Int64(System.Int128 value) => new((long) value);
        public static implicit operator System.Int128(Int64 value) => (System.Int128) value.Value;

        public static implicit operator Int64(byte value) => new((long) value);
        public static explicit operator byte(Int64 value) => (byte) value.Value;

        public static implicit operator Int64(ushort value) => new((long) value);
        public static explicit operator ushort(Int64 value) => (ushort) value.Value;

        public static implicit operator Int64(uint value) => new((long) value);
        public static explicit operator uint(Int64 value) => (uint) value.Value;

        public static explicit operator Int64(ulong value) => new((long) value);
        public static explicit operator ulong(Int64 value) => (ulong) value.Value;

        public static explicit operator Int64(nuint value) => new((long) value);
        public static explicit operator nuint(Int64 value) => (nuint) value.Value;

        public static explicit operator Int64(System.UInt128 value) => new((long) value);
        public static explicit operator System.UInt128(Int64 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Int64 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Int64 other && Equals(other);
    }


    public readonly struct Int(nint value) : Signed, IEquatable<Int> {
        public readonly nint Value = value;
        
        public static implicit operator Int(nint value) => new(value);
        public static implicit operator nint(Int value) => value.Value;
        
        public static Int operator +(Int a, Int b) => (nint) (a.Value + b.Value);
        public static Int operator ++(Int a) => (nint) (a.Value + (nint)1);
        public static Int operator --(Int a) => (nint) (a.Value - (nint)1);


        public static Int operator -(Int a) => (nint) (-(nint)a.Value);


        public static Int operator +(Int a) => (nint) (+(nint)a.Value);
        public static Int operator -(Int a, Int b) => (nint) (a.Value - b.Value);
        public static Int operator *(Int a, Int b) => (nint) (a.Value * b.Value);
        public static Int operator /(Int a, Int b) => (nint) (a.Value / b.Value);
        public static bool operator <(Int a, Int b) => a.Value < b.Value;
        public static bool operator >(Int a, Int b) => a.Value > b.Value;
        public static bool operator ==(Int a, Int b) => a.Value == b.Value;
        public static bool operator !=(Int a, Int b) => !(a == b);
        
        
        public static implicit operator Int(Half value) => new((nint) value);
        public static explicit operator Half(Int value) => (Half) value.Value;

        public static explicit operator Int(float value) => new((nint) value);
        public static implicit operator float(Int value) => (float) value.Value;

        public static explicit operator Int(double value) => new((nint) value);
        public static implicit operator double(Int value) => (double) value.Value;

        public static explicit operator Int(decimal value) => new((nint) value);
        public static implicit operator decimal(Int value) => (decimal) value.Value;

        public static implicit operator Int(sbyte value) => new((nint) value);
        public static explicit operator sbyte(Int value) => (sbyte) value.Value;

        public static implicit operator Int(short value) => new((nint) value);
        public static explicit operator short(Int value) => (short) value.Value;

        public static implicit operator Int(int value) => new((nint) value);
        public static explicit operator int(Int value) => (int) value.Value;

        public static implicit operator Int(long value) => new((nint) value);
        public static implicit operator long(Int value) => (long) value.Value;

        public static explicit operator Int(System.Int128 value) => new((nint) value);
        public static implicit operator System.Int128(Int value) => (System.Int128) value.Value;

        public static implicit operator Int(byte value) => new((nint) value);
        public static explicit operator byte(Int value) => (byte) value.Value;

        public static implicit operator Int(ushort value) => new((nint) value);
        public static explicit operator ushort(Int value) => (ushort) value.Value;

        public static implicit operator Int(uint value) => new((nint) value);
        public static explicit operator uint(Int value) => (uint) value.Value;

        public static explicit operator Int(ulong value) => new((nint) value);
        public static explicit operator ulong(Int value) => (ulong) value.Value;

        public static explicit operator Int(nuint value) => new((nint) value);
        public static explicit operator nuint(Int value) => (nuint) value.Value;

        public static explicit operator Int(System.UInt128 value) => new((nint) value);
        public static explicit operator System.UInt128(Int value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Int other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Int other && Equals(other);
    }


    public readonly struct Int128(System.Int128 value) : Signed, IEquatable<Int128> {
        public readonly System.Int128 Value = value;
        
        public static implicit operator Int128(System.Int128 value) => new(value);
        public static implicit operator System.Int128(Int128 value) => value.Value;
        
        public static Int128 operator +(Int128 a, Int128 b) => (System.Int128) (a.Value + b.Value);
        public static Int128 operator ++(Int128 a) => (System.Int128) (a.Value + (System.Int128)1);
        public static Int128 operator --(Int128 a) => (System.Int128) (a.Value - (System.Int128)1);


        public static Int128 operator -(Int128 a) => (System.Int128) (-(System.Int128)a.Value);


        public static Int128 operator +(Int128 a) => (System.Int128) (+(System.Int128)a.Value);
        public static Int128 operator -(Int128 a, Int128 b) => (System.Int128) (a.Value - b.Value);
        public static Int128 operator *(Int128 a, Int128 b) => (System.Int128) (a.Value * b.Value);
        public static Int128 operator /(Int128 a, Int128 b) => (System.Int128) (a.Value / b.Value);
        public static bool operator <(Int128 a, Int128 b) => a.Value < b.Value;
        public static bool operator >(Int128 a, Int128 b) => a.Value > b.Value;
        public static bool operator ==(Int128 a, Int128 b) => a.Value == b.Value;
        public static bool operator !=(Int128 a, Int128 b) => !(a == b);
        
        
        public static implicit operator Int128(Half value) => new((System.Int128) value);
        public static explicit operator Half(Int128 value) => (Half) value.Value;

        public static explicit operator Int128(float value) => new((System.Int128) value);
        public static implicit operator float(Int128 value) => (float) value.Value;

        public static explicit operator Int128(double value) => new((System.Int128) value);
        public static implicit operator double(Int128 value) => (double) value.Value;

        public static implicit operator Int128(decimal value) => new((System.Int128) value);
        public static explicit operator decimal(Int128 value) => (decimal) value.Value;

        public static implicit operator Int128(sbyte value) => new((System.Int128) value);
        public static explicit operator sbyte(Int128 value) => (sbyte) value.Value;

        public static implicit operator Int128(short value) => new((System.Int128) value);
        public static explicit operator short(Int128 value) => (short) value.Value;

        public static implicit operator Int128(int value) => new((System.Int128) value);
        public static explicit operator int(Int128 value) => (int) value.Value;

        public static implicit operator Int128(long value) => new((System.Int128) value);
        public static explicit operator long(Int128 value) => (long) value.Value;

        public static implicit operator Int128(nint value) => new((System.Int128) value);
        public static explicit operator nint(Int128 value) => (nint) value.Value;

        public static implicit operator Int128(byte value) => new((System.Int128) value);
        public static explicit operator byte(Int128 value) => (byte) value.Value;

        public static implicit operator Int128(ushort value) => new((System.Int128) value);
        public static explicit operator ushort(Int128 value) => (ushort) value.Value;

        public static implicit operator Int128(uint value) => new((System.Int128) value);
        public static explicit operator uint(Int128 value) => (uint) value.Value;

        public static implicit operator Int128(ulong value) => new((System.Int128) value);
        public static explicit operator ulong(Int128 value) => (ulong) value.Value;

        public static implicit operator Int128(nuint value) => new((System.Int128) value);
        public static explicit operator nuint(Int128 value) => (nuint) value.Value;

        public static explicit operator Int128(System.UInt128 value) => new((System.Int128) value);
        public static explicit operator System.UInt128(Int128 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Int128 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Int128 other && Equals(other);
    }


    public readonly struct UInt8(byte value) : Unsigned, IEquatable<UInt8> {
        public readonly byte Value = value;
        
        public static implicit operator UInt8(byte value) => new(value);
        public static implicit operator byte(UInt8 value) => value.Value;
        
        public static UInt8 operator +(UInt8 a, UInt8 b) => (byte) (a.Value + b.Value);
        public static UInt8 operator ++(UInt8 a) => (byte) (a.Value + (byte)1);
        public static UInt8 operator --(UInt8 a) => (byte) (a.Value - (byte)1);


        public static Int8 operator -(UInt8 a) => -(Int8)(byte)a.Value;


        public static UInt8 operator +(UInt8 a) => (byte) (+(byte)a.Value);
        public static UInt8 operator -(UInt8 a, UInt8 b) => (byte) (a.Value - b.Value);
        public static UInt8 operator *(UInt8 a, UInt8 b) => (byte) (a.Value * b.Value);
        public static UInt8 operator /(UInt8 a, UInt8 b) => (byte) (a.Value / b.Value);
        public static bool operator <(UInt8 a, UInt8 b) => a.Value < b.Value;
        public static bool operator >(UInt8 a, UInt8 b) => a.Value > b.Value;
        public static bool operator ==(UInt8 a, UInt8 b) => a.Value == b.Value;
        public static bool operator !=(UInt8 a, UInt8 b) => !(a == b);
        
        
        public static explicit operator UInt8(Half value) => new((byte) value);
        public static implicit operator Half(UInt8 value) => (Half) value.Value;

        public static explicit operator UInt8(float value) => new((byte) value);
        public static implicit operator float(UInt8 value) => (float) value.Value;

        public static explicit operator UInt8(double value) => new((byte) value);
        public static implicit operator double(UInt8 value) => (double) value.Value;

        public static explicit operator UInt8(decimal value) => new((byte) value);
        public static implicit operator decimal(UInt8 value) => (decimal) value.Value;

        public static explicit operator UInt8(sbyte value) => new((byte) value);
        public static explicit operator sbyte(UInt8 value) => (sbyte) value.Value;

        public static explicit operator UInt8(short value) => new((byte) value);
        public static implicit operator short(UInt8 value) => (short) value.Value;

        public static explicit operator UInt8(int value) => new((byte) value);
        public static implicit operator int(UInt8 value) => (int) value.Value;

        public static explicit operator UInt8(long value) => new((byte) value);
        public static implicit operator long(UInt8 value) => (long) value.Value;

        public static explicit operator UInt8(nint value) => new((byte) value);
        public static implicit operator nint(UInt8 value) => (nint) value.Value;

        public static explicit operator UInt8(System.Int128 value) => new((byte) value);
        public static implicit operator System.Int128(UInt8 value) => (System.Int128) value.Value;

        public static explicit operator UInt8(ushort value) => new((byte) value);
        public static implicit operator ushort(UInt8 value) => (ushort) value.Value;

        public static explicit operator UInt8(uint value) => new((byte) value);
        public static implicit operator uint(UInt8 value) => (uint) value.Value;

        public static explicit operator UInt8(ulong value) => new((byte) value);
        public static implicit operator ulong(UInt8 value) => (ulong) value.Value;

        public static explicit operator UInt8(nuint value) => new((byte) value);
        public static implicit operator nuint(UInt8 value) => (nuint) value.Value;

        public static explicit operator UInt8(System.UInt128 value) => new((byte) value);
        public static implicit operator System.UInt128(UInt8 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(UInt8 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UInt8 other && Equals(other);
    }


    public readonly struct UInt16(ushort value) : Unsigned, IEquatable<UInt16> {
        public readonly ushort Value = value;
        
        public static implicit operator UInt16(ushort value) => new(value);
        public static implicit operator ushort(UInt16 value) => value.Value;
        
        public static UInt16 operator +(UInt16 a, UInt16 b) => (ushort) (a.Value + b.Value);
        public static UInt16 operator ++(UInt16 a) => (ushort) (a.Value + (ushort)1);
        public static UInt16 operator --(UInt16 a) => (ushort) (a.Value - (ushort)1);


        public static Int16 operator -(UInt16 a) => -(Int16)(ushort)a.Value;


        public static UInt16 operator +(UInt16 a) => (ushort) (+(ushort)a.Value);
        public static UInt16 operator -(UInt16 a, UInt16 b) => (ushort) (a.Value - b.Value);
        public static UInt16 operator *(UInt16 a, UInt16 b) => (ushort) (a.Value * b.Value);
        public static UInt16 operator /(UInt16 a, UInt16 b) => (ushort) (a.Value / b.Value);
        public static bool operator <(UInt16 a, UInt16 b) => a.Value < b.Value;
        public static bool operator >(UInt16 a, UInt16 b) => a.Value > b.Value;
        public static bool operator ==(UInt16 a, UInt16 b) => a.Value == b.Value;
        public static bool operator !=(UInt16 a, UInt16 b) => !(a == b);
        
        
        public static explicit operator UInt16(Half value) => new((ushort) value);
        public static implicit operator Half(UInt16 value) => (Half) value.Value;

        public static explicit operator UInt16(float value) => new((ushort) value);
        public static implicit operator float(UInt16 value) => (float) value.Value;

        public static explicit operator UInt16(double value) => new((ushort) value);
        public static implicit operator double(UInt16 value) => (double) value.Value;

        public static explicit operator UInt16(decimal value) => new((ushort) value);
        public static implicit operator decimal(UInt16 value) => (decimal) value.Value;

        public static explicit operator UInt16(sbyte value) => new((ushort) value);
        public static explicit operator sbyte(UInt16 value) => (sbyte) value.Value;

        public static explicit operator UInt16(short value) => new((ushort) value);
        public static explicit operator short(UInt16 value) => (short) value.Value;

        public static explicit operator UInt16(int value) => new((ushort) value);
        public static implicit operator int(UInt16 value) => (int) value.Value;

        public static explicit operator UInt16(long value) => new((ushort) value);
        public static implicit operator long(UInt16 value) => (long) value.Value;

        public static explicit operator UInt16(nint value) => new((ushort) value);
        public static implicit operator nint(UInt16 value) => (nint) value.Value;

        public static explicit operator UInt16(System.Int128 value) => new((ushort) value);
        public static implicit operator System.Int128(UInt16 value) => (System.Int128) value.Value;

        public static implicit operator UInt16(byte value) => new((ushort) value);
        public static explicit operator byte(UInt16 value) => (byte) value.Value;

        public static explicit operator UInt16(uint value) => new((ushort) value);
        public static implicit operator uint(UInt16 value) => (uint) value.Value;

        public static explicit operator UInt16(ulong value) => new((ushort) value);
        public static implicit operator ulong(UInt16 value) => (ulong) value.Value;

        public static explicit operator UInt16(nuint value) => new((ushort) value);
        public static implicit operator nuint(UInt16 value) => (nuint) value.Value;

        public static explicit operator UInt16(System.UInt128 value) => new((ushort) value);
        public static implicit operator System.UInt128(UInt16 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(UInt16 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UInt16 other && Equals(other);
    }


    public readonly struct UInt32(uint value) : Unsigned, IEquatable<UInt32> {
        public readonly uint Value = value;
        
        public static implicit operator UInt32(uint value) => new(value);
        public static implicit operator uint(UInt32 value) => value.Value;
        
        public static UInt32 operator +(UInt32 a, UInt32 b) => (uint) (a.Value + b.Value);
        public static UInt32 operator ++(UInt32 a) => (uint) (a.Value + (uint)1);
        public static UInt32 operator --(UInt32 a) => (uint) (a.Value - (uint)1);


        public static Int32 operator -(UInt32 a) => -(Int32)(uint)a.Value;


        public static UInt32 operator +(UInt32 a) => (uint) (+(uint)a.Value);
        public static UInt32 operator -(UInt32 a, UInt32 b) => (uint) (a.Value - b.Value);
        public static UInt32 operator *(UInt32 a, UInt32 b) => (uint) (a.Value * b.Value);
        public static UInt32 operator /(UInt32 a, UInt32 b) => (uint) (a.Value / b.Value);
        public static bool operator <(UInt32 a, UInt32 b) => a.Value < b.Value;
        public static bool operator >(UInt32 a, UInt32 b) => a.Value > b.Value;
        public static bool operator ==(UInt32 a, UInt32 b) => a.Value == b.Value;
        public static bool operator !=(UInt32 a, UInt32 b) => !(a == b);
        
        
        public static explicit operator UInt32(Half value) => new((uint) value);
        public static explicit operator Half(UInt32 value) => (Half) value.Value;

        public static explicit operator UInt32(float value) => new((uint) value);
        public static implicit operator float(UInt32 value) => (float) value.Value;

        public static explicit operator UInt32(double value) => new((uint) value);
        public static implicit operator double(UInt32 value) => (double) value.Value;

        public static explicit operator UInt32(decimal value) => new((uint) value);
        public static implicit operator decimal(UInt32 value) => (decimal) value.Value;

        public static explicit operator UInt32(sbyte value) => new((uint) value);
        public static explicit operator sbyte(UInt32 value) => (sbyte) value.Value;

        public static explicit operator UInt32(short value) => new((uint) value);
        public static explicit operator short(UInt32 value) => (short) value.Value;

        public static explicit operator UInt32(int value) => new((uint) value);
        public static explicit operator int(UInt32 value) => (int) value.Value;

        public static explicit operator UInt32(long value) => new((uint) value);
        public static implicit operator long(UInt32 value) => (long) value.Value;

        public static explicit operator UInt32(nint value) => new((uint) value);
        public static implicit operator nint(UInt32 value) => (nint) value.Value;

        public static explicit operator UInt32(System.Int128 value) => new((uint) value);
        public static implicit operator System.Int128(UInt32 value) => (System.Int128) value.Value;

        public static implicit operator UInt32(byte value) => new((uint) value);
        public static explicit operator byte(UInt32 value) => (byte) value.Value;

        public static implicit operator UInt32(ushort value) => new((uint) value);
        public static explicit operator ushort(UInt32 value) => (ushort) value.Value;

        public static explicit operator UInt32(ulong value) => new((uint) value);
        public static implicit operator ulong(UInt32 value) => (ulong) value.Value;

        public static explicit operator UInt32(nuint value) => new((uint) value);
        public static implicit operator nuint(UInt32 value) => (nuint) value.Value;

        public static explicit operator UInt32(System.UInt128 value) => new((uint) value);
        public static implicit operator System.UInt128(UInt32 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(UInt32 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UInt32 other && Equals(other);
    }


    public readonly struct UInt64(ulong value) : Unsigned, IEquatable<UInt64> {
        public readonly ulong Value = value;
        
        public static implicit operator UInt64(ulong value) => new(value);
        public static implicit operator ulong(UInt64 value) => value.Value;
        
        public static UInt64 operator +(UInt64 a, UInt64 b) => (ulong) (a.Value + b.Value);
        public static UInt64 operator ++(UInt64 a) => (ulong) (a.Value + (ulong)1);
        public static UInt64 operator --(UInt64 a) => (ulong) (a.Value - (ulong)1);


        public static Int64 operator -(UInt64 a) => -(Int64)(ulong)a.Value;


        public static UInt64 operator +(UInt64 a) => (ulong) (+(ulong)a.Value);
        public static UInt64 operator -(UInt64 a, UInt64 b) => (ulong) (a.Value - b.Value);
        public static UInt64 operator *(UInt64 a, UInt64 b) => (ulong) (a.Value * b.Value);
        public static UInt64 operator /(UInt64 a, UInt64 b) => (ulong) (a.Value / b.Value);
        public static bool operator <(UInt64 a, UInt64 b) => a.Value < b.Value;
        public static bool operator >(UInt64 a, UInt64 b) => a.Value > b.Value;
        public static bool operator ==(UInt64 a, UInt64 b) => a.Value == b.Value;
        public static bool operator !=(UInt64 a, UInt64 b) => !(a == b);
        
        
        public static explicit operator UInt64(Half value) => new((ulong) value);
        public static explicit operator Half(UInt64 value) => (Half) value.Value;

        public static explicit operator UInt64(float value) => new((ulong) value);
        public static implicit operator float(UInt64 value) => (float) value.Value;

        public static explicit operator UInt64(double value) => new((ulong) value);
        public static implicit operator double(UInt64 value) => (double) value.Value;

        public static explicit operator UInt64(decimal value) => new((ulong) value);
        public static implicit operator decimal(UInt64 value) => (decimal) value.Value;

        public static explicit operator UInt64(sbyte value) => new((ulong) value);
        public static explicit operator sbyte(UInt64 value) => (sbyte) value.Value;

        public static explicit operator UInt64(short value) => new((ulong) value);
        public static explicit operator short(UInt64 value) => (short) value.Value;

        public static explicit operator UInt64(int value) => new((ulong) value);
        public static explicit operator int(UInt64 value) => (int) value.Value;

        public static explicit operator UInt64(long value) => new((ulong) value);
        public static explicit operator long(UInt64 value) => (long) value.Value;

        public static explicit operator UInt64(nint value) => new((ulong) value);
        public static explicit operator nint(UInt64 value) => (nint) value.Value;

        public static explicit operator UInt64(System.Int128 value) => new((ulong) value);
        public static implicit operator System.Int128(UInt64 value) => (System.Int128) value.Value;

        public static implicit operator UInt64(byte value) => new((ulong) value);
        public static explicit operator byte(UInt64 value) => (byte) value.Value;

        public static implicit operator UInt64(ushort value) => new((ulong) value);
        public static explicit operator ushort(UInt64 value) => (ushort) value.Value;

        public static implicit operator UInt64(uint value) => new((ulong) value);
        public static explicit operator uint(UInt64 value) => (uint) value.Value;

        public static implicit operator UInt64(nuint value) => new((ulong) value);
        public static implicit operator nuint(UInt64 value) => (nuint) value.Value;

        public static explicit operator UInt64(System.UInt128 value) => new((ulong) value);
        public static implicit operator System.UInt128(UInt64 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(UInt64 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UInt64 other && Equals(other);
    }


    public readonly struct UInt(nuint value) : Unsigned, IEquatable<UInt> {
        public readonly nuint Value = value;
        
        public static implicit operator UInt(nuint value) => new(value);
        public static implicit operator nuint(UInt value) => value.Value;
        
        public static UInt operator +(UInt a, UInt b) => (nuint) (a.Value + b.Value);
        public static UInt operator ++(UInt a) => (nuint) (a.Value + (nuint)1);
        public static UInt operator --(UInt a) => (nuint) (a.Value - (nuint)1);


        public static Int operator -(UInt a) => -(Int)(nuint)a.Value;


        public static UInt operator +(UInt a) => (nuint) (+(nuint)a.Value);
        public static UInt operator -(UInt a, UInt b) => (nuint) (a.Value - b.Value);
        public static UInt operator *(UInt a, UInt b) => (nuint) (a.Value * b.Value);
        public static UInt operator /(UInt a, UInt b) => (nuint) (a.Value / b.Value);
        public static bool operator <(UInt a, UInt b) => a.Value < b.Value;
        public static bool operator >(UInt a, UInt b) => a.Value > b.Value;
        public static bool operator ==(UInt a, UInt b) => a.Value == b.Value;
        public static bool operator !=(UInt a, UInt b) => !(a == b);
        
        
        public static explicit operator UInt(Half value) => new((nuint) value);
        public static explicit operator Half(UInt value) => (Half) value.Value;

        public static explicit operator UInt(float value) => new((nuint) value);
        public static implicit operator float(UInt value) => (float) value.Value;

        public static explicit operator UInt(double value) => new((nuint) value);
        public static implicit operator double(UInt value) => (double) value.Value;

        public static explicit operator UInt(decimal value) => new((nuint) value);
        public static implicit operator decimal(UInt value) => (decimal) value.Value;

        public static explicit operator UInt(sbyte value) => new((nuint) value);
        public static explicit operator sbyte(UInt value) => (sbyte) value.Value;

        public static explicit operator UInt(short value) => new((nuint) value);
        public static explicit operator short(UInt value) => (short) value.Value;

        public static explicit operator UInt(int value) => new((nuint) value);
        public static explicit operator int(UInt value) => (int) value.Value;

        public static explicit operator UInt(long value) => new((nuint) value);
        public static explicit operator long(UInt value) => (long) value.Value;

        public static explicit operator UInt(nint value) => new((nuint) value);
        public static explicit operator nint(UInt value) => (nint) value.Value;

        public static explicit operator UInt(System.Int128 value) => new((nuint) value);
        public static implicit operator System.Int128(UInt value) => (System.Int128) value.Value;

        public static implicit operator UInt(byte value) => new((nuint) value);
        public static explicit operator byte(UInt value) => (byte) value.Value;

        public static implicit operator UInt(ushort value) => new((nuint) value);
        public static explicit operator ushort(UInt value) => (ushort) value.Value;

        public static implicit operator UInt(uint value) => new((nuint) value);
        public static explicit operator uint(UInt value) => (uint) value.Value;

        public static implicit operator UInt(ulong value) => new((nuint) value);
        public static implicit operator ulong(UInt value) => (ulong) value.Value;

        public static explicit operator UInt(System.UInt128 value) => new((nuint) value);
        public static implicit operator System.UInt128(UInt value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(UInt other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UInt other && Equals(other);
    }


    public readonly struct UInt128(System.UInt128 value) : Unsigned, IEquatable<UInt128> {
        public readonly System.UInt128 Value = value;
        
        public static implicit operator UInt128(System.UInt128 value) => new(value);
        public static implicit operator System.UInt128(UInt128 value) => value.Value;
        
        public static UInt128 operator +(UInt128 a, UInt128 b) => (System.UInt128) (a.Value + b.Value);
        public static UInt128 operator ++(UInt128 a) => (System.UInt128) (a.Value + (System.UInt128)1);
        public static UInt128 operator --(UInt128 a) => (System.UInt128) (a.Value - (System.UInt128)1);


        public static Int128 operator -(UInt128 a) => -(Int128)(System.UInt128)a.Value;


        public static UInt128 operator +(UInt128 a) => (System.UInt128) (+(System.UInt128)a.Value);
        public static UInt128 operator -(UInt128 a, UInt128 b) => (System.UInt128) (a.Value - b.Value);
        public static UInt128 operator *(UInt128 a, UInt128 b) => (System.UInt128) (a.Value * b.Value);
        public static UInt128 operator /(UInt128 a, UInt128 b) => (System.UInt128) (a.Value / b.Value);
        public static bool operator <(UInt128 a, UInt128 b) => a.Value < b.Value;
        public static bool operator >(UInt128 a, UInt128 b) => a.Value > b.Value;
        public static bool operator ==(UInt128 a, UInt128 b) => a.Value == b.Value;
        public static bool operator !=(UInt128 a, UInt128 b) => !(a == b);
        
        
        public static explicit operator UInt128(Half value) => new((System.UInt128) value);
        public static explicit operator Half(UInt128 value) => (Half) value.Value;

        public static explicit operator UInt128(float value) => new((System.UInt128) value);
        public static implicit operator float(UInt128 value) => (float) value.Value;

        public static explicit operator UInt128(double value) => new((System.UInt128) value);
        public static implicit operator double(UInt128 value) => (double) value.Value;

        public static explicit operator UInt128(decimal value) => new((System.UInt128) value);
        public static explicit operator decimal(UInt128 value) => (decimal) value.Value;

        public static explicit operator UInt128(sbyte value) => new((System.UInt128) value);
        public static explicit operator sbyte(UInt128 value) => (sbyte) value.Value;

        public static explicit operator UInt128(short value) => new((System.UInt128) value);
        public static explicit operator short(UInt128 value) => (short) value.Value;

        public static explicit operator UInt128(int value) => new((System.UInt128) value);
        public static explicit operator int(UInt128 value) => (int) value.Value;

        public static explicit operator UInt128(long value) => new((System.UInt128) value);
        public static explicit operator long(UInt128 value) => (long) value.Value;

        public static explicit operator UInt128(nint value) => new((System.UInt128) value);
        public static explicit operator nint(UInt128 value) => (nint) value.Value;

        public static explicit operator UInt128(System.Int128 value) => new((System.UInt128) value);
        public static explicit operator System.Int128(UInt128 value) => (System.Int128) value.Value;

        public static implicit operator UInt128(byte value) => new((System.UInt128) value);
        public static explicit operator byte(UInt128 value) => (byte) value.Value;

        public static implicit operator UInt128(ushort value) => new((System.UInt128) value);
        public static explicit operator ushort(UInt128 value) => (ushort) value.Value;

        public static implicit operator UInt128(uint value) => new((System.UInt128) value);
        public static explicit operator uint(UInt128 value) => (uint) value.Value;

        public static implicit operator UInt128(ulong value) => new((System.UInt128) value);
        public static explicit operator ulong(UInt128 value) => (ulong) value.Value;

        public static implicit operator UInt128(nuint value) => new((System.UInt128) value);
        public static explicit operator nuint(UInt128 value) => (nuint) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(UInt128 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UInt128 other && Equals(other);
    }


    public readonly struct Float16(Half value) : AbstractFloat, IEquatable<Float16> {
        public readonly Half Value = value;
        
        public static implicit operator Float16(Half value) => new(value);
        public static implicit operator Half(Float16 value) => value.Value;
        
        public static Float16 operator +(Float16 a, Float16 b) => (Half) (a.Value + b.Value);
        public static Float16 operator ++(Float16 a) => (Half) (a.Value + (Half)1);
        public static Float16 operator --(Float16 a) => (Half) (a.Value - (Half)1);


        public static Float16 operator -(Float16 a) => (Half) (-(Half)a.Value);


        public static Float16 operator +(Float16 a) => (Half) (+(Half)a.Value);
        public static Float16 operator -(Float16 a, Float16 b) => (Half) (a.Value - b.Value);
        public static Float16 operator *(Float16 a, Float16 b) => (Half) (a.Value * b.Value);
        public static Float16 operator /(Float16 a, Float16 b) => (Half) (a.Value / b.Value);
        public static bool operator <(Float16 a, Float16 b) => a.Value < b.Value;
        public static bool operator >(Float16 a, Float16 b) => a.Value > b.Value;
        public static bool operator ==(Float16 a, Float16 b) => a.Value == b.Value;
        public static bool operator !=(Float16 a, Float16 b) => !(a == b);
        
        
        public static explicit operator Float16(float value) => new((Half) value);
        public static implicit operator float(Float16 value) => (float) value.Value;

        public static explicit operator Float16(double value) => new((Half) value);
        public static implicit operator double(Float16 value) => (double) value.Value;

        public static explicit operator Float16(decimal value) => new((Half) value);
        public static implicit operator decimal(Float16 value) => (decimal) value.Value;

        public static implicit operator Float16(sbyte value) => new((Half) value);
        public static explicit operator sbyte(Float16 value) => (sbyte) value.Value;

        public static implicit operator Float16(short value) => new((Half) value);
        public static explicit operator short(Float16 value) => (short) value.Value;

        public static explicit operator Float16(int value) => new((Half) value);
        public static implicit operator int(Float16 value) => (int) value.Value;

        public static explicit operator Float16(long value) => new((Half) value);
        public static implicit operator long(Float16 value) => (long) value.Value;

        public static explicit operator Float16(nint value) => new((Half) value);
        public static implicit operator nint(Float16 value) => (nint) value.Value;

        public static explicit operator Float16(System.Int128 value) => new((Half) value);
        public static implicit operator System.Int128(Float16 value) => (System.Int128) value.Value;

        public static implicit operator Float16(byte value) => new((Half) value);
        public static explicit operator byte(Float16 value) => (byte) value.Value;

        public static implicit operator Float16(ushort value) => new((Half) value);
        public static explicit operator ushort(Float16 value) => (ushort) value.Value;

        public static explicit operator Float16(uint value) => new((Half) value);
        public static explicit operator uint(Float16 value) => (uint) value.Value;

        public static explicit operator Float16(ulong value) => new((Half) value);
        public static explicit operator ulong(Float16 value) => (ulong) value.Value;

        public static explicit operator Float16(nuint value) => new((Half) value);
        public static explicit operator nuint(Float16 value) => (nuint) value.Value;

        public static explicit operator Float16(System.UInt128 value) => new((Half) value);
        public static explicit operator System.UInt128(Float16 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Float16 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Float16 other && Equals(other);
    }


    public readonly struct Float32(float value) : AbstractFloat, IEquatable<Float32> {
        public readonly float Value = value;
        
        public static implicit operator Float32(float value) => new(value);
        public static implicit operator float(Float32 value) => value.Value;
        
        public static Float32 operator +(Float32 a, Float32 b) => (float) (a.Value + b.Value);
        public static Float32 operator ++(Float32 a) => (float) (a.Value + (float)1);
        public static Float32 operator --(Float32 a) => (float) (a.Value - (float)1);


        public static Float32 operator -(Float32 a) => (float) (-(float)a.Value);


        public static Float32 operator +(Float32 a) => (float) (+(float)a.Value);
        public static Float32 operator -(Float32 a, Float32 b) => (float) (a.Value - b.Value);
        public static Float32 operator *(Float32 a, Float32 b) => (float) (a.Value * b.Value);
        public static Float32 operator /(Float32 a, Float32 b) => (float) (a.Value / b.Value);
        public static bool operator <(Float32 a, Float32 b) => a.Value < b.Value;
        public static bool operator >(Float32 a, Float32 b) => a.Value > b.Value;
        public static bool operator ==(Float32 a, Float32 b) => a.Value == b.Value;
        public static bool operator !=(Float32 a, Float32 b) => !(a == b);
        
        
        public static implicit operator Float32(Half value) => new((float) value);
        public static explicit operator Half(Float32 value) => (Half) value.Value;

        public static explicit operator Float32(double value) => new((float) value);
        public static implicit operator double(Float32 value) => (double) value.Value;

        public static implicit operator Float32(decimal value) => new((float) value);
        public static explicit operator decimal(Float32 value) => (decimal) value.Value;

        public static implicit operator Float32(sbyte value) => new((float) value);
        public static explicit operator sbyte(Float32 value) => (sbyte) value.Value;

        public static implicit operator Float32(short value) => new((float) value);
        public static explicit operator short(Float32 value) => (short) value.Value;

        public static implicit operator Float32(int value) => new((float) value);
        public static explicit operator int(Float32 value) => (int) value.Value;

        public static implicit operator Float32(long value) => new((float) value);
        public static explicit operator long(Float32 value) => (long) value.Value;

        public static implicit operator Float32(nint value) => new((float) value);
        public static explicit operator nint(Float32 value) => (nint) value.Value;

        public static implicit operator Float32(System.Int128 value) => new((float) value);
        public static explicit operator System.Int128(Float32 value) => (System.Int128) value.Value;

        public static implicit operator Float32(byte value) => new((float) value);
        public static explicit operator byte(Float32 value) => (byte) value.Value;

        public static implicit operator Float32(ushort value) => new((float) value);
        public static explicit operator ushort(Float32 value) => (ushort) value.Value;

        public static implicit operator Float32(uint value) => new((float) value);
        public static explicit operator uint(Float32 value) => (uint) value.Value;

        public static implicit operator Float32(ulong value) => new((float) value);
        public static explicit operator ulong(Float32 value) => (ulong) value.Value;

        public static implicit operator Float32(nuint value) => new((float) value);
        public static explicit operator nuint(Float32 value) => (nuint) value.Value;

        public static implicit operator Float32(System.UInt128 value) => new((float) value);
        public static explicit operator System.UInt128(Float32 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Float32 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Float32 other && Equals(other);
    }


    public readonly struct Float64(double value) : AbstractFloat, IEquatable<Float64> {
        public readonly double Value = value;
        
        public static implicit operator Float64(double value) => new(value);
        public static implicit operator double(Float64 value) => value.Value;
        
        public static Float64 operator +(Float64 a, Float64 b) => (double) (a.Value + b.Value);
        public static Float64 operator ++(Float64 a) => (double) (a.Value + (double)1);
        public static Float64 operator --(Float64 a) => (double) (a.Value - (double)1);


        public static Float64 operator -(Float64 a) => (double) (-(double)a.Value);


        public static Float64 operator +(Float64 a) => (double) (+(double)a.Value);
        public static Float64 operator -(Float64 a, Float64 b) => (double) (a.Value - b.Value);
        public static Float64 operator *(Float64 a, Float64 b) => (double) (a.Value * b.Value);
        public static Float64 operator /(Float64 a, Float64 b) => (double) (a.Value / b.Value);
        public static bool operator <(Float64 a, Float64 b) => a.Value < b.Value;
        public static bool operator >(Float64 a, Float64 b) => a.Value > b.Value;
        public static bool operator ==(Float64 a, Float64 b) => a.Value == b.Value;
        public static bool operator !=(Float64 a, Float64 b) => !(a == b);
        
        
        public static implicit operator Float64(Half value) => new((double) value);
        public static explicit operator Half(Float64 value) => (Half) value.Value;

        public static implicit operator Float64(float value) => new((double) value);
        public static explicit operator float(Float64 value) => (float) value.Value;

        public static implicit operator Float64(decimal value) => new((double) value);
        public static explicit operator decimal(Float64 value) => (decimal) value.Value;

        public static implicit operator Float64(sbyte value) => new((double) value);
        public static explicit operator sbyte(Float64 value) => (sbyte) value.Value;

        public static implicit operator Float64(short value) => new((double) value);
        public static explicit operator short(Float64 value) => (short) value.Value;

        public static implicit operator Float64(int value) => new((double) value);
        public static explicit operator int(Float64 value) => (int) value.Value;

        public static implicit operator Float64(long value) => new((double) value);
        public static explicit operator long(Float64 value) => (long) value.Value;

        public static implicit operator Float64(nint value) => new((double) value);
        public static explicit operator nint(Float64 value) => (nint) value.Value;

        public static implicit operator Float64(System.Int128 value) => new((double) value);
        public static explicit operator System.Int128(Float64 value) => (System.Int128) value.Value;

        public static implicit operator Float64(byte value) => new((double) value);
        public static explicit operator byte(Float64 value) => (byte) value.Value;

        public static implicit operator Float64(ushort value) => new((double) value);
        public static explicit operator ushort(Float64 value) => (ushort) value.Value;

        public static implicit operator Float64(uint value) => new((double) value);
        public static explicit operator uint(Float64 value) => (uint) value.Value;

        public static implicit operator Float64(ulong value) => new((double) value);
        public static explicit operator ulong(Float64 value) => (ulong) value.Value;

        public static implicit operator Float64(nuint value) => new((double) value);
        public static explicit operator nuint(Float64 value) => (nuint) value.Value;

        public static implicit operator Float64(System.UInt128 value) => new((double) value);
        public static explicit operator System.UInt128(Float64 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Float64 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Float64 other && Equals(other);
    }


    public readonly struct Float128(decimal value) : AbstractFloat, IEquatable<Float128> {
        public readonly decimal Value = value;
        
        public static implicit operator Float128(decimal value) => new(value);
        public static implicit operator decimal(Float128 value) => value.Value;
        
        public static Float128 operator +(Float128 a, Float128 b) => (decimal) (a.Value + b.Value);
        public static Float128 operator ++(Float128 a) => (decimal) (a.Value + (decimal)1);
        public static Float128 operator --(Float128 a) => (decimal) (a.Value - (decimal)1);


        public static Float128 operator -(Float128 a) => (decimal) (-(decimal)a.Value);


        public static Float128 operator +(Float128 a) => (decimal) (+(decimal)a.Value);
        public static Float128 operator -(Float128 a, Float128 b) => (decimal) (a.Value - b.Value);
        public static Float128 operator *(Float128 a, Float128 b) => (decimal) (a.Value * b.Value);
        public static Float128 operator /(Float128 a, Float128 b) => (decimal) (a.Value / b.Value);
        public static bool operator <(Float128 a, Float128 b) => a.Value < b.Value;
        public static bool operator >(Float128 a, Float128 b) => a.Value > b.Value;
        public static bool operator ==(Float128 a, Float128 b) => a.Value == b.Value;
        public static bool operator !=(Float128 a, Float128 b) => !(a == b);
        
        
        public static implicit operator Float128(Half value) => new((decimal) value);
        public static explicit operator Half(Float128 value) => (Half) value.Value;

        public static explicit operator Float128(float value) => new((decimal) value);
        public static implicit operator float(Float128 value) => (float) value.Value;

        public static explicit operator Float128(double value) => new((decimal) value);
        public static implicit operator double(Float128 value) => (double) value.Value;

        public static implicit operator Float128(sbyte value) => new((decimal) value);
        public static explicit operator sbyte(Float128 value) => (sbyte) value.Value;

        public static implicit operator Float128(short value) => new((decimal) value);
        public static explicit operator short(Float128 value) => (short) value.Value;

        public static implicit operator Float128(int value) => new((decimal) value);
        public static explicit operator int(Float128 value) => (int) value.Value;

        public static implicit operator Float128(long value) => new((decimal) value);
        public static explicit operator long(Float128 value) => (long) value.Value;

        public static implicit operator Float128(nint value) => new((decimal) value);
        public static explicit operator nint(Float128 value) => (nint) value.Value;

        public static explicit operator Float128(System.Int128 value) => new((decimal) value);
        public static implicit operator System.Int128(Float128 value) => (System.Int128) value.Value;

        public static implicit operator Float128(byte value) => new((decimal) value);
        public static explicit operator byte(Float128 value) => (byte) value.Value;

        public static implicit operator Float128(ushort value) => new((decimal) value);
        public static explicit operator ushort(Float128 value) => (ushort) value.Value;

        public static implicit operator Float128(uint value) => new((decimal) value);
        public static explicit operator uint(Float128 value) => (uint) value.Value;

        public static implicit operator Float128(ulong value) => new((decimal) value);
        public static explicit operator ulong(Float128 value) => (ulong) value.Value;

        public static implicit operator Float128(nuint value) => new((decimal) value);
        public static explicit operator nuint(Float128 value) => (nuint) value.Value;

        public static explicit operator Float128(System.UInt128 value) => new((decimal) value);
        public static explicit operator System.UInt128(Float128 value) => (System.UInt128) value.Value;

        
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
        public bool Equals(Float128 other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is Float128 other && Equals(other);
    }

}