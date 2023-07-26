using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace StructMarshal;
public ref struct SpanWriter
{
    readonly Span<byte> _span;
    int                 _position;

    public SpanWriter(Span<byte> span)
    {
        _span = span;
        _position = 0;
    }
    
    public static SpanWriter CreateWriter<T>(Span<T> span)
        where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        var dest = MemoryMarshal.Cast<T, byte>(span);
        return new SpanWriter(dest);
    }
    public static SpanWriter CreateWriter<T>(T[] span)
        where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        var dest = MemoryMarshal.Cast<T, byte>(span);
        return new SpanWriter(dest);
    }
    
    [MethodImpl(AggressiveInlining)]
    public void Write<T>(T value)
        where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        var dest = _span.Slice(_position, size);
        StructMarshal.Bytes(ref value).CopyTo(dest);
        _position += size;
    }
    [MethodImpl(AggressiveInlining)]
    public void Write<T>(T[] values)
        where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        var dest = _span.Slice(_position, size * values.Length);
        StructMarshal.Bytes(values).CopyTo(dest);
        _position += size * values.Length;
    }
    [MethodImpl(AggressiveInlining)]
    public void Write<T>(Span<T> values)
        where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        var dest = _span.Slice(_position, size * values.Length);
        StructMarshal.Bytes(values).CopyTo(dest);
        _position += size * values.Length;
    }
    [MethodImpl(AggressiveInlining)]
    public void Write<T>(Memory<T> values)
        where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        var dest = _span.Slice(_position, size * values.Length);
        StructMarshal.Bytes(values.Span).CopyTo(dest);
        _position += size * values.Length;
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