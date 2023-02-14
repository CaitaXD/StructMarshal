using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Struct;
/// <summary>
/// Indicates that the implenting type can be viewed in byte representation
/// </summary>
/// <typeparam name="TStruct"></typeparam>
public interface IByteCastable
{
    public static virtual Span<byte> AsBytes<TStruct>(ref TStruct value)
        where TStruct : struct
    {
        return StructMarshal.AsBytes(ref value);
    }
    public static virtual ref TStruct AsStruct<TStruct>(Span<byte> bytes)
        where TStruct : struct
    {
        return ref MemoryMarshal.GetReference(MemoryMarshal.Cast<byte, TStruct>(bytes));
    }
    [Obsolete("Do not use this testing only", true)]
    static virtual TObject? Deserialize<TObject>(Memory<byte> bytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(bytes.Span);
        memStream.Seek(0, SeekOrigin.Begin);
        TObject? obj = (TObject)binForm.Deserialize(memStream);
        return obj;
    }
    [Obsolete("Do not use this testing only", true)]
    static virtual Memory<byte> Serialize<TObject>(TObject value)
    {
        if (value is null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, value);
        return ms.ToArray();
    }
}
public static class ByteCastable
{
    public static Span<byte> AsBytes<TStruct>(ref this TStruct byteSerializable)
        where TStruct : struct, IByteCastable
    {
        return TStruct.AsBytes(ref byteSerializable);
    }
    public static ref TStruct AsStruct<TStruct>(this Span<byte> bytes)
        where TStruct : struct, IByteCastable
    {
        return ref TStruct.AsStruct<TStruct>(bytes);
    }
    [Obsolete("Do not use this testing only", true)]
    public static Memory<byte> Serialize<TObject>(this TObject value)
        where TObject : class, IByteCastable
    {
        return TObject.Serialize(value);
    }
    [Obsolete("Do not use this testing only", true)]
    public static TObject? Deserialize<TObject>(this Memory<byte> memory)
        where TObject : class, IByteCastable
    {
        return TObject.Deserialize<TObject>(memory);
    }
}