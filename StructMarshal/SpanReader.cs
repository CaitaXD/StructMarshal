﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace StructMarshal;
using static Unsafe;
using static MemoryMarshal;

public ref struct SpanReader 
{
    readonly Span<byte> _span;
    int                 _position;

    public SpanReader(Span<byte> span)
    {
        _span = span;
        _position = 0;
    }
    [MethodImpl(AggressiveInlining)]
    public T Read<T>()
        where T : unmanaged
    {
        var size = SizeOf<T>();
        var dest = _span.Slice(_position, size);
        _position += size;
        return MemoryMarshal.Read<T>(dest);
    }
    [MethodImpl(AggressiveInlining)]
    public Span<T> Read<T>(int count)
        where T : unmanaged
    {
        var size = SizeOf<T>();
        var dest = _span.Slice(_position, size * count);
        _position += size * count;
        return Cast<byte, T>(dest);
    }
    [MethodImpl(AggressiveInlining)]
    public void Seek(SeekOrigin origin, int offset)
    {
        if ((uint)_position > _span.Length) {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, null);
        }
        switch (origin) {
        case SeekOrigin.Begin:
            _position = offset;
            break;
        case SeekOrigin.Current:
            _position += offset;
            break;
        case SeekOrigin.End:
            _position = _span.Length - offset;
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }
    }
    public int Position
    {
        [MethodImpl(AggressiveInlining)]
        get => _position;
        [MethodImpl(AggressiveInlining)]
        set
        {
            if ((uint)value > _span.Length) {
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            _position = value;
        }
    }
}