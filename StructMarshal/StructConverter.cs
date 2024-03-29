﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace StructMarshal;

[PublicAPI]
public static class ReinterpretCast
{
    /// <summary>
    /// Reinterprets the given Struct as a Span of bytes.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Span<byte> AsBytes<TStruct>(scoped ref TStruct value)
        where TStruct : struct
    {
        ref var bytes = ref Unsafe.As<TStruct, byte>(ref value);
        unsafe {
            byte* ptr    = (byte*)Unsafe.AsPointer(ref bytes);
            int   lenght = Unsafe.SizeOf<TStruct>();
            return new Span<byte>(ptr, lenght);
        }
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
        var bytes = AsBytes(ref value);

        if (MemoryMarshal.TryRead<TTo>(bytes, out var ret)) {
            return ret;
        }

        Span<byte> newSpan = stackalloc byte[Unsafe.SizeOf<TTo>()];
        bytes.CopyTo(newSpan);
        return MemoryMarshal.Read<TTo>(newSpan);
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
        var bytes = MemoryMarshal.AsBytes(span);

        if (MemoryMarshal.TryRead<TTo>(bytes, out var ret)) {
            return ret;
        }

        Span<byte> newSpan = stackalloc byte[Unsafe.SizeOf<TTo>()];
        bytes.CopyTo(newSpan);
        return MemoryMarshal.Read<TTo>(newSpan);
    }

    /// <summary>
    /// <code>
    /// Reinterpret Cast of a struct to another struct,
    /// The size of the final struct must be lesser or equal to that of the initial one
    /// </code>
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="reference"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public static ref TTo Cast<TFrom, TTo>(ref TFrom reference)
        where TFrom : struct
        where TTo : struct
    {
        if (Unsafe.SizeOf<TTo>() > Unsafe.SizeOf<TFrom>())
            throw new InvalidCastException("Cannot cast to a larger struct");

        return ref Unsafe.As<TFrom, TTo>(ref reference);
    }

    /// <summary>
    /// <code>
    /// Reinterpret Cast of a span of structs to a struct,
    /// The size of the final struct must be lesser or equal to that of the initial span
    /// </code>
    /// </summary>
    /// <typeparam name="TTo"></typeparam>
    /// <typeparam name="TFrom"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"></exception>
    public static ref TTo Cast<TFrom, TTo>(Span<TFrom> span)
        where TFrom : struct
        where TTo : struct
    {
        if (Unsafe.SizeOf<TTo>() > span.Length * Unsafe.SizeOf<TFrom>())
            throw new InvalidCastException("Cannot cast to a larger struct");

        var      bytes = MemoryMarshal.AsBytes(span);
        ref byte first = ref MemoryMarshal.GetReference(bytes);
        return ref Unsafe.As<byte, TTo>(ref first);
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
        return MemoryMarshal.Cast<byte, TTo>(AsBytes(ref value));
    }

    public static unsafe Span<TTo> AsSpan<TStruct, TTo>(void* value)
        where TStruct : struct
        where TTo : struct
    {
        int size = Unsafe.SizeOf<TStruct>();
        var span = new Span<byte>(value, size);
        return MemoryMarshal.Cast<byte, TTo>(span);
    }

    /// <summary>
    /// Reinterpret Cast of a struct to a span of another struct
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static Span<TTo> AsSpan<TStruct, TTo>(ref TStruct value, int length)
        where TStruct : struct
        where TTo : struct
    {
        return AsSpan<TStruct, TTo>(ref value).Slice(0, length);
    }

    /// <summary>
    /// Reinterpret Cast of a struct to a span of another struct
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    /// <param name="value"></param>
    /// <param name="start"></param>
    /// <param name="lenght"></param>
    /// <returns></returns>
    public static Span<TTo> AsSpan<TStruct, TTo>(ref TStruct value, int start, int lenght)
        where TStruct : struct
        where TTo : struct
    {
        return AsSpan<TStruct, TTo>(ref value).Slice(start, lenght);
    }

    public static unsafe TTo* AsPointer<TFrom, TTo>(ref TFrom from)
        where TTo : unmanaged
        where TFrom : struct
    {
        if (Unsafe.SizeOf<TTo>() > Unsafe.SizeOf<TFrom>()) throw new InvalidCastException();

        return (TTo*)Unsafe.AsPointer(ref from);
    }
}