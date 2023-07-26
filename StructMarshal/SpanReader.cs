using System.Runtime.CompilerServices;
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

    public static SpanReader CreateReader<T>(Span<T> span)
        where T : unmanaged
    {
        var dest = Cast<T, byte>(span);
        return new SpanReader(dest);
    }
    public static SpanReader CreateReader<T>(T[] span)
        where T : unmanaged
    {
        var dest = Cast<T, byte>(span);
        return new SpanReader(dest);
    }
    public SpanReader(Span<byte> span)
    {
        _span = span;
        _position = 0;
    }
    [MethodImpl(AggressiveInlining)]
    public void Read<T>(T[] values)
        where T : unmanaged
    {
        var size = SizeOf<T>();
        var dest = _span.Slice(_position, size * values.Length);
        dest.CopyTo(Cast<T,byte>(values));
        _position += size * values.Length;
    }
    [MethodImpl(AggressiveInlining)]
    public void Read<T>(Memory<T> values)
        where T : unmanaged
    {
        var size = SizeOf<T>();
        var dest = _span.Slice(_position, size * values.Length);
        dest.CopyTo(Cast<T,byte>(values.Span));
        _position += size * values.Length;
    }
    [MethodImpl(AggressiveInlining)]
    public void Read<T>(Span<T> values)
        where T : unmanaged
    {
        var size = SizeOf<T>();
        var dest = _span.Slice(_position, size * values.Length);
        dest.CopyTo(Cast<T,byte>(values));
        _position += size * values.Length;
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
    public T Read<T>()
        where T : unmanaged
    {
        var size = SizeOf<T>();
        var dest = _span.Slice(_position, size);
        _position += size;
        return MemoryMarshal.Read<T>(dest);
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
        [MethodImpl(AggressiveInlining)] get => _position;
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