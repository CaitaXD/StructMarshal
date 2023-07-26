using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace StructMarshal;
using static Unsafe;
using static MemoryMarshal;
using static StructMarshal;
using static MethodImplOptions;

[PublicAPI]
public static class StructMarshal
{
    [MethodImpl(AggressiveInlining)]
    public static Span<byte> Bytes<TStruct>(ref TStruct value)
        where TStruct : unmanaged
    {
        var span = CreateSpan(ref value, 1);
        return Cast<TStruct, byte>(span);
    }    
    [MethodImpl(AggressiveInlining)]
    public static Span<byte> Bytes<TStruct>(Span<TStruct> span)
        where TStruct : unmanaged
    {
        return Cast<TStruct, byte>(span);
    }
    [MethodImpl(AggressiveInlining)]
    public static Span<byte> Bytes<TStruct>(Memory<TStruct> memory)
        where TStruct : unmanaged
    {
        return Cast<TStruct, byte>(memory.Span);
    }
    [MethodImpl(AggressiveInlining)]
    public static Span<byte> Bytes<TStruct>(TStruct[] span)
        where TStruct : unmanaged
    {
        return Cast<TStruct, byte>(span);
    }
    [MethodImpl(AggressiveInlining)]
    public static Span<TTo> Span<TFrom, TTo>(ref TFrom value)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        var span = CreateSpan(ref value, 1);
        return Cast<TFrom, TTo>(span);
    }
    [MethodImpl(AggressiveInlining)]
    public static ref TTo Reinterpret<TFrom, TTo>(ref TFrom reference)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        var fromSize = SizeOf<TFrom>();
        var toSize   = SizeOf<TTo>();

        if (toSize > fromSize) {
            throw new InvalidCastException($"Attempted to cast {fromSize} bytes to {toSize}-byte struct");
        }

        return ref As<TFrom, TTo>(ref reference);
    }
    [MethodImpl(AggressiveInlining)]
    public static ref TTo Reinterpret<TFrom, TTo>(Span<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        var fromSize = SizeOf<TFrom>();
        var toSize   = SizeOf<TTo>();

        if (fromSize > span.Length * toSize) {
            throw new InvalidCastException($"Attempted to cast {span.Length * fromSize}-byte struct to {toSize}-byte struct");
        }

        var bytes = AsBytes(span);
        return ref AsRef<TTo>(bytes);
    }
}
[PublicAPI]
public static class StructMarshal<TTo> where TTo : unmanaged
{
    [MethodImpl(AggressiveInlining)]
    public static ref TTo Ref<TFrom>(scoped in TFrom st) =>
        ref As<TFrom, TTo>(ref AsRef(st));

    [MethodImpl(AggressiveInlining)]
    public static ref TTo Ref<TStruct>(Span<TStruct> st) where TStruct : unmanaged =>
        ref Reinterpret<TStruct, TTo>(st);

    [MethodImpl(AggressiveInlining)]
    public static ref TTo Ref<TStruct>(ReadOnlySpan<TStruct> st) where TStruct : unmanaged =>
        ref GetReference(Cast<TStruct, TTo>(st));

    [MethodImpl(AggressiveInlining)]
    public static ref TTo Ref<TStruct>(Memory<TStruct> st) where TStruct : unmanaged =>
        ref Reinterpret<TStruct, TTo>(st.Span);

    [MethodImpl(AggressiveInlining)]
    public static ref TTo Ref<TStruct>(ReadOnlyMemory<TStruct> st) where TStruct : unmanaged =>
        ref GetReference(Cast<TStruct, TTo>(st.Span));

    [MethodImpl(AggressiveInlining)]
    public static Span<TTo> Span<TStruct>(scoped in TStruct st) where TStruct : unmanaged =>
        Span<TStruct, TTo>(ref AsRef(st));
}