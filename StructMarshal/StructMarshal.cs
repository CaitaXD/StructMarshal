using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Struct;
public static class StructMarshal
{
    /// <summary>
    /// Reinterprets the given Struct as a Span of bytes.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<byte> AsBytes<TStruct>(ref TStruct value)
        where TStruct : struct
    {
        var span = MemoryMarshal.CreateSpan(ref value, 1);
        return MemoryMarshal.Cast<TStruct, byte>(span);
    }
    /// <summary>
    /// <code>
    /// Copies a struct reinterpreted as another
    /// If the final struct is larger than the initial one, the remaining bytes are set to zero.
    /// </code>
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TTo Read<TStruct, TTo>(ref TStruct value)
    where TStruct : struct
    where TTo : struct
    {
        if (Unsafe.SizeOf<TStruct>() >= Unsafe.SizeOf<TTo>())
        {
            return Unsafe.As<TStruct, TTo>(ref value);
        }
        Span<byte> new_block = stackalloc byte[Unsafe.SizeOf<TTo>()];
        var as_span = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
        var old_block = MemoryMarshal.AsBytes(as_span);
        old_block.CopyTo(new_block);
        return MemoryMarshal.Read<TTo>(new_block);
    }
    /// <summary>
    /// <code>
    /// Copies a span of structs reinterpreted as another struct
    /// If the final struct is larger than the size of the initial span, the remaining bytes are set to zero.
    /// </code>
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="span"></param>
    /// <returns></returns>
    public static TTo Read<TStruct, TTo>(Span<TStruct> span)
    where TStruct : struct
    where TTo : struct
    {
        if (span.Length * Unsafe.SizeOf<TStruct>() >= Unsafe.SizeOf<TTo>())
        {
            ref var value = ref MemoryMarshal.GetReference(span);
            return Unsafe.As<TStruct, TTo>(ref value);
        }
        Span<byte> new_block = stackalloc byte[Unsafe.SizeOf<TTo>()];

        ref var src = ref MemoryMarshal.GetReference(MemoryMarshal.Cast<TStruct, byte>(span));
        ref var dst = ref MemoryMarshal.GetReference(new_block);

        for (int i = 0; i < Unsafe.SizeOf<TTo>(); ++i)
            Unsafe.Add(ref dst, i) = Unsafe.Add(ref src, i);

        return MemoryMarshal.Read<TTo>(new_block);
    }
    /// <summary>
    /// <code>
    /// Reinterpret Cast of a struct to another struct,
    /// The size of the final struct must be lesser or equal to that of the initial one
    /// </code>
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public static ref TTo Cast<TStruct, TTo>(ref TStruct value)
    where TStruct : struct
    where TTo : struct
    {
        if (Unsafe.SizeOf<TStruct>() >= Unsafe.SizeOf<TTo>())
            return ref Unsafe.As<TStruct, TTo>(ref value);

        throw new InvalidCastException("Cannot cast to a larger struct");
    }
    /// <summary>
    /// <code>
    /// Reinterpret Cast of a span of structs to a struct,
    /// The size of the final struct must be lesser or equal to that of the initial span
    /// </code>
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public static ref TTo Cast<TFrom, TTo>(Span<TFrom> span)
    where TFrom : struct
    where TTo : struct
    {
        if (span.Length * Unsafe.SizeOf<TFrom>() >= Unsafe.SizeOf<TTo>())
        {
            ref var value = ref MemoryMarshal.GetReference(span);
            return ref Unsafe.As<TFrom, TTo>(ref value);
        }
        throw new InvalidCastException("Casting to a larger struct, would cause an out of bounds access");
    }
    /// <summary>
    /// Reinterpret Cast of a struct to a span of another struct
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<TTo> AsSpan<TStruct, TTo>(ref TStruct value)
    where TStruct : struct
    where TTo : struct
    {
        var span = MemoryMarshal.CreateSpan(ref value, 1);
        return MemoryMarshal.Cast<TStruct, TTo>(span);
    }
    /// <summary>
    /// Reinterpret Cast of a struct to a span of another struct
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<TTo> AsSpan<TStruct, TTo>(ref TStruct value, int length)
    where TStruct : struct
    where TTo : struct
    {
        var span = MemoryMarshal.CreateSpan(ref value, 1);
        return MemoryMarshal.Cast<TStruct, TTo>(span).Slice(0, length);
    }
    /// <summary>
    /// Reinterpret Cast of a struct to a span of another struct
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<TTo> AsSpan<TStruct, TTo>(ref TStruct value, int start, int lenght)
    where TStruct : struct
    where TTo : struct
    {
        var span = MemoryMarshal.CreateSpan(ref value, 1);
        return MemoryMarshal.Cast<TStruct, TTo>(span).Slice(start, lenght);
    }
}


