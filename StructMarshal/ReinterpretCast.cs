using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static System.Runtime.InteropServices.MemoryMarshal;

namespace StructMarshal;

using static Unsafe;
using static ReinterpretCast;

[PublicAPI]
public static class ReinterpretCast<T> where T : unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<TStruct>(scoped in TStruct structure)
        where TStruct : unmanaged
    {
        return ref As<TStruct, T>(ref AsRef(structure));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> GetSpan<TStruct>(scoped in TStruct structure)
        where TStruct : unmanaged
    {
        return AsSpan<TStruct, T>(ref AsRef(structure));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> GetSpan<TStruct>(Memory<TStruct> memory)
        where TStruct : unmanaged
    {
        return MemoryMarshal.Cast<TStruct, T>(memory.Span);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> GetSpan<TStruct>(Span<TStruct> structure)
        where TStruct : unmanaged
    {
        return MemoryMarshal.Cast<TStruct, T>(structure);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> GetReadOnlySpan<TStruct>(scoped in TStruct structure)
        where TStruct : unmanaged
    {
        return AsSpan<TStruct, T>(ref AsRef(structure));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<TStruct>(Span<TStruct> structure)
        where TStruct : unmanaged
    {
        return ref Cast<TStruct, T>(structure);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<TStruct>(Memory<TStruct> memory)
        where TStruct : unmanaged
    {
        return ref Cast<TStruct, T>(memory.Span);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<TStruct>(TStruct[] array)
        where TStruct : unmanaged
    {
        return ref Cast<TStruct, T>(array.AsSpan());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<TStruct>(ReadOnlyMemory<TStruct> memory)
        where TStruct : unmanaged
    {
        return ref Cast<TStruct, T>(AsMemory(memory).Span);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRef<TStruct>(ReadOnlySpan<TStruct> @struct)
        where TStruct : unmanaged
    {
        return ref GetReference(MemoryMarshal.Cast<TStruct, T>(@struct));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T* Pointer<TStruct>(in TStruct @struct)
        where TStruct : unmanaged
    {
        return (T*)AsPointer(ref AsRef(@struct));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T* Pointer<TStruct>(Span<TStruct> span)
        where TStruct : unmanaged
    {
        return (T*)AsPointer(ref Cast<TStruct, T>(span));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T* Pointer<TStruct>(ReadOnlySpan<TStruct> span)
        where TStruct : unmanaged
    {
        return (T*)AsPointer(ref GetReference(MemoryMarshal.Cast<TStruct, T>(span)));
    }
}