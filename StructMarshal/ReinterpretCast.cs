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
    public static unsafe T BitCast<TStruct>(TStruct structure)
    where TStruct : unmanaged
    {
        #if !NET8_0_OR_GREATER
        return *Pointer(structure);
        #else
        return BitCast<TStruct,T>(structure);
        #endif
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ref T GetRef<TStruct>(
        #if !NET8_0_OR_GREATER
        scoped in TStruct structure
        #else
        scoped ref readonly TStruct structure
        #endif
    )
    where TStruct : unmanaged
    {
        #if !NET8_0_OR_GREATER
        return ref *Pointer(structure);
        #else
        return ref *Pointer(in structure);
        #endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> GetSpan<TStruct>(
        #if !NET8_0_OR_GREATER
        scoped in TStruct structure
        #else
        scoped ref readonly TStruct structure
        #endif
    )
    where TStruct : unmanaged
    {
        return AsSpan<TStruct, T>(ref *(TStruct*)Pointer(in structure));
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
    public static ReadOnlySpan<T> GetReadOnlySpan<TStruct>
    (
        #if !NET8_0_OR_GREATER
        in TStruct structure
        #else
        ref readonly TStruct structure
    #endif
    )
    where TStruct : unmanaged
    {
        return AsSpan<TStruct, T>(ref AsRef(in structure));
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
    public static unsafe T* Pointer<TStruct>(
        #if !NET8_0_OR_GREATER
        in TStruct @struct
        #else
        ref readonly TStruct @struct
    #endif
    )
    where TStruct : unmanaged
    {
        return (T*)AsPointer(ref AsRef(in @struct));
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