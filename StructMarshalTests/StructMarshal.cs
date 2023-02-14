using Newtonsoft.Json.Linq;
using Struct;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace StructMarshalTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Read_Small_Struct_To_Larger_One()
        {
            Vector2 v2 = new(1, 2);

            Vector4 v4 = StructMarshal.Read<Vector2, Vector4>(ref v2);

            Assert.False(Unsafe.AreSame(ref Unsafe.As<Vector2, Vector4>(ref v2), ref v4));
            Assert.That(v4, Is.EqualTo(new Vector4(1, 2, 0, 0)));

            Span<float> floats = stackalloc float[] { 1, 2 };
            v4 = StructMarshal.Read<float, Vector4>(floats);

            Assert.False(Unsafe.AreSame(ref Unsafe.As<float, Vector4>(ref MemoryMarshal.GetReference(floats)), ref v4));
            Assert.That(v4, Is.EqualTo(new Vector4(1, 2, 0, 0)));
        }
        [Test]
        public void Read_Large_Struct_To_Smaller_One()
        {
            Vector4 v4 = new(1, 2, 3, 4);

            Vector2 v2 = StructMarshal.Read<Vector4, Vector2>(ref v4);

            Assert.False(Unsafe.AreSame(ref Unsafe.As<Vector4, Vector2>(ref v4), ref v2));
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));

            Span<float> floats = stackalloc float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            v2 = StructMarshal.Read<float, Vector2>(floats);

            Assert.False(Unsafe.AreSame(ref Unsafe.As<float, Vector2>(ref MemoryMarshal.GetReference(floats)), ref v2));
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));
        }
        [Test]
        public void Cast_Large_Struct_To_Smaller_One()
        {
            Vector4 v4 = new(1, 2, 3, 4);

            ref Vector2 v2 = ref StructMarshal.Cast<Vector4, Vector2>(ref v4);

            Assert.True(Unsafe.AreSame(ref Unsafe.As<Vector4, Vector2>(ref v4), ref v2));
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));

            Span<float> floats = stackalloc float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            v2 = ref StructMarshal.Cast<float, Vector2>(floats);

            Assert.True(Unsafe.AreSame(ref Unsafe.As<float, Vector2>(ref MemoryMarshal.GetReference(floats)), ref v2));
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));
        }
        [Test]
        public void Cast_Small_Struct_To_Larger_One()
        {
            Vector2 v2 = new(1, 2);

            Vector4 v4() => StructMarshal.Cast<Vector2, Vector4>(ref v2);

            Assert.Throws<InvalidCastException>(() => v4());

            Span<float> floats = stackalloc float[] { 1, 2 };

            try
            {
                StructMarshal.Cast<float, Vector4>(floats);
            }
            catch (InvalidCastException)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }
        [Test]
        public void Castable_Struct_Interface()
        {
            var ppm_header = new PPM_Header(1, 2, 3);
            var bytes = ppm_header.AsBytes();
            Assert.True(ppm_header.BiwiseEquality(bytes));
            Assert.That(bytes.ToArray(), Has.Length.EqualTo(Unsafe.SizeOf<PPM_Header>()));
        }
        [Test]
        [Obsolete]
        public void Serialization()
        {
            var foo = new Foo()
            {
                a = 1,
                Bar = "Hi!!!"
            };
            var bytes = foo.Serialize();
            var foo_back = bytes.Deserialize<Foo>()!;

            Assert.True(foo_back.a == foo.a && foo_back.Bar == foo.Bar);

            Assert.Throws<InvalidCastException>(() => bytes.Deserialize<Oof>());
        }
        public struct PPM_Header : IByteCastable, IByteEquatable
        {
            public int Width;
            public int Height;
            public int MaxValue;
            public PPM_Header(int width, int height, int maxValue)
            {
                Width = width;
                Height = height;
                MaxValue = maxValue;
            }
        }
        [Serializable]
        public class Foo : IByteCastable

        {
            public int a;
            public string? Bar;
        }
        [Serializable]
        public class Oof : IByteCastable
        {
            public int a;
            public char[]? Baar1;
            public long off;
        }
    }
}