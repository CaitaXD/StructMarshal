using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Struct;
/// <summary>
/// Checks for equality in the bitwise representation
/// </summary>
public interface IByteEquatable
{
    static virtual bool BiwiseEquality<TStruct,TTo>(ref TStruct left, ref TTo right, IEqualityComparer<byte>? comparer = null)
        where TTo : struct
        where TStruct : struct
    {
        return StructMarshal.AsBytes(ref left).SequenceEqual(StructMarshal.AsBytes(ref right), comparer);
    }
    static virtual bool BiwiseEquality<TStruct,TTo>(ref TStruct left, Span<TTo> right, IEqualityComparer<byte>? comparer = null)
        where TTo : struct
        where TStruct : struct
    {
        return StructMarshal.AsBytes(ref left).SequenceEqual(MemoryMarshal.Cast<TTo, byte>(right));
    }
    static virtual bool BiwiseEquality<TStruct,TTo>(Span<TStruct> left, ref TTo right, IEqualityComparer<byte>? comparer = null)
        where TTo : struct
        where TStruct : struct
    {
        return MemoryMarshal.Cast<TStruct, byte>(left).SequenceEqual(StructMarshal.AsBytes(ref right), comparer);
    }
    static bool BiwiseEquality<TStruct,TTo>(ref TStruct left, ref TTo right)
        where TTo : struct
        where TStruct : struct, IByteEquatable
    {
        return TStruct.BiwiseEquality(ref left, ref right, null);
    }
    static bool BiwiseEquality<TStruct,TTo>(ref TStruct left, Span<TTo> right)
        where TTo : struct
        where TStruct : struct, IByteEquatable
    {
        return TStruct.BiwiseEquality(ref left, right, null);
    }
    static bool BiwiseEquality<TStruct,TTo>(Span<TStruct> left, ref TTo right)
        where TTo : struct
        where TStruct : struct, IByteEquatable
    {
        return TStruct.BiwiseEquality(left, ref right, null);
    }
}
public static class ByteEquatable
{
    public static bool BiwiseEquality<TStruct, TTo>(ref this TStruct left, ref TTo right, IEqualityComparer<byte>? comparer = null)
    where TStruct : struct, IByteEquatable
    where TTo : struct
    {
        return TStruct.BiwiseEquality(ref left, ref right, comparer);
    }
    public static bool BiwiseEquality<TStruct, TTo>(this Span<TStruct> left, ref TTo right, IEqualityComparer<byte>? comparer = null)
        where TStruct : struct, IByteEquatable
        where TTo : struct
    {
        return TStruct.BiwiseEquality(left, ref right, comparer);
    }
    public static bool BiwiseEquality<TStruct, TTo>(this ref TStruct left, Span<TTo> right, IEqualityComparer<byte>? comparer = null)
        where TStruct : struct, IByteEquatable
        where TTo : struct
    {
        return TStruct.BiwiseEquality(ref left, right, comparer);
    }
    public static bool BiwiseEquality<TStruct,TTo>(ref this TStruct left, ref TTo right)
        where TStruct : struct, IByteEquatable
        where TTo : struct
    {
        return TStruct.BiwiseEquality(ref left, ref right);
    }
    public static bool BiwiseEquality<TStruct,TTo>(this Span<TStruct> left, ref TTo right)
        where TStruct : struct, IByteEquatable
        where TTo : struct
    {
        return TStruct.BiwiseEquality(left, ref right);
    }
    public static bool BiwiseEquality<TStruct,TTo>(this ref TStruct left, Span<TTo> right)
        where TStruct : struct, IByteEquatable
        where TTo : struct
    {
        return TStruct.BiwiseEquality(ref left, right);
    }
}